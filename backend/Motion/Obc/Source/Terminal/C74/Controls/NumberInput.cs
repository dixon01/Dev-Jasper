// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumberInput.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NumberInput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// A number editor for the C74. It can be highlighted and uses the keys to edit the value.
    /// To start editing, the user needs to press OK first.
    /// Once he's in editing mode (<see cref="IsEditing"/>
    /// </summary>
    public partial class NumberInput : UserControl, IC74Input
    {
        private const int EnterValue = 10;

        private readonly List<int> digits = new List<int>();

        private int currentPosition = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberInput"/> class.
        /// </summary>
        public NumberInput()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Event that is fired whenever the <see cref="Value"/> changes.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Gets or sets a value indicating whether this control is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.BackColor == SystemColors.ControlLight;
            }

            set
            {
                if (this.IsSelected == value)
                {
                    return;
                }

                this.BackColor = value ? SystemColors.ControlLight : SystemColors.ControlDarkDark;
                if (!value)
                {
                    this.currentPosition = -1;
                    this.UpdateText();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this control is in editing mode (the user can change digits).
        /// </summary>
        public bool IsEditing
        {
            get
            {
                return this.currentPosition >= 0;
            }

            set
            {
                if (this.IsEditing == value)
                {
                    return;
                }

                this.currentPosition = value ? 0 : -1;
                this.UpdateText();
            }
        }

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// if a negative number is provided.
        /// </exception>
        public int Value
        {
            get
            {
                var value = 0;
                foreach (var digit in this.digits)
                {
                    if (digit == EnterValue)
                    {
                        break;
                    }

                    value *= 10;
                    value += digit;
                }

                return value;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "Value can't be negative");
                }

                if (this.currentPosition > 0)
                {
                    this.currentPosition = 0;
                }

                this.digits.Clear();
                if (value == 0)
                {
                    this.digits.Add(0);
                }
                else
                {
                    var remainder = value;
                    while (remainder > 0)
                    {
                        this.digits.Insert(0, remainder % 10);
                        remainder /= 10;
                    }
                }

                this.UpdateText();
            }
        }

        /// <summary>
        /// Processes the given key.
        /// </summary>
        /// <param name="key">
        /// The key. This is never <see cref="C74Keys.None"/>.
        /// </param>
        /// <returns>
        /// True if the key was handled otherwise false.
        /// </returns>
        public bool ProcessKey(C74Keys key)
        {
            if (!this.IsSelected)
            {
                return false;
            }

            if (this.currentPosition < 0)
            {
                if (key != C74Keys.Ok)
                {
                    return false;
                }

                this.currentPosition = 0;
                this.UpdateText();
                return true;
            }

            int digit;
            var modulo = this.currentPosition > 0 && this.currentPosition == this.digits.Count - 1 ? 11 : 10;
            switch (key)
            {
                case C74Keys.Back:
                    this.currentPosition--;
                    if (this.digits[this.digits.Count - 1] == EnterValue)
                    {
                        this.digits.Remove(EnterValue);
                    }

                    this.UpdateText();
                    break;
                case C74Keys.Up:
                    digit = this.digits[this.currentPosition];
                    this.digits[this.currentPosition] = (digit + modulo - 1) % modulo;
                    this.UpdateText();
                    this.RaiseValueChanged();
                    break;
                case C74Keys.Down:
                    digit = this.digits[this.currentPosition];
                    this.digits[this.currentPosition] = (digit + 1) % modulo;
                    this.UpdateText();
                    this.RaiseValueChanged();
                    break;
                case C74Keys.Ok:
                    if (this.digits[this.digits.Count - 1] == EnterValue)
                    {
                        this.digits.Remove(EnterValue);
                        this.currentPosition = -1;
                    }
                    else
                    {
                        this.currentPosition++;
                        if (this.currentPosition == this.digits.Count)
                        {
                            this.digits.Add(EnterValue);
                        }
                    }

                    this.UpdateText();
                    break;
            }

            return true;
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        protected virtual void RaiseValueChanged()
        {
            var handler = this.ValueChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void UpdateText()
        {
            var builder = new StringBuilder();
            for (var i = 0; i < this.digits.Count; i++)
            {
                if (i == this.currentPosition)
                {
                    builder.Append('[');
                }

                var digit = this.digits[i];
                if (digit == EnterValue)
                {
                    builder.Append('>');
                }
                else
                {
                    builder.Append(digit);
                }

                if (i == this.currentPosition)
                {
                    builder.Append(']');
                }
            }

            this.label1.Text = builder.ToString();
        }
    }
}