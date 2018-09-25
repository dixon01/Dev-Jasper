// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownBecObjectTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnknownBecObjectTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transcoder.Bec
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Gorba.Common.Medi.Core.Tests.Transcoder.Bec.Utils;
    using Gorba.Common.Medi.Core.Transcoder.Bec.Engine;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="UnknownBecObject"/> and indirectly for <see cref="UnknownBecList"/>.
    /// </summary>
    [TestClass]
    public class UnknownBecObjectTest : BecAppDomainTestBase
    {
        /// <summary>
        /// Test with a simple unknown type.
        /// </summary>
        [TestMethod]
        public void UnknownSimpleTypeTest()
        {
            var testerA = this.CreateTester("A");
            var testerB = this.CreateTester("B");
            var testerC = this.CreateTester("C");

            try
            {
                var typeA = testerA.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeof(string)));
                var typeC = testerC.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeof(string)));

                foreach (var serializerTypes in this.GetSerializerTypes())
                {
                    testerA.Configure(serializerTypes.Item1);
                    testerB.Configure(serializerTypes.Item1);
                    testerC.Configure(serializerTypes.Item2);

                    for (int i = 0; i < 3; i++)
                    {
                        var objA = testerA.CreateObject(typeA);

                        objA.SetPropertyValue("PropA", "Test" + i);
                        var result = objA.GetPropertyValue("PropA");
                        Assert.AreEqual("Test" + i, result);

                        var objC = testerC.CreateObject(typeC);

                        objC.SetPropertyValue("PropA", "Hello" + i);
                        result = objC.GetPropertyValue("PropA");
                        Assert.AreEqual("Hello" + i, result);

                        var serialized = testerA.Serialize("B", objA);
                        var deserialized = testerB.Deserialize("A", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        Assert.AreEqual(typeof(UnknownBecObject).FullName, deserialized.FullName);

                        serialized = testerB.Serialize("C", deserialized);
                        deserialized = testerC.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Test" + i, result);

                        serialized = testerC.Serialize("B", objC);
                        deserialized = testerB.Deserialize("C", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        Assert.AreEqual(typeof(UnknownBecObject).FullName, deserialized.FullName);

                        serialized = testerB.Serialize("A", deserialized);
                        deserialized = testerA.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Hello" + i, result);
                    }
                }
            }
            finally
            {
                testerA.Dispose();
                testerB.Dispose();
                testerC.Dispose();
            }
        }

        /// <summary>
        /// Test with an unknown type containing another unknown type.
        /// </summary>
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        [TestMethod]
        public void UnknownComplexTypeTest()
        {
            var testerA = this.CreateTester("A");
            var testerB = this.CreateTester("B");
            var testerC = this.CreateTester("C");

            try
            {
                var typeA1 = testerA.CreateClass(
                    "This.Is.My.SubObject",
                    new PropertyDescription("Prop1", typeof(int)));
                var typeA = testerA.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeA1),
                    new PropertyDescription("PropB", typeof(string)));

                var typeC1 = testerC.CreateClass(
                    "This.Is.My.SubObject",
                    new PropertyDescription("Prop1", typeof(int)));
                var typeC = testerC.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeC1),
                    new PropertyDescription("PropB", typeof(string)));

                foreach (var serializerTypes in this.GetSerializerTypes())
                {
                    testerA.Configure(serializerTypes.Item1);
                    testerB.Configure(serializerTypes.Item1);
                    testerC.Configure(serializerTypes.Item2);

                    for (int i = 0; i < 3; i++)
                    {
                        var objA = testerA.CreateObject(typeA);
                        var objA1 = testerA.CreateObject(typeA1);

                        objA1.SetPropertyValue("Prop1", i * 17);
                        objA.SetPropertyValue("PropA", objA1);
                        objA.SetPropertyValue("PropB", "Test" + i);

                        var objC = testerC.CreateObject(typeC);
                        var objC1 = testerC.CreateObject(typeC1);

                        objC1.SetPropertyValue("Prop1", i * 37);
                        objC.SetPropertyValue("PropA", objC1);
                        objC.SetPropertyValue("PropB", "Hello" + i);

                        var serialized = testerA.Serialize("B", objA);
                        var deserialized = testerB.Deserialize("A", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        Assert.AreEqual(typeof(UnknownBecObject).FullName, deserialized.FullName);

                        serialized = testerB.Serialize("C", deserialized);
                        deserialized = testerC.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        var wrapper = deserialized.GetPropertyWrapperValue("PropA");
                        Assert.IsNotNull(wrapper);
                        var result = wrapper.GetPropertyValue("Prop1");
                        Assert.AreEqual(i * 17, result);
                        result = deserialized.GetPropertyValue("PropB");
                        Assert.AreEqual("Test" + i, result);

                        serialized = testerC.Serialize("B", objC);
                        deserialized = testerB.Deserialize("C", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        Assert.AreEqual(typeof(UnknownBecObject).FullName, deserialized.FullName);

                        serialized = testerB.Serialize("A", deserialized);
                        deserialized = testerA.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        wrapper = deserialized.GetPropertyWrapperValue("PropA");
                        Assert.IsNotNull(wrapper);
                        result = wrapper.GetPropertyValue("Prop1");
                        Assert.AreEqual(i * 37, result);
                        result = deserialized.GetPropertyValue("PropB");
                        Assert.AreEqual("Hello" + i, result);
                    }
                }
            }
            finally
            {
                testerA.Dispose();
                testerB.Dispose();
                testerC.Dispose();
            }
        }

        /// <summary>
        /// Test with an unknown type containing an unknown enum.
        /// </summary>
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        [TestMethod]
        public void UnknownComplexTypeWithEnumTest()
        {
            var testerA = this.CreateTester("A");
            var testerB = this.CreateTester("B");
            var testerC = this.CreateTester("C");

            try
            {
                var typeA1 = testerA.CreateEnum(
                    "This.Is.My.Enum",
                    new ClassDescription(typeof(int)),
                    new KeyValuePair<string, object>("Default", 0),
                    new KeyValuePair<string, object>("One", 1),
                    new KeyValuePair<string, object>("Two", 2),
                    new KeyValuePair<string, object>("Three", 3));
                var typeA = testerA.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeA1),
                    new PropertyDescription("PropB", typeof(string)));

                var typeC1 = testerC.CreateEnum(
                    "This.Is.My.Enum",
                    new ClassDescription(typeof(int)),
                    new KeyValuePair<string, object>("Default", 0),
                    new KeyValuePair<string, object>("One", 1),
                    new KeyValuePair<string, object>("Two", 2),
                    new KeyValuePair<string, object>("Three", 3));
                var typeC = testerC.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeC1),
                    new PropertyDescription("PropB", typeof(string)));

                foreach (var serializerTypes in this.GetSerializerTypes())
                {
                    testerA.Configure(serializerTypes.Item1);
                    testerB.Configure(serializerTypes.Item1);
                    testerC.Configure(serializerTypes.Item2);

                    for (int i = 0; i < 3; i++)
                    {
                        var objA = testerA.CreateObject(typeA);

                        objA.SetPropertyValue("PropA", i);
                        objA.SetPropertyValue("PropB", "Test" + i);
                        var hashA = objA.GetPropertyWrapperValue("PropA").InvokeMethod("GetHashCode");

                        var objC = testerC.CreateObject(typeC);

                        objC.SetPropertyValue("PropA", i + 1);
                        objC.SetPropertyValue("PropB", "Hello" + i);
                        var hashC = objC.GetPropertyWrapperValue("PropA").InvokeMethod("GetHashCode");

                        var serialized = testerA.Serialize("B", objA);
                        var deserialized = testerB.Deserialize("A", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        Assert.AreEqual(typeof(UnknownBecObject).FullName, deserialized.FullName);

                        serialized = testerB.Serialize("C", deserialized);
                        deserialized = testerC.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        var wrapper = deserialized.GetPropertyWrapperValue("PropA");
                        Assert.IsNotNull(wrapper);
                        var result = wrapper.InvokeMethod("GetHashCode");
                        Assert.AreEqual(hashA, result);
                        result = deserialized.GetPropertyValue("PropB");
                        Assert.AreEqual("Test" + i, result);

                        serialized = testerC.Serialize("B", objC);
                        deserialized = testerB.Deserialize("C", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        Assert.AreEqual(typeof(UnknownBecObject).FullName, deserialized.FullName);

                        serialized = testerB.Serialize("A", deserialized);
                        deserialized = testerA.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        wrapper = deserialized.GetPropertyWrapperValue("PropA");
                        Assert.IsNotNull(wrapper);
                        result = wrapper.InvokeMethod("GetHashCode");
                        Assert.AreEqual(hashC, result);
                        result = deserialized.GetPropertyValue("PropB");
                        Assert.AreEqual("Hello" + i, result);
                    }
                }
            }
            finally
            {
                testerA.Dispose();
                testerB.Dispose();
                testerC.Dispose();
            }
        }

        /// <summary>
        /// Unit test with an unknown type that contains a list of unknown type objects.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method")]
        public void UnknownTypeWithUnknownListTest()
        {
            var testerA = this.CreateTester("A");
            var testerB = this.CreateTester("B");
            var testerC = this.CreateTester("C");

            try
            {
                var typeA1 = testerA.CreateClass(
                    "This.Is.My.SubObject",
                    new PropertyDescription("Prop1", typeof(int)));
                var typeListA1 = testerA.GetGenericClass(
                    new ClassDescription(typeof(List<>)), new ClassDescription(typeA1));
                var typeA = testerA.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeListA1),
                    new PropertyDescription("PropB", typeof(string)));

                var typeC1 = testerC.CreateClass(
                    "This.Is.My.SubObject",
                    new PropertyDescription("Prop1", typeof(int)));
                var typeListC1 = testerC.GetGenericClass(
                    new ClassDescription(typeof(List<>)), new ClassDescription(typeC1));
                var typeC = testerC.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeListC1),
                    new PropertyDescription("PropB", typeof(string)));

                foreach (var serializerTypes in this.GetSerializerTypes())
                {
                    testerA.Configure(serializerTypes.Item1);
                    testerB.Configure(serializerTypes.Item1);
                    testerC.Configure(serializerTypes.Item2);

                    for (int i = 0; i < 3; i++)
                    {
                        var objA = testerA.CreateObject(typeA);
                        var objListA1 = testerA.CreateObject(typeListA1);
                        var objA1 = testerA.CreateObject(typeA1);

                        objA1.SetPropertyValue("Prop1", i * 17);
                        objListA1.InvokeMethod("Add", objA1);
                        objA.SetPropertyValue("PropA", objListA1);
                        objA.SetPropertyValue("PropB", "Test" + i);

                        var objC = testerC.CreateObject(typeC);
                        var objListC1 = testerC.CreateObject(typeListC1);
                        var objC1 = testerC.CreateObject(typeC1);
                        var objC2 = testerC.CreateObject(typeC1);

                        objC1.SetPropertyValue("Prop1", i * 37);
                        objC2.SetPropertyValue("Prop1", i * 57);
                        objListC1.InvokeMethod("Add", objC1);
                        objListC1.InvokeMethod("Add", objC2);
                        objC.SetPropertyValue("PropA", objListC1);
                        objC.SetPropertyValue("PropB", "Hello" + i);

                        var serialized = testerA.Serialize("B", objA);
                        var deserialized = testerB.Deserialize("A", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        Assert.AreEqual(typeof(UnknownBecObject).FullName, deserialized.FullName);

                        serialized = testerB.Serialize("C", deserialized);
                        deserialized = testerC.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        var wrapper = deserialized.GetPropertyWrapperValue("PropA");
                        Assert.AreEqual(1, wrapper.GetPropertyValue("Count"));
                        var item = wrapper.InvokeMethodAndWrap("get_Item", 0);
                        Assert.AreEqual(i * 17, item.GetPropertyValue("Prop1"));
                        var result = deserialized.GetPropertyValue("PropB");
                        Assert.AreEqual("Test" + i, result);

                        serialized = testerC.Serialize("B", objC);
                        deserialized = testerB.Deserialize("C", serialized, 0, serialized.Length);
                        Assert.AreEqual(typeof(UnknownBecObject).FullName, deserialized.FullName);

                        serialized = testerB.Serialize("A", deserialized);
                        deserialized = testerA.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        wrapper = deserialized.GetPropertyWrapperValue("PropA");
                        Assert.AreEqual(2, wrapper.GetPropertyValue("Count"));
                        item = wrapper.InvokeMethodAndWrap("get_Item", 0);
                        Assert.AreEqual(i * 37, item.GetPropertyValue("Prop1"));
                        item = wrapper.InvokeMethodAndWrap("get_Item", 1);
                        Assert.AreEqual(i * 57, item.GetPropertyValue("Prop1"));
                        result = deserialized.GetPropertyValue("PropB");
                        Assert.AreEqual("Hello" + i, result);
                    }
                }
            }
            finally
            {
                testerA.Dispose();
                testerB.Dispose();
                testerC.Dispose();
            }
        }

        /// <summary>
        /// Test with a unknown type that becomes known suddenly.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test")]
        [TestMethod]
        public void SuddenlyKnownTypeTest()
        {
            var testerA = this.CreateTester("A");
            var testerC = this.CreateTester("C");

            try
            {
                var typeA = testerA.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeof(string)));
                var typeC = testerC.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeof(string)));

                foreach (var serializerTypes in this.GetSerializerTypes())
                {
                    var testerB = this.CreateTester("B");
                    try
                    {
                        testerA.Configure(serializerTypes.Item1);
                        testerB.Configure(serializerTypes.Item1);
                        testerC.Configure(serializerTypes.Item2);

                        var objA = testerA.CreateObject(typeA);

                        objA.SetPropertyValue("PropA", "Test");
                        var result = objA.GetPropertyValue("PropA");
                        Assert.AreEqual("Test", result);

                        var objC = testerC.CreateObject(typeC);

                        objC.SetPropertyValue("PropA", "Hello");
                        result = objC.GetPropertyValue("PropA");
                        Assert.AreEqual("Hello", result);

                        var serialized = testerA.Serialize("B", objA);
                        var deserialized = testerB.Deserialize("A", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        Assert.AreEqual(typeof(UnknownBecObject).FullName, deserialized.FullName);

                        serialized = testerB.Serialize("C", deserialized);
                        deserialized = testerC.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Test", result);

                        serialized = testerC.Serialize("B", objC);
                        deserialized = testerB.Deserialize("C", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        Assert.AreEqual(typeof(UnknownBecObject).FullName, deserialized.FullName);

                        serialized = testerB.Serialize("A", deserialized);
                        deserialized = testerA.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Hello", result);

                        var typeB = testerB.CreateClass(
                            "This.Is.My.TestObject",
                            new PropertyDescription("PropA", typeof(string)));
                        var objB = testerB.CreateObject(typeB);
                        objB.SetPropertyValue("PropA", "OtherTest");

                        // REAL TEST:
                        // when serializing a now known object, we shouldn't get an ArgumentException
                        serialized = testerB.Serialize("C", objB);
                        deserialized = testerC.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("OtherTest", result);
                    }
                    finally
                    {
                        testerB.Dispose();
                    }
                }
            }
            finally
            {
                testerA.Dispose();
                testerC.Dispose();
            }
        }

        /// <summary>
        /// Test with a unknown type that becomes known suddenly.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test")]
        [TestMethod]
        public void SuddenlyUnknownTypeTest()
        {
            var testerA = this.CreateTester("A");
            var testerC = this.CreateTester("C");

            try
            {
                var typeA = testerA.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeof(string)));
                var typeC = testerC.CreateClass(
                    "This.Is.My.TestObject",
                    new PropertyDescription("PropA", typeof(string)));

                foreach (var serializerTypes in this.GetSerializerTypes())
                {
                    var testerB = this.CreateTester("B");
                    try
                    {
                        testerA.Configure(serializerTypes.Item1);
                        testerB.Configure(serializerTypes.Item1);
                        testerC.Configure(serializerTypes.Item2);

                        var objA = testerA.CreateObject(typeA);

                        objA.SetPropertyValue("PropA", "Test");
                        var result = objA.GetPropertyValue("PropA");
                        Assert.AreEqual("Test", result);

                        var objC = testerC.CreateObject(typeC);

                        objC.SetPropertyValue("PropA", "Hello");
                        result = objC.GetPropertyValue("PropA");
                        Assert.AreEqual("Hello", result);

                        var serialized = testerA.Serialize("B", objA);
                        var deserialized = testerB.Deserialize("A", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        Assert.AreEqual(typeof(UnknownBecObject).FullName, deserialized.FullName);

                        serialized = testerB.Serialize("C", deserialized);
                        deserialized = testerC.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Test", result);

                        serialized = testerC.Serialize("B", objC);
                        deserialized = testerB.Deserialize("C", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        Assert.AreEqual(typeof(UnknownBecObject).FullName, deserialized.FullName);

                        serialized = testerB.Serialize("A", deserialized);
                        deserialized = testerA.Deserialize("B", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Hello", result);

                        objA = testerA.CreateObject(typeA);

                        objA.SetPropertyValue("PropA", "Test");
                        result = objA.GetPropertyValue("PropA");
                        Assert.AreEqual("Test", result);

                        objC = testerC.CreateObject(typeC);

                        objC.SetPropertyValue("PropA", "Hello");
                        result = objC.GetPropertyValue("PropA");
                        Assert.AreEqual("Hello", result);

                        serialized = testerA.Serialize("B2", objA);
                        deserialized = testerB.Deserialize("A2", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        Assert.AreEqual(typeof(UnknownBecObject).FullName, deserialized.FullName);

                        var typeB = testerB.CreateClass(
                            "This.Is.My.TestObject",
                            new PropertyDescription("PropA", typeof(string)));
                        var objB = testerB.CreateObject(typeB);
                        objB.SetPropertyValue("PropA", "OtherTest");

                        // REAL TEST:
                        // when serializing an unknown object (that just become known),
                        // we shouldn't get a TargetException
                        var intermediateSerialized = testerB.Serialize("C2", objB);
                        var intermediateDeserialized = testerC.Deserialize(
                            "B2",
                            intermediateSerialized,
                            0,
                            intermediateSerialized.Length);
                        Assert.IsNotNull(intermediateDeserialized);
                        result = intermediateDeserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("OtherTest", result);

                        serialized = testerB.Serialize("C2", deserialized);
                        deserialized = testerC.Deserialize("B2", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Test", result);

                        serialized = testerC.Serialize("B", objC);
                        deserialized = testerB.Deserialize("C", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        Assert.AreEqual(typeof(UnknownBecObject).FullName, deserialized.FullName);

                        serialized = testerB.Serialize("A2", deserialized);
                        deserialized = testerA.Deserialize("B2", serialized, 0, serialized.Length);
                        Assert.IsNotNull(deserialized);
                        result = deserialized.GetPropertyValue("PropA");
                        Assert.AreEqual("Hello", result);
                    }
                    finally
                    {
                        testerB.Dispose();
                    }
                }
            }
            finally
            {
                testerA.Dispose();
                testerC.Dispose();
            }
        }
    }
}
