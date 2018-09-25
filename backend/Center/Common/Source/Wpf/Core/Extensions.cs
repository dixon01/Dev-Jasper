// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Definition of extension methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Core
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
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
                    Contract.Assert(unaryExpression != null);
                    memberExpression = unaryExpression.Operand as MemberExpression;
                }
                else
                {
                    memberExpression = lambda.Body as MemberExpression;
                }

                Contract.Assert(memberExpression != null);
                Contract.Assert(memberExpression.Member != null);
                string propertyName = memberExpression.Member.Name;
                handler(sender, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}