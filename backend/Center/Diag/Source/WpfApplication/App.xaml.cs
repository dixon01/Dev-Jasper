// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The App.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.WpfApplication
{
    using System;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Extensions;
    using Gorba.Center.Common.Wpf.Views;
    using Gorba.Center.Diag.Core;
    using Gorba.Center.Diag.Core.Controllers;
    using Gorba.Center.Diag.Core.Models;
    using Gorba.Center.Diag.Core.ViewModels;

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
            Logger.Info("Starting icenter.diag version: {0}", fileVersionInfo.FileVersion);

            var compositionBatch = new CompositionBatch();
            compositionBatch.TryExportApplicationStateManagerValue<DiagApplicationState, IDiagApplicationState>(
                "Diag", "DiagApplication");
            var bootstrapper = new DiagBootstrapper(
                compositionBatch,
                typeof(DiagShell).Assembly,
                typeof(ShellWindow).Assembly,
                typeof(CommandRegistry).Assembly);

            if (!bootstrapper.CheckOsRequirements())
            {
                return;
            }

            SetRenderMode();

            var applicationController = bootstrapper.Bootstrap<IDiagApplicationController, IDiagApplicationState>();
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

        private static void CleanupState(IDiagApplicationState state)
        {
        }

        private static void SetRenderMode()
        {
            var renderingTier = RenderCapability.Tier >> 16;
            Logger.Debug(
                "Rendering information: Render mode '{0}', Rendering tier: '{1}'",
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
