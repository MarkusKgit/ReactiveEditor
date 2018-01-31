using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ReactiveEditor.Helpers
{
    public static class VisualHelpers
    {
        public static Canvas GetItemsCanvas(DependencyObject dep)
        {
            var itemsControl = VisualHelpers.GetVisualParent<ItemsControl>(dep);
            var itemsPresenter = VisualHelpers.GetVisualChild<Canvas>(itemsControl);
            return itemsPresenter;
        }

        public static T GetVisualParent<T>(DependencyObject child) where T : Visual
        {
            var parent = VisualTreeHelper.GetParent(child);
            if (parent == null)
                return null;
            if (parent is T)
                return parent as T;
            else
                return GetVisualParent<T>(parent);
        }

        public static T GetVisualChild<T>(DependencyObject parent) where T : Visual
        {
            T child = default(T);

            var numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                var v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }
}