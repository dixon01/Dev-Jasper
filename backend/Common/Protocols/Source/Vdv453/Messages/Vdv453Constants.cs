// -------------------------------------------------------------------------------------------------------------------
// <copyright file="VDV453Constants.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines VDV453 constants.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines VDV453 constants.
    /// ========================================================================
    /// IMPORTANT: don't change the names of the constants, they are according 
    /// to the English version of the VDV 453 specification.
    /// ========================================================================
    /// See VDV 453 English (2.3), chapters 9.2 and 9.3 for all translations
    /// We use the VDV 453 English Aliases, not the SIRI translation
    /// ========================================================================
    /// Only exception: Replaced CAPS words with PascalCase 
    /// </summary>
    public static class Vdv453Constants
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Defines VDV453 element name SubscriptionRequest (AboAnfrage)
        /// </summary>
        public const string SubscriptionRequest = "AboAnfrage";

        /// <summary>
        /// Defines VDV453 element name SubscriptionReply (AboAntwort)
        /// </summary>
        public const string SubscriptionReply = "AboAntwort";

        /// <summary>
        /// Defines VDV453 element name DeleteSubscription (AboLoeschen)
        /// </summary>
        public const string DeleteSubscription = "AboLoeschen";

        /// <summary>
        /// Defines VDV453 element name DeleteSubscriptionsAll (AboLoeschenAlle)
        /// </summary>
        public const string DeleteSubscriptionsAll = "AboLoeschenAlle";

        /// <summary>
        /// Defines VDV453 element name DISSubscription (AboAZB)
        /// </summary>
        public const string DisSubscription = "AboAZB";

        /// <summary>
        /// Defines VDV453 element name DISRefSubscription (AboAZBRef)
        /// </summary>
        public const string DisRefSubscription = "AboAZBRef";

        /// <summary>
        /// Defines VDV453 element name DISSchedule (AZBFahrplan)
        /// </summary>
        public const string DisSchedule = "AZBFahrplan";

        /// <summary>
        /// Defines VDV453 element name DISDeviation (AZBFahrplanlage)
        /// </summary>
        public const string DisDeviation = "AZBFahrplanlage";

        /// <summary>
        /// Defines VDV453 element name DISTripDelete (AZBFahrtLoeschen)
        /// </summary>
        public const string DisTripDelete = "AZBFahrtLoeschen";

        /// <summary>
        /// Defines VDV453 element name DISLineSpecialText (AZBLinienspezialtext)
        /// </summary>
        public const string DisLineSpecialText = "AZBLinienspezialtext";

        /// <summary>
        /// Defines VDV453 element name DISLineSpecialTextDelete (AZBLinienspezialtextLoeschen)
        /// </summary>
        public const string DisLineSpecialTextDelete = "AZBLinienspezialtextLoeschen";

        /// <summary>
        /// Defines VDV453 element name DISMessage (AZBNachricht)
        /// </summary>
        public const string DisMessage = "AZBNachricht";

        /// <summary>
        /// Defines VDV453 element name DataSupplyRequest (DatenAbrufenAnfrage)
        /// </summary>
        public const string DataSupplyRequest = "DatenAbrufenAnfrage";

        /// <summary>
        /// Defines VDV453 element name DataSupplyAnswer (DatenAbrufenAntwort)
        /// </summary>
        public const string DataSupplyAnswer = "DatenAbrufenAntwort";

        /// <summary>
        /// Defines VDV453 element name DataReadyRequest (DatenBereitAnfrage)
        /// </summary>
        public const string DataReadyRequest = "DatenBereitAnfrage";

        /// <summary>
        /// Defines VDV453 element name DataReadyAnswer (DatenBereitAntwort)
        /// </summary>
        public const string DataReadyAnswer = "DatenBereitAntwort";

        /// <summary>
        /// Defines VDV453 element name DataAvailable (DatenBereit)
        /// </summary>
        public const string DataAvailable = "DatenBereit";

        /// <summary>
        /// Defines VDV453 element name StartServiceTimeStamp (StartDienstZst)
        /// </summary>
        public const string StartServiceTimeStamp = "StartDienstZst";

        /// <summary>
        /// Defines VDV453 element name TripFilter (FahrtFilter)
        /// </summary>
        public const string TripFilter = "FahrtFilter";

        /// <summary>
        /// Defines VDV453 element name TripID (FahrtID)
        /// </summary>
        public const string TripId = "FahrtID";

        /// <summary>
        /// Defines VDV453 element name TripInfo (FahrtInfo)
        /// </summary>
        public const string TripInfo = "FahrtInfo";

        /// <summary>
        /// Defines VDV453 element name StopPositionChange (HaltepositionsAenderung)
        /// </summary>
        public const string StopPositionChange = "HaltepositionsAenderung";

        /// <summary>
        /// Defines VDV453 attribute name Sender (Sender)
        /// </summary>
        public const string Sender = "Sender";

        /// <summary>
        /// Defines VDV453 element name AllData (DatensatzAlle)
        /// </summary>
        public const string AllData = "DatensatzAlle";

        /// <summary>
        /// Defines VDV453 element name PendingData (WeitereDaten)
        /// </summary>
        public const string PendingData = "WeitereDaten";

        /// <summary>
        /// Defines VDV453 element name StatusRequest (StatusAnfrage)
        /// </summary>
        public const string StatusRequest = "StatusAnfrage";

        /// <summary>
        /// Defines VDV453 element name StatusReply (StatusAntwort)
        /// </summary>
        public const string StatusReply = "StatusAntwort";

        /// <summary>
        /// Defines VDV453 element name Status (Status)
        /// </summary>
        public const string Status = "Status";

        /// <summary>
        /// Defines VDV453 element name Acknowledge (Bestaetigung)
        /// </summary>
        public const string Acknowledge = "Bestaetigung";

        /// <summary>
        /// Defines VDV453 element name Result (Ergebnis)
        /// </summary>
        public const string Result = "Ergebnis";

        /// <summary>
        /// Defines VDV453 attribute name ErrorNumber (Fehlernummer)
        /// </summary>
        public const string ErrorNumber = "Fehlernummer";

        /// <summary>
        /// Defines VDV453 element name DISID (AZBID)
        /// </summary>
        public const string DisId = "AZBID";

        /// <summary>
        /// Defines VDV453 element name SubscriptionId (AboID)
        /// </summary>
        public const string SubscriptionId = "AboID";

        /// <summary>
        /// Defines VDV453 attribute name TimeStamp (Zst)
        /// </summary>
        public const string TimeStamp = "Zst";

        /// <summary>
        /// Defines VDV453 element name DirectionId (RichtungsID)
        /// </summary>
        public const string DirectionId = "RichtungsID";

        /// <summary>
        /// Defines VDV453 element name DirectionText (RichtungsText)
        /// </summary>
        public const string DirectionText = "RichtungsText";

        /// <summary>
        /// Defines VDV453 element name LineId (LinienID)
        /// </summary>
        public const string LineId = "LinienID";

        /// <summary>
        /// Defines VDV453 element name LineText (LinienText)
        /// </summary>
        public const string LineText = "LinienText";

        /// <summary>
        /// Defines VDV453 element name LineSpecialText (Linienspezialtext)
        /// </summary>
        public const string LineSpecialText = "Linienspezialtext";

        /// <summary>
        /// Defines VDV453 element name Priority (Prioritaet)
        /// </summary>
        public const string Priority = "Prioritaet";

        /// <summary>
        /// Defines VDV453 element name EarliestDepartureTime (FruehesteAbfahrtszeit)
        /// </summary>
        public const string EarliestDepartureTime = "FruehesteAbfahrtszeit";

        /// <summary>
        /// Defines VDV453 element name LatestDepartureTime (SpaetesteAbfahrtszeit)
        /// </summary>
        public const string LatestDepartureTime = "SpaetesteAbfahrtszeit";

        /// <summary>
        /// Defines VDV453 attribute name ValidUntilTimeStamp (VerfallZst)
        /// </summary>
        public const string ValidUntilTimeStamp = "VerfallZst";

        /// <summary>
        /// Defines VDV453 element name StopSeqCount (HstSeqZaehler)
        /// </summary>
        public const string StopSeqCount = "HstSeqZaehler";

        /// <summary>
        /// Defines VDV453 element name ScheduledDISArrivalTime (AnkunftszeitAZBPlan)
        /// </summary>
        public const string ScheduledDisArrivalTime = "AnkunftszeitAZBPlan";

        /// <summary>
        /// Defines VDV453 element name ScheduledDISDepartureTime (AbfahrtszeitAZBPlan)
        /// </summary>
        public const string ScheduledDisDepartureTime = "AbfahrtszeitAZBPlan";

        /// <summary>
        /// Defines VDV453 element name MaxNumOfTrips (MaxAnzahlFahrten)
        /// </summary>
        public const string MaxNumOfTrips = "MaxAnzahlFahrten";

        /// <summary>
        /// Defines VDV453 element name MaxTextLength (MaxTextLaenge)
        /// </summary>
        public const string MaxTextLength = "MaxTextLaenge";

        /// <summary>
        /// Defines VDV453 element name DataValidUntil (DatenGueltigBis)
        /// </summary>
        public const string DataValidUntil = "DatenGueltigBis";

        /// <summary>
        /// Defines VDV453 element name DayType (Betriebstag)
        /// </summary>
        public const string DayType = "Betriebstag";

        /// <summary>
        /// Defines VDV453 element name PreviewTime (Vorschauzeit)
        /// </summary>
        public const string PreviewTime = "Vorschauzeit";

        /// <summary>
        /// Defines VDV453 element name Hysteresis (Hysterese)
        /// </summary>
        public const string Hysteresis = "Hysterese";

        /// <summary>
        /// Defines VDV453 element name TripName (FahrtBezeichner)
        /// </summary>
        public const string TripName = "FahrtBezeichner";

        /// <summary>
        /// Defines VDV453 element name VehicleID (FahrzeugID)
        /// </summary>
        public const string VehicleId = "FahrzeugID";

        /// <summary>
        /// Defines VDV453 element name LineNumber (LinienNr)
        /// </summary>
        public const string LineNumber = "LinienNr";

        /// <summary>
        /// Defines VDV453 element name BlockNumber (UmlaufNr)
        /// </summary>
        public const string BlockNumber = "UmlaufNr";

        /// <summary>
        /// Defines VDV453 element name RunNumber (KursNr)
        /// </summary>
        public const string RunNumber = "KursNr";

        /// <summary>
        /// Defines VDV453 element name DepartureStopLong (StartHstLang)
        /// </summary>
        public const string DepartureStopLong = "StartHstLang";

        /// <summary>
        /// Defines VDV453 element name DepartureStop (StartHst)
        /// </summary>
        public const string DepartureStop = "StartHst";

        /// <summary>
        /// Defines VDV453 element name DestinationStopLong (ZielHstLang)
        /// </summary>
        public const string DestinationStopLong = "ZielHstLang";

        /// <summary>
        /// Defines VDV453 element name DestinationStop (ZielHst)
        /// </summary>
        public const string DestinationStop = "ZielHst";

        /// <summary>
        /// Defines VDV453 element name DepartureTimeStartStop (AbfahrtszeitStartHst)
        /// </summary>
        public const string DepartureTimeStartStop = "AbfahrtszeitStartHst";

        /// <summary>
        /// Defines VDV453 element name ArrivalTimeDestinationStop (AnkunftszeitZielHst)
        /// </summary>
        public const string ArrivalTimeDestinationStop = "AnkunftszeitZielHst";

        /// <summary>
        /// Defines VDV453 element name ProductID (ProduktID)
        /// </summary>
        public const string ProductId = "ProduktID";

        /// <summary>
        /// Defines VDV453 element name Operator (Betreiber)
        /// </summary>
        public const string Operator = "Betreiber";

        /// <summary>
        /// Defines VDV453 element name ServiceAttribute (ServiceMerkmal)
        /// </summary>
        public const string ServiceAttribute = "ServiceMerkmal";

        /// <summary>
        /// Defines VDV453 element name Trainset (Traktion)
        /// </summary>
        public const string Trainset = "Traktion";

        /// <summary>
        /// Defines VDV453 element name TrainsetID (TraktionsID)
        /// </summary>
        public const string TrainsetId = "TraktionsID";

        /// <summary>
        /// Defines VDV453 element name NumOfTrips (AnzahlFahrten)
        /// </summary>
        public const string NumOfTrips = "AnzahlFahrten";

        /// <summary>
        /// Defines VDV453 element name Position (Position)
        /// </summary>
        public const string Position = "Position";

        /// <summary>
        /// Defines VDV453 element name AtDISPoint (AufAZB)
        /// </summary>
        public const string AtDisPoint = "AufAZB";

        /// <summary>
        /// Defines VDV453 element name ViaStop1 (ViaHst1Lang)
        /// </summary>
        public const string ViaStop1 = "ViaHst1Lang";

        /// <summary>
        /// Defines VDV453 element name ViaStop2 (ViaHst2Lang)
        /// </summary>
        public const string ViaStop2 = "ViaHst2Lang";

        /// <summary>
        /// Defines VDV453 element name ViaStop3 (ViaHst3Lang)
        /// </summary>
        public const string ViaStop3 = "ViaHst3Lang";

        /// <summary>
        /// Defines VDV453 element name TripStatus (FahrtStatus)
        /// </summary>
        public const string TripStatus = "FahrtStatus";

        /// <summary>
        /// Defines VDV453 element name ExpectedDISArrivalTime (AnkunftszeitAZBPrognose)
        /// </summary>
        public const string ExpectedDisArrivalTime = "AnkunftszeitAZBPrognose";

        /// <summary>
        /// Defines VDV453 element name ExpectedDISDepartureTime (AbfahrtszeitAZBPrognose)
        /// </summary>
        public const string ExpectedDisDepartureTime = "AbfahrtszeitAZBPrognose";

        /// <summary>
        /// Defines VDV453 element name AimedDISDepartureTime (AbfahrtszeitAZBDisposition)
        /// </summary>
        public const string AimedDisDepartureTime = "AbfahrtszeitAZBDisposition";

        /// <summary>
        /// Defines VDV453 element name TripSpecialText (Fahrtspezialtext)
        /// </summary>
        public const string TripSpecialText = "Fahrtspezialtext";

        /// <summary>
        /// Defines VDV453 element name SpeechOutput (Sprachausgabe)
        /// </summary>
        public const string SpeechOutput = "Sprachausgabe";

        /// <summary>
        /// Defines VDV453 element name StopId (HaltID)
        /// </summary>
        public const string StopId = "HaltID";

        /// <summary>
        /// Defines VDV453 element name StopPositionText (HaltepositionsText)
        /// </summary>
        public const string StopPositionText = "HaltepositionsText";

        /// <summary>
        /// Defines VDV453 element name QueueIndicator (Stauindikator)
        /// </summary>
        public const string QueueIndicator = "Stauindikator";

        /// <summary>
        /// Defines VDV453 element name VehicleNumber (BetrieblicheFahrzeugnummer)
        /// </summary>
        public const string VehicleNumber = "BetrieblicheFahrzeugnummer";

        /// <summary>
        /// Defines VDV453 element name Reason (Ursache)
        /// </summary>
        public const string Reason = "Ursache";

        /// <summary>
        /// Defines VDV453 element name DepartureNoticeID (AbmeldeID)
        /// </summary>
        public const string DepartureNoticeId = "AbmeldeID";

        /// <summary>
        /// Defines VDV453 element name Errortext (Fehlertext)
        /// </summary>
        public const string ErrorText = "Fehlertext";

        /// <summary>
        /// Defines VDV453 element name ShortestPossibleCycleTime (KuerzMoeglicherZyklus)
        /// </summary>
        public const string ShortestPossibleCycleTime = "KuerzMoeglicherZyklus";

        /// <summary>
        /// Defines VDV453 URI name for subscription messages (aboverwalten.xml)
        /// </summary>
        public const string UriSubscription = "aboverwalten.xml";

        /// <summary>
        /// Defines VDV453 URI name for status messages (status.xml)
        /// </summary>
        public const string UriStatus = "status.xml";

        /// <summary>
        /// Defines VDV453 URI name for DataAvailable messages (datenbereit.xml)
        /// </summary>
        public const string UriDataready = "datenbereit.xml";

        /// <summary>
        /// Defines VDV453 URI name for GetData messages (datenabrufen.xml)
        /// </summary>
        public const string UriGetdata = "datenabrufen.xml";

        /// <summary>
        /// Defines VDV453 date and time format
        /// </summary>
        public const string DatetimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";

        /// <summary>
        /// Defines VDV453 date format
        /// </summary>
        public const string DateFormat = "yyyy'-'MM'-'dd";

        // ReSharper enable InconsistentNaming
    }
}