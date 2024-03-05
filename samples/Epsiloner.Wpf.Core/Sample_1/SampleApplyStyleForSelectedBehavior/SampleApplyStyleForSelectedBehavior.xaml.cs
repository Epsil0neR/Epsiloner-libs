using System;
using System.Windows;

namespace Sample_1.SampleApplyStyleForSelectedBehavior
{
    /// <summary>
    /// Interaction logic for SampleApplyStyleForSelectedBehavior.xaml
    /// </summary>
    public partial class SampleApplyStyleForSelectedBehavior : Window
    {
        public SampleApplyStyleForSelectedBehavior()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var dc = DataContext as ViewModel;
            if (dc == null)
                return;

            var v = DateTime.Now.ToString("O");
            dc.EmptyList.Add(v);
            dc.SelectedEmpty = v;
        }
    }
}
