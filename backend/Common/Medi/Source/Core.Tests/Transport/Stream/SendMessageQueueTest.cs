// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendMessageQueueTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SendMessageQueueTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transport.Stream
{
    using System;

    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Transport.Stream;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="SendMessageQueue"/>
    /// </summary>
    [TestClass]
    public class SendMessageQueueTest
    {
        /// <summary>
        /// Tests that the <see cref="SendMessageQueue.Count"/> property 
        /// behaves correctly when using <see cref="SendMessageQueue.Enqueue"/>
        /// and <see cref="SendMessageQueue.Dequeue"/>.
        /// </summary>
        [TestMethod]
        public void EnqueueDequeueTest()
        {
            uint frameId = 0;
            var target = new SendMessageQueue();
            for (int i = 0; i < 20; i++)
            {
                target.Enqueue(new SendMessageBuffer(++frameId));
            }

            Assert.AreEqual(20, target.Count);

            for (int i = 1; i <= 20; i++)
            {
                var buffer = target.Dequeue();
                Assert.AreEqual(20 - i, target.Count);
                Assert.AreEqual((uint)i, buffer.FrameId);
            }
        }

        /// <summary>
        /// Tests that the <see cref="SendMessageQueue.Count"/> property 
        /// behaves correctly when using <see cref="SendMessageQueue.Clear"/>.
        /// </summary>
        [TestMethod]
        public void ClearTest()
        {
            uint frameId = 0;
            var target = new SendMessageQueue();
            for (int i = 0; i < 20; i++)
            {
                target.Enqueue(new SendMessageBuffer(++frameId));
            }

            Assert.AreEqual(20, target.Count);

            target.Clear();

            Assert.AreEqual(0, target.Count);

            target.Restart();

            Assert.AreEqual(0, target.Count);
        }

        /// <summary>
        /// Tests that <see cref="SendMessageQueue.Dequeue"/> throws an
        /// <see cref="InvalidOperationException"/> if the queue is empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DequeueEmptyTest()
        {
            var target = new SendMessageQueue();
            target.Dequeue();
        }

        /// <summary>
        /// Tests that <see cref="SendMessageQueue.Acknowledge"/> and 
        /// <see cref="SendMessageQueue.Restart"/> work properly.
        /// </summary>
        [TestMethod]
        public void AcknowledgeRestartTest()
        {
            uint frameId = 0;
            var target = new SendMessageQueue();
            target.FramingEnabled = true;
            for (int i = 0; i < 20; i++)
            {
                target.Enqueue(new SendMessageBuffer(++frameId));
            }

            Assert.AreEqual(20, target.Count);

            for (int i = 0; i < 5; i++)
            {
                target.Dequeue();
            }

            Assert.AreEqual(15, target.Count);

            target.Acknowledge(4);

            Assert.AreEqual(15, target.Count);

            for (int i = 0; i < 5; i++)
            {
                target.Dequeue();
            }

            Assert.AreEqual(10, target.Count);

            target.Acknowledge(8);

            Assert.AreEqual(10, target.Count);

            target.Restart();

            Assert.AreEqual(12, target.Count);

            for (uint i = 9; i <= 20; i++)
            {
                var buffer = target.Dequeue();
                Assert.AreEqual(i, buffer.FrameId);
            }

            Assert.AreEqual(0, target.Count);

            target.Acknowledge(17);

            Assert.AreEqual(0, target.Count);

            target.Restart();

            Assert.AreEqual(3, target.Count);

            for (uint i = 18; i <= 20; i++)
            {
                var buffer = target.Dequeue();
                Assert.AreEqual(i, buffer.FrameId);
            }

            Assert.AreEqual(0, target.Count);

            target.Acknowledge(20);

            Assert.AreEqual(0, target.Count);

            target.Restart();

            Assert.AreEqual(0, target.Count);
        }

        /// <summary>
        /// Tests that <see cref="SendMessageQueue.Acknowledge"/> throws an
        /// <see cref="ArgumentOutOfRangeException"/> if it is called with a
        /// frame id greater than the last sent buffer.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AcknwoledgeBadIdTest()
        {
            var target = new SendMessageQueue();
            target.FramingEnabled = true;
            target.Enqueue(new SendMessageBuffer(1));
            target.Dequeue();

            target.Acknowledge(2);
        }
    }
}
