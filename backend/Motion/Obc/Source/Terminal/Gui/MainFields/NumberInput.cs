// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumberInput.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NumberInput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;

    using Gorba.Motion.Obc.Terminal.Core;
    using Gorba.Motion.Obc.Terminal.Gui.Utility;

    /// <summary>
    /// The number input.
    /// </summary>
    /// <remarks>
    /// Input2 and hint are the same input, so they are not usable together.
    /// </remarks>
    public partial class NumberInput : MainField, ILoginNumberInput, INumberInput
    {
        private string inputCaption = string.Empty;

        private string mainCaption = string.Empty;

        private bool showLanguageDe;

        private bool showLanguageEn;

        private bool showLanguageFr;

        private string mainCaption2 = string.Empty;

        private bool showEdit2;

        private int maxLength = 4; // 4 par défaut et 6 pour la PS (block) BZ 167

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberInput"/> class.
        /// </summary>
        public NumberInput()
        {
            this.InitializeComponent();

            ScreenUtil.Adapt4Ihmi(this, false, false);
        }

        /// <summary>
        /// The input update event.
        /// The index value will be the value of this input.
        /// </summary>
        public event EventHandler<IndexEventArgs> InputUpdate;

        /// <summary>
        /// The input done event.
        /// </summary>
        public event EventHandler<NumberInputEventArgs> InputDone;

        /// <summary>
        ///   The changed language event. Parameter is an int. 1: German, 2: French, 3: English
        /// </summary>
        public event EventHandler<IndexEventArgs> LanguageChanged;

        /// <summary>
        /// Gets or sets the hint text.
        /// </summary>
        public string HintText
        {
            get
            {
                return this.lblHint.Text;
            }

            set
            {
                this.lblHint.Text = value;
            }
        }

        /// <summary>
        ///   Initialize the main field. Call it in the method show()
        /// </summary>
        /// <param name = "main">The main caption</param>
        /// <param name = "input">The caption directly above the input box</param>
        /// <param name="maxLen">The maximum length of the input</param>
        public void Init(string main, string input, int maxLen)
        {
            this.Init(main, input, maxLen, string.Empty, false, false, false);
        }

        /// <summary>
        ///   Initialize the main field. Call it in the method show()
        /// </summary>
        /// <param name="main">The main caption</param>
        /// <param name="input">The input caption</param>
        /// <param name="maxLen">The maximum length of the input</param>
        /// <param name="input2">The second input caption</param>
        /// <param name="showGerman">Enable German button. -> LanguageEvent 1</param>
        /// <param name="showFrench">Enable French button. -> LanguageEvent 2</param>
        /// <param name="showEnglish">Enable English button. -> LanguageEvent 3</param>
        public void Init(
            string main,
            string input,
            int maxLen,
            string input2,
            bool showGerman,
            bool showFrench,
            bool showEnglish)
        {
            this.mainCaption = main;
            this.inputCaption = input;
            this.showLanguageDe = showGerman;
            this.showLanguageFr = showFrench;
            this.showLanguageEn = showEnglish;

            this.maxLength = maxLen;

            this.mainCaption2 = input2;
            this.showEdit2 = !string.IsNullOrEmpty(input2);

            this.RealInit();
        }

        /// <summary>
        /// Show this main field.
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
        /// Sets the text value.
        /// </summary>
        /// <param name="number">
        /// The number.
        /// </param>
        public void SetTextValue(int number)
        {
            this.txtInputNumber.Text = number.ToString(CultureInfo.InvariantCulture);
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

        private void RealInit()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.RealInit));
            }
            else
            {
                this.lblMainCaption.Text = this.mainCaption;
                this.lblCaption.Text = this.inputCaption;

                this.txtInputNumber.Text = string.Empty;
                this.txtInputNumber2.Text = string.Empty;

                txtInputNumber2.Visible = this.showEdit2;
                if (this.showEdit2)
                {
                    this.lblHint.Text = this.mainCaption2;
                    txtInputNumber2.Password = true;
                }
                else
                {
                    this.lblHint.Text = string.Empty;
                }

                this.btnGerman.Visible = this.showLanguageDe;
                this.btnFrench.Visible = this.showLanguageFr;
                this.btnEnglish.Visible = this.showLanguageEn;
            }
        }

        private void CallReturnEvent()
        {
            if (this.Visible)
            {
                if (this.txtInputNumber.Text.Length == 0)
                {
                    return;
                }

                if (this.showEdit2 && this.txtInputNumber2.Text.Length == 0)
                {
                    return;
                }

                if (this.InputDone != null)
                {
                    int num1 = int.Parse(this.txtInputNumber.Text);
                    int num2 = -1;

                    if (this.showEdit2)
                    {
                        num2 = int.Parse(this.txtInputNumber2.Text);
                    }

                    this.InputDone(this, new NumberInputEventArgs(num1, num2));
                }
            }
        }

        private void ChangeLanguage(int languageNbr)
        {
            if (this.LanguageChanged != null)
            {
                this.LanguageChanged(this, new IndexEventArgs(languageNbr));
            }
        }

        private void BtnEnglishMouseDown(object sender, MouseEventArgs e)
        {
            this.ChangeLanguage(3);
        }

        private void BtnFrenchMouseDown(object sender, MouseEventArgs e)
        {
            this.ChangeLanguage(2);
        }

        private void BtnGermanMouseDown(object sender, MouseEventArgs e)
        {
            this.ChangeLanguage(1);
        }

        private void BtnClearMouseDown(object sender, MouseEventArgs e)
        {
            if (this.showEdit2)
            {
                this.txtInputNumber2.Focus();
                this.txtInputNumber2.Clear();
            }

            this.txtInputNumber.Focus();
            this.txtInputNumber.Clear();
        }

        private void BtnBackspaceMouseDown(object sender, MouseEventArgs e)
        {
            // TODO: verify if this is OK, or should we also check on the first one to be focused
            if (this.showEdit2 && this.txtInputNumber2.Focused)
            {
                this.txtInputNumber2.RemoveLastChar();
            }
            else
            {
                this.txtInputNumber.RemoveLastChar();
            }
        }

        private void AppendCharacter(char c)
        {
            if (this.txtInputNumber.Text.Length == 0)
            {
                // by default, we use input1
                this.txtInputNumber.Focus();
            }

            // TODO: verify if this is OK, or should we also check on the first one to be focused
            if (this.showEdit2 && this.txtInputNumber2.Focused)
            {
                this.txtInputNumber2.AppendCharacter(c);
            }
            else
            {
                this.txtInputNumber.AppendCharacter(c);
            }
        }

        private void Btn0MouseDown(object sender, MouseEventArgs e)
        {
            this.AppendCharacter('0');
        }

        private void Btn1MouseDown(object sender, MouseEventArgs e)
        {
            this.AppendCharacter('1');
        }

        private void Btn2MouseDown(object sender, MouseEventArgs e)
        {
            this.AppendCharacter('2');
        }

        private void Btn3MouseDown(object sender, MouseEventArgs e)
        {
            this.AppendCharacter('3');
        }

        private void Btn4MouseDown(object sender, MouseEventArgs e)
        {
            this.AppendCharacter('4');
        }

        private void Btn5MouseDown(object sender, MouseEventArgs e)
        {
            this.AppendCharacter('5');
        }

        private void Btn6MouseDown(object sender, MouseEventArgs e)
        {
            this.AppendCharacter('6');
        }

        private void Btn7MouseDown(object sender, MouseEventArgs e)
        {
            this.AppendCharacter('7');
        }

        private void Btn8MouseDown(object sender, MouseEventArgs e)
        {
            this.AppendCharacter('8');
        }

        private void Btn9MouseDown(object sender, MouseEventArgs e)
        {
            this.AppendCharacter('9');
        }

        private void TxtInputNumberTextChanged(object sender, EventArgs e)
        {
            if (this.txtInputNumber.Text.Length > this.maxLength)
            {
                this.txtInputNumber.Text = this.txtInputNumber.Text.Substring(0, this.maxLength);
            }

            if (this.Visible)
            {
                if (this.InputUpdate != null)
                {
                    string text = this.txtInputNumber.Text;
                    int value = text.Length > 0 ? int.Parse(text) : 0;
                    this.InputUpdate(this, new IndexEventArgs(value));
                }
            }
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
            // btnEnter, the btnEnter is not anymore redrawn. So the button stays gray
            // until the message box disappear... (by checking the enabled changed event)
            this.timerStartValidation.Enabled = false;
            this.CallReturnEvent();
        }
    }
}