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
//  06/02/2026 - J. Ritchie Carroll
//       Added pluggable per-destination augmentation pipeline (SEL CWS channel ID rewriting);
//       dropped Gemstone.Communication dependency, receive now uses the BCL UdpClient.
//
//******************************************************************************************************
// ReSharper disable LocalizableElement

using System.Net;
using System.Net.Sockets;
using System.Reflection;
using UDPRebroadcaster.Augmentations;

namespace UDPRebroadcaster;

public partial class UDPRebroadcaster : Form
{
    private const long UpdateInterval = TimeSpan.TicksPerSecond * 2;

    private AppSettings m_settings = new();
    private IReadOnlyList<AugmentationOption> m_augmentationOptions = [];

    // Receive side: BCL UdpClient bound to the listen port. A background task pumps it and
    // dispatches each datagram into the per-destination fan-out path.
    private UdpClient? m_receiveClient;
    private CancellationTokenSource? m_receiveCts;

    // Send side: a single UdpClient on an ephemeral local port, used for per-destination sends.
    // When augmentation is active each destination needs its own payload, so an indexed
    // IPEndPoint array gives us deterministic per-destination control.
    private UdpClient? m_sendClient;
    private IPEndPoint[]? m_destinations;
    private IRebroadcastAugmentation? m_augmentation;

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

        // Populate the augmentation drop-down once, from reflection.
        m_augmentationOptions = AugmentationDiscovery.DiscoverAll();
        AugmentationOptions.Items.Clear();

        foreach (AugmentationOption option in m_augmentationOptions)
            AugmentationOptions.Items.Add(option);

        // Restore previously selected augmentation by type name; fall back to the first entry
        // (the NoAugmentation default per AugmentationDiscovery's ordering) if missing.
        // Setting SelectedItem fires SelectedIndexChanged, which in turn syncs per-destination
        // defaults into AppSettings and recomputes the Settings button's enabled state — no
        // explicit call to either here.
        AugmentationOption? restored = m_augmentationOptions.FirstOrDefault(o => o.Type.Name == m_settings.Augmentation);
        AugmentationOptions.SelectedItem = restored ?? m_augmentationOptions.FirstOrDefault();

        Version version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);
        Status.Text = $"Version {version.Major}.{version.Minor}.{version.Build}";

        if (AutoListen.Checked)
            Listen_Click(this, EventArgs.Empty);
    }

    private void UDPRebroadcaster_FormClosing(object? sender, FormClosingEventArgs e)
    {
        StopRebroadcasting();

        // Make a final pass so any in-flight edits to the destinations field that never lost
        // focus still get reflected in the augmentation's per-destination defaults before save.
        SynchronizeAugmentationDefaults();

        m_settings.Port = Port.Text;
        m_settings.RebroadcastDestinations = RebroadcastDestinations.Text;
        m_settings.AutoListen = AutoListen.Checked;
        m_settings.Augmentation = (AugmentationOptions.SelectedItem as AugmentationOption)?.Type.Name ?? nameof(NoAugmentation);
        m_settings.CaptureWindowLayout(this);
        m_settings.Save();
    }

    private void AugmentationOptions_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // Selection just changed — give the new augmentation a chance to materialize
        // per-destination defaults (e.g., station labels) in AppSettings before the user clicks
        // anywhere else, then refresh the Settings button's enabled state for the new type.
        SynchronizeAugmentationDefaults();
        UpdateSettingsButtonState();
    }

    private void RebroadcastDestinations_Leave(object? sender, EventArgs e)
    {
        // User finished editing the destinations text box; resync the selected augmentation's
        // per-destination defaults so a freshly-added endpoint already has a sensible default
        // label by the time the listener (or the settings dialog) reads them.
        SynchronizeAugmentationDefaults();
    }

    private void AugmentationSettings_Click(object? sender, EventArgs e)
    {
        if (AugmentationOptions.SelectedItem is not AugmentationOption selected)
            return;

        SettingsFormAttribute? attribute = selected.Type.GetCustomAttribute<SettingsFormAttribute>();

        if (attribute is null)
            return;

        // Sync first so the dialog renders exactly one row per current destination, with default
        // labels in place for any that the user hasn't customized yet.
        SynchronizeAugmentationDefaults();

        IPEndPoint[] destinations;

        try
        {
            destinations = ParseDestinations(RebroadcastDestinations.Text);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Cannot open settings — current destinations are not parseable:\n\n{ex.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            using Form form = (Form)Activator.CreateInstance(attribute.FormType, m_settings, destinations)!;
            form.ShowDialog(this);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SynchronizeAugmentationDefaults()
    {
        if (AugmentationOptions.SelectedItem is not AugmentationOption selected)
            return;

        // Transient instance — created only so SynchronizeDefaults has somewhere to dispatch to;
        // discarded immediately after. Augmentations without IConfigurableAugmentation contribute
        // nothing here, which is correct (no per-destination state to maintain).
        if (selected.Create() is not IConfigurableAugmentation configurable)
            return;

        configurable.SynchronizeDefaults(m_settings, CountDestinations(RebroadcastDestinations.Text));
    }

    private void UpdateSettingsButtonState()
    {
        // Enabled iff the current augmentation declares a settings form AND we're not in the
        // middle of a listening session (the augmentation captured its settings at Start, so
        // editing mid-session would be silently ignored — same pattern as the other locked
        // controls).
        bool hasForm = AugmentationOptions.SelectedItem is AugmentationOption selected
            && selected.Type.GetCustomAttribute<SettingsFormAttribute>() is not null;

        AugmentationSettings.Enabled = hasForm && m_receiveClient is null;
    }

    private static int CountDestinations(string text)
    {
        // Cheap count for sync purposes — avoids DNS resolution / port validation that
        // ParseDestinations does, so it's safe to call during transient text-box states.
        return text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length;
    }

    private void Listen_Click(object? sender, EventArgs e)
    {
        try
        {
            if (m_receiveClient is null)
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

        // Port, destinations, and augmentation are all captured below and never re-read during
        // the listening session. Disable their controls so the user can see the selections are
        // locked in. StopRebroadcasting re-enables them (including from the catch path in
        // Listen_Click if startup fails).
        Port.Enabled = false;
        RebroadcastDestinations.Enabled = false;
        AugmentationOptions.Enabled = false;
        AugmentationSettings.Enabled = false;

        SampleCount.Text = "0";
        SampleRate.Text = "0";
        m_samples = 0;
        m_lastSamples = 0;
        m_lastUpdate = DateTime.UtcNow.Ticks;

        if (!int.TryParse(Port.Text, out int listenPort) || listenPort is < 1 or > 65535)
            throw new FormatException($"Invalid listen port '{Port.Text}'.");

        // Resolve destinations to IPEndPoint[] up-front so per-packet sends do no DNS / parsing.
        m_destinations = ParseDestinations(RebroadcastDestinations.Text);

        if (m_destinations.Length == 0)
            throw new InvalidOperationException("At least one rebroadcast destination is required.");

        // Instantiate the selected augmentation strategy and let it size itself to the
        // destination list.
        AugmentationOption selected = AugmentationOptions.SelectedItem as AugmentationOption
            ?? m_augmentationOptions.First(option => option.Type == typeof(NoAugmentation));

        m_augmentation = selected.Create();

        // ApplySettings runs before Initialize so an augmentation's per-destination setup (e.g.,
        // random-mode ID generation) can read whatever settings the user persisted via the
        // settings dialog. Augmentations without IConfigurableAugmentation skip this step.
        (m_augmentation as IConfigurableAugmentation)?.ApplySettings(m_settings);

        m_augmentation.Initialize(m_destinations);

        // Send socket: ephemeral local port, IPv4. Sends are synchronous; UDP sends are fast
        // enough that this is cheaper than the async machinery for typical fan-out sizes.
        m_sendClient = new UdpClient(AddressFamily.InterNetwork) { Client = { SendBufferSize = 65536 } };

        // Receive socket: bind 0.0.0.0:listenPort, then drain it from a background task.
        m_receiveClient = new UdpClient(listenPort) { Client = {ReceiveBufferSize = 65536 } };

        m_receiveCts = new CancellationTokenSource();
        CancellationToken token = m_receiveCts.Token;
        UdpClient receiveClient = m_receiveClient;

        // Fire-and-forget; the loop terminates when the CTS is cancelled or the client is
        // disposed. Errors are absorbed inside the loop.
        Task.Run(() => ReceiveLoopAsync(receiveClient, token), token);
    }

    private void StopRebroadcasting()
    {
        Listen.Text = "&Start";
        Port.Enabled = true;
        RebroadcastDestinations.Enabled = true;
        AugmentationOptions.Enabled = true;

        // Cancel first so the loop sees IsCancellationRequested even if it was about to issue
        // another ReceiveAsync; then dispose the socket which unblocks an in-flight receive.
        if (m_receiveCts is not null)
        {
            m_receiveCts.Cancel();
            m_receiveCts.Dispose();
            m_receiveCts = null;
        }

        if (m_receiveClient is not null)
        {
            m_receiveClient.Dispose();
            m_receiveClient = null;
        }

        if (m_sendClient is not null)
        {
            m_sendClient.Dispose();
            m_sendClient = null;
        }

        m_destinations = null;
        m_augmentation = null;

        // Run this AFTER m_receiveClient is nulled out — the helper reads that field to decide
        // whether to enable the button, so calling it earlier would leave the button disabled
        // even after the listener has fully stopped.
        UpdateSettingsButtonState();
    }

    private async Task ReceiveLoopAsync(UdpClient client, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                UdpReceiveResult result = await client.ReceiveAsync(cancellationToken).ConfigureAwait(false);
                HandleReceivedDatagram(result.Buffer, result.Buffer.Length);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when Stop() cancels the token.
        }
        catch (ObjectDisposedException)
        {
            // Expected when Stop() disposes the receive client.
        }
        catch (SocketException)
        {
            // Socket-level failure; treat as a stop signal.
        }
    }

    private void HandleReceivedDatagram(byte[] buffer, int length)
    {
        // Capture all references once at entry. The form may be closing on another thread; a
        // concurrent Stop() must not be able to NullReference us mid-fan-out.
        IPEndPoint[]? destinations = m_destinations;
        UdpClient? sendClient = m_sendClient;
        IRebroadcastAugmentation? augmentation = m_augmentation;

        if (destinations is null || sendClient is null || augmentation is null)
            return;

        Span<byte> packet = buffer.AsSpan(0, length);

        // Render the hex display string here, on the receive thread, BEFORE the fan-out
        // mutates the buffer. Marshalling a finished string (not a buffer reference) to the UI
        // thread sidesteps both the in-place-mutation race and any per-packet byte[] snapshot
        // allocation. The string is the only allocation, paid only when ShowData is on.
        string? hexDisplay = ShowData.Checked && !IsDisposed ? FormatHexDump(packet) : null;

        // Let the augmentation snapshot any per-packet state it needs (e.g., the original
        // channel ID) before the loop starts overwriting bytes.
        augmentation.BeginPacket(packet);

        for (int i = 0; i < destinations.Length; i++)
        {
            // TransformForDestination returns the exact slice to send — may be the same length
            // as 'packet', a prefix of it (augmentation shrunk the payload), or a span into the
            // augmentation's scratch buffer (augmentation grew the payload past the receive
            // buffer's capacity). Either way the slice is only valid until the next call into
            // the augmentation, which is fine — Send copies synchronously below.
            ReadOnlySpan<byte> outgoing = augmentation.TransformForDestination(packet, i);

            try
            {
                // Span overload — UdpClient.Send copies into the kernel send buffer
                // synchronously, so we're free to mutate 'packet' again for the next iteration.
                sendClient.Send(outgoing, destinations[i]);
            }
            catch (ObjectDisposedException)
            {
                // Stop() ran between our capture and this send; drop the rest of the fan-out
                // and return — there is nothing to retry to.
                return;
            }
            catch (SocketException)
            {
                // One destination unreachable shouldn't kill the rest of the broadcast.
            }
        }

        m_samples++;

        if (hexDisplay is not null)
            BeginInvoke(() => UDPFrame.Text = hexDisplay);

        long now = DateTime.UtcNow.Ticks;

        if (now - m_lastUpdate < UpdateInterval)
            return;

        m_sampleRate = (m_samples - m_lastSamples) / ((now - m_lastUpdate) / (double)TimeSpan.TicksPerSecond);
        m_lastUpdate = now;
        m_lastSamples = m_samples;

        if (!IsDisposed)
            BeginInvoke(UpdateStats);
    }

    private static IPEndPoint[] ParseDestinations(string text)
    {
        return text
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(ParseEndPoint)
            .ToArray();
    }

    private static IPEndPoint ParseEndPoint(string entry)
    {
        int colon = entry.LastIndexOf(':');

        if (colon <= 0 || colon == entry.Length - 1)
            throw new FormatException($"Invalid destination '{entry}'. Expected host:port.");

        string host = entry[..colon].Trim();
        string portText = entry[(colon + 1)..].Trim();

        if (!int.TryParse(portText, out int port) || port is < 1 or > 65535)
            throw new FormatException($"Invalid port in destination '{entry}'.");

        IPAddress address;

        if (IPAddress.TryParse(host, out IPAddress? parsed))
        {
            address = parsed;
        }
        else
        {
            // Resolve DNS once at start; per-packet sends never block on DNS.
            IPAddress[] resolved = Dns.GetHostAddresses(host);

            address = resolved.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)
                ?? throw new FormatException($"Could not resolve '{host}' to an IPv4 address.");
        }

        return new IPEndPoint(address, port);
    }

    private void About_Click(object? sender, EventArgs e)
    {
        using AboutBox about = new();
        about.ShowDialog(this);
    }

    private static string FormatHexDump(ReadOnlySpan<byte> buffer)
    {
        if (buffer.IsEmpty)
            return string.Empty;

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

        return new string(spaced, 0, Math.Max(0, j - 1));
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
