using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Epsiloner.Collections
{
    /// <summary>
    /// Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed
    /// and provides handling when item added, removed or moved.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection</typeparam>
    public class ObservableCollection<T>
        : System.Collections.ObjectModel.ObservableCollection<T>
    {
        private readonly List<ItemHandlerDelegate<T>> _handlers = new List<ItemHandlerDelegate<T>>();
        private readonly object _smartReplaceLock = new object();

        #region Constructors
        public ObservableCollection()
        {
        }

        public ObservableCollection(List<T> list)
            : base(list)
        {
        }

        public ObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        ~ObservableCollection()
        {
            while (_handlers.Count > 0)
            {
                UnregisterHandler(_handlers[0], true);
            }
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Registers action for each added and removed item. 
        /// </summary>
        /// <param name="handler">Action which will be invoked for each added and removed item.</param>
        /// <param name="runForExistingItems">Indicated if <paramref name="handler"/> will be invoked for existing items.</param>
        public void RegisterHandler(ItemHandlerDelegate<T> handler, bool runForExistingItems = false)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _handlers.Add(handler);

            if (!runForExistingItems)
                return;

            var array = this.ToArray();
            for (var index = 0; index < array.Length; index++)
            {
                var item = array[index];
                RunHandlerForItem(handler, true, item, index);
            }
        }

        /// <summary>
        /// Unregisters action from each added and removed item.
        /// </summary>
        /// <param name="handler">Action which will be invoked for each added and removed item.</param>
        /// <param name="runForExistingItems">Indicated if <paramref name="handler"/> will be invoked for existing items.</param>
        public void UnregisterHandler(ItemHandlerDelegate<T> handler, bool runForExistingItems = false)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (!_handlers.Remove(handler) || !runForExistingItems)
                return;


            var array = this.ToArray();
            for (var index = 0; index < array.Length; index++)
            {
                var item = array[index];

                try
                {
                    RunHandlerForItem(handler, false, item, index);
                }
                catch {/* NEPP-1432 - Sometimes on finalizing throws exception in handler. */}
            }

        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="items">The items whose elements should be added to the end of the <see cref="ObservableCollection{T}"/>.
        ///  The collection itself cannot be <see langword="null" />, but it can contain elements that are <see langword="null" />, if type <typeparamref name="T"/> is a reference type.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="items" /> is <see langword="null" />.</exception>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            AddRangeItems(items);
        }

        /// <summary>
        /// Replaces all items in the <see cref="ObservableCollection{T}"/> with specified <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The object to replace all existing items in the <see cref="ObservableCollection{T}"/>. The value can be <see langword="null" /> for reference types.</param>
        public void Replace(T item)
        {
            ReplaceRangeItems(new[] { item });
        }

        /// <summary>
        /// Replaces all items in the <see cref="ObservableCollection{T}"/> with specified objects of the specified collection.
        /// </summary>
        /// <param name="items">
        /// The items whose elements will replace all existing items in the <see cref="ObservableCollection{T}"/>.
        /// The collection itself cannot be <see langword="null" />, but it can contain elements that are <see langword="null" />, if type <typeparamref name="T"/> is a reference type.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="items" /> is <see langword="null" />.</exception>
        public void ReplaceRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            ReplaceRangeItems(items);
        }

        /// <summary>
        /// Replaces all items in the <see cref="ObservableCollection{T}"/> with specified objects of the specified collection and raises events for all changes.
        /// Recommended for usage with UI and other parts that does not handle correctly when <see cref="NotifyCollectionChangedEventArgs.Action"/> is set to <see cref="NotifyCollectionChangedAction.Reset"/>.
        /// </summary>
        /// <param name="items"></param>
        public void ReplaceRangeSmart(IEnumerable<T> items)
        {
            lock (_smartReplaceLock)
            {
                var list = items?.ToList() ?? throw new ArgumentNullException(nameof(items));
                var removed = this.Except(list).ToList();
                var added = list.Except(this).ToList();

                foreach (var itm in removed)
                    while (Remove(itm)) { } // Perform removal in while as there same instance can be added multiple times.

                var skippedItems = 0;
                var c = list.Count;
                for (var i = 0; i < c; i++)
                {
                    var itm = list.ElementAt(i);
                    var ind = IndexOf(itm);
                    var indNew = i - skippedItems;
                    if (ind >= 0 && ind != indNew)
                        Move(ind, indNew);
                    else if (ind < 0) // Item not found in source.
                        skippedItems++;
                }

                foreach (var itm in added)
                    InsertItem(list.IndexOf(itm), itm);
            }
        }

        /// <summary>Removes a range of elements from the <see cref="ObservableCollection{T}" />.</summary>
        /// <param name="items">Collection of specified objects to remove.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="items" /> is <see langword="null" />.</exception>
        public void RemoveRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            RemoveRangeItems(items);
        }

        /// <summary>
        /// Thread safe operation to get copy list.
        /// </summary>
        /// <param name="maxTries">Maximum tries to create a copy of collection. 0 - infinity.</param>
        /// <returns></returns>
        public IList<T> ToListSafe(int maxTries = 10)
        {
            if (maxTries <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxTries));

            List<T> rv = null;
            var tryIndex = 0;
            do
            {
                try
                {
                    tryIndex++;
                    var copy = this.ToList();
                    rv = copy;
                }
                catch (Exception e)
                {
                    // Throw exception only if tried maximum allowed times and still no result.
                    if (maxTries > 0 && tryIndex >= maxTries)
                        throw e;

                    // Suggestion from Nerijus to wait 1ms.
                    Thread.Sleep(1);
                }
            } while (rv == null);

            return rv;
        }
        #endregion

        #region Protected Overrides
        //NOTE: MoveItem() does not causes insertion and deletion, so do not execute handlers for that method.

        protected virtual void AddRangeItems(IEnumerable<T> items)
        {
            CheckReentrancy();
            var itms = items.ToArray();

            var startIndex = Items.Count;
            for (var i = 0; i < itms.Length; i++)
            {
                Items.Insert(i + startIndex, itms[i]);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itms, startIndex));
            RaisePropertyChanged(nameof(Count));
            RaisePropertyChanged("Item[]");

            for (var index = 0; index < itms.Length; index++)
            {
                var itm = itms[index];
                RunAllHandlersForItem(true, itm, index);
            }
        }

        protected virtual void ReplaceRangeItems(IEnumerable<T> items)
        {
            CheckReentrancy();
            var itms = items.ToArray();
            var old = Items.ToArray();

            Items.Clear();

            for (var i = 0; i < itms.Length; i++)
            {
                Items.Insert(i, itms[i]);
            };

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            RaisePropertyChanged(nameof(Count));
            RaisePropertyChanged("Item[]");

            for (var index = 0; index < old.Length; index++)
            {
                var item = old[index];
                RunAllHandlersForItem(false, item, index);
            }

            for (var index = 0; index < itms.Length; index++)
            {
                var itm = itms[index];
                RunAllHandlersForItem(true, itm, index);
            }
        }

        protected virtual void RemoveRangeItems(IEnumerable<T> items)
        {
            CheckReentrancy();
            var itms = items.ToArray();

            var removed = itms.Where(x => Items.Remove(x)).ToList();

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            RaisePropertyChanged(nameof(Count));
            RaisePropertyChanged("Item[]");

            for (var index = 0; index < removed.Count; index++)
            {
                var item = removed[index];
                RunAllHandlersForItem(false, item, index);
            }
        }

        protected override void ClearItems()
        {
            var items = this.ToArray();

            base.ClearItems();

            for (var index = 0; index < items.Length; index++)
            {
                var item = items[index];
                RunAllHandlersForItem(false, item, index);
            }
        }

        protected override void RemoveItem(int index)
        {
            var hasItem = index >= 0 && index < Items.Count;
            var item = Items.ElementAtOrDefault(index);

            base.RemoveItem(index);

            if (hasItem)
                RunAllHandlersForItem(false, item, index);
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            RunAllHandlersForItem(true, item, index);
        }

        protected override void SetItem(int index, T item)
        {
            var hasItem = index >= 0 && index < Items.Count;
            var old = Items.ElementAtOrDefault(index);

            base.SetItem(index, item);
            //!EqualityComparer<T>.Default.Equals(item, old)) 
            if (!ReferenceEquals(item, old))
            {
                if (hasItem)
                    RunAllHandlersForItem(false, old, index);
                RunAllHandlersForItem(true, item, index);
            }
        }
        #endregion

        #region Private methods

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void RunAllHandlersForItem(bool inserted, T item, int index)
        {
            foreach (var handler in _handlers.ToArray())
            {
                RunHandlerForItem(handler, inserted, item, index);
            }
        }

        protected virtual void RunHandlerForItem(ItemHandlerDelegate<T> handler, bool inserted, T item, int index)
        {
            handler?.Invoke(inserted, item, index);
        }

        #endregion
    }
}
