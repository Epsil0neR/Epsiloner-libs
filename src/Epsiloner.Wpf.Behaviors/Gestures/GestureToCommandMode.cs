namespace Epsiloner.Wpf.Behaviors.Gestures
{
    /// <summary>
    /// Indicates when gesture will hook input.
    /// </summary>
    public enum GestureToCommandMode
    {
        /// <summary>
        /// Gesture will handle gesture before others elements handles.
        /// </summary>
        Before,

        /// <summary>
        /// Gesture will handle gesture after others elements handles.
        /// </summary>
        After,
    }
}