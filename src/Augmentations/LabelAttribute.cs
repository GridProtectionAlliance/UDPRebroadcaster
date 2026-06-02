//******************************************************************************************************
//  LabelAttribute.cs - Gbtc
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
/// Provides a human-readable label for a class, used by the UI to populate a drop-down entry
/// for the decorated type.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class LabelAttribute(string label) : Attribute
{
    /// <summary>
    /// Gets the label text shown in the UI for the decorated type.
    /// </summary>
    public string Label { get; } = label;
}
