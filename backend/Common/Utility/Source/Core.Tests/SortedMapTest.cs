// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortedMapTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SortedMapTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="SortedMap{TKey,TValue}"/>.
    /// </summary>
    [TestClass]
    public class SortedMapTest
    {
        /// <summary>
        /// Unit test for <see cref="SortedMap{TKey,TValue}.Count"/>.
        /// </summary>
        [TestMethod]
        public void CountTest()
        {
            var target = new SortedMap<int, string>();
            target.Add(14, "fourteen");
            target.Add(9, "nine");
            target.Add(5, "five");

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(3, target.Keys.Count);
            Assert.AreEqual(3, target.Values.Count);
        }

        /// <summary>
        /// Unit test for <see cref="SortedMap{TKey,TValue}.Clear"/>.
        /// </summary>
        [TestMethod]
        public void ClearTest()
        {
            var target = new SortedMap<int, string>();
            target.Add(14, "fourteen");
            target.Add(9, "nine");
            target.Add(5, "five");

            target.Clear();

            Assert.AreEqual(0, target.Count);
            Assert.AreEqual(0, target.Keys.Count);
            Assert.AreEqual(0, target.Values.Count);
        }

        /// <summary>
        /// Unit test for <see cref="SortedMap{TKey,TValue}.ContainsKey"/>.
        /// </summary>
        [TestMethod]
        public void ContainsKeyTest()
        {
            var target = new SortedMap<int, string>();
            target.Add(14, "fourteen");
            target.Add(9, "nine");
            target.Add(5, "five");

            Assert.IsTrue(target.ContainsKey(14));
            Assert.IsTrue(target.ContainsKey(9));
            Assert.IsFalse(target.ContainsKey(7));
            Assert.IsTrue(target.ContainsKey(5));
        }

        /// <summary>
        /// Unit test for <see cref="SortedMap{TKey,TValue}.Remove"/>.
        /// </summary>
        [TestMethod]
        public void RemoveTest()
        {
            var target = new SortedMap<int, string>();
            target.Add(14, "fourteen");
            target.Add(9, "nine");
            target.Add(5, "five");

            Assert.AreEqual(3, target.Count);

            Assert.IsTrue(target.Remove(14));
            Assert.AreEqual(2, target.Count);

            Assert.IsTrue(target.Remove(9));
            Assert.AreEqual(1, target.Count);

            Assert.IsFalse(target.Remove(7));
            Assert.AreEqual(1, target.Count);

            Assert.IsTrue(target.Remove(5));
            Assert.AreEqual(0, target.Count);
        }

        /// <summary>
        /// Unit test for <see cref="SortedMap{TKey,TValue}.TryGetValue"/>.
        /// </summary>
        [TestMethod]
        public void TryGetValueTest()
        {
            var target = new SortedMap<int, string>();
            target.Add(14, "fourteen");
            target.Add(5, "five");
            target.Add(9, "nine");

            string value;
            Assert.IsTrue(target.TryGetValue(14, out value));
            Assert.AreEqual("fourteen", value);
            Assert.IsTrue(target.TryGetValue(9, out value));
            Assert.AreEqual("nine", value);
            Assert.IsFalse(target.TryGetValue(7, out value));
            Assert.IsNull(value);
            Assert.IsTrue(target.TryGetValue(5, out value));
            Assert.AreEqual("five", value);
        }

        /// <summary>
        /// Unit test for <see cref="SortedMap{TKey,TValue}.GetEnumerator"/>.
        /// </summary>
        [TestMethod]
        public void GetEnumeratorTest()
        {
            var target = new SortedMap<int, string>();
            target.Add(14, "fourteen");
            target.Add(9, "nine");
            target.Add(5, "five");
            target.Add(11, "eleven");
            target.Add(22, "twentytwo");
            target.Add(17, "seventeen");

            var enumerator = target.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            var pair = enumerator.Current;
            Assert.AreEqual(5, pair.Key);
            Assert.AreEqual("five", pair.Value);

            Assert.IsTrue(enumerator.MoveNext());
            pair = enumerator.Current;
            Assert.AreEqual(9, pair.Key);
            Assert.AreEqual("nine", pair.Value);

            Assert.IsTrue(enumerator.MoveNext());
            pair = enumerator.Current;
            Assert.AreEqual(11, pair.Key);
            Assert.AreEqual("eleven", pair.Value);

            Assert.IsTrue(enumerator.MoveNext());
            pair = enumerator.Current;
            Assert.AreEqual(14, pair.Key);
            Assert.AreEqual("fourteen", pair.Value);

            Assert.IsTrue(enumerator.MoveNext());
            pair = enumerator.Current;
            Assert.AreEqual(17, pair.Key);
            Assert.AreEqual("seventeen", pair.Value);

            Assert.IsTrue(enumerator.MoveNext());
            pair = enumerator.Current;
            Assert.AreEqual(22, pair.Key);
            Assert.AreEqual("twentytwo", pair.Value);

            Assert.IsFalse(enumerator.MoveNext());
        }

        /// <summary>
        /// Unit test for <see cref="SortedMap{TKey,TValue}.Keys"/> enumeration.
        /// </summary>
        [TestMethod]
        public void KeysGetEnumeratorTest()
        {
            var target = new SortedMap<int, string>();
            target.Add(14, "fourteen");
            target.Add(9, "nine");
            target.Add(22, "twentytwo");
            target.Add(17, "seventeen");
            target.Add(5, "five");
            target.Add(11, "eleven");

            var enumerator = target.Keys.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            var key = enumerator.Current;
            Assert.AreEqual(5, key);

            Assert.IsTrue(enumerator.MoveNext());
            key = enumerator.Current;
            Assert.AreEqual(9, key);

            Assert.IsTrue(enumerator.MoveNext());
            key = enumerator.Current;
            Assert.AreEqual(11, key);

            Assert.IsTrue(enumerator.MoveNext());
            key = enumerator.Current;
            Assert.AreEqual(14, key);

            Assert.IsTrue(enumerator.MoveNext());
            key = enumerator.Current;
            Assert.AreEqual(17, key);

            Assert.IsTrue(enumerator.MoveNext());
            key = enumerator.Current;
            Assert.AreEqual(22, key);

            Assert.IsFalse(enumerator.MoveNext());
        }

        /// <summary>
        /// Unit test for <see cref="SortedMap{TKey,TValue}.Values"/> enumeration.
        /// </summary>
        [TestMethod]
        public void ValuesGetEnumeratorTest()
        {
            var target = new SortedMap<int, string>();
            target.Add(11, "eleven");
            target.Add(22, "twentytwo");
            target.Add(9, "nine");
            target.Add(17, "seventeen");
            target.Add(14, "fourteen");
            target.Add(5, "five");

            var enumerator = target.Values.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            var value = enumerator.Current;
            Assert.AreEqual("five", value);

            Assert.IsTrue(enumerator.MoveNext());
            value = enumerator.Current;
            Assert.AreEqual("nine", value);

            Assert.IsTrue(enumerator.MoveNext());
            value = enumerator.Current;
            Assert.AreEqual("eleven", value);

            Assert.IsTrue(enumerator.MoveNext());
            value = enumerator.Current;
            Assert.AreEqual("fourteen", value);

            Assert.IsTrue(enumerator.MoveNext());
            value = enumerator.Current;
            Assert.AreEqual("seventeen", value);

            Assert.IsTrue(enumerator.MoveNext());
            value = enumerator.Current;
            Assert.AreEqual("twentytwo", value);

            Assert.IsFalse(enumerator.MoveNext());
        }

        /// <summary>
        /// Unit test for <see cref="SortedMap{TKey,TValue}.Keys"/> contains method.
        /// </summary>
        [TestMethod]
        public void KeysContainsTest()
        {
            var target = new SortedMap<int, string>();
            target.Add(14, "fourteen");
            target.Add(9, "nine");
            target.Add(5, "five");

            Assert.IsTrue(target.Keys.Contains(14));
            Assert.IsTrue(target.Keys.Contains(9));
            Assert.IsFalse(target.Keys.Contains(7));
            Assert.IsTrue(target.Keys.Contains(5));
        }

        /// <summary>
        /// Unit test for <see cref="SortedMap{TKey,TValue}.Keys"/> contains method.
        /// </summary>
        [TestMethod]
        public void ValuesContainsTest()
        {
            var target = new SortedMap<int, string>();
            target.Add(14, "fourteen");
            target.Add(9, "nine");
            target.Add(5, "five");

            Assert.IsTrue(target.Values.Contains("fourteen"));
            Assert.IsTrue(target.Values.Contains("nine"));
            Assert.IsFalse(target.Values.Contains("seven"));
            Assert.IsTrue(target.Values.Contains("five"));
        }
    }
}
