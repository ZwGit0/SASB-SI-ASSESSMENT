using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace SASB_SI_ASSESSMENT.Behaviors
{
    public static class AutoScrollBehavior
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(AutoScrollBehavior),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static void SetIsEnabled(DependencyObject element, bool value) =>
            element.SetValue(IsEnabledProperty, value);

        public static bool GetIsEnabled(DependencyObject element) =>
            (bool)element.GetValue(IsEnabledProperty);

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ListBox listBox) return;

            if ((bool)e.NewValue)
            {
                if (listBox.ItemsSource is INotifyCollectionChanged collection)
                {
                    collection.CollectionChanged += (s, args) =>
                    {
                        if (args.Action == NotifyCollectionChangedAction.Add && listBox.Items.Count > 0)
                        {
                            // Scroll newest log into view (index 0 if newest on top)
                            listBox.ScrollIntoView(listBox.Items[0]);
                        }
                    };
                }
            }
        }
    }
}
