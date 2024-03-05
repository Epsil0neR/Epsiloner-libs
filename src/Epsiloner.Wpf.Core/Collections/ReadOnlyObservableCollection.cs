using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Epsiloner.Wpf.Collections
{
    /// <summary>
    /// Represents a read-only collection with custom dispatcher host for events.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class ReadOnlyObservableCollection<T>
        : Epsiloner.Collections.ReadOnlyObservableCollection<T>
    {
        private System.Windows.Threading.Dispatcher _dispatcher;

        #region Properties
        /// <summary>
        /// Dispatcher that hosts events like <see cref="System.Collections.ObjectModel.ReadOnlyObservableCollection{T}.CollectionChanged"/> and <see cref="System.Collections.ObjectModel.ReadOnlyObservableCollection{T}.PropertyChanged"/>.
        /// Can be different from <see cref="ObservableCollection{T}.Dispatcher"/>.
        /// If empty, events are raised on same thread that modified collection.
        /// </summary>
        public System.Windows.Threading.Dispatcher Dispatcher
        {
            get => _dispatcher;
            set
            {
                if (value == _dispatcher)
                    return;

                _dispatcher = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        /// <inheritdoc />
        public ReadOnlyObservableCollection(Epsiloner.Collections.ObservableCollection<T> list)
            : base(list)
        {
        }

        /// <summary>
        /// Raises the <see cref="System.Collections.ObjectModel.ReadOnlyObservableCollection{T}.PropertyChanged"/> event with the provided arguments on configured <see cref="Dispatcher"/>.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (_dispatcher != null && !_dispatcher.HasShutdownFinished)
            {
                if (_dispatcher.CheckAccess())
                    base.OnPropertyChanged(e);
                else
                    _dispatcher.InvokeAsync(() => base.OnPropertyChanged(e));
            }
            else
            {
                base.OnPropertyChanged(e);
            }
        }


        /// <summary>
        /// Raises the <see cref="System.Collections.ObjectModel.ReadOnlyObservableCollection{T}.CollectionChanged"/> event with the provided arguments on configured <see cref="Dispatcher"/>.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_dispatcher != null && !_dispatcher.HasShutdownFinished)
            {
                if (_dispatcher.CheckAccess())
                    base.OnCollectionChanged(e);
                else
                    _dispatcher.InvokeAsync(() => base.OnCollectionChanged(e));
            }
            else if (_dispatcher == null)
            {
                base.OnCollectionChanged(e);
            }
        }
    }
}