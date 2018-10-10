// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BecChangingPropertiesTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BecChangingPropertiesTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transcoder.Bec
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using Gorba.Common.Medi.Core.Tests.Transcoder.Bec.Utils;
    using Gorba.Common.Medi.Core.Transcoder.Bec;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test to verify that BEC supports objects with more/less properties.
    /// </summary>
    [TestClass]
    public class BecChangingPropertiesTest : BecAppDomainTestBase
    {
        /// <summary>
        /// This method just tests that the framework set up to test changing BEC properties actually works.
        /// </summary>
        [TestMethod]
        public void SimpleSerializeDeserializeTest()
        {
            using (var testerA = this.CreateTester("A"))
            {
                var typeA = testerA.CreateClass(
                        "This.Is.My.TestObject", new PropertyDescription("PropA", typeof(string)));
                foreach (BecCodecConfig.SerializerType serializerType in
                    Enum.GetValues(typeof(BecCodecConfig.SerializerType)))
                {
                    testerA.Configure(serializerType);
                    var objA = testerA.CreateObject(typeA);
                    objA.SetPropertyValue("PropA", "Test");
                    var result = objA.GetPropertyValue("PropA");
                    Assert.AreEqual("Test", result);

                    var serialized = testerA.Serialize("A", objA);
                    var deserialized = testerA.Deserialize("A", serialized, 0, serialized.Length);
                    Assert.IsNotNull(deserialized);
                    result = deserialized.GetPropertyValue("PropA");
                    Assert.AreEqual("Test", result);
                }
            }
        }

        /// <summary>
        /// This method tests that properties can be reordered.
        /// </summary>
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        [TestMethod]
        public void ReorderPropertiesTest()
        {
            var testerA = this.CreateTester("A");
            var testerB = this.CreateTester("B");

            try
            {
                var typeA = testerA.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeof(string)),
                    new PropertyDescription("PropB", typeof(FileAccess)),
                    new PropertyDescription("PropC", typeof(double)),
                    new PropertyDescription("PropD", typeof(int)));
                var typeB = testerB.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeof(string)),
                    new PropertyDescription("PropD", typeof(int)),
                    new PropertyDescription("PropC", typeof(double)),
                    new PropertyDescription("PropB", typeof(FileAccess)));

                foreach (var serializerTypes in this.GetSerializerTypes())
                {
                    testerA.Configure(serializerTypes.Item1);
                    testerB.Configure(serializerTypes.Item2);

                    for (int i = 0; i < 3; i++)
                    {
                        var objA = testerA.CreateObject(typeA);

                        objA.SetPropertyValue("PropA", "Test" + i);
                        objA.SetPropertyValue("PropB", FileAccess.ReadWrite);
                        objA.SetPropertyValue("PropC", 0.777 * i);
                        objA.SetPropertyValue("PropD", i);
                        var result = objA.GetPropertyValue("PropA");
                        Assert.AreEqual("Test" + i, result);
                        result = objA.GetPropertyValue("PropB");
                        Assert.AreEqual(FileAccess.ReadWrite, result);
                        result = objA.GetPropertyValue("PropC");
                        Assert.AreEqual(0.777 * i, result);
                        result = objA.GetPropertyValue("PropD");
                        Assert.AreEqual(i, result);

                        var objB = testerB.CreateObject(typeB);

                        objB.SetPropertyValue("PropA", "Hello" + i);
                        objB.SetPropertyValue("PropB", FileAccess.Read);
                        objB.SetPropertyValue("PropC", 3.55 * i);
                        objB.SetPropertyValue("PropD", i + 1);
                        result = objB.GetPropertyValue("PropA");
                        Assert.AreEqual("Hello" + i, result);
                        result = objB.GetPropertyValue("PropB");
                        Assert.AreEqual(FileAccess.Read, result);
                        result = objB.GetPropertyValue("PropC");
                        Assert.AreEqual(3.55 * i, result);
                        result = objB.GetPropertyValue("PropD");
                        Assert.AreEqual(i + 1, result);

                        var serialized = testerA.Serialize("B", objA);
                        var deserialized = testerB.Deserialize("A", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Test" + i, result);
                        result = deserialized.GetPropertyValue("PropB");
                        Assert.AreEqual(FileAccess.ReadWrite, result);
                        result = deserialized.GetPropertyValue("PropC");
                        Assert.AreEqual(0.777 * i, result);
                        result = deserialized.GetPropertyValue("PropD");
                        Assert.AreEqual(i, result);

                        serialized = testerB.Serialize("A", objB);
                        deserialized = testerA.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Hello" + i, result);
                        result = deserialized.GetPropertyValue("PropB");
                        Assert.AreEqual(FileAccess.Read, result);
                        result = deserialized.GetPropertyValue("PropC");
                        Assert.AreEqual(3.55 * i, result);
                        result = deserialized.GetPropertyValue("PropD");
                        Assert.AreEqual(i + 1, result);
                    }
                }
            }
            finally
            {
                testerA.Dispose();
                testerB.Dispose();
            }
        }

        /// <summary>
        /// This method tests that a string property can be added.
        /// </summary>
        [TestMethod]
        public void AddStringPropertyTest()
        {
            var testerA = this.CreateTester("A");
            var testerB = this.CreateTester("B");

            try
            {
                var typeA = testerA.CreateClass(
                        "This.Is.My.TestObject", new PropertyDescription("PropA", typeof(string)));
                var typeB = testerB.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeof(string)),
                    new PropertyDescription("PropB", typeof(string)));

                foreach (var serializerTypes in this.GetSerializerTypes())
                {
                    testerA.Configure(serializerTypes.Item1);
                    testerB.Configure(serializerTypes.Item2);

                    for (int i = 0; i < 3; i++)
                    {
                        var objA = testerA.CreateObject(typeA);

                        objA.SetPropertyValue("PropA", "Test" + i);
                        var result = objA.GetPropertyValue("PropA");
                        Assert.AreEqual("Test" + i, result);

                        var objB = testerB.CreateObject(typeB);

                        objB.SetPropertyValue("PropA", "Foo" + i);
                        objB.SetPropertyValue("PropB", "Bar" + i);
                        result = objB.GetPropertyValue("PropA");
                        Assert.AreEqual("Foo" + i, result);
                        result = objB.GetPropertyValue("PropB");
                        Assert.AreEqual("Bar" + i, result);

                        var serialized = testerA.Serialize("B", objA);
                        var deserialized = testerB.Deserialize("A", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Test" + i, result);
                        result = deserialized.GetPropertyValue("PropB");
                        Assert.IsNull(result);

                        serialized = testerB.Serialize("A", objB);
                        deserialized = testerA.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Foo" + i, result);
                    }
                }
            }
            finally
            {
                testerA.Dispose();
                testerB.Dispose();
            }
        }

        /// <summary>
        /// This method tests that an int property can be added.
        /// </summary>
        [TestMethod]
        public void AddIntPropertyTest()
        {
            var testerA = this.CreateTester("A");
            var testerB = this.CreateTester("B");

            try
            {
                var typeA = testerA.CreateClass(
                        "This.Is.My.TestObject", new PropertyDescription("PropA", typeof(string)));
                var typeB = testerB.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeof(string)),
                    new PropertyDescription("PropB", typeof(int)));

                foreach (var serializerTypes in this.GetSerializerTypes())
                {
                    testerA.Configure(serializerTypes.Item1);
                    testerB.Configure(serializerTypes.Item2);

                    for (int i = 0; i < 3; i++)
                    {
                        var objA = testerA.CreateObject(typeA);

                        objA.SetPropertyValue("PropA", "Test" + i);
                        var result = objA.GetPropertyValue("PropA");
                        Assert.AreEqual("Test" + i, result);

                        var objB = testerB.CreateObject(typeB);

                        objB.SetPropertyValue("PropA", "Foo" + i);
                        objB.SetPropertyValue("PropB", i);
                        result = objB.GetPropertyValue("PropA");
                        Assert.AreEqual("Foo" + i, result);
                        result = objB.GetPropertyValue("PropB");
                        Assert.AreEqual(i, result);

                        var serialized = testerA.Serialize("B", objA);
                        var deserialized = testerB.Deserialize("A", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Test" + i, result);
                        result = deserialized.GetPropertyValue("PropB");
                        Assert.AreEqual(0, result);

                        serialized = testerB.Serialize("A", objB);
                        deserialized = testerA.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Foo" + i, result);
                    }
                }
            }
            finally
            {
                testerA.Dispose();
                testerB.Dispose();
            }
        }

        /// <summary>
        /// This method tests that a complex type property can be added.
        /// </summary>
        [TestMethod]
        public void AddComplexPropertyTest()
        {
            var testerA = this.CreateTester("A");
            var testerB = this.CreateTester("B");

            try
            {
                var typeA = testerA.CreateClass(
                        "This.Is.My.TestObject", new PropertyDescription("PropA", typeof(string)));

                var typeB1 = testerB.CreateClass(
                        "This.Is.My.TestProperty", new PropertyDescription("Prop1", typeof(string)));
                var typeB = testerB.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeof(string)),
                    new PropertyDescription("PropB", typeB1));

                foreach (var serializerTypes in this.GetSerializerTypes())
                {
                    testerA.Configure(serializerTypes.Item1);
                    testerB.Configure(serializerTypes.Item2);

                    for (int i = 0; i < 3; i++)
                    {
                        var objA = testerA.CreateObject(typeA);

                        objA.SetPropertyValue("PropA", "Test" + i);
                        var result = objA.GetPropertyValue("PropA");
                        Assert.AreEqual("Test" + i, result);

                        var objB = testerB.CreateObject(typeB);
                        var objB1 = testerB.CreateObject(typeB1);

                        objB1.SetPropertyValue("Prop1", "Bar" + i);

                        objB.SetPropertyValue("PropA", "Foo" + i);
                        objB.SetPropertyValue("PropB", objB1);
                        result = objB.GetPropertyValue("PropA");
                        Assert.AreEqual("Foo" + i, result);
                        var wrapper = objB.GetPropertyWrapperValue("PropB");
                        Assert.IsNotNull(wrapper);
                        result = wrapper.GetPropertyValue("Prop1");
                        Assert.AreEqual("Bar" + i, result);

                        var serialized = testerA.Serialize("B", objA);
                        var deserialized = testerB.Deserialize("A", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Test" + i, result);
                        result = deserialized.GetPropertyWrapperValue("PropB");
                        Assert.IsNull(result);

                        serialized = testerB.Serialize("A", objB);
                        deserialized = testerA.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Foo" + i, result);
                    }
                }
            }
            finally
            {
                testerA.Dispose();
                testerB.Dispose();
            }
        }

        /// <summary>
        /// This method tests that a string property can be removed.
        /// </summary>
        [TestMethod]
        public void RemoveStringPropertyTest()
        {
            var testerA = this.CreateTester("A");
            var testerB = this.CreateTester("B");

            try
            {
                var typeA = testerA.CreateClass(
                        "This.Is.My.TestObject",
                        new PropertyDescription("PropA", typeof(string)),
                        new PropertyDescription("PropB", typeof(string)));
                var typeB = testerB.CreateClass(
                        "This.Is.My.TestObject", new PropertyDescription("PropA", typeof(string)));

                foreach (var serializerTypes in this.GetSerializerTypes())
                {
                    testerA.Configure(serializerTypes.Item1);
                    testerB.Configure(serializerTypes.Item2);

                    for (int i = 0; i < 3; i++)
                    {
                        var objA = testerA.CreateObject(typeA);

                        objA.SetPropertyValue("PropA", "Test" + i);
                        objA.SetPropertyValue("PropB", "Other" + i);
                        var result = objA.GetPropertyValue("PropA");
                        Assert.AreEqual("Test" + i, result);
                        result = objA.GetPropertyValue("PropB");
                        Assert.AreEqual("Other" + i, result);

                        var objB = testerB.CreateObject(typeB);

                        objB.SetPropertyValue("PropA", "Foo" + i);
                        result = objB.GetPropertyValue("PropA");
                        Assert.AreEqual("Foo" + i, result);

                        var serialized = testerA.Serialize("B", objA);
                        var deserialized = testerB.Deserialize("A", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Test" + i, result);

                        serialized = testerB.Serialize("A", objB);
                        deserialized = testerA.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Foo" + i, result);
                        result = deserialized.GetPropertyValue("PropB");
                        Assert.IsNull(result);
                    }
                }
            }
            finally
            {
                testerA.Dispose();
                testerB.Dispose();
            }
        }

        /// <summary>
        /// This method tests that an int property can be removed.
        /// </summary>
        [TestMethod]
        public void RemoveIntPropertyTest()
        {
            var testerA = this.CreateTester("A");
            var testerB = this.CreateTester("B");

            try
            {
                var typeA = testerA.CreateClass(
                        "This.Is.My.TestObject",
                        new PropertyDescription("PropA", typeof(string)),
                        new PropertyDescription("PropB", typeof(int)));
                var typeB = testerB.CreateClass(
                        "This.Is.My.TestObject", new PropertyDescription("PropA", typeof(string)));

                foreach (var serializerTypes in this.GetSerializerTypes())
                {
                    testerA.Configure(serializerTypes.Item1);
                    testerB.Configure(serializerTypes.Item2);

                    for (int i = 0; i < 3; i++)
                    {
                        var objA = testerA.CreateObject(typeA);

                        objA.SetPropertyValue("PropA", "Test" + i);
                        objA.SetPropertyValue("PropB", i);
                        var result = objA.GetPropertyValue("PropA");
                        Assert.AreEqual("Test" + i, result);
                        result = objA.GetPropertyValue("PropB");
                        Assert.AreEqual(i, result);

                        var objB = testerB.CreateObject(typeB);

                        objB.SetPropertyValue("PropA", "Foo" + i);
                        result = objB.GetPropertyValue("PropA");
                        Assert.AreEqual("Foo" + i, result);

                        var serialized = testerA.Serialize("B", objA);
                        var deserialized = testerB.Deserialize("A", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Test" + i, result);

                        serialized = testerB.Serialize("A", objB);
                        deserialized = testerA.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Foo" + i, result);
                        result = deserialized.GetPropertyValue("PropB");
                        Assert.AreEqual(0, result);
                    }
                }
            }
            finally
            {
                testerA.Dispose();
                testerB.Dispose();
            }
        }

        /// <summary>
        /// This method tests that a complex type property can be removed.
        /// </summary>
        [TestMethod]
        public void RemoveComplexPropertyTest()
        {
            var testerA = this.CreateTester("A");
            var testerB = this.CreateTester("B");

            try
            {
                var typeA1 = testerA.CreateClass(
                        "This.Is.My.TestProperty",
                        new PropertyDescription("Prop1", typeof(string)));
                var typeA = testerA.CreateClass(
                        "This.Is.My.TestObject",
                        new PropertyDescription("PropA", typeof(string)),
                        new PropertyDescription("PropB", typeA1));

                var typeB = testerB.CreateClass(
                        "This.Is.My.TestObject", new PropertyDescription("PropA", typeof(string)));

                foreach (var serializerTypes in this.GetSerializerTypes())
                {
                    testerA.Configure(serializerTypes.Item1);
                    testerB.Configure(serializerTypes.Item2);

                    for (int i = 0; i < 3; i++)
                    {
                        var objA = testerA.CreateObject(typeA);
                        var objA1 = testerA.CreateObject(typeA1);

                        objA1.SetPropertyValue("Prop1", "Other" + i);
                        objA.SetPropertyValue("PropA", "Test" + i);
                        objA.SetPropertyValue("PropB", objA1);
                        var result = objA.GetPropertyValue("PropA");
                        Assert.AreEqual("Test" + i, result);
                        var wrapper = objA.GetPropertyWrapperValue("PropB");
                        Assert.IsNotNull(wrapper);
                        result = wrapper.GetPropertyValue("Prop1");
                        Assert.AreEqual("Other" + i, result);

                        var objB = testerB.CreateObject(typeB);

                        objB.SetPropertyValue("PropA", "Foo" + i);
                        result = objB.GetPropertyValue("PropA");
                        Assert.AreEqual("Foo" + i, result);

                        var serialized = testerA.Serialize("B", objA);
                        var deserialized = testerB.Deserialize("A", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Test" + i, result);

                        serialized = testerB.Serialize("A", objB);
                        deserialized = testerA.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Foo" + i, result);
                        result = deserialized.GetPropertyWrapperValue("PropB");
                        Assert.IsNull(result);
                    }
                }
            }
            finally
            {
                testerA.Dispose();
                testerB.Dispose();
            }
        }
    }
}
