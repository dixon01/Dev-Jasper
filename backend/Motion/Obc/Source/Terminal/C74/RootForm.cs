// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootForm.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RootForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Motion.Obc.Terminal.C74.Controls;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The root form for the C74 GUI.
    /// </summary>
    public partial class RootForm : Form, IUiRoot
    {
        private MainField mainField;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootForm"/> class.
        /// </summary>
        public RootForm()
        {
            this.InitializeComponent();

            this.Bounds = new Rectangle(0, 0, 640, 480);

            // use some dummy objects for UI parts that are not available in C74
            this.ButtonBar = new DummyButtonBar();
        }

        /// <summary>
        /// The short key pressed event.
        /// </summary>
        public event EventHandler<ShortKeyEventArgs> ShortKeyPressed;

        /// <summary>
        /// Gets the icon bar.
        /// </summary>
        public IIconBar IconBar
        {
            get
            {
                return this.iconBar;
            }
        }

        /// <summary>
        /// Gets the button bar which is actually not implemented for C74 (there is no button pane).
        /// </summary>
        public IButtonBar ButtonBar { get; private set; }

        /// <summary>
        /// Gets the status field.
        /// </summary>
        public IStatusField StatusField
        {
            get
            {
                return this.statusField;
            }
        }

        /// <summary>
        /// Gets the message field.
        /// </summary>
        public IMessageField MessageField
        {
            get
            {
                return this.messageField;
            }
        }

        /// <summary>
        /// Gets the number input main field.
        /// </summary>
        public INumberInput NumberInput
        {
            get
            {
                return this.numberInputMainField;
            }
        }

        /// <summary>
        /// Gets the driver block input main field.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Always since C74 doesn't support driver block input (yet).
        /// </exception>
        public IDriverBlockInput DriverBlockInput
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the login number input main field.
        /// </summary>
        public ILoginNumberInput LoginNumberInput
        {
            get
            {
                return this.numberInputMainField;
            }
        }

        /// <summary>
        /// Gets the drive select main field.
        /// </summary>
        public IDriveSelect DriveSelect
        {
            get
            {
                return this.driveSelectMainField;
            }
        }

        /// <summary>
        /// Gets the list main field.
        /// </summary>
        public IList List
        {
            get
            {
                return this.listMainField;
            }
        }

        /// <summary>
        /// Gets the block drive wait main field.
        /// </summary>
        public IBlockDriveWait BlockDriveWait
        {
            get
            {
                return this.driveWaitMainField;
            }
        }

        /// <summary>
        /// Gets the special destination drive main field.
        /// </summary>
        public ISpecialDestinationDrive SpecialDestinationDrive
        {
            get
            {
                return this.driveWaitMainField;
            }
        }

        /// <summary>
        /// Gets the block drive main field.
        /// </summary>
        public IBlockDrive BlockDrive
        {
            get
            {
                return this.blockDriveMainField;
            }
        }

        /// <summary>
        /// Gets the main status main field.
        /// </summary>
        public IStatusMainField MainStatus
        {
            get
            {
                return this.statusMainField;
            }
        }

        /// <summary>
        /// Gets the icon list main field.
        /// </summary>
        public IMessageList IconList
        {
            get
            {
                return this.messageListMainField;
            }
        }

        /// <summary>
        /// Gets the iqube radio main field.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Always since C74 will never support IRA.
        /// </exception>
        public IIqubeRadio IqubeRadio
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Sets the active main field.
        /// </summary>
        /// <param name="main">
        /// The main field.
        /// </param>
        public void SetMainField(IMainField main)
        {
            var mf = main as MainField;
            if (mf == null)
            {
                throw new ArgumentException("MainField expected", "main"); // MLHIDE
            }

            mf.SafeBeginInvoke(
                () =>
                {
                    lock (this)
                    {
                        if (this.mainField != null)
                        {
                            this.mainField.MakeVisible(false);
                            this.mainField.MenuButtonPressed -= this.MainFieldOnMenuButtonPressed;
                        }

                        this.mainField = mf;
                        this.mainField.MakeVisible(true);
                        this.mainField.MenuButtonPressed += this.MainFieldOnMenuButtonPressed;
                    }
                });
        }

        /// <summary>
        /// Runs the user interface.
        /// </summary>
        public void Run()
        {
            Application.Run(this);
        }

        /// <summary>
        /// Stops running the user interface.
        /// </summary>
        public void Stop()
        {
            Application.Exit();
        }

        /// <summary>
        /// Processes a command key.
        /// </summary>
        /// <returns>
        /// true if the keystroke was processed and consumed by the control;
        /// otherwise, false to allow further processing.
        /// </returns>
        /// <param name="msg">
        /// A <see cref="T:System.Windows.Forms.Message"/>, passed by reference,
        /// that represents the Win32 message to process.
        /// </param>
        /// <param name="keyData">
        /// One of the <see cref="T:System.Windows.Forms.Keys"/> values that represents the key to process.
        /// </param>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                case Keys.Back:
                case Keys.Home:
                case Keys.Delete:
                case Keys.Escape:
                    this.ProcessKey(C74Keys.Back);
                    break;
                case Keys.Up:
                case Keys.PageUp:
                    this.ProcessKey(C74Keys.Up);
                    break;
                case Keys.Down:
                case Keys.PageDown:
                    this.ProcessKey(C74Keys.Down);
                    break;
                case Keys.Right:
                case Keys.Enter:
                case Keys.End:
                case Keys.Tab:
                    this.ProcessKey(C74Keys.Ok);
                    break;
            }

            return true;
        }

        private void ProcessKey(C74Keys key)
        {
            if (this.messageField.ProcessKey(key))
            {
                return;
            }

            var mf = this.mainField;
            if (mf != null)
            {
                mf.ProcessKey(key);
            }
        }

        private void MainFieldOnMenuButtonPressed(object sender, EventArgs eventArgs)
        {
            this.iconBar.PressMenuButton();
        }

        private class DummyButtonBar : IButtonBar
        {
            public event EventHandler<CommandEventArgs> ButtonClick;
        }
    }
}