// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuggestTextBox.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SuggestTextBox type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// TextBox that shows a gray suggestion text when there is no
    /// text set in the TextBox.
    /// </summary>
    public class SuggestTextBox : TextBox
    {
        private string suggestion = string.Empty;

        private string text = string.Empty;

        private bool losingFocus;

        /// <summary>
        /// Gets or sets the suggestion text.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue("")]
        public string Suggestion
        {
            get
            {
                return this.suggestion;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.suggestion = value;
                if (this.ShowSuggestion)
                {
                    base.Text = this.suggestion;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current text in the <see cref="T:System.Windows.Forms.TextBox"/>.
        /// </summary>
        /// <returns>
        /// The text displayed in the control.
        /// </returns>
        public override string Text
        {
            get
            {
                return this.ShowSuggestion ? string.Empty : this.text = base.Text;
            }

            set
            {
                this.text = value;
                base.Text = this.ShowSuggestion ? this.suggestion : value;
                this.UpdateForeColor();
            }
        }

        /// <summary>
        /// Gets the length of text in the control.
        /// </summary>
        /// <returns>
        /// The number of characters contained in the text of the control.
        /// </returns>
        public override int TextLength
        {
            get
            {
                return this.text.Length;
            }
        }

        private bool ShowSuggestion
        {
            get
            {
                return this.text.Length == 0 && (!this.Focused || this.losingFocus);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.GotFocus"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            this.UpdateForeColor();

            if (!base.Text.Equals(this.text))
            {
                base.Text = this.text;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Leave"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnLeave(EventArgs e)
        {
            this.losingFocus = true;
            base.OnLeave(e);

            this.UpdateForeColor();
            this.Text = base.Text;
            this.losingFocus = false;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.TextChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnTextChanged(EventArgs e)
        {
            if (this.ShowSuggestion)
            {
                return;
            }

            this.text = base.Text;
            base.OnTextChanged(e);
        }

        private void UpdateForeColor()
        {
            this.ForeColor = this.ShowSuggestion ? Color.LightGray : SystemColors.ControlText;
        }
    }
}
