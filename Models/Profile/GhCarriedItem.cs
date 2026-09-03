namespace MudPlay.Models.Profile;

// One item a Roomba sweep was still holding when it ended, and where it was
// taking it. JSON-friendly (public settable properties, "map/room" coordinate
// strings) because it round-trips through CharacterProfile — the point of the
// record is to outlive the sweep that made it, including an app restart.
//
// From is kept as well as To so a restored move still reports where the item
// came from in the sweep summary, exactly as an in-session move does.
public sealed class GhCarriedItem
{
    public string? From { get; set; }
    public string? To { get; set; }
    public string? Item { get; set; }
    public int Count { get; set; }

    public GhCarriedItem() { }

    public GhCarriedItem(string from, string to, string item, int count)
    {
        From = from;
        To = to;
        Item = item;
        Count = count;
    }
}
