// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisConfigTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for IbisConfig class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Config
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO.Ports;
    using System.Text.RegularExpressions;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Configuration.Protran.Transformations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for <see cref="IbisConfig"/> class.
    /// </summary>
    [TestClass]
    public class IbisConfigTest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Tests a valid IBIS config for Protran version 1.2.1220.1492.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Config\IbisConfigV2_0_1326.xml")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit Test method.")]
        public void TestIbisConfigV2_0_1326()
        {
            var schema = IbisConfig.Schema;
            var configurator = new Configurator("IbisConfigV2_0_1326.xml", schema);
            var config = configurator.Deserialize<IbisConfig>();

            Assert.IsNotNull(config.Behaviour);
            Assert.AreEqual(2, config.Behaviour.IbisAddresses.Count);
            Assert.AreEqual(8, config.Behaviour.IbisAddresses[0]);
            Assert.AreEqual(10, config.Behaviour.IbisAddresses[1]);
            Assert.AreEqual(TimeSpan.FromSeconds(60), config.Behaviour.ConnectionTimeOut);
            Assert.IsTrue(config.Behaviour.CheckCrc);
            Assert.AreEqual(ByteType.Ascii7, config.Behaviour.ByteType);
            Assert.AreEqual(ProcessPriorityClass.AboveNormal, config.Behaviour.ProcessPriority);
            CheckUsedFor(config.Behaviour.ConnectionStatusUsedFor, "SystemStatus", "RemotePC", "0");

            Assert.IsNotNull(config.Sources);
            Assert.AreEqual(IbisSourceType.SerialPort, config.Sources.Active);
            Assert.IsNotNull(config.Sources.Simulation);

            Assert.IsNotNull(config.Sources.Simulation);
            Assert.AreEqual("./ibisSimulation2.log", config.Sources.Simulation.SimulationFile);
            Assert.AreEqual(TimeSpan.FromSeconds(5), config.Sources.Simulation.InitialDelay);
            Assert.AreEqual(TimeSpan.FromSeconds(1), config.Sources.Simulation.IntervalBetweenTelegrams);
            Assert.AreEqual(2, config.Sources.Simulation.TimesToRepeat);

            Assert.IsNotNull(config.Sources.SerialPort);
            Assert.AreEqual("COM1", config.Sources.SerialPort.ComPort);
            Assert.AreEqual(1200, config.Sources.SerialPort.BaudRate);
            Assert.AreEqual(7, config.Sources.SerialPort.DataBits);
            Assert.AreEqual(StopBits.Two, config.Sources.SerialPort.StopBits);
            Assert.AreEqual(Parity.Even, config.Sources.SerialPort.Parity);
            Assert.AreEqual(10, config.Sources.SerialPort.RetryCount);

            Assert.IsNotNull(config.Sources.UdpServer);
            Assert.AreEqual(47555, config.Sources.UdpServer.LocalPort);
            Assert.AreEqual(TelegramFormat.NoChecksum, config.Sources.UdpServer.ReceiveFormat);
            Assert.AreEqual(TelegramFormat.NoFooter, config.Sources.UdpServer.SendFormat);

            Assert.IsNotNull(config.Recording);
            Assert.IsTrue(config.Recording.Active);
            Assert.AreEqual(RecordingFormat.Protran, config.Recording.Format);
            Assert.AreEqual("./ibis.log", config.Recording.FileAbsPath);

            Assert.IsNotNull(config.TimeSync);
            Assert.IsTrue(config.TimeSync.Enabled);
            Assert.AreEqual(TimeSpan.FromSeconds(10), config.TimeSync.InitialDelay);
            Assert.AreEqual(3, config.TimeSync.WaitTelegrams);
            Assert.AreEqual(TimeSpan.Zero, config.TimeSync.Tolerance);

            Assert.IsNotNull(config.Telegrams);
            Assert.AreEqual(14, config.Telegrams.Count);

            Assert.IsInstanceOfType(config.Telegrams[0], typeof(TelegramConfig));
            {
                var telegram = config.Telegrams[0];
                Assert.AreEqual("DS001", telegram.Name);
                Assert.IsTrue(telegram.Enabled);
                Assert.AreEqual("Line", telegram.TransfRef);
                Assert.IsNotNull(telegram.Answer);
                Assert.IsNull(telegram.Answer.Telegram);
                CheckUsedFor(telegram.UsedFor, "Route", "Line", "0");
            }

            Assert.IsInstanceOfType(config.Telegrams[1], typeof(TelegramConfig));
            {
                var telegram = config.Telegrams[1];
                Assert.AreEqual("DS001a", telegram.Name);
                Assert.IsTrue(telegram.Enabled);
                Assert.AreEqual("Default", telegram.TransfRef);
                Assert.IsNotNull(telegram.Answer);
                Assert.IsNull(telegram.Answer.Telegram);
                CheckUsedFor(telegram.UsedFor, "Route", "SpecialLine", "0");
            }

            Assert.IsInstanceOfType(config.Telegrams[2], typeof(TelegramConfig));
            {
                var telegram = config.Telegrams[2];
                Assert.AreEqual("DS003a", telegram.Name);
                Assert.IsTrue(telegram.Enabled);
                Assert.AreEqual("Destination", telegram.TransfRef);
                Assert.IsNotNull(telegram.Answer);
                Assert.IsNull(telegram.Answer.Telegram);
                CheckUsedFor(telegram.UsedFor, "Destination", "DestinationName", "0");
            }

            Assert.IsInstanceOfType(config.Telegrams[3], typeof(TelegramConfig));
            {
                var telegram = config.Telegrams[3];
                Assert.AreEqual("DS005", telegram.Name);
                Assert.IsTrue(telegram.Enabled);
                Assert.AreEqual("Time", telegram.TransfRef);
                Assert.IsNotNull(telegram.Answer);
                Assert.IsNull(telegram.Answer.Telegram);
                Assert.IsNull(telegram.UsedFor);
            }

            Assert.IsInstanceOfType(config.Telegrams[4], typeof(TelegramConfig));
            {
                var telegram = config.Telegrams[4];
                Assert.AreEqual("DS006", telegram.Name);
                Assert.IsTrue(telegram.Enabled);
                Assert.AreEqual("Date", telegram.TransfRef);
                Assert.IsNotNull(telegram.Answer);
                Assert.IsNull(telegram.Answer.Telegram);
                CheckUsedFor(telegram.UsedFor, "SystemStatus", "Date", "0");
            }

            Assert.IsInstanceOfType(config.Telegrams[5], typeof(TelegramConfig));
            {
                var telegram = config.Telegrams[5];
                Assert.AreEqual("DS010b", telegram.Name);
                Assert.IsTrue(telegram.Enabled);
                Assert.AreEqual("Number", telegram.TransfRef);
                Assert.IsNotNull(telegram.Answer);
                Assert.IsNull(telegram.Answer.Telegram);
                Assert.IsNull(telegram.UsedFor);
            }

            Assert.IsInstanceOfType(config.Telegrams[6], typeof(TelegramConfig));
            {
                var telegram = config.Telegrams[6];
                Assert.AreEqual("DS020", telegram.Name);
                Assert.IsTrue(telegram.Enabled);
                Assert.AreEqual(string.Empty, telegram.TransfRef);
                Assert.IsNotNull(telegram.Answer);
                Assert.IsNotNull(telegram.Answer.Telegram);
                Assert.IsNull(telegram.UsedFor);

                Assert.IsInstanceOfType(telegram.Answer.Telegram, typeof(DS120Config));
                var answer = (DS120Config)telegram.Answer.Telegram;
                Assert.AreEqual("DS120", answer.Name);
                Assert.IsTrue(answer.Enabled);
                Assert.AreEqual(string.Empty, answer.TransfRef);
                Assert.IsNotNull(answer.Answer);
                Assert.IsNull(answer.Answer.Telegram);
                Assert.IsNull(answer.UsedFor);
                Assert.AreEqual(2, answer.Responses.Count);
                Assert.AreEqual(Status.NoData, answer.Responses[0].Status);
                Assert.AreEqual(3, answer.Responses[0].Value);
                Assert.AreEqual(Status.MissingData, answer.Responses[1].Status);
                Assert.AreEqual(4, answer.Responses[1].Value);
                Assert.AreEqual(0, answer.DefaultResponse);
            }

            Assert.IsInstanceOfType(config.Telegrams[7], typeof(DS021AConfig));
            {
                var telegram = (DS021AConfig)config.Telegrams[7];
                Assert.AreEqual("DS021a", telegram.Name);
                Assert.IsTrue(telegram.Enabled);
                Assert.AreEqual("Stops021a", telegram.TransfRef);
                Assert.IsNotNull(telegram.Answer);
                Assert.IsNotNull(telegram.Answer.Telegram);
                CheckUsedFor(telegram.UsedFor, "Stops", "StopName", "{0}");
                CheckUsedFor(telegram.UsedForTransfers, -1, "Stops", "StopInfo", "{0}");
                CheckUsedFor(telegram.UsedForTransferSymbols, -1, "Stops", "StopTransferSymbols", "{0}");
                CheckUsedFor(telegram.UsedForDestination, "Destination", "DestinationName", "0");
                CheckUsedFor(telegram.UsedForDestinationTransfers, "Destination", "DestinationInfo", "0");
                CheckUsedFor(
                    telegram.UsedForDestinationTransferSymbols, "Destination", "DestinationTransferSymbols", "0");
                Assert.AreEqual(6, telegram.FlushNumberOfStations);
                Assert.AreEqual(TimeSpan.FromSeconds(10), telegram.FlushTimeout);
                Assert.AreEqual(1, telegram.FirstStopIndexValue);
                Assert.AreEqual(999, telegram.EndingStopValue);

                Assert.IsInstanceOfType(telegram.Answer.Telegram, typeof(DS120Config));
                var answer = (DS120Config)telegram.Answer.Telegram;
                Assert.AreEqual("DS120", answer.Name);
                Assert.IsTrue(answer.Enabled);
                Assert.AreEqual(string.Empty, answer.TransfRef);
                Assert.IsNotNull(answer.Answer);
                Assert.IsNull(answer.Answer.Telegram);
                Assert.IsNull(answer.UsedFor);
                Assert.AreEqual(1, answer.Responses.Count);
                Assert.AreEqual(Status.IncorrectRecord, answer.Responses[0].Status);
                Assert.AreEqual(6, answer.Responses[0].Value);
                Assert.AreEqual(0, answer.DefaultResponse);
            }

            Assert.IsInstanceOfType(config.Telegrams[8], typeof(DS080Config));
            {
                var telegram = (DS080Config)config.Telegrams[8];
                Assert.AreEqual("DS080", telegram.Name);
                Assert.IsTrue(telegram.Enabled);
                Assert.AreEqual(string.Empty, telegram.TransfRef);
                Assert.IsNotNull(telegram.Answer);
                Assert.IsNull(telegram.Answer.Telegram);
                Assert.AreEqual("1", telegram.OpenValue);
                Assert.AreEqual("0", telegram.CloseValue);
                Assert.AreEqual(true, telegram.ResetWithDS010B);
                CheckUsedFor(telegram.UsedFor, "SystemStatus", "DoorStatus", "0");
            }

            Assert.IsInstanceOfType(config.Telegrams[9], typeof(DS081Config));
            {
                var telegram = (DS081Config)config.Telegrams[9];
                Assert.AreEqual("DS081", telegram.Name);
                Assert.IsTrue(telegram.Enabled);
                Assert.AreEqual(string.Empty, telegram.TransfRef);
                Assert.IsNotNull(telegram.Answer);
                Assert.IsNull(telegram.Answer.Telegram);
                Assert.AreEqual("0", telegram.Value);
                CheckUsedFor(telegram.UsedFor, "SystemStatus", "DoorStatus", "0");
            }

            Assert.IsInstanceOfType(config.Telegrams[10], typeof(TelegramConfig));
            {
                var telegram = config.Telegrams[10];
                Assert.AreEqual("GO001", telegram.Name);
                Assert.IsTrue(telegram.Enabled);
                Assert.AreEqual("Number", telegram.TransfRef);
                Assert.IsNotNull(telegram.Answer);
                Assert.IsNull(telegram.Answer.Telegram);
                CheckUsedFor(telegram.UsedFor, "Route", "ApproachingStop", "0");
            }

            Assert.IsInstanceOfType(config.Telegrams[11], typeof(GO002Config));
            {
                var telegram = (GO002Config)config.Telegrams[11];
                Assert.AreEqual("GO002", telegram.Name);
                Assert.IsTrue(telegram.Enabled);
                Assert.AreEqual("ConnectionDest", telegram.TransfRef);
                Assert.IsNotNull(telegram.Answer);
                Assert.IsNotNull(telegram.Answer.Telegram);

                Assert.IsTrue(telegram.CheckLength);
                Assert.AreEqual("ConnectionDest", telegram.TransfRef);
                Assert.AreEqual(2, telegram.StopIndexSize);
                Assert.AreEqual(1, telegram.RowNumberSize);
                Assert.AreEqual(1, telegram.PictogramSize);
                Assert.AreEqual(5, telegram.LineNumberSize);
                Assert.AreEqual(2, telegram.TrackNumberSize);
                Assert.AreEqual(4, telegram.ScheduleDeviationSize);
                Assert.AreEqual(1, telegram.FirstStopIndex);
                Assert.AreEqual(1, telegram.FirstRowIndex);
                Assert.AreEqual(9, telegram.LastRowIndex);
                Assert.AreEqual(@"D:\Infomedia\Symbols\Conn_{0}.png", telegram.PictogramFormat);
                Assert.AreEqual(@"D:\Infomedia\Symbols\L{0}.png", telegram.LineNumberFormat);
                Assert.IsNotNull(telegram.ScheduleDeviation);
                Assert.AreEqual("ok", telegram.ScheduleDeviation.OnTime);
                Assert.AreEqual("{0}' früher", telegram.ScheduleDeviation.Ahead);
                Assert.AreEqual("{0}' später", telegram.ScheduleDeviation.Delayed);
                Assert.IsFalse(telegram.ShowForNextStopOnly);

                CheckUsedFor(telegram.UsedFor, "Connections", "ConnectionDestinationName", "{0}");
                CheckUsedFor(telegram.UsedForPictogram, "Connections", "ConnectionTransportType", "{0}");
                CheckUsedFor(telegram.UsedForLineNumber, "Connections", "ConnectionLineSymbol", "{0}");
                CheckUsedFor(telegram.UsedForDepartureTime, "Connections", "ConnectionTime", "{0}");
                CheckUsedFor(telegram.UsedForTrackNumber, "Connections", "ConnectionPlatform", "{0}");
                CheckUsedFor(telegram.UsedForScheduleDeviation, "Connections", "ConnectionDelay", "{0}");

                Assert.IsInstanceOfType(telegram.Answer.Telegram, typeof(DS120Config));
                var answer = (DS120Config)telegram.Answer.Telegram;
                Assert.AreEqual("DS120", answer.Name);
                Assert.IsTrue(answer.Enabled);
                Assert.AreEqual(string.Empty, answer.TransfRef);
                Assert.IsNotNull(answer.Answer);
                Assert.IsNull(answer.Answer.Telegram);
                Assert.IsNull(answer.UsedFor);
                Assert.AreEqual(0, answer.Responses.Count);
                Assert.AreEqual(0, answer.DefaultResponse);
            }

            Assert.IsInstanceOfType(config.Telegrams[12], typeof(TelegramConfig));
            {
                var telegram = config.Telegrams[12];
                Assert.AreEqual("GO003", telegram.Name);
                Assert.IsTrue(telegram.Enabled);
                Assert.AreEqual("Stops", telegram.TransfRef);
                Assert.IsNotNull(telegram.Answer);
                Assert.IsNotNull(telegram.Answer.Telegram);
                CheckUsedFor(telegram.UsedFor, "Stops", "StopName", "{0}");

                Assert.IsInstanceOfType(telegram.Answer.Telegram, typeof(DS120Config));
                var answer = (DS120Config)telegram.Answer.Telegram;
                Assert.AreEqual("DS120", answer.Name);
                Assert.IsTrue(answer.Enabled);
                Assert.AreEqual(string.Empty, answer.TransfRef);
                Assert.IsNotNull(answer.Answer);
                Assert.IsNull(answer.Answer.Telegram);
                Assert.IsNull(answer.UsedFor);
                Assert.AreEqual(1, answer.Responses.Count);
                Assert.AreEqual(Status.IncorrectRecord, answer.Responses[0].Status);
                Assert.AreEqual(5, answer.Responses[0].Value);
                Assert.AreEqual(0, answer.DefaultResponse);
            }

            Assert.IsInstanceOfType(config.Telegrams[13], typeof(HPW074Config));
            {
                var telegram = (HPW074Config)config.Telegrams[13];
                Assert.AreEqual("HPW074", telegram.Name);
                Assert.IsTrue(telegram.Enabled);
                Assert.AreEqual("Number", telegram.TransfRef);
                Assert.IsNotNull(telegram.Answer);
                Assert.IsNull(telegram.Answer.Telegram);
                CheckUsedFor(telegram.UsedFor, "PassengerMessages", "MessageText", "0");
                Assert.AreEqual(@"D:\infomedia\layout\specialtext.csv", telegram.SpecialTextFile);
                Assert.AreEqual("UTF-8", telegram.Encoding);
            }

            Assert.IsNotNull(config.Transformations);
            Assert.AreEqual(9, config.Transformations.Count);

            var chain = config.Transformations[0];
            Assert.AreEqual("Default", chain.Id);
            Assert.IsNotNull(chain.Transformations);
            Assert.AreEqual(0, chain.Transformations.Count);

            chain = config.Transformations[1];
            Assert.AreEqual("ConnectionDest", chain.Id);
            Assert.IsNotNull(chain.Transformations);
            Assert.AreEqual(1, chain.Transformations.Count);
            Assert.IsInstanceOfType(chain.Transformations[0], typeof(RegexMapping));
            {
                var transformation = (RegexMapping)chain.Transformations[0];
                Assert.AreEqual(1, transformation.Mappings.Count);
                Assert.AreEqual(@"[ \t]+$", transformation.Mappings[0].From);
                Assert.AreEqual(string.Empty, transformation.Mappings[0].To);
            }

            chain = config.Transformations[2];
            Assert.AreEqual("Time", chain.Id);
            Assert.IsNotNull(chain.Transformations);
            Assert.AreEqual(1, chain.Transformations.Count);
            Assert.IsInstanceOfType(chain.Transformations[0], typeof(RegexMapping));
            {
                var transformation = (RegexMapping)chain.Transformations[0];
                Assert.AreEqual(1, transformation.Mappings.Count);
                Assert.AreEqual(@"^(\d\d)(\d\d)$", transformation.Mappings[0].From);
                Assert.AreEqual("$1:$2", transformation.Mappings[0].To);
            }

            chain = config.Transformations[3];
            Assert.AreEqual("Date", chain.Id);
            Assert.IsNotNull(chain.Transformations);
            Assert.AreEqual(1, chain.Transformations.Count);
            Assert.IsInstanceOfType(chain.Transformations[0], typeof(RegexMapping));
            {
                var transformation = (RegexMapping)chain.Transformations[0];
                Assert.AreEqual(1, transformation.Mappings.Count);
                Assert.AreEqual(@"^(\d\d)(\d\d)(\d\d)$", transformation.Mappings[0].From);
                Assert.AreEqual("$1.$2.20$3", transformation.Mappings[0].To);
            }

            chain = config.Transformations[4];
            Assert.AreEqual("Stops", chain.Id);
            Assert.IsNotNull(chain.Transformations);
            Assert.AreEqual(1, chain.Transformations.Count);
            Assert.IsInstanceOfType(chain.Transformations[0], typeof(RegexDivider));
            {
                var transformation = (RegexDivider)chain.Transformations[0];
                Assert.AreEqual(@"\u000A", transformation.Regex);
                Assert.AreEqual(RegexOptions.None, transformation.Options);
            }

            chain = config.Transformations[5];
            Assert.AreEqual("Destination", chain.Id);
            Assert.IsNotNull(chain.Transformations);
            Assert.AreEqual(10, chain.Transformations.Count);
            Assert.IsInstanceOfType(chain.Transformations[0], typeof(RegexMapping));
            {
                var transformation = (RegexMapping)chain.Transformations[0];
                Assert.AreEqual(2, transformation.Mappings.Count);
                Assert.AreEqual(@"^[0-9]", transformation.Mappings[0].From);
                Assert.AreEqual(string.Empty, transformation.Mappings[0].To);
                Assert.AreEqual(@"\n", transformation.Mappings[1].From);
                Assert.AreEqual(" ", transformation.Mappings[1].To);
            }

            Assert.IsInstanceOfType(chain.Transformations[1], typeof(RegexDivider));
            {
                var transformation = (RegexDivider)chain.Transformations[1];
                Assert.AreEqual(@"(.{16})", transformation.Regex);
                Assert.AreEqual(RegexOptions.None, transformation.Options);
            }

            Assert.IsInstanceOfType(chain.Transformations[2], typeof(StringMapping));
            {
                var transformation = (StringMapping)chain.Transformations[2];
                Assert.IsNotNull(transformation.Mappings);
                Assert.AreEqual(7, transformation.Mappings.Count);
                Assert.AreEqual("{", transformation.Mappings[0].From);
                Assert.AreEqual("ä", transformation.Mappings[0].To);
                Assert.AreEqual("[", transformation.Mappings[1].From);
                Assert.AreEqual("Ä", transformation.Mappings[1].To);
                Assert.AreEqual("|", transformation.Mappings[2].From);
                Assert.AreEqual("ö", transformation.Mappings[2].To);
                Assert.AreEqual("\\", transformation.Mappings[3].From);
                Assert.AreEqual("Ö", transformation.Mappings[3].To);
                Assert.AreEqual("}", transformation.Mappings[4].From);
                Assert.AreEqual("ü", transformation.Mappings[4].To);
                Assert.AreEqual("]", transformation.Mappings[5].From);
                Assert.AreEqual("Ü", transformation.Mappings[5].To);
                Assert.AreEqual("~", transformation.Mappings[6].From);
                Assert.AreEqual("ß", transformation.Mappings[6].To);
            }

            Assert.IsInstanceOfType(chain.Transformations[3], typeof(RegexMapping));
            {
                var transformation = (RegexMapping)chain.Transformations[3];
                Assert.AreEqual(1, transformation.Mappings.Count);
                Assert.AreEqual(@" +$", transformation.Mappings[0].From);
                Assert.AreEqual(string.Empty, transformation.Mappings[0].To);
            }

            Assert.IsInstanceOfType(chain.Transformations[4], typeof(Join));
            {
                var transformation = (Join)chain.Transformations[4];
                Assert.AreEqual("#", transformation.Separator);
            }

            Assert.IsInstanceOfType(chain.Transformations[5], typeof(RegexMapping));
            {
                var transformation = (RegexMapping)chain.Transformations[5];
                Assert.AreEqual(1, transformation.Mappings.Count);
                Assert.AreEqual(@"^#([^#]*##[^#]*)##.*$", transformation.Mappings[0].From);
                Assert.AreEqual("$1", transformation.Mappings[0].To);
            }

            Assert.IsInstanceOfType(chain.Transformations[6], typeof(RegexDivider));
            {
                var transformation = (RegexDivider)chain.Transformations[6];
                Assert.AreEqual(@"(\W+)", transformation.Regex);
                Assert.AreEqual(RegexOptions.None, transformation.Options);
            }

            Assert.IsInstanceOfType(chain.Transformations[7], typeof(Capitalize));
            {
                var transformation = (Capitalize)chain.Transformations[7];
                Assert.IsNull(transformation.Exceptions);
                Assert.AreEqual(CapitalizeMode.UpperLower, transformation.Mode);
            }

            chain = config.Transformations[6];
            Assert.AreEqual("Stops021a", chain.Id);
            Assert.IsNotNull(chain.Transformations);
            Assert.AreEqual(2, chain.Transformations.Count);
            Assert.IsInstanceOfType(chain.Transformations[0], typeof(RegexDivider));
            {
                var transformation = (RegexDivider)chain.Transformations[0];
                Assert.AreEqual(@"[\u0003-\u0005#]", transformation.Regex);
                Assert.AreEqual(RegexOptions.None, transformation.Options);
            }

            Assert.IsInstanceOfType(chain.Transformations[1], typeof(StringMapping));
            {
                var transformation = (StringMapping)chain.Transformations[1];
                Assert.IsNotNull(transformation.Mappings);
                Assert.AreEqual(7, transformation.Mappings.Count);
                Assert.AreEqual("{", transformation.Mappings[0].From);
                Assert.AreEqual("ä", transformation.Mappings[0].To);
                Assert.AreEqual("[", transformation.Mappings[1].From);
                Assert.AreEqual("Ä", transformation.Mappings[1].To);
                Assert.AreEqual("|", transformation.Mappings[2].From);
                Assert.AreEqual("ö", transformation.Mappings[2].To);
                Assert.AreEqual("\\", transformation.Mappings[3].From);
                Assert.AreEqual("Ö", transformation.Mappings[3].To);
                Assert.AreEqual("}", transformation.Mappings[4].From);
                Assert.AreEqual("ü", transformation.Mappings[4].To);
                Assert.AreEqual("]", transformation.Mappings[5].From);
                Assert.AreEqual("Ü", transformation.Mappings[5].To);
                Assert.AreEqual("~", transformation.Mappings[6].From);
                Assert.AreEqual("ß", transformation.Mappings[6].To);
            }

            chain = config.Transformations[7];
            Assert.AreEqual("Line", chain.Id);
            Assert.IsNotNull(chain.Transformations);
            Assert.AreEqual(1, chain.Transformations.Count);
            Assert.IsInstanceOfType(chain.Transformations[0], typeof(RegexMapping));
            {
                var transformation = (RegexMapping)chain.Transformations[0];
                Assert.AreEqual(1, transformation.Mappings.Count);
                Assert.AreEqual("^0+", transformation.Mappings[0].From);
                Assert.AreEqual(string.Empty, transformation.Mappings[0].To);
            }

            chain = config.Transformations[8];
            Assert.AreEqual("Number", chain.Id);
            Assert.IsNotNull(chain.Transformations);
            Assert.AreEqual(1, chain.Transformations.Count);
            Assert.IsInstanceOfType(chain.Transformations[0], typeof(Integer));
            {
                var transformation = (Integer)chain.Transformations[0];
                Assert.AreEqual(NumberStyles.Integer, transformation.NumberStyle);
            }
        }

        /// <summary>
        /// Tests a valid DS030 IBIS config for Protran version 2.4.0.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Config\IbisConfigV2_5_0.xml")]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit Test method.")]
        public void TestIbisConfigV2_5_0()
        {
            var schema = IbisConfig.Schema;
            var configurator = new Configurator("IbisConfigV2_5_0.xml", schema);
            var config = configurator.Deserialize<IbisConfig>();

            Assert.IsInstanceOfType(config.Telegrams[18], typeof(TelegramConfig));
            {
                var telegram = config.Telegrams[18];
                Assert.AreEqual("DS030", telegram.Name);
                Assert.IsTrue(telegram.Enabled);
                Assert.AreEqual(string.Empty, telegram.TransfRef);
                Assert.IsNotNull(telegram.Answer);
                Assert.IsNotNull(telegram.Answer.Telegram);
                Assert.IsNull(telegram.UsedFor);

                Assert.IsInstanceOfType(telegram.Answer.Telegram, typeof(DS130Config));
                var answer = (DS130Config)telegram.Answer.Telegram;
                Assert.AreEqual("DS130", answer.Name);
                Assert.IsTrue(answer.Enabled);
                Assert.AreEqual(string.Empty, answer.TransfRef);
                Assert.IsNotNull(answer.Answer);
                Assert.IsNull(answer.Answer.Telegram);
                Assert.IsNull(answer.UsedFor);
                Assert.AreEqual(2, answer.Responses.Count);
                Assert.AreEqual(Status.NoData, answer.Responses[0].Status);
                Assert.AreEqual(3, answer.Responses[0].Value);
                Assert.AreEqual(Status.MissingData, answer.Responses[1].Status);
                Assert.AreEqual(4, answer.Responses[1].Value);
                Assert.AreEqual(0, answer.DefaultResponse);
            }
        }

        private static void CheckUsedFor(
            GenericUsageDS021Base usage,
            int blockIndex,
            string table,
            string column,
            string row,
            int rowOffset = 0)
        {
            CheckUsedFor(usage, blockIndex, "0", table, column, row, rowOffset);
        }

        private static void CheckUsedFor(
            GenericUsageDS021Base usage,
            int blockIndex,
            string language,
            string table,
            string column,
            string row,
            int rowOffset = 0)
        {
            CheckUsedFor(usage, language, table, column, row, rowOffset);
            Assert.AreEqual(blockIndex, usage.FromBlock);
        }

        private static void CheckUsedFor(
            GenericUsage usage, string table, string column, string row, int rowOffset = 0)
        {
            CheckUsedFor(usage, "0", table, column, row, rowOffset);
        }

        private static void CheckUsedFor(
            GenericUsage usage, string language, string table, string column, string row, int rowOffset = 0)
        {
            Assert.IsNotNull(usage);
            Assert.AreEqual(language, usage.Language);
            Assert.AreEqual(table, usage.Table);
            Assert.AreEqual(column, usage.Column);
            Assert.AreEqual(row, usage.Row);
            Assert.AreEqual(rowOffset, usage.RowOffset);
        }
        // ReSharper restore InconsistentNaming
    }
}
