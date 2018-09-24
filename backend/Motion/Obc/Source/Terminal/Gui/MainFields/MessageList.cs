// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageList.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Motion.Obc.Terminal.Core;
    using Gorba.Motion.Obc.Terminal.Gui.Control;
    using Gorba.Motion.Obc.Terminal.Gui.Utility;

    /// <summary>
    /// The message list main field.
    /// </summary>
    public partial class MessageList : MainField, IMessageList
    {
        private bool allowEscape = true;

        private string caption = "List Caption";

        private bool hasElements;

        private int initialSelection;

        private bool isInitialized;

        private List<MessageListEntry> items;

        private int lastFocusPos; // used for the Enabler interface..

        private int listLength;

        private int listPosition;

        private int maxListLength = 5;

        private List<CustomMenuItem> menuItems;

        private List<PictureBox> menuItemsPictures;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageList"/> class.
        /// </summary>
        public MessageList()
        {
            this.InitializeComponent();
            this.hasElements = false;
        }

        /*public void Init(string caption, string errorDescription)
        {
            this.errorDescription = errorDescription;
            Init(caption, new List<string>(), false, false);
        }*/

        /// <summary>
        ///   Enter pressed
        /// </summary>
        public event EventHandler<IndexEventArgs> SelectedIndexChanged;

        /// <summary>
        ///   List for TC
        /// </summary>
        /// <param name = "captionString">List caption/header</param>
        /// <param name = "itemList">Items to be displayed</param>
        public void Init(string captionString, List<MessageListEntry> itemList)
        {
            this.Init(captionString, itemList, 0);
        }

        /// <summary>
        ///   List for TC
        /// </summary>
        /// <param name = "captionString">List caption/header</param>
        /// <param name = "itemList">Items to be displayed</param>
        /// <param name="focusIndex">Index of the focused element in the items</param>
        public void Init(string captionString, List<MessageListEntry> itemList, int focusIndex)
        {
            this.caption = captionString;
            this.items = itemList;
            this.allowEscape = true;
            if (itemList.Count > focusIndex)
            {
                this.initialSelection = focusIndex;
            }

            this.SafeInvoke(this.RealInit);
        }

        /// <summary>
        ///   Hides the message box
        /// </summary>
        public override void HideMessageBox()
        {
            base.HideMessageBox();
            if (this.menuItems != null)
            {
                this.SafeBeginInvoke(() => this.menuItems[this.lastFocusPos].Focus());
            }
        }

        /// <summary>
        ///   Hides the progress bar
        /// </summary>
        public override void HideProgressBar()
        {
            base.HideProgressBar();
            if (this.menuItems != null)
            {
                this.SafeBeginInvoke(() => this.menuItems[this.lastFocusPos].Focus());
            }
        }

        /// <summary>
        /// Raises the <see cref="MainField.EscapePressed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnEscapePressed(EventArgs e)
        {
            if (this.Visible && this.allowEscape)
            {
                base.OnEscapePressed(e);
            }
        }

        /// <summary>
        /// Raises the <see cref="MainField.ReturnPressed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnReturnPressed(EventArgs e)
        {
            // do not call base class: base.OnReturnPressed(e);
            this.HandleReturn();
        }

        /// <summary>
        /// Raises the <see cref="MainField.UpPressed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnUpPressed(EventArgs e)
        {
            base.OnUpPressed(e);
            this.HandleUp();
        }

        /// <summary>
        /// Raises the <see cref="MainField.DownPressed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnDownPressed(EventArgs e)
        {
            base.OnDownPressed(e);
            this.HandleDown();
        }

        private void RealInit()
        {
            this.Controls.Clear();
            this.InitializeComponent();

            ScreenUtil.Adapt4Ihmi(this, true, true);

            this.lblCaption.Text = this.caption;
            this.listPosition = 0;
            if (this.items.Count > 0)
            {
                this.hasElements = true;
                this.menuItems = new List<CustomMenuItem>
                                     {
                                         this.menuItem1,
                                         this.menuItem2,
                                         this.menuItem3,
                                         this.menuItem4,
                                         this.menuItem5
                                     };

                this.menuItemsPictures = new List<PictureBox>
                                             {
                                                 this.pictureBox1,
                                                 this.pictureBox2,
                                                 this.pictureBox3,
                                                 this.pictureBox4,
                                                 this.pictureBox5
                                             };

                foreach (var mi in this.menuItems)
                {
                    mi.PaintBorder = false;
                    mi.BorderColorFocused = SystemColors.ControlDarkDark;
                }

                this.maxListLength = this.menuItems.Count;
                int tmpEntries = 6;
                if (this.items.Count < 6)
                {
                    tmpEntries = this.items.Count;
                }

                this.picDown.Location = new Point(
                    this.picDown.Location.X,
                    (this.menuItem2.Location.Y - this.menuItem1.Location.Y) * tmpEntries);
                this.listLength = this.items.Count;

                if (this.listLength < this.maxListLength)
                {
                    this.menuItems.RemoveRange(this.items.Count, this.maxListLength - this.listLength);
                    this.menuItemsPictures.RemoveRange(this.items.Count, this.maxListLength - this.listLength);
                    this.maxListLength = this.listLength;
                }

                this.FillUpItems();
                this.menuItem6.Focus();
                this.menuItem5.Focus();
                this.menuItem4.Focus();
                this.menuItem3.Focus();
                this.menuItem2.Focus();
                this.menuItem1.Focus();

                for (int i = 0; i < this.initialSelection; i++)
                {
                    this.HandleDown();
                }

                foreach (var menuItem in this.menuItems)
                {
                    menuItem.SetMultiline(true);
                }
            }
            else
            {
                this.hasElements = false;
            }

            this.SetPicUpDownVisibility();
        }

        private void FillUpItems()
        {
            for (int i = 0; this.menuItems.Count > i; i++)
            {
                this.menuItems[i].Text = string.Format(
                    "          {0}\n{1}",
                    this.items[this.listPosition + i].Date.ToString("HH:mm"),
                    this.items[this.listPosition + i].Message);
                this.menuItems[i].Visible = true;
                this.menuItems[i].ClickAble = true;
                this.menuItems[i].Refresh();

                this.menuItemsPictures[i].Image = this.GetImage(this.items[this.listPosition + i].IconType);
                this.menuItemsPictures[i].Visible = true;
                ////menuItemsPictures[i].BringToFront();
                this.menuItemsPictures[i].Refresh();
            }

            this.Refresh();
        }

        private Image GetImage(MessageType msgType)
        {
            switch (msgType)
            {
                case MessageType.Info:
                    return this.imageList1.Images[0];
                case MessageType.Instruction:
                    return this.imageList1.Images[1];
                case MessageType.Error:
                    return this.imageList1.Images[2];
                default:
                    return this.imageList1.Images[0]; ////return null;
            }
        }

        /*  void msgbox_BtnOKPressed(VMxMsgBox.BtnPressed button)
        {
            SendReturnEvent();
        }*/

        private void HandleReturn()
        {
            // VMxList Code: if (this.Visible && this.hasElements)
            if (this.Visible)
            {
                if (this.SelectedIndexChanged != null)
                {
                    this.SelectedIndexChanged(this, new IndexEventArgs(this.GetSelectedIndex()));
                }
            }
        }

        private int GetSelectedIndex()
        {
            return this.hasElements ? this.GetFocusPos() + this.listPosition : -1;
        }

        private int GetFocusPos()
        {
            if (!this.hasElements)
            {
                return 0;
            }

            int counter = 0;
            foreach (var menuItem in this.menuItems)
            {
                if (menuItem.Focused)
                {
                    break;
                }

                counter++;
            }

            if (counter >= this.menuItems.Count)
            {
                counter = 0;
            }

            this.lastFocusPos = counter;
            return counter;
        }

        private void SetPicUpDownVisibility()
        {
            ////if (items.Count == 0)
            if (this.items.Count <= 6)
            {
                this.picUp.Visible = false;
                this.picDown.Visible = false;
            }
            else
            {
                int index = this.GetSelectedIndex();
                this.picUp.Visible = index > 0;
                this.picDown.Visible = index != this.items.Count - 1;
            }
        }

        private void HandleDown()
        {
            if (this.hasElements)
            {
                int focusPos = this.GetFocusPos();
                if (focusPos == this.maxListLength - 1)
                {
                    if (this.listPosition + this.maxListLength < this.listLength)
                    {
                        this.listPosition++;
                        this.FillUpItems();
                    }
                }
                else
                {
                    this.menuItems[focusPos + 1].Focus();
                }

                this.SetPicUpDownVisibility();
            }
        }

        private void HandleUp()
        {
            if (this.hasElements)
            {
                int focusPos = this.GetFocusPos();
                if (focusPos == 0)
                {
                    if (this.listPosition > 0)
                    {
                        this.listPosition--;
                        this.FillUpItems();
                    }
                }
                else
                {
                    this.menuItems[focusPos - 1].Focus();
                }

                this.SetPicUpDownVisibility();
            }
        }

        private void ListPaint(object sender, PaintEventArgs e)
        {
            if ((this.isInitialized == false) && (this.menuItems != null))
            {
                this.menuItems[this.lastFocusPos].Focus();
                this.isInitialized = true;
            }
        }

        private void ListEnabledChanged(object sender, EventArgs e)
        {
            if (this.Enabled == false)
            {
                this.isInitialized = false;
            }
        }

        private void MenuItem1Click(object sender, EventArgs e)
        {
            this.HandleReturn();
        }

        private void MenuItem2Click(object sender, EventArgs e)
        {
            this.HandleReturn();
        }

        private void MenuItem3Click(object sender, EventArgs e)
        {
            this.HandleReturn();
        }

        private void MenuItem4Click(object sender, EventArgs e)
        {
            this.HandleReturn();
        }

        private void MenuItem5Click(object sender, EventArgs e)
        {
            this.HandleReturn();
        }

        private void MenuItem6Click(object sender, EventArgs e)
        {
            this.HandleReturn();
        }

        private void MenuItem1MouseDown(object sender, MouseEventArgs e)
        {
            this.menuItem1.Focus();
        }

        private void MenuItem2MouseDown(object sender, MouseEventArgs e)
        {
            this.menuItem2.Focus();
        }

        private void MenuItem3MouseDown(object sender, MouseEventArgs e)
        {
            this.menuItem3.Focus();
        }

        private void MenuItem4MouseDown(object sender, MouseEventArgs e)
        {
            this.menuItem4.Focus();
        }

        private void MenuItem5MouseDown(object sender, MouseEventArgs e)
        {
            this.menuItem5.Focus();
        }

        private void MenuItem6MouseDown(object sender, MouseEventArgs e)
        {
            this.menuItem6.Focus();
        }

        private void PicDownMouseDown(object sender, MouseEventArgs e)
        {
            this.HandleDown();
        }

        private void PicUpMouseDown(object sender, MouseEventArgs e)
        {
            this.HandleUp();
        }

        private void MenuItem1GotFocus(object sender, EventArgs e)
        {
            this.menuItem1.BackColor = Color.FromArgb(13, 32, 134);
            this.SetPicUpDownVisibility();
        }

        private void MenuItem1LostFocus(object sender, EventArgs e)
        {
            this.menuItem1.BackColor = SystemColors.ControlDarkDark;
        }

        private void MenuItem2GotFocus(object sender, EventArgs e)
        {
            this.menuItem2.BackColor = Color.FromArgb(13, 32, 134);
            this.SetPicUpDownVisibility();
        }

        private void MenuItem2LostFocus(object sender, EventArgs e)
        {
            this.menuItem2.BackColor = SystemColors.ControlDarkDark;
        }

        private void MenuItem3GotFocus(object sender, EventArgs e)
        {
            this.menuItem3.BackColor = Color.FromArgb(13, 32, 134);
            this.SetPicUpDownVisibility();
        }

        private void MenuItem3LostFocus(object sender, EventArgs e)
        {
            this.menuItem3.BackColor = SystemColors.ControlDarkDark;
        }

        private void MenuItem4GotFocus(object sender, EventArgs e)
        {
            this.menuItem4.BackColor = Color.FromArgb(13, 32, 134);
            this.SetPicUpDownVisibility();
        }

        private void MenuItem4LostFocus(object sender, EventArgs e)
        {
            this.menuItem4.BackColor = SystemColors.ControlDarkDark;
        }

        private void MenuItem5GotFocus(object sender, EventArgs e)
        {
            this.menuItem5.BackColor = Color.FromArgb(13, 32, 134);
            this.SetPicUpDownVisibility();
        }

        private void MenuItem5LostFocus(object sender, EventArgs e)
        {
            this.menuItem5.BackColor = SystemColors.ControlDarkDark;
        }

        private void MenuItem6GotFocus(object sender, EventArgs e)
        {
            this.menuItem6.BackColor = Color.FromArgb(13, 32, 134);
            this.SetPicUpDownVisibility();
        }

        private void MenuItem6LostFocus(object sender, EventArgs e)
        {
            this.menuItem6.BackColor = SystemColors.ControlDarkDark;
        }
    }
}