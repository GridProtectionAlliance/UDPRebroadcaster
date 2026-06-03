//******************************************************************************************************
//  IConfigurableAugmentation.cs - Gbtc
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

namespace UDPRebroadcaster.Augmentations;

/// <summary>
/// Optional companion to <see cref="IRebroadcastAugmentation"/> implemented by augmentations that
/// read user-tunable values out of <see cref="AppSettings"/>.
/// </summary>
/// <remarks>
/// The main form invokes <see cref="ApplySettings"/> on a freshly-created augmentation instance
/// after <see cref="AugmentationOption.Create"/> and before
/// <see cref="IRebroadcastAugmentation.Initialize"/> — so an implementation's <c>Initialize</c>
/// can rely on whatever state <c>ApplySettings</c> populated. Augmentations with no tunable
/// state (e.g., <c>NoAugmentation</c>) simply do not implement this interface.
/// </remarks>
internal interface IConfigurableAugmentation
{
    /// <summary>
    /// Pulls this augmentation's persisted settings out of the supplied <see cref="AppSettings"/>
    /// and stashes them on the instance for use during the upcoming listening session.
    /// </summary>
    void ApplySettings(AppSettings settings);

    /// <summary>
    /// Brings any per-destination defaults in <paramref name="settings"/> into line with the
    /// supplied <paramref name="destinationCount"/>, mutating <paramref name="settings"/> in
    /// place. The main form calls this whenever the user picks this augmentation in the
    /// drop-down or finishes editing the destinations text box, so that per-destination defaults
    /// (e.g., station labels) materialize without requiring a trip through the settings dialog.
    /// </summary>
    /// <remarks>
    /// Implementations should be additive — pad new entries with defaults, preserve existing
    /// user-supplied values, and avoid truncating tail entries so that temporarily shrinking
    /// then re-growing the destination list doesn't lose user labels.
    /// </remarks>
    void SynchronizeDefaults(AppSettings settings, int destinationCount);
}
