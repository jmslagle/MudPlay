using System.Collections.Generic;
using System.Linq;
using MudPlay.Game.Map;
using MudPlay.Models.Profile;
using MudPlay.Services;
using Xunit;

namespace MudPlay.Tests;

// The manifest exists so an interrupted sweep's load doesn't become the player's
// problem. Its whole value is surviving whatever ended the sweep, so these pin
// the persistence contract rather than any decision logic.
public sealed class GhCarryManifestStoreTests
{
    private static GhCarriedItem Item(string name, string from = "1/10", string to = "1/20", int count = 1)
        => new(from, to, name, count);

    [Fact]
    public void SavedLoadSurvivesIntoTheProfile()
    {
        ProfileService profile = new();
        profile.LoadBlank();
        GhCarryManifestStore store = new(profile);

        store.Save(new[] { Item("war hammer"), Item("mace", count: 3) });

        Assert.True(store.Any);
        IReadOnlyList<GhCarriedItem> loaded = store.Load();
        Assert.Equal(2, loaded.Count);
        Assert.Equal("war hammer", loaded[0].Item);
        Assert.Equal("1/20", loaded[0].To);
        Assert.Equal(3, loaded[1].Count);
        // Written through to the profile, which is what makes it outlive the app.
        Assert.Equal(2, profile.Current!.GhCarriedItems!.Count);
    }

    [Fact]
    public void SavingAnEmptyLoadClearsAPreviousOne()
    {
        // A clean sweep must clear the record, or its load would be re-delivered
        // by every later sweep forever.
        ProfileService profile = new();
        profile.LoadBlank();
        GhCarryManifestStore store = new(profile);
        store.Save(new[] { Item("war hammer") });

        store.Save(System.Array.Empty<GhCarriedItem>());

        Assert.False(store.Any);
        Assert.Empty(store.Load());
        Assert.Null(profile.Current!.GhCarriedItems);
    }

    [Fact]
    public void NoProfileLoaded_IsANoOp()
    {
        ProfileService profile = new();
        GhCarryManifestStore store = new(profile);

        store.Save(new[] { Item("war hammer") });

        Assert.False(store.Any);
        Assert.Empty(store.Load());
    }

    [Fact]
    public void LoadReturnsACopy_SoCallersCannotMutateTheStoredList()
    {
        ProfileService profile = new();
        profile.LoadBlank();
        GhCarryManifestStore store = new(profile);
        store.Save(new[] { Item("war hammer") });

        List<GhCarriedItem> loaded = store.Load().ToList();
        loaded.Clear();

        Assert.Single(store.Load());
    }
}
