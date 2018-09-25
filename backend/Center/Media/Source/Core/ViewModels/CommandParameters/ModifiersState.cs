// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModifiersState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ModifiersState.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    using System.Windows.Input;

    /// <summary>
    /// Represents the state of the modifier keys
    /// </summary>
    public class ModifiersState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiersState"/> class.
        /// </summary>
        public ModifiersState()
        {
            this.IsControlPressed = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            this.IsShiftPressed = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            this.IsAltPressed = (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the ctrl key is pressed
        /// </summary>
        public bool IsControlPressed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the shift key is pressed
        /// </summary>
        public bool IsShiftPressed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the alt key is pressed
        /// </summary>
        public bool IsAltPressed { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            const string Format = "[ModifiersState ctrl: {0}, shift: {1}, alt: {2}]";
            return string.Format(Format, this.IsControlPressed, this.IsShiftPressed, this.IsAltPressed);
        }
    }
}