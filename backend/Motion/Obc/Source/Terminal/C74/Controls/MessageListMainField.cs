// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageListMainField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageListMainField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System;
    using System.Collections.Generic;

    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The message list main field.
    /// </summary>
    public partial class MessageListMainField : MainField, IMessageList
    {
        private readonly ScrollableListModel<MessageListEntry> model;

        private readonly ButtonInput[] buttons;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageListMainField"/> class.
        /// </summary>
        public MessageListMainField()
        {
            this.InitializeComponent();

            this.buttons = new[]
                               {
                                   this.buttonInput1, this.buttonInput2, this.buttonInput3, this.buttonInput4,
                                   this.buttonInput5
                               };
            this.model = new ScrollableListModel<MessageListEntry>(this.buttons.Length);
            this.model.SelectedIndexChanged += this.ModelOnSelectedIndexChanged;
        }

        /// <summary>
        ///   Enter pressed
        /// </summary>
        public event EventHandler<IndexEventArgs> SelectedIndexChanged;

        /// <summary>
        ///   List for TC
        /// </summary>
        /// <param name = "caption">List caption/header</param>
        /// <param name = "items">Items to be displayed</param>
        public void Init(string caption, List<MessageListEntry> items)
        {
            this.Init(caption, items, 0);
        }

        /// <summary>
        ///   List for TC
        /// </summary>
        /// <param name = "caption">List caption/header</param>
        /// <param name = "items">Items to be displayed</param>
        /// <param name="focusIndex">Index of the focused element in the items</param>
        public void Init(string caption, List<MessageListEntry> items, int focusIndex)
        {
            if (this.InvokeRequired)
            {
                this.SafeBeginInvoke(() => this.Init(caption, items, focusIndex));
                return;
            }

            this.Init();

            this.labelCaption.Text = caption;

            this.model.Fill(items);
            this.model.SelectedIndex = focusIndex;
        }

        private static string GetTypeString(MessageType iconType)
        {
            // TODO: later this should be an icon, I just simplified it to show something
            switch (iconType)
            {
                case MessageType.Info:
                    return "i";
                case MessageType.Instruction:
                    return "?";
                case MessageType.Error:
                    return "!";
                case MessageType.Alarm:
                    return "X";
                default:
                    return string.Empty;
            }
        }

        private void ModelOnSelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i < this.buttons.Length; i++)
            {
                var item = this.model.GetVisibleItem(i);
                if (item != null)
                {
                    this.buttons[i].Text = string.Format(
                        "{0}         {1}\n{2}",
                        GetTypeString(item.IconType),
                        item.Date.ToString("HH:mm"),
                        item.Message);
                    this.buttons[i].Visible = true;
                    this.buttons[i].IsSelected = i == this.model.SelectedIndex - this.model.Offset;
                }
                else
                {
                    this.buttons[i].Visible = false;
                    this.buttons[i].IsSelected = false;
                }
            }
        }
    }
}
