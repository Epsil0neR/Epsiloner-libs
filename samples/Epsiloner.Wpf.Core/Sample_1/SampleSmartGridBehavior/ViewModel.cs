using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Epsiloner.Collections;

namespace Sample_1.SampleSmartGridBehavior
{
    public class ViewModel : Epsiloner.Wpf.ViewModels.ViewModel
    {
        public ObservableCollection<TextBlock> TextBlocks { get; }
        public ReadOnlyObservableCollection<TextBlock> Items { get; }

        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand ClearCommand { get; }

        public ViewModel()
        {
            TextBlocks = new ObservableCollection<TextBlock>();

            for (var i = 1; i <= 1000; i++)
                TextBlocks.Add(new TextBlock { Text = i.ToString() });

            Items = new ReadOnlyObservableCollection<TextBlock>(TextBlocks);
            AddCommand = new RelayCommand(Add);
            RemoveCommand = new RelayCommand(Remove);
            ClearCommand = new RelayCommand(Clear);
        }

        private void Add(object obj)
        {
            TextBlocks.Add(new TextBlock { Text = (TextBlocks.Count + 1).ToString() });
        }

        private void Remove(object obj)
        {
            var itm = TextBlocks.LastOrDefault();
            TextBlocks.Remove(itm);
        }

        private void Clear(object obj)
        {
            TextBlocks.Clear();
        }
    }
}
