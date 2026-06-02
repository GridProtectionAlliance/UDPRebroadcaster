//******************************************************************************************************
//  NoAugmentation.cs - Gbtc
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
//
//******************************************************************************************************

using System.Net;

namespace UDPRebroadcaster.Augmentations;

/// <summary>
/// Default pass-through augmentation: rebroadcasts every received packet to every destination
/// byte-for-byte. All hot-path methods are true no-ops — zero work, zero allocations.
/// </summary>
[Label("No augmentation")]
internal sealed class NoAugmentation : IRebroadcastAugmentation
{
    public void Initialize(IReadOnlyList<IPEndPoint> destinations)
    {
    }

    public void BeginPacket(ReadOnlySpan<byte> source)
    {
    }

    public void TransformForDestination(Span<byte> buffer, int destinationIndex)
    {
    }
}
