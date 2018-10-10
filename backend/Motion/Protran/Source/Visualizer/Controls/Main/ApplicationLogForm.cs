// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationLogForm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationLogForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Main
{
    using System;
    using System.Drawing;

    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Win32.Wrapper;

    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using NLog.Targets.Wrappers;
    using NLog.Windows.Forms;

    /// <summary>
    /// The application log MDI child form.
    /// </summary>
    public partial class ApplicationLogForm : MdiChildBase
    {
        private LoggingRule loggingRule;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationLogForm"/> class.
        /// </summary>
        public ApplicationLogForm()
        {
            this.InitializeComponent();
            this.Icon = ShellFileInfo.GetFileIcon(ApplicationHelper.GetEntryAssemblyLocation(), false, false);
            this.comboBoxLogLevel.Items.Add(LogLevel.Fatal);
            this.comboBoxLogLevel.Items.Add(LogLevel.Error);
            this.comboBoxLogLevel.Items.Add(LogLevel.Warn);
            this.comboBoxLogLevel.Items.Add(LogLevel.Info);
            this.comboBoxLogLevel.Items.Add(LogLevel.Debug);
            this.comboBoxLogLevel.Items.Add(LogLevel.Trace);

            this.comboBoxLogLevel.SelectedItem = LogLevel.Info;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.ConfigureLogging();
        }

        private void ConfigureLogging()
        {
            if (this.loggingRule != null)
            {
                return;
            }

            var config = LogManager.Configuration ?? new LoggingConfiguration();

            var target = new RichTextBoxTarget
            {
                Layout = "${time} <${logger:shortName=true}> "
                 + "${message}${onexception: ${newline}${exception:format=tostring}}",
                Name = "textbox-logging",
                FormName = this.Name,
                ControlName = this.nlogTextBox.Name,
                AutoScroll = true
            };
            target.RowColoringRules.Add(
                new RichTextBoxRowColoringRule
                {
                    Condition = "level == LogLevel.Fatal",
                    FontColor = Color.DarkRed.Name
                });
            target.RowColoringRules.Add(
                new RichTextBoxRowColoringRule
                {
                    Condition = "level == LogLevel.Error",
                    FontColor = Color.Red.Name
                });
            target.RowColoringRules.Add(
                new RichTextBoxRowColoringRule
                {
                    Condition = "level == LogLevel.Warn",
                    FontColor = Color.Orange.Name
                });
            target.RowColoringRules.Add(
                new RichTextBoxRowColoringRule
                {
                    Condition = "level == LogLevel.Info",
                    FontColor = Color.White.Name
                });
            target.RowColoringRules.Add(
                new RichTextBoxRowColoringRule
                {
                    Condition = "level == LogLevel.Debug",
                    FontColor = Color.LightGray.Name
                });
            target.RowColoringRules.Add(
                new RichTextBoxRowColoringRule
                {
                    Condition = "level == LogLevel.Trace",
                    FontColor = Color.DimGray.Name
                });

            config.AddTarget(target.Name, target);
            var asyncTarget = new AsyncTargetWrapper(target);
            config.AddTarget("async-" + target.Name, asyncTarget);

            var level = (LogLevel)this.comboBoxLogLevel.SelectedItem;
            this.loggingRule = new LoggingRule("*", level, asyncTarget);
            config.LoggingRules.Add(this.loggingRule);

            if (LogManager.Configuration == null)
            {
                // NLog wasn't configured before, so we have configure it for the first time
                LogManager.Configuration = config;
            }
            else
            {
                // NLog was configured before, so we just reload it
                LogManager.Configuration.Reload();
            }
        }

        private void ComboBoxLogLevelOnSelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.loggingRule == null)
            {
                return;
            }

            bool enabled = true;
            foreach (LogLevel level in this.comboBoxLogLevel.Items)
            {
                if (enabled)
                {
                    this.loggingRule.EnableLoggingForLevel(level);
                }
                else
                {
                    this.loggingRule.DisableLoggingForLevel(level);
                }

                if (level.Equals(this.comboBoxLogLevel.SelectedItem))
                {
                    enabled = false;
                }
            }

            LogManager.Configuration.Reload();
            LogManager.ReconfigExistingLoggers();
        }
    }
}
