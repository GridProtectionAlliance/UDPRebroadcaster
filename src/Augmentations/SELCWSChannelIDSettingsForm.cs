//******************************************************************************************************
//  SELCWSChannelIDSettingsForm.cs - Gbtc
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
// ReSharper disable LocalizableElement

using System.Net;

namespace UDPRebroadcaster.Augmentations;

/// <summary>
/// Settings dialog for <see cref="SELCWSChannelIDAugmentation"/>: pick the channel-ID generation
/// strategy and edit one station label per current destination. Reads from
/// <see cref="AppSettings.SELCWSChannelID"/> on open and writes back on <c>OK</c> (then persists
/// the entire <see cref="AppSettings"/> to disk); <c>Cancel</c> discards.
/// </summary>
/// <remarks>
/// The constructor signature is what <see cref="SettingsFormAttribute"/> mandates — the main form
/// uses <see cref="Activator.CreateInstance(Type, object?[])"/> to build the dialog and depends
/// on this exact <c>(AppSettings, IReadOnlyList&lt;IPEndPoint&gt;)</c> shape.
/// </remarks>
internal sealed partial class SELCWSChannelIDSettingsForm : Form
{
    // Width of one row inside the FlowLayoutPanel: panel client width minus the vertical
    // scrollbar (~17px) minus a small margin so rows never get clipped on the right edge.
    private const int RowWidth = 390;
    private const int RowHeight = 28;
    private const int RowLabelWidth = 180;
    private const int RowTextBoxLeft = RowLabelWidth + 8;

    private readonly AppSettings m_settings;
    private readonly IReadOnlyList<IPEndPoint> m_destinations;
    private readonly TextBox[] m_labelTextBoxes;

    public SELCWSChannelIDSettingsForm(AppSettings settings, IReadOnlyList<IPEndPoint> destinations)
    {
        InitializeComponent();

        m_settings = settings;
        m_destinations = destinations;
        m_labelTextBoxes = new TextBox[destinations.Count];

        // Selection state pulled directly out of the live settings the caller passed in;
        // SynchronizeDefaults has already padded StationLabels for every current destination, so
        // BuildLabelRows can just read positionally.
        IncrementedRadio.Checked = settings.SELCWSChannelID.GenerationMode == ChannelIDGenerationMode.Incremented;
        RandomRadio.Checked = settings.SELCWSChannelID.GenerationMode == ChannelIDGenerationMode.Random;

        BuildLabelRows();
    }

    private void BuildLabelRows()
    {
        // SuspendLayout / ResumeLayout around bulk Add so the panel doesn't reflow per row.
        StationLabelsPanel.SuspendLayout();

        try
        {
            List<string> stored = m_settings.SELCWSChannelID.StationLabels;

            for (int i = 0; i < m_destinations.Count; i++)
            {
                Panel row = new()
                {
                    Size = new Size(RowWidth, RowHeight),
                    Margin = new Padding(3),
                };

                Label endpointLabel = new()
                {
                    Location = new Point(0, 5),
                    Size = new Size(RowLabelWidth, 18),
                    TextAlign = ContentAlignment.MiddleRight,
                    Text = $"Endpoint {i + 1} ({m_destinations[i]}):",
                };

                TextBox labelBox = new()
                {
                    Location = new Point(RowTextBoxLeft, 2),
                    Size = new Size(RowWidth - RowTextBoxLeft, 23),
                    Text = i < stored.Count ? 
                        stored[i] :
                        SELCWSChannelIDSettings.DefaultStationLabel(i),
                };

                row.Controls.Add(endpointLabel);
                row.Controls.Add(labelBox);
                StationLabelsPanel.Controls.Add(row);

                m_labelTextBoxes[i] = labelBox;
            }
        }
        finally
        {
            StationLabelsPanel.ResumeLayout(true);
        }
    }

    private void OkBtn_Click(object? sender, EventArgs e)
    {
        m_settings.SELCWSChannelID.GenerationMode = RandomRadio.Checked
            ? ChannelIDGenerationMode.Random
            : ChannelIDGenerationMode.Incremented;

        // Write each visible row back to its slot in the persisted list, growing the list if
        // needed. Slots past m_destinations.Count are left alone so a shrink-then-grow cycle on
        // the main form preserves the user's prior labels (matches SynchronizeDefaults's policy).
        List<string> stored = m_settings.SELCWSChannelID.StationLabels;

        for (int i = 0; i < m_labelTextBoxes.Length; i++)
        {
            string value = m_labelTextBoxes[i].Text;

            if (i < stored.Count)
                stored[i] = value;
            else
                stored.Add(value);
        }

        m_settings.Save();
    }
}
