//******************************************************************************************************
//  AppSettings.cs - Gbtc
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
// ReSharper disable ConvertTypeCheckPatternToNullCheck

using System.Text.Json;

namespace UDPRebroadcaster;

internal sealed class AppSettings
{
    private static readonly string s_settingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Grid Protection Alliance",
        "UDP Rebroadcaster",
        "settings.json");

    public string Port { get; set; } = "3050";

    public string RebroadcastDestinations { get; set; } = "127.0.0.1:3060, 127.0.0.1:3070";

    public bool AutoListen { get; set; }

    public int? WindowX { get; set; }

    public int? WindowY { get; set; }

    public int? WindowWidth { get; set; }

    public int? WindowHeight { get; set; }

    public FormWindowState WindowState { get; set; } = FormWindowState.Normal;

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(s_settingsPath))
                return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(s_settingsPath)) ?? new AppSettings();
        }
        catch
        {
            // Fall through to defaults on any deserialization failure
        }

        return new AppSettings();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(s_settingsPath)!);
            File.WriteAllText(s_settingsPath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch
        {
            // Settings persistence is best-effort; do not crash the app if the disk is unwritable.
        }
    }

    public void CaptureWindowLayout(Form form)
    {
        // Always capture the restored bounds so a maximized window still remembers its prior size/position.
        Rectangle bounds = form.WindowState == FormWindowState.Normal ? form.Bounds : form.RestoreBounds;

        WindowX = bounds.X;
        WindowY = bounds.Y;
        WindowWidth = bounds.Width;
        WindowHeight = bounds.Height;
        WindowState = form.WindowState == FormWindowState.Minimized ? FormWindowState.Normal : form.WindowState;
    }

    public void ApplyWindowLayout(Form form)
    {
        if (WindowWidth is int w && WindowHeight is int h && WindowX is int x && WindowY is int y)
        {
            Rectangle desired = new(x, y, w, h);

            // Only restore the position if some screen still contains it; otherwise let the form default.
            if (Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(desired)))
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Bounds = desired;
            }
        }

        if (WindowState == FormWindowState.Maximized)
            form.WindowState = FormWindowState.Maximized;
    }
}
