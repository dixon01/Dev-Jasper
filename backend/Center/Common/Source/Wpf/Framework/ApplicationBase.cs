// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The application base.
//   It provides common functions used by all Center WPF applications.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework
{
    using System;
    using System.IO;
    using System.Windows;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;

    using NLog;
    using NLog.Targets;
    using NLog.Targets.Wrappers;

    using Telerik.Windows.Controls;

    /// <summary>
    /// The application base.
    /// It provides common functions used by all Center WPF applications.
    /// </summary>
    public abstract class ApplicationBase : Application
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// If not overwritten shows a message box with the unhandled exception and a link to the log directory.
        /// After the message box has been closed, the application exits without the default "crash" dialog.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="unhandledExceptionEventArgs">
        /// The unhandled exception event args.
        /// </param>
        protected virtual void OnUnhandledException(
            object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var ex = unhandledExceptionEventArgs.ExceptionObject as Exception;
            Logger.FatalException("Unhandled exception", ex);
            LogManager.Flush();
            var directory = GetLogFileDirectory();
            Uri logUri = null;
            if (!string.IsNullOrEmpty(directory))
            {
                logUri = new Uri(directory, UriKind.Absolute);
            }

            var alertContent = new ErrorDialogContent(
                FrameworkStrings.Application_UnhandledExceptionMessage,
                logUri,
                ex,
                FrameworkStrings.Application_UnhandledExceptionHyperlink);
            var dialogParameters = new DialogParameters
            {
                Content = alertContent,
                ContentStyle = this.Resources["ModalErrorContentStyle"] as Style,
                Header = FrameworkStrings.Application_UnhandledExceptionTitle,
                WindowStyle = this.Resources["RadWindowDialogStyle"] as Style,
                Owner = this.MainWindow,
                DialogStartupLocation = WindowStartupLocation.CenterOwner
            };
            RadWindow.Alert(dialogParameters);
            Environment.Exit(1);
        }

        private static string GetLogFileDirectory()
        {
            if (LogManager.Configuration == null || LogManager.Configuration.ConfiguredNamedTargets.Count == 0)
            {
                return null;
            }

            var target = LogManager.Configuration.FindTargetByName("file");
            if (target == null)
            {
                return null;
            }

            FileTarget fileTarget;
            var wrapperTarget = target as WrapperTargetBase;

            if (wrapperTarget == null)
            {
                fileTarget = target as FileTarget;
            }
            else
            {
                fileTarget = wrapperTarget.WrappedTarget as FileTarget;
            }

            if (fileTarget == null)
            {
                return null;
            }

            var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
            var path = fileTarget.FileName.Render(logEventInfo);
            return !File.Exists(path) ? null : Path.GetDirectoryName(path);
        }
    }
}
