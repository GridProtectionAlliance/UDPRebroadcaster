//******************************************************************************************************
//  IRebroadcastAugmentation.cs - Gbtc
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
//       Switched to in-place Span<byte> mutation; added BeginPacket for per-packet state.
//  06/02/2026 - J. Ritchie Carroll
//       TransformForDestination now returns ReadOnlySpan<byte> so an augmentation can change
//       the per-destination payload length (e.g., SEL CWS variable-length ChannelName rewrite).
//
//******************************************************************************************************

using System.Net;

namespace UDPRebroadcaster.Augmentations;

/// <summary>
/// Defines a protocol- or content-aware transformation applied to outgoing UDP packets on a
/// per-destination basis as part of the rebroadcast pipeline.
/// </summary>
/// <remarks>
/// <para>
/// Implementations are discovered by reflection. Each concrete implementation should be decorated
/// with a <see cref="LabelAttribute"/> providing the user-facing drop-down label.
/// </para>
/// <para>
/// Per-packet lifecycle: <see cref="BeginPacket"/> is called exactly once with the original
/// packet bytes, followed by <see cref="TransformForDestination"/> called once for each
/// destination index in ascending order (0..N-1). The same buffer is passed by reference each
/// iteration — implementations mutate it in place to produce per-destination payloads without
/// allocating. <see cref="BeginPacket"/> is the implementation's opportunity to snapshot any
/// state (e.g., the original channel ID) it needs to derive per-destination values, since
/// subsequent <see cref="TransformForDestination"/> calls will have overwritten the buffer.
/// </para>
/// <para>
/// Threading: instances are NOT required to be thread-safe. The receive loop guarantees that
/// <see cref="BeginPacket"/> / <see cref="TransformForDestination"/> calls for a given instance
/// are serialized.
/// </para>
/// </remarks>
internal interface IRebroadcastAugmentation
{
    /// <summary>
    /// Called once when the listener starts. Receives the resolved destination endpoints in the
    /// same index order they will be referenced by <see cref="TransformForDestination"/>.
    /// </summary>
    void Initialize(IReadOnlyList<IPEndPoint> destinations);

    /// <summary>
    /// Called once at the start of each packet's fan-out, with the original packet bytes.
    /// Implementations should snapshot anything they need from the original bytes before
    /// <see cref="TransformForDestination"/> begins overwriting them.
    /// </summary>
    /// <param name="source">The original packet bytes. MUST NOT be mutated.</param>
    void BeginPacket(ReadOnlySpan<byte> source);

    /// <summary>
    /// Mutates <paramref name="buffer"/> in place (or, if the per-destination payload must grow,
    /// fills an augmentation-owned scratch buffer) and returns the exact byte slice to send to
    /// the destination at <paramref name="destinationIndex"/>. Called once per destination per
    /// packet, with destination indices in ascending order (0..N-1).
    /// </summary>
    /// <param name="buffer">Working buffer. On entry holds whatever the previous destination's
    /// call left behind (or the original packet bytes on the first call). Augmentations that
    /// don't change packet length should mutate it in place and return it; those that shrink
    /// the packet should mutate it in place and return a prefix slice; those that may grow it
    /// past <c>buffer.Length</c> must back the rewrite with an internal scratch buffer and
    /// return a slice into that.</param>
    /// <param name="destinationIndex">Zero-based index of the destination to produce bytes for.</param>
    /// <returns>The bytes to send. Valid only until the next call into the augmentation —
    /// callers must consume immediately (a synchronous <c>Send</c> is the expected pattern).</returns>
    ReadOnlySpan<byte> TransformForDestination(Span<byte> buffer, int destinationIndex);
}
