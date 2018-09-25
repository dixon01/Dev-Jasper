// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Definition of extension methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.WpfCore
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    /// <summary>
    /// Definition of extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        public static void RaisePropertyChangedEvent(
            this PropertyChangedEventHandler handler, object sender, Expression<Func<object>> expression)
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

                if (memberExpression != null)
                {
                    string propertyName = memberExpression.Member.Name;
                    handler(sender, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}