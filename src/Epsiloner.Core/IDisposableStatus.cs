namespace Epsiloner
{
    /// <summary>
    /// Implementing interfaces knows about disposed status.
    /// </summary>
    public interface IDisposableStatus
    {
        /// <summary>
        /// Current disposed status, set once after dispose
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// If is currently disposing.
        /// </summary>
        bool IsDisposing { get; }
    }
}
