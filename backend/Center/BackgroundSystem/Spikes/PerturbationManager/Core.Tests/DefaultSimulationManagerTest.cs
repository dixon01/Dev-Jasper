namespace Core.Tests
{
    using Google.Transit.Realtime;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DefaultSimulationManagerTest
    {
        [TestMethod]
        public void GetNextIdSingleEntityTest()
        {
            var feedMessage = new FeedMessage { entity = { new FeedEntity { id = "-1" } } };
            var simulationManager = new SimulationManager.DefaultSimulationManager();
            simulationManager.Initialize(feedMessage);
            Assert.AreEqual("1", feedMessage.entity[0].id);
            var next = simulationManager.GetNext();
            Assert.AreEqual("2", next.entity[0].id);
        }
        [TestMethod]
        public void GetNextIdMultipleEntitiesTest()
        {
            var feedMessage = new FeedMessage
                {
                    entity =
                        {
                            new FeedEntity { id = "-1" }, new FeedEntity { id = "-1" }, new FeedEntity { id = "-1" }
                        }
                };
            var simulationManager = new SimulationManager.DefaultSimulationManager();
            simulationManager.Initialize(feedMessage);
            Assert.AreEqual("1", feedMessage.entity[0].id);
            Assert.AreEqual("2", feedMessage.entity[1].id);
            Assert.AreEqual("3", feedMessage.entity[2].id);
            var next = simulationManager.GetNext();
            Assert.AreEqual("4", next.entity[0].id);
            Assert.AreEqual("5", next.entity[1].id);
            Assert.AreEqual("6", next.entity[2].id);
            next = simulationManager.GetNext();
            Assert.AreEqual("7", next.entity[0].id);
            Assert.AreEqual("8", next.entity[1].id);
            Assert.AreEqual("9", next.entity[2].id);
        }
    }
}
