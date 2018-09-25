// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoTextFrameProviderTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AutoTextFrameProviderTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Tests.Providers
{
    using Gorba.Common.Protocols.Ahdlc.Frames;
    using Gorba.Common.Protocols.Ahdlc.Providers;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="AutoTextFrameProvider"/>.
    /// </summary>
    [TestClass]
    public class AutoTextFrameProviderTest
    {
        /// <summary>
        /// Tests the <see cref="AutoTextFrameProvider"/> with a single text.
        /// </summary>
        [TestMethod]
        public void SingleTextTest()
        {
            var target = new AutoTextFrameProvider(2, 5, "Hello World");

            var setup = target.SetupCommand;
            Assert.IsNotNull(setup);

            Assert.AreEqual(DisplayMode.AutoText, setup.Mode);
            Assert.AreEqual(1, setup.DataBlockCount);
            Assert.AreEqual(0, setup.ScrollBlocks.Count);
            Assert.IsNotNull(setup.Data);
            Assert.AreEqual(6, setup.Data.Length);
            Assert.AreEqual(3, setup.Data[3]);
            Assert.AreEqual(2, setup.Data[4]);
            Assert.AreEqual(5, setup.Data[5]);
            Assert.AreEqual(1, setup.Data[6]);
            Assert.AreEqual(0, setup.Data[7]);
            Assert.AreEqual(0, setup.Data[8]);

            var en = target.GetOutputCommands().GetEnumerator();
            Assert.IsTrue(en.MoveNext());

            var output = en.Current;
            Assert.IsNotNull(output);
            Assert.IsNotNull(output.Data);
            Assert.AreEqual(12, output.Data.Length);
            Assert.AreEqual((byte)'H', output.Data[0]);
            Assert.AreEqual((byte)'e', output.Data[1]);
            Assert.AreEqual((byte)'l', output.Data[2]);
            Assert.AreEqual((byte)'l', output.Data[3]);
            Assert.AreEqual((byte)'o', output.Data[4]);
            Assert.AreEqual((byte)' ', output.Data[5]);
            Assert.AreEqual((byte)'W', output.Data[6]);
            Assert.AreEqual((byte)'o', output.Data[7]);
            Assert.AreEqual((byte)'r', output.Data[8]);
            Assert.AreEqual((byte)'l', output.Data[9]);
            Assert.AreEqual((byte)'d', output.Data[10]);
            Assert.AreEqual((byte)'\r', output.Data[11]);

            Assert.IsFalse(en.MoveNext());
        }

        /// <summary>
        /// Tests the <see cref="AutoTextFrameProvider"/> with multiple texts.
        /// </summary>
        [TestMethod]
        public void MultiTextTest()
        {
            var target = new AutoTextFrameProvider(2, 5, "Hello", "World");

            var setup = target.SetupCommand;
            Assert.IsNotNull(setup);

            Assert.AreEqual(DisplayMode.AutoText, setup.Mode);
            Assert.AreEqual(1, setup.DataBlockCount);
            Assert.AreEqual(0, setup.ScrollBlocks.Count);
            Assert.IsNotNull(setup.Data);
            Assert.AreEqual(6, setup.Data.Length);
            Assert.AreEqual(3, setup.Data[3]);
            Assert.AreEqual(2, setup.Data[4]);
            Assert.AreEqual(5, setup.Data[5]);
            Assert.AreEqual(1, setup.Data[6]);
            Assert.AreEqual(0, setup.Data[7]);
            Assert.AreEqual(0, setup.Data[8]);

            var en = target.GetOutputCommands().GetEnumerator();
            Assert.IsTrue(en.MoveNext());

            var output = en.Current;
            Assert.IsNotNull(output);
            Assert.IsNotNull(output.Data);
            Assert.AreEqual(12, output.Data.Length);
            Assert.AreEqual((byte)'H', output.Data[0]);
            Assert.AreEqual((byte)'e', output.Data[1]);
            Assert.AreEqual((byte)'l', output.Data[2]);
            Assert.AreEqual((byte)'l', output.Data[3]);
            Assert.AreEqual((byte)'o', output.Data[4]);
            Assert.AreEqual((byte)'\n', output.Data[5]);
            Assert.AreEqual((byte)'W', output.Data[6]);
            Assert.AreEqual((byte)'o', output.Data[7]);
            Assert.AreEqual((byte)'r', output.Data[8]);
            Assert.AreEqual((byte)'l', output.Data[9]);
            Assert.AreEqual((byte)'d', output.Data[10]);
            Assert.AreEqual((byte)'\r', output.Data[11]);

            Assert.IsFalse(en.MoveNext());
        }

        /// <summary>
        /// Tests the <see cref="AutoTextFrameProvider"/> with a single long text.
        /// </summary>
        [TestMethod]
        public void LongTextTest()
        {
            var target = new AutoTextFrameProvider(2, 5, "Hello World, this text is more than 30 characters");

            var setup = target.SetupCommand;
            Assert.IsNotNull(setup);

            Assert.AreEqual(DisplayMode.AutoText, setup.Mode);
            Assert.AreEqual(2, setup.DataBlockCount);
            Assert.AreEqual(0, setup.ScrollBlocks.Count);
            Assert.IsNotNull(setup.Data);
            Assert.AreEqual(6, setup.Data.Length);
            Assert.AreEqual(3, setup.Data[3]);
            Assert.AreEqual(2, setup.Data[4]);
            Assert.AreEqual(5, setup.Data[5]);
            Assert.AreEqual(1, setup.Data[6]);
            Assert.AreEqual(0, setup.Data[7]);
            Assert.AreEqual(0, setup.Data[8]);

            var en = target.GetOutputCommands().GetEnumerator();
            Assert.IsTrue(en.MoveNext());

            var output = en.Current;
            Assert.IsNotNull(output);
            Assert.IsNotNull(output.Data);
            Assert.AreEqual(30, output.Data.Length);
            Assert.AreEqual((byte)'H', output.Data[0]);
            Assert.AreEqual((byte)'e', output.Data[1]);
            Assert.AreEqual((byte)'l', output.Data[2]);
            Assert.AreEqual((byte)'l', output.Data[3]);
            Assert.AreEqual((byte)'o', output.Data[4]);
            Assert.AreEqual((byte)' ', output.Data[5]);
            Assert.AreEqual((byte)'W', output.Data[6]);
            Assert.AreEqual((byte)'o', output.Data[7]);
            Assert.AreEqual((byte)'r', output.Data[8]);
            Assert.AreEqual((byte)'l', output.Data[9]);
            Assert.AreEqual((byte)'d', output.Data[10]);

            // ... and so on ...
            Assert.AreEqual((byte)'e', output.Data[29]);

            Assert.IsTrue(en.MoveNext());

            output = en.Current;
            Assert.IsNotNull(output);
            Assert.IsNotNull(output.Data);
            Assert.AreEqual(20, output.Data.Length);
            Assert.AreEqual((byte)' ', output.Data[0]);
            Assert.AreEqual((byte)'t', output.Data[1]);
            Assert.AreEqual((byte)'h', output.Data[2]);
            Assert.AreEqual((byte)'a', output.Data[3]);
            Assert.AreEqual((byte)'n', output.Data[4]);
            Assert.AreEqual((byte)' ', output.Data[5]);
            Assert.AreEqual((byte)'3', output.Data[6]);
            Assert.AreEqual((byte)'0', output.Data[7]);
            Assert.AreEqual((byte)' ', output.Data[8]);
            Assert.AreEqual((byte)'c', output.Data[9]);
            Assert.AreEqual((byte)'h', output.Data[10]);

            // ... and so on ...
            Assert.AreEqual((byte)'\r', output.Data[19]);

            Assert.IsFalse(en.MoveNext());
        }
    }
}
