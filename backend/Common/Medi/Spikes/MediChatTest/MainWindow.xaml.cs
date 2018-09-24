// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.MediChatTest
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;

    using NLog;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// The user id.
        /// </summary>
        private readonly string userId;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            var rnd = new Random();
            this.userId = "ID" + rnd.Next().ToString();

            this.Title = "Medi Chat " + this.userId;
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ConfigFileNameTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            this.EstablishMediConnection();
        }

        private void ConfigureButtonClick(object sender, RoutedEventArgs e)
        {
            this.EstablishMediConnection();
        }

        private void MessageToSendTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            this.SendMessage();
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            this.SendMessage();
        }

        private void SendMessage()
        {
            if (MessageDispatcher.Instance == null)
            {
                MessageBox.Show("Please connect before sending a text", "Connection missing");
                return;
            }

            if (this.MessageToSendTextBox.Text == string.Empty)
            {
                return;
            }

            var msg = new StringMessage(this.MessageToSendTextBox.Text, this.userId);

            MessageDispatcher.Instance.Broadcast(msg);
            this.MessageToSendTextBox.Clear();
            Thread.Sleep(10);
        }

        /// <summary>
        /// The establish medi connection.
        /// </summary>
        private void EstablishMediConnection()
        {
            if (!File.Exists(this.ConfigFileNameTextBox.Text))
            {
                MessageBox.Show(this.ConfigFileNameTextBox.Text + " doesn't exist");
            }

            MessageDispatcher.Instance.Configure(new FileConfigurator(this.ConfigFileNameTextBox.Text));
            MessageDispatcher.Instance.Subscribe<StringMessage>(this.ChatMessageHandler);
            Thread.Sleep(1000);
        }

        private void ChatMessageHandler(object sender, MessageEventArgs<StringMessage> e)
        {
            Application.Current.Dispatcher.Invoke(() =>
                this.SentAndReceivedMessagesListBox.Items.Add(e.Message.UserId + " " + e.Message.Value));
        }
    }
}