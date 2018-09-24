// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerShellApplication.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerShellApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ShellApp
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;

    using CommandLineParser;

    using Gorba.Common.SystemManagement.Host;
    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Motion.SystemManager.Core;

    using Microsoft.Win32;

    using NLog;

    /// <summary>
    /// The system manager shell application.
    /// </summary>
    internal class SystemManagerShellApplication : IRunnableApplication
    {
        private const string WinLogonKey = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\WinLogon";

        private const string ShellKeyName = "Shell";

        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly SystemManagerApplication application;

        private ApplicationContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemManagerShellApplication"/> class.
        /// </summary>
        public SystemManagerShellApplication()
        {
            this.application = new SystemManagerApplication();
        }

        /// <summary>
        /// Configures this application with the given name.
        /// The application should use the given name to register itself with the System Manager Client.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void Configure(string name)
        {
        }

        /// <summary>
        /// Start is not supported.
        /// </summary>
        public void Start()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Stops this application.
        /// </summary>
        public void Stop()
        {
            this.Exit("Stop called");
        }

        /// <summary>
        /// Runs this application.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        public void Run(string[] args)
        {
            var options = new SystemManagerShellOptions();
            var parser = new CommandLineParser();
            parser.ExtractArgumentAttributes(options);

            try
            {
                parser.ParseCommandLine(args);
            }
            catch (Exception ex)
            {
                this.logger.Warn(ex, "Couldn't read command line arguments");
                var writer = new StringWriter();
                writer.WriteLine("Couldn't read command line arguments, application will exit.");
                writer.WriteLine(ex.Message);
                parser.PrintUsage(writer);
                MessageBox.Show(
                    writer.ToString(),
                    "System Manager Shell Command Line Options",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (options.ShowUsage)
            {
                var writer = new StringWriter();
                parser.PrintUsage(writer);
                MessageBox.Show(
                    writer.ToString(),
                    "System Manager Shell Command Line Options",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            this.CheckRegistry(options);

            if (options.InstallShell || options.UninstallShell)
            {
                return;
            }

            if (!this.application.ShouldStart(options))
            {
                return;
            }

            var mainForm = new MainForm(this.application);
            this.context = new ApplicationContext(mainForm);
            mainForm.Load += this.MainFormOnLoad;

            this.application.Configure(options);

            Application.Run(this.context);
            this.logger.Debug("Application loop exited");

            this.Exit("Shell form closed");
            LogManager.Flush();

            this.application.Dispose();
        }

        /// <summary>
        /// Asks the System Manager to re-launch this application with the given reason.
        /// If this application was not registered with the System Manager, it will simply stop.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void Relaunch(string reason)
        {
            this.application.Controller.RequestReboot(reason);
        }

        /// <summary>
        /// Asks the System Manager to exit this application with the given reason.
        /// If this application was not registered with the System Manager, it will simply stop.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void Exit(string reason)
        {
            this.application.Controller.Stop(true, ApplicationReason.ApplicationExit, reason);
        }

        private void CheckRegistry(SystemManagerShellOptions options)
        {
            try
            {
                var path = Assembly.GetExecutingAssembly().Location;

                var key = Registry.CurrentUser.OpenSubKey(WinLogonKey, true);
                if (key != null)
                {
                    var value = key.GetValue(ShellKeyName) as string;
                    this.logger.Debug("Current value of HKCU\\{0}\\{1} is '{2}'", WinLogonKey, ShellKeyName, value);

                    if (path.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (options.UninstallShell)
                        {
                            key.SetValue(ShellKeyName, string.Empty, RegistryValueKind.String);
                        }

                        return;
                    }

                    if (!options.VerifyShell && !options.InstallShell)
                    {
                        return;
                    }

                    if (!options.InstallShell
                        && MessageBox.Show(
                            "Shell of the current user is not set to System Manager, would you like to do this now?",
                            "System Manager Shell",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button2) == DialogResult.No)
                    {
                        return;
                    }

                    key.SetValue(ShellKeyName, path, RegistryValueKind.String);
                    this.logger.Info("Set HKCU\\{0}\\{1} to '{2}'", WinLogonKey, ShellKeyName, path);
                }
            }
            catch (Exception ex)
            {
                this.logger.Warn(ex, "Couldn't check registry key");
            }
        }

        private void MainFormOnLoad(object s, EventArgs e)
        {
            var controllerThread = new Thread(this.RunApplication);
            controllerThread.IsBackground = false;
            controllerThread.Start();
        }

        private void RunApplication()
        {
            this.application.Run(this.context);
            Application.Exit();
        }
    }
}