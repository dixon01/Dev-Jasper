// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BecSerializerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for BecSerializerTest and is intended
//   to contain all BecSerializerTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transcoder.Bec
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Tests.Messages;
    using Gorba.Common.Medi.Core.Transcoder.Bec;
    using Gorba.Common.Utility.Core;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for <see cref="BecSerializer"/> and is intended
    /// to contain all <see cref="BecSerializer"/> Unit Tests
    /// </summary>
    [TestClass]
    public class BecSerializerTest
    {
        #region Public Properties

        /// <summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        // You can use the following additional attributes as you write your tests:
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext)
        // {
        // }
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup()
        // {
        // }
        // Use TestInitialize to run code before running each test
        // [TestInitialize()]
        // public void MyTestInitialize()
        // {
        // }
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }
        #region Public Methods

        /// <summary>
        /// A test for Serialize and Deserialize
        /// </summary>
        [TestMethod]
        public void DefaultSerializeDeserializeTest()
        {
            this.SerializeDeserializeTest(new BecCodecConfig { Serializer = BecCodecConfig.SerializerType.Default });
        }

        /// <summary>
        /// A test for Serialize
        /// </summary>
        [TestMethod]
        public void GeneratedSerializeDeserializeTest()
        {
            this.SerializeDeserializeTest(new BecCodecConfig { Serializer = BecCodecConfig.SerializerType.Generated });
        }

        /// <summary>
        /// A test for Serialize
        /// </summary>
        [TestMethod]
        public void ReflectionSerializeDeserializeTest()
        {
            this.SerializeDeserializeTest(new BecCodecConfig { Serializer = BecCodecConfig.SerializerType.Reflection });
        }

        /// <summary>
        /// A test for Serialize
        /// </summary>
        [TestMethod]
        public void ReflectionSerializeGeneratedDeserializeTest()
        {
            this.SerializeDeserializeTest(
                new BecCodecConfig { Serializer = BecCodecConfig.SerializerType.Reflection },
                new BecCodecConfig { Serializer = BecCodecConfig.SerializerType.Generated });
        }

        /// <summary>
        /// A test for Serialize
        /// </summary>
        [TestMethod]
        public void GeneratedSerializeReflectionDeserializeTest()
        {
            this.SerializeDeserializeTest(
                new BecCodecConfig { Serializer = BecCodecConfig.SerializerType.Generated },
                new BecCodecConfig { Serializer = BecCodecConfig.SerializerType.Reflection });
        }

        private void SerializeDeserializeTest(BecCodecConfig config)
        {
            this.SerializeDeserializeTest(config, config);
        }

        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit Test code")]
        private void SerializeDeserializeTest(
            BecCodecConfig serializerConfig, BecCodecConfig deserializerConfig, int version = 1)
        {
            var serializer = new BecSerializer(serializerConfig, new FrameController(), version);
            var deserializer = new BecSerializer(deserializerConfig, new FrameController(), version);

            var sentMsg = new MediMessage { Payload = new SimpleMessage() };
            var recvMsg = this.SerializeDeserialize(serializer, deserializer, sentMsg);
            Assert.AreEqual(sentMsg.Payload, recvMsg.Payload);

            sentMsg = new MediMessage { Payload = new ListMessage() };
            recvMsg = this.SerializeDeserialize(serializer, deserializer, sentMsg);
            Assert.AreEqual(sentMsg.Payload, recvMsg.Payload);

            sentMsg = new MediMessage { Payload = new ListMessage { Integers = new List<int> { 1, 2, 3 } } };
            recvMsg = this.SerializeDeserialize(serializer, deserializer, sentMsg);
            Assert.AreEqual(sentMsg.Payload, recvMsg.Payload);

            sentMsg = new MediMessage { Payload = new PropertiesMessage() };
            recvMsg = this.SerializeDeserialize(serializer, deserializer, sentMsg);
            Assert.AreEqual(sentMsg.Payload, recvMsg.Payload);

            var data = new byte[1024];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(i * 13);
            }

            sentMsg = new MediMessage { Payload = data };
            recvMsg = this.SerializeDeserialize(serializer, deserializer, sentMsg);
            CollectionAssert.AreEqual((ICollection)sentMsg.Payload, (ICollection)recvMsg.Payload);

            sentMsg = new MediMessage
                {
                    Payload =
                        new PropertiesMessage
                            {
                                IntValue = 15, LongValue = long.MaxValue, Message = new SimpleMessage()
                            }
                };
            recvMsg = this.SerializeDeserialize(serializer, deserializer, sentMsg);
            Assert.AreEqual(sentMsg.Payload, recvMsg.Payload);

            sentMsg = new MediMessage
            {
                Payload =
                    new PropertiesMessage
                    {
                        IntValue = 15,
                        LongValue = long.MaxValue,
                        String = "FooBar"
                    }
            };
            recvMsg = this.SerializeDeserialize(serializer, deserializer, sentMsg);
            Assert.AreEqual(sentMsg.Payload, recvMsg.Payload);

            sentMsg = new MediMessage
                {
                    Payload =
                        new ListMessage { Messages = new[] { new SimpleMessage(5), new SimpleMessage(int.MinValue) } }
                };
            recvMsg = this.SerializeDeserialize(serializer, deserializer, sentMsg);
            Assert.AreEqual(sentMsg.Payload, recvMsg.Payload);

            sentMsg = new MediMessage
                {
                    Payload =
                        new CompositeObject
                            {
                                Name = "Root",
                                Children =
                                    new List<BaseObject>
                                        {
                                            new SubObject { Name = "Sub", Value = 365 },
                                            new BaseObject { Name = "Base" }
                                        }
                            }
                };
            recvMsg = this.SerializeDeserialize(serializer, deserializer, sentMsg);
            Assert.AreEqual(sentMsg.Payload, recvMsg.Payload);

            sentMsg = new MediMessage
                {
                    Payload =
                        new EnumMessage
                            {
                                DateTimeKind = DateTimeKind.Utc,
                                ByteEnum = EnumMessage.MyByteEnum.B,
                                Enum = EnumMessage.MyEnum.C,
                                UlongEnum = EnumMessage.MyUlongEnum.A
                            }
                };
            recvMsg = this.SerializeDeserialize(serializer, deserializer, sentMsg);
            Assert.AreEqual(sentMsg.Payload, recvMsg.Payload);

            sentMsg = new MediMessage
                {
                    Payload =
                        new BuiltInTypesObject
                        {
                            Name = "John",
                            DateTime = TimeProvider.Current.UtcNow,
                            Guid = Guid.NewGuid(),
                            Type = typeof(BuiltInTypesObject)
                        }
                };
            recvMsg = this.SerializeDeserialize(serializer, deserializer, sentMsg);
            Assert.AreEqual(sentMsg.Payload, recvMsg.Payload);
        }

        private MediMessage SerializeDeserialize(
            BecSerializer serializer, BecSerializer deserializer, MediMessage sentMsg)
        {
            MediMessage recvMsg = null;
            foreach (var buffer in serializer.Serialize(sentMsg))
            {
                var recv = deserializer.Deserialize(buffer);
                Assert.IsTrue(recv == null || (recv is MediMessage));
                recvMsg = recv as MediMessage;
            }

            Assert.IsNotNull(recvMsg);
            Assert.IsNotNull(recvMsg.Payload);
            return recvMsg;
        }

        #endregion

        /// <summary>
        /// Base class for a complex object hierarchy.
        /// </summary>
        public class BaseObject
        {
            /// <summary>
            /// Gets or sets Name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
            /// </summary>
            /// <returns>
            /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/> otherwise false.
            /// </returns>
            /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>. </param>
            public override bool Equals(object obj)
            {
                var other = obj as BaseObject;
                if (other == null || this.GetType() != other.GetType())
                {
                    return false;
                }

                return object.Equals(this.Name, other.Name);
            }

            /// <summary>
            /// Serves as a hash function for a particular type.
            /// </summary>
            /// <returns>
            /// A hash code for the current <see cref="Object"/>.
            /// </returns>
            public override int GetHashCode()
            {
                int hashcode = 0;
                if (this.Name != null)
                {
                    hashcode = this.Name.GetHashCode();
                }

                return hashcode;
            }
        }

        /// <summary>
        /// Subclass for complex object graph.
        /// </summary>
        public class SubObject : BaseObject
        {
            /// <summary>
            /// Gets or sets Value.
            /// </summary>
            public int Value { get; set; }

            /// <summary>
            /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
            /// </summary>
            /// <returns>
            /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/> otherwise false.
            /// </returns>
            /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>. </param>
            public override bool Equals(object obj)
            {
                var other = obj as SubObject;
                if (other == null || this.GetType() != other.GetType())
                {
                    return false;
                }

                return base.Equals(other) && this.Value == other.Value;
            }

            /// <summary>
            /// Serves as a hash function for a particular type.
            /// </summary>
            /// <returns>
            /// A hash code for the current <see cref="Object"/>.
            /// </returns>
            public override int GetHashCode()
            {
                return base.GetHashCode() ^ this.Value;
            }
        }

        /// <summary>
        /// Subclass for special built-in types.
        /// </summary>
        public class BuiltInTypesObject : BaseObject
        {
            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            public Type Type { get; set; }

            /// <summary>
            /// Gets or sets the date time.
            /// </summary>
            public DateTime DateTime { get; set; }

            /// <summary>
            /// Gets or sets the GUID.
            /// </summary>
            public Guid Guid { get; set; }

            /// <summary>
            /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
            /// </summary>
            /// <returns>
            /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/> otherwise false.
            /// </returns>
            /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>. </param>
            public override bool Equals(object obj)
            {
                var other = obj as BuiltInTypesObject;
                if (other == null || this.GetType() != other.GetType())
                {
                    return false;
                }

                return base.Equals(other) && this.Type == other.Type && this.DateTime.Equals(other.DateTime)
                       && this.Guid.Equals(other.Guid);
            }

            /// <summary>
            /// Serves as a hash function for a particular type.
            /// </summary>
            /// <returns>
            /// A hash code for the current <see cref="Object"/>.
            /// </returns>
            public override int GetHashCode()
            {
                return base.GetHashCode() ^ this.Type.GetHashCode();
            }
        }

        /// <summary>
        /// Object containing several objects for testing complex object graphs.
        /// </summary>
        public class CompositeObject : BaseObject
        {
            /// <summary>
            /// Gets or sets Children.
            /// </summary>
            public List<BaseObject> Children { get; set; }

            /// <summary>
            /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
            /// </summary>
            /// <returns>
            /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/> otherwise false.
            /// </returns>
            /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>. </param>
            public override bool Equals(object obj)
            {
                var other = obj as CompositeObject;
                if (other == null || this.GetType() != other.GetType())
                {
                    return false;
                }

                if (!base.Equals(other))
                {
                    return false;
                }

                if ((this.Children == null) != (other.Children == null))
                {
                    return false;
                }

                if (this.Children != null && other.Children != null && !this.Children.SequenceEqual(other.Children))
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Serves as a hash function for a particular type.
            /// </summary>
            /// <returns>
            /// A hash code for the current <see cref="Object"/>.
            /// </returns>
            public override int GetHashCode()
            {
                return base.GetHashCode() ^ (this.Children != null ? this.Children.Count : -1);
            }
        }
    }
}