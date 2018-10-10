// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOManagement.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IOManagement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace IOControlMGI
{
    using System;
    using System.Windows.Forms;

    using Kontron.Jida32;
    using Kontron.Jida32.I2C;
    using Kontron.Jida32.IO;

    /// <summary>
    /// The io management.
    /// </summary>
    public partial class IoManagement : Form
    {
        private readonly JidaApi jida = new JidaApi();

        private readonly CheckBox[] checkBoxInputs;
        private readonly CheckBox[] checkBoxOutputs;
        private readonly CheckBox[] checkBoxIOWrites;
        private readonly CheckBox[] checkBoxIOReads;

        private readonly Timer updateTimer;

        private JidaBoard board;

        private FakeInputOutput ios;

        private I2CBus busI2C;

        private bool blinkState;

        /// <summary>
        /// Initializes a new instance of the <see cref="IoManagement"/> class.
        /// </summary>
        public IoManagement()
        {
            this.InitializeComponent();
            this.checkBoxInputs = new[]
                                      {
                                          this.checkBoxInput0,
                                          this.checkBoxInput1,
                                          this.checkBoxInput2,
                                          this.checkBoxInput3,
                                          this.checkBoxInput4,
                                          this.checkBoxInput5,
                                          this.checkBoxInput6,
                                          this.checkBoxInput7
                                      };

            this.checkBoxOutputs = new[]
                                      {
                                          this.checkBoxOutput0,
                                          this.checkBoxOutput1,
                                          this.checkBoxOutput2,
                                          this.checkBoxOutput3,
                                          this.checkBoxOutput4,
                                          this.checkBoxOutput5,
                                          this.checkBoxOutput6,
                                          this.checkBoxOutput7
                                      };

            this.checkBoxIOWrites = new[]
                                      {
                                          this.checkBoxIOWrite0,
                                          this.checkBoxIOWrite1,
                                          this.checkBoxIOWrite2,
                                          this.checkBoxIOWrite3,
                                          this.checkBoxIOWrite4,
                                          this.checkBoxIOWrite5,
                                          this.checkBoxIOWrite6,
                                          this.checkBoxIOWrite7
                                      };

            this.checkBoxIOReads = new[]
                                      {
                                          this.checkBoxIORead0,
                                          this.checkBoxIORead1,
                                          this.checkBoxIORead2,
                                          this.checkBoxIORead3,
                                          this.checkBoxIORead4,
                                          this.checkBoxIORead5,
                                          this.checkBoxIORead6,
                                          this.checkBoxIORead7
                                      };

            this.updateTimer = new Timer();
            this.updateTimer.Interval = 200;
            this.updateTimer.Tick += this.UpdateTimerOnTick;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Closed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            this.jida.Dispose();
        }

        private void ButtonVersionClick(object sender, EventArgs e)
        {
            var version = this.jida.Version;
            this.ShowMessage(string.Format("Jida dll version number is {0}", version));
        }

        private void ButtonStartClick(object sender, EventArgs e)
        {
            if (!this.jida.Initialize())
            {
                this.ShowMessage("Jida dll not initialized!");
                return;
            }

            this.board = this.jida.OpenBoard(JidaApi.BoardClassIO, 0);

            if (this.board != null)
            {
                this.ShowMessage(
                    string.Format("Found I/Os: {0}\r\nFound I2C busses: {1}", this.board.IOCount, this.board.I2CCount));
                if (this.board.IOCount == 0)
                {
                    return;
                }

                this.buttonGetDirection.Enabled = true;
                this.buttonReadIO.Enabled = true;
                this.buttonWriteIO.Enabled = true;
                this.busI2C = this.board.GetI2CBus(0);
                this.ios = new FakeInputOutput(this.busI2C, 0x44);

                this.SetUpdateLed(false);
                this.updateTimer.Enabled = true;
            }
            else
            {
                this.ShowMessage("Board could not be opened");
            }
        }

        private void ButtonGetDirectionClick(object sender, EventArgs e)
        {
            try
            {
                var directions = this.ios.Directions;
                for (int i = 0; i < directions.Length; i++)
                {
                    ////this.ShowMessage("I/O " + i + " has direction " + directions[i]);
                    this.checkBoxInputs[i].Checked = directions[i] == IODirection.Input;
                    this.checkBoxOutputs[i].Checked = directions[i] == IODirection.Output;
                    this.checkBoxIOWrites[i].Enabled = directions[i] == IODirection.Output;
                }
            }
            catch (Exception exception)
            {
                this.ShowMessage(string.Format("IO direction not received due to {0}", exception.Message));
            }
        }

        private void ButtonReadIOClick(object sender, EventArgs e)
        {
            try
            {
                var values = this.ios.Read();
                for (int i = 0; i < values.Length; i++)
                {
                    this.checkBoxIOReads[i].Checked = values[i];
                }
            }
            catch (Exception exception)
            {
                this.ShowMessage(string.Format("IO read not successful due to {0}", exception.Message));
            }
        }

        private void ButtonWriteIOClick(object sender, EventArgs e)
        {
            try
            {
                var values = new IOValues<bool>();
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = this.checkBoxIOWrites[i].Checked;
                }

                this.ios.Write(values);
            }
            catch (Exception exception)
            {
                this.ShowMessage(string.Format("IO write not successful due to {0}", exception.Message));
            }
        }

        private void CheckBoxUpdateLedCheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxUpdateLed.CheckState == CheckState.Checked)
            {
                this.SetUpdateLed(true);
            }
            else if (this.checkBoxUpdateLed.CheckState == CheckState.Unchecked)
            {
                this.SetUpdateLed(false);
            }
        }

        private void UpdateTimerOnTick(object sender, EventArgs eventArgs)
        {
            if (this.checkBoxUpdateLed.CheckState == CheckState.Indeterminate)
            {
                this.blinkState = !this.blinkState;
                this.SetUpdateLed(this.blinkState);
            }

            this.checkBoxButton.Checked = (this.busI2C.ReadByte(0x42) & 0x40) == 0;
        }

        private void SetUpdateLed(bool value)
        {
            this.busI2C.WriteByte(0x42, (byte)(value ? 0x7F : 0xFF));
        }

        private void ShowMessage(string message)
        {
            MessageBox.Show(this, message);
        }

        /// <summary>
        /// This class is required because MGI is actually not using the Kontron I/O API, but rather the I2C API
        /// </summary>
        private class FakeInputOutput
        {
            private readonly I2CBus busI2C;

            private readonly byte address;

            public FakeInputOutput(I2CBus busI2C, byte address)
            {
                this.busI2C = busI2C;
                this.address = address;
            }

            public IOValues<IODirection> Directions
            {
                get
                {
                    var directions = new IOValues<IODirection>();
                    for (int i = 0; i < directions.Length; i++)
                    {
                        // This is currently hardcoded: 4 inputs (0..3) and 4 outputs (4..7)
                        directions[i] = i < 4 ? IODirection.Input : IODirection.Output;
                    }

                    return directions;
                }
            }

            public IOValues<bool> Read()
            {
                var outputValues = this.busI2C.ReadByte(this.address);
                var values = this.busI2C.WriteReadByte(this.address, (byte)(outputValues | 0x0F));
                var flags = new IOValues<bool>();
                for (int i = 0; i < flags.Length; i++)
                {
                    flags[i] = (values & (1 << i)) == 0;
                }

                return flags;
            }

            public void Write(IOValues<bool> values)
            {
                byte value = 0x0F;
                for (int i = 0; i < values.Length; i++)
                {
                    if (!values[i])
                    {
                        value |= (byte)(1 << i);
                    }
                }

                this.busI2C.WriteByte(this.address, value);
            }
        }
    }
}
