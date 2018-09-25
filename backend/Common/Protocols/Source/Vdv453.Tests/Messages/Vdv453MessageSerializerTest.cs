// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv453MessageSerializerTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv453MessageSerializerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Tests.Messages
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    using Gorba.Common.Protocols.Vdv453.Messages;
    using Gorba.Common.Utility.Core;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Test for VDV453 message serialization and deserialization
    /// </summary>
    [TestClass]
    public class Vdv453MessageSerializerTest
    {
        private const string VdvXmlDeclaration = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>";

        private static readonly DateTime UtcNow = new DateTime(2013, 02, 26, 13, 53, 52, DateTimeKind.Utc);

        /// <summary>
        /// Initializes this test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            var timeProviderMock = new Mock<TimeProvider>();
            timeProviderMock.SetupGet(provider => provider.UtcNow).Returns(UtcNow);
            TimeProvider.Current = timeProviderMock.Object;
        }

        /// <summary>
        /// Cleanups this test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            TimeProvider.ResetToDefault();
        }

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Test serialization of DataReadyAnswer
        /// </summary>
        [TestMethod]
        public void Serialize_DataReadyAnswer()
        {
            var dataReadyAnswer = new DataReadyAnswer();
            const string Msg =
                "<DatenBereitAntwort><Bestaetigung Zst=\"2013-09-22T17:55:44\" Ergebnis=\"notok\" Fehlernummer=\"401\" /></DatenBereitAntwort>";
            dataReadyAnswer.Acknowledge.TimeStamp = new DateTime(2013, 09, 22, 17, 55, 44, 324);
            dataReadyAnswer.Acknowledge.ErrorNumber = 401;
            dataReadyAnswer.Acknowledge.Result = Result.notok;
            TestSerialize(
                dataReadyAnswer,
                Vdv453Constants.DataReadyAnswer,
                Msg,
                result =>
                    {
                        Assert.AreEqual(new DateTime(2013, 09, 22, 17, 55, 44), result.Acknowledge.TimeStamp);
                        Assert.AreEqual(401, result.Acknowledge.ErrorNumber);
                        Assert.AreEqual(Result.notok, dataReadyAnswer.Acknowledge.Result);
                    });
        }

        /// <summary>
        /// Test serialization of DataReadyAnswer using xml namespace.
        /// </summary>
        [TestMethod]
        public void Serialize_DataReadyAnswer_WithXmlNamespace()
        {
            var dataReadyAnswer = new DataReadyAnswer();

            dataReadyAnswer.Acknowledge.TimeStamp = new DateTime(2013, 09, 22, 17, 55, 44, 324);
            dataReadyAnswer.Acknowledge.ErrorNumber = 401;
            dataReadyAnswer.Acknowledge.Result = Result.notok;

            const string Msg =
                "<vdv453:DatenBereitAntwort xmlns:vdv453=\"vdv453ger\"><Bestaetigung Zst=\"2013-09-22T17:55:44\" Ergebnis=\"notok\" Fehlernummer=\"401\" /></vdv453:DatenBereitAntwort>";

            TestSerialize(
                dataReadyAnswer,
                Vdv453Constants.DataReadyAnswer,
                Msg,
                result =>
                    {
                        Assert.AreEqual(new DateTime(2013, 09, 22, 17, 55, 44), result.Acknowledge.TimeStamp);
                        Assert.AreEqual(401, result.Acknowledge.ErrorNumber);
                        Assert.AreEqual(Result.notok, dataReadyAnswer.Acknowledge.Result);
                    },
                    "vdv453ger");
        }

        /// <summary>
        /// Test serialization of DataSupplyRequest
        /// </summary>
        [TestMethod]
        public void Serialize_DataSupplyRequest()
        {
            var dataSupplyRequest = new DataSupplyRequest();

            dataSupplyRequest.Sender = "iqube";
            dataSupplyRequest.AllData = true;
            dataSupplyRequest.TimeStamp = new DateTime(2013, 09, 22, 17, 55, 44, 324);

            const string Msg =
                "<DatenAbrufenAnfrage Sender=\"iqube\" Zst=\"2013-09-22T17:55:44\"><DatensatzAlle>true</DatensatzAlle></DatenAbrufenAnfrage>";

            TestSerialize(
                dataSupplyRequest,
                Vdv453Constants.DataSupplyRequest,
                Msg,
                result =>
                    {
                        Assert.AreEqual("iqube", result.Sender);
                        Assert.AreEqual(true, result.AllData);
                        Assert.AreEqual(new DateTime(2013, 09, 22, 17, 55, 44), result.TimeStamp);
                    });
        }

        /// <summary>
        /// Test serialization of DataSupplyRequest using xml namespace.
        /// </summary>
        [TestMethod]
        public void Serialize_DataSupplyRequest_WithXmlNamespace()
        {
            var dataSupplyRequest = new DataSupplyRequest();

            dataSupplyRequest.Sender = "iqube";
            dataSupplyRequest.AllData = true;
            dataSupplyRequest.TimeStamp = new DateTime(2013, 09, 22, 17, 55, 44, 324);

            const string Msg =
                "<vdv453:DatenAbrufenAnfrage Sender=\"iqube\" Zst=\"2013-09-22T17:55:44\" xmlns:vdv453=\"vdv453ger\"><DatensatzAlle>true</DatensatzAlle></vdv453:DatenAbrufenAnfrage>";

            TestSerialize(
                dataSupplyRequest,
                Vdv453Constants.DataSupplyRequest,
                Msg,
                result =>
                    {
                        Assert.AreEqual("iqube", result.Sender);
                        Assert.AreEqual(true, result.AllData);
                        Assert.AreEqual(new DateTime(2013, 09, 22, 17, 55, 44), result.TimeStamp);
                    },
                    "vdv453ger");
        }

        /// <summary>
        /// Test serialization of SubscriptionRequest with a DisRefSubscription
        /// </summary>
        [TestMethod]
        public void Serialize_SubscriptionRequest_DisRefSubscription()
        {
            var subscriptionRequest = new SubscriptionRequest();

            subscriptionRequest.Sender = "iqube";
            subscriptionRequest.TimeStamp = new DateTime(2013, 09, 22, 17, 55, 44, 324);

            var displayAreaRefSubscription = new DisRefSubscription
                {
                    SubscriptionId = 17,
                    ValidUntilTimeStamp = new DateTime(2013, 09, 22, 22, 0, 0, 0),
                    DisId = "AZBID",
                    LineId = null,
                    DirectionId = null,
                    EarliestDepartureTime = new DateTime(2013, 09, 22, 18, 0, 0, 0),
                    LatestDepartureTime = new DateTime(2013, 09, 22, 23, 0, 0, 0)
                };

            // add subscription item
            subscriptionRequest.Items.Add(displayAreaRefSubscription);

            const string Msg =
                "<AboAnfrage Sender=\"iqube\" Zst=\"2013-09-22T17:55:44\"><AboAZBRef AboID=\"17\" VerfallZst=\"2013-09-22T22:00:00\"><AZBID>AZBID</AZBID><FruehesteAbfahrtszeit>2013-09-22T18:00:00</FruehesteAbfahrtszeit><SpaetesteAbfahrtszeit>2013-09-22T23:00:00</SpaetesteAbfahrtszeit></AboAZBRef></AboAnfrage>";
            TestSerialize(
                subscriptionRequest,
                Vdv453Constants.SubscriptionRequest,
                Msg,
                result =>
                    {
                        Assert.AreEqual("iqube", result.Sender);
                        Assert.AreEqual(new DateTime(2013, 09, 22, 17, 55, 44), result.TimeStamp);
                        Assert.IsNotNull(result.Items);
                        Assert.AreEqual(1, result.Items.Count);
                        var item = result.Items[0];
                        Assert.IsInstanceOfType(item, typeof(DisRefSubscription));
                        displayAreaRefSubscription = (DisRefSubscription)item;
                        Assert.AreEqual(17u, displayAreaRefSubscription.SubscriptionId);
                        Assert.AreEqual(new DateTime(2013, 09, 22, 22, 0, 0), displayAreaRefSubscription.ValidUntilTimeStamp);
                        Assert.AreEqual("AZBID", displayAreaRefSubscription.DisId);
                        Assert.AreEqual(null, displayAreaRefSubscription.LineId);
                        Assert.AreEqual(null, displayAreaRefSubscription.DirectionId);
                        Assert.AreEqual(new DateTime(2013, 09, 22, 18, 0, 0), displayAreaRefSubscription.EarliestDepartureTime);
                        Assert.AreEqual(new DateTime(2013, 09, 22, 23, 0, 0), displayAreaRefSubscription.LatestDepartureTime);
                    });
        }

        /// <summary>
        /// Test serialization of SubscriptionRequest with a DisRefSubscription using xml namespace.
        /// </summary>
        [TestMethod]
        public void Serialize_SubscriptionRequest_DisRefSubscription_WithXmlNamespace()
        {
            var subscriptionRequest = new SubscriptionRequest();

            subscriptionRequest.Sender = "iqube";
            subscriptionRequest.TimeStamp = new DateTime(2013, 09, 22, 17, 55, 44, 324);

            var displayAreaRefSubscription = new DisRefSubscription
                {
                    SubscriptionId = 17,
                    ValidUntilTimeStamp = new DateTime(2013, 09, 22, 22, 0, 0, 0),
                    DisId = "AZBID",
                    LineId = null,
                    DirectionId = null,
                    EarliestDepartureTime = new DateTime(2013, 09, 22, 18, 0, 0, 0),
                    LatestDepartureTime = new DateTime(2013, 09, 22, 23, 0, 0, 0)
                };

            // add subscription item
            subscriptionRequest.Items.Add(displayAreaRefSubscription);

            const string Msg =
                "<vdv453:AboAnfrage Sender=\"iqube\" Zst=\"2013-09-22T17:55:44\" xmlns:vdv453=\"vdv453ger\"><AboAZBRef AboID=\"17\" VerfallZst=\"2013-09-22T22:00:00\"><AZBID>AZBID</AZBID><FruehesteAbfahrtszeit>2013-09-22T18:00:00</FruehesteAbfahrtszeit><SpaetesteAbfahrtszeit>2013-09-22T23:00:00</SpaetesteAbfahrtszeit></AboAZBRef></vdv453:AboAnfrage>";

            TestSerialize(
                subscriptionRequest,
                Vdv453Constants.SubscriptionRequest,
                Msg,                
                result =>
                    {
                        Assert.AreEqual("iqube", result.Sender);
                        Assert.AreEqual(new DateTime(2013, 09, 22, 17, 55, 44), result.TimeStamp);
                        Assert.IsNotNull(result.Items);
                        Assert.AreEqual(1, result.Items.Count);
                        var item = result.Items[0];
                        Assert.IsInstanceOfType(item, typeof(DisRefSubscription));
                        displayAreaRefSubscription = (DisRefSubscription)item;
                        Assert.AreEqual(17u, displayAreaRefSubscription.SubscriptionId);
                        Assert.AreEqual(new DateTime(2013, 09, 22, 22, 0, 0), displayAreaRefSubscription.ValidUntilTimeStamp);
                        Assert.AreEqual("AZBID", displayAreaRefSubscription.DisId);
                        Assert.AreEqual(null, displayAreaRefSubscription.LineId);
                        Assert.AreEqual(null, displayAreaRefSubscription.DirectionId);
                        Assert.AreEqual(new DateTime(2013, 09, 22, 18, 0, 0), displayAreaRefSubscription.EarliestDepartureTime);
                        Assert.AreEqual(new DateTime(2013, 09, 22, 23, 0, 0), displayAreaRefSubscription.LatestDepartureTime);
                    },
                    "vdv453ger");
        }

        /// <summary>
        /// Test serialization of SubscriptionRequest with a DisSubscription
        /// </summary>
        [TestMethod]
        public void Serialize_SubscriptionRequest_DisSubscription()
        {
            var subscriptionRequest = new SubscriptionRequest();

            subscriptionRequest.Sender = "iqube";
            subscriptionRequest.TimeStamp = new DateTime(2013, 09, 22, 17, 55, 44, 324);

            var dis = new DisSubscription
            {
                SubscriptionId = 17,
                Hysteresis = 15,
                MaxNumOfTrips = 10,
                MaxTextLength = 250,
                DisId = "AZBID",
                LineId = null,
                DirectionId = null,
                PreviewTime = 120,
                ValidUntilTimeStamp = new DateTime(2013, 09, 22, 22, 0, 0)
            };

            // add subscription item
            subscriptionRequest.Items.Add(dis);

            const string Msg =
                "<AboAnfrage Sender=\"iqube\" Zst=\"2013-09-22T17:55:44\"><AboAZB AboID=\"17\" VerfallZst=\"2013-09-22T22:00:00\"><AZBID>AZBID</AZBID><Vorschauzeit>120</Vorschauzeit><MaxAnzahlFahrten>10</MaxAnzahlFahrten><Hysterese>15</Hysterese><MaxTextLaenge>250</MaxTextLaenge></AboAZB></AboAnfrage>";

            TestSerialize(
                subscriptionRequest,
                Vdv453Constants.SubscriptionRequest,
                Msg,
                result =>
                {
                    Assert.AreEqual("iqube", result.Sender);
                    Assert.AreEqual(new DateTime(2013, 09, 22, 17, 55, 44), result.TimeStamp);
                    Assert.IsNotNull(result.Items);
                    Assert.AreEqual(1, result.Items.Count);
                    var item = result.Items[0];
                    Assert.IsInstanceOfType(item, typeof(DisSubscription));
                    dis = (DisSubscription)item;
                    Assert.AreEqual(17u, dis.SubscriptionId);
                    Assert.AreEqual(10u, dis.MaxNumOfTrips);
                    Assert.AreEqual(250u, dis.MaxTextLength);
                    Assert.AreEqual(120u, dis.PreviewTime);
                    Assert.AreEqual(new DateTime(2013, 09, 22, 22, 0, 0), dis.ValidUntilTimeStamp);
                    Assert.AreEqual("AZBID", dis.DisId);
                    Assert.AreEqual(null, dis.LineId);
                    Assert.AreEqual(null, dis.DirectionId);
                });
        }

        /// <summary>
        /// Test serialization of SubscriptionRequest with a DisSubscription using xml namespace.
        /// </summary>
        [TestMethod]
        public void Serialize_SubscriptionRequest_DisSubscription_WithXmlNamespace()
        {
            var subscriptionRequest = new SubscriptionRequest();

            subscriptionRequest.Sender = "iqube";
            subscriptionRequest.TimeStamp = new DateTime(2013, 09, 22, 17, 55, 44, 324);

            var dis = new DisSubscription
            {
                SubscriptionId = 17,
                Hysteresis = 15,
                MaxNumOfTrips = 10,
                MaxTextLength = 250,
                DisId = "AZBID",
                LineId = null,
                DirectionId = null,
                PreviewTime = 120,
                ValidUntilTimeStamp = new DateTime(2013, 09, 22, 22, 0, 0)
            };

            // add subscription item
            subscriptionRequest.Items.Add(dis);

            const string Msg =
                "<vdv453:AboAnfrage Sender=\"iqube\" Zst=\"2013-09-22T17:55:44\" xmlns:vdv453=\"vdv453ger\"><AboAZB AboID=\"17\" VerfallZst=\"2013-09-22T22:00:00\"><AZBID>AZBID</AZBID><Vorschauzeit>120</Vorschauzeit><MaxAnzahlFahrten>10</MaxAnzahlFahrten><Hysterese>15</Hysterese><MaxTextLaenge>250</MaxTextLaenge></AboAZB></vdv453:AboAnfrage>";

            TestSerialize(
                subscriptionRequest,
                Vdv453Constants.SubscriptionRequest,
                Msg,
                result =>
                {
                    Assert.AreEqual("iqube", result.Sender);
                    Assert.AreEqual(new DateTime(2013, 09, 22, 17, 55, 44), result.TimeStamp);
                    Assert.IsNotNull(result.Items);
                    Assert.AreEqual(1, result.Items.Count);
                    var item = result.Items[0];
                    Assert.IsInstanceOfType(item, typeof(DisSubscription));
                    dis = (DisSubscription)item;
                    Assert.AreEqual(17u, dis.SubscriptionId);
                    Assert.AreEqual(10u, dis.MaxNumOfTrips);
                    Assert.AreEqual(250u, dis.MaxTextLength);
                    Assert.AreEqual(120u, dis.PreviewTime);
                    Assert.AreEqual(new DateTime(2013, 09, 22, 22, 0, 0), dis.ValidUntilTimeStamp);
                    Assert.AreEqual("AZBID", dis.DisId);
                    Assert.AreEqual(null, dis.LineId);
                    Assert.AreEqual(null, dis.DirectionId);
                },
                "vdv453ger");
        }

        /// <summary>
        /// Test serialization of SubscriptionRequest with a DeleteSubscription
        /// </summary>
        [TestMethod]
        public void Serialize_SubscriptionRequest_DeleteSubscription()
        {
            var subscriptionRequest = new SubscriptionRequest();

            subscriptionRequest.Sender = "iqube";
            subscriptionRequest.TimeStamp = new DateTime(2013, 09, 22, 17, 55, 44, 324);

            var delete = new DeleteSubscription { SubscriptionId = 117u };

            // add delete item
            subscriptionRequest.Items.Add(delete);

            const string Msg =
                "<AboAnfrage Sender=\"iqube\" Zst=\"2013-09-22T17:55:44\"><AboLoeschen>117</AboLoeschen></AboAnfrage>";

            TestSerialize(
                subscriptionRequest,
                Vdv453Constants.SubscriptionRequest,
                Msg,
                result =>
                {
                    Assert.AreEqual("iqube", result.Sender);
                    Assert.AreEqual(new DateTime(2013, 09, 22, 17, 55, 44), result.TimeStamp);
                    Assert.IsNotNull(result.Items);
                    Assert.AreEqual(1, result.Items.Count);
                    var item = result.Items[0];
                    Assert.IsInstanceOfType(item, typeof(DeleteSubscription));
                    delete = (DeleteSubscription)item;
                    Assert.AreEqual(117u, delete.SubscriptionId);
                });
        }

        /// <summary>
        /// Test serialization of SubscriptionRequest with a DeleteSubscription using xml namespace.
        /// </summary>
        [TestMethod]
        public void Serialize_SubscriptionRequest_DeleteSubscription_WithXmlNamespace()
        {
            var subscriptionRequest = new SubscriptionRequest();

            subscriptionRequest.Sender = "iqube";
            subscriptionRequest.TimeStamp = new DateTime(2013, 09, 22, 17, 55, 44, 324);

            var delete = new DeleteSubscription { SubscriptionId = 117u };

            // add delete item
            subscriptionRequest.Items.Add(delete);

            const string Msg =
                "<vdv453:AboAnfrage Sender=\"iqube\" Zst=\"2013-09-22T17:55:44\" xmlns:vdv453=\"vdv453ger\"><AboLoeschen>117</AboLoeschen></vdv453:AboAnfrage>";

            TestSerialize(
                subscriptionRequest,
                Vdv453Constants.SubscriptionRequest,
                Msg,
                result =>
                {
                    Assert.AreEqual("iqube", result.Sender);
                    Assert.AreEqual(new DateTime(2013, 09, 22, 17, 55, 44), result.TimeStamp);
                    Assert.IsNotNull(result.Items);
                    Assert.AreEqual(1, result.Items.Count);
                    var item = result.Items[0];
                    Assert.IsInstanceOfType(item, typeof(DeleteSubscription));
                    delete = (DeleteSubscription)item;
                    Assert.AreEqual(117u, delete.SubscriptionId);
                },
                "vdv453ger");
        }

        /// <summary>
        /// Test serialization of SubscriptionRequest with a DeleteSubscriptionsAll
        /// </summary>
        [TestMethod]
        public void Serialize_SubscriptionRequest_DeleteSubscriptionsAll()
        {
            var subscriptionRequest = new SubscriptionRequest();

            subscriptionRequest.Sender = "iqube";
            subscriptionRequest.TimeStamp = new DateTime(2013, 09, 22, 17, 55, 44, 324);

            var delete = new DeleteSubscriptionsAll();

            // add delete item
            subscriptionRequest.Items.Add(delete);

            const string Msg =
                "<AboAnfrage Sender=\"iqube\" Zst=\"2013-09-22T17:55:44\"><AboLoeschenAlle>true</AboLoeschenAlle></AboAnfrage>";

            TestSerialize(
                subscriptionRequest,
                Vdv453Constants.SubscriptionRequest,
                Msg,
                result =>
                {
                    Assert.AreEqual("iqube", result.Sender);
                    Assert.AreEqual(new DateTime(2013, 09, 22, 17, 55, 44), result.TimeStamp);
                    Assert.IsNotNull(result.Items);
                    Assert.AreEqual(1, result.Items.Count);
                    var item = result.Items[0];
                    Assert.IsInstanceOfType(item, typeof(DeleteSubscriptionsAll));
                    delete = (DeleteSubscriptionsAll)item;
                    Assert.AreEqual(true, delete.Delete);
                });
        }

        /// <summary>
        /// Test serialization of SubscriptionRequest with a DeleteSubscriptionsAll using xml namespace.
        /// </summary>
        [TestMethod]
        public void Serialize_SubscriptionRequest_DeleteSubscriptionsAll_WithXmlNamespace()
        {
            var subscriptionRequest = new SubscriptionRequest();

            subscriptionRequest.Sender = "iqube";
            subscriptionRequest.TimeStamp = new DateTime(2013, 09, 22, 17, 55, 44, 324);

            var delete = new DeleteSubscriptionsAll();

            // add delete item
            subscriptionRequest.Items.Add(delete);

            const string Msg =
                "<vdv453:AboAnfrage Sender=\"iqube\" Zst=\"2013-09-22T17:55:44\" xmlns:vdv453=\"vdv453ger\"><AboLoeschenAlle>true</AboLoeschenAlle></vdv453:AboAnfrage>";
            TestSerialize(
                subscriptionRequest,
                Vdv453Constants.SubscriptionRequest,
                Msg,
                result =>
                {
                    Assert.AreEqual("iqube", result.Sender);
                    Assert.AreEqual(new DateTime(2013, 09, 22, 17, 55, 44), result.TimeStamp);
                    Assert.IsNotNull(result.Items);
                    Assert.AreEqual(1, result.Items.Count);
                    var item = result.Items[0];
                    Assert.IsInstanceOfType(item, typeof(DeleteSubscriptionsAll));
                    delete = (DeleteSubscriptionsAll)item;
                    Assert.AreEqual(true, delete.Delete);
                },
                "vdv453ger");
        }

        /// <summary>
        /// Test serialization of StatusRequest
        /// </summary>
        [TestMethod]
        public void SerializeStatusRequestTest()
        {
            var statusRequest = new StatusRequest
                                    {
                                        Sender = "iqube",
                                        TimeStamp = new DateTime(2013, 09, 22, 17, 55, 44, 324)
                                    };

            const string Msg = "<StatusAnfrage Sender=\"iqube\" Zst=\"2013-09-22T17:55:44\" />";
            TestSerialize(
                statusRequest,
                Vdv453Constants.StatusRequest,
                Msg,                
                result =>
                    {
                        Assert.AreEqual("iqube", result.Sender);
                        Assert.AreEqual(new DateTime(2013, 09, 22, 17, 55, 44), result.TimeStamp);
                    });
        }

        /// <summary>
        /// Test serialization of StatusRequest using xml namespace.
        /// </summary>
        [TestMethod]
        public void SerializeStatusRequestTest_WithXmlNamespace()
        {
            var statusRequest = new StatusRequest
                                    {
                                        Sender = "iqube",
                                        TimeStamp = new DateTime(2013, 09, 22, 17, 55, 44, 324)
                                    };

            const string Msg = "<vdv453:StatusAnfrage Sender=\"iqube\" Zst=\"2013-09-22T17:55:44\" xmlns:vdv453=\"vdv453ger\" />";
            TestSerialize(
                statusRequest,
                Vdv453Constants.StatusRequest,
                Msg,
                result =>
                    {
                        Assert.AreEqual("iqube", result.Sender);
                        Assert.AreEqual(new DateTime(2013, 09, 22, 17, 55, 44), result.TimeStamp);
                    },
                    "vdv453ger");
        }

        /// <summary>
        /// Tests the SubscriptionReply serialization.
        /// </summary>
        [TestMethod]
        public void SerializeSubscriptionReplyTest()
        {
            var subscriptionReply = new SubscriptionReply
                {
                    Acknowledge =
                        new Acknowledge
                            {
                                DataValidUntilSpecified = true,
                                DataValidUntilStr = "2013-09-22T17:55:44",
                                ErrorNumber = 0,
                                ErrorText = null,
                                Result = Result.ok,
                                ShortestPossibleCycleTime = 1,
                                TimeStampStr = "2013-09-22T17:55:44"
                            }
                };

            const string Msg =
                "<AboAntwort><Bestaetigung Zst=\"2013-09-22T17:55:44\" Ergebnis=\"ok\" Fehlernummer=\"0\"><DatenGueltigBis>2013-09-22T17:55:44</DatenGueltigBis><KuerzMoeglicherZyklus>1</KuerzMoeglicherZyklus></Bestaetigung></AboAntwort>";

            TestSerialize(
                subscriptionReply,
                Vdv453Constants.SubscriptionReply,
                Msg,
                result =>
                {
                    Assert.AreEqual("2013-09-22T17:55:44", result.Acknowledge.DataValidUntilStr);
                    Assert.AreEqual(Result.ok, result.Acknowledge.Result);
                });
        }

        /// <summary>
        /// Tests the SubscriptionReply serialization.
        /// </summary>
        [TestMethod]
        public void SerializeSubscriptionReplyTest_WithXmlNamespace()
        {
            var subscriptionReply = new SubscriptionReply
                {
                    Acknowledge =
                        new Acknowledge
                            {
                                DataValidUntilSpecified = true,
                                DataValidUntilStr = "2013-09-22T17:55:44",
                                ErrorNumber = 0,
                                ErrorText = null,
                                Result = Result.ok,
                                ShortestPossibleCycleTime = 1,
                                TimeStampStr = "2013-09-22T17:55:44"
                            }
                };

            const string Msg =
                "<vdv453:AboAntwort xmlns:vdv453=\"vdv453ger\"><Bestaetigung Zst=\"2013-09-22T17:55:44\" Ergebnis=\"ok\" Fehlernummer=\"0\"><DatenGueltigBis>2013-09-22T17:55:44</DatenGueltigBis><KuerzMoeglicherZyklus>1</KuerzMoeglicherZyklus></Bestaetigung></vdv453:AboAntwort>";

            TestSerialize(
                subscriptionReply,
                Vdv453Constants.SubscriptionReply,
                Msg,
                result =>
                {
                    Assert.AreEqual("2013-09-22T17:55:44", result.Acknowledge.DataValidUntilStr);
                    Assert.AreEqual(Result.ok, result.Acknowledge.Result);
                },
                "vdv453ger");
        }

        /// <summary>
        /// Tests the serialization of a StatusReply.
        /// </summary>
        [TestMethod]
        public void SerializeStatusReplyTest()
        {
            var statusReply = new StatusReply
                {
                    DataAvailable = true,
                    StartServiceTimeStamp = TimeProvider.Current.UtcNow,
                    Status = new Status
                        {
                            Result = Result.ok,
                            TimeStamp = TimeProvider.Current.UtcNow
                        }
                };

            const string Msg =
                "<StatusAntwort><Status Zst=\"2013-02-26T13:53:52Z\" Ergebnis=\"ok\" /><DatenBereit>true</DatenBereit><StartDienstZst>2013-02-26T13:53:52Z</StartDienstZst></StatusAntwort>";
            TestSerialize(
                statusReply,
                Vdv453Constants.StatusReply,
                Msg,
                result =>
                {
                    Assert.IsTrue(result.DataAvailable);
                    Assert.AreEqual(TimeProvider.Current.UtcNow, result.StartServiceTimeStamp);
                    Assert.AreEqual(Result.ok, result.Status.Result);
                });
        }

        /// <summary>
        /// Tests the serialization of a StatusReply using xml namespace.
        /// </summary>
        [TestMethod]
        public void SerializeStatusReplyTest_WithXmlNamespace()
        {
            var statusReply = new StatusReply
                {
                    DataAvailable = true,
                    StartServiceTimeStamp = TimeProvider.Current.UtcNow,
                    Status = new Status
                        {
                            Result = Result.ok,
                            TimeStamp = TimeProvider.Current.UtcNow
                        }
                };

            const string Msg =
                "<vdv453:StatusAntwort xmlns:vdv453=\"vdv453test\"><Status Zst=\"2013-02-26T13:53:52Z\" Ergebnis=\"ok\" /><DatenBereit>true</DatenBereit><StartDienstZst>2013-02-26T13:53:52Z</StartDienstZst></vdv453:StatusAntwort>";
            TestSerialize(
                statusReply,
                Vdv453Constants.StatusReply,
                Msg,
                result =>
                {
                    Assert.IsTrue(result.DataAvailable);
                    Assert.AreEqual(TimeProvider.Current.UtcNow, result.StartServiceTimeStamp);
                    Assert.AreEqual(Result.ok, result.Status.Result);
                },
                "vdv453test");
        }

        /// <summary>
        /// Tests the serialization of DataSupplyAnswer.
        /// </summary>
        [TestMethod]
        public void SerializeDataSupplyAnswerTest()
        {
            var dataSupplyAnswer = new DataSupplyAnswer
                {
                    Acknowledge =
                        new Acknowledge
                            {
                                DataValidUntilSpecified = true,
                                DataValidUntilStr = "2013-02-26T13:53:52Z",
                                Result = Result.ok,
                                ShortestPossibleCycleTime = 1,
                                TimeStamp = TimeProvider.Current.UtcNow
                            },
                            Messages = new List<DisMessage>
                                {
                                    new DisMessage
                                        {
                                            Messages = new List<Vdv453Message>
                                                {
                                                    new DisTripDelete
                                                        {
                                                            DepartureNoticeId = 1,
                                                            DirectionId = "1",
                                                            DirectionText = "1",
                                                            DisId = "1",
                                                            LineId = "1",
                                                            LineText = "1",
                                                            StopSeqCount = "1",
                                                            TripId = new TripId
                                                                {
                                                                    DayType = UtcNow.Date,
                                                                    TripName = "1"
                                                                }
                                                        }
                                                }
                                        }
                                },
                    PendingData = true
                };

            const string Msg =
                "<DatenAbrufenAntwort><Bestaetigung Zst=\"2013-02-26T13:53:52Z\" Ergebnis=\"ok\" Fehlernummer=\"0\"><DatenGueltigBis>2013-02-26T13:53:52Z</DatenGueltigBis><KuerzMoeglicherZyklus>1</KuerzMoeglicherZyklus></Bestaetigung><WeitereDaten>true</WeitereDaten><AZBNachricht><AZBFahrtLoeschen Zst=\"0001-01-01T00:00:00\"><AZBID>1</AZBID><FahrtID><FahrtBezeichner>1</FahrtBezeichner><Betriebstag>2013-02-26</Betriebstag></FahrtID><HstSeqZaehler>1</HstSeqZaehler><LinienID>1</LinienID><LinienText>1</LinienText><RichtungsID>1</RichtungsID><RichtungsText>1</RichtungsText><AbmeldeID>1</AbmeldeID></AZBFahrtLoeschen></AZBNachricht></DatenAbrufenAntwort>";
            TestSerialize(
                dataSupplyAnswer,
                Vdv453Constants.DataSupplyAnswer,
                Msg,
                result =>
                {
                    Assert.AreEqual(Result.ok, result.Acknowledge.Result);
                    Assert.AreEqual(1U, result.Acknowledge.ShortestPossibleCycleTime);
                });
        }

        /// <summary>
        /// Tests the serialization of DataSupplyAnswer using xml namespace.
        /// </summary>
        [TestMethod]
        public void SerializeDataSupplyAnswerTest_WithXmlNamespace()
        {
            var dataSupplyAnswer = new DataSupplyAnswer
                {
                    Acknowledge =
                        new Acknowledge
                            {
                                DataValidUntilSpecified = true,
                                DataValidUntilStr = "2013-02-26T13:53:52Z",
                                Result = Result.ok,
                                ShortestPossibleCycleTime = 1,
                                TimeStamp = TimeProvider.Current.UtcNow
                            },
                            Messages = new List<DisMessage>
                                {
                                    new DisMessage
                                        {
                                            Messages = new List<Vdv453Message>
                                                {
                                                    new DisTripDelete
                                                        {
                                                            DepartureNoticeId = 1,
                                                            DirectionId = "1",
                                                            DirectionText = "1",
                                                            DisId = "1",
                                                            LineId = "1",
                                                            LineText = "1",
                                                            StopSeqCount = "1",
                                                            TripId = new TripId
                                                                {
                                                                    DayType = UtcNow.Date,
                                                                    TripName = "1"
                                                                }
                                                        }
                                                }
                                        }
                                },
                    PendingData = true
                };

            const string Msg =
                "<vdv453:DatenAbrufenAntwort xmlns:vdv453=\"vdv453test\"><Bestaetigung Zst=\"2013-02-26T13:53:52Z\" Ergebnis=\"ok\" Fehlernummer=\"0\"><DatenGueltigBis>2013-02-26T13:53:52Z</DatenGueltigBis><KuerzMoeglicherZyklus>1</KuerzMoeglicherZyklus></Bestaetigung><WeitereDaten>true</WeitereDaten><AZBNachricht><AZBFahrtLoeschen Zst=\"0001-01-01T00:00:00\"><AZBID>1</AZBID><FahrtID><FahrtBezeichner>1</FahrtBezeichner><Betriebstag>2013-02-26</Betriebstag></FahrtID><HstSeqZaehler>1</HstSeqZaehler><LinienID>1</LinienID><LinienText>1</LinienText><RichtungsID>1</RichtungsID><RichtungsText>1</RichtungsText><AbmeldeID>1</AbmeldeID></AZBFahrtLoeschen></AZBNachricht></vdv453:DatenAbrufenAntwort>";
            TestSerialize(
                dataSupplyAnswer,
                Vdv453Constants.DataSupplyAnswer,
                Msg,
                result =>
                {
                    Assert.AreEqual(Result.ok, result.Acknowledge.Result);
                    Assert.AreEqual(1U, result.Acknowledge.ShortestPossibleCycleTime);
                },
                "vdv453test");
        }

        private static void TestSerialize<T>(
            T message,
            string elementName,
            string expectedMessage,
            Action<T> deserializeVerification,
            string xmlNameSpace = null)
            where T : Vdv453Message
        {
            var expectedNoNsInclude = VdvXmlDeclaration + expectedMessage;
            var expectedNoNsOmit = expectedMessage;

            var serializer = new Vdv453MessageSerializer<T>(elementName, xmlNameSpace);
            Assert.AreEqual(expectedNoNsInclude, serializer.Serialize(message, false));
            Assert.AreEqual(expectedMessage, serializer.Serialize(message, true));

            var result = serializer.Deserialize(new XmlTextReader(new StringReader(expectedNoNsInclude)));
            deserializeVerification(result);

            result = serializer.Deserialize(new XmlTextReader(new StringReader(expectedNoNsOmit)));
            deserializeVerification(result);
        }

        // ReSharper restore InconsistentNaming
    }
}
