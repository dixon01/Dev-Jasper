// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableReadOnlyDictionaryTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ObservableReadOnlyDictionaryTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Tests
{
    using System.Collections.Generic;

    using Gorba.Center.Common.ServiceModel.Collections;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the observable readonly dictionary.
    /// </summary>
    [TestClass]
    public class ObservableReadOnlyDictionaryTest
    {
        /// <summary>
        /// Tests that the PropertyChanged event is fired when a key is updated.
        /// It also verifies that the event is not fired when setting the same value.
        /// </summary>
        [TestMethod]
        public void TestPropertyChangedEvent()
        {
            var set = false;
            var dictionary =
                new ObservableReadOnlyDictionary<string, string>(new Dictionary<string, string> { { "key", "value" } });
            dictionary.PropertyChanged += (sender, args) => set = true;
            var value = dictionary["key"];
            Assert.AreEqual("value", value);

            // Set the same value: the event shouldn't be fired
            dictionary["key"] = "value";
            Assert.IsFalse(set);

            // Setting a different value: the event should be fired
            dictionary["key"] = "new value";
            Assert.IsTrue(set);
            value = dictionary["key"];
            Assert.AreEqual("new value", value);

            // Set the same (new)value: the event shouldn't be fired
            set = false;
            dictionary["key"] = "new value";
            Assert.IsFalse(set);
        }
    }
}