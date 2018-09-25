// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationObject.cs" company="">
//   
// </copyright>
// <summary>
//   The notification object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.UIFramework.Common.MVVM.ViewModelHelpers
{
   #region

    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    #endregion

   /// <summary>The notification object.</summary>
   public class NotificationObject : INotifyPropertyChanged
   {
      #region Public Events

      /// <summary>The property changed.</summary>
      public event PropertyChangedEventHandler PropertyChanged;

      #endregion

      #region Methods

      /// <summary>The raise property changed.</summary>
      /// <param name="action">The action.</param>
      /// <typeparam name="T"></typeparam>
      protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
      {
         var propertyName = GetPropertyName(action);
         this.RaisePropertyChanged(propertyName);
      }

      /// <summary>The get property name.</summary>
      /// <param name="action">The action.</param>
      /// <typeparam name="T"></typeparam>
      /// <returns>The <see cref="string"/>.</returns>
      private static string GetPropertyName<T>(Expression<Func<T>> action)
      {
         var expression = (MemberExpression)action.Body;
         var propertyName = expression.Member.Name;
         return propertyName;
      }

      /// <summary>The raise property changed.</summary>
      /// <param name="propertyName">The property name.</param>
      private void RaisePropertyChanged(string propertyName)
      {
         if (this.PropertyChanged != null)
         {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
         }
      }

      #endregion
   }
}