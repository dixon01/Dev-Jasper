// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressDialog.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextInputDialog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// Dialog that prompts the user to input a string.
    /// </summary>
    public partial class ProgressDialog : Form
    {
        private const int MaxProgressValue = 10000;
        private IProgressTask task;

        private bool finished;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressDialog"/> class.
        /// </summary>
        public ProgressDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the label shown above the text input field.
        /// </summary>
        [DefaultValue("")]
        public string Label
        {
            get
            {
                return this.label.Text;
            }

            set
            {
                this.label.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the label shown right above the progress bar.
        /// </summary>
        [DefaultValue("")]
        public string ProgressLabel
        {
            get
            {
                return this.labelProgress.Text;
            }

            set
            {
                this.labelProgress.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the task to be executed in this dialog.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IProgressTask Task
        {
            get
            {
                return this.task;
            }

            set
            {
                if (this.task != null)
                {
                    this.task.ValueChanged -= this.TaskOnValueChanged;
                    this.task.StateChanged -= this.TaskOnStateChanged;
                }

                this.task = value;

                if (this.task != null)
                {
                    this.progressBar.Minimum = 0;
                    this.progressBar.Maximum = MaxProgressValue;
                    this.SetValue(value.Value);
                    this.labelProgress.Text = this.Task.State;
                    this.task.ValueChanged += this.TaskOnValueChanged;
                    this.task.StateChanged += this.TaskOnStateChanged;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.Task == null)
            {
                throw new NotSupportedException("Task is required to run a progress dialog");
            }

            ThreadPool.QueueUserWorkItem(this.Run);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Closed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnClosed(EventArgs e)
        {
            if (!this.finished)
            {
                this.finished = true;
                this.Task.Cancel();
            }

            base.OnClosed(e);
        }

        private void Run(object state)
        {
            try
            {
                this.Task.Run();
                if (this.finished)
                {
                    return;
                }

                this.finished = true;
                this.Invoke(new MethodInvoker(() => this.DialogResult = DialogResult.OK));
            }
            catch (Exception ex)
            {
                if (this.finished)
                {
                    return;
                }

                this.finished = true;
                this.Invoke(
                    new MethodInvoker(
                        () =>
                            {
                                MessageBox.Show(
                                    this, ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.DialogResult = DialogResult.Cancel;
                            }));
            }
        }

        private void SetValue(double value)
        {
            var intValue = (int)(value * MaxProgressValue);
            intValue = Math.Min(Math.Max(intValue, 0), MaxProgressValue);
            this.progressBar.Value = intValue;
        }

        private void TaskOnValueChanged(object sender, EventArgs eventArgs)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(this.TaskOnValueChanged));
                return;
            }

            this.SetValue(this.Task.Value);
        }

        private void TaskOnStateChanged(object sender, EventArgs eventArgs)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(this.TaskOnStateChanged));
                return;
            }

            this.labelProgress.Text = this.Task.State;
        }
    }
}
