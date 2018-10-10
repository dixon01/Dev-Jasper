namespace Luminator.PresentationLogging.UnitTest
{
    using Luminator.PresentationPlayLogging.Core.Models;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InfotransitPresentationInfoTests
    {
        [TestMethod]
        public void InfoTransData_WithCompleteData_Is_Valid()
        {
            var info = new InfotransitPresentationInfo
                            {
                                StartedLatitude = "33.019844",
                                StartedLongitude = "-96.698883",
                                StoppedLatitude = "33.019846",
                                StoppedLongitude = "-96.698886",
                                FileName = @"C:\Foobar1.jpg",
                                VehicleId = "112233",
                                Route = "ROUTE1",
                                UnitName = "TFT-1-2-3-4"
                            };

            Assert.IsTrue(info.IsValid);
        }

        [TestMethod]
        public void IsValid_WillReturnFalse_IfNoUnitExists()
        {
            var info = new InfotransitPresentationInfo
                           {
                               FileName = "123",
                               UnitName = string.Empty
                           };
            
            Assert.IsFalse(info.IsValid);
        }

        [TestMethod]
        public void IsValid_WillReturnFalse_IfNoFileNameExists()
        {
            var info = new InfotransitPresentationInfo
                           {
                               UnitName = "A"
                           };

            Assert.IsFalse(info.IsValid);
        }

        [TestMethod]
        public void IsValid_WillReturnTrue_IfFileNameAndUnitExists()
        {
            var info = new InfotransitPresentationInfo
                           {
                               FileName = "123",
                               UnitName = "Green"
                           };

            Assert.IsTrue(info.IsValid);
        }

        [TestMethod]
        public void InfoTransData_WithoutData_IsInvalid()
        {
            var info = new InfotransitPresentationInfo();
            Assert.IsFalse(info.IsValid);
        }
    }
}