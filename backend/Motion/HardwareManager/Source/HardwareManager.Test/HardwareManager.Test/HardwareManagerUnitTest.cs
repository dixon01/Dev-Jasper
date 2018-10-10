// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="UnitTest1.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace HardwareManager.Test
{
    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.HardwareManager;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Motion.HardwareManager.Core.Settings;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>The HardwareManagerUnitTest.</summary>
    [DeploymentItem(@"Config\HardwareManager.xml")]
    [TestClass]
    public class HardwareManagerUnitTest
    {
        #region Public Methods and Operators

        /// <summary>The read hardware manager xml settings.</summary>
        [TestMethod]
        public void ReadHardwareManagerConfigXmlSettings()
        {
            var configManager = new ConfigManager<HardwareManagerConfig>
            {
                FileName =
            PathManager.Instance.GetPath(FileType.Config, "HardwareManager.xml"),
                EnableCaching = true,
                XmlSchema = HardwareManagerConfig.Schema
            };

            var config = configManager.Config;
            Assert.IsNotNull(config);
            Assert.AreEqual(false, config.Mgi.Enabled);
            Assert.AreEqual(false, config.Vdv301.Enabled);
            Assert.IsNull(config.Gps.Client);
        }

        [TestMethod]
        [Ignore]
        public void ApplySettings()
        {
            var handler = new HostnameSettingsHandler();
            var hardwareManagerSetting = new HardwareManagerSetting()
            {
                HostnameSource = HostnameSource.MacAddress,
                UseDhcp = true
            };

            // Caution machine rename possible
            handler.ApplySetting(hardwareManagerSetting);
        }

        #endregion
    }
}