namespace Luminator.AdhocMessaging.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AdhocConfigurationTests
    {
        [TestMethod]
        public void AdhocConfigurationEmptyTest()
        {
            var ac = new AdhocConfiguration();
            Assert.IsNotNull(ac);
        }

        [TestMethod]
        public void AdhocConfigurationParamsTest()
        {
            var ac = new AdhocConfiguration();
            Assert.IsNotNull(ac);
        }

        [TestMethod]
        public void AdhocConfigurationTest()
        {
            var server = "http://swdevicntrapp.luminatorusa.com/";
            var port = string.Empty;
            var serverMessagesApi = "http://swdevicntrweb.luminatorusa.com/";
            var portMessagesApi = string.Empty;
            var ac = new AdhocConfiguration(server, port, serverMessagesApi, portMessagesApi);

            Assert.IsNotNull(ac);
        }
    }
}