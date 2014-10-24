using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace WPFLocales.Tool.ViewModels.Common
{
    internal class ViewModelBase: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        protected void RaisePropertyChanged(string propertyName)
        {
            var propertyChanged = this.PropertyChanged;

            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected bool Set<T>(ref T backingField, T value, Expression<Func<T>> propertyExpression)
        {
            var isChanged = !EqualityComparer<T>.Default.Equals(backingField, value);
            if (!isChanged) 
                return false;

            backingField = value;
            RaisePropertyChanged(ExtractPropertyName(propertyExpression));
            return true;
        }

        private static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExp = propertyExpression.Body as MemberExpression;

            if (memberExp == null)
            {
                throw new ArgumentException("Expression noc contains Property", "propertyExpression");
            }

            return memberExp.Member.Name;
        }
    }
}
