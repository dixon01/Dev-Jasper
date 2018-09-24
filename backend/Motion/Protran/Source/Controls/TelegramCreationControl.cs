// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramCreationControl.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramCreationControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Control that allows to create the different types of IBIS telegrams
    /// </summary>
    public partial class TelegramCreationControl : UserControl
    {
        private readonly DS021CRowControl[] tlgDs021CControls;

        private readonly TextBox[] tlgDs021TextBoxes;

        private readonly GO002RowControl[] tlgGo002Controls;

        private readonly GO004RowControl[] tlgGo004Controls;

        private readonly GO005RowControl[] tlgGo005Controls;

        private readonly TextBox[] tlgGo007TextBoxes;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramCreationControl"/> class.
        /// </summary>
        public TelegramCreationControl()
        {
            this.InitializeComponent();

            this.tlgDs021CControls = new[]
            {
                this.dS021CControl1, this.dS021CControl2, this.dS021CControl3, this.dS021CControl4,
                this.dS021CControl5, this.dS021CControl6, this.dS021CControl7, this.dS021CControl8,
                this.dS021CControl9, this.dS021CControl10, this.dS021CControl11
            };

            this.tlgDs021TextBoxes = new[]
            {
                this.textBoxStop1, this.textBoxStop2, this.textBoxStop3, this.textBoxStop4,
                this.textBoxStop5, this.textBoxStop6, this.textBoxStop7, this.textBoxStop8,
                this.textBoxStop9, this.textBoxStop10, this.textBoxStop11, this.textBoxStop12,
                this.textBoxStop13, this.textBoxStop14, this.textBoxStop15
            };

            this.tlgGo002Controls = new[]
            {
                this.gO002Control1,
                this.gO002Control2,
                this.gO002Control3,
                this.gO002Control4,
                this.gO002Control5,
                this.gO002Control6,
                this.gO002Control7
            };

            this.tlgGo004Controls = new[]
            {
                this.gO004Control1,
                this.gO004Control2,
                this.gO004Control3,
                this.gO004Control4,
                this.gO004Control5,
                this.gO004Control6,
                this.gO004Control7,
                this.gO004Control8,
                this.gO004Control9,
                this.gO004Control10
            };

            this.tlgGo005Controls = new[]
            {
                this.gO005Control1,
                this.gO005Control2,
                this.gO005Control3,
                this.gO005Control4,
                this.gO005Control5,
                this.gO005Control6,
                this.gO005Control7
            };

            this.tlgGo007TextBoxes = new[]
            {
                this.textBoxGO007Stop1, this.textBoxGO007Transfer1, this.textBoxGO007Stop2, this.textBoxGO007Transfer2,
                this.textBoxGO007Stop3, this.textBoxGO007Transfer3, this.textBoxGO007Stop4, this.textBoxGO007Transfer4,
                this.textBoxGO007Stop5, this.textBoxGO007Transfer5, this.textBoxGO007Stop6, this.textBoxGO007Transfer6,
                this.textBoxGO007Stop7, this.textBoxGO007Transfer7, this.textBoxGO007Stop8, this.textBoxGO007Transfer8
            };

            int byteSize = this.radioButton16Bits.Checked ? 2 : 1;
            foreach (var control in this.tlgGo002Controls)
            {
                control.ByteSize = byteSize;
            }

            this.radioButton7Bits.Checked = true;
            this.CalculateCrc();
        }

        /// <summary>
        /// Event that is fired every time a new ibis telegram is created
        /// </summary>
        public event EventHandler<DataEventArgs> IbisTelegramCreated;

        /// <summary>
        /// Gets or sets the number of bits per character.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// If the value is not one of the following: 7, 8, 16
        /// </exception>
        [DefaultValue(7)]
        public int BitsPerCharacter
        {
            get
            {
                if (this.radioButton7Bits.Checked)
                {
                    return 7;
                }

                return this.radioButton8Bits.Checked ? 8 : 16;
            }

            set
            {
                switch (value)
                {
                    case 7:
                        this.radioButton7Bits.Checked = true;
                        return;
                    case 8:
                        this.radioButton8Bits.Checked = true;
                        return;
                    case 16:
                        this.radioButton16Bits.Checked = true;
                        return;
                    default:
                        throw new ArgumentException("Only 7, 8 and 16 bit supported.", "value");
                }
            }
        }

        private void RadioButton8BitsCheckedChanged(object sender, EventArgs e)
        {
            this.textBoxEndTelegram.Text = this.radioButton16Bits.Checked ? 0x0D.ToString("X4") : 0x0D.ToString("X2");
            int byteSize = this.radioButton16Bits.Checked ? 2 : 1;
            foreach (var control in this.tlgGo002Controls)
            {
                control.ByteSize = byteSize;
            }

            this.CalculateCrc();
        }

        private void CalculateCrc()
        {
            string textHeader = this.textBoxHeader.Text;
            string textPayload = this.textBoxPayload.Text;
            string textEndTelegram = this.textBoxEndTelegram.Text;

            byte[] headerBuffer = this.HexStringToByteArray(textHeader);
            byte[] paylodBuffer = this.HexStringToByteArray(textPayload);
            byte[] endTelegramBuffer = this.HexStringToByteArray(textEndTelegram);

            if (headerBuffer == null)
            {
                MessageBox.Show("Header wrong.");
                return;
            }

            if (paylodBuffer == null)
            {
                MessageBox.Show("Payload wrong.");
                return;
            }

            if (endTelegramBuffer == null)
            {
                MessageBox.Show("Ending telegram wrong.");
                return;
            }

            int mask;
            if (this.radioButton7Bits.Checked)
            {
                mask = 0x7F;
            }
            else if (this.radioButton8Bits.Checked)
            {
                mask = 0xFF;
            }
            else
            {
                mask = 0xFFFF;
            }

            int crc = mask;
            try
            {
                this.UpdateCrc(headerBuffer, mask, ref crc);
                this.UpdateCrc(paylodBuffer, mask, ref crc);
                this.UpdateCrc(endTelegramBuffer, mask, ref crc);
            }
            catch (Exception)
            {
                MessageBox.Show("Error in your bytes.");
                return;
            }

            this.textBoxCrc.Text = crc.ToString(this.radioButton16Bits.Checked ? "X4" : "X2");
        }

        private void UpdateCrc(byte[] data, int mask, ref int crc)
        {
            int increment = this.radioButton16Bits.Checked ? 2 : 1;
            for (int i = 0; i < data.Length; i += increment)
            {
                int tmp = increment == 2
                              ? (data[i] << 8 | data[i + 1])
                              : (data[i] & mask);
                crc ^= tmp;
            }
        }

        private void ButtonUpdateCrcClick(object sender, EventArgs e)
        {
            this.CalculateCrc();
        }

        private byte[] HexStringToByteArray(string s)
        {
            var buf = new byte[s.Length / 2];
            try
            {
                s = s.Replace(" ", string.Empty);
                for (int i = 0; i < s.Length; i += 2)
                {
                    buf[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
                }

                return buf;
            }
            catch (FormatException)
            {
                return null;
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
            catch (OverflowException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private string FromAddressToHexString(string ibisAddress)
        {
            int address;
            if (!int.TryParse(ibisAddress, out address))
            {
                return null;
            }

            return this.FromAddressToHexString(address);
        }

        private string FromAddressToHexString(int address)
        {
            if (address < 0 || address >= 16)
            {
                MessageBox.Show("Number out of range for hex: " + address);
            }

            address += '0';

            return address.ToString(this.radioButton16Bits.Checked ? "X4" : "X2");
        }

        private string FromStringToHexString(string s)
        {
            Encoding encoding;
            if (this.radioButton7Bits.Checked)
            {
                encoding = Encoding.ASCII;
            }
            else if (this.radioButton8Bits.Checked)
            {
                encoding = Encoding.UTF8;
            }
            else
            {
                encoding = Encoding.BigEndianUnicode;
            }

            var buffer = encoding.GetBytes(s);
            return this.FromByteArrayToHexString(buffer, 0, buffer.Length);
        }

        private string FromByteArrayToHexString(byte[] buffer, int offset, int count)
        {
            if (buffer == null || buffer.Length <= 0)
            {
                // invalid buffer.
                return string.Empty;
            }

            if (offset >= buffer.Length)
            {
                // invalid start offset
                return string.Empty;
            }

            if (count == 0)
            {
                // no byte to take in consideration.
                return string.Empty;
            }

            if (offset + count > buffer.Length)
            {
                // invalid dimension. I'll go out of buffer
                return string.Empty;
            }

            // ok, it seems that the parameters are good.
            var sb = new StringBuilder(count * 2);
            for (int i = 0; i < count; i++)
            {
                sb.AppendFormat("{0:X2}", buffer[offset + i]);
            }

            return sb.ToString();
        }

        private void ButtonApplyLineDS001Click(object sender, EventArgs e)
        {
            this.textBoxPayload.Text = this.FromStringToHexString(this.textBoxLineDS001Value.Text);
            this.textBoxHeader.Text = this.FromStringToHexString("l");
            this.CalculateCrc();
        }

        private void ApplySendButtonRunNumberClick(object sender, EventArgs e)
        {
            this.textBoxPayload.Text = this.FromStringToHexString(this.textBoxRunNumber.Text);
            this.textBoxHeader.Text = this.FromStringToHexString("k");
            this.CalculateCrc();
        }

        private void ApplySendButtonDestNumberClick(object sender, EventArgs e)
        {
            this.textBoxPayload.Text = this.FromStringToHexString(this.textBoxDestNumber.Text);
            this.textBoxHeader.Text = this.FromStringToHexString("z");
            this.CalculateCrc();
        }

        private void ButtonApplyDS003AClick(object sender, EventArgs e)
        {
            var lines = this.textBoxDestinationDS003A.Text.Split(
                new[] { Environment.NewLine }, StringSplitOptions.None);
            var sb = new StringBuilder((this.textBoxDestinationDS003A.TextLength * 2) + 1);
            sb.Append(this.FromAddressToHexString(lines.Length));
            foreach (var line in lines)
            {
                sb.Append(this.FromStringToHexString(string.Format("{0,-16}", line)));
            }

            this.textBoxPayload.Text = sb.ToString();
            this.textBoxHeader.Text = this.FromStringToHexString("zA");
            this.CalculateCrc();
        }

        private void ButtonApplyDS003CClick(object sender, EventArgs e)
        {
            var text = this.textBoxDS003C.Text;
            if (text.Length % 4 != 0)
            {
                text = text.PadRight((int)Math.Ceiling(text.Length / 4.0) * 4, ' ');
            }

            this.textBoxPayload.Text = this.FromAddressToHexString(text.Length / 4) + this.FromStringToHexString(text);
            this.textBoxHeader.Text = this.FromStringToHexString("zI");
            this.CalculateCrc();
        }

        private void ButtonApplyDS005Click(object sender, EventArgs e)
        {
            this.textBoxPayload.Text = this.FromStringToHexString(this.textBoxTimeDS005.Text);
            this.textBoxHeader.Text = this.FromStringToHexString("u");
            this.CalculateCrc();
        }

        private void ButtonApplyDS006Click(object sender, EventArgs e)
        {
            this.textBoxPayload.Text = this.FromStringToHexString(this.textBoxDateDS006.Text);
            this.textBoxHeader.Text = this.FromStringToHexString("d");
            this.CalculateCrc();
        }

        private void ButtonApplyDS006AClick(object sender, EventArgs e)
        {
            this.textBoxPayload.Text = this.FromStringToHexString(this.textBoxDS006A.Text);
            this.textBoxHeader.Text = this.FromStringToHexString("dU");
            this.CalculateCrc();
        }

        private void ButtonSetDateTimeClick(object sender, EventArgs e)
        {
            this.textBoxDS006A.Text = DateTime.Now.ToString("ddMMyyyyHHmmss");
        }

        private void ButtonApplyDS008Click(object sender, EventArgs e)
        {
            this.textBoxPayload.Text = this.FromStringToHexString(this.textBoxDS008.Text);
            this.textBoxHeader.Text = this.FromStringToHexString("n");
            this.CalculateCrc();
        }

        private void ButtonApplyDS009Click(object sender, EventArgs e)
        {
            this.SetDS009Text(this.textBoxDS009.Text, 16);
        }

        private void ButtonApplyDS009AClick(object sender, EventArgs e)
        {
            this.SetDS009Text(this.textBoxDS009A.Text, 20);
        }

        private void ButtonApplyDS009BClick(object sender, EventArgs e)
        {
            this.SetDS009Text(this.textBoxDS009B.Text, 24);
        }

        private void SetDS009Text(string text, int length)
        {
            if (text.Length < length)
            {
                text = text.PadRight(length, ' ');
            }
            else if (text.Length > length)
            {
                text = text.Substring(0, length);
            }

            this.textBoxPayload.Text = this.FromStringToHexString(text);
            this.textBoxHeader.Text = this.FromStringToHexString("v");
            this.CalculateCrc();
        }

        private void ButtonApplyStatusRequestDS020Click(object sender, EventArgs e)
        {
            var address = this.FromAddressToHexString(this.textBoxStatusRequestDS020Value.Text);
            if (address == null)
            {
                MessageBox.Show("Invalid address.");
                return;
            }

            this.textBoxPayload.Text = address;
            this.textBoxHeader.Text = this.FromStringToHexString("a");
            this.CalculateCrc();
        }

        private void ButtonApplyDS021Click(object sender, EventArgs e)
        {
            var lines = this.textBoxDS021Destination.Text.Split(
                new[] { Environment.NewLine }, StringSplitOptions.None);
            var sb = new StringBuilder((this.textBoxDS021Destination.TextLength * 2) + 1);
            sb.Append(this.FromAddressToHexString(lines.Length));
            foreach (var line in lines)
            {
                sb.Append(this.FromStringToHexString(string.Format("{0,-16}", line)));
            }

            this.textBoxPayload.Text = sb.ToString();
            this.textBoxHeader.Text = this.FromStringToHexString("aA")
                                      + this.FromAddressToHexString(this.textBoxDS021Address.Text);
            this.CalculateCrc();
        }

        private void ButtonApplyGO003Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            foreach (var textBox in this.tlgDs021TextBoxes)
            {
                if (textBox.TextLength == 0)
                {
                    continue;
                }

                if (sb.Length > 0)
                {
                    sb.Append('\n');
                }

                sb.Append(textBox.Text);
            }

            var address = this.FromAddressToHexString(this.textBoxIbisAddr.Text);
            if (address == null)
            {
                MessageBox.Show("Invalid address.");
                return;
            }

            string blockLength = this.FromStringToHexString(this.textBoxBlockLength.Text);
            this.textBoxPayload.Text = this.FromStringToHexString(sb.ToString());
            this.textBoxHeader.Text = this.FromStringToHexString("aB") + address + blockLength;
            this.CalculateCrc();
        }

        private void ButtonApplySpecLineDS001AClick(object sender, EventArgs e)
        {
            this.textBoxPayload.Text = this.FromStringToHexString(this.textBoxSpecLineDS001AValue.Text);
            this.textBoxHeader.Text = this.FromStringToHexString("lE");
            this.CalculateCrc();
        }

        private void ButtonApplyStopIndexDS010Click(object sender, EventArgs e)
        {
            this.textBoxPayload.Text = this.FromStringToHexString(this.textBoxStopIndexDS010Value.Text);
            this.textBoxHeader.Text = this.FromStringToHexString("x");
            this.CalculateCrc();
        }

        private void ButtonApplyStopIndexDS010BClick(object sender, EventArgs e)
        {
            this.textBoxPayload.Text = this.FromStringToHexString(this.textBoxStopIndexDS010BValue.Text);
            this.textBoxHeader.Text = this.FromStringToHexString("xI");
            this.CalculateCrc();
        }

        private void DS021AControlApplyClick(object sender, EventArgs e)
        {
            var address = this.FromAddressToHexString(this.dS021AControl1.Address);
            if (address == null)
            {
                MessageBox.Show("Invalid address.");
                return;
            }

            var row = this.dS021AControl1.SelectedRow;
            if (row == null)
            {
                return;
            }

            var payload = string.Format(
                "{0}\x03{1}\x04{2}\x05{3}",
                this.dS021AControl1.BlockLength,
                row.StopIndex,
                row.StopName,
                row.Transfers);
            if (!string.IsNullOrEmpty(row.TransferSymbols))
            {
                payload += string.Format("#{0}", row.TransferSymbols);
            }

            if (!string.IsNullOrEmpty(row.TravelTimes))
            {
                payload += string.Format("${0}", row.TravelTimes);
            }

            this.textBoxPayload.Text = this.FromStringToHexString(payload);
            this.textBoxHeader.Text = this.FromStringToHexString("aL") + address;
            this.CalculateCrc();
        }

        private void DS021CControlApplyClick(object sender, EventArgs e)
        {
            var address = this.FromAddressToHexString(this.textBoxIbisAddrDS021C.Text);
            if (address == null)
            {
                MessageBox.Show("Invalid address.");
                return;
            }

            var control = (DS021CRowControl)sender;
            foreach (var other in this.tlgDs021CControls)
            {
                other.BackColor = (other == control) ? Color.Gray : Color.Transparent;
            }

            var payload = string.Format(
                "{0}\x03{1}\x04{2}\x05{3}",
                control.Status,
                control.StopIndex,
                control.StopName,
                control.Transfers);
            if (!string.IsNullOrEmpty(control.TransferSymbols))
            {
                payload += string.Format("#{0}", control.TransferSymbols);
            }

            this.textBoxPayload.Text = this.FromStringToHexString(payload);
            this.textBoxHeader.Text = this.FromStringToHexString("aX") + address;
            this.CalculateCrc();
        }

        private void ButtonApplyDS030Click(object sender, EventArgs e)
        {
            this.textBoxPayload.Text = string.Empty;
            this.textBoxHeader.Text = this.FromStringToHexString("hS");
            this.CalculateCrc();
        }

        private void ButtonApplyDS036Click(object sender, EventArgs e)
        {
            this.textBoxPayload.Text = this.FromStringToHexString(this.textBoxDS036.Text);
            this.textBoxHeader.Text = this.FromStringToHexString("hP");
            this.CalculateCrc();
        }

        private void GO002ControlApplyClick(object sender, EventArgs e)
        {
            var control = (GO002RowControl)sender;
            foreach (var other in this.tlgGo002Controls)
            {
                other.BackColor = (other == control) ? Color.Gray : Color.Transparent;
            }

            var payload =
                control.DataLength +
                control.StopIndex +
                control.RowNumber +
                control.Pictogram +
                control.LineNumber +
                control.DepartureTime +
                control.TrackNumber +
                control.Deviation +
                control.DestinationName;
            this.textBoxPayload.Text = this.FromStringToHexString(payload);
            this.textBoxHeader.Text = this.FromStringToHexString("aU");
            this.CalculateCrc();
        }

        private void GO004ControlApplyClick(object sender, EventArgs e)
        {
            var address = this.FromAddressToHexString(this.textBoxAddressGO004.Text);
            if (address == null)
            {
                MessageBox.Show("Invalid address.");
                return;
            }

            var control = (GO004RowControl)sender;
            foreach (var other in this.tlgGo004Controls)
            {
                other.BackColor = (other == control) ? Color.Gray : Color.Transparent;
            }

            var payload =
                control.MessageIndex +
                control.MessageType +
                control.TimeRangeStart +
                control.TimeRangeEnd +
                control.MessageText;
            this.textBoxPayload.Text = address + this.FromStringToHexString(payload);
            this.textBoxHeader.Text = this.FromStringToHexString("aM");
            this.CalculateCrc();
        }

        private void GO005ControlApplyClick(object sender, EventArgs e)
        {
            var address = this.FromAddressToHexString(this.textBoxAddressGO005.Text);
            if (address == null)
            {
                MessageBox.Show("Invalid address.");
                return;
            }

            var control = (GO005RowControl)sender;
            foreach (var other in this.tlgGo005Controls)
            {
                other.BackColor = (other == control) ? Color.Gray : Color.Transparent;
            }

            var payload = new StringBuilder();
            payload.Append(control.DataLength);
            payload.Append("\x03");
            payload.AppendFormat("{0,4}", control.LineNumber);
            payload.AppendFormat("{0,4}", control.StopIndex);
            if (control.Destination.Length > 0)
            {
                payload.Append("\x04");
                payload.Append(control.Destination);
                payload.Append('\x0a');
            }

            while (payload.Length % 4 != control.DataLength.Length)
            {
                payload.Append(' ');
            }

            this.textBoxPayload.Text = this.FromStringToHexString(payload.ToString());
            this.textBoxHeader.Text = this.FromStringToHexString("aA") + address;
            this.CalculateCrc();
        }

        private void ButtonApplyDS080Click(object sender, EventArgs e)
        {
            this.textBoxPayload.Text = string.Empty;
            this.textBoxHeader.Text = this.FromStringToHexString("bT");
            this.CalculateCrc();
        }

        private void ButtonApplyDS081Click(object sender, EventArgs e)
        {
            this.textBoxPayload.Text = string.Empty;
            this.textBoxHeader.Text = this.FromStringToHexString("bM");
            this.CalculateCrc();
        }

        private void ButtonApplyGO006Click(object sender, EventArgs e)
        {
            string line = string.Format("{0}{1}", this.textBoxGO006LineFirstChar.Text, this.textBoxGO006LineValue.Text);
            this.textBoxPayload.Text = this.FromStringToHexString(line);
            this.textBoxHeader.Text = this.FromStringToHexString("z");
            this.CalculateCrc();
        }

        private void ButtonApplyHPW074Click(object sender, EventArgs e)
        {
            var address = this.FromAddressToHexString(this.textBoxIbisAddrHPW074.Text);
            if (address == null)
            {
                MessageBox.Show("Invalid address.");
                return;
            }

            this.textBoxPayload.Text = this.FromStringToHexString(this.textBoxIndexHPW074.Text);
            this.textBoxHeader.Text = this.FromStringToHexString("sN") + address;
            this.CalculateCrc();
        }

        private void ApplySendButtonEventClick(object sender, EventArgs e)
        {
            var address = this.FromAddressToHexString(this.textBoxAddressGO001.Text);
            if (address == null)
            {
                MessageBox.Show("Invalid address.");
                return;
            }

            var payload = this.textBoxNumberGO001.Text + this.textBoxParametersGO001.Text;
            this.textBoxPayload.Text = this.FromStringToHexString(payload);
            this.textBoxHeader.Text = this.FromStringToHexString("xE") + address;
            this.CalculateCrc();
        }

        private void ButtonSendTelegramClick(object sender, EventArgs e)
        {
            string textHeader = this.textBoxHeader.Text;
            string textPayload = this.textBoxPayload.Text;
            string textTrailer = this.textBoxEndTelegram.Text;
            string textCrc = this.textBoxCrc.Text;

            byte[] headerBuffer = this.HexStringToByteArray(textHeader);
            byte[] payloadBuffer = this.HexStringToByteArray(textPayload);
            byte[] trailerBuffer = this.HexStringToByteArray(textTrailer);
            byte[] crcBuffer = this.HexStringToByteArray(textCrc);

            var telegram =
                new byte[headerBuffer.Length + payloadBuffer.Length + trailerBuffer.Length + crcBuffer.Length];
            Array.Copy(headerBuffer, 0, telegram, 0, headerBuffer.Length);
            Array.Copy(payloadBuffer, 0, telegram, headerBuffer.Length, payloadBuffer.Length);
            Array.Copy(trailerBuffer, 0, telegram, headerBuffer.Length + payloadBuffer.Length, trailerBuffer.Length);
            Array.Copy(
                crcBuffer,
                0,
                telegram,
                headerBuffer.Length + payloadBuffer.Length + trailerBuffer.Length,
                crcBuffer.Length);

            var handler = this.IbisTelegramCreated;
            if (handler != null)
            {
                handler(this, new DataEventArgs { Data = telegram });
            }
        }

        private void ButtonApplyGO007Click(object sender, EventArgs e)
        {
            var dataLength = 0;
            this.textBoxGO007DataLength.Text = string.Format("{0}{1}", dataLength / 16, (dataLength % 16) / 4);
            var sb = new StringBuilder();
            sb.Append("\x03");
            sb.AppendFormat("{0,4}", this.textBoxGO007LineNumber.Text);
            dataLength += 1 + this.textBoxGO007LineNumber.TextLength;
            this.textBoxGO007DataLength.Text = string.Format("{0}{1}", dataLength / 16, (dataLength % 16) / 4);
            var stop = true;
            foreach (var textBox in this.tlgGo007TextBoxes)
            {
                if (textBox.TextLength == 0)
                {
                    stop = !stop;

                    continue;
                }

                if (!stop)
                {
                    sb.Append("\x05");
                    sb.Append(textBox.Text);
                    dataLength += 1 + textBox.TextLength;
                    this.textBoxGO007DataLength.Text = string.Format("{0}{1}", dataLength / 16, (dataLength % 16) / 4);
                    stop = true;
                }
                else
                {
                    sb.Append("\x04");
                    sb.Append(textBox.Text);
                    dataLength += 1 + textBox.TextLength;
                    this.textBoxGO007DataLength.Text = string.Format("{0}{1}", dataLength / 16, (dataLength % 16) / 4);
                    stop = false;
                }
            }

            var address = this.FromAddressToHexString(this.textBoxGO007IbisAddr.Text);
            if (address == null)
            {
                MessageBox.Show("Invalid address.");
                return;
            }

            var payload = new StringBuilder();
            payload.Append(dataLength);
            payload.Append(sb);
            this.textBoxPayload.Text = this.FromStringToHexString(payload.ToString());
            this.textBoxHeader.Text = this.FromStringToHexString("aA") + address;
            this.CalculateCrc();
        }
    }
}
