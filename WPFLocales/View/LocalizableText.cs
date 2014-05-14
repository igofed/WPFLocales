using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace WPFLocales.View
{
    [MarkupExtensionReturnType(typeof(string))]
    public class LocalizableText : MarkupExtension
    {
        private readonly HashSet<DependencyObject> _targetObjects;
        private DependencyProperty _targetProperty;
        private readonly static bool IsDesignMode;

        public Enum Key
        {
            get
            {
                return _key;
            }
            set
            {
                if (value == null)
                    return;

                _key = value;

                UpdateTarget();

            }
        }
        private Enum _key;

        static LocalizableText()
        {
            IsDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }

        public LocalizableText()
        {
            _targetObjects = new HashSet<DependencyObject>();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var providerValuetarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            if (providerValuetarget.TargetObject.GetType().FullName == "System.Windows.SharedDp")
            {
                return this;
            }

            var targetObject = providerValuetarget.TargetObject as DependencyObject;
            var targetProperty = providerValuetarget.TargetProperty as DependencyProperty;
            if (targetObject != null && targetProperty != null)
            {
                _targetProperty = targetProperty;

                if (!_targetObjects.Contains(targetObject))
                {
                    _targetObjects.Add(targetObject);
                }
            }

            var text = "Key not set yet";
            if (_key == null)
                return text;
            
            if (IsDesignMode)
            {
                if(_targetObjects.Count == 1)
                { 
                    Localization.DesignTimeLocaleChanged += OnLocalizationDesignTimeLocaleChanged;
                }

                text = Localization.GetTextByKey(targetObject, _key);
            }
            else
            {
                if (_targetObjects.Count == 1)
                {
                    Localization.LocaleChanged += OnLocalizationLocaleChanged;
                }
                text = Localization.GetTextByKey(_key);
            }

            return text;
        }

        private void OnLocalizationDesignTimeLocaleChanged()
        {
            UpdateTarget();
        }

        private void OnLocalizationLocaleChanged()
        {
            UpdateTarget();
        }

        private void UpdateTarget()
        {
            if (_key == null || _targetProperty == null)
                return;

            foreach (var targetObject in _targetObjects)
            {
                var text = IsDesignMode
                    ? Localization.GetTextByKey(targetObject, _key)
                    : Localization.GetTextByKey(_key);

                targetObject.SetValue(_targetProperty, text);
            }
        }
    }
}