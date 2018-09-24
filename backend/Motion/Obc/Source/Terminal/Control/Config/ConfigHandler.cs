// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Config
{
    using System;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Motion.Obc.Terminal.Control.Data;

    /// <summary>
    ///   Responsible to handle the config files
    ///   There exist the main configuration, the short key config and the menu config
    /// </summary>
    internal class ConfigHandler
    {
        private readonly ConfigManager<TerminalConfig> config;

        private readonly ConfigManager<MultiLangListItem> configMenu;

        private readonly ConfigManager<ShortKeyConfig> configShortKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigHandler"/> class.
        /// </summary>
        public ConfigHandler()
        {
            this.config = new ConfigManager<TerminalConfig>();
            this.config.EnableCaching = true;
            this.config.FileName = PathManager.Instance.GetPath(FileType.Config, ConfigPaths.Config);

            this.configMenu = new ConfigManager<MultiLangListItem>();
            this.configMenu.EnableCaching = true;
            this.configMenu.FileName = PathManager.Instance.GetPath(FileType.Config, ConfigPaths.ConfigMenu);

            this.configShortKey = new ConfigManager<ShortKeyConfig>();
            this.configShortKey.EnableCaching = true;
            this.configShortKey.FileName = PathManager.Instance.GetPath(FileType.Config, ConfigPaths.ConfigShortKey);
        }

        /// <summary>
        ///   Gets the main configuration.
        ///   Check first IsConfigValid()
        /// </summary>
        /// <returns>the main configuration</returns>
        public TerminalConfig GetConfig()
        {
            return this.config.Config;
        }

        /// <summary>
        ///   Gets the short key configuration
        /// </summary>
        /// <returns>the short key configuration</returns>
        public ShortKeyConfig GetShortKeyConfig()
        {
            return this.configShortKey.Config;
        }

        /// <summary>
        ///   Gets the menu configuration
        /// </summary>
        /// <returns>the menu configuration</returns>
        public MultiLangListItem GetMenuConfig()
        {
            return this.configMenu.Config;
        }
    }
}