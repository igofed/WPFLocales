using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFLocales.View;

namespace WPFLocales.Utils
{
    internal static class BindingExtensions
    {
        internal static IEnumerable<DependencyProperty> GetDependencyProperties(this DependencyObject dependencyObject)
        {
            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(dependencyObject, new Attribute[] { new PropertyFilterAttribute(PropertyFilterOptions.All) }))
            {
                var dpd = DependencyPropertyDescriptor.FromProperty(pd);

                if (dpd != null)
                {
                    yield return dpd.DependencyProperty;
                }
            }
        }

        internal static void EnumElementsWithProperties(this DependencyObject dependencyObject, Action<DependencyObject, DependencyProperty> process)
        {
            foreach (var element in dependencyObject.EnumerateVisualChildrenRecoursively())
            {
                foreach (var  property in element.GetDependencyProperties())
                {
                    process(element, property);
                }
            }
        }


        internal static void UpdateBindingTargets(this DependencyObject dependencyObject)
        {
            dependencyObject.EnumElementsWithProperties((element, property) =>
            {
                var bindingExpression = BindingOperations.GetBindingExpressionBase(element, property);
                if (bindingExpression != null)
                {
                    bindingExpression.UpdateTarget();
                }
            });
        }

        internal static void UpdateBindingConverterParents(this Control dependencyObject)
        {
            dependencyObject.EnumElementsWithProperties((element, property) =>
            {
                var binding = BindingOperations.GetBinding(element, property);
                if (binding != null)
                {
                    var converter = binding.Converter;
                    if (converter is LocalizableConverter)
                    {
                        (converter as LocalizableConverter).Parent = element;
                        (converter as LocalizableConverter).DesignLocaleParent = dependencyObject;
                    }
                }
                else
                {
                    var multiBinding = BindingOperations.GetMultiBinding(element, property);
                    if (multiBinding != null)
                    {
                        var converter = multiBinding.Converter;
                        if (converter is LocalizableConverter)
                        {
                            (converter as LocalizableConverter).Parent = element;
                            (converter as LocalizableConverter).DesignLocaleParent = dependencyObject;
                        }
                    }
                }
            });
        }

        internal static void UpdateBindingTargets(this Application application)
        {
            foreach (var window in application.Windows)
            {
                if (window is DependencyObject)
                    (window as DependencyObject).UpdateBindingTargets();
            }
        }
    }
}
