// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfigurationReader.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   reads information from unit configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    using Gorba.Center.Media.Core.DataViewModels.Compatibility;

    using NLog;

    /// <summary>
    /// The unit configuration reader.
    /// </summary>
    public static class UnitConfigurationReader
    {
        /// <summary>
        /// The get component version.
        /// </summary>
        /// <param name="xmlDoc">
        /// The xml doc.
        /// </param>
        /// <param name="logger">
        /// The logger to log unknown components
        /// </param>
        /// <returns>
        /// The version list.
        /// </returns>
        /// <exception>
        /// If passed an invalid XML an exception is thrown.
        /// </exception>
        public static List<FeatureComponentRequirements.SoftwareConfig> GetComponentVersions(
            string xmlDoc, Logger logger = null)
        {
            var versions = new List<FeatureComponentRequirements.SoftwareConfig>();
            var reader = XmlReader.Create(new StringReader(xmlDoc));

            while (reader.ReadToFollowing("Part"))
            {
                var keyValue = reader.GetAttribute("Key");

                if (keyValue == null || !keyValue.Equals("Conclusion.SoftwareVersions"))
                {
                    continue;
                }

                var versionreader = reader.ReadSubtree();

                while (versionreader.ReadToFollowing("Value"))
                {
                    var componentString = reader.GetAttribute("Key");
                    var versionString = reader.GetAttribute("Value");

                    if (string.IsNullOrEmpty(componentString) || string.IsNullOrEmpty(versionString))
                    {
                        throw new Exception("Invalid unit configuration.");
                    }

                    FeatureComponentRequirements.SoftwareComponent component;
                    try
                    {
                        component = GetComponentValue(componentString);
                    }
                    catch (Exception)
                    {
                        if (logger != null)
                        {
                            logger.Warn("Component '{0}' won't be checked for compatibility.", componentString);
                        }

                        continue;
                    }

                    versions.Add(new FeatureComponentRequirements.SoftwareConfig(
                            component,
                            new SoftwareComponentVersion(versionString)));
                }

                break;
            }

            return versions;
        }

        private static FeatureComponentRequirements.SoftwareComponent GetComponentValue(string componentString)
        {
            switch (componentString)
            {
                case "Gorba.Motion.Infomedia.AhdlcRenderer":
                    return FeatureComponentRequirements.SoftwareComponent.AhdlcRenderer;
                case "Gorba.Motion.Infomedia.AudioRenderer":
                    return FeatureComponentRequirements.SoftwareComponent.AudioRenderer;
                case "Gorba.Motion.Infomedia.Composer":
                    return FeatureComponentRequirements.SoftwareComponent.Composer;
                case "Gorba.Motion.Infomedia.DirectXRenderer":
                    return FeatureComponentRequirements.SoftwareComponent.DxRenderer;
                case "Gorba.Motion.HardwareManager":
                    return FeatureComponentRequirements.SoftwareComponent.HardwareManager;
                case "Gorba.Motion.Protran":
                    return FeatureComponentRequirements.SoftwareComponent.Protran;
                case "Gorba.Motion.SystemManager":
                    return FeatureComponentRequirements.SoftwareComponent.SystemManager;
                case "Gorba.Motion.Update":
                    return FeatureComponentRequirements.SoftwareComponent.Update;
                default:
                    throw new Exception("Unknown component.");
            }
        }
    }
}
