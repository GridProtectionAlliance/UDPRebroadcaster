//******************************************************************************************************
//  SettingsFormAttribute.cs - Gbtc
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
/// Declares the WinForms <see cref="Form"/> type that the main form should display when the user
/// clicks the <c>Settings…</c> button for the decorated augmentation. Presence of this attribute
/// is what enables the button; absence keeps it disabled.
/// </summary>
/// <remarks>
/// <para>
/// The form type must derive from <see cref="Form"/> and expose a public constructor with the
/// signature <c>(AppSettings settings, IReadOnlyList&lt;IPEndPoint&gt; destinations)</c>. The main
/// form instantiates the type via <see cref="Activator.CreateInstance(Type, object?[])"/> and
/// passes the live <see cref="AppSettings"/> instance plus the parsed destination list; the
/// settings form is responsible for reading any previously-persisted values out of
/// <c>AppSettings</c> on open and writing them back on close.
/// </para>
/// <para>
/// The destination count is fixed at the moment the dialog opens — the form uses it to render one
/// row per destination (e.g., per-destination labels) without needing to react to live changes,
/// since the main form's destination text box is editable only while the listener is stopped.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class SettingsFormAttribute(Type formType) : Attribute
{
    /// <summary>
    /// Gets the <see cref="Form"/>-derived type that provides the settings UI for the decorated
    /// augmentation.
    /// </summary>
    public Type FormType { get; } = formType;
}
