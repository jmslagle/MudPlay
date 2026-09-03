namespace MudPlay.Game.Map;

// Why a Roomba sweep left a floor item where it was, for surfacing in GhSweepReport
// / the GH Management tab so "left in place" isn't an undifferentiated bucket.
public enum GhLeftReason
{
    // No labeled room matched the item and there was no catch-all room.
    NoMatchingRoom,
    // A `get` for it failed because it was gone by sort time (decayed / taken).
    GoneBySortTime,
    // Too heavy to ever carry within the working encumbrance budget, so no
    // delivery could free enough room to move it.
    TooHeavy,
    // Every room that could hold it — each labeled match, then the catch-all —
    // refused the drop as full. Requeueing it would just retry the same wall, so
    // it waits for the next sweep, by which time the rooms may have space.
    AllDestinationsFull,
}
