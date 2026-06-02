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

/// <summary>
/// Represents the application settings for the UDP Rebroadcaster.
/// </summary>
/// <remarks>
/// This class encapsulates configuration options such as network settings, window layout, 
/// and user preferences. It provides methods to load and save settings to a JSON file 
/// located in the user's local application data folder, as well as methods to capture 
/// and apply window layout configurations.
/// </remarks>
internal sealed class AppSettings
{
    private static readonly string s_settingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Grid Protection Alliance",
        "UDP Rebroadcaster",
        "settings.json");

    /// <summary>
    /// Gets or sets the port number used for UDP rebroadcasting.
    /// </summary>
    /// <remarks>
    /// The default value is "3050".
    /// </remarks>
    public string Port { get; set; } = "3050";

    /// <summary>
    /// Gets or sets the destinations for rebroadcasting UDP packets.
    /// </summary>
    /// <remarks>
    /// The value is a comma-separated list of destination addresses and ports in the format "IP:Port".
    /// For example: "127.0.0.1:3060, 127.0.0.1:3070".
    /// </remarks>
    public string RebroadcastDestinations { get; set; } = "127.0.0.1:3060, 127.0.0.1:3070";

    /// <summary>
    /// Gets or sets a value indicating whether the application should automatically start listening
    /// for incoming UDP packets upon launch.
    /// </summary>
    /// <value>
    /// <c>true</c> if the application should automatically start listening; otherwise, <c>false</c>.
    /// </value>
    public bool AutoListen { get; set; }

    /// <summary>
    /// Type name (<see cref="Type.Name"/>) of the selected <c>IRebroadcastAugmentation</c>
    /// implementation. Persisted by type name rather than label so the saved selection survives
    /// label-text changes; defaults to <c>NoAugmentation</c> if missing or unrecognized.
    /// </summary>
    public string Augmentation { get; set; } = "NoAugmentation";

    /// <summary>
    /// 
    /// </summary>
    public int? WindowX { get; set; }

    /// <summary>
    /// Gets or sets the Y-coordinate of the window's position on the screen.
    /// </summary>
    /// <remarks>
    /// This property is used to store and restore the vertical position of the application window
    /// when its layout is saved and reapplied.
    /// </remarks>
    public int? WindowY { get; set; }

    /// <summary>
    /// Gets or sets the width of the application window.
    /// </summary>
    /// <remarks>
    /// This property is used to store and retrieve the width of the window when saving or
    /// restoring the application layout.
    /// </remarks>
    public int? WindowWidth { get; set; }

    /// <summary>
    /// Gets or sets the height of the application window.
    /// </summary>
    /// <value>
    /// An <see cref="int"/> representing the height of the window in pixels, or <c>null</c> if not set.
    /// </value>
    /// <remarks>
    /// This property is used to store and restore the height of the application window
    /// when saving and applying the window layout.
    /// </remarks>
    public int? WindowHeight { get; set; }

    /// <summary>
    /// Gets or sets the state of the application window (e.g., Normal, Minimized, or Maximized).
    /// </summary>
    /// <value>
    /// A <see cref="FormWindowState"/> value representing the current state of the application window.
    /// </value>
    /// <remarks>
    /// This property is used to persist and restore the window's state between application sessions.
    /// </remarks>
    public FormWindowState WindowState { get; set; } = FormWindowState.Normal;

    /// <summary>
    /// Loads the application settings from a JSON file located in the user's local application data folder.
    /// If the file does not exist or deserialization fails, default settings are returned.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="AppSettings"/> containing the loaded or default settings.
    /// </returns>
    /// <remarks>
    /// The settings are stored in a file named "settings.json" under the path:
    /// %LocalAppData%\Grid Protection Alliance\UDP Rebroadcaster\.
    /// </remarks>
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

    /// <summary>
    /// Saves the current application settings to a JSON file located in the user's local application data folder.
    /// </summary>
    /// <remarks>
    /// The settings are stored in a file named "settings.json" under the path:
    /// %LocalAppData%\Grid Protection Alliance\UDP Rebroadcaster\.
    /// This method ensures the directory exists before attempting to write the file.
    /// If the save operation fails (e.g., due to an unwritable disk), the exception is caught,
    /// and the application continues without crashing.
    /// </remarks>
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

    /// <summary>
    /// Captures the current layout of the specified form, including its size, position, and window state.
    /// </summary>
    /// <param name="form">
    /// The <see cref="Form"/> whose layout is to be captured.
    /// </param>
    /// <remarks>
    /// This method ensures that the restored bounds of the form are captured, even if the form is maximized.
    /// If the form is minimized, the window state is recorded as <see cref="FormWindowState.Normal"/>.
    /// The captured layout can later be applied using the <see cref="ApplyWindowLayout"/> method.
    /// </remarks>
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

    /// <summary>
    /// Applies the previously captured layout to the specified form, including its size, position, and window state.
    /// </summary>
    /// <param name="form">
    /// The <see cref="Form"/> to which the layout is to be applied.
    /// </param>
    /// <remarks>
    /// This method restores the form's bounds only if they intersect with the working area of any screen.
    /// If the form's window state was captured as <see cref="FormWindowState.Maximized"/>, it will be restored as maximized.
    /// </remarks>
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
