using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Markup;
using Epsiloner.Wpf.Converters;
using Epsiloner.Wpf.Serializers;

namespace Epsiloner.Wpf.KeyBinding
{
    /// <summary>
    /// Represents configuration for single named key binding.
    /// </summary>
    public sealed class KeyBindingConfig : INotifyPropertyChanged
    {
        private KeyGesture _gesture;
        private string _description;
        private string _name;
        private bool _isHidden;

        /// <summary>
        /// Config name.
        /// </summary>
        /// <exception cref="InvalidOperationException">Can be thrown if <see cref="IsLocked"/> set to <see cref="bool.True"/>.</exception>
        public string Name
        {
            get => _name;
            set
            {
                if (ReferenceEquals(_name, value))
                    return;

                if (IsLocked)
                    throw new InvalidOperationException("Gesture is locked.");

                _name = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Config description.
        /// </summary>
        /// <exception cref="InvalidOperationException">Can be thrown if <see cref="IsLocked"/> set to <see cref="bool.True"/>.</exception>
        public string Description
        {
            get => _description;
            set
            {
                if (ReferenceEquals(_description, value))
                    return;

                if (IsLocked)
                    throw new InvalidOperationException("Gesture is locked.");

                _description = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gesture associated with config.
        /// </summary>
        /// <exception cref="InvalidOperationException">Can be thrown if <see cref="IsLocked"/> set to <see cref="bool.True"/>.</exception>
        [TypeConverter(typeof(MultiKeyGestureConverter))]
        [ValueSerializer(typeof(MultiKeyGestureSerializer))]
        public KeyGesture Gesture
        {
            get => _gesture;
            set
            {
                if (ReferenceEquals(_gesture, value))
                    return;

                if (IsLocked)
                    throw new InvalidOperationException("Gesture is locked.");

                _gesture = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Indicates if config is locked and cannot be changed.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsLocked { get; private set; }

        /// <summary>
        /// Indicates if config is hidden from end-user.
        /// </summary>
        public bool IsHidden
        {
            get => _isHidden;
            set
            {
                if (_isHidden == value)
                    return;

                if (IsLocked)
                    throw new InvalidOperationException("Gesture is locked.");

                _isHidden = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Locks config and prevents it from edits.
        /// </summary>
        public void Lock()
        {
            IsLocked = true;
        }

        /// <summary>
        /// Clones config for edit mode or use only mode.
        /// </summary>
        /// <param name="lockStatus">Indicates what mode will have cloned config.</param>
        /// <returns></returns>
        public KeyBindingConfig Clone(bool lockStatus)
        {
            var rv = new KeyBindingConfig()
            {
                Name = Name,
                Description = Description,
                Gesture = Gesture,
                IsHidden = IsHidden,
                IsLocked = lockStatus,
            };
            return rv;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
