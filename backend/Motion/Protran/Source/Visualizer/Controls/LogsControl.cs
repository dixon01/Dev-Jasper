// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogsControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogsControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Protran.Visualizer.Properties;

    /// <summary>
    /// Control that shows log information
    /// </summary>
    public partial class LogsControl : UserControl, IVisualizationControl
    {
        private readonly Queue<TelegramEvent> disabledQueue = new Queue<TelegramEvent>();

        private readonly Form detailsDialog = new Form();
        private readonly TextBox ximpleControl = new TextBox();

        private SideTab sideTab;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogsControl"/> class.
        /// </summary>
        public LogsControl()
        {
            this.InitializeComponent();

            this.ximpleControl.Font = new Font(FontFamily.GenericMonospace, 8);
            this.ximpleControl.ReadOnly = true;
            this.ximpleControl.Multiline = true;
            this.ximpleControl.WordWrap = false;
            this.ximpleControl.ScrollBars = ScrollBars.Vertical;
        }

        /// <summary>
        /// Type of log event.
        /// </summary>
        protected enum EventType
        {
            /// <summary>
            /// A telegram has arrived.
            /// </summary>
            TelegramIn,

            /// <summary>
            /// A telegram was (virtually or really) sent out.
            /// </summary>
            TelegramOut,

            /// <summary>
            /// A telegram was parsed.
            /// </summary>
            Parse,

            /// <summary>
            /// The contents of a telegram was transformed.
            /// </summary>
            Transform,

            /// <summary>
            /// A telegram was handled.
            /// </summary>
            Handle,

            /// <summary>
            /// Ximple data was sent out.
            /// </summary>
            Ximple
        }

        /// <summary>
        /// Assigns a <see cref="SideTab"/> to this control.
        /// This can be used to keep a reference to the tab
        /// and update it when events arrive.
        /// </summary>
        /// <param name="tab">
        /// The side tab.
        /// </param>
        public void SetSideTab(SideTab tab)
        {
            this.sideTab = tab;
        }

        /// <summary>
        /// Adds an event to the list (or to the queue if logging was disabled).
        /// This method is thread safe.
        /// </summary>
        /// <param name="type">
        /// The type of event.
        /// </param>
        /// <param name="description">
        /// The description of the event that will be shown in the list.
        /// </param>
        /// <param name="e">
        /// The event arguments that are associated to this event.
        /// </param>
        protected void AddEvent(EventType type, string description, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.AddEvent(type, description, e)));
                return;
            }

            var evnt = new TelegramEvent { Type = type, Description = description, EventArgs = e };
            if (this.checkBoxTelegramsEnabled.Checked)
            {
                this.AddEvent(evnt);
            }
            else
            {
                this.disabledQueue.Enqueue(evnt);
            }
        }

        /// <summary>
        /// Adds a Ximple event to the log.
        /// This method calls <see cref="AddEvent(LogsControl.EventType,string,System.EventArgs)"/>
        /// with the right arguments.
        /// </summary>
        /// <param name="e">
        /// The ximple event arguments.
        /// </param>
        protected void AddXimpleEvent(XimpleEventArgs e)
        {
            this.AddEvent(EventType.Ximple, string.Format("Ximple [count={0}]", e.Ximple.Cells.Count), e);
        }

        /// <summary>
        /// Subclasses have to provide a (reusable) control that can be shown
        /// when an item in the log list was double-clicked.
        /// The control has to be filled with the provided event information.
        /// Subclasses should always call the base method.
        /// </summary>
        /// <param name="timestamp">The timestamp of the event to represent.</param>
        /// <param name="type">The type of event to represent.</param>
        /// <param name="description">The description.</param>
        /// <param name="e">The event arguments.</param>
        /// <returns>a control (this method should not create a new control for each request) or null if the
        /// event can't be shown in a detail control.</returns>
        protected virtual Control GetDetailControl(DateTime timestamp, EventType type, string description, EventArgs e)
        {
            switch (type)
            {
                case EventType.Ximple:
                    this.ximpleControl.Text = ((XimpleEventArgs)e).Ximple.ToXmlString();
                    return this.ximpleControl;
                default:
                    return null;
            }
        }

        private void AddEvent(TelegramEvent evnt)
        {
            this.listBoxTelegramLog.Items.Add(evnt);
            this.listBoxTelegramLog.SelectedIndex = this.listBoxTelegramLog.Items.Count - 1;
            this.sideTab.Description = string.Format("{0} events", this.listBoxTelegramLog.Items.Count);
        }

        private void CheckBoxTelegramsEnabledCheckedChanged(object sender, EventArgs e)
        {
            if (!this.checkBoxTelegramsEnabled.Checked)
            {
                return;
            }

            this.listBoxTelegramLog.BeginUpdate();
            while (this.disabledQueue.Count > 0)
            {
                this.AddEvent(this.disabledQueue.Dequeue());
            }

            this.listBoxTelegramLog.EndUpdate();
        }

        private void ListBoxTelegramLogDoubleClick(object sender, EventArgs e)
        {
            var item = this.listBoxTelegramLog.SelectedItem as TelegramEvent;
            if (item == null)
            {
                return;
            }

            var control = this.GetDetailControl(item.Timestamp, item.Type, item.Description, item.EventArgs);
            if (control == null)
            {
                return;
            }

            control.Dock = DockStyle.Fill;
            this.detailsDialog.Controls.Clear();
            this.detailsDialog.Controls.Add(control);
            this.detailsDialog.StartPosition = FormStartPosition.CenterParent;
            this.detailsDialog.Text = item.Description;
            var image = Resources.ResourceManager.GetObject(item.Type.ToString()) as Bitmap;
            if (image != null)
            {
                this.detailsDialog.Icon = Icon.FromHandle(image.GetHicon());
            }

            var form = this.FindForm();
            if (form != null)
            {
                this.detailsDialog.Bounds = Rectangle.Inflate(form.Bounds, -50, -50);
            }

            this.detailsDialog.ShowDialog(this);
        }

        private void ListBoxTelegramLogDrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index >= this.listBoxTelegramLog.Items.Count)
            {
                return;
            }

            var item = this.listBoxTelegramLog.Items[e.Index] as TelegramEvent;
            if (item == null)
            {
                return;
            }

            var bounds = e.Bounds;
            var g = e.Graphics;
            var image = Resources.ResourceManager.GetObject(item.Type.ToString()) as Bitmap;
            if (image != null)
            {
                g.DrawImage(image, bounds.X, bounds.Y + 1);
            }

            bounds.Inflate(0, -2);
            bounds.X += 20;
            bounds.Width -= 20;
            g.DrawString(
                string.Format("{0:HH:mm:ss.fff}  {1}", item.Timestamp, item.Description),
                e.Font,
                new SolidBrush(e.ForeColor),
                bounds,
                StringFormat.GenericDefault);

            e.DrawFocusRectangle();
        }

        private class TelegramEvent
        {
            public TelegramEvent()
            {
                this.Timestamp = DateTime.Now;
            }

            public DateTime Timestamp { get; private set; }

            public EventType Type { get; set; }

            public string Description { get; set; }

            public EventArgs EventArgs { get; set; }

            public override string ToString()
            {
                return string.Format("{0}: {1}", this.Type, this.Description);
            }
        }
    }
}
