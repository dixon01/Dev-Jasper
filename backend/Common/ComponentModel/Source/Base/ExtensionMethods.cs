// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.ComponentModel.Base
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    /// <summary>
    /// Defines extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="expression">The expression.</param>
        public static void RaisePropertyChangedEvent(this PropertyChangedEventHandler handler, object sender, Expression<Func<object>> expression)
        {
            if (handler != null)
            {
                var lambda = expression as LambdaExpression;
                MemberExpression memberExpression;
                if (lambda.Body is UnaryExpression)
                {
                    var unaryExpression = lambda.Body as UnaryExpression;
                    memberExpression = unaryExpression.Operand as MemberExpression;
                }
                else
                {
                    memberExpression = lambda.Body as MemberExpression;
                }

                var constantExpression = memberExpression.Expression as ConstantExpression;
                string propertyName = memberExpression.Member.Name;
                handler(sender, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
