// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumberInputMainField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NumberInputMainField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System;

    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The number input main field.
    /// </summary>
    public partial class NumberInputMainField : MainField, INumberInput, ILoginNumberInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumberInputMainField"/> class.
        /// </summary>
        public NumberInputMainField()
        {
            this.InitializeComponent();
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
                return this.labelHint.Text;
            }

            set
            {
                this.labelHint.Text = value;
            }
        }

        /// <summary>
        ///   Initialize the main field. Call it in the method show()
        /// </summary>
        /// <param name = "mainCaption">The main caption</param>
        /// <param name = "inputCaption">The caption directly above the input box</param>
        /// <param name="maxLen">The maximum length of the input</param>
        public void Init(string mainCaption, string inputCaption, int maxLen)
        {
            this.Init(mainCaption, inputCaption, maxLen, string.Empty, false, false, false);
        }

        /// <summary>
        ///   Initialize the main field. Call it in the method show()
        /// </summary>
        /// <param name="mainCaption">The main caption</param>
        /// <param name="inputCaption">The input caption</param>
        /// <param name="maxLen">The maximum length of the input</param>
        /// <param name="input2Caption">The second input caption</param>
        /// <param name="showGerman">Enable German button. -> LanguageEvent 1</param>
        /// <param name="showFrench">Enable French button. -> LanguageEvent 2</param>
        /// <param name="showEnglish">Enable English button. -> LanguageEvent 3</param>
        public void Init(
            string mainCaption,
            string inputCaption,
            int maxLen,
            string input2Caption,
            bool showGerman,
            bool showFrench,
            bool showEnglish)
        {
            // TODO: maxLen is not yet handled
            this.Init();

            this.labelCaption.Text = mainCaption;
            this.label1.Text = inputCaption;
            this.numberInput1.Value = 0;
            if (string.IsNullOrEmpty(input2Caption))
            {
                this.label2.Visible = false;
                this.numberInput2.Visible = false;
            }
            else
            {
                this.label2.Text = input2Caption;
                this.numberInput2.Value = 0;
                this.label2.Visible = true;
                this.numberInput2.Visible = true;
            }

            var languageButtons = new[] { this.buttonLanguage1, this.buttonLangauge2, this.buttonLanguage3 };
            var index = 0;

            if (showEnglish)
            {
                languageButtons[index].Visible = true;
                languageButtons[index].Text = "E";
                languageButtons[index].Tag = 3;
                index++;
            }

            if (showFrench)
            {
                languageButtons[index].Visible = true;
                languageButtons[index].Text = "F";
                languageButtons[index].Tag = 2;
                index++;
            }

            if (showGerman)
            {
                languageButtons[index].Visible = true;
                languageButtons[index].Text = "D";
                languageButtons[index].Tag = 1;
                index++;
            }

            for (; index < languageButtons.Length; index++)
            {
                languageButtons[index].Visible = false;
            }

            this.numberInput1.IsSelected = true;
            this.numberInput1.IsEditing = true;
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

        /// <summary>
        /// Raises the <see cref="LanguageChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseLanguageChanged(IndexEventArgs e)
        {
            var handler = this.LanguageChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void NumberInput1OnValueChanged(object sender, EventArgs e)
        {
            this.RaiseInputUpdate(new IndexEventArgs(this.numberInput1.Value));
        }

        private void ButtonOkOnPressed(object sender, EventArgs e)
        {
            this.RaiseInputDone(new NumberInputEventArgs(this.numberInput1.Value, this.numberInput2.Value));
        }

        private void ButtonLanguageOnPressed(object sender, EventArgs e)
        {
            this.RaiseLanguageChanged(new IndexEventArgs((int)((ButtonInput)sender).Tag));
        }
    }
}