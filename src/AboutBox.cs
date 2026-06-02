//******************************************************************************************************
//  AboutBox.cs - Gbtc
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
//  06/02/2026 - J .Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Diagnostics;
using System.Reflection;

namespace UDPRebroadcaster;

internal sealed partial class AboutBox : Form
{
    public AboutBox()
    {
        InitializeComponent();

        Assembly assembly = Assembly.GetExecutingAssembly();
        Version version = assembly.GetName().Version ?? new Version(0, 0, 0);
        string product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "UDP Rebroadcaster";
        string copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? string.Empty;

        Title.Text = $"{product}  •  Version {version.Major}.{version.Minor}.{version.Build}";
        CopyrightLabel.Text = copyright;
        Logo.Image = LoadEmbeddedImage("UDPRebroadcaster.HelpAboutLogo.png");
        Disclaimer.Text = LoadEmbeddedText("UDPRebroadcaster.Disclaimer.txt");
    }

    private void CompanyUrl_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo(CompanyUrl.Text) { UseShellExecute = true });
        }
        catch
        {
            // Browser launch failed — silently ignore so the About dialog stays usable.
        }
    }

    private static Image? LoadEmbeddedImage(string resourceName)
    {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        return stream is null ? null : Image.FromStream(stream);
    }

    private static string LoadEmbeddedText(string resourceName)
    {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

        if (stream is null)
            return string.Empty;

        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}
