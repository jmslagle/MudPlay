# MudPlay

<!-- current-version:start -->
> **Version 3.47.3**
> - Fixed Roomba retrying an item the game refuses forever: the "GET {Amount} {Currency}" refusal uses braces, not the brackets the matcher was looking for, so it had never matched — one run sent `get piece of amber` 42 times and left the sweep ping-ponging between two rooms it could never empty
> - Roomba no longer writes off heavy items as "too heavy to carry" because your pack happened to be full of loot at the time — that call is now judged against the pack at its emptiest, while live headroom still tracks reality
> - When your pack is so full of things Roomba didn't collect that it has room for barely one item, it now delivers what it's holding and stops with the reason, instead of shuttling a single item per round trip forever
> - Roomba's command pacing no longer speeds up when the game tells it to slow down: a prompt can gate a send but can't release one early, since every rate-limit line the game emits carries a prompt of its own
> - A run of rate-limit complaints is treated as one incident rather than restarting the back-off (and filling the log) once per line
> - An interrupted sweep no longer dumps its load on you: whatever Roomba was still carrying is remembered per character and delivered by the next sweep, verified against a real inventory read first so anything you dealt with by hand is dropped from the list
> - New **Resume** button on the Roomba tab — carries on from a stopped sweep's queue without re-walking the whole circuit to rediscover what it already found. It's always visible and greys out when there's nothing to resume, and the queue is saved per character so it survives closing the client
> - The manifest survives an app restart or a relog, so it can't be defeated by whatever ended the sweep
> - Roomba stops collecting once the pack passes 80% of its carry budget and empties down below 40% before filling again, heaviest destination first. A saturated pack used to alternate deliver-one / collect-one and walk the same long leg twice for every single item while carrying dozens it never delivered
> - Roomba no longer collects items auto-discard is set to throw away — the two were fighting over the same loot every lap
> - If anything else empties your hands mid-sweep (auto-discard, a manual drop), Roomba forgets the item instead of planning a delivery for something it no longer holds. This mattered: the game matches a drop's name against what you actually carry, so a drop for a missing item could latch onto a different item you still had
> - `You may not drop that item!` now triggers the same inventory check as the currency-syntax refusal
> - A Roomba drop the game refuses with its currency-syntax complaint now triggers an inventory check: if the item genuinely isn't held the move is discarded instead of being retried every lap
> - Roomba sends its get/drop commands one per game prompt instead of dumping a whole room's batch at once, which was tripping the game's command-rate limit and getting the entire batch — plus the move that followed it — silently dropped
> - A command the game reports as dropped is re-sent after a short back-off rather than lost
> - Roomba handles a destination room being full: it reroutes everything bound there to the next room labelled for the same category, then the catch-all, instead of re-sending refused drops on every lap forever
> - A backup room is just a second room labelled for the same category — no new setting
> - Items with nowhere left to go are recorded with the reason rather than retried, and the sweep summary names the rooms that filled up
> - Full rooms are now prioritised as places to collect from, since clearing the out-of-place items in them is what frees the space
>
> See the [version history](CHANGELOG.md) for the full changelog.
<!-- current-version:end -->

A modern Telnet terminal client for **MajorMUD** and other BBS door games, built in C# / .NET 10 with [Avalonia](https://avaloniaui.net/). It renders a faithful CP437 cell grid with full VT100/ANSI parsing, and layers a MegaMUD-style automation suite (combat, party, navigation, healing, and more) on top — all in modeless, dockable windows so the terminal stays live while you configure anything.

Linux is the primary platform; Windows and macOS are supported through Avalonia.

## Features

- **Faithful terminal** — Telnet (RFC 854/855 with NAWS + TERM-TYPE), an explicit VT100/ANSI escape-sequence parser, and a CP437 cell grid rendered by a custom Avalonia control that scales crisply to fill the window. No host TTY dependency.
- **Connections & profiles** — per-character profiles layered over a 4-tier settings hierarchy (installed defaults → all characters → this BBS → this character, storing deltas only); multiple BBSes each with their own host/port/accounts; automated logon-menu navigation; and configurable redial/reconnect policies (connect-fail, carrier-lost, server-stall, post-cleanup).
- **Combat automation** — primary/alternate attack and spell settings, target ordering and priority, backstab handling, area/single-target debuff spells with an immunity-aware fallback cascade, crowd handling, rest-aware back-off, and per-monster attack/priority overrides.
- **Healing & spells** — HP/mana thresholds, rest management, cures, buffs, mana-regen roll-spell rerolling, a class-aware **Spell Book** (with per-spell cast-success odds and an interactive damage calculator), and a **Buff Watchdog** that shows your configured buffs, live timers, and recast-window markers at a glance.
- **Navigation & looping** — a room-graph map with go-to routing over saved GOTO locations (search box or right-click menu), storable/runnable loops with exp/hour estimates, an **Auto-Lair** mode, trap and hazard handling (obtain-then-cross and fewest-traps routing), stash rooms, and map overlays.
- **Party play** — party tracking, coordinated healing/blessing, leader-aware wait/invite logic, reconnect handling, and remote `@`-commands over chat channels — query (@health, @level, @exp, @inv…), move-me, change-my-settings, act-on-my-behalf, and party coordination — each gated by per-player permissions.
- **Cash & items** — automated loot collection with sell/buy/stash/discard engines, banking, and equipment sets with auto-equip triggers.
- **Character Workshop** — a unified hub for character management and development: live stats; **Equipment Manager** gear sets; an **Item Finder** with trial gearsets for what-if stat/encumbrance comparisons; **CP allocation** plans; **level projection**; quest, boss, and death tracking (with boss respawn timers you can sync between clients); character-info calculators; and **Roomba** — an automated gang-house item sorter backed by a shared item-location log you can query in-game with `@roomba`.
- **Automation tools** — macros, aliases, triggers, and events; auto-engine toggles with per-character base modes and reconnect reconciliation; a one-press all-off kill switch; and a Sprint mode.
- **Game data** — import MajorMUD `.MDB` databases, keep multiple game-data sets, and browse or override records across the 4-tier hierarchy in the Game Data Browser — whose **Monsters tab** curates the full roster with grouped min/max filters (combat stats, per-element resists signed to find vulnerabilities, spell immunity, magic-weapon requirement, type, and loot). Every engine reads from this data. A dedicated **Monster Intel** window answers "can I safely fight this thing right now" — Hits-You-% threshold checkboxes against your live AC/Dodge, an estimated rounds-to-kill per monster with your current weapon, plus its attacks and your own combat history against it — without digging into the Browser.
- **Conversation & chat** — a dedicated conversation pane with per-channel filtering, search, logging, and history.
- **Tools & diagnostics** — a timestamped full-ANSI scrollback with search/filter, a **Program Log**, **Session Stats**, a **Wire Inspector** for raw/classified stream inspection, and a ***built-in bug reporter (USE THIS WHEN REPORTING ISSUES — IT CAPTURES FAR MORE THAN YOU CAN DESCRIBE OR SHOW IN A SCREENSHOT)***.
- **Customization & quality of life** — an editable toolbar, fully rebindable keybinds, a customizable terminal right-click menu (add commands, direct links to a Workshop tab or calculator, and your own fly-out folders — rename, reorder, and import/export to share), edge-snapping windows that move together as a cluster, customizable navigation-line and font styling, output scaling, and type-through so keystrokes keep reaching the terminal while other windows are open.

## Getting started

### Requirements

- The [.NET 10 SDK](https://dotnet.microsoft.com/) (the exact version is pinned in `global.json`).

### Build & run

```bash
git clone https://github.com/Tehshortbus/MudPlay.git
cd MudPlay
dotnet build      # compile check
dotnet run        # launch
```

If local state ever gets weird, `dotnet clean` and rebuild.

### First connection

1. Launch the app and create a character profile (auth + which BBS to connect to).
2. Set the BBS host/port and connect.
3. For the full automation suite, open **Game Data** and import a MajorMUD `.MDB` database — this populates the monster/item/spell/room tables the engines read from. The terminal itself works without it.

### Where your data lives

Everything is stored under a single app-data folder, resolved per platform:

- **Linux** — `~/.local/share/MudPlay/`
- **Windows** — `%AppData%\MudPlay\`
- **macOS** — `~/Library/Application Support/MudPlay/`

Profiles, per-BBS settings, global settings, imported game data, and logs each live in their own subfolder. Settings files store only deltas from the tier beneath them, so they stay small and easy to back up. (Updating from an older build automatically lifts your data out of the previous nested `Data/` subfolder on first launch.)

## Reporting a bug

MudPlay has a **built-in bug reporter** that snapshots the client's state at the moment of the problem — far more useful than describing it from memory. Please use it when filing an issue:

1. **Capture** — click the **Bug Report** button in the menu bar (or right-click the terminal → **Bug report…**). Type a short description of what went wrong and confirm.
2. MudPlay writes a Markdown report to your **Desktop**, named `<realm>-<timestamp>.md`. It contains your player/inventory state, movement-engine status, relevant settings, the program log, and recent scrollback — with time-sensitive data frozen at click time.
3. **File the issue** — open a new issue at **https://github.com/Tehshortbus/MudPlay/issues/new**, describe the problem, and **attach the generated `.md` file**.

The bug report includes almost all of the info needed to isolate the problem but a good description helps me target it faster. You can review the bug report before submitting if you wish but please leave as much context in the report as possible. The bug report does include all your settings, your character name, stats, inventory, client info, the program log and ~750 lines of backscroll.  ***It DOES NOT include your BBS login name or password or your login menu navigation settings.***

## Contributing

- The build is **zero-warning** (`TreatWarningsAsErrors` + `EnforceCodeStyleInBuild`) and XAML bindings are compile-checked — a clean `dotnet build` is the baseline.
- `dotnet test` runs the xUnit suite (parsers, structural invariants, and critical decision logic).
- Coding conventions, architecture rules, and the per-change Definition of Done live in [`CLAUDE.md`](CLAUDE.md).

## License

MudPlay is licensed under the **MIT License** — see [`LICENSE`](LICENSE).

It bundles third-party components under their own licenses. The full text of each is viewable in-app under **Help → About**:

| Component | License |
|---|---|
| [Avalonia](https://avaloniaui.net/) | MIT |
| [JetDatabaseReader](https://github.com/diegoripera/JetDatabaseReader) | MIT |
| [JetBrains Mono](https://github.com/JetBrains/JetBrainsMono) font | SIL Open Font License 1.1 |
| [IBM Plex Sans](https://github.com/IBM/plex) font | SIL Open Font License 1.1 |
| [Px437 / Mx437 (Oldschool PC Fonts)](https://int10h.org/oldschool-pc-fonts/) | CC BY-SA 4.0 |
