// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyBoard.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the KeyBoard type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.Hardware
{
    using System;

    /// <summary>
    /// The keyboard.
    /// </summary>
    public abstract class KeyBoard : IDisposable
    {
        /// <summary>
        /// If you register on this event DO NOT forget to release it (...-=...)!!! After you don't use it anymore
        /// </summary>
        public abstract event EventHandler<KeyEventArgs> KeyPressed;

        /// <summary>
        /// The key action.
        /// </summary>
        public enum KeyAction
        {
            /// <summary>
            /// The key was pressed for a short time.
            /// </summary>
            Short = 0x1000000,

            /// <summary>
            /// The key is held down for a long time.
            /// </summary>
            LongDown = 0x2000000,

            /// <summary>
            /// The key has been released after holding it for a long time.
            /// </summary>
            LongUp = 0x4000000
        }

        /// <summary>
        /// The key type.
        /// </summary>
        public enum KeyType
        {
            /// <summary>
            /// The F1 key.
            /// </summary>
            F1 = 0x01,

            /// <summary>
            /// The F2 key.
            /// </summary>
            F2 = 0x02,

            /// <summary>
            /// The F3 key.
            /// </summary>
            F3 = 0x04,

            /// <summary>
            /// The F4 key.
            /// </summary>
            F4 = 0x08,

            /// <summary>
            /// The return key.
            /// </summary>
            Return = 0x10,

            /// <summary>
            /// The down key.
            /// </summary>
            Down = 0x20,

            /// <summary>
            /// The up key.
            /// </summary>
            Up = 0x40,

            /// <summary>
            /// The escape key.
            /// </summary>
            Escape = 0x80,

            /// <summary>
            /// The F5 key.
            /// </summary>
            F5 = 0x100,

            /// <summary>
            /// The F6 key.
            /// </summary>
            F6 = 0x200,

            /// <summary>
            /// The F7 key.
            /// </summary>
            F7 = 0x400,

            /// <summary>
            /// The F8 key.
            /// </summary>
            F8 = 0x800,

            /// <summary>
            /// The maximum value.
            /// </summary>
            Max = 0x800000
        }

        /// <summary>
        /// Gets the single instance.
        /// </summary>
        public static KeyBoard Instance
        {
            get
            {
                // TODO: provide a single keyboard or get completely rid of this class (better implementation?)
                return new DummyImplementation();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the keyboard is enabled.
        /// You can block the keyboard by setting this to true. No keyboard events will be thrown anymore
        /// </summary>
        public abstract bool KeyboardEnabled { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }

        /// <summary>
        /// The key event arguments.
        /// </summary>
        public class KeyEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets the action.
            /// </summary>
            public KeyAction Action { get; set; }

            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            public KeyType Type { get; set; }
        }

        private class DummyImplementation : KeyBoard
        {
            public override event EventHandler<KeyEventArgs> KeyPressed;

            public override bool KeyboardEnabled { get; set; }
        }
    }
}