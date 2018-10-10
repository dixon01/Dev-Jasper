// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpMessageQueueTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a test class for HttpMessageQueueTest.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Tests.Communication
{
    using Gorba.Common.Protocols.Vdv453.Communication;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class to test the HTTP message queue.
    /// </summary>
    [TestClass]
    public class HttpMessageQueueTest
    {
        /// <summary>
        /// A test for testing the HttpMessageQueue
        /// </summary>
        [TestMethod]
        public void TestHttpMessageQueue()
        {
            // create message queue
            var queue = new HttpMessageQueue();

            Assert.IsNotNull(queue);
            Assert.AreEqual(0, queue.Count);

            string message = queue.Peek();
            Assert.IsNull(message);

            message = queue.Dequeue();
            Assert.IsNull(message);

            // insert a message into queue
            string oriMessage = "This is a test message";
            queue.Enqueue(oriMessage);

            Assert.AreEqual(1, queue.Count);

            message = queue.Peek();

            Assert.IsNotNull(message);
            Assert.AreEqual(1, queue.Count);
            Assert.AreEqual(oriMessage, message);

            message = queue.Dequeue();

            Assert.IsNotNull(message);
            Assert.AreEqual(0, queue.Count);
            Assert.AreEqual(oriMessage, message);

            message = queue.Peek();
            Assert.IsNull(message);

            message = queue.Dequeue();
            Assert.IsNull(message);
        }
    }
}