// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriverBlockInput.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DriverBlockInput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Utility.Compatibility;
    using Gorba.Motion.Obc.Terminal.Core;
    using Gorba.Motion.Obc.Terminal.Gui.Utility;

    /// <summary>
    /// The driver block input main field.
    /// </summary>
    public partial class DriverBlockInput : MainField, IDriverBlockInput
    {
        private string alpha1;

        private string alpha2;

        private string alpha3;

        private string alpha4;

        private string alpha5;

        private string alpha6;

        private string alpha7;

        private string alpha8;

        private string alpha1ShortName;

        private string alpha2ShortName;

        private string alpha3ShortName;

        private string alpha4ShortName;

        private string alpha5ShortName;

        private string alpha6ShortName;

        private string alpha7ShortName;

        private string alpha8ShortName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverBlockInput"/> class.
        /// </summary>
        public DriverBlockInput()
        {
            this.InitializeComponent();

            ScreenUtil.Adapt4Ihmi(this, false, false);
        }

        /// <summary>
        /// The input done event.
        /// </summary>
        public event EventHandler<NumberInputEventArgs> InputDone;

        /// <summary>
        /// The input update event.
        /// The index value will be the value of this input.
        /// </summary>
        public event EventHandler<IndexEventArgs> InputUpdate;

        /// <summary>
        /// Gets or sets the hint text.
        /// </summary>
        public string HintText { get; set; }

        /// <summary>
        /// Gets or sets the block number.
        /// </summary>
        public string Block
        {
            get
            {
                return this.txtInputNumber.Text;
            }

            set
            {
                this.txtInputNumber.Text = value;
            }
        }

        // ReSharper disable ParameterHidesMember

        /// <summary>
        /// Initialize the main field. Call it in the method show()
        /// </summary>
        /// <param name="mainCaption">
        /// The main caption
        /// </param>
        /// <param name="inputCaption">
        /// The caption directly above the input box
        /// </param>
        /// <param name="maxLen">
        /// The maximum input length.
        /// </param>
        /// <param name="alpha1">
        /// The alpha 1.
        /// </param>
        /// <param name="alpha2">
        /// The alpha 2.
        /// </param>
        /// <param name="alpha3">
        /// The alpha 3.
        /// </param>
        /// <param name="alpha4">
        /// The alpha 4.
        /// </param>
        /// <param name="alpha5">
        /// The alpha 5.
        /// </param>
        /// <param name="alpha6">
        /// The alpha 6.
        /// </param>
        /// <param name="alpha7">
        /// The alpha 7.
        /// </param>
        /// <param name="alpha8">
        /// The alpha 8.
        /// </param>
        /// <param name="alpha1ShortName">
        /// The alpha 1 Short Name.
        /// </param>
        /// <param name="alpha2ShortName">
        /// The alpha 2 Short Name.
        /// </param>
        /// <param name="alpha3ShortName">
        /// The alpha 3 Short Name.
        /// </param>
        /// <param name="alpha4ShortName">
        /// The alpha 4 Short Name.
        /// </param>
        /// <param name="alpha5ShortName">
        /// The alpha 5 Short Name.
        /// </param>
        /// <param name="alpha6ShortName">
        /// The alpha 6 Short Name.
        /// </param>
        /// <param name="alpha7ShortName">
        /// The alpha 7 Short Name.
        /// </param>
        /// <param name="alpha8ShortName">
        /// The alpha 8 Short Name.
        /// </param>
        public void Init(
            string mainCaption,
            string inputCaption,
            int maxLen,
            string alpha1,
            string alpha2,
            string alpha3,
            string alpha4,
            string alpha5,
            string alpha6,
            string alpha7,
            string alpha8,
            string alpha1ShortName,
            string alpha2ShortName,
            string alpha3ShortName,
            string alpha4ShortName,
            string alpha5ShortName,
            string alpha6ShortName,
            string alpha7ShortName,
            string alpha8ShortName)
        {
            this.alpha1 = alpha1;
            this.alpha2 = alpha2;
            this.alpha3 = alpha3;
            this.alpha4 = alpha4;
            this.alpha5 = alpha5;
            this.alpha6 = alpha6;
            this.alpha7 = alpha7;
            this.alpha8 = alpha8;

            this.alpha1ShortName = alpha1ShortName;
            this.alpha2ShortName = alpha2ShortName;
            this.alpha3ShortName = alpha3ShortName;
            this.alpha4ShortName = alpha4ShortName;
            this.alpha5ShortName = alpha5ShortName;
            this.alpha6ShortName = alpha6ShortName;
            this.alpha7ShortName = alpha7ShortName;
            this.alpha8ShortName = alpha8ShortName;

            this.RealInit();
        }

        // ReSharper restore ParameterHidesMember

        /// <summary>
        /// Shows this field.
        /// </summary>
        public new void Show()
        {
            base.Show();
            this.txtInputNumber.BringToFront();
            this.txtInputNumber.Focus();
        }

        /// <summary>
        ///   Hides the message box
        /// </summary>
        public override void HideMessageBox()
        {
            base.HideMessageBox();
            this.SafeBeginInvoke(() => this.txtInputNumber.Focus());
        }

        /// <summary>
        ///   Hides the progress bar
        /// </summary>
        public override void HideProgressBar()
        {
            base.HideProgressBar();
            this.SafeBeginInvoke(() => this.txtInputNumber.Focus());
        }

        /// <summary>
        /// Make sure that calling code is running in the GUI thread!!!
        ///   Check with GetControl().InvokeRequired
        /// </summary>
        /// <param name="visible">
        /// A flag indicating if this field should be made visible.
        /// </param>
        public override void MakeVisible(bool visible)
        {
            if (!visible)
            {
                this.Invalidate();
            }

            base.MakeVisible(visible);
        }

        /// <summary>
        /// Raises the <see cref="MainField.ReturnPressed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnReturnPressed(EventArgs e)
        {
            this.CallReturnEvent();
        }

        /// <summary>
        /// Raises the <see cref="InputDone"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseInputDone(NumberInputEventArgs e)
        {
            var handler = this.InputDone;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="InputUpdate"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseInputUpdate(IndexEventArgs e)
        {
            var handler = this.InputUpdate;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RealInit()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.RealInit));
            }
            else
            {
                this.lblMainCaption.Text = "Prise de service"; // this.mainCaption;
                this.lblCaption.Text = "Numéro de Service agent"; // this.inputCaption;

                ////this.txtInputNumber.Text = string.Empty;

                this.vBtnAlpha1.Text = string.IsNullOrEmpty(this.alpha1ShortName) ? this.alpha1 : this.alpha1ShortName;
                this.vBtnAlpha2.Text = string.IsNullOrEmpty(this.alpha2ShortName) ? this.alpha2 : this.alpha2ShortName;
                this.vBtnAlpha3.Text = string.IsNullOrEmpty(this.alpha3ShortName) ? this.alpha3 : this.alpha3ShortName;
                this.vBtnAlpha4.Text = string.IsNullOrEmpty(this.alpha4ShortName) ? this.alpha4 : this.alpha4ShortName;
                this.vBtnAlpha5.Text = string.IsNullOrEmpty(this.alpha5ShortName) ? this.alpha5 : this.alpha5ShortName;
                this.vBtnAlpha6.Text = string.IsNullOrEmpty(this.alpha6ShortName) ? this.alpha6 : this.alpha6ShortName;
                this.vBtnAlpha7.Text = string.IsNullOrEmpty(this.alpha7ShortName) ? this.alpha7 : this.alpha7ShortName;
                this.vBtnAlpha8.Text = string.IsNullOrEmpty(this.alpha8ShortName) ? this.alpha8 : this.alpha8ShortName;

                this.vBtnAlpha1.Visible = !string.IsNullOrEmpty(this.alpha1);
                this.vBtnAlpha2.Visible = !string.IsNullOrEmpty(this.alpha2);
                this.vBtnAlpha3.Visible = !string.IsNullOrEmpty(this.alpha3);
                this.vBtnAlpha4.Visible = !string.IsNullOrEmpty(this.alpha4);
                this.vBtnAlpha5.Visible = !string.IsNullOrEmpty(this.alpha5);
                this.vBtnAlpha6.Visible = !string.IsNullOrEmpty(this.alpha6);
                this.vBtnAlpha7.Visible = !string.IsNullOrEmpty(this.alpha7);
                this.vBtnAlpha8.Visible = !string.IsNullOrEmpty(this.alpha8);
            }
        }

        private int GetBlockNumber()
        {
            int block;
            return ParserUtil.TryParse(this.txtInputNumber.Text, out block) ? block : -1;
        }

        private void CallReturnEvent()
        {
            if (!this.Visible)
            {
                return;
            }

            if (this.txtInputNumber.Text.Length == 0)
            {
                return;
            }

            this.RaiseInputDone(new NumberInputEventArgs(this.GetBlockNumber()));
        }

        private void BtnClearMouseDown(object sender, MouseEventArgs e)
        {
            this.txtInputNumber.Focus();
            this.txtInputNumber.Clear();
        }

        private void BtnBackspaceMouseDown(object sender, MouseEventArgs e)
        {
            this.txtInputNumber.Focus();
            this.txtInputNumber.RemoveLastChar();
        }

        private void Btn0MouseDown(object sender, MouseEventArgs e)
        {
            this.txtInputNumber.Focus();
            this.txtInputNumber.AppendCharacter('0');
        }

        private void Btn1MouseDown(object sender, MouseEventArgs e)
        {
            this.txtInputNumber.Focus();
            this.txtInputNumber.AppendCharacter('1');
        }

        private void Btn2MouseDown(object sender, MouseEventArgs e)
        {
            this.txtInputNumber.Focus();
            this.txtInputNumber.AppendCharacter('2');
        }

        private void Btn3MouseDown(object sender, MouseEventArgs e)
        {
            this.txtInputNumber.Focus();
            this.txtInputNumber.AppendCharacter('3');
        }

        private void Btn4MouseDown(object sender, MouseEventArgs e)
        {
            this.txtInputNumber.Focus();
            this.txtInputNumber.AppendCharacter('4');
        }

        private void Btn5MouseDown(object sender, MouseEventArgs e)
        {
            this.txtInputNumber.Focus();
            this.txtInputNumber.AppendCharacter('5');
        }

        private void Btn6MouseDown(object sender, MouseEventArgs e)
        {
            this.txtInputNumber.Focus();
            this.txtInputNumber.AppendCharacter('6');
        }

        private void Btn7MouseDown(object sender, MouseEventArgs e)
        {
            this.txtInputNumber.Focus();
            this.txtInputNumber.AppendCharacter('7');
        }

        private void Btn8MouseDown(object sender, MouseEventArgs e)
        {
            this.txtInputNumber.Focus();
            this.txtInputNumber.AppendCharacter('8');
        }

        private void Btn9MouseDown(object sender, MouseEventArgs e)
        {
            this.txtInputNumber.Focus();
            this.txtInputNumber.AppendCharacter('9');
        }

        private void BtnEscClick(object sender, EventArgs e)
        {
            this.OnEscapePressed(e);
        }

        private void BtnEnterClick(object sender, EventArgs e)
        {
            this.timerStartValidation.Enabled = true;
        }

        private void TimerStartValidationTick(object sender, EventArgs e)
        {
            // This timer is used to send the Return event. It's a workaround!!
            // The problem is: if the ProgressBar or MessageBox is displayed from the
            // btnEnter, the btnEnter is not anymored redrawn. So the button stays gray
            // until the message box disapear... (by checking the enabled changed event)
            this.timerStartValidation.Enabled = false;
            this.CallReturnEvent();
        }

        private void BtnAlpha1OnMouseDown(object sender, MouseEventArgs e)
        {
            this.txtInputNumber.Text += this.alpha1;
            this.RaiseInputUpdate(new IndexEventArgs(this.GetBlockNumber()));
        }

        private void BtnAlpha2OnMouseDown(object sender, MouseEventArgs e)
        {
            this.txtInputNumber.Text += this.alpha2;
            this.RaiseInputUpdate(new IndexEventArgs(this.GetBlockNumber()));
        }

        private void BtnAlpha3OnMouseDown(object sender, MouseEventArgs e)
        {
            this.txtInputNumber.Text += this.alpha3;
            this.RaiseInputUpdate(new IndexEventArgs(this.GetBlockNumber()));
        }

        private void BtnAlpha4OnClick(object sender, EventArgs e)
        {
            this.txtInputNumber.Text += this.alpha4;
            this.RaiseInputUpdate(new IndexEventArgs(this.GetBlockNumber()));
        }

        private void BtnAlpha5OnClick(object sender, EventArgs e)
        {
            this.txtInputNumber.Text += this.alpha5;
            this.RaiseInputUpdate(new IndexEventArgs(this.GetBlockNumber()));
        }

        private void BtnAlpha6OnClick(object sender, EventArgs e)
        {
            this.txtInputNumber.Text += this.alpha6;
            this.RaiseInputUpdate(new IndexEventArgs(this.GetBlockNumber()));
        }

        private void BtnAlpha7OnClick(object sender, EventArgs e)
        {
            this.txtInputNumber.Text += this.alpha7;
            this.RaiseInputUpdate(new IndexEventArgs(this.GetBlockNumber()));
        }

        private void BtnAlpha8OnClick(object sender, EventArgs e)
        {
            this.txtInputNumber.Text += this.alpha8;
            this.RaiseInputUpdate(new IndexEventArgs(this.GetBlockNumber()));
        }
    }
}