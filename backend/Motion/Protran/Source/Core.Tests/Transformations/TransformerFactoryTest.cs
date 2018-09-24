// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformerFactoryTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformerFactoryTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Tests.Transformations
{
    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Motion.Protran.Core.Transformations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="TransformerFactory"/>.
    /// </summary>
    [TestClass]
    public class TransformerFactoryTest
    {
        /// <summary>
        /// Verifies that the <see cref="TransformerFactory"/> properly creates a <see cref="ReplaceTransformer"/>.
        /// </summary>
        [TestMethod]
        public void CreateReplaceTransformer()
        {
            var config = new Replace { Mappings = { new Mapping { From = "Foo", To = "Bar" } } };
            var transformer = TransformerFactory.CreateTransformer(config, typeof(string));
            Assert.IsInstanceOfType(transformer, typeof(ReplaceTransformer));
        }

        /// <summary>
        /// Verifies that the <see cref="TransformerFactory"/> properly creates a
        /// <see cref="ArrayTransformerWrapper{TInput,TOutput,TConfig}"/> if needed.
        /// </summary>
        [TestMethod]
        public void CreateWrapperTransformer()
        {
            var config = new Replace { Mappings = { new Mapping { From = "Foo", To = "Bar" } } };
            var transformer = TransformerFactory.CreateTransformer(config, typeof(string[]));
            Assert.IsInstanceOfType(transformer, typeof(ArrayTransformerWrapper<string, string, Replace>));
        }
    }
}
