// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularBufferTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for CircularBufferTest and is intended
//   to contain all CircularBufferTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Tests.Buffers
{
    using System;
    using System.Text;

    using Gorba.Motion.Protran.Core.Buffers;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for CircularBufferTest and is intended
    /// to contain all CircularBufferTest Unit Tests
    /// </summary>
    [TestClass]
    public class CircularBufferTest
    {
        /// <summary>
        /// A test for Clear
        /// </summary>
        [TestMethod]
        public void ClearTest()
        {
            const int Size = 128;
            var circBuffer = new CircularBuffer(Size);

            const string Original = "Hello World!";
            byte[] buffer = Encoding.ASCII.GetBytes(Original);
            Array.Copy(buffer, 0, circBuffer.Buffer, circBuffer.Tail, buffer.Length);
            circBuffer.UpdateTail(buffer.Length);

            circBuffer.Clear();
            Assert.AreEqual(0, circBuffer.CurrentLength);
        }

        /// <summary>
        /// A test for GetBufferPiece
        /// </summary>
        [TestMethod]
        public void GetBufferPieceTest()
        {
            const int Size = 128;
            var circBuffer = new CircularBuffer(Size);

            const string Original = "Hello World!";
            byte[] buffer = Encoding.ASCII.GetBytes(Original);
            Array.Copy(buffer, 0, circBuffer.Buffer, circBuffer.Tail, buffer.Length);
            circBuffer.UpdateTail(buffer.Length);

            byte[] piece = circBuffer.GetBufferPiece(buffer.Length);
            string result = Encoding.ASCII.GetString(piece);
            Assert.AreEqual(Original, result);
        }

        /// <summary>
        /// A test for IsIndexBetweenHeadAndTail
        /// </summary>
        [TestMethod]
        public void IsIndexBetweenHeadAndTailTest()
        {
            const int Size = 128;
            var circBuffer = new CircularBuffer(Size);

            const string Original = "Hello World!";
            byte[] buffer = Encoding.ASCII.GetBytes(Original);
            Array.Copy(buffer, 0, circBuffer.Buffer, circBuffer.Tail, buffer.Length);
            circBuffer.UpdateTail(buffer.Length);

            for (int i = 0; i < buffer.Length; i++)
            {
                Assert.IsTrue(circBuffer.IsIndexBetweenHeadAndTail(i));
            }
        }

        /// <summary>
        /// A test for IsTailBiggerThan
        /// </summary>
        [TestMethod]
        public void IsTailBiggerThanTest()
        {
            const int Size = 128;
            var circBuffer = new CircularBuffer(Size);

            const string Original = "Hello World!";
            byte[] buffer = Encoding.ASCII.GetBytes(Original);
            Array.Copy(buffer, 0, circBuffer.Buffer, circBuffer.Tail, buffer.Length);
            circBuffer.UpdateTail(buffer.Length);

            for (int i = 0; i < buffer.Length - 1; i++)
            {
                Assert.IsTrue(circBuffer.IsTailBiggerThan(i));
            }
        }

        /// <summary>
        /// A test for UpdateHead
        /// </summary>
        [TestMethod]
        public void UpdateHeadTest()
        {
            const int Size = 128;
            var circBuffer = new CircularBuffer(Size);

            const string Original = "Hello World!";
            byte[] buffer = Encoding.ASCII.GetBytes(Original);
            Array.Copy(buffer, 0, circBuffer.Buffer, circBuffer.Tail, buffer.Length);
            circBuffer.UpdateTail(buffer.Length);

            byte[] piece = circBuffer.GetBufferPiece(buffer.Length);
            circBuffer.UpdateHead(piece.Length);
            Assert.AreEqual(piece.Length, circBuffer.Head);
        }

        /// <summary>
        /// A test for UpdateTail
        /// </summary>
        [TestMethod]
        public void UpdateTailTest()
        {
            const int Size = 128;
            var circBuffer = new CircularBuffer(Size);

            const string Original = "Hello World!";
            byte[] buffer = Encoding.ASCII.GetBytes(Original);
            Array.Copy(buffer, 0, circBuffer.Buffer, circBuffer.Tail, buffer.Length);
            circBuffer.UpdateTail(buffer.Length);

            Assert.AreEqual(buffer.Length, circBuffer.Tail);
        }

        /// <summary>
        /// A test for Buffer
        /// </summary>
        [TestMethod]
        public void GetBufferTest()
        {
            const int Size = 128;
            var circBuffer = new CircularBuffer(Size);

            Assert.AreEqual(circBuffer.Buffer.Length, Size);
        }

        /// <summary>
        /// A test for CurrentLength
        /// </summary>
        [TestMethod]
        public void CurrentLengthTest()
        {
            const int Size = 128;
            var circBuffer = new CircularBuffer(Size);

            const string Original = "Hello World!";
            byte[] buffer = Encoding.ASCII.GetBytes(Original);
            Array.Copy(buffer, 0, circBuffer.Buffer, circBuffer.Tail, buffer.Length);
            circBuffer.UpdateTail(buffer.Length);

            byte[] piece = circBuffer.GetBufferPiece(buffer.Length);
            circBuffer.UpdateHead(piece.Length);
            Assert.AreEqual(0, circBuffer.CurrentLength);
        }

        /// <summary>
        /// A test for RemainingLength
        /// </summary>
        [TestMethod]
        public void RemainingLengthTest()
        {
            const int Size = 128;
            var circBuffer = new CircularBuffer(Size);

            const string Original = "Hello World!";
            byte[] buffer = Encoding.ASCII.GetBytes(Original);
            Array.Copy(buffer, 0, circBuffer.Buffer, circBuffer.Tail, buffer.Length);
            circBuffer.UpdateTail(buffer.Length);

            Assert.AreEqual(Size - buffer.Length, circBuffer.RemainingLength);
        }

        /// <summary>
        /// A test for Tail
        /// </summary>
        [TestMethod]
        [DeploymentItem("Gorba.Motion.Protran.Protocols.dll")]
        public void TailTest()
        {
            const int Size = 128;
            var circBuffer = new CircularBuffer(Size);

            const string Original = "Hello World!";
            byte[] buffer = Encoding.ASCII.GetBytes(Original);
            Array.Copy(buffer, 0, circBuffer.Buffer, circBuffer.Tail, buffer.Length);
            circBuffer.UpdateTail(buffer.Length);

            Assert.AreEqual(buffer.Length, circBuffer.Tail);
        }
    }
}
