namespace ARSoft.Tools.Net
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal static class LinqExtension
    {
        public delegate TResult Func<T, TResult>(T arg);

        public static IEnumerable<TSource> OrderBy<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            var list = source.ToList();
            list.Sort((a, b) => keySelector(a).CompareTo(keySelector(b)));
            return list;
        }

        public static IEnumerable<TSource> OrderByDescending<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            var list = source.ToList();
            list.Sort((a, b) => -keySelector(a).CompareTo(keySelector(b)));
            return list;
        }

        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    return true;
                }
            }

            return false;
        }

        public static IEnumerable<TSource> Where<TSource>(
            this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var list = new List<TSource>();
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    list.Add(item);
                }
            }

            return list;
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.AsList().ConvertAll(i => selector(i));
        }

        public static TSource First<TSource>(this IEnumerable<TSource> source)
        {
            var en = source.GetEnumerator();
            if (!en.MoveNext())
            {
                throw new InvalidOperationException();
            }

            return en.Current;
        }

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            var en = source.GetEnumerator();
            if (!en.MoveNext())
            {
                return default(TSource);
            }

            return en.Current;
        }

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return source.Where(predicate).FirstOrDefault();
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            var list = new List<TResult>();
            foreach (var item in source)
            {
                list.AddRange(selector(item));
            }

            return list;
        }

        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var map = new Dictionary<TKey, Grouping<TKey, TSource>>();
            var result = new List<IGrouping<TKey, TSource>>();
            foreach (var item in source)
            {
                var key = keySelector(item);
                Grouping<TKey, TSource> grouping;
                if (!map.TryGetValue(key, out grouping))
                {
                    grouping = new Grouping<TKey, TSource>(key);
                    map[key] = grouping;
                    result.Add(grouping);
                }

                grouping.Elements.Add(item);
            }

            return result;
        }

        public static List<T> ToList<T>(this IEnumerable<T> source)
        {
            return new List<T>(source);
        }

        public static T[] ToArray<T>(this IEnumerable<T> source)
        {
            return source.AsList().ToArray();
        }

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            var firstEnum = first.GetEnumerator();
            var secondEnum = second.GetEnumerator();
            while (firstEnum.MoveNext() && secondEnum.MoveNext())
            {
                if (!Equals(firstEnum.Current, secondEnum.Current))
                {
                    return false;
                }
            }

            return !firstEnum.MoveNext() && !secondEnum.MoveNext();
        }

        public static int Min(this IEnumerable<int> source)
        {
            int min = int.MaxValue;
            bool found = false;
            foreach (var item in source)
            {
                min = Math.Min(min, item);
                found = true;
            }

            if (!found)
            {
                throw new InvalidOperationException();
            }

            return min;
        }

        public static ushort Max(this IEnumerable<ushort> source)
        {
            ushort max = ushort.MinValue;
            bool found = false;
            foreach (var item in source)
            {
                max = Math.Max(max, item);
                found = true;
            }

            if (!found)
            {
                throw new InvalidOperationException();
            }

            return max;
        }

        public static int Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            return source.Select(selector).Min();
        }

        public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            return source.Select(selector).Sum();
        }

        public static int Sum(this IEnumerable<int> source)
        {
            int value = 0;
            foreach (var item in source)
            {
                value += item;
            }

            return value;
        }

        public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
        {
            var list = source.ToList();
            list.Reverse();
            return list;
        }

        public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var count = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    count++;
                }
            }

            return count;
        }

        public static int Count<TSource>(this IEnumerable<TSource> source)
        {
            var count = 0;
            var en = source.GetEnumerator();
            while (en.MoveNext())
            {
                count++;
            }

            return count;
        }

        public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
        {
            var list = new List<TResult>();
            foreach (var item in source)
            {
                list.Add((TResult)item);
            }

            return list;
        }

        public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
        {
            var list = new List<TResult>();
            foreach (var item in source)
            {
                if (item is TResult)
                {
                    list.Add((TResult)item);
                }
            }

            return list;
        }

        private static List<T> AsList<T>(this IEnumerable<T> source)
        {
            var list = source as List<T>;
            if (list != null)
            {
                return list;
            }

            return new List<T>(source);
        }

        public interface IGrouping<TKey, TElement> : IEnumerable<TElement>
        {
            TKey Key { get; }
        }

        private class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
        {
            public Grouping(TKey key)
            {
                this.Elements = new List<TElement>();
                this.Key = key;
            }

            public TKey Key { get; private set; }

            public List<TElement> Elements { get; private set; }

            public IEnumerator<TElement> GetEnumerator()
            {
                return this.Elements.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.Elements.GetEnumerator();
            }
        }
    }
}
