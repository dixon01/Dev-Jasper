// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootForm.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RootForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.Form
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Terminal.Core;
    using Gorba.Motion.Obc.Terminal.Gui.Hardware;
    using Gorba.Motion.Obc.Terminal.Gui.MainFields;
    using Gorba.Motion.Obc.Terminal.Gui.Utility;

    using NLog;

    /// <summary>
    /// The root form.
    /// </summary>
    public partial class RootForm : Form, IUiRoot
    {
        private const int IhmiScalingFactor = 2;

        private static readonly Logger Logger = LogHelper.GetLogger<RootForm>();

        private readonly DriveSelect driveSelect = new DriveSelect();

        private readonly MessageList iconList = new MessageList();

        private readonly IqubeRadio iqubeRadio = new IqubeRadio();

        private readonly List list = new List();

        private readonly DriveWaitScreen mainBlockDriveWait = new DriveWaitScreen();

        private readonly DriveBlock mainBlockDriving = new DriveBlock();

        private readonly StatusMain mainStatus = new StatusMain();

        private readonly NumberInput numberInput = new NumberInput();

        private readonly DriverBlockInput driverBlockInput = new DriverBlockInput();

        private readonly PortListener backlight;

        private MainField mainField;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootForm"/> class.
        /// </summary>
        public RootForm()
        {
            this.InitializeComponent();

            this.ConfigureMainField(this.numberInput);
            this.ConfigureMainField(this.driverBlockInput);
            this.ConfigureMainField(this.driveSelect);
            this.ConfigureMainField(this.list);
            this.ConfigureMainField(this.mainBlockDriveWait);
            this.ConfigureMainField(this.mainBlockDriving);
            this.ConfigureMainField(this.mainStatus);
            this.ConfigureMainField(this.iconList);
            this.ConfigureMainField(this.iqubeRadio);

            KeyBoard.Instance.KeyPressed += this.KeyboardOnKeyPressed;
            this.ihmiRightButton.EscapeClick += (s, e) => this.KeyEscPressed();

            this.backlight = new PortListener(
                new MediAddress(MessageDispatcher.Instance.LocalAddress.Unit, "*"),
                "Backlight");
            this.backlight.Start(TimeSpan.FromSeconds(1));
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
                return this.vmxIconBar1;
            }
        }

        /// <summary>
        /// Gets the button bar.
        /// </summary>
        public IButtonBar ButtonBar
        {
            get
            {
                return this.ihmiRightButton;
            }
        }

        /// <summary>
        /// Gets the status field.
        /// </summary>
        public IStatusField StatusField
        {
            get
            {
                return this.vmxStatusField;
            }
        }

        /// <summary>
        /// Gets the message field.
        /// </summary>
        public IMessageField MessageField
        {
            get
            {
                return this.vmxMessageField;
            }
        }

        /// <summary>
        /// Gets the number input main field.
        /// </summary>
        public INumberInput NumberInput
        {
            get
            {
                return this.numberInput;
            }
        }

        /// <summary>
        /// Gets the login number input main field.
        /// </summary>
        public ILoginNumberInput LoginNumberInput
        {
            get
            {
                return this.numberInput;
            }
        }

        /// <summary>
        /// Gets the drive select main field.
        /// </summary>
        public IDriveSelect DriveSelect
        {
            get
            {
                return this.driveSelect;
            }
        }

        /// <summary>
        /// Gets the list main field.
        /// </summary>
        public IList List
        {
            get
            {
                return this.list;
            }
        }

        /// <summary>
        /// Gets the block drive wait main field.
        /// </summary>
        public IBlockDriveWait BlockDriveWait
        {
            get
            {
                return this.mainBlockDriveWait;
            }
        }

        /// <summary>
        /// Gets the special destination drive main field.
        /// </summary>
        public ISpecialDestinationDrive SpecialDestinationDrive
        {
            get
            {
                return this.mainBlockDriveWait;
            }
        }

        /// <summary>
        /// Gets the block drive main field.
        /// </summary>
        public IBlockDrive BlockDrive
        {
            get
            {
                return this.mainBlockDriving;
            }
        }

        /// <summary>
        /// Gets the iqube radio main field.
        /// </summary>
        public IIqubeRadio IqubeRadio
        {
            get
            {
                return this.iqubeRadio;
            }
        }

        ////public IList GetMainMenu() { return list; }

        /// <summary>
        /// Gets the main status main field.
        /// </summary>
        public IStatusMainField MainStatus
        {
            get
            {
                return this.mainStatus;
            }
        }

        /// <summary>
        /// Gets the icon list main field.
        /// </summary>
        public IMessageList IconList
        {
            get
            {
                return this.iconList;
            }
        }

        /// <summary>
        /// Gets the driver block input main field.
        /// </summary>
        public IDriverBlockInput DriverBlockInput
        {
            get
            {
                return this.driverBlockInput;
            }
        }

        /// <summary>
        /// Runs the user interface.
        /// </summary>
        public void Run()
        {
            try
            {
                ScreenUtil.StartFullScreen(this);
                ScreenUtil.SetCursorVisible(false);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't initialize fullscreen form", ex);
            }

            Application.Run(this);
        }

        /// <summary>
        /// Stops running the user interface.
        /// </summary>
        public void Stop()
        {
            Application.Exit();
            this.backlight.Value = FlagValues.False;
            ScreenUtil.SetCursorVisible(true);
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
                            }

                            this.mainField = mf;
                            this.mainField.MakeVisible(true);
                        }
                    });
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (ScreenUtil.IsIhmi)
            {
                this.Width = 800;
                this.Height = 480;

                vmxDigitalClock1.Size = new Size(184, 48); // clock
                vmxDigitalClock1.Location = new Point(0, 0);

                vmxStatusField.Size = new Size(616, 48); // top status bar
                vmxStatusField.Location = new Point(184, 0);

                vmxMessageField.Size = new Size(550, 48); // 2nd top status bar
                vmxMessageField.Location = new Point(0, 48);

                vmxIconBar1.Location = new Point(551, 48); // right bar
                vmxIconBar1.Size = new Size(90, 480 - 48);
                //// vmxIconBar1.Scale(new SizeF(factor, factor));

                pnlMainField.Size = new Size(275 * 2, 192 * 2); // main panel
                pnlMainField.Location = new Point(0, 97);

                ihmiRightButton.Location = new Point(642, 48);
                ihmiRightButton.Size = new Size(158, 432);

                //// WindowState = FormWindowState.Maximized;

                ////AutoScaleMode = AutoScaleMode.
                ////AutoSize = False
            }

            var t = new Timer { Interval = 500 };
            t.Tick += (s, ev) =>
                {
                    t.Enabled = false;
                    this.backlight.Value = FlagValues.True;
                };
            t.Enabled = true;
        }

        private static ShortKey GetShortKey(KeyBoard.KeyEventArgs e)
        {
            if (e.Action == KeyBoard.KeyAction.Short)
            {
                switch (e.Type)
                {
                    case KeyBoard.KeyType.F1:
                        return ShortKey.F1Short;
                    case KeyBoard.KeyType.F2:
                        return ShortKey.F2Short;
                    case KeyBoard.KeyType.F3:
                        return ShortKey.F3Short;
                    case KeyBoard.KeyType.F4:
                        return ShortKey.F4Short;
                    case KeyBoard.KeyType.F5:
                        return ShortKey.F5Short;
                    case KeyBoard.KeyType.F6:
                        return ShortKey.F6Short;
                }
            }
            else if (e.Action == KeyBoard.KeyAction.LongDown)
            {
                switch (e.Type)
                {
                    case KeyBoard.KeyType.F1:
                        return ShortKey.F1Long;
                    case KeyBoard.KeyType.F2:
                        return ShortKey.F2Long;
                    case KeyBoard.KeyType.F3:
                        return ShortKey.F3Long;
                    case KeyBoard.KeyType.F4:
                        return ShortKey.F4Long;
                    case KeyBoard.KeyType.F5:
                        return ShortKey.F5Long;
                }
            }

            return ShortKey.None;
        }

        private void ConfigureMainField(MainField main)
        {
            main.Location = new Point(0, 0);
            main.Size = new Size(275, 192);

            if (ScreenUtil.IsIhmi)
            {
                main.Scale(new SizeF(IhmiScalingFactor, IhmiScalingFactor));
                this.list.Scale(new SizeF(IhmiScalingFactor, IhmiScalingFactor));
            }

            main.Visible = true;
            main.Hide();
            this.pnlMainField.Controls.Add(main);
        }

        private void KeyboardOnKeyPressed(object sender, KeyBoard.KeyEventArgs e)
        {
            if (this.mainField == null)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                this.BeginInvoke(
                    new EventHandler<KeyBoard.KeyEventArgs>(this.KeyboardOnKeyPressed),
                    new[] { sender, e });
                return;
            }

            switch (e.Type)
            {
                case KeyBoard.KeyType.Escape:
                    this.KeyEscPressed();
                    break;
                case KeyBoard.KeyType.Down:
                    this.KeyDownPressed();
                    break;
                case KeyBoard.KeyType.Up:
                    this.KeyUpPressed();
                    break;
                case KeyBoard.KeyType.Return:
                    this.KeyReturnPressed();
                    break;
                default:
                    if (this.ShortKeyPressed != null)
                    {
                        this.ShortKeyPressed(this, new ShortKeyEventArgs(GetShortKey(e)));
                    }

                    break;
            }
        }

        private void KeyEscPressed()
        {
            this.mainField.SafeBeginInvoke(this.mainField.KeyDispatcher.KeyEscPressed);
        }

        private void KeyUpPressed()
        {
            this.mainField.SafeBeginInvoke(this.mainField.KeyDispatcher.KeyUpPressed);
        }

        private void KeyDownPressed()
        {
            this.mainField.SafeBeginInvoke(this.mainField.KeyDispatcher.KeyDownPressed);
        }

        private void KeyReturnPressed()
        {
            this.mainField.SafeBeginInvoke(this.mainField.KeyDispatcher.KeyReturnPressed);
        }
    }
}