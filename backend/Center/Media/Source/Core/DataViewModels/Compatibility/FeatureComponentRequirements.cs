// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureComponentRequirements.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Compatibility
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The version feature compatibility.
    /// </summary>
    public static class FeatureComponentRequirements
    {
        /// <summary>
        /// The version feature lookup.
        /// </summary>
        public static readonly List<FeatureRequirements> VersionFeatureLookup =
            new List<FeatureRequirements>
                {
                    new FeatureRequirements(
                        Features.MultiFonts,
                        new List<SoftwareConfig>
                            {
                                new SoftwareConfig(
                                    SoftwareComponent.AhdlcRenderer,
                                    new SoftwareComponentVersion("2.5"))
                            }),
                    new FeatureRequirements(
                        Features.RssTickerElement,
                        new List<SoftwareConfig>
                            {
                                new SoftwareConfig(
                                    SoftwareComponent.Composer,
                                    new SoftwareComponentVersion("2.5")),
                                    new SoftwareConfig(
                                    SoftwareComponent.Protran,
                                    new SoftwareComponentVersion("2.5")),
                                    new SoftwareConfig(
                                        SoftwareComponent.DxRenderer,
                                        new SoftwareComponentVersion("2.5"))
                            }),
                    new FeatureRequirements(
                        Features.LiveStreamElement,
                        new List<SoftwareConfig>
                            {
                                new SoftwareConfig(
                                    SoftwareComponent.Composer,
                                    new SoftwareComponentVersion("2.5")),
                                new SoftwareConfig(
                                        SoftwareComponent.DxRenderer,
                                        new SoftwareComponentVersion("2.5"))
                            }),
                    new FeatureRequirements(
                        Features.SpecialFonts,
                        new List<SoftwareConfig>
                            {
                                new SoftwareConfig(
                                    SoftwareComponent.Composer,
                                    new SoftwareComponentVersion("2.5")),
                                new SoftwareConfig(
                                    SoftwareComponent.AhdlcRenderer,
                                    new SoftwareComponentVersion("2.5")),
                            }),
                    new FeatureRequirements(
                        Features.RingScroll,
                        new List<SoftwareConfig>
                            {
                                new SoftwareConfig(
                                    SoftwareComponent.Composer,
                                    new SoftwareComponentVersion("2.5")),
                                new SoftwareConfig(
                                    SoftwareComponent.DxRenderer,
                                    new SoftwareComponentVersion("2.5")),
                                new SoftwareConfig(
                                    SoftwareComponent.AhdlcRenderer,
                                    new SoftwareComponentVersion("2.5")),
                            }),
                };

        /// <summary>
        /// The software components which are version checked.
        /// </summary>
        public enum SoftwareComponent
        {
            /// <summary>
            /// The background system.
            /// </summary>
            BackgroundSystem,

            /// <summary>
            /// The protran.
            /// </summary>
            Protran,

            /// <summary>
            /// The composer.
            /// </summary>
            Composer,

            /// <summary>
            /// The AHDLC renderer.
            /// </summary>
            AhdlcRenderer,

            /// <summary>
            /// The dx renderer.
            /// </summary>
            DxRenderer,

            /// <summary>
            /// The audio renderer.
            /// </summary>
            AudioRenderer,

            /// <summary>
            /// The hardware manager.
            /// </summary>
            HardwareManager,

            /// <summary>
            /// The system manager.
            /// </summary>
            SystemManager,

            /// <summary>
            /// The update.
            /// </summary>
            Update
        }

        /// <summary>
        /// The features which are subject to version checks.
        /// </summary>
        public enum Features
        {
            /// <summary>
            /// The multi fonts.
            /// </summary>
            MultiFonts,

            /// <summary>
            /// The RSS ticker element.
            /// </summary>
            RssTickerElement,

            /// <summary>
            /// The live stream element.
            /// </summary>
            LiveStreamElement,

            /// <summary>
            /// The cux fonts.
            /// </summary>
            SpecialFonts,

            /// <summary>
            /// The ring scroll.
            /// </summary>
            RingScroll,

            /// <summary>
            /// The code conversion formula.
            /// </summary>
            CodeConversion
        }

        /// <summary>
        /// The get feature name.
        /// </summary>
        /// <param name="feature">
        /// The feature.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetFeatureName(Features feature)
        {
            switch (feature)
            {
                case Features.MultiFonts:
                    return "multi font selection";
                case Features.RssTickerElement:
                    return "RSS ticker element";
                case Features.LiveStreamElement:
                    return "live stream element";
                case Features.SpecialFonts:
                    return "special fonts";
                case Features.RingScroll:
                    return "ring scroll";
                case Features.CodeConversion:
                    return "code conversion";
                default:
                    return feature.ToString();
            }
        }

        /// <summary>
        /// The merges the featuresRequirements into the requirements list. Always the biggest version is kept.
        /// </summary>
        /// <param name="requirements">
        /// The requirements.
        /// </param>
        /// <param name="featureRequirements">
        /// The feature requirements.
        /// </param>
        public static void MergeRequirements(
            List<SoftwareConfig> requirements,
            List<SoftwareConfig> featureRequirements)
        {
            foreach (var required in featureRequirements)
            {
                var current = requirements.FirstOrDefault(r => r.Component == required.Component);

                if (current == null)
                {
                    requirements.Add(required);
                    continue;
                }

                if (current.Version < required.Version)
                {
                    requirements.Remove(current);
                    requirements.Add(required);
                }
            }
        }

        /// <summary>
        /// The get lowest versions.
        /// </summary>
        /// <param name="softwareConfigsList">
        /// The list of software config list.
        /// </param>
        /// <returns>
        /// The lowest version for all components.
        /// </returns>
        public static List<SoftwareConfig> GetLowestVersions(IEnumerable<List<SoftwareConfig>> softwareConfigsList)
        {
            var result = new List<SoftwareConfig>();

            foreach (var configsList in softwareConfigsList)
            {
                foreach (var softwareConfig in configsList)
                {
                    var current = result.FirstOrDefault(c => c.Component == softwareConfig.Component);

                    if (current == null)
                    {
                        result.Add(softwareConfig);
                        continue;
                    }

                    if (current.Version > softwareConfig.Version)
                    {
                        result.Remove(current);
                        result.Add(softwareConfig);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The requirements ok.
        /// </summary>
        /// <param name="available">
        /// The available.
        /// </param>
        /// <param name="requirements">
        /// The requirements.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool RequirementsOk(
            List<SoftwareConfig> available,
            List<SoftwareConfig> requirements)
        {
            foreach (var required in requirements)
            {
                var availableVersion = available.FirstOrDefault(a => a.Component == required.Component);

                // missing component
                if (availableVersion == null)
                {
                    return false;
                }

                // version not meet
                if (availableVersion.Version < required.Version)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// The feature requirements.
        /// Lists all packages and their minimum version which are required by a feature.
        /// </summary>
        public class FeatureRequirements
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FeatureRequirements"/> class.
            /// </summary>
            /// <param name="feature">
            /// The feature.
            /// </param>
            /// <param name="requirements">
            /// The requirements.
            /// </param>
            public FeatureRequirements(Features feature, List<SoftwareConfig> requirements)
            {
                this.Feature = feature;
                this.PackageConfig = requirements;
            }

            /// <summary>
            /// Gets or sets the feature name.
            /// </summary>
            public Features Feature { get; set; }

            /// <summary>
            /// Gets or sets the package config.
            /// </summary>
            public List<SoftwareConfig> PackageConfig { get; set; }
        }

        /// <summary>
        /// The package config.
        /// A specific software package identified by its name and version.
        /// </summary>
        public class SoftwareConfig
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SoftwareConfig"/> class.
            /// </summary>
            /// <param name="component">
            /// The component.
            /// </param>
            /// <param name="version">
            /// The version.
            /// </param>
            public SoftwareConfig(SoftwareComponent component, SoftwareComponentVersion version)
            {
                this.Version = version;
                this.Component = component;
            }

            /// <summary>
            /// Gets or sets the version.
            /// </summary>
            public SoftwareComponentVersion Version { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public SoftwareComponent Component { get; set; }

            /// <summary>
            /// The get component name.
            /// </summary>
            /// <returns>
            /// The <see cref="object"/>.
            /// </returns>
            public object GetComponentName()
            {
                switch (this.Component)
                {
                    case SoftwareComponent.BackgroundSystem:
                        return "Background System";
                    case SoftwareComponent.Protran:
                        return "Protran";
                    case SoftwareComponent.Composer:
                        return "Composer";
                    case SoftwareComponent.AhdlcRenderer:
                        return "AHDLC Renderer";
                    case SoftwareComponent.DxRenderer:
                        return "DirectX Renderer";
                    case SoftwareComponent.AudioRenderer:
                        return "Audio Renderer";
                    case SoftwareComponent.HardwareManager:
                        return "Hardware Manager";
                    case SoftwareComponent.SystemManager:
                        return "System Manager";
                    case SoftwareComponent.Update:
                        return "Update";
                    default:
                        return this.Component.ToString();
                }
            }
        }
    }
}
