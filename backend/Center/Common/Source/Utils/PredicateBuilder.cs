// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PredicateBuilder.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Extension methods used to build predicates.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Utils
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Extension methods used to build predicates.
    /// </summary>
    /// <remarks>
    /// Taken from http://www.albahari.com/nutshell/linqkit.aspx.
    /// Additional logical operators taken from:
    /// http://weblogs.asp.net/ricardoperes/archive/2010/12/14/basic-logical-operations-with-linq-expressions.aspx.
    /// </remarks>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    public static class PredicateBuilder
    {
        /// <summary>
        /// Returns a true expression.
        /// </summary>
        /// <typeparam name="T">Type of the parameter.</typeparam>
        /// <returns>A true expression.</returns>
        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }

        /// <summary>
        /// Returns a false expression.
        /// </summary>
        /// <typeparam name="T">Type of the parameter.</typeparam>
        /// <returns>A false expression.</returns>
        public static Expression<Func<T, bool>> False<T>()
        {
            return f => false;
        }

        /// <summary>
        /// Performs a logical OR between two expressions.
        /// </summary>
        /// <typeparam name="T">The type of the parameter.</typeparam>
        /// <param name="expr1">The expr1.</param>
        /// <param name="expr2">The expr2.</param>
        /// <returns>The evaluated expression.</returns>
        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// Performs a logical AND between two expressions.
        /// </summary>
        /// <typeparam name="T">The type of the parameter.</typeparam>
        /// <param name="expr1">The expr1.</param>
        /// <param name="expr2">The expr2.</param>
        /// <returns>The evaluated expression.</returns>
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// Xors the specified expr.
        /// </summary>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <param name="expr">The expr.</param>
        /// <param name="other">The other.</param>
        /// <returns>An expression.</returns>
        public static Expression<Func<T, bool>> Xor<T>(
            this Expression<Func<T, bool>> expr, Expression<Func<T, bool>> other)
        {
            var invokedExpr = Expression.Invoke(other, expr.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.ExclusiveOr(expr.Body, invokedExpr), expr.Parameters);
        }

        /// <summary>
        /// Nots the specified expr.
        /// </summary>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <param name="expr">The expr.</param>
        /// <returns>An expression.</returns>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Not(expr.Body), expr.Parameters);
        }
    }
}
