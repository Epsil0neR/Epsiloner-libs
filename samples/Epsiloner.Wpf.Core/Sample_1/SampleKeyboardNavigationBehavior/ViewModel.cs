using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace Sample_1.SampleKeyboardNavigationBehavior
{
    public class ViewModel : Epsiloner.Wpf.ViewModels.ViewModel
    {
        private string _selected;

        public List<string> Items { get; }
        public ICommand ActivateCommand { get; }

        public string Selected
        {
            get => _selected;
            set
            {
                Set(ref _selected, value, nameof(Selected), nameof(Test));
            }
        }

        [DependsOn(nameof(ViewModel.Selected))]
        public string Test => $"({Selected}) {Items.Count}";

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

            ActivateCommand = new RelayCommand(Activate);

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propName = e.PropertyName;
            // When `Selected` changes -> `Test` also should notify that `Test` changed.
        }

        private void Activate(object obj)
        {
            MessageBox.Show("Activating: " + obj);
        }
    }
}
