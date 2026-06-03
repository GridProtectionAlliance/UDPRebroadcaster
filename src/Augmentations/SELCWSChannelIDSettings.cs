//******************************************************************************************************
//  SELCWSChannelIDSettings.cs - Gbtc
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
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  06/02/2026 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

namespace UDPRebroadcaster.Augmentations;

/// <summary>
/// Selects how <see cref="SELCWSChannelIDAugmentation"/> derives the per-destination unique
/// channel ID from the original.
/// </summary>
internal enum ChannelIDGenerationMode
{
    /// <summary>
    /// Destination <c>N</c> receives <c>originalChannelID + (N + 1)</c> — deterministic, preserves
    /// a relation to the original ID, never collides with the original or other destinations.
    /// </summary>
    Incremented = 0,

    /// <summary>
    /// Destination <c>N</c> receives a random <c>ulong</c> generated once per listening session
    /// (in <c>Initialize</c>). Useful when the original ID's neighborhood is reserved or noisy
    /// and the receiver only needs to disambiguate streams from each other.
    /// </summary>
    Random = 1,
}

/// <summary>
/// User-tunable settings for <see cref="SELCWSChannelIDAugmentation"/>, persisted as the
/// <c>SELCWSChannelID</c> section of <see cref="AppSettings"/>.
/// </summary>
internal sealed class SELCWSChannelIDSettings
{
    /// <summary>
    /// Gets or sets the strategy used to derive each destination's unique channel ID.
    /// </summary>
    public ChannelIDGenerationMode GenerationMode { get; set; } = ChannelIDGenerationMode.Incremented;

    /// <summary>
    /// Gets or sets the per-destination station labels written into outgoing configuration frames.
    /// Index <c>N</c> applies to destination <c>N</c>; entries past the configured destination
    /// count are ignored, and destinations past the end of this list fall back to the default
    /// <c>STATION_A</c>, <c>STATION_B</c>, … sequence.
    /// </summary>
    public List<string> StationLabels { get; set; } = [];

    /// <summary>
    /// Returns the conventional default label for destination index <paramref name="index"/>:
    /// <c>STATION_A</c>, <c>STATION_B</c>, …, <c>STATION_Z</c>, then <c>STATION_AA</c>,
    /// <c>STATION_AB</c>, … for indices ≥ 26.
    /// </summary>
    public static string DefaultStationLabel(int index)
    {
        Span<char> suffix = stackalloc char[8];
        int length = 0;
        int n = index;

        do
        {
            suffix[length++] = (char)('A' + n % 26);
            n = n / 26 - 1;
        }
        while (n >= 0 && length < suffix.Length);

        suffix[..length].Reverse();
        return string.Concat("STATION_", suffix[..length]);
    }
}
