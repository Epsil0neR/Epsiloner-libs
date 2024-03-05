using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Epsiloner.Collections
{
    /// <summary>
    /// Represents a read-only <see cref="ObservableCollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class ReadOnlyObservableCollection<T>
        : System.Collections.ObjectModel.ReadOnlyObservableCollection<T>
    {
        private readonly ObservableCollection<T> _observableCollectionWrap;

        public ReadOnlyObservableCollection(ObservableCollection<T> list) : base(list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            _observableCollectionWrap = list;
        }

        /// <inheritdoc />
        public new event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { base.CollectionChanged += value; }
            remove { base.CollectionChanged -= value; }
        }

        #region Public methods

        /// <summary>
        /// Registers action for each added and removed item. 
        /// </summary>
        /// <param name="handler">Action which will be invoked for each added and removed item.</param>
        /// <param name="runForExistingItems">Indicated if <paramref name="handler"/> will be invoked for existing items.</param>
        public void RegisterHandler(ItemHandlerDelegate<T> handler, bool runForExistingItems = false)
        {
            _observableCollectionWrap.RegisterHandler(handler, runForExistingItems);
        }

        /// <summary>
        /// Unregisters action from each added and removed item.
        /// </summary>
        /// <param name="handler">Action which will be invoked for each added and removed item.</param>
        /// <param name="runForExistingItems">Indicated if <paramref name="handler"/> will be invoked for existing items.</param>
        public void UnregisterHandler(ItemHandlerDelegate<T> handler, bool runForExistingItems = false)
        {
            _observableCollectionWrap.UnregisterHandler(handler, runForExistingItems);
        }
        #endregion

        public TR ReadLock<TR>(Func<ReadOnlyObservableCollection<T>, TR> action)
        {
            return action(this);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}