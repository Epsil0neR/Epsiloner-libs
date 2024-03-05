using System.Timers;

namespace Epsiloner.Cooldowns
{
    public interface IEventCooldown<in T>
    {
        /// <summary>
        /// Executes event with no cooldown. 
        /// </summary>
        void Now();

        /// <summary>
        /// Executes event with no cooldown. 
        /// </summary>
        void Now(T value);

        /// <summary>
        /// Executes event with no cooldown asynchronously.
        /// </summary>
        /// <returns></returns>
        void NowAsync(T value);

        /// <summary>
        /// Puts event in cooldown. 
        /// In case no more events comes then OnElapsed will be called.
        /// </summary>
        void Accumulate(T value);

        /// <summary>
        /// For debugging purposes keeps last stack trace of execution. 
        /// Effective only in DEBUG release mode. 
        /// </summary>
        bool KeepLastStackTrace { get; set; }

        /// <summary>
        /// Provides stack trace of last <see cref="Accumulate"/> and <see cref="Now()"/> and <see cref="Now(T)"/>.
        /// </summary>
        string LastStackTrace { get; }

        /// <summary>
        /// Was last cooldown called by now?
        /// </summary>
        bool IsNow { get; }

        /// <summary>
        /// Cancels accumulation.
        /// </summary>
        void Cancel();

        /// <summary>
        /// in there any items queued?
        /// </summary>
        /// <returns></returns>
        bool Any();
    }
}
