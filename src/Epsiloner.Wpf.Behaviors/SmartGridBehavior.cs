using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Epsiloner.Cooldowns;
using Microsoft.Xaml.Behaviors;

namespace Epsiloner.Wpf.Behaviors
{
    /// <summary>
    /// Populates grid with specified items via <see cref="ContentControl"/> in equal columns.
    /// </summary>
    public class SmartGridBehavior : Behavior<Grid>
    {
        /// <summary>
        /// Items for <see cref="Grid"/>.
        /// </summary>
        public static DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(SmartGridBehavior),
            new PropertyMetadata(null, ItemsSourceChanged));

        /// <summary>
        /// Maximum items in single row. Null -> all items in 1 row.
        /// </summary>
        public static DependencyProperty MaxInRowProperty = DependencyProperty.Register(
            nameof(MaxInRow),
            typeof(int?),
            typeof(SmartGridBehavior),
            new PropertyMetadata(null, MaxInRowChanged));

        private readonly EventCooldown _cooldown;
        private bool _attached;

        /// <summary>
        /// (Dependency property) Items for <see cref="Grid"/>.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// (Dependency property) Maximum items in single row. Null -> all items in 1 row.
        /// </summary>
        public int? MaxInRow
        {
            get => (int?)GetValue(MaxInRowProperty);
            set => SetValue(MaxInRowProperty, value);
        }

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var s = (SmartGridBehavior)d;
            s?._cooldown.Now();
        }
        private static void MaxInRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var s = (SmartGridBehavior)d;
            s?._cooldown.Now();
        }

        /// <summary>
        /// Constructor for <see cref="SmartGridBehavior"/>.
        /// </summary>
        public SmartGridBehavior()
        {
            _cooldown = new EventCooldown(TimeSpan.FromMilliseconds(100), CooldownHandler);
        }

        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();
            _attached = true;
            CleanUp();
            if (ItemsSource is INotifyCollectionChanged c)
                c.CollectionChanged += OnCollectionChanged;
            _cooldown.Now();
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            _attached = false;
            CleanUp();
            if (ItemsSource is INotifyCollectionChanged c)
                c.CollectionChanged -= OnCollectionChanged;
            _cooldown.Now();
            base.OnDetaching();
        }

        private void CleanUp()
        {
            AssociatedObject?.ColumnDefinitions.Clear();
            AssociatedObject?.RowDefinitions.Clear();
            AssociatedObject?.Children.Clear();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _cooldown.Accumulate();
        }

        private List<ContentControl> _cached = new List<ContentControl>();

        private void CooldownHandler()
        {
            Dispatcher.Invoke(() =>
            {
                var items = ItemsSource?.Cast<object>().ToList() ?? new List<object>();
                int cols = items.Count;
                int rows = 0;

                if (MaxInRow.HasValue && MaxInRow > 0)
                {
                    rows = Math.DivRem(items.Count, MaxInRow.Value, out var rem);
                    if (rows > 0)
                    {
                        cols = MaxInRow.Value;
                        rows += (rem > 0 ? 1 : 0);
                    }
                }

                if (cols == 0 || !_attached)
                {
                    CleanUp();
                    CleanUpCache(items);
                    return;
                }

                if (rows != AssociatedObject.RowDefinitions.Count)
                {
                    var rd = AssociatedObject.RowDefinitions;
                    rd.Clear();
                    for (var i = 0; i < rows; i++)
                        rd.Add(new RowDefinition { Height = GridLength.Auto });
                }

                if (cols != AssociatedObject.ColumnDefinitions.Count)
                {
                    var cd = AssociatedObject.ColumnDefinitions;
                    cd.Clear();
                    for (int i = 0; i < cols; i++)
                        cd.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                }

                for (var index = 0; index < items.Count; index++)
                {
                    var item = items.ElementAtOrDefault(index);
                    var cc = GetOrCreateContentControl(item);

                    if (rows > 0)
                    {
                        var row = Math.DivRem(index, cols, out var col);
                        Grid.SetColumn(cc, col);
                        Grid.SetRow(cc, row);
                    }
                    else if (cols > 0)
                    {
                        Grid.SetColumn(cc, index);
                    }
                }

                CleanUpCache(items);
            });
        }

        /// <summary>
        /// Tries to find existing <see cref="ContentControl"/> for specified <paramref name="data"/> or create new one.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ContentControl GetOrCreateContentControl(object data)
        {
            lock (_cached)
            {
                var rv = _cached.FirstOrDefault(x => ReferenceEquals(x.Content, data));
                if (rv != null)
                    return rv;

                rv = new ContentControl
                {
                    Content = data
                };
                AssociatedObject.Children.Add(rv);

                _cached.Add(rv);
                return rv;
            }
        }

        /// <summary>
        /// Clean ups cache from removed items.
        /// </summary>
        /// <param name="current">Currently used items.</param>
        private void CleanUpCache(IEnumerable<object> current)
        {
            lock (_cached)
            {
                var toRemove = _cached.Where(x => !current.Contains(x.Content)).ToList();
                foreach (var control in toRemove)
                {
                    _cached.Remove(control);
                    AssociatedObject.Children.Remove(control);
                }
            }
        }
    }
}