using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epsiloner.Cooldowns
{
    /// <summary>
    /// Same as EventCooldown&gt;T&lt; but keeps list of all values and fires into accumulate event.
    /// </summary>
    /// <typeparam name="T">type of element</typeparam>
    public class EventCooldownListOf<T> : IEventCooldown<T>
    {
        private readonly List<T> _items;
        private readonly object _itemsListPadlock = new object();
        private int _inActionCount;
        private readonly Action<List<T>> _action;
        private readonly bool _cleanAfterAccumulateInvoke;
        private readonly EventCooldown _eventCooldown;


        /// <summary>
        /// Accumulates event values into list and after <paramref name="accumulateAfter"/> period invokes <paramref name="action"/>.
        /// </summary>
        /// <param name="action">action to invoke after <paramref name="accumulateAfter"/></param>
        /// <param name="accumulateAfter">invoke <paramref name="action"/> after <paramref name="accumulateAfter"/> of silence</param>
        /// <param name="cleanAfterAccumulateInvoke">flag to clean list of items after <paramref name="action"/>. in case of true, values will be always accumulating in list, even previous ones.</param>
        public EventCooldownListOf(
            Action<List<T>> action,
            TimeSpan accumulateAfter,
            bool cleanAfterAccumulateInvoke = true)
        : this(action, accumulateAfter, cleanAfterAccumulateInvoke, null)
        { }

        /// <summary>
        /// Accumulates event values into list and after <paramref name="accumulateAfter"/> period invokes <paramref name="action"/>.
        /// </summary>
        /// <param name="action">action to invoke after <paramref name="accumulateAfter"/></param>
        /// <param name="accumulateAfter">invoke <paramref name="action"/> after <paramref name="accumulateAfter"/> of silence</param>
        /// <param name="cleanAfterAccumulateInvoke">flag to clean list of items after <paramref name="action"/>. in case of true, values will be always accumulating in list, even previous ones.</param>
        /// <param name="maxAccumulateAfter">(Optional) Maximum timespan after first event execute action.</param>
        public EventCooldownListOf(
        Action<List<T>> action,
        TimeSpan accumulateAfter,
        bool cleanAfterAccumulateInvoke = true,
        TimeSpan? maxAccumulateAfter = null)
        {
            _action = action;
            _cleanAfterAccumulateInvoke = cleanAfterAccumulateInvoke;
            _items = new List<T>();
            _eventCooldown = new EventCooldown(accumulateAfter, Accumulated, maxAccumulateAfter);
        }

        /// <inheritdoc />
        public string LastStackTrace => _eventCooldown.LastStackTrace;

        /// <inheritdoc />
        public bool IsNow => _eventCooldown.IsNow;

        /// <summary>
        /// Cleans collected list from passing to next accumulation.
        /// </summary>
        public void ClearList()
        {
            lock (_itemsListPadlock)
            {
                _items.Clear();
            }
        }

        /// <summary>
        /// Clears collected items that matches predicate from passing to next accumulation.
        /// </summary>
        /// <param name="match"></param>
        public void ClearList(Predicate<T> match)
        {
            lock (_itemsListPadlock)
            {
                _items.RemoveAll(match);
            }
        }

        /// <summary>
        /// Filters collected items that will pass to next accumulation.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public List<T> ContainingItems(Predicate<T> match)
        {
            lock (_itemsListPadlock)
            {
                return _items.Where(x => match(x)).ToList();
            }
        }


        /// <inheritdoc />
        public void Now()
        {
            _eventCooldown.Now();
        }

        /// <inheritdoc />
        public void Now(T value)
        {
            Add(value);
            Now();
        }

        /// <inheritdoc />
        public void NowAsync(T value)
        {
            Task.Factory.StartNew(() => Now(value));
        }

        /// <inheritdoc />
        public void Accumulate(T value)
        {
            if (Add(value))
                _eventCooldown.Accumulate();
        }

        /// <inheritdoc />
        public bool KeepLastStackTrace
        {
            get => _eventCooldown.KeepLastStackTrace;
            set => _eventCooldown.KeepLastStackTrace = value;
        }

        /// <inheritdoc />
        public void Cancel()
        {
            _eventCooldown.Cancel();
        }

        public bool Any()
        {
            lock (_itemsListPadlock)
            {
                return _items.Any();
            }
        }

        public Func<List<T>, T, bool> AddPredicate { get; set; } = (list, item) => true;

        private bool Add(T value)
        {
            if (value == null)
                return false;

            lock (_itemsListPadlock)
            {
                if (AddPredicate(_items, value))
                    _items.Add(value);
                else
                    return false;
            }

            return true;
        }


        private void Accumulated()
        {
            if (_inActionCount > 0)
                return;

            try
            {
                _inActionCount++;

                List<T> items;
                lock (_itemsListPadlock)
                {
                    items = new List<T>(_items);
                    if (_cleanAfterAccumulateInvoke)
                        _items.Clear();
                }

                _action.Invoke(items);
            }
            finally
            {
                _inActionCount = 0;
            }
        }
    }
}