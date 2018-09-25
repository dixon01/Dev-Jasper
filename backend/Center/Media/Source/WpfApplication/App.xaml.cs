// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for App.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.WpfApplication
{
    using System;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Extensions;
    using Gorba.Center.Common.Wpf.Views;
    using Gorba.Center.Media.Core;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Options;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.WpfApplication.Properties;

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
            if (Settings.Default.EnableCustomUndhandledExceptionDialog)
            {
                AppDomain.CurrentDomain.UnhandledException += this.OnUnhandledException;
            }

            SetTheme();
            /*Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");*/
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            Logger.Info("Starting icenter.media version: {0}", fileVersionInfo.FileVersion);

            var compositionBatch = new CompositionBatch();
            var knownTypes = new[]
                                 {
                                     typeof(LocalResourceOptionCategory), typeof(LocalResourceOptionGroup),
                                     typeof(RendererOptionGroup), typeof(RendererOptionCategory)
                                 };
            compositionBatch.TryExportApplicationStateManagerValue<MediaApplicationState, IMediaApplicationState>(
                "Media", "MediaApplication", knownTypes);
            var bootstrapper = new MediaBootstrapper(
                compositionBatch,
                typeof(MediaShell).Assembly,
                typeof(ShellWindow).Assembly,
                typeof(CommandRegistry).Assembly);

            if (!bootstrapper.CheckOsRequirements())
            {
                return;
            }

            SetRenderMode();

            var applicationController =
                bootstrapper.Bootstrap<IMediaApplicationController, IMediaApplicationState>();
            foreach (var resourceDictionary in applicationController.ResourceDictionaries.OfType<ResourceDictionary>())
            {
                this.Resources.MergedDictionaries.Add(resourceDictionary);
            }

            ////CleanupState(applicationController.State);
            applicationController.Controller.ShutdownCompleted += this.HandleShutdownCompleted;
            applicationController.Controller.Run();
            Logger.Info("Application started.");
        }

        private static void CleanupState(IMediaApplicationState state)
        {
            try
            {
                var recentProjectIds =
                    state.RecentProjects.Select(
                        (model, i) =>
                        new
                            {
                                Index = i, model.ProjectId, model.FilePath, Exists = File.Exists(model.FilePath)
                            })
                         .OrderByDescending(arg => arg.Index)
                         .ToList();
                recentProjectIds.Where(p => !p.Exists).ToList().ForEach(
                    i => state.RecentProjects.RemoveAt(i.Index));

                var recentMediaResourcesProjectIds =
                    state.RecentMediaResources.Keys.Where(
                        id => recentProjectIds.Where(p => p.Exists).All(p => p.ProjectId != id)).ToList();
                recentMediaResourcesProjectIds.ForEach(id => state.RecentMediaResources.Remove(id));
            }
            catch (Exception exception)
            {
                Logger.Error("Error while cleaning up the state {0}", exception.Message);
            }
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

        private static void SetTheme()
        {
            var colorHighlight = Color.FromRgb(0xfd, 0xaf, 0x00);
            StyleManager.ApplicationTheme = new Windows8Theme();
            Windows8Palette.Palette.AccentColor = colorHighlight;
        }

        private void HandleShutdownCompleted(object sender, EventArgs eventArgs)
        {
            this.Shutdown();
        }
    }
}