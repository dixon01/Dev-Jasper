// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriveSelect.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DriveSelect type.
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
    /// The drive select main field.
    /// </summary>
    public partial class DriveSelect : MainField, IDriveSelect
    {
        private string caption = "List Caption";

        private bool hasElements;

        private bool isAdditionalActive;

        private bool isInitialized;

        private bool isDrivingSchoolActive;

        private List<string> items;

        private int lastFocusPos;

        private int listLength;

        private int maxListLength = 4;

        private List<CustomMenuItem> menuItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="DriveSelect"/> class.
        /// </summary>
        public DriveSelect()
        {
            this.InitializeComponent();
            this.hasElements = false;
        }

        /// <summary>
        /// The drive confirmed event.
        /// </summary>
        public event EventHandler<DriveSelectedEventArgs> DriveConfirmed;

        /// <summary>
        /// Initialization of this field.
        /// </summary>
        /// <param name = "captionString">
        /// The caption.
        /// </param>
        /// <param name = "itemList">
        /// The drives to select from.
        /// </param>
        /// <param name = "drivingSchoolActive">
        /// if true the driving school button will be selected
        /// </param>
        /// <param name = "additionalActive">
        /// if true the additional drive button will be selected
        /// </param>
        /// <param name="focusIndex">
        /// Index of the focused drive.
        /// </param>
        public void Init(
            string captionString,
            List<string> itemList,
            bool drivingSchoolActive,
            bool additionalActive,
            int focusIndex)
        {
            this.isInitialized = false;
            this.lastFocusPos = focusIndex;
            this.caption = captionString;
            this.items = itemList;
            this.isDrivingSchoolActive = drivingSchoolActive;
            this.isAdditionalActive = additionalActive;
            if (itemList.Count > 4)
            {
                throw new Exception("max items exceeded");
            }

            this.RealInit();
        }

        /// <summary>
        ///   Hides the message box
        /// </summary>
        public override void HideMessageBox()
        {
            base.HideMessageBox();
            this.SafeBeginInvoke(() => this.menuItems[this.lastFocusPos].Focus());
        }

        /// <summary>
        ///   Hides the progress bar
        /// </summary>
        public override void HideProgressBar()
        {
            base.HideProgressBar();
            this.SafeBeginInvoke(() => this.menuItems[this.lastFocusPos].Focus());
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

            if (this.hasElements)
            {
                int focusPos = this.GetFocusPos();
                if (focusPos == this.maxListLength - 1)
                {
                }
                else
                {
                    this.menuItems[focusPos + 1].Focus();
                }
            }
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

            if (this.hasElements)
            {
                int focusPos = this.GetFocusPos();
                if (focusPos == 0)
                {
                }
                else
                {
                    this.menuItems[focusPos - 1].Focus();
                }
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
            this.SendReturnEvent();
        }

        private void RealInit()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.RealInit));
            }
            else
            {
                this.Controls.Clear();
                this.InitializeComponent();

                ScreenUtil.Adapt4Ihmi(this, true, true);

                this.lblCaption.Text = this.caption;

                this.hasElements = true;

                this.menuItems = new List<CustomMenuItem>
                                     {
                                         this.menuItem1,
                                         this.menuItem2,
                                         this.menuItem3,
                                         this.menuItem4
                                     };

                this.btnEnter.Visible = false;
                this.maxListLength++;

                this.maxListLength = this.menuItems.Count;
                this.listLength = this.items.Count;

                if (this.listLength < this.maxListLength)
                {
                    this.menuItems.RemoveRange(this.items.Count, this.maxListLength - this.listLength);
                    this.maxListLength = this.listLength;
                }

                this.btnLoginSchool.IsPushed = this.isDrivingSchoolActive;
                this.btnLoginExtensionCourse.IsPushed = this.isAdditionalActive;

                this.FillUpItems();
                this.menuItem1.Focus();
            }
        }

        private void FillUpItems()
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                this.menuItems[i].Text = this.items[i];
                this.menuItems[i].Visible = true;
                this.menuItems[i].ClickAble = true;
                this.menuItems[i].Refresh();
            }
        }

        private void SendReturnEvent()
        {
            if (this.Visible)
            {
                if (this.DriveConfirmed != null)
                {
                    var e = new DriveSelectedEventArgs(
                        this.GetSelectedIndex(),
                        this.btnLoginSchool.IsPushed,
                        this.btnLoginExtensionCourse.IsPushed);
                    this.DriveConfirmed(this, e);
                }
            }
        }

        /*  void msgbox_BtnOKPressed(VT3MsgBox.BtnPressed button)
    {
        SendReturnEvent();
    }*/

        private int GetSelectedIndex()
        {
            return this.hasElements ? this.GetFocusPos() : -1;
        }

        private int GetFocusPos()
        {
            if (this.hasElements)
            {
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

            this.lastFocusPos = 0;
            return 0;
        }

        private void ListPaint(object sender, PaintEventArgs e)
        {
            if ((this.isInitialized == false) && (this.menuItems != null))
            {
                this.menuItems[this.lastFocusPos].Focus();
                this.isInitialized = true;
            }
        }

        private void MenuItem1Click(object sender, EventArgs e)
        {
            this.SendReturnEvent();
        }

        private void MenuItem2Click(object sender, EventArgs e)
        {
            this.SendReturnEvent();
        }

        private void MenuItem3Click(object sender, EventArgs e)
        {
            this.SendReturnEvent();
        }

        private void MenuItem4Click(object sender, EventArgs e)
        {
            this.SendReturnEvent();
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

        private void MenuItem1GotFocus(object sender, EventArgs e)
        {
            this.menuItem1.BackColor = Color.FromArgb(13, 32, 134);
        }

        private void MenuItem2GotFocus(object sender, EventArgs e)
        {
            this.menuItem2.BackColor = Color.FromArgb(13, 32, 134);
        }

        private void MenuItem3GotFocus(object sender, EventArgs e)
        {
            this.menuItem3.BackColor = Color.FromArgb(13, 32, 134);
        }

        private void MenuItem4GotFocus(object sender, EventArgs e)
        {
            this.menuItem4.BackColor = Color.FromArgb(13, 32, 134);
        }

        private void MenuItem1LostFocus(object sender, EventArgs e)
        {
            this.menuItem1.BackColor = SystemColors.ControlDarkDark;
        }

        private void MenuItem2LostFocus(object sender, EventArgs e)
        {
            this.menuItem2.BackColor = SystemColors.ControlDarkDark;
        }

        private void MenuItem3LostFocus(object sender, EventArgs e)
        {
            this.menuItem3.BackColor = SystemColors.ControlDarkDark;
        }

        private void MenuItem4LostFocus(object sender, EventArgs e)
        {
            this.menuItem4.BackColor = SystemColors.ControlDarkDark;
        }
    }
}