// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureComponentRequirementsTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.Compatibility
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Compatibility;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for <see cref="SoftwareComponentVersion"/>.
    /// </summary>
    [TestClass]
    public class FeatureComponentRequirementsTest
    {
        /// <summary>
        /// The merge requirements test.
        /// </summary>
        [TestMethod]
        public void MergeRequirementsTest()
        {
            var currentRequirements = new List<FeatureComponentRequirements.SoftwareConfig>();

            // adds entries
            var newRequirements = new List<FeatureComponentRequirements.SoftwareConfig>();
            newRequirements.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer,
                    new SoftwareComponentVersion("2.3")));

            FeatureComponentRequirements.MergeRequirements(currentRequirements, newRequirements);
            Assert.IsTrue(currentRequirements.Count == 1);
            Assert.IsTrue(
                currentRequirements.Any(
                    r =>
                    r.Component == FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer
                    && r.Version == new SoftwareComponentVersion("2.3")));

            // increases version
            newRequirements.Clear();
            newRequirements.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer,
                    new SoftwareComponentVersion("2.4")));
            FeatureComponentRequirements.MergeRequirements(currentRequirements, newRequirements);
            Assert.IsTrue(currentRequirements.Count == 1);
            Assert.IsTrue(
                currentRequirements.Any(
                    r =>
                    r.Component == FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer
                    && r.Version == new SoftwareComponentVersion("2.4")));

            // does not cange on minor version
            newRequirements.Clear();
            newRequirements.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer,
                    new SoftwareComponentVersion("1.4")));
            FeatureComponentRequirements.MergeRequirements(currentRequirements, newRequirements);
            Assert.IsTrue(currentRequirements.Count == 1);
            Assert.IsTrue(
                currentRequirements.Any(
                    r =>
                    r.Component == FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer
                    && r.Version == new SoftwareComponentVersion("2.4")));

            // add many
            newRequirements.Clear();
            newRequirements.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer,
                    new SoftwareComponentVersion("2.5.2")));
            newRequirements.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.Protran,
                    new SoftwareComponentVersion("2.1.2345")));
            newRequirements.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.Protran,
                    new SoftwareComponentVersion("2.2.1111")));
            FeatureComponentRequirements.MergeRequirements(currentRequirements, newRequirements);
            Assert.IsTrue(currentRequirements.Count == 2);
            Assert.IsTrue(
                currentRequirements.Any(
                    r =>
                    r.Component == FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer
                    && r.Version == new SoftwareComponentVersion("2.5.2")));
            Assert.IsTrue(
                currentRequirements.Any(
                    r =>
                    r.Component == FeatureComponentRequirements.SoftwareComponent.Protran
                    && r.Version == new SoftwareComponentVersion("2.2.1111")));
        }

        /// <summary>
        /// The requirements ok test version false.
        /// </summary>
        [TestMethod]
        public void RequirementsOkTestVersionFalse()
        {
            var requirements = new List<FeatureComponentRequirements.SoftwareConfig>();
            requirements.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer,
                    new SoftwareComponentVersion("2.3")));
            requirements.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.Protran,
                    new SoftwareComponentVersion("2.1")));

            var available = new List<FeatureComponentRequirements.SoftwareConfig>();
            available.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer,
                    new SoftwareComponentVersion("2.1")));
            available.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.Protran,
                    new SoftwareComponentVersion("1.0")));

            Assert.IsFalse(FeatureComponentRequirements.RequirementsOk(available, requirements));
        }

        /// <summary>
        /// The requirements ok test version missing info.
        /// </summary>
        [TestMethod]
        public void RequirementsOkTestVersionMissingInfo()
        {
            var requirements = new List<FeatureComponentRequirements.SoftwareConfig>();
            requirements.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer,
                    new SoftwareComponentVersion("2.3")));

            var available = new List<FeatureComponentRequirements.SoftwareConfig>();
            available.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.Protran,
                    new SoftwareComponentVersion("2.1")));

            Assert.IsFalse(FeatureComponentRequirements.RequirementsOk(available, requirements));
        }

        /// <summary>
        /// The requirements ok test version true.
        /// </summary>
        [TestMethod]
        public void RequirementsOkTestVersionTrue()
        {
            var requirements = new List<FeatureComponentRequirements.SoftwareConfig>();
            requirements.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer,
                    new SoftwareComponentVersion("2.3")));
            requirements.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.Protran,
                    new SoftwareComponentVersion("2.1")));

            var available = new List<FeatureComponentRequirements.SoftwareConfig>();
            available.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer,
                    new SoftwareComponentVersion("2.3")));
            available.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.Protran,
                    new SoftwareComponentVersion("3.2")));

            Assert.IsTrue(FeatureComponentRequirements.RequirementsOk(available, requirements));
        }

        /// <summary>
        /// The get lowest versions with same components.
        /// </summary>
        [TestMethod]
        public void GetLowestVersions()
        {
            var list1 = new List<FeatureComponentRequirements.SoftwareConfig>();
            list1.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer,
                    new SoftwareComponentVersion("1.3")));
            list1.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.Protran,
                    new SoftwareComponentVersion("2.0")));

            var list2 = new List<FeatureComponentRequirements.SoftwareConfig>();
            list2.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer,
                    new SoftwareComponentVersion("1.0")));
            list2.Add(
                new FeatureComponentRequirements.SoftwareConfig(
                    FeatureComponentRequirements.SoftwareComponent.Protran,
                    new SoftwareComponentVersion("2.0")));
            list2.Add(
            new FeatureComponentRequirements.SoftwareConfig(
                FeatureComponentRequirements.SoftwareComponent.Composer,
                new SoftwareComponentVersion("1.2")));

            var testLists = new List<List<FeatureComponentRequirements.SoftwareConfig>> { list1, list2 };
            var lowestVersions = FeatureComponentRequirements.GetLowestVersions(testLists);

            Assert.IsTrue(lowestVersions != null);
            Assert.IsTrue(lowestVersions.Count == 3);
            Assert.IsTrue(lowestVersions.Any(c =>
                c.Component == FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer
                && c.Version == new SoftwareComponentVersion("1.0")));
            Assert.IsTrue(lowestVersions.Any(c =>
                c.Component == FeatureComponentRequirements.SoftwareComponent.Protran
                && c.Version == new SoftwareComponentVersion("2.0")));
            Assert.IsTrue(lowestVersions.Any(c =>
                c.Component == FeatureComponentRequirements.SoftwareComponent.Composer
                && c.Version == new SoftwareComponentVersion("1.2")));
        }
    }
}
