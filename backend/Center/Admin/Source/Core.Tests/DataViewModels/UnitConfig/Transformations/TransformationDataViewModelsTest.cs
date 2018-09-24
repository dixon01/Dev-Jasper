// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationDataViewModelsTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformationDataViewModelsTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Tests.DataViewModels.UnitConfig.Transformations
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Gorba.Center.Admin.Core.DataViewModels.UnitConfig.Transformations;
    using Gorba.Common.Configuration.Protran.Transformations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for all <see cref="TransformationDataViewModelBase"/> implementations.
    /// </summary>
    [TestClass]
    public class TransformationDataViewModelsTest
    {
        /// <summary>
        /// Tests that there is a data view model for each <see cref="TransformationConfig"/>.
        /// If this test fails, you probably forgot to create a DVM for a new config,
        /// or a type that should be abstract isn't abstract.
        /// </summary>
        [TestMethod]
        public void TestAllDataViewModelsDefined()
        {
            var configBaseType = typeof(TransformationConfig);
            var dataViewModelBaseType = typeof(TransformationDataViewModelBase);

            var configTypes =
                configBaseType.Assembly.ExportedTypes.Where(configBaseType.IsAssignableFrom)
                    .Where(t => !t.IsAbstract)
                    .ToList();
            var dataViewModelTypes =
                dataViewModelBaseType.Assembly.ExportedTypes.Where(dataViewModelBaseType.IsAssignableFrom)
                    .Where(t => !t.IsAbstract)
                    .ToList();

            // All binary-to-string transformations are not available
            configTypes.Remove(typeof(Utf8));

            // Integer is not available on purpose (it is generated automatically when needed)
            configTypes.Remove(typeof(Integer));

            foreach (var type in dataViewModelTypes)
            {
                var dataViewModel = (TransformationDataViewModelBase)Activator.CreateInstance(type);
                var config = dataViewModel.CreateConfig();
                Assert.IsTrue(
                    configTypes.Remove(config.GetType()),
                    "{0} has no corresponding config type",
                    type.FullName);
            }

            foreach (var type in configTypes)
            {
                Assert.Fail("Type {0} doesn't have a corresponding DataViewModel", type.FullName);
            }
        }

        /// <summary>
        /// Unit test for <see cref="CapitalizeDataViewModel"/> cloning.
        /// </summary>
        [TestMethod]
        public void TestCapitalizeClone()
        {
            var originalCounter = 0;
            var original = new CapitalizeDataViewModel();
            original.PropertyChanged += (s, e) => originalCounter++;
            original.Exceptions.CollectionChanged += (s, e) => originalCounter++;
            Assert.AreEqual(0, originalCounter);
            original.Exceptions.Add(new ExceptionDataViewModel { Value = "Hello" });
            Assert.AreEqual(1, originalCounter);
            original.Mode = CapitalizeMode.LowerOnly;
            Assert.AreEqual(2, originalCounter);

            var cloneCounter = 0;
            var clone = original.Clone() as CapitalizeDataViewModel;
            Assert.IsNotNull(clone);
            clone.PropertyChanged += (s, e) => cloneCounter++;
            clone.Exceptions.CollectionChanged += (s, e) => cloneCounter++;
            Assert.AreEqual(2, originalCounter);
            Assert.AreEqual(0, cloneCounter);
            Assert.AreEqual(1, clone.Exceptions.Count);
            Assert.AreEqual("Hello", clone.Exceptions[0].Value);
            Assert.AreEqual(CapitalizeMode.LowerOnly, clone.Mode);

            clone.Exceptions.Add(new ExceptionDataViewModel { Value = "World" });
            Assert.AreEqual(2, originalCounter);
            Assert.AreEqual(1, cloneCounter);
            Assert.AreEqual(1, original.Exceptions.Count);
            Assert.AreEqual(2, clone.Exceptions.Count);
            Assert.AreEqual("Hello", original.Exceptions[0].Value);
            Assert.AreEqual("Hello", clone.Exceptions[0].Value);
            Assert.AreEqual("World", clone.Exceptions[1].Value);
        }

        /// <summary>
        /// Unit test for <see cref="CapitalizeDataViewModel"/> to
        /// <see cref="Capitalize"/> conversion (and vice versa).
        /// </summary>
        [TestMethod]
        public void TestCapitalizeCreateConfig()
        {
            var target = new CapitalizeDataViewModel();
            target.Exceptions.Add(new ExceptionDataViewModel { Value = "Hello" });
            target.Mode = CapitalizeMode.LowerOnly;

            var config = target.CreateConfig() as Capitalize;
            Assert.IsNotNull(config);
            Assert.AreEqual(1, config.Exceptions.Length);
            Assert.AreEqual("Hello", config.Exceptions[0]);
            Assert.AreEqual(CapitalizeMode.LowerOnly, config.Mode);

            var dvm = TransformationDataViewModelBase.Create(config) as CapitalizeDataViewModel;
            Assert.IsNotNull(dvm);
            Assert.AreEqual(1, dvm.Exceptions.Count);
            Assert.AreEqual("Hello", dvm.Exceptions[0].Value);
            Assert.AreEqual(CapitalizeMode.LowerOnly, dvm.Mode);
        }

        /// <summary>
        /// Unit test for <see cref="ChainRefDataViewModel"/> to
        /// <see cref="ChainRef"/> conversion (and vice versa).
        /// </summary>
        [TestMethod]
        public void TestChainRefCreateConfig()
        {
            var target = new ChainRefDataViewModel();
            target.TransfRef = "Transf";

            var config = target.CreateConfig() as ChainRef;
            Assert.IsNotNull(config);
            Assert.AreEqual("Transf", config.TransfRef);

            var dvm = TransformationDataViewModelBase.Create(config) as ChainRefDataViewModel;
            Assert.IsNotNull(dvm);
            Assert.AreEqual("Transf", dvm.TransfRef);
        }

        /// <summary>
        /// Unit test for <see cref="JoinDataViewModel"/> to
        /// <see cref="Join"/> conversion (and vice versa).
        /// </summary>
        [TestMethod]
        public void TestJoinCreateConfig()
        {
            var target = new JoinDataViewModel();
            target.Separator = "-";

            var config = target.CreateConfig() as Join;
            Assert.IsNotNull(config);
            Assert.AreEqual("-", config.Separator);

            var dvm = TransformationDataViewModelBase.Create(config) as JoinDataViewModel;
            Assert.IsNotNull(dvm);
            Assert.AreEqual("-", dvm.Separator);
        }

        /// <summary>
        /// Unit test for <see cref="LawoStringDataViewModel"/> to
        /// <see cref="LawoString"/> conversion (and vice versa).
        /// </summary>
        [TestMethod]
        public void TestLawoStringCreateConfig()
        {
            var target = new LawoStringDataViewModel();
            target.CodePage = 858;

            var config = target.CreateConfig() as LawoString;
            Assert.IsNotNull(config);
            Assert.AreEqual(858, config.CodePage);

            var dvm = TransformationDataViewModelBase.Create(config) as LawoStringDataViewModel;
            Assert.IsNotNull(dvm);
            Assert.AreEqual(858, dvm.CodePage);
        }

        /// <summary>
        /// Unit test for <see cref="RegexDividerDataViewModel"/> to
        /// <see cref="RegexDivider"/> conversion (and vice versa).
        /// </summary>
        [TestMethod]
        public void TestRegexDividerCreateConfig()
        {
            var target = new RegexDividerDataViewModel();
            target.Regex = "^0+";
            target.Options = RegexOptions.IgnoreCase;

            var config = target.CreateConfig() as RegexDivider;
            Assert.IsNotNull(config);
            Assert.AreEqual("^0+", config.Regex);
            Assert.AreEqual(RegexOptions.IgnoreCase, config.Options);

            var dvm = TransformationDataViewModelBase.Create(config) as RegexDividerDataViewModel;
            Assert.IsNotNull(dvm);
            Assert.AreEqual(RegexOptions.IgnoreCase, dvm.Options);
        }

        /// <summary>
        /// Unit test for <see cref="RegexMappingDataViewModel"/> cloning.
        /// </summary>
        [TestMethod]
        public void TestRegexMappingClone()
        {
            var originalCounter = 0;
            var original = new RegexMappingDataViewModel();
            original.PropertyChanged += (s, e) => originalCounter++;
            original.Mappings.CollectionChanged += (s, e) => originalCounter++;
            Assert.AreEqual(0, originalCounter);
            original.Mappings.Add(new MappingDataViewModel { From = " +", To = " " });
            Assert.AreEqual(2, originalCounter);

            var cloneCounter = 0;
            var clone = original.Clone() as RegexMappingDataViewModel;
            Assert.IsNotNull(clone);
            clone.PropertyChanged += (s, e) => cloneCounter++;
            clone.Mappings.CollectionChanged += (s, e) => cloneCounter++;
            Assert.AreEqual(2, originalCounter);
            Assert.AreEqual(1, clone.Mappings.Count);
            Assert.AreEqual(" +", clone.Mappings[0].From);

            clone.Mappings.Add(new MappingDataViewModel { From = "[@#]+", To = string.Empty });
            Assert.AreEqual(2, originalCounter);
            Assert.AreEqual(2, cloneCounter);
            Assert.AreEqual(1, original.Mappings.Count);
            Assert.AreEqual(2, clone.Mappings.Count);
            Assert.AreEqual(" +", original.Mappings[0].From);
            Assert.AreEqual(" +", clone.Mappings[0].From);
            Assert.AreEqual("[@#]+", clone.Mappings[1].From);
        }

        /// <summary>
        /// Unit test for <see cref="RegexMappingDataViewModel"/> to
        /// <see cref="RegexMapping"/> conversion (and vice versa).
        /// </summary>
        [TestMethod]
        public void TestRegexMappingCreateConfig()
        {
            var target = new RegexMappingDataViewModel();
            target.Mappings.Add(new MappingDataViewModel { From = " +", To = " " });
            target.Mappings.Add(new MappingDataViewModel { From = "[@#]+", To = string.Empty });

            var config = target.CreateConfig() as RegexMapping;
            Assert.IsNotNull(config);
            Assert.AreEqual(2, config.Mappings.Count);
            Assert.AreEqual(" +", config.Mappings[0].From);
            Assert.AreEqual(" ", config.Mappings[0].To);
            Assert.AreEqual("[@#]+", config.Mappings[1].From);
            Assert.AreEqual(string.Empty, config.Mappings[1].To);

            var dvm = TransformationDataViewModelBase.Create(config) as RegexMappingDataViewModel;
            Assert.IsNotNull(dvm);
            Assert.AreEqual(2, dvm.Mappings.Count);
            Assert.AreEqual(" +", dvm.Mappings[0].From);
            Assert.AreEqual(" ", dvm.Mappings[0].To);
            Assert.AreEqual("[@#]+", dvm.Mappings[1].From);
            Assert.AreEqual(string.Empty, dvm.Mappings[1].To);
        }

        /// <summary>
        /// Unit test for <see cref="ReplaceDataViewModel"/> to
        /// <see cref="Replace"/> conversion (and vice versa).
        /// </summary>
        [TestMethod]
        public void TestReplaceCreateConfig()
        {
            var target = new ReplaceDataViewModel();
            target.CaseSensitive = true;
            target.Mappings.Add(new MappingDataViewModel { From = "Hello", To = "Foo" });
            target.Mappings.Add(new MappingDataViewModel { From = "World", To = "Bar" });

            var config = target.CreateConfig() as Replace;
            Assert.IsNotNull(config);
            Assert.IsTrue(config.CaseSensitive);
            Assert.AreEqual(2, config.Mappings.Count);
            Assert.AreEqual("Hello", config.Mappings[0].From);
            Assert.AreEqual("Foo", config.Mappings[0].To);
            Assert.AreEqual("World", config.Mappings[1].From);
            Assert.AreEqual("Bar", config.Mappings[1].To);

            var dvm = TransformationDataViewModelBase.Create(config) as ReplaceDataViewModel;
            Assert.IsNotNull(dvm);
            Assert.IsTrue(dvm.CaseSensitive);
            Assert.AreEqual(2, dvm.Mappings.Count);
            Assert.AreEqual("Hello", dvm.Mappings[0].From);
            Assert.AreEqual("Foo", dvm.Mappings[0].To);
            Assert.AreEqual("World", dvm.Mappings[1].From);
            Assert.AreEqual("Bar", dvm.Mappings[1].To);
        }

        /// <summary>
        /// Unit test for <see cref="ReverseDataViewModel"/> to
        /// <see cref="Reverse"/> conversion (and vice versa).
        /// </summary>
        [TestMethod]
        public void TestReverseCreateConfig()
        {
            var target = new ReverseDataViewModel();

            var config = target.CreateConfig() as Reverse;
            Assert.IsNotNull(config);

            var dvm = TransformationDataViewModelBase.Create(config) as ReverseDataViewModel;
            Assert.IsNotNull(dvm);
        }

        /// <summary>
        /// Unit test for <see cref="StringMappingDataViewModel"/> to
        /// <see cref="StringMapping"/> conversion (and vice versa).
        /// </summary>
        [TestMethod]
        public void TestStringMappingCreateConfig()
        {
            var target = new StringMappingDataViewModel();
            target.Mappings.Add(new MappingDataViewModel { From = "Hello", To = "Foo" });
            target.Mappings.Add(new MappingDataViewModel { From = "World", To = "Bar" });

            var config = target.CreateConfig() as StringMapping;
            Assert.IsNotNull(config);
            Assert.AreEqual(2, config.Mappings.Count);
            Assert.AreEqual("Hello", config.Mappings[0].From);
            Assert.AreEqual("Foo", config.Mappings[0].To);
            Assert.AreEqual("World", config.Mappings[1].From);
            Assert.AreEqual("Bar", config.Mappings[1].To);

            var dvm = TransformationDataViewModelBase.Create(config) as StringMappingDataViewModel;
            Assert.IsNotNull(dvm);
            Assert.AreEqual(2, dvm.Mappings.Count);
            Assert.AreEqual("Hello", dvm.Mappings[0].From);
            Assert.AreEqual("Foo", dvm.Mappings[0].To);
            Assert.AreEqual("World", dvm.Mappings[1].From);
            Assert.AreEqual("Bar", dvm.Mappings[1].To);
        }

        /// <summary>
        /// Unit test for <see cref="SymbolDividerDataViewModel"/> to
        /// <see cref="SymbolDivider"/> conversion (and vice versa).
        /// </summary>
        [TestMethod]
        public void TestSymbolDividerCreateConfig()
        {
            var target = new SymbolDividerDataViewModel();
            target.Symbol = "-";

            var config = target.CreateConfig() as SymbolDivider;
            Assert.IsNotNull(config);
            Assert.AreEqual("-", config.Symbol);

            var dvm = TransformationDataViewModelBase.Create(config) as SymbolDividerDataViewModel;
            Assert.IsNotNull(dvm);
            Assert.AreEqual("-", dvm.Symbol);
        }
    }
}
