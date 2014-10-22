using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace WPFLocales.View
{
    [MarkupExtensionReturnType(typeof(string))]
    public class LocalizableText : MarkupExtension
    {
        /// <summary>
        /// Localization key for current text
        /// </summary>
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


        internal Control DesignLocaleParent { get; set; }
       

        private readonly bool _isInDesignMode;
        private readonly HashSet<DependencyObject> _targetObjects;
        private DependencyProperty _targetProperty;
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

            _targetProperty = targetProperty;
            if (!_targetObjects.Contains(targetObject))
            {
                _targetObjects.Add(targetObject);
            }

            if(_isInDesignMode)
                Localization.RegisterLocalizableTextForElement(this, targetObject);
            else if(_targetObjects.Count == 1)
                Localization.CurrentLocaleChanged += OnLocalizationCurrentLocaleChanged;
            
            return _key == null ? "Key not set yet" : GetTextByKey();
        }


        internal void UpdateTarget()
        {
            if (_key == null || _targetProperty == null)
                return;

            foreach (var targetObject in _targetObjects)
            {
                var text = GetTextByKey();

                targetObject.SetValue(_targetProperty, text);
            }
        }

        private void OnLocalizationCurrentLocaleChanged()
        {
            UpdateTarget();
        }

        private string GetTextByKey()
        {
            return _isInDesignMode ? Localization.GetTextByLocalizationKey(DesignLocaleParent, _key) : Localization.GetTextByLocalizationKey(_key);
        }
    }
}