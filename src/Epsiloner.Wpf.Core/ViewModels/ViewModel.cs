using System.ComponentModel;
using System.Runtime.CompilerServices;
using Epsiloner.Wpf.Attributes;
using Epsiloner.Wpf.Utils;

namespace Epsiloner.Wpf.ViewModels
{
    public interface IViewModel : INotifyPropertyChanged
    {
    }

    /// <summary>
    /// Base view model.
    /// </summary>
    public abstract class ViewModel : IViewModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected ViewModel()
        {
            ViewModelUtil = new ViewModelUtil(this.GetType(), RaisePropertyChanged);
        }

        /// <summary>
        /// Util that actually contains whole logic for value set and notification of all related properties.
        /// </summary>
        protected ViewModelUtil ViewModelUtil { get; }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets new value for backing field and raises <see cref="INotifyPropertyChanged.PropertyChanged"/> event for <paramref name="propertyName"/> and all depending properties.
        /// Depending properties can be specified in param <paramref name="dependingPropertyNames"/> or via attribute <see cref="DependsOnAttribute"/> in same class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField">Backing field</param>
        /// <param name="newValue">new value</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="dependingPropertyNames">Depending properties</param>
        /// <returns></returns>
        protected bool Set<T>(ref T backingField, T newValue, [CallerMemberName] string propertyName = null, params string[] dependingPropertyNames)
        {
            return ViewModelUtil.Set(ref backingField, newValue, propertyName, dependingPropertyNames);
        }

        /// <summary>
        /// Raises <see cref="INotifyPropertyChanged.PropertyChanged"/> event for specified <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">Name of property which value has been changed.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
