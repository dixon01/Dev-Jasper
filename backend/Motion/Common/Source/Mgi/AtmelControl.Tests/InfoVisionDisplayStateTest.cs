// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoVisionDisplayStateTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InfoVisionDisplayStateTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.Tests
{
    using System.IO;
    using System.Text;

    using Gorba.Motion.Common.Mgi.AtmelControl.JsonRpc;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit test for the <see cref="InfovisionDisplayState"/> class.
    /// </summary>
    [TestClass]
    public class InfoVisionDisplayStateTest
    {
        /// <summary>
        /// Tests de-serializing JSON.
        /// </summary>
        [TestMethod]
        public void DeserializeTest()
        {
            var memory =
                new MemoryStream(Encoding.ASCII.GetBytes(
                    "{ \"jsonrpc\": \"2.0\", \"method\": \"notifyObject\", \"params\": "
                    + "{ \"objectName\": \"InfovisionDisplayState\", \"ConnectedDisplayNo\": 1, "
                    + "\"Display\": [ { \"Address\": 5, \"ConnectionState\": \"connected\", "
                    + "\"BacklightExternal_1_OK\": 0, \"BacklightExternal_2_OK\": 0, \"Ignition_On\": 1, "
                    + "\"PowerHold_On\": 0, \"Backlight24V_OK\": 1, \"PowerState\": 1, "
                    + "\"BacklightInternal_1_OK\": 1, \"BacklightInternal_2_OK\": 1, \"GenesisPresent_1\": 1, "
                    + "\"GenesisPresent_2\": 0, \"BacklightMode_1\": 0, \"BacklightMode_2\": 0, "
                    + "\"Panel\": [ { \"PanelNo\": 0, \"Temperature\": 58, \"BacklightValue\": 100, "
                    + "\"BacklightMin\": 1, \"BacklightMax\": 255, \"BacklightSpeed\": 1, \"Lux\": 367, "
                    + "\"EqLevel\": 0, \"SignalStable\": 1, \"SignalFlags\": 64, \"Contrast\": 1, "
                    + "\"Sharpness\": 1, \"ColorRed\": 25, \"ColorGreen\": 75, \"ColorBlue\": 99 } ] } ] } }"));
            var target = new JsonRpcStreamHandler(memory);

            var read = target.Read();

            Assert.IsNotNull(read);
            Assert.IsInstanceOfType(read, typeof(RpcNotification));
            var notification = (RpcNotification)read;

            Assert.AreEqual("notifyObject", notification.method);
            Assert.IsNotNull(notification.@params);

            var state = notification.GetParams<InfovisionDisplayState>();
            Assert.IsNotNull(state);

            Assert.AreEqual(1, state.ConnectedDisplayNo);

            Assert.IsNotNull(state.Display);
            Assert.AreEqual(1, state.Display.Length);

            var display = state.Display[0];
            Assert.AreEqual(5, display.Address);
            Assert.AreEqual(DisplayConnectionState.connected, display.ConnectionState);
            Assert.AreEqual(0, display.BacklightExternal_1_OK);
            Assert.AreEqual(0, display.BacklightExternal_2_OK);
            Assert.AreEqual(1, display.Ignition_On);
            Assert.AreEqual(0, display.PowerHold_On);
            Assert.AreEqual(1, display.Backlight24V_OK);
            Assert.AreEqual(1, display.PowerState);
            Assert.AreEqual(1, display.BacklightInternal_1_OK);
            Assert.AreEqual(1, display.BacklightInternal_2_OK);
            Assert.AreEqual(1, display.GenesisPresent_1); // Genesis_Present_1 vs GenesisPresent_1 ???
            Assert.AreEqual(0, display.GenesisPresent_2);

            Assert.IsNotNull(display.Panel);
            Assert.AreEqual(1, display.Panel.Length);

            var panel = display.Panel[0];
            Assert.AreEqual(0, panel.PanelNo); // forgotten in spec!!!
            Assert.AreEqual(58, panel.Temperature);
            Assert.AreEqual(100, panel.BacklightValue);
            Assert.AreEqual(1, panel.BacklightMin);
            Assert.AreEqual(255, panel.BacklightMax);
            Assert.AreEqual(1, panel.BacklightSpeed);
            Assert.AreEqual(367, panel.Lux);
            Assert.AreEqual(0, panel.EqLevel);
            Assert.AreEqual(1, panel.SignalStable);
            Assert.AreEqual(64, panel.SignalFlags);
            Assert.AreEqual(1, panel.Contrast);
            Assert.AreEqual(1, panel.Sharpness);
            Assert.AreEqual(25, panel.ColorRed);
            Assert.AreEqual(75, panel.ColorGreen);
            Assert.AreEqual(99, panel.ColorBlue);
        }
    }
}
