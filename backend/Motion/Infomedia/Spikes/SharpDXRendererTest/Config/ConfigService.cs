// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigService.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Config
{
    using Gorba.Common.Configuration.Core;

    /// <summary>
    /// The configuration service.
    /// </summary>
    public class ConfigService
    {
        private readonly ConfigManager<RendererConfig> configManager;
 
        static ConfigService()
        {
            Instance = new ConfigService();
        }

        private ConfigService()
        {
            this.configManager = new ConfigManager<RendererConfig>();
            this.configManager.FileName = "SharpDXRenderer.xml";
            this.configManager.EnableCaching = true;
        }

        /// <summary>
        /// Gets the single instance of this service.
        /// </summary>
        public static ConfigService Instance { get; private set; }

        /// <summary>
        /// Gets the renderer config.
        /// </summary>
        public RendererConfig Config
        {
            get
            {
                return this.configManager.Config;
            }
        }
    }
}
