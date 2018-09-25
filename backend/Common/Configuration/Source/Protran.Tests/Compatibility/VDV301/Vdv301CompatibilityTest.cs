// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301CompatibilityTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv301CompatibilityTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Tests.Compatibility.Vdv301
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.VDV301;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The VDV 301 config file compatibility test.
    /// </summary>
    [TestClass]
    public class Vdv301CompatibilityTest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Tests <c>vdv301.xml</c> version 2.2.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\VDV301\vdv301_v2.2.xml")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method.")]
        public void TestV2_2()
        {
            var configManager = new ConfigManager<Vdv301ProtocolConfig>();
            configManager.FileName = "vdv301_v2.2.xml";
            configManager.XmlSchema = Vdv301ProtocolConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.IsNotNull(config.Services);
            Assert.IsNotNull(config.Services.CustomerInformationService);
            Assert.IsNull(config.Services.CustomerInformationService.Host);
            Assert.IsNull(config.Services.CustomerInformationService.Path);
            Assert.AreEqual(0, config.Services.CustomerInformationService.Port);

            Assert.IsNotNull(config.Services.CustomerInformationService.GetAllData);
            Assert.IsTrue(config.Services.CustomerInformationService.GetAllData.Subscribe);
            Assert.AreEqual(TimeSpan.Zero, config.Services.CustomerInformationService.GetAllData.SubscriptionTimeout);
            Assert.IsNotNull(config.Services.CustomerInformationService.GetAllData.TripInformation);
            Assert.IsNotNull(config.Services.CustomerInformationService.GetAllData.TripInformation.StopSequence);

            var stopPoint =
                config.Services.CustomerInformationService.GetAllData.TripInformation.StopSequence.StopPoint;
            Assert.IsNotNull(stopPoint);
            Assert.AreEqual(1, stopPoint.StopName.Count);

            var stopName = stopPoint.StopName[0];
            Assert.IsTrue(stopName.Enabled);
            Assert.IsNull(stopName.TransfRef);
            AssertAreEqual("0", "Stops", "StopName", "{0}", stopName);

            Assert.IsNotNull(stopPoint.DisplayContent);
            Assert.IsNotNull(stopPoint.DisplayContent.LineInformation);
            Assert.AreEqual(1, stopPoint.DisplayContent.LineInformation.LineNumber.Count);

            var lineNumber = stopPoint.DisplayContent.LineInformation.LineNumber[0];
            Assert.IsTrue(lineNumber.Enabled);
            Assert.IsNull(lineNumber.TransfRef);
            AssertAreEqual("0", "Route", "Line", "0", lineNumber);

            Assert.IsNotNull(stopPoint.DisplayContent.Destination);
            Assert.AreEqual(1, stopPoint.DisplayContent.Destination.DestinationName.Count);
            var destinationName = stopPoint.DisplayContent.Destination.DestinationName[0];
            Assert.IsTrue(destinationName.Enabled);
            Assert.IsNull(destinationName.TransfRef);
            AssertAreEqual("0", "Destination", "DestinationName", "0", destinationName);

            Assert.IsNotNull(stopPoint.Connection);
            Assert.IsNotNull(stopPoint.Connection.DisplayContent);
            Assert.IsNotNull(stopPoint.Connection.DisplayContent.Destination);
            Assert.AreEqual(1, stopPoint.Connection.DisplayContent.Destination.DestinationName.Count);

            destinationName = stopPoint.Connection.DisplayContent.Destination.DestinationName[0];
            Assert.IsTrue(destinationName.Enabled);
            Assert.IsNull(destinationName.TransfRef);
            AssertAreEqual("0", "Connections", "ConnectionDestinationName", "{0}", destinationName);

            Assert.AreEqual(1, stopPoint.Connection.Platform.Count);

            var platform = stopPoint.Connection.Platform[0];
            Assert.IsTrue(platform.Enabled);
            Assert.IsNull(platform.TransfRef);
            AssertAreEqual("0", "Connections", "ConnectionPlatform", "{0}", platform);

            Assert.IsNotNull(stopPoint.Connection.TransportMode);
            Assert.AreEqual(1, stopPoint.Connection.TransportMode.Name.Count);

            var transportMode = stopPoint.Connection.TransportMode.Name[0];
            Assert.IsTrue(transportMode.Enabled);
            Assert.IsNull(transportMode.TransfRef);
            AssertAreEqual("0", "Connections", "ConnectionTransportType", "{0}", transportMode);

            Assert.AreEqual(1, config.Services.CustomerInformationService.GetAllData.DoorState.Count);
            var doorState = config.Services.CustomerInformationService.GetAllData.DoorState[0];
            Assert.IsTrue(doorState.Enabled);
            Assert.AreEqual("DoorStatus", doorState.TransfRef);
            AssertAreEqual("0", "SystemStatus", "DoorStatus", "0", doorState);

            Assert.AreEqual(1, config.Services.CustomerInformationService.GetAllData.VehicleStopRequested.Count);
            var stopRequested = config.Services.CustomerInformationService.GetAllData.VehicleStopRequested[0];
            Assert.IsTrue(stopRequested.Enabled);
            Assert.IsNull(stopRequested.TransfRef);
            AssertAreEqual("0", "SystemStatus", "StopRequestedState", "0", stopRequested);

            Assert.AreEqual(1, config.Services.CustomerInformationService.GetAllData.ExitSide.Count);
            var exitSide = config.Services.CustomerInformationService.GetAllData.ExitSide[0];
            Assert.IsFalse(exitSide.Enabled);
            Assert.AreEqual("ExitSide", exitSide.TransfRef);
            AssertAreEqual("0", "SystemStatus", "ExitSide", "0", exitSide);

            Assert.IsNull(config.Services.CustomerInformationService.GetCurrentAnnouncement);
            Assert.IsNull(config.Services.CustomerInformationService.GetCurrentConnectionInformation);
            Assert.IsNull(config.Services.CustomerInformationService.GetCurrentDisplayContent);
            Assert.IsNull(config.Services.CustomerInformationService.GetCurrentStopIndex);
            Assert.IsNull(config.Services.CustomerInformationService.GetCurrentStopPoint);
            Assert.IsNull(config.Services.CustomerInformationService.GetTripData);
            Assert.IsNull(config.Services.CustomerInformationService.GetVehicleData);

            Assert.AreEqual(1, config.Languages.Count);

            var language = config.Languages[0];
            Assert.AreEqual("de", language.Vdv301Input);
            Assert.AreEqual("default", language.XimpleOutput);

            Assert.AreEqual(2, config.Transformations.Count);

            var chain = config.Transformations[0];
            Assert.AreEqual("DoorStatus", chain.Id);
            Assert.AreEqual(1, chain.Transformations.Count);

            chain = config.Transformations[1];
            Assert.AreEqual("ExitSide", chain.Id);
            Assert.AreEqual(1, chain.Transformations.Count);
        }

        /// <summary>
        /// Tests <c>vdv301.xml</c> version 2.4.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\VDV301\vdv301_v2.4.xml")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method.")]
        public void TestV2_4()
        {
            var configManager = new ConfigManager<Vdv301ProtocolConfig>();
            configManager.FileName = "vdv301_v2.4.xml";
            configManager.XmlSchema = Vdv301ProtocolConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.IsNotNull(config.Services);
            Assert.IsNotNull(config.Services.CustomerInformationService);
            Assert.IsNull(config.Services.CustomerInformationService.Host);
            Assert.IsNull(config.Services.CustomerInformationService.Path);
            Assert.AreEqual(0, config.Services.CustomerInformationService.Port);

            Assert.IsNotNull(config.Services.CustomerInformationService.GetAllData);
            Assert.IsTrue(config.Services.CustomerInformationService.GetAllData.Subscribe);
            Assert.AreEqual(TimeSpan.Zero, config.Services.CustomerInformationService.GetAllData.SubscriptionTimeout);
            Assert.IsNotNull(config.Services.CustomerInformationService.GetAllData.TripInformation);
            Assert.IsNotNull(config.Services.CustomerInformationService.GetAllData.TripInformation.StopSequence);

            var stopPoint =
                config.Services.CustomerInformationService.GetAllData.TripInformation.StopSequence.StopPoint;
            Assert.IsNotNull(stopPoint);
            Assert.AreEqual(1, stopPoint.StopName.Count);

            var stopName = stopPoint.StopName[0];
            Assert.IsTrue(stopName.Enabled);
            Assert.IsNull(stopName.TransfRef);
            AssertAreEqual("0", "Stops", "StopName", "{0}", stopName);

            Assert.IsNotNull(stopPoint.DisplayContent);
            Assert.IsNotNull(stopPoint.DisplayContent.LineInformation);
            Assert.AreEqual(1, stopPoint.DisplayContent.LineInformation.LineNumber.Count);

            var lineNumber = stopPoint.DisplayContent.LineInformation.LineNumber[0];
            Assert.IsTrue(lineNumber.Enabled);
            Assert.IsNull(lineNumber.TransfRef);
            AssertAreEqual("0", "Route", "Line", "0", lineNumber);

            Assert.IsNotNull(stopPoint.DisplayContent.Destination);
            Assert.AreEqual(1, stopPoint.DisplayContent.Destination.DestinationName.Count);
            var destinationName = stopPoint.DisplayContent.Destination.DestinationName[0];
            Assert.IsTrue(destinationName.Enabled);
            Assert.IsNull(destinationName.TransfRef);
            AssertAreEqual("0", "Destination", "DestinationName", "0", destinationName);

            Assert.IsNotNull(stopPoint.Connection);
            Assert.IsNotNull(stopPoint.Connection.DisplayContent);
            Assert.IsNotNull(stopPoint.Connection.DisplayContent.Destination);
            Assert.AreEqual(1, stopPoint.Connection.DisplayContent.Destination.DestinationName.Count);

            destinationName = stopPoint.Connection.DisplayContent.Destination.DestinationName[0];
            Assert.IsTrue(destinationName.Enabled);
            Assert.IsNull(destinationName.TransfRef);
            AssertAreEqual("0", "Connections", "ConnectionDestinationName", "{0}", destinationName);

            Assert.AreEqual(1, stopPoint.Connection.Platform.Count);

            var platform = stopPoint.Connection.Platform[0];
            Assert.IsTrue(platform.Enabled);
            Assert.IsNull(platform.TransfRef);
            AssertAreEqual("0", "Connections", "ConnectionPlatform", "{0}", platform);

            Assert.IsNotNull(stopPoint.Connection.TransportMode);
            Assert.AreEqual(1, stopPoint.Connection.TransportMode.Name.Count);

            var transportMode = stopPoint.Connection.TransportMode.Name[0];
            Assert.IsTrue(transportMode.Enabled);
            Assert.IsNull(transportMode.TransfRef);
            AssertAreEqual("0", "Connections", "ConnectionTransportType", "{0}", transportMode);

            Assert.AreEqual(1, config.Services.CustomerInformationService.GetAllData.DoorState.Count);
            var doorState = config.Services.CustomerInformationService.GetAllData.DoorState[0];
            Assert.IsTrue(doorState.Enabled);
            Assert.AreEqual("DoorStatus", doorState.TransfRef);
            AssertAreEqual("0", "SystemStatus", "DoorStatus", "0", doorState);

            Assert.AreEqual(1, config.Services.CustomerInformationService.GetAllData.VehicleStopRequested.Count);
            var stopRequested = config.Services.CustomerInformationService.GetAllData.VehicleStopRequested[0];
            Assert.IsTrue(stopRequested.Enabled);
            Assert.IsNull(stopRequested.TransfRef);
            AssertAreEqual("0", "SystemStatus", "StopRequestedState", "0", stopRequested);

            Assert.AreEqual(1, config.Services.CustomerInformationService.GetAllData.ExitSide.Count);
            var exitSide = config.Services.CustomerInformationService.GetAllData.ExitSide[0];
            Assert.IsFalse(exitSide.Enabled);
            Assert.AreEqual("ExitSide", exitSide.TransfRef);
            AssertAreEqual("0", "SystemStatus", "ExitSide", "0", exitSide);

            Assert.IsNull(config.Services.CustomerInformationService.GetCurrentAnnouncement);
            Assert.IsNull(config.Services.CustomerInformationService.GetCurrentConnectionInformation);
            Assert.IsNull(config.Services.CustomerInformationService.GetCurrentDisplayContent);
            Assert.IsNull(config.Services.CustomerInformationService.GetCurrentStopIndex);
            Assert.IsNull(config.Services.CustomerInformationService.GetCurrentStopPoint);
            Assert.IsNull(config.Services.CustomerInformationService.GetTripData);
            Assert.IsNull(config.Services.CustomerInformationService.GetVehicleData);

            Assert.AreEqual(1, config.Languages.Count);

            var language = config.Languages[0];
            Assert.AreEqual("de", language.Vdv301Input);
            Assert.AreEqual("default", language.XimpleOutput);

            Assert.AreEqual(2, config.Transformations.Count);

            var chain = config.Transformations[0];
            Assert.AreEqual("DoorStatus", chain.Id);
            Assert.AreEqual(1, chain.Transformations.Count);

            chain = config.Transformations[1];
            Assert.AreEqual("ExitSide", chain.Id);
            Assert.AreEqual(1, chain.Transformations.Count);
        }

        /// <summary>
        /// Tests that <c>AhdlcRenderer.xml</c> can be saved with binary serialization (used for .cache file).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\VDV301\vdv301_v2.4.xml")]
        public void TestBinarySerialization()
        {
            var configManager = new ConfigManager<Vdv301ProtocolConfig>();
            configManager.FileName = "vdv301_v2.4.xml";
            configManager.XmlSchema = Vdv301ProtocolConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            var memory = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(memory, config);
            Assert.IsTrue(memory.Position > 0);
        }

        private static void AssertAreEqual(string lang, string table, string column, string row, GenericUsage actual)
        {
            Assert.AreEqual(lang, actual.Language);
            Assert.AreEqual(table, actual.Table);
            Assert.AreEqual(column, actual.Column);
            Assert.AreEqual(row, actual.Row);
            Assert.AreEqual(0, actual.RowOffset);
        }

        // ReSharper restore InconsistentNaming
    }
}
