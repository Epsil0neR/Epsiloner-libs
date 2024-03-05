namespace Epsiloner.Wpf.KeyBinding
{
    /// <summary>
    /// Manager update mode.
    /// </summary>
    public enum KeyBindingManagerUpdateMode
    {
        /// <summary>
        /// Can only update gestures for existing configs.
        /// </summary>
        OnlyUpdateExisting,

        /// <summary>
        /// Updates existing gestures and creates new.
        /// </summary>
        Full,
    }
}