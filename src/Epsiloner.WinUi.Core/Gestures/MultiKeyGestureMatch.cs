namespace Epsiloner.WinUi.Gestures;

public enum MultiKeyGestureMatch
{
    /// <summary>
    /// Input does not match at all.
    /// </summary>
    NoMatch,

    /// <summary>
    /// Matches single item in sequence of gestures.
    /// Has more items in sequence to filter.
    /// </summary>
    MatchNotFinal,

    /// <summary>
    /// Fully matches item or last item in sequence.
    /// </summary>
    Matches,
}