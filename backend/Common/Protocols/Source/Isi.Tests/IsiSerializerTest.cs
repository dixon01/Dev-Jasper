// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsiSerializerTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsiSerializerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Isi.Tests
{
    using System.Globalization;
    using System.IO;
    using System.Text;

    using Gorba.Common.Protocols.Isi;
    using Gorba.Common.Protocols.Isi.Messages;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="IsiSerializer"/>
    /// </summary>
    [TestClass]
    public class IsiSerializerTest
    {
        /// <summary>
        /// Simple test for serializing an <see cref="IsiGet"/> message.
        /// </summary>
        [TestMethod]
        public void SerializeIsiGetSimpleTest()
        {
            var memory = new MemoryStream();
            var target = new IsiSerializer { Output = memory };

            var message = new IsiGet { Items = new DataItemRequestList("Date") };
            target.Serialize(message);

            var serialized = Encoding.UTF8.GetString(memory.ToArray());
            Assert.AreEqual("<IsiGet><Items>Date</Items></IsiGet>", serialized);
        }

        /// <summary>
        /// Simple test for serializing an <see cref="IsiPut"/> message.
        /// </summary>
        [TestMethod]
        public void SerializeIsiPutTest()
        {
            var memory = new MemoryStream();
            var target = new IsiSerializer { Output = memory };

            var message = new IsiPut();
            message.Items.Add(new DataItem { Name = "AppName", Value = "ClientX" });

            target.Serialize(message);

            var serialized = Encoding.UTF8.GetString(memory.ToArray());
            Assert.AreEqual("<IsiPut><AppName>ClientX</AppName></IsiPut>", serialized);
        }

        /// <summary>
        /// Simple test for serializing an <see cref="IsiRegister"/> message.
        /// </summary>
        [TestMethod]
        public void SerializeIsiRegisterTest()
        {
            var memory = new MemoryStream();
            var target = new IsiSerializer { Output = memory };

            var message = new IsiRegister
                { Commands = new DataItemRequestList("PlayInternalAnnouncement", "PlayExternalAnnouncement") };
            target.Serialize(message);

            var serialized = Encoding.UTF8.GetString(memory.ToArray());
            Assert.AreEqual("<IsiRegister><Commands>PlayInternalAnnouncement PlayExternalAnnouncement</Commands></IsiRegister>", serialized);
        }

        /// <summary>
        /// Complex test for serializing an <see cref="IsiGet"/> message.
        /// </summary>
        [TestMethod]
        public void SerializeIsiGetComplexTest()
        {
            var memory = new MemoryStream();
            var target = new IsiSerializer { Output = memory };

            var message = new IsiGet
                {
                    Items = new DataItemRequestList("BlockNo", "ConsecutiveTripNo"),
                    OnChange = new DataItemRequestList("ConsecutiveTripNo"),
                    Cyclic = 60
                };
            target.Serialize(message);

            var serialized = Encoding.UTF8.GetString(memory.ToArray());
            Assert.AreEqual("<IsiGet><Items>BlockNo ConsecutiveTripNo</Items><OnChange>ConsecutiveTripNo</OnChange><Cyclic>60</Cyclic></IsiGet>", serialized);
        }

        /// <summary>
        /// Complex test for serializing an <see cref="IsiGet"/> message which contains an IsiId tag.
        /// </summary>
        [TestMethod]
        public void SerializeIsiGetComplexWithIdTest()
        {
            var memory = new MemoryStream();
            var target = new IsiSerializer { Output = memory };

            var message = new IsiGet
                {
                    IsiId = 123,
                    Items = new DataItemRequestList("BlockNo", "ConsecutiveTripNo"),
                    OnChange = new DataItemRequestList("ConsecutiveTripNo"),
                    Cyclic = 60
                };
            target.Serialize(message);

            var serialized = Encoding.UTF8.GetString(memory.ToArray());
            Assert.AreEqual("<IsiGet><IsiId>123</IsiId><Items>BlockNo ConsecutiveTripNo</Items><OnChange>ConsecutiveTripNo</OnChange><Cyclic>60</Cyclic></IsiGet>", serialized);
        }

        /// <summary>
        /// Test for serializing an <see cref="IsiGet"/> message that disables a certain request by ID.
        /// </summary>
        [TestMethod]
        public void SerializeIsiGetDisableTest()
        {
            var memory = new MemoryStream();
            var target = new IsiSerializer { Output = memory };

            var message = new IsiGet { IsiId = 123 };
            target.Serialize(message);

            var serialized = Encoding.UTF8.GetString(memory.ToArray());
            Assert.AreEqual("<IsiGet><IsiId>123</IsiId></IsiGet>", serialized);
        }

        /// <summary>
        /// Simple test for serializing an <see cref="IsiGet"/> message which requires UTF-8.
        /// </summary>
        [TestMethod]
        public void SerializeIsiGetUtf8Test()
        {
            var memory = new MemoryStream();
            var target = new IsiSerializer { Output = memory };

            var message = new IsiGet { Items = new DataItemRequestList("ÄÖÜäöü") };
            target.Serialize(message);

            var serialized = Encoding.UTF8.GetString(memory.ToArray());
            Assert.AreEqual("<IsiGet><Items>ÄÖÜäöü</Items></IsiGet>", serialized);
        }

        /// <summary>
        /// Simple test for deserializing an <see cref="IsiPut"/> message.
        /// </summary>
        [TestMethod]
        public void DeserializeIsiPutTest()
        {
            const string Input = "<IsiPut><Date>2005-09-30</Date></IsiPut>";
            var memory = new MemoryStream(new UTF8Encoding(false).GetBytes(Input));
            var target = new IsiSerializer { Input = memory };

            var message = target.Deserialize();

            Assert.IsInstanceOfType(message, typeof(IsiPut));
            var put = (IsiPut)message;

            Assert.AreEqual(-1, put.IsiId);
            Assert.AreEqual(1, put.Items.Count);

            var item = put.Items.Find(i => i.Name == "Date");
            Assert.AreEqual("2005-09-30", item.Value);
        }

        /// <summary>
        /// Simple test for deserializing an <see cref="IsiPut"/> message which requires UTF-8.
        /// </summary>
        [TestMethod]
        public void DeserializeIsiPutUtf8Test()
        {
            const string Input = "<IsiPut><Test>ÄÖÜäöü</Test></IsiPut>";
            var memory = new MemoryStream(new UTF8Encoding(false).GetBytes(Input));
            var target = new IsiSerializer { Input = memory };

            var message = target.Deserialize();

            Assert.IsInstanceOfType(message, typeof(IsiPut));
            var put = (IsiPut)message;

            Assert.AreEqual(-1, put.IsiId);
            Assert.AreEqual(1, put.Items.Count);

            var item = put.Items.Find(i => i.Name == "Test");
            Assert.AreEqual("ÄÖÜäöü", item.Value);
        }

        /// <summary>
        /// Simple test for deserializing an <see cref="IsiGet"/> message.
        /// </summary>
        [TestMethod]
        public void DeserializeIsiGetTest()
        {
            const string Input = "<IsiGet><Items>AppName</Items></IsiGet>";
            var memory = new MemoryStream(new UTF8Encoding(false).GetBytes(Input));
            var target = new IsiSerializer { Input = memory };

            var message = target.Deserialize();

            Assert.IsInstanceOfType(message, typeof(IsiGet));
            var get = (IsiGet)message;

            Assert.AreEqual(-1, get.IsiId);
            Assert.AreEqual(1, get.Items.Count);
            Assert.AreEqual("AppName", get.Items[0]);
        }

        /// <summary>
        /// Simple test for deserializing an <see cref="IsiExecute"/> message.
        /// </summary>
        [TestMethod]
        public void DeserializeIsiExecuteTest()
        {
            const string Input = "<IsiExecute><PlayInternalAnnouncement>12345</PlayInternalAnnouncement></IsiExecute>";
            var memory = new MemoryStream(new UTF8Encoding(false).GetBytes(Input));
            var target = new IsiSerializer { Input = memory };

            var message = target.Deserialize();

            Assert.IsInstanceOfType(message, typeof(IsiExecute));
            var get = (IsiExecute)message;

            Assert.AreEqual(1, get.Items.Count);
            Assert.AreEqual("12345", get.Items["PlayInternalAnnouncement"]);
        }

        /// <summary>
        /// Complex test for deserializing an <see cref="IsiPut"/> message.
        /// </summary>
        [TestMethod]
        public void DeserializeIsiPutComplexTest()
        {
            const string Input = "<IsiPut><IsiId>123</IsiId><BlockNo>66102</BlockNo><ConsecutiveTripNo>1</ConsecutiveTripNo></IsiPut>";
            var memory = new MemoryStream(new UTF8Encoding(false).GetBytes(Input));
            var target = new IsiSerializer { Input = memory };

            var message = target.Deserialize();

            Assert.IsInstanceOfType(message, typeof(IsiPut));
            var put = (IsiPut)message;

            Assert.AreEqual(123, put.IsiId);
            Assert.AreEqual(2, put.Items.Count);

            var item = put.Items.Find(i => i.Name == "BlockNo");
            Assert.AreEqual("66102", item.Value);

            item = put.Items.Find(i => i.Name == "ConsecutiveTripNo");
            Assert.AreEqual("1", item.Value);
        }

        /// <summary>
        /// Test for deserializing multiple <see cref="IsiPut"/> messages that require more space than the input buffer is.
        /// </summary>
        [TestMethod]
        public void DeserializeIsiPutFragmentationTest()
        {
            const int Count = 200;
            var memory = new MemoryStream();
            var writer = new StreamWriter(memory, new UTF8Encoding(false));
            for (int i = 0; i < Count; i++)
            {
                writer.Write("<IsiPut><IsiId>123</IsiId><BlockNo>66102</BlockNo><ConsecutiveTripNo>{0}</ConsecutiveTripNo></IsiPut>", i);
            }

            writer.Flush();

            memory.Position = 0;
            var target = new IsiSerializer { Input = memory };

            for (int i = 0; i < Count; i++)
            {
                var message = target.Deserialize();

                Assert.IsInstanceOfType(message, typeof(IsiPut));
                var put = (IsiPut)message;

                Assert.AreEqual(123, put.IsiId);
                Assert.AreEqual(2, put.Items.Count);

                var item = put.Items.Find(o => o.Name == "BlockNo");
                Assert.AreEqual("66102", item.Value);

                item = put.Items.Find(o => o.Name == "ConsecutiveTripNo");
                Assert.AreEqual(i.ToString(CultureInfo.InvariantCulture), item.Value);
            }

            try
            {
                target.Deserialize();
            }
            catch (EndOfStreamException)
            {
                // we only expect this exception at the very end, so we can't use [ExpectedException]
                return;
            }

            Assert.Fail("Did not get EndOfStreamException");
        }
    }
}
