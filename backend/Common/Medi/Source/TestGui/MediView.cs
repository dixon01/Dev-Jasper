// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediView.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Subscription;
    using Gorba.Common.Medi.TestGui.Messages;

    using NLog;

    /// <summary>
    /// The MessageDispatcher GUI.
    /// </summary>
    public partial class MediView : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="MediView"/> class.
        /// </summary>
        public MediView()
        {
            this.InitializeComponent();

            this.AddMessageType<HelloWorld>();
            this.AddMessageType<FooBar>();
            ////this.AddMessageType<NewOne>();
        }

        private interface ISubscriptionHandler
        {
            Type Type { get; }

            void Subscribe();

            void Unsubscribe();
        }

        private void AddMessageType<T>()
            where T : class
        {
            this.checkedListBox1.Items.Add(new SubscriptionHandler<T>(this));
            this.comboBox1.Items.Add(new TypeInfo(typeof(T)));
        }

        private void CheckedListBox1ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var item = this.checkedListBox1.Items[e.Index] as ISubscriptionHandler;

            if (item == null)
            {
                return;
            }

            if (e.NewValue == CheckState.Checked)
            {
                item.Subscribe();
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                item.Unsubscribe();
            }
        }

        private void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            this.label1.Enabled = this.textBox2.Enabled = this.textBox3.Enabled = !this.checkBox1.Checked;
        }

        private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            var info = this.comboBox1.SelectedItem as TypeInfo;

            this.buttonSend.Enabled = info != null;
            this.buttonSchedule.Enabled = info != null;

            if (info != null)
            {
                this.propertyGrid1.SelectedObject = Activator.CreateInstance(info.Type);
            }
        }

        private void ButtonSendClick(object sender, EventArgs e)
        {
            var message = this.propertyGrid1.SelectedObject;
            if (message == null)
            {
                return;
            }

            if (this.checkBox1.Checked)
            {
                Logger.Debug("Broadcasting {0}", message);
                MessageDispatcher.Instance.Broadcast(message);
            }
            else
            {
                var dest = new MediAddress { Unit = this.textBox2.Text, Application = this.textBox3.Text };

                Logger.Debug("Sending to {1}: {0}", message, dest);
                MessageDispatcher.Instance.Send(dest, message);
            }
        }

        private void ButtonScheduleClick(object sender, EventArgs e)
        {
            var message = this.propertyGrid1.SelectedObject;
            if (message == null)
            {
                return;
            }

            var scheduling = new SchedulingForm { Message = message };
            if (!this.checkBox1.Checked)
            {
                scheduling.Destination = new MediAddress(this.textBox2.Text, this.textBox3.Text);
            }

            scheduling.Show();
        }

        private void TextBox4TextChanged(object sender, EventArgs e)
        {
            this.UpdateAddEnabled();
        }

        private void TextBox5TextChanged(object sender, EventArgs e)
        {
            this.UpdateAddEnabled();
        }

        private void UpdateAddEnabled()
        {
            this.buttonAddBroadcastSubscription.Enabled = !string.IsNullOrEmpty(this.textBox4.Text)
                                                          && !string.IsNullOrEmpty(this.textBox5.Text);
        }

        private void ButtonAddBroadcastSubscriptionClick(object sender, EventArgs e)
        {
            foreach (ISubscriptionHandler handler in this.checkedListBox1.CheckedItems)
            {
                var type = typeof(BroadcastSubscription<>).MakeGenericType(handler.Type);
                this.listBoxBroadcastSubscriptions.Items.Add(
                    Activator.CreateInstance(type, new MediAddress(this.textBox4.Text, this.textBox5.Text)));
            }
        }

        private void ButtonRemoveBroadcastSubscriptionClick(object sender, EventArgs e)
        {
            var disposable = this.listBoxBroadcastSubscriptions.SelectedItem as IDisposable;
            if (disposable == null)
            {
                return;
            }

            this.listBoxBroadcastSubscriptions.Items.Remove(disposable);
            disposable.Dispose();
        }

        private void ListBoxBroadcastSubscriptionsSelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonRemoveBroadcastSubscription.Enabled = this.listBoxBroadcastSubscriptions.SelectedItem != null;
        }

        private class BroadcastSubscription<T> : IDisposable
        {
            private readonly MediAddress mediAddress;

            private readonly IDisposable subscription;

            public BroadcastSubscription(MediAddress mediAddress)
            {
                this.mediAddress = mediAddress;
                this.subscription =
                    MessageDispatcher.Instance.GetService<IBroadcastSubscriptionService>()
                        .AddSubscription<T>(mediAddress);
            }

            public void Dispose()
            {
                this.subscription.Dispose();
            }

            public override string ToString()
            {
                return string.Format("{0} @ {1}", typeof(T).Name, this.mediAddress);
            }
        }

        private class SubscriptionHandler<T> : ISubscriptionHandler
            where T : class
        {
            private readonly MediView mediView;

            public SubscriptionHandler(MediView mediView)
            {
                this.mediView = mediView;
            }

            public Type Type
            {
                get
                {
                    return typeof(T);
                }
            }

            public void Subscribe()
            {
                MessageDispatcher.Instance.Subscribe<T>(this.HandleMessage);
            }

            public void Unsubscribe()
            {
                MessageDispatcher.Instance.Unsubscribe<T>(this.HandleMessage);
            }

            public override string ToString()
            {
                return typeof(T).Name;
            }

            private void HandleMessage(object sender, MessageEventArgs<T> e)
            {
                var message = string.Format(
                    "{0}  {1}->{2}:  {3}{4}",
                    DateTime.Now.ToLongTimeString(),
                    e.Source,
                    e.Destination,
                    e.Message,
                    Environment.NewLine);
                Logger.Debug("Received message: {0}", message);
                this.mediView.Invoke(
                    new EventHandler(
                        (s, ev) =>
                        this.mediView.textBox1.AppendText(message)));
            }
        }
    }
}
