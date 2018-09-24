// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimplePortTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimplePortTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Tests
{
    using Gorba.Common.Gioom.Core.Values;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit test for <see cref="SimplePort"/>.
    /// </summary>
    [TestClass]
    public class SimplePortTest
    {
        /// <summary>
        /// Tests that the <see cref="PortBase.ValueChanged"/> is triggered correctly.
        /// </summary>
        [TestMethod]
        public void TestValueChanged()
        {
            var target = new SimplePort("ValueChanged", true, false, new FlagValues(), FlagValues.False);

            var changes = 0;
            target.ValueChanged += (s, e) => changes++;
            Assert.AreEqual(0, changes);

            target.Value = FlagValues.True;
            Assert.AreEqual(FlagValues.True, target.Value);
            Assert.AreEqual(1, changes);

            target.Value = FlagValues.True;
            Assert.AreEqual(FlagValues.True, target.Value);
            Assert.AreEqual(1, changes);

            target.Value = FlagValues.False;
            Assert.AreEqual(FlagValues.False, target.Value);
            Assert.AreEqual(2, changes);
        }
    }
}