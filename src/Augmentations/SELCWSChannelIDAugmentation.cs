//******************************************************************************************************
//  SELCWSChannelIDAugmentation.cs - Gbtc
//
//  Copyright © 2026, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  06/02/2026 - J. Ritchie Carroll
//       Generated original version of source code.
//  06/02/2026 - J. Ritchie Carroll
//       Switched to in-place Span<byte> mutation; original channel ID snap-shotted in BeginPacket.
//  06/02/2026 - J. Ritchie Carroll
//       Added user-tunable settings (Incremented/Random channel ID, per-destination station
//       labels written into configuration frames) backed by AppSettings.SELCWSChannelID.
//  06/02/2026 - J. Ritchie Carroll
//       Reworked ChannelName rewrite to match the SEL-735 CWS spec — null-terminated UTF-8 at
//       offset (24 + 4*NumAnalogs), variable length, with Size header updated and SignalNames
//       shifted into place. Per-destination output length now varies; an internal scratch
//       buffer covers the (rare) case where a new label is longer than the original.
//
//******************************************************************************************************

using System.Buffers.Binary;
using System.Net;
using System.Text;

namespace UDPRebroadcaster.Augmentations;

/// <summary>
/// SEL CWS frame types enumeration.
/// </summary>
public enum FrameType : byte
{
    /// <summary>
    /// Data frame.
    /// </summary>
    DataFrame = 0x00,
    /// <summary>
    /// Configuration frame.
    /// </summary>
    ConfigurationFrame = 0x01
}

/// <summary>
/// Augmentation that rewrites the <c>Uint64</c> ChannelID in each SEL CWS frame to a unique value
/// per destination, and (for configuration frames) overwrites the variable-length null-terminated
/// UTF-8 <c>ChannelName</c> field with a user-supplied per-destination label.
/// </summary>
/// <remarks>
/// <para>
/// Lets multiple copies of one upstream SEL CWS stream be sent to the same downstream receiver
/// (e.g., the SEL CWS Receiver in the SEL SynchroWave Rebroadcaster, which listens on a single
/// port) while keeping each copy distinguishable.
/// </para>
/// <para>
/// Per-destination ChannelID is derived from <see cref="ChannelIDGenerationMode"/>:
/// <list type="bullet">
/// <item><see cref="ChannelIDGenerationMode.Incremented"/> — destination <c>N</c> receives
/// <c>originalChannelID + (N + 1)</c>; deterministic, preserves a relation to the original.</item>
/// <item><see cref="ChannelIDGenerationMode.Random"/> — destination <c>N</c> receives a random
/// <c>Uint64</c> generated once per listening session in <see cref="Initialize"/>.</item>
/// </list>
/// </para>
/// <para>
/// SEL CWS configuration frame layout (multibyte fields are big-endian, per SEL-735 IM, Appendix
/// J, Table J.3):
/// <code>
/// Offset                  Size  Field
///   0                      2   FrameID (type + version; low byte is version)
///   2                      4   Size = bytes that follow, not including FrameID and Size
///   6                      8   ChannelID                          &lt;-- rewritten here
///  14                      2   PktCount
///  16                      2   NumAnalogs (N)
///  18                      2   NumDigitals
///  20                      4   SampleRate
///  24                    4*N   Scalars (Float32 array)
///  24+4*N              2..21   ChannelName (UTF-8, null-terminated) &lt;-- rewritten here
///  after ChannelName  variable SignalNames (N packed null-terminated UTF-8 names)
/// </code>
/// SignalNames is described as "21 bytes" in Table J.3 but is in fact a sequence of N
/// null-terminated UTF-8 names back-to-back, each variable-length — the "21" is just the typical
/// total for the SEL-735's six standard waveform names. The rewrite path makes no assumption
/// about this section's internal structure; it snapshots whatever bytes follow the original
/// ChannelName and lays them down verbatim at the new offset.
/// </para>
/// <para>
/// Data frames carry the same FrameID/Size/ChannelID/PktCount header but a fixed 1218-byte
/// payload (Timestamp + SampleSets) and no ChannelName. The augmentation recognizes them by the
/// FrameID's type byte (<see cref="FrameType.DataFrame"/> = 0x00, <see cref="FrameType.ConfigurationFrame"/>
/// = 0x01) and rewrites only the ChannelID.
/// </para>
/// <para>
/// Implementation note: <see cref="BeginPacket"/> snapshots the original ChannelID, the original
/// ChannelName length, the Size header value, and the variable-length tail (SignalNames) bytes
/// once per packet. <see cref="TransformForDestination"/> then rebuilds the per-destination
/// output by writing the new ChannelID, the new ChannelName + null, the snap-shotted tail at its
/// new offset, and the adjusted Size header. The original buffer is the target when the new
/// packet fits; an augmentation-owned scratch buffer takes over only if a label happens to
/// encode to more UTF-8 bytes than the original ChannelName — a corner case for typical SEL
/// TIDs and per-destination labels.
/// </para>
/// </remarks>
[Label("SEL CWS unique per end-point channel ID")]
[SettingsForm(typeof(SELCWSChannelIDSettingsForm))]
internal sealed class SELCWSChannelIDAugmentation : IRebroadcastAugmentation, IConfigurableAugmentation
{
    // Fixed-offset fields shared by configuration and data frames.
    private const int FrameTypeOffset = 0;
    private const int VersionOffset = 1;
    private const int SizeFieldOffset = 2;
    private const int SizeFieldSize = 4;
    private const int ChannelIDOffset = 6;
    private const int ChannelIDSize = 8;
    private const int NumAnalogsOffset = 16;

    // Minimum buffer length to peek FrameID, Size, ChannelID, PktCount, NumAnalogs.
    private const int MinimumFrameLength = 18;

    // Bytes [0..24) cover everything up to (but not including) the Scalars array. ChannelName
    // begins at FixedHeaderSize + 4 * NumAnalogs.
    private const int FixedHeaderSize = 24;

    // ChannelName per spec is 2..21 bytes including the null terminator, so the content (excluding
    // the null) is bounded at 20 UTF-8 bytes.
    private const int MaxChannelNameBytes = 21;
    private const int MaxChannelNameContentBytes = MaxChannelNameBytes - 1;

    // FrameID is a UInt16 at offset 0: high byte is the type designator (see FrameType enum —
    // 0x00 = data, 0x01 = configuration), low byte is the protocol version. Recognition is then
    // (version == 0x01) AND (type is a known FrameType value).
    private const byte SELCWSVersion = 0x01;

    // Scratch buffers pre-allocated in Initialize so the hot path stays allocation-free.
    // m_workBuffer covers the rare grow-the-frame case; m_tailBuffer holds the snap-shotted
    // SignalNames bytes between BeginPacket and the per-destination TransformForDestination
    // calls. 1024 / 256 are generous given the spec ceiling on these fields.
    private const int WorkBufferSize = 1024;
    private const int TailBufferSize = 256;

    private SELCWSChannelIDSettings m_settings = new();

    // Per-destination contribution to the rewritten channel ID, populated once at Initialize for
    // both modes:
    //   Incremented — m_channelIDs[i] = (ulong)(i + 1); TransformForDestination adds the
    //                 per-packet original.
    //   Random      — m_channelIDs[i] = a fresh random ulong; TransformForDestination writes it
    //                 directly with no per-packet input.
    private ulong[] m_channelIDs = [];

    // Per-destination UTF-8 encoded label bytes (no null terminator — that's appended at write
    // time). Encoded once at Initialize and capped at MaxChannelNameContentBytes so the on-wire
    // ChannelName never exceeds the spec maximum.
    private byte[][] m_encodedLabels = [];

    // Pre-allocated scratch buffers, see WorkBufferSize / TailBufferSize comments above.
    private byte[] m_workBuffer = [];
    private byte[] m_tailBuffer = [];

    // Per-packet snapshot captured in BeginPacket and consumed by every TransformForDestination
    // call in the same fan-out.
    private ulong m_originalChannelID;
    private uint m_originalSize;
    private int m_channelNameOffset;
    private int m_originalChannelNameLength; // includes null terminator
    private int m_tailLength;
    private bool m_isSELCWSFrame;
    private bool m_canRewriteChannelName;

    public void ApplySettings(AppSettings settings)
    {
        m_settings = settings.SELCWSChannelID;
    }

    public void SynchronizeDefaults(AppSettings settings, int destinationCount)
    {
        // Pad up to destinationCount; preserve any existing entries (including those past the
        // current count, so shrink-then-grow doesn't lose user-supplied labels).
        List<string> labels = settings.SELCWSChannelID.StationLabels;

        for (int i = labels.Count; i < destinationCount; i++)
            labels.Add(SELCWSChannelIDSettings.DefaultStationLabel(i));
    }

    public void Initialize(IReadOnlyList<IPEndPoint> destinations)
    {
        // Pre-encode each destination's label as UTF-8 bytes, capped at the spec content
        // maximum. Truncation iteratively drops the trailing .NET char until the encoded byte
        // count fits — guarantees no half-encoded multibyte sequence at the end.
        m_encodedLabels = new byte[destinations.Count][];

        for (int i = 0; i < destinations.Count; i++)
        {
            string label = i < m_settings.StationLabels.Count ? 
                m_settings.StationLabels[i] : 
                SELCWSChannelIDSettings.DefaultStationLabel(i);

            m_encodedLabels[i] = EncodeChannelName(label);
        }

        // Pre-compute each destination's channel-ID contribution once per listening session.
        // Incremented mode stores offsets; Random mode stores absolute fresh ulong's (regenerated
        // on the next Start). See m_channelIDs's declaration comment for hot-path semantics.
        m_channelIDs = new ulong[destinations.Count];

        if (m_settings.GenerationMode == ChannelIDGenerationMode.Incremented)
        {
            for (int i = 0; i < destinations.Count; i++)
                m_channelIDs[i] = (ulong)(i + 1);
        }
        else
        {
            for (int i = 0; i < destinations.Count; i++)
                m_channelIDs[i] = unchecked((ulong)Random.Shared.NextInt64());
        }

        // Pre-allocate the scratch buffers once. The work buffer only sees use on the rare
        // (label-longer-than-original) growth path; the tail buffer is touched once per
        // configuration frame.
        m_workBuffer = new byte[WorkBufferSize];
        m_tailBuffer = new byte[TailBufferSize];
    }

    public void BeginPacket(ReadOnlySpan<byte> source)
    {
        m_isSELCWSFrame = IsSELCWSFrame(source);
        m_canRewriteChannelName = false;
        m_originalChannelID = 0UL;
        m_originalSize = 0;
        m_channelNameOffset = 0;
        m_originalChannelNameLength = 0;
        m_tailLength = 0;

        if (!m_isSELCWSFrame)
            return;

        m_originalChannelID = BinaryPrimitives.ReadUInt64BigEndian(source.Slice(ChannelIDOffset, ChannelIDSize));

        // Data frames stop here — only ChannelID gets rewritten downstream. The FrameType byte
        // at offset 0 is the authoritative discriminator per the FrameType enum.
        if ((FrameType)source[FrameTypeOffset] != FrameType.ConfigurationFrame)
            return;

        // Read NumAnalogs to locate the dynamic ChannelName offset.
        ushort numAnalogs = BinaryPrimitives.ReadUInt16BigEndian(source.Slice(NumAnalogsOffset, 2));
        int channelNameOffset = FixedHeaderSize + 4 * numAnalogs;

        // Bail (leave canRewriteChannelName false → only the ChannelID will be rewritten) if the
        // frame is too short to contain Scalars plus at least an empty null-terminated
        // ChannelName.
        if (channelNameOffset + 1 > source.Length)
            return;

        // Scan for the ChannelName's null terminator within the spec's 21-byte cap.
        int scanLimit = Math.Min(channelNameOffset + MaxChannelNameBytes, source.Length);
        int nullPos = channelNameOffset;

        while (nullPos < scanLimit && source[nullPos] != 0)
            nullPos++;

        // No null within the spec ceiling — treat as malformed; pass through with ChannelID
        // rewrite only.
        if (nullPos >= scanLimit)
            return;

        int channelNameLength = nullPos - channelNameOffset + 1; // includes the null
        int tailOffset = channelNameOffset + channelNameLength;
        int tailLength = source.Length - tailOffset;

        // Defensive: if the post-ChannelName remainder doesn't fit in our snapshot buffer (it
        // should, comfortably — spec has it at ~21 bytes for the SEL-735's six analogs), skip
        // the ChannelName rewrite rather than truncate downstream content.
        if (tailLength < 0 || tailLength > m_tailBuffer.Length)
            return;

        m_originalSize = BinaryPrimitives.ReadUInt32BigEndian(source.Slice(SizeFieldOffset, SizeFieldSize));
        m_channelNameOffset = channelNameOffset;
        m_originalChannelNameLength = channelNameLength;
        m_tailLength = tailLength;

        // Snapshot the tail (SignalNames + anything after) so per-destination rewrites can
        // restore it at the new position — previous destinations' rewrites may have moved it.
        source.Slice(tailOffset, tailLength).CopyTo(m_tailBuffer);

        m_canRewriteChannelName = true;
    }

    public ReadOnlySpan<byte> TransformForDestination(Span<byte> buffer, int destinationIndex)
    {
        // Non-SEL-CWS packets pass through unchanged rather than being dropped — keeps the
        // user's chosen destinations reachable even if a stray packet sneaks in from another
        // source.
        if (!m_isSELCWSFrame)
            return buffer;

        // ChannelID rewrite applies to both configuration and data frames.
        ulong uniqueChannelID = m_settings.GenerationMode == ChannelIDGenerationMode.Random ? 
            m_channelIDs[destinationIndex] : 
            unchecked(m_originalChannelID + m_channelIDs[destinationIndex]);

        BinaryPrimitives.WriteUInt64BigEndian(buffer.Slice(ChannelIDOffset, ChannelIDSize), uniqueChannelID);

        // Data frames (and malformed config frames flagged in BeginPacket) stop here — buffer
        // length is unchanged.
        if (!m_canRewriteChannelName)
            return buffer;

        byte[] label = m_encodedLabels[destinationIndex];
        int newChannelNameLength = label.Length + 1; // +1 for the null terminator
        int newEffectiveLength = m_channelNameOffset + newChannelNameLength + m_tailLength;

        // Pick in-place vs. scratch buffer. In-place works whenever the new packet fits in the
        // original receive buffer's byte[]; the scratch path covers the (rare) case where the
        // user-supplied label is encoded into more bytes than the device's original TID.
        bool inPlace = newEffectiveLength <= buffer.Length;
        Span<byte> target;

        if (inPlace)
        {
            target = buffer;
        }
        else
        {
            // Defensive: if even the scratch buffer can't hold this, fall back to ChannelID-only
            // rewrite rather than truncate or crash. The packet still goes out, just without
            // the per-destination label.
            if (newEffectiveLength > m_workBuffer.Length)
                return buffer;

            target = m_workBuffer.AsSpan();

            // Carry the rewritten header (incl. new ChannelID and the original Scalars) into
            // the scratch buffer so the only remaining writes are Size, ChannelName, and tail.
            buffer[..m_channelNameOffset].CopyTo(target);
        }

        // Update the Size header for the new total length. Size is "bytes that follow", so the
        // delta is just (L_new − L_orig) of the ChannelName.
        uint newSize = (uint)(m_originalSize + newChannelNameLength - m_originalChannelNameLength);
        BinaryPrimitives.WriteUInt32BigEndian(target.Slice(SizeFieldOffset, SizeFieldSize), newSize);

        // Write the new ChannelName (UTF-8) followed by the null terminator. Span.CopyTo handles
        // overlapping ranges, and the ChannelName region doesn't overlap with the tail region
        // we're about to write next.
        label.AsSpan().CopyTo(target.Slice(m_channelNameOffset, label.Length));
        target[m_channelNameOffset + label.Length] = 0;

        // Restore the tail at its new offset from the BeginPacket snapshot — NOT from buffer,
        // since previous destinations may have shifted SignalNames around to make room for
        // their own labels.
        m_tailBuffer.AsSpan(0, m_tailLength).CopyTo(target.Slice(m_channelNameOffset + newChannelNameLength, m_tailLength));

        return target[..newEffectiveLength];
    }

    private static byte[] EncodeChannelName(string label)
    {
        if (string.IsNullOrEmpty(label))
            return [];

        // Truncate the .NET string from the right until UTF-8 encoding fits within the spec's
        // content cap. Dropping whole chars at a time guarantees we never split a multibyte
        // sequence — the common ASCII case usually hits in one iteration.
        int charCount = label.Length;
        int byteCount = Encoding.UTF8.GetByteCount(label);

        while (byteCount > MaxChannelNameContentBytes && charCount > 0)
        {
            charCount--;
            byteCount = Encoding.UTF8.GetByteCount(label.AsSpan(0, charCount));
        }

        return charCount == label.Length ? 
            Encoding.UTF8.GetBytes(label) : 
            Encoding.UTF8.GetBytes(label[..charCount]);
    }

    private static bool IsSELCWSFrame(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < MinimumFrameLength || buffer[VersionOffset] != SELCWSVersion)
            return false;

        // FrameType byte must be one of the known enum values; anything else with a v1
        // version byte is some other protocol that just happens to share that byte.
        FrameType frameType = (FrameType)buffer[FrameTypeOffset];
        return frameType is FrameType.DataFrame or FrameType.ConfigurationFrame;
    }
}
