using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Microsoft.Practices.Prism.Commands;
using QBR.Infrastructure.Interfaces;

namespace QBR.Utilities
{
    /// <summary>
    /// A Wrapper Class from ICommand that registers CommandManager.RequerySuggest Event.
    /// </summary>
    public static class DelegateCommandExtensions
    {
        public static DelegateCommand ListenOn<ObservedType, PropertyType>(this DelegateCommand delegateCommand,
            ObservedType observedObject,
            Expression<Func<ObservedType, PropertyType>> propertyExpression, 
            IDispatcherService dispatcherService = null)
            where ObservedType : INotifyPropertyChanged
        {
            var propetyName = PropertyHelpers.GetPropertyName(propertyExpression);

            observedObject.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == propetyName)
                {
                    if (dispatcherService != null)
                    {
                        dispatcherService.Dispatch(delegateCommand.RaiseCanExecuteChanged);
                    }
                    else
                    {
                        delegateCommand.RaiseCanExecuteChanged();
                    }
                }
            };
            return delegateCommand;
        }
    }
}
