// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for App.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.WpfApplication
{
    using System;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;

    using Gorba.Center.Admin.Core;
    using Gorba.Center.Admin.Core.Controllers;
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Admin.Core.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Extensions;
    using Gorba.Center.Common.Wpf.Framework.ViewModels.Options;
    using Gorba.Center.Common.Wpf.Views;

    using NLog;

    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// More than one instance of the <see cref="T:System.Windows.Application"/> class is created
        /// per <see cref="T:System.AppDomain"/>.
        /// </exception>
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += this.OnUnhandledException;

            SetTheme();

            /*Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");*/
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            Logger.Info("Starting icenter.admin version: {0}", fileVersionInfo.FileVersion);

            var renderingTier = RenderCapability.Tier >> 16;
            Logger.Debug("Rendering tier: {0}", renderingTier);
            if (renderingTier == 0)
            {
                Logger.Debug("Setting render mode to SoftwareOnly.");
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            }

            Logger.Debug("Render mode: {0}", RenderOptions.ProcessRenderMode);

            var compositionBatch = new CompositionBatch();
            compositionBatch.TryExportApplicationStateManagerValue<AdminApplicationState, IAdminApplicationState>(
                "Admin", "AdminApplication");
            var bootstrapper = new AdminBootstrapper(
                compositionBatch,
                typeof(AdminShell).Assembly,
                typeof(ShellWindow).Assembly,
                typeof(CommandRegistry).Assembly);

            if (!bootstrapper.CheckOsRequirements())
            {
                return;
            }

            SetRenderMode();

            var applicationController = bootstrapper.Bootstrap<IAdminApplicationController, IAdminApplicationState>();
            foreach (var resourceDictionary in applicationController.ResourceDictionaries.OfType<ResourceDictionary>())
            {
                this.Resources.MergedDictionaries.Add(resourceDictionary);
            }

            CleanupState(applicationController.State);
            applicationController.Controller.ShutdownCompleted += this.HandleShutdownCompleted;
            applicationController.Controller.Run();
            Logger.Info("Application started.");
        }

        private static void SetTheme()
        {
            var colorWhite = Color.FromRgb(0xff, 0xff, 0xff);
            var colorBlack = Color.FromRgb(0x00, 0x00, 0x00);
            var colorDark = Color.FromRgb(0x35, 0x34, 0x3d);
            var colorBrighterDark = Color.FromRgb(0x50, 0x53, 0x5c);
            var colorHighlightedBrighterDark = Color.FromRgb(0x7f, 0x82, 0x83);
            var colorDarkForeground = Color.FromRgb(0x7f, 0x82, 0x83);
            var colorHighlight = Color.FromRgb(0xfd, 0xaf, 0x00);
            var colorHighlightHover = Color.FromRgb(0xf9, 0x9d, 0x1c);
            var colorMediumGray = Color.FromRgb(0xe6, 0xe7, 0xe8);
            var colorDarkerMediumGray = Color.FromRgb(0xdb, 0xdb, 0xdd);
            var colorDarkGray = Color.FromRgb(0xc3, 0xc6, 0xc8);
            var colorExtraDarkGray = Color.FromRgb(0xa7, 0xad, 0xaf);
            var colorLightGray = Color.FromRgb(0xf1, 0xf1, 0xf1);

            StyleManager.ApplicationTheme = new Windows8Theme();
            ////Windows8Palette.Palette.MainColor = colorWhite;
            Windows8Palette.Palette.AccentColor = colorHighlight;
            ////Windows8Palette.Palette.BasicColor = colorMediumGray;
            ////Windows8Palette.Palette.StrongColor = colorDarkGray;
            ////Windows8Palette.Palette.MarkerColor = colorDark;
            ////Windows8Palette.Palette.ValidationColor = Colors.Red;
        }

        private static void CleanupState(IAdminApplicationState state)
        {
        }

        private static void SetRenderMode()
        {
            var renderingTier = RenderCapability.Tier >> 16;
            Logger.Debug(
                "Rendering information: Admin Render mode '{0}', Rendering tier: '{1}'",
                RenderOptions.ProcessRenderMode,
                renderingTier);
            if (renderingTier == 0)
            {
                Logger.Debug("Setting render mode to SoftwareOnly.");
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            }
        }

        private void HandleShutdownCompleted(object sender, EventArgs eventArgs)
        {
            this.Shutdown();
        }
    }
}
