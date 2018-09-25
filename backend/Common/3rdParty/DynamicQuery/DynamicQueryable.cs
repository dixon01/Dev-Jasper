// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicQueryable.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines static extensions to create dynamic Linq queries.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicQuery
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Defines static extensions to create dynamic Linq queries.
    /// </summary>
    public static class DynamicQueryable
    {
        /// <summary>
        /// Searches for a dynamic expression.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="values">The values.</param>
        /// <returns>The filtered query.</returns>
        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, params object[] values)
        {
            return (IQueryable<T>)Where((IQueryable)source, predicate, values);
        }

        /// <summary>
        /// Searches for a dynamic expression.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="values">The values.</param>
        /// <returns>The filtered query.</returns>
        public static IQueryable Where(this IQueryable source, string predicate, params object[] values)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            var lambda = DynamicExpression.ParseLambda(source.ElementType, typeof(bool), predicate, values);
            return
                source.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        "Where",
                        new Type[] { source.ElementType },
                        source.Expression,
                        Expression.Quote(lambda)));
        }

        /// <summary>
        /// Creates a dynamic selection..
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="values">The values.</param>
        /// <returns>The selection.</returns>
        public static IQueryable Select(this IQueryable source, string selector, params object[] values)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            var lambda = DynamicExpression.ParseLambda(source.ElementType, null, selector, values);
            return
                source.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        "Select",
                        new Type[] { source.ElementType, lambda.Body.Type },
                        source.Expression,
                        Expression.Quote(lambda)));
        }

        /// <summary>
        /// Orders the results with a dynamic expression.
        /// </summary>
        /// <typeparam name="T">The type of the entities.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="ordering">The ordering.</param>
        /// <param name="values">The values.</param>
        /// <returns>The ordered results.</returns>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, params object[] values)
        {
            return (IQueryable<T>)OrderBy((IQueryable)source, ordering, values);
        }

        /// <summary>
        /// Orders the results with a dynamic expression.
        /// </summary>
        /// <typeparam name="T">The type of the entities.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="ordering">The ordering.</param>
        /// <param name="values">The values.</param>
        /// <returns>The ordered results.</returns>
        public static IQueryable OrderBy(this IQueryable source, string ordering, params object[] values)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (ordering == null)
            {
                throw new ArgumentNullException("ordering");
            }

            var parameters = new ParameterExpression[] { Expression.Parameter(source.ElementType, string.Empty) };
            var parser = new ExpressionParser(parameters, ordering, values);
            var orderings = parser.ParseOrdering();
            var queryExpr = source.Expression;
            var methodAsc = "OrderBy";
            var methodDesc = "OrderByDescending";
            foreach (var o in orderings)
            {
                queryExpr = Expression.Call(
                    typeof(Queryable),
                    o.Ascending ? methodAsc : methodDesc,
                    new Type[] { source.ElementType, o.Selector.Type },
                    queryExpr,
                    Expression.Quote(Expression.Lambda(o.Selector, parameters)));
                methodAsc = "ThenBy";
                methodDesc = "ThenByDescending";
            }

            return source.Provider.CreateQuery(queryExpr);
        }

        /// <summary>
        /// Implements the Take operator for dynamic expressions.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="count">The count.</param>
        /// <returns>The query.</returns>
        public static IQueryable DynamicTake(this IQueryable source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return
                source.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        "Take",
                        new Type[] { source.ElementType },
                        source.Expression,
                        Expression.Constant(count)));
        }

        public static IQueryable DynamicSkip(this IQueryable source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            return
                source.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        "Skip",
                        new Type[] { source.ElementType },
                        source.Expression,
                        Expression.Constant(count)));
        }

        public static IQueryable GroupBy(
            this IQueryable source,
            string keySelector,
            string elementSelector,
            params object[] values)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (elementSelector == null) throw new ArgumentNullException("elementSelector");
            LambdaExpression keyLambda = DynamicExpression.ParseLambda(source.ElementType, null, keySelector, values);
            LambdaExpression elementLambda = DynamicExpression.ParseLambda(
                source.ElementType,
                null,
                elementSelector,
                values);
            return
                source.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        "GroupBy",
                        new Type[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type },
                        source.Expression,
                        Expression.Quote(keyLambda),
                        Expression.Quote(elementLambda)));
        }

        public static bool Any(this IQueryable source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return
                (bool)
                source.Provider.Execute(
                    Expression.Call(typeof(Queryable), "Any", new Type[] { source.ElementType }, source.Expression));
        }

        public static int Count(this IQueryable source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return
                (int)
                source.Provider.Execute(
                    Expression.Call(typeof(Queryable), "Count", new Type[] { source.ElementType }, source.Expression));
        }
    }
}