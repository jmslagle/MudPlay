using System.Collections.Generic;
using System.Linq;
using MudPlay.Models.Profile;
using MudPlay.Services;

namespace MudPlay.Game.Map;

// What a Roomba sweep was still carrying when it stopped, persisted per
// character so the next sweep can finish the job.
//
// Without this, an aborted sweep leaves its whole load in the player's pack and
// takes the only record of where any of it belonged with it — the user is left
// holding a couple of dozen items and sorting them by hand. Persisting is what
// makes the record survive whatever killed the sweep, including an app restart
// or a relog; keeping it in memory would only cover the cases that were already
// the least painful.
//
// Deliberately NOT trusted on restore. The player may have dropped, sold or worn
// any of it in between, so the sweep verifies the manifest against a real
// inventory read before acting on it. Per-character (like GhManagedRoomStore),
// since the pack is the character's.
public sealed class GhCarryManifestStore
{
    private const string LogCategory = "GhSweep";

    private readonly ProfileService _profile;
    private readonly LogService? _log;

    public GhCarryManifestStore(ProfileService profile, LogService? log = null)
    {
        ArgumentNullException.ThrowIfNull(profile);
        _profile = profile;
        _log = log;
    }

    // Everything the last sweep left in the pack, oldest entry first. Empty when
    // the last sweep finished with empty hands — the normal case.
    public IReadOnlyList<GhCarriedItem> Load()
        => _profile.Current?.GhCarriedItems is { Count: > 0 } items
            ? items.ToList()
            : Array.Empty<GhCarriedItem>();

    public bool Any => _profile.Current?.GhCarriedItems is { Count: > 0 };

    // Record what's still in the pack. Called on every sweep end, normal or not:
    // a clean finish saves an empty list, which is what clears a previous one.
    public void Save(IEnumerable<GhCarriedItem> carried)
    {
        ArgumentNullException.ThrowIfNull(carried);
        if (_profile.Current is not { } current) return;

        List<GhCarriedItem> items = carried.ToList();
        bool had = current.GhCarriedItems is { Count: > 0 };
        if (items.Count == 0 && !had) return;   // nothing to write, nothing to clear

        current.GhCarriedItems = items.Count > 0 ? items : null;
        _profile.Save();

        if (items.Count > 0)
            _log?.Info(LogCategory,
                $"remembered {items.Count} item(s) still carried; the next sweep delivers them first");
        else if (had)
            _log?.Info(LogCategory, "carried-item manifest cleared — the pack is empty");
    }

    public void Clear() => Save(Array.Empty<GhCarriedItem>());
}
