# Changelog

All notable changes to UDP Rebroadcaster.

## 2.1.0 — 2026-06-02

Adds per-augmentation settings UI so augmentation-specific tuning no longer requires hand-editing the JSON. `SELCWSChannelIDAugmentation` is the first augmentation wired up: it gains an **Incremented / Random** channel-ID mode toggle and **per-destination station labels** that are written into outgoing configuration frames.

### Highlights

- New **`Settings…`** button on the main form, immediately right of the augmentation drop-down. Enabled only when the selected augmentation declares a settings form and the listener is stopped — same lock-during-listen pattern as the other capture-at-Start controls.
- Augmentations opt into a settings UI by class-level `[SettingsForm(typeof(…))]` attribute (same discovery shape as `[Label("…")]`); the main form has no per-augmentation knowledge.
- Per-destination defaults (e.g., station labels) materialize automatically the moment the user picks the augmentation or finishes editing the destinations field — even if they never open the dialog.

### Added

- **`SettingsFormAttribute`** (`src/Augmentations/SettingsFormAttribute.cs`) — declares the WinForms `Form` to display for the decorated augmentation. The form must expose a constructor `(AppSettings, IReadOnlyList<IPEndPoint>)`; the main form instantiates it via `Activator.CreateInstance`.
- **`IConfigurableAugmentation`** (`src/Augmentations/IConfigurableAugmentation.cs`) — companion interface to `IRebroadcastAugmentation` for augmentations with tunable state. `ApplySettings(AppSettings)` runs before `Initialize`; `SynchronizeDefaults(AppSettings, int destinationCount)` is invoked by the main form on augmentation-selected and destinations-changed.
- **`SELCWSChannelIDSettings`** (`src/Augmentations/SELCWSChannelIDSettings.cs`) — POCO persisted as the `SELCWSChannelID` section of `AppSettings`. Carries the `ChannelIDGenerationMode` (Incremented / Random) and a `List<string>` of per-destination station labels.
- **`SELCWSChannelIDSettingsForm`** (`src/Augmentations/SELCWSChannelIDSettingsForm.cs` + `.Designer.cs`) — fixed-size dialog with a `Unique Channel ID Generation` group (Incremented / Random radios) and a vertically-scrolling `FlowLayoutPanel` of station-label text boxes, one per current destination. OK persists `AppSettings` to disk; Cancel discards.
- **`AugmentationSettings`** button on the main form — `Top|Right` anchored, 88×27 (matches `Start` / `About…`), positioned to the right of the augmentation drop-down with a 6-px gap. Tool-tip explains why it's disabled when it is.
- **`SELCWSChannelID` section** in `settings.json` — holds the generation mode and station-label list. Missing on first run of 2.1; defaults populate transparently.
- **`ChannelIDGenerationMode.Random`** — destination *N* receives a fresh random `ulong` generated once at `Initialize` time. Stable for the lifetime of the listening session; fresh on the next Start.
- **Station-label rewrite in SEL CWS configuration frames** — `SELCWSChannelIDAugmentation` now also overwrites the variable-length `ChannelName` field with the user-supplied per-destination label, written as null-terminated UTF-8 per SEL-735 IM Appendix J Table J.3. The field's offset is `24 + 4 * NumAnalogs` (read live from the frame header), its length is bounded at 21 bytes total per spec; the augmentation reads the original field's length out of the incoming frame, rewrites the field, then shifts the post-ChannelName tail (`SignalNames`) to its new offset and updates the `Size` header to match. Data frames are untouched. Frames too short or otherwise malformed are passed through with only the ChannelID rewrite applied.
- **Frame recognition** — CWS frames are recognized by the FrameID's two bytes: high byte is the type designator (see new `FrameType` enum — `DataFrame = 0x00`, `ConfigurationFrame = 0x01`), low byte is the protocol version (`0x01` for v1). `IsSELCWSFrame` accepts any frame whose version byte is `0x01` and whose type byte is one of the known `FrameType` values; `BeginPacket` then uses the type byte to discriminate configuration vs. data. Replaces the 2.0 byte-pattern recognizer which had the type-byte values swapped (it assumed config `0x00` / data `0x01`, but the wire is the opposite per the authoritative enum).
- **`FrameType` enum** (`src/Augmentations/FrameType.cs`) — authoritative SEL CWS frame-type designator used by `SELCWSChannelIDAugmentation` for config-vs-data discrimination. Public + `[Serializable]` so future SEL CWS code in the project can reference the same values.
- **Variable-length SignalNames** — the post-ChannelName tail is treated as opaque bytes whose internal structure (N packed null-terminated UTF-8 names per Table J.3) doesn't matter to the rewrite. Snapshotted byte-for-byte in `BeginPacket`, laid down verbatim at the new ChannelName offset. The earlier draft that assumed a fixed 21-byte tail would have shifted the wrong number of bytes for any frame whose total SignalNames span deviated from 21.
- **`IRebroadcastAugmentation.TransformForDestination` return type widened** from `void` to `ReadOnlySpan<byte>` — augmentations now hand back the exact slice to send. Lets a per-destination rewrite change the packet length (e.g., a SEL CWS label longer or shorter than the device's original TID). `NoAugmentation` simply returns `buffer`; the receive loop hands the returned slice straight to `UdpClient.Send`.
- **Internal scratch buffers in `SELCWSChannelIDAugmentation`** — a small pre-allocated work buffer covers the rare case where the user-supplied label encodes to more UTF-8 bytes than the source device's original ChannelName, plus a tail-snapshot buffer that preserves `SignalNames` across the fan-out so a per-destination shift doesn't read garbage left behind by the prior destination's rewrite. Both allocated once at `Initialize`; zero per-packet heap allocations on the hot path.
- **Default station labels** — `STATION_A`, `STATION_B`, …, `STATION_Z`, then `STATION_AA`, `STATION_AB`, … materialized for each destination automatically (no need to open the settings dialog).

### Changed

- **Main form expanded** to `ClientSize` 878×481 (was 784×481) and `MinimumSize` 819×455 (was 725×455) — accommodates the new `Settings…` button with the same 15-px right margin and 6-px inter-control gap as the rest of the top row.
- **`AugmentationOptions` drop-down** keeps its `Top|Left|Right` anchor; design-time right margin grows to 109 px so the new button slots in to its right and the gap between them stays constant under resize.
- **`StopRebroadcasting`** now calls `UpdateSettingsButtonState` to re-enable the `Settings…` button (instead of forcing it on), so the enabled state respects whether the current augmentation has a settings form.
- **`UDPRebroadcaster_FormClosing`** runs `SynchronizeAugmentationDefaults` immediately before `Save` so any unfocused destination-text edits still get reflected in the persisted defaults.

### Architecture / internals

- **Settings flow at Start**: `selected.Create()` → `(augmentation as IConfigurableAugmentation)?.ApplySettings(m_settings)` → `augmentation.Initialize(destinations)`. `ApplySettings` is the augmentation's only chance to read tunables before `Initialize` runs its per-destination setup (e.g., pre-generating random IDs).
- **Default-sync triggers**: `AugmentationOptions.SelectedIndexChanged`, `RebroadcastDestinations.Leave`, before opening the settings dialog, and in `FormClosing`. Each invocation creates a transient augmentation instance solely to dispatch `SynchronizeDefaults(settings, destinationCount)`; the instance is discarded immediately.
- **Settings dialog contract**: the form's constructor receives the live `AppSettings` instance plus the parsed `IPEndPoint[]` for the *current* destinations field. The form reads from / writes back to that same `AppSettings` and calls `Save()` on OK.
- **`Settings…` button enabled state** = augmentation declares `[SettingsForm]` AND `m_receiveClient is null` (listener stopped). Recomputed on selection change and on Stop; explicitly forced off in Start.
- **Station-label policy on destination-count changes**: additive. Pad to `destinationCount` with defaults; preserve all existing entries even when the destination count shrinks — so a temporary remove-then-add doesn't lose user-supplied labels.
- **Random-mode allocation**: one `ulong[]` per listening session (sized to destination count), populated in `Initialize`. Hot-path read is a single indexed access — zero per-packet allocation, same budget as Incremented mode.

### Notes for users

- **Settings are augmentation-specific.** The `Settings…` button is disabled when **No augmentation** is selected (no settings form declared) and while the listener is running (the augmentation captured its settings at Start; mid-session edits would be silently ignored).
- **Station labels apply to configuration frames only.** Data frames still pass through with just the per-destination channel-ID rewrite — same behavior as 2.0.
- **First-run upgrade from 2.0** is transparent: the new `SELCWSChannelID` section materializes with defaults the first time you exit 2.1.

## 2.0.0 — 2026-06-02

First release of the modernized version: a complete .NET 9 rewrite of the 2005-era VB.NET utility (continuing from the unfinished 2011 C# port) plus a new pluggable per-destination augmentation pipeline.

### Highlights

- Runs on **.NET 9** as a single self-contained Windows executable — no runtime install required on target machines.
- **Zero NuGet dependencies** — pure BCL.
- New **pluggable augmentation pipeline** lets you apply protocol-specific per-destination transformations to outgoing packets without touching the rebroadcast loop. Ships with a built-in **SEL CWS Channel ID rewriter** so you can drive multiple logical receivers from one upstream device.
- **Zero per-packet heap allocation** on the steady-state hot path (in-place `Span<byte>` mutation reused across all destinations).

### Added

- **Augmentation pipeline** (`src/Augmentations/`).
  - `IRebroadcastAugmentation` — the pluggable contract: `Initialize(IReadOnlyList<IPEndPoint>)`, `BeginPacket(ReadOnlySpan<byte>)`, `TransformForDestination(Span<byte>, int)`.
  - `LabelAttribute` — supplies the user-facing drop-down text for an implementation.
  - `AugmentationDiscovery` — reflection-based discovery; populates the UI combo automatically. `NoAugmentation` pinned to the top, rest alphabetical.
  - `NoAugmentation` — default pass-through (true no-op; both pipeline methods are empty bodies).
  - `SELCWSChannelIDAugmentation` — rewrites the `ulong` Channel ID at offset 6 of each SEL CWS frame so destination index `N` receives `originalChannelID + (N + 1)`. Enables fanning one upstream SEL CWS stream into multiple distinguishable copies on a single downstream receiver port (e.g., the SEL CWS Receiver in the SEL SynchroWave Rebroadcaster).
- **Augmentation drop-down** on the main form, populated from discovery; selected augmentation persists across sessions.
- **JSON settings persistence** at `%LocalAppData%\Grid Protection Alliance\UDP Rebroadcaster\settings.json` — listen port, destinations, auto-listen flag, augmentation (by type name), and window position/size/state.
- **About dialog** rebuilt as a standard WinForms partial-class pair (`AboutBox.cs` + `AboutBox.Designer.cs`); loads the logo and disclaimer from embedded resources.
- **Refreshed application logo** — replaced the legacy "openPDC / PMU Connection Tester" wordmark with a styled "UDP Rebroadcaster" wordmark in matching red Arial Narrow Bold Italic; GPA padlock preserved verbatim, same 500×65 dimensions, white background.
- **Single-file self-contained publish** via `dotnet publish -c Release` — win-x64 executable with the .NET runtime bundled.
- **README**, **LICENSE** (MIT), and this **CHANGELOG** at the repo root.

### Changed

- **Receive side**: switched from event-driven `Gemstone.Communication.UdpClient` to BCL `System.Net.Sockets.UdpClient` with a cancellable async receive loop (`Task.Run` + `await ReceiveAsync(CancellationToken)`).
- **Send side**: switched from `Gemstone.Communication.UdpServer.MulticastAsync` (which would have sent identical bytes to every client) to a single BCL `UdpClient` on an ephemeral port plus a pre-resolved `IPEndPoint[]`. Per-destination sends use the `Send(ReadOnlySpan<byte>, IPEndPoint?)` overload — no allocation, deterministic destination ordering, supports per-destination augmented payloads.
- **Augmentation pipeline switched to `Span<byte>` in-place mutation**. An earlier draft of the pipeline allocated one `byte[]` per destination per packet (`out byte[]` from the transform method). The current design mutates the receive buffer in place across all destinations; `BeginPacket(ReadOnlySpan<byte>)` snapshots per-packet state once before the fan-out starts.
- **Hex display path optimization**. The hex string is now formatted on the receive thread *before* fan-out and marshaled to the UI as an immutable string. Replaces the earlier "snapshot the `byte[]` then format on UI thread" approach — eliminates the per-packet snapshot allocation and the race against in-place mutation.
- **Lock during listen**: `Listen on port`, `Rebroadcast destinations`, and `Augmentation` controls disable while the listener is running. All three are captured once at Start, so changes mid-session were silently ignored before; now the UI reflects that.
- **Class name fix**: `UDPRebroacaster` (long-standing typo, missing "d") → `UDPRebroadcaster`. About box renamed/split accordingly.
- **Statistics math**: `Ticks.PerSecond` → BCL `TimeSpan.TicksPerSecond` (same constant, no dependency).
- **Project layout**: code consolidated under `src/`, screenshot under `docs/`, with README + LICENSE + CHANGELOG at the repo root.

### Removed

- **`Gemstone.Communication` NuGet dependency** — receive and send now use pure BCL `System.Net.Sockets.UdpClient`. The project's `.csproj` has zero `<PackageReference>` entries.
- **Legacy `app.config`** — superseded by SDK-style csproj.
- **Legacy `Properties/AssemblyInfo.cs`, `Resources.resx`, `Settings.settings`** — SDK-style csproj auto-generates assembly metadata; Resources / Settings were unused.
- **Embedded-assembly resource loader** from the 2005 VB code (used to bundle TVA assemblies into the single exe) — .NET 9's `PublishSingleFile` handles deployment now.

### Architecture / internals

- Receive loop: `Task.Run(() => ReceiveLoopAsync(client, ct))` awaits `ReceiveAsync(ct)`; `UdpReceiveResult.Buffer` is a fresh `byte[]` per datagram owned exclusively by that packet's pipeline (no buffer-reuse hazards).
- Per-packet flow:
  1. If `Show data` is on, format hex string from the original bytes on the receive thread.
  2. `augmentation.BeginPacket(span)` snapshots per-packet state (e.g., original Channel ID).
  3. For `i = 0..N-1`: `augmentation.TransformForDestination(span, i)` mutates in place, then `sendClient.Send(span, destinations[i])` synchronously copies into the kernel send buffer.
  4. `BeginInvoke` the finished hex string to the UI thread if applicable; rolling sample rate updated on a 2-second wall-clock window.
- Stop: `CancellationTokenSource.Cancel()` first (loop checks `IsCancellationRequested`), then `Dispose()` the receive client (unblocks an in-flight `ReceiveAsync`). `OperationCanceledException`, `ObjectDisposedException`, and `SocketException` inside the loop are silently swallowed as expected on stop.
- SEL CWS frame validation: type ∈ {0x00, 0x01}, version 0x01, length ≥ 16. Non-SEL-CWS packets pass through unchanged rather than being dropped — keeps destinations reachable if a stray non-protocol packet sneaks in.
- SEL CWS has no checksum, so the 8-byte big-endian Channel ID overwrite requires no CRC recompute.
- Steady-state allocation budget per packet (excluding the BCL-allocated receive buffer itself):

  | Mode | Heap allocations |
  | --- | --- |
  | `ShowData` off, `NoAugmentation` | 0 |
  | `ShowData` off, `SELCWSChannelIDAugmentation`, N destinations | 0 |
  | `ShowData` on, any augmentation | 1 (the hex display string) |

### Notes for users

- **Augmentation is opt-in**: leave the drop-down on **No augmentation** for byte-for-byte rebroadcast (identical behavior to all prior versions). Pick a specific augmentation only when you need protocol-specific per-destination payload rewriting.
- **Changing Augmentation / Port / Destinations during a listening session has no effect.** The controls disable while listening to make that obvious; Stop, change, Start to apply.
- **First-run settings file**: the application creates `%LocalAppData%\Grid Protection Alliance\UDP Rebroadcaster\settings.json` on first clean exit. No prior-version settings file to migrate from.
- **Single-file deployment**: the published `UDPRebroadcaster.exe` runs on any Windows x64 machine without installing the .NET runtime; first launch may be slightly slower as the runtime self-extracts.

### Scaling-up testing recipe (SEL CWS)

Point a single SEL 735 (or any SEL CWS source) at the rebroadcaster, set three identical destinations:

```
127.0.0.1:3050, 127.0.0.1:3050, 127.0.0.1:3050
```

…and pick **SEL CWS unique per end-point channel ID**. The downstream SEL CWS Receiver listening on `3050` sees three streams with channel IDs `original+1`, `original+2`, `original+3` — three logical channels driven by one hardware device.

### Known limitations

- IPv4 only — `Dns.GetHostAddresses` filters to `AddressFamily.InterNetwork` and the send socket binds `AddressFamily.InterNetwork`.
- Windows only (`net9.0-windows`); the WinForms front-end isn't ported to cross-platform UI.
- The receive loop calls a single-instance augmentation serialized per packet, so implementations don't need to be thread-safe but can't take advantage of parallelism across destinations either.

### History context

The original VB.NET / .NET 2.0 implementation dates to 2005, built against the TVA shared code libraries (the predecessors of Grid Solutions Framework (GSF) and the Gemstone Libraries). A partial 2011 C# port targeting .NET Framework 4 against GSF existed but was never finished. This 2.0.0 release modernizes the project end-to-end to .NET 9, swaps every external dependency for the BCL, and adds the augmentation pipeline.
