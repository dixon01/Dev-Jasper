using Microsoft.VisualStudio.TestTools.UnitTesting;
using Luminator.Multicast.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminator.Multicast.Core.Tests
{
    using System.Diagnostics;
    using System.Runtime.Remoting.Channels;
    using System.Threading;

    [TestClass()]
    public class MulticastManagerTests
    {
      
        [TestMethod]
        public void CreateMulticastManagerTest()
        {
            var multicastManager = new MulticastManager();
            Assert.IsNotNull(multicastManager);
            multicastManager.StartMulticast();
            multicastManager.StopMulticast();
        }

        [TestMethod]
        public void MulticastCommandUpdateEventRaisedTest()
        {
            ManualResetEvent signal = new ManualResetEvent(false);
            var multicastManager = new MulticastManager();
            Assert.IsNotNull(multicastManager);
            multicastManager.MultiCastCommand += (sender, args) =>
                {
                    MulticastCommandEventArgs e = args as MulticastCommandEventArgs;
                    if (e != null)
                    {
                        signal.Set();
                        Console.WriteLine($"Received Multicast Command {e.CommandToExecute}");
                        Assert.AreEqual(e.CommandToExecute, MulticastCommands.Update);
                    }
                };

            multicastManager.MulticastActionCommand = MulticastCommands.Update;
            Assert.IsTrue(signal.WaitOne(2000));
           
            multicastManager.StopMulticast();
        }

        [TestMethod]
        public void MulticastCommandShowDisplayEventRaisedTest()
        {
            var multicastManager = new MulticastManager();
            Assert.IsNotNull(multicastManager);
            multicastManager.MultiCastCommand += (sender, args) =>
            {
                MulticastCommandEventArgs e = args as MulticastCommandEventArgs;
                if (e != null)
                {
                    Console.WriteLine($"Received Multicast Command {e.CommandToExecute}");
                    Assert.AreEqual(e.CommandToExecute, MulticastCommands.ShowDisplay);
                }
            };

            multicastManager.MulticastActionCommand = MulticastCommands.ShowDisplay;
           
            multicastManager.StopMulticast();
        }

        [TestMethod]
        public void CheckIfProcessIsRunning()
        {
            Process[] localByName = Process.GetProcessesByName("devenv");
            Assert.IsNotNull(localByName);

        }

       
    }
}