using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Epsiloner;
using Epsiloner.Collections;
using Epsiloner.Cooldowns;
using Epsiloner.Tasks;

namespace Sample_1
{
    public partial class MainWindow
    {
        private readonly EventCooldown _cooldown;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSingleTaskExecutor();

            _cooldown = new EventCooldown(
                TimeSpan.FromSeconds(2),
                Action,
                TimeSpan.FromSeconds(5));

            var i = 32;
            _runQueue = new RunQueue(() => DoNothing(i));
        }

        private void Action()
        {
            MessageBox.Show("Accumulated", "Test");
        }

        private void btnAccumulate_OnClick(object sender, RoutedEventArgs e)
        {
            _cooldown.Accumulate();
        }

        private void btnNow_OnClick(object sender, RoutedEventArgs e)
        {
            _cooldown.Now();
        }

        #region SingleTaskExecutor

        private readonly SingleTaskExecutor<DateTime> _singleTaskExecutor = new SingleTaskExecutor<DateTime>();
        public ObservableCollection<DateTime> Dates { get; } = new ObservableCollection<DateTime>();
        private uint index = 0;
        private RunQueue _runQueue;

        private void InitializeSingleTaskExecutor()
        {
            //_singleTaskExecutor.Task.ContinueWith(t =>
            //{
            //    Dispatcher.Invoke(() => Dates.Add(t.Result));
            //}, TaskContinuationOptions.OnlyOnRanToCompletion);
        }


        private void singleTaskExecutor_OnClick(object sender, RoutedEventArgs e)
        {
            _singleTaskExecutor.Next(token => DoAsyncWork(DateTime.Now, token));
        }

        private async Task<DateTime> DoAsyncWork(DateTime date, CancellationToken token)
        {
            await Task.Delay(2500, token);
            await Dispatcher.InvokeAsync(() => Dates.Add(date), DispatcherPriority.Normal, token);
            return date;
        }

        #endregion

        private void btnRunQueue_OnClick(object sender, RoutedEventArgs e)
        {
            _runQueue.Run();
        }

        private void DoNothing(int i)
        {
        }
    }
}
