using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace WPFLocales.View
{
    [MarkupExtensionReturnType(typeof(string))]
    public class LocalizableText : MarkupExtension
    {
        private readonly bool _isInDesignMode;
        private readonly HashSet<DependencyObject> _targetObjects;
        private DependencyProperty _targetProperty;

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

        public LocalizableText()
        {
            _isInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
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

            if (targetObject == null || targetProperty == null)
                throw new NotSupportedException("LocalizableText supported only for dependency properties");

            if (!_targetObjects.Contains(targetObject))
            {
                _targetObjects.Add(targetObject);
            }
            if (_targetObjects.Count == 1)
            {
                _targetProperty = targetProperty;

                Locales.CurrentLocaleChanged += OnLocalizationCurrentLocaleChanged;
            }

            var text = _key == null ? "Key not set yet" : GetTextByKey();

            return text;
        }

        private void OnLocalizationCurrentLocaleChanged()
        {
            UpdateTarget();
        }

        private void UpdateTarget()
        {
            if (_key == null || _targetProperty == null)
                return;

            foreach (var targetObject in _targetObjects)
            {
                var text = _isInDesignMode ? Locales.GetTextByLocaleKey(targetObject, _key) : Locales.GetTextByLocaleKey(_key);

                targetObject.SetValue(_targetProperty, text);
            }
        }

        private string GetTextByKey()
        {
            return _isInDesignMode ? Locales.GetTextByLocaleKey(_targetObjects.First(), _key) : Locales.GetTextByLocaleKey(_key);
        }
    }
}