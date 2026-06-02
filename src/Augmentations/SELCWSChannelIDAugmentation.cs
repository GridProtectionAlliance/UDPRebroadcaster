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
//       Switched to in-place Span<byte> mutation; original channel ID snapshotted in BeginPacket.
//
//******************************************************************************************************

using System.Buffers.Binary;
using System.Net;

namespace UDPRebroadcaster.Augmentations;

/// <summary>
/// Augmentation that rewrites the <c>ulong</c> Channel ID in each SEL CWS frame to a unique value
/// per destination.
/// </summary>
/// <remarks>
/// <para>
/// Lets multiple copies of one upstream SEL CWS stream be sent to the same downstream receiver
/// (e.g., the SEL CWS Receiver in the SEL SynchroWave Rebroadcaster, which listens on a single
/// port) while keeping each copy distinguishable. The replacement channel ID for destination
/// index <c>N</c> is <c>originalChannelID + (N + 1)</c> — deterministic, preserves a relation
/// to the original ID, and never collides with the original or other destinations.
/// </para>
/// <para>
/// Implementation note: <see cref="BeginPacket"/> snapshots the original channel ID once per
/// packet, then <see cref="TransformForDestination"/> overwrites bytes [6..14) in the buffer in
/// place. Zero heap allocations on the hot path — the fan-out loop reuses the receive buffer
/// across all destinations.
/// </para>
/// <para>
/// SEL CWS frame layout (all multibyte fields are big-endian):
/// <code>
/// Offset  Size  Field
///   0      1   Frame type (0x00 = configuration, 0x01 = data)
///   1      1   Version (0x01)
///   2      4   Frame length minus 6
///   6      8   Channel ID (ulong)        &lt;-- rewritten here
///  14      2   Packet count
///  16+     ..  Payload (no checksum, no footer)
/// </code>
/// Both configuration and data frames carry the channel ID at offset 6, so a single rewrite
/// path covers both. SEL CWS has no checksum, so the byte swap requires no recompute.
/// </para>
/// </remarks>
[Label("SEL CWS unique per end-point channel ID")]
internal sealed class SELCWSChannelIDAugmentation : IRebroadcastAugmentation
{
    private const int FrameTypeOffset = 0;
    private const int VersionOffset = 1;
    private const int ChannelIDOffset = 6;
    private const int ChannelIDSize = 8;
    private const int MinimumFrameLength = 16;
    private const byte SELCWSVersion = 0x01;
    private const byte ConfigurationFrameType = 0x00;
    private const byte DataFrameType = 0x01;

    private ulong m_originalChannelID;
    private bool m_isSELCWSFrame;

    public void Initialize(IReadOnlyList<IPEndPoint> destinations)
    {
        // Stateless w.r.t. destinations — per-packet state is captured in BeginPacket.
    }

    public void BeginPacket(ReadOnlySpan<byte> source)
    {
        m_isSELCWSFrame = IsSELCWSFrame(source);
        
        m_originalChannelID = m_isSELCWSFrame ? 
            BinaryPrimitives.ReadUInt64BigEndian(source.Slice(ChannelIDOffset, ChannelIDSize)) : 
            0UL;
    }

    public void TransformForDestination(Span<byte> buffer, int destinationIndex)
    {
        // If the packet didn't look like SEL CWS at BeginPacket time, pass it through unchanged
        // rather than dropping it — keeps the user's chosen destinations reachable even if a
        // stray packet sneaks in from another source.
        if (!m_isSELCWSFrame)
            return;

        ulong uniqueChannelID = unchecked(m_originalChannelID + (ulong)(destinationIndex + 1));
        BinaryPrimitives.WriteUInt64BigEndian(buffer.Slice(ChannelIDOffset, ChannelIDSize), uniqueChannelID);
    }

    private static bool IsSELCWSFrame(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < MinimumFrameLength)
            return false;

        if (buffer[VersionOffset] != SELCWSVersion)
            return false;

        byte frameType = buffer[FrameTypeOffset];
        return frameType is ConfigurationFrameType or DataFrameType;
    }
}
