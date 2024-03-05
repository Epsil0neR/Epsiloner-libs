using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Epsiloner.Wpf.Collections;

namespace Sample_1.SampleApplyStyleForSelectedBehavior
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string _selected;
        private string _selectedEmpty;
        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> Items { get; }
        public System.Collections.ObjectModel.ObservableCollection<string> EmptyList { get; set; }

        public string Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }

        public string SelectedEmpty
        {
            get => _selectedEmpty;
            set
            {
                _selectedEmpty = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public ViewModel()
        {
            Items = new List<string>()
            {
                "One",
                "Two",
                "Three",
                "Four",
                "Five",
                "Six",
                "Seven",
                "Eight",
                "Nine",
                "Ten",
            };

            EmptyList = new System.Collections.ObjectModel.ObservableCollection<string>();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
