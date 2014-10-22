using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace WPFLocales.Utils
{
    internal static class VisualChildUtils
    {
        internal static IEnumerable<DependencyObject> EnumerateVisualChildren(this DependencyObject dependencyObject)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                yield return VisualTreeHelper.GetChild(dependencyObject, i);
            }
        }

        internal static IEnumerable<DependencyObject> EnumerateVisualChildrenRecoursively(this DependencyObject dependencyObject)
        {
            yield return dependencyObject;

            foreach (var child in dependencyObject.EnumerateVisualChildren())
            {
                foreach (var descendent in child.EnumerateVisualChildrenRecoursively())
                {
                    yield return descendent;
                }
            }
        }
    }
}
