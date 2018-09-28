﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhereExtensions.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WhereExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Access
{
    using System;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    using System.Linq.Expressions;

    using Gorba.Center.Common.ServiceModel.Filters;

    using DynamicExpression = DynamicQuery.DynamicExpression;
    using StringComparison = Gorba.Center.Common.ServiceModel.Filters.StringComparison;

    /// <summary>
    /// Extensions to apply filters to an <see cref="ObjectQuery{T}"/>.
    /// </summary>
    public static partial class WhereExtensions
    {
        /// <summary>
        /// Applies the given <paramref name="filter"/> to the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="filter">
        /// The filter to apply, can be null - in which case this method does nothing.
        /// </param>
        /// <param name="prefix">
        /// The prefix to add before any WHERE clause.
        /// </param>
        /// <param name="property">
        /// The property for which the filter is to be applied.
        /// </param>
        /// <typeparam name="T">
        /// The type of <see cref="ObjectQuery{T}"/>.
        /// </typeparam>
        /// <returns>
        /// The <see cref="ObjectQuery{T}"/> with the additional querying for the given <paramref name="filter"/>.
        /// </returns>
        public static ObjectQuery<T> Where<T>(
            this ObjectQuery<T> query, Int32PropertyValueFilter filter, string prefix, string property)
        {
            if (filter == null)
            {
                return query;
            }

            string where;
            switch (filter.Comparison)
            {
                case Int32Comparison.ExactMatch:
                    where = string.Format("{0}{1}.Equals(@0)", prefix, property);
                    break;
                case Int32Comparison.Different:
                    where = string.Format("!{0}{1}.Equals(@0)", prefix, property);
                    break;
                case Int32Comparison.GreaterThan:
                    where = string.Format("{0}{1} > @0", prefix, property);
                    break;
                case Int32Comparison.GreaterThanOrEqualTo:
                    where = string.Format("{0}{1} >= @0", prefix, property);
                    break;
                case Int32Comparison.LessThan:
                    where = string.Format("{0}{1} < @0", prefix, property);
                    break;
                case Int32Comparison.LessThanOrEqualTo:
                    where = string.Format("{0}{1} <= @0", prefix, property);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return query.ApplyObjectQuery(where, filter.Value);
        }

        /// <summary>
        /// Applies the given <paramref name="filter"/> to the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="filter">
        /// The filter to apply, can be null - in which case this method does nothing.
        /// </param>
        /// <param name="prefix">
        /// The prefix to add before any WHERE clause.
        /// </param>
        /// <param name="property">
        /// The property for which the filter is to be applied.
        /// </param>
        /// <typeparam name="T">
        /// The type of <see cref="ObjectQuery{T}"/>.
        /// </typeparam>
        /// <returns>
        /// The <see cref="ObjectQuery{T}"/> with the additional querying for the given <paramref name="filter"/>.
        /// </returns>
        public static ObjectQuery<T> Where<T>(
            this ObjectQuery<T> query, Int64PropertyValueFilter filter, string prefix, string property)
        {
            if (filter == null)
            {
                return query;
            }

            string where;
            switch (filter.Comparison)
            {
                case Int64Comparison.ExactMatch:
                    where = string.Format("{0}{1}.Equals(@0)", prefix, property);
                    break;
                case Int64Comparison.Different:
                    where = string.Format("!{0}{1}.Equals(@0)", prefix, property);
                    break;
                case Int64Comparison.GreaterThan:
                    where = string.Format("{0}{1} > @0", prefix, property);
                    break;
                case Int64Comparison.GreaterThanOrEqualTo:
                    where = string.Format("{0}{1} >= @0", prefix, property);
                    break;
                case Int64Comparison.LessThan:
                    where = string.Format("{0}{1} < @0", prefix, property);
                    break;
                case Int64Comparison.LessThanOrEqualTo:
                    where = string.Format("{0}{1} <= @0", prefix, property);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return query.ApplyObjectQuery(where, filter.Value);
        }

        /// <summary>
        /// Applies the given <paramref name="filter"/> to the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="filter">
        /// The filter to apply, can be null - in which case this method does nothing.
        /// </param>
        /// <param name="prefix">
        /// The prefix to add before any WHERE clause.
        /// </param>
        /// <param name="property">
        /// The property for which the filter is to be applied.
        /// </param>
        /// <typeparam name="T">
        /// The type of <see cref="ObjectQuery{T}"/>.
        /// </typeparam>
        /// <returns>
        /// The <see cref="ObjectQuery{T}"/> with the additional querying for the given <paramref name="filter"/>.
        /// </returns>
        public static ObjectQuery<T> Where<T>(
            this ObjectQuery<T> query, DateTimePropertyValueFilter filter, string prefix, string property)
        {
            if (filter == null)
            {
                return query;
            }

            string where;
            switch (filter.Comparison)
            {
                case DateTimeComparison.ExactMatch:
                    where = string.Format("{0}{1}.Equals(@0)", prefix, property);
                    break;
                case DateTimeComparison.Different:
                    where = string.Format("!{0}{1}.Equals(@0)", prefix, property);
                    break;
                case DateTimeComparison.GreaterThan:
                    where = string.Format("{0}{1} > @0", prefix, property);
                    break;
                case DateTimeComparison.GreaterThanOrEqualTo:
                    where = string.Format("{0}{1} >= @0", prefix, property);
                    break;
                case DateTimeComparison.LessThan:
                    where = string.Format("{0}{1} < @0", prefix, property);
                    break;
                case DateTimeComparison.LessThanOrEqualTo:
                    where = string.Format("{0}{1} <= @0", prefix, property);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return query.ApplyObjectQuery(where, filter.Value);
        }

        /// <summary>
        /// Applies the given <paramref name="filter"/> to the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="filter">
        /// The filter to apply, can be null - in which case this method does nothing.
        /// </param>
        /// <param name="prefix">
        /// The prefix to add before any WHERE clause.
        /// </param>
        /// <param name="property">
        /// The property for which the filter is to be applied.
        /// </param>
        /// <typeparam name="T">
        /// The type of <see cref="ObjectQuery{T}"/>.
        /// </typeparam>
        /// <returns>
        /// The <see cref="ObjectQuery{T}"/> with the additional querying for the given <paramref name="filter"/>.
        /// </returns>
        public static ObjectQuery<T> Where<T>(
            this ObjectQuery<T> query, NullableDateTimePropertyValueFilter filter, string prefix, string property)
        {
            if (filter == null)
            {
                return query;
            }

                string where;
            if (filter.Value.HasValue)
            {
                switch (filter.Comparison)
                {
                    case NullableDateTimeComparison.ExactMatch:
                        where = string.Format("{0}{1}.HasValue && {1}.Value.Equals(@0)", prefix, property);
                        break;
                    case NullableDateTimeComparison.Different:
                        where = string.Format("{0}{1}.HasValue && !{1}.Value.Equals(@0)", prefix, property);
                        break;
                    case NullableDateTimeComparison.GreaterThan:
                        where = string.Format("{0}{1}.HasValue && {1}.Value > @0", prefix, property);
                        break;
                    case NullableDateTimeComparison.GreaterThanOrEqualTo:
                        where = string.Format("{0}{1}.HasValue && {1}.Value >= @0", prefix, property);
                        break;
                    case NullableDateTimeComparison.LessThan:
                        where = string.Format("{0}{1}.HasValue && {1}.Value < @0", prefix, property);
                        break;
                    case NullableDateTimeComparison.LessThanOrEqualTo:
                        where = string.Format("{0}{1}.HasValue && {1}.Value <= @0", prefix, property);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return query.ApplyObjectQuery(where, filter.Value);
            }

            switch (filter.Comparison)
            {
                case NullableDateTimeComparison.ExactMatch:
                    where = string.Format("{0}!{1}.HasValue", prefix, property);
                    break;
                case NullableDateTimeComparison.Different:
                    where = string.Format("{0}{1}.HasValue", prefix, property);
                    break;
                case NullableDateTimeComparison.GreaterThan:
                case NullableDateTimeComparison.GreaterThanOrEqualTo:
                case NullableDateTimeComparison.LessThan:
                case NullableDateTimeComparison.LessThanOrEqualTo:
                    throw new ArgumentException(
                            "Only ExactMatch and Different are supported if the specified value is null");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return query.Where(where);
        }

        /// <summary>
        /// Applies the given <paramref name="filter"/> to the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="filter">
        /// The filter to apply, can be null - in which case this method does nothing.
        /// </param>
        /// <param name="prefix">
        /// The prefix to add before any WHERE clause.
        /// </param>
        /// <param name="property">
        /// The property for which the filter is to be applied.
        /// </param>
        /// <typeparam name="T">
        /// The type of <see cref="ObjectQuery{T}"/>.
        /// </typeparam>
        /// <returns>
        /// The <see cref="ObjectQuery{T}"/> with the additional querying for the given <paramref name="filter"/>.
        /// </returns>
        public static ObjectQuery<T> Where<T>(
            this ObjectQuery<T> query, StringPropertyValueFilter filter, string prefix, string property)
        {
            if (filter == null)
            {
                return query;
            }

            string where;
            var value = filter.Value;
            switch (filter.Comparison)
            {
                case StringComparison.ExactMatch:
                    where = string.Format(
                        value == null ? "{0}{1} == null" : "{0}{1} != null && {1}.Equals(@0)",
                        prefix,
                        property);

                    break;
                case StringComparison.Different:
                    where = string.Format(
                        value == null ? "{0}{1} != null" : "{0}{1} != null && !{1}.Equals(@0)",
                        prefix,
                        property);

                    break;
                case StringComparison.CaseInsensitiveMatch:
                    if (value == null)
                    {
                        where = string.Format("{0}{1} == null", prefix, property);
                    }
                    else
                    {
                        where = string.Format("{0}{1} != null && {1}.ToLower().Equals(@0)", prefix, property);
                        value = value.ToLower();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return ApplyObjectQuery(query, where, value);
        }

        /// <summary>
        /// Applies the given <paramref name="filter"/> to the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="filter">
        /// The filter to apply, can be null - in which case this method does nothing.
        /// </param>
        /// <param name="prefix">
        /// The prefix to add before any WHERE clause.
        /// </param>
        /// <param name="property">
        /// The property for which the filter is to be applied.
        /// </param>
        /// <typeparam name="T">
        /// The type of <see cref="ObjectQuery{T}"/>.
        /// </typeparam>
        /// <typeparam name="TEnum">
        /// The type of enum used in the filter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="ObjectQuery{T}"/> with the additional querying for the given <paramref name="filter"/>.
        /// </returns>
        public static ObjectQuery<T> Where<T, TEnum>(
            this ObjectQuery<T> query, EnumPropertyValueFilter filter, string prefix, string property)
        {
            if (filter == null)
            {
                return query;
            }

            string where;
            switch (filter.Comparison)
            {
                case EnumComparison.ExactMatch:
                    where = string.Format("{0}{1} == @0", prefix, property);
                    break;
                case EnumComparison.Different:
                    where = string.Format("{0}{1} != @0", prefix, property);
                    break;
                default:
                    throw new NotSupportedException(
                        "Only ExactMatch and Different are supported by enumerations");
            }

            return query.ApplyObjectQuery(where, (TEnum)(object)filter.Value);
        }

        /// <summary>
        /// Applies the given <paramref name="filter"/> to the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="filter">
        /// The filter to apply, can be null - in which case this method does nothing.
        /// </param>
        /// <param name="prefix">
        /// The prefix to add before any WHERE clause.
        /// </param>
        /// <param name="property">
        /// The property for which the filter is to be applied.
        /// </param>
        /// <typeparam name="T">
        /// The type of <see cref="ObjectQuery{T}"/>.
        /// </typeparam>
        /// <returns>
        /// The <see cref="ObjectQuery{T}"/> with the additional querying for the given <paramref name="filter"/>.
        /// </returns>
        public static ObjectQuery<T> Where<T>(
            this ObjectQuery<T> query, GuidPropertyValueFilter filter, string prefix, string property)
        {
            if (filter == null)
            {
                return query;
            }

            string where;
            switch (filter.Comparison)
            {
                case GuidComparison.ExactMatch:
                    where = string.Format("{0}{1}.Equals(@0)", prefix, property);
                    break;
                case GuidComparison.Different:
                    where = string.Format("!{0}{1}.Equals(@0)", prefix, property);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return query.ApplyObjectQuery(where, filter.Value);
        }

        /// <summary>
        /// Applies the given <paramref name="filter"/> to the given <paramref name="query"/>.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="filter">
        /// The filter to apply, can be null - in which case this method does nothing.
        /// </param>
        /// <param name="prefix">
        /// The prefix to add before any WHERE clause.
        /// </param>
        /// <param name="property">
        /// The property for which the filter is to be applied.
        /// </param>
        /// <typeparam name="T">
        /// The type of <see cref="ObjectQuery{T}"/>.
        /// </typeparam>
        /// <returns>
        /// The <see cref="ObjectQuery{T}"/> with the additional querying for the given <paramref name="filter"/>.
        /// </returns>
        public static ObjectQuery<T> Where<T>(
            this ObjectQuery<T> query, BooleanPropertyValueFilter filter, string prefix, string property)
        {
            if (filter == null)
            {
                return query;
            }

            string where;
            switch (filter.Comparison)
            {
                case BooleanComparison.ExactMatch:
                    where = string.Format("{0}{1} != null && {1}.Equals(@0)", prefix, property);
                    break;
                case BooleanComparison.Different:
                    where = string.Format("{0}{1} != null && !{1}.Equals(@0)", prefix, property);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return query.ApplyObjectQuery(where, filter.Value);
        }

        private static ObjectQuery<T> ApplyObjectQuery<T>(this ObjectQuery<T> query, string where, object value)
        {
            var source = (IQueryable)query;
            var lambda = DynamicExpression.ParseLambda(source.ElementType, typeof(bool), where, value);
            return (ObjectQuery<T>)source.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        "Where",
                        new[] { source.ElementType },
                        source.Expression,
                        Expression.Quote(lambda)));
        }
    }
}