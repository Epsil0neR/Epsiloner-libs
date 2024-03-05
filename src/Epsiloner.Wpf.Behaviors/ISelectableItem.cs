namespace Epsiloner.Wpf.Behaviors
{
    /// <summary>
    /// Item which can be selected by <see cref="KeyboardNavigationBehavior"/>.
    /// </summary>
    public interface ISelectableItem
    {
        /// <summary>
        /// Indicates that this item is currently selected.
        /// </summary>
        bool IsSelected { get; set; }
    }
}
