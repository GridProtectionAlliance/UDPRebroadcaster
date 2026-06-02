//******************************************************************************************************
//  AugmentationDiscovery.cs - Gbtc
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

using System.Reflection;

namespace UDPRebroadcaster.Augmentations;

/// <summary>
/// Represents one discovered <see cref="IRebroadcastAugmentation"/> implementation, suitable for
/// binding directly to a WinForms <c>ComboBox</c> (<see cref="ToString"/> yields the drop-down
/// label).
/// </summary>
internal sealed class AugmentationOption
{
    public AugmentationOption(string label, Type type)
    {
        Label = label;
        Type = type;
    }

    /// <summary>User-facing label, from the implementation's <see cref="LabelAttribute"/>.</summary>
    public string Label { get; }

    /// <summary>The augmentation implementation type.</summary>
    public Type Type { get; }

    /// <summary>Instantiates a fresh augmentation instance for this option.</summary>
    public IRebroadcastAugmentation Create() => (IRebroadcastAugmentation)Activator.CreateInstance(Type)!;

    public override string ToString() => Label;
}

/// <summary>
/// Discovers <see cref="IRebroadcastAugmentation"/> implementations in the executing assembly.
/// </summary>
internal static class AugmentationDiscovery
{
    /// <summary>
    /// Returns one <see cref="AugmentationOption"/> per discovered implementation. The
    /// <see cref="NoAugmentation"/> entry is sorted to the top of the list; all other entries
    /// follow in case-insensitive label order.
    /// </summary>
    public static IReadOnlyList<AugmentationOption> DiscoverAll()
    {
        return Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false } && typeof(IRebroadcastAugmentation).IsAssignableFrom(type))
            .Select(type => new AugmentationOption(type.GetCustomAttribute<LabelAttribute>()?.Label ?? type.Name, type))
            // Pin NoAugmentation first by sorting it under the empty string; everything else is
            // alphabetical so future additions land in a predictable spot.
            .OrderBy(option => option.Type == typeof(NoAugmentation) ? string.Empty : option.Label, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
