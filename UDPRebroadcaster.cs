//******************************************************************************************************
//  UDPRebroadcaster.cs - Gbtc
//
//  Copyright © 2025, Grid Protection Alliance.  All Rights Reserved.
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
//  06/01/2005 - J. Ritchie Carroll
//       Generated original version of source code (Visual Basic)
//  08/29/2011 - J. Ritchie Carroll
//       Updated to C# using GSF libraries
//  06/02/2012 - J. Ritchie Carroll
//       Ported to .NET 9 using Gemstone Libraries
//
//******************************************************************************************************
// ReSharper disable LocalizableElement

using System.Reflection;
using Gemstone;
using Gemstone.Communication;

namespace UDPRebroadcaster;

public partial class UDPRebroadcaster : Form
{
    private const long UpdateInterval = Ticks.PerSecond * 2;

    private AppSettings m_settings = new();
    private UdpClient? m_udpClient;
    private UdpServer? m_udpServer;
    private long m_samples;
    private long m_lastSamples;
    private long m_lastUpdate;
    private double m_sampleRate;

    public UDPRebroadcaster()
    {
        InitializeComponent();
    }

    private void UDPRebroadcaster_Load(object? sender, EventArgs e)
    {
        m_settings = AppSettings.Load();

        Port.Text = m_settings.Port;
        RebroadcastDestinations.Text = m_settings.RebroadcastDestinations;
        AutoListen.Checked = m_settings.AutoListen;
        m_settings.ApplyWindowLayout(this);

        Version version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);
        Status.Text = $"Version {version.Major}.{version.Minor}.{version.Build}";

        if (AutoListen.Checked)
            Listen_Click(this, EventArgs.Empty);
    }

    private void UDPRebroadcaster_FormClosing(object? sender, FormClosingEventArgs e)
    {
        StopRebroadcasting();

        m_settings.Port = Port.Text;
        m_settings.RebroadcastDestinations = RebroadcastDestinations.Text;
        m_settings.AutoListen = AutoListen.Checked;
        m_settings.CaptureWindowLayout(this);
        m_settings.Save();
    }

    private void Listen_Click(object? sender, EventArgs e)
    {
        try
        {
            if (m_udpServer is null)
                StartRebroadcasting();
            else
                StopRebroadcasting();
        }
        catch (Exception ex)
        {
            // Make sure we don't leave partial state behind if startup failed midway.
            StopRebroadcasting();
            MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void StartRebroadcasting()
    {
        Listen.Text = "&Stop";
        SampleCount.Text = "0";
        SampleRate.Text = "0";
        m_samples = 0;
        m_lastSamples = 0;
        m_lastUpdate = DateTime.UtcNow.Ticks;

        m_udpClient = new UdpClient($"port={Port.Text}") { ReceiveBufferSize = 65536 };
        m_udpClient.ReceiveDataComplete += UdpClient_ReceiveDataComplete;
        m_udpClient.ConnectAsync();

        m_udpServer = new UdpServer($"port=-1; clients={RebroadcastDestinations.Text}; interface=0.0.0.0") { SendBufferSize = 65536 };
        m_udpServer.Start();
    }

    private void StopRebroadcasting()
    {
        Listen.Text = "&Start";

        if (m_udpClient is not null)
        {
            m_udpClient.ReceiveDataComplete -= UdpClient_ReceiveDataComplete;
            m_udpClient.Disconnect();
            m_udpClient.Dispose();
            m_udpClient = null;
        }

        if (m_udpServer is not null)
        {
            m_udpServer.Stop();
            m_udpServer.Dispose();
            m_udpServer = null;
        }
    }

    private void UdpClient_ReceiveDataComplete(object? sender, EventArgs<byte[], int> e)
    {
        byte[] buffer = e.Argument1;
        int length = e.Argument2;

        // Capture server reference once so a concurrent Stop() can't NullReference us mid-call.
        UdpServer? server = m_udpServer;
        server?.MulticastAsync(buffer, 0, length);

        m_samples++;

        if (ShowData.Checked && !IsDisposed)
        {
            // Snapshot bytes — buffer is reused by the reception loop.
            byte[] snapshot = new byte[length];
            Buffer.BlockCopy(buffer, 0, snapshot, 0, length);
            BeginInvoke(() => ShowBinaryData(snapshot));
        }

        long now = DateTime.UtcNow.Ticks;

        if (now - m_lastUpdate < UpdateInterval)
            return;
        
        m_sampleRate = (m_samples - m_lastSamples) / ((now - m_lastUpdate) / (double)TimeSpan.TicksPerSecond);
        m_lastUpdate = now;
        m_lastSamples = m_samples;

        if (!IsDisposed)
            BeginInvoke(UpdateStats);
    }

    private void About_Click(object? sender, EventArgs e)
    {
        using AboutBox about = new();
        about.ShowDialog(this);
    }

    private void ShowBinaryData(byte[] buffer)
    {
        // Convert.ToHexString produces unseparated uppercase pairs; insert a space between each pair.
        string hex = Convert.ToHexString(buffer);
        char[] spaced = new char[hex.Length + buffer.Length];
        int j = 0;

        for (int i = 0; i < hex.Length; i += 2)
        {
            spaced[j++] = hex[i];
            spaced[j++] = hex[i + 1];
            spaced[j++] = ' ';
        }

        UDPFrame.Text = new string(spaced, 0, Math.Max(0, j - 1));
    }

    private void UpdateStats()
    {
        SampleCount.Text = m_samples.ToString();
        SampleRate.Text = m_sampleRate.ToString("0.00");
    }

    private void ShowData_CheckedChanged(object? sender, EventArgs e)
    {
        if (!ShowData.Checked)
            UDPFrame.Text = string.Empty;
    }
}
