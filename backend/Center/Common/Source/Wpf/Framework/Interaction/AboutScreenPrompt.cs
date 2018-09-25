// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AboutScreenPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The about screen prompt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction
{
    using System.Deployment.Application;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows.Media;

    using Gorba.Center.Common.Wpf.Framework.Notifications;

    /// <summary>
    /// The about screen prompt.
    /// </summary>
    public class AboutScreenPrompt : PromptNotification
    {
        private ImageSource applicationIconSource;

        private string productName;

        private string copyRight;

        private string assemblyVersion;

        private bool showLuminatorAbout;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutScreenPrompt"/> class.
        /// </summary>
        public AboutScreenPrompt()
        {
            var assembly = Assembly.GetEntryAssembly();
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.AssemblyVersion = ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() : versionInfo.FileVersion;
            this.Copyright = versionInfo.LegalCopyright;
            this.ProductName = versionInfo.ProductName;
#if __UseLuminatorTftDisplay
            this.showLuminatorAbout = true;
            this.Copyright = @"© 2011-2017 Luminator Technology Group";
#else
            this.showLuminatorAbout = false;
#endif
        }

        /// <summary>
        /// Gets or sets the product name.
        /// </summary>
        public string ProductName
        {
            get
            {
                return this.productName;
            }

            set
            {
                this.SetProperty(ref this.productName, value, () => this.ProductName);
            }
        }

        /// <summary>
        /// Gets or sets the copyright text.
        /// </summary>
        public string Copyright
        {
            get
            {
                return this.copyRight;
            }

            set
            {
                this.SetProperty(ref this.copyRight, value, () => this.Copyright);
            }
        }

        /// <summary>
        /// Gets or sets the assembly version.
        /// </summary>
        public string AssemblyVersion
        {
            get
            {
                return this.assemblyVersion;
            }

            set
            {
                this.SetProperty(ref this.assemblyVersion, value, () => this.AssemblyVersion);
            }
        }

        /// <summary>
        /// Gets or sets the application icon source.
        /// </summary>
        public ImageSource ApplicationIconSource
        {
            get
            {
                return this.applicationIconSource;
            }

            set
            {
                this.SetProperty(ref this.applicationIconSource, value, () => this.ApplicationIconSource);
            }
        }

        /// <summary>
        /// Returns 'true' if the Luminator 'about' box should be used.
        /// </summary>
        public bool ShowLuminatorAbout
        {
            get
            {
                return this.showLuminatorAbout;
            }
        }

        /// <summary>
        /// Returns 'true' if the Gorba 'about' box should be used.
        /// </summary>
        public bool ShowGorbaAbout
        {
            get
            {
                return !this.showLuminatorAbout;
            }
        }
    }
}
