// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialPortFrameHandler.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SerialPortFrameHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Ports;
    using System.Runtime.InteropServices;
    using System.Threading;

    using Gorba.Common.Configuration.Infomedia.AhdlcRenderer;
    using Gorba.Common.Protocols.Ahdlc;
    using Gorba.Common.Protocols.Ahdlc.Frames;
    using Gorba.Common.Utility.Core;

    using NLog;

    using Math = System.Math;

    /// <summary>
    /// Frame handler that uses a serial port for sending and receiving frames.
    /// IMPORTANT: the private classes and enumerations were mostly taken from <c>Motion.Obc</c> solution.
    /// TODO: replace those private classes and enumerations with the proper types from the
    /// Gorba.Motion.Common solution when they are available (probably Q1/2015)
    /// </summary>
    public partial class SerialPortFrameHandler : IFrameHandler, IDisposable
    {
        private static readonly Logger Logger = LogHelper.GetLogger<SerialPortFrameHandler>();

        private readonly RtsMode rtsMode;

        private readonly CommDevice device;

        private readonly FrameDecoder decoder;
        private readonly FrameEncoder encoder;

        private readonly byte[] readBuffer = new byte[256];
        private readonly LinkedList<FrameBase> readFrames = new LinkedList<FrameBase>();

        private readonly double millisecondsPerByte;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialPortFrameHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The serial port configuration.
        /// </param>
        public SerialPortFrameHandler(SerialPortConfig config)
        {
            var portName = config.ComPort;
            if (portName == null)
            {
                throw new ArgumentException("config.ComPort is null");
            }

            if (!portName.EndsWith(":"))
            {
                portName += ":";
            }

            this.rtsMode = config.RtsMode;

            this.device = new CommDevice();
            this.device.Open(portName);
            this.device.SetTimeouts(uint.MaxValue, 10000, uint.MaxValue, 0, 0);
            this.device.SetState(
                (BaudRate)config.BaudRate,
                (ByteSize)config.DataBits,
                config.Parity,
                config.Parity != Parity.None,
                config.StopBits,
                true,
                this.GetInitialRtsState());

            this.decoder = new FrameDecoder(config.IsHighSpeed) { IgnoreFrameStart = config.IgnoreFrameStart };
            this.encoder = new FrameEncoder(config.IsHighSpeed);

            double bitsPerByte = config.DataBits;
            switch (config.StopBits)
            {
                case StopBits.One:
                    bitsPerByte++;
                    break;
                case StopBits.Two:
                    bitsPerByte += 2;
                    break;
                case StopBits.OnePointFive:
                    bitsPerByte += 1.5;
                    break;
            }

            if (config.Parity != Parity.None)
            {
                bitsPerByte++;
            }

            this.millisecondsPerByte = bitsPerByte / config.BaudRate * 1000;
        }

        // ReSharper disable UnusedMember.Local
        [Flags]
        private enum FileFlag : uint
        {
            None = 0,
            Overlapped = 0x40000000,
            WriteThrough = 0x80000000
        }

        private enum BaudRate
        {
            RateUnknown = -1,
            Rate110 = 110,
            Rate300 = 300,
            Rate600 = 600,
            Rate1200 = 1200,
            Rate2400 = 2400,
            Rate4800 = 4800,
            Rate9600 = 9600,
            Rate14400 = 14400,
            Rate19200 = 19200,
            Rate38400 = 38400,
            Rate57600 = 57600,
            Rate115200 = 115200,
            Rate128000 = 128000,
            Rate256000 = 256000
        }

        private enum ByteSize
        {
            Unknown = -1,
            Size5 = 0x05,
            Size6 = 0x06,
            Size7 = 0x07,
            Size8 = 0x08
        }

        [Flags]
        private enum CommMask : uint
        {
            RxChar = 0x0001,  // Any Character received
            RxFlag = 0x0002,  // Received certain character
            TxEmpty = 0x0004,  // Transmitt Queue Empty
            Cts = 0x0008,  // CTS changed state
            Dsr = 0x0010,  // DSR changed state
            Rlsd = 0x0020,  // RLSD changed state
            Break = 0x0040,  // BREAK received
            Err = 0x0080,  // Line status error occurred
            Ring = 0x0100  // Ring signal detected
        }

        private enum RtsControl : uint
        {
            Disable = 0x00,
            Enable = 0x01,
            Handshake = 0x02,
            Toggle = 0x03
        }

        // ReSharper restore UnusedMember.Local

        /// <summary>
        /// Reads the next frame from the underlying stream.
        /// This method blocks until an entire frame is available or the
        /// end of the stream was reached (EOS).
        /// </summary>
        /// <returns>
        /// The decoded <see cref="FrameBase"/> or null if the end of the stream was reached (EOS).
        /// </returns>
        public FrameBase ReadNextFrame()
        {
            if (this.readFrames.Count > 0)
            {
                var node = this.readFrames.First;
                this.readFrames.Remove(node);
                return node.Value;
            }

            int read;
            while ((read = this.device.Read(this.readBuffer, this.readBuffer.Length)) >= 0)
            {
                foreach (var frame in this.decoder.AddBytes(this.readBuffer, 0, read))
                {
                    this.readFrames.AddLast(frame);
                }

                if (this.readFrames.Count > 0)
                {
                    var node = this.readFrames.First;
                    this.readFrames.Remove(node);
                    return node.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Writes an entire frame to the underlying stream.
        /// </summary>
        /// <param name="frame">
        /// The frame to write.
        /// </param>
        public void WriteFrame(FrameBase frame)
        {
            var data = this.encoder.Encode(frame);
            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("Writing frame {0} ({1} bytes)", BitConverter.ToString(data), data.Length);
            }

            this.SetRtsSending(true);
            this.device.Write(data, data.Length);

            // add 10% to make sure the data is sent
            var sleepTime = Math.Ceiling(data.Length * this.millisecondsPerByte * 1.1);

            Logger.Trace("Sleeping {0:0} ms", sleepTime);
            Thread.Sleep((int)sleepTime);
            this.SetRtsSending(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.device.RtsControl(this.GetInitialRtsState());
            this.device.Close();
            this.device.Dispose();
        }

        private RtsControl GetInitialRtsState()
        {
            switch (this.rtsMode)
            {
                case RtsMode.Default:
                case RtsMode.Auto:
                    return RtsControl.Toggle;
                case RtsMode.DisableForTx:
                case RtsMode.Enabled:
                    return RtsControl.Enable;
                case RtsMode.EnableForTx:
                case RtsMode.Disabled:
                    return RtsControl.Disable;
                default:
                    return RtsControl.Toggle;
            }
        }

        private void SetRtsSending(bool sending)
        {
            switch (this.rtsMode)
            {
                case RtsMode.EnableForTx:
                    this.device.RtsControl(sending ? RtsControl.Enable : RtsControl.Disable);
                    break;
                case RtsMode.DisableForTx:
                    this.device.RtsControl(sending ? RtsControl.Disable : RtsControl.Enable);
                    break;
            }
        }

        private class NativeObject : IDisposable
        {
            ~NativeObject()
            {
                this.Dispose(false);
            }

            public bool IsValid
            {
                get
                {
                    return this.Handle != IntPtr.Zero;
                }
            }

            protected IntPtr Handle { get; set; }

            protected bool Disposed { get; private set; }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            public virtual void Close()
            {
                if (!this.IsValid)
                {
                    return;
                }

                NativeMethods.CloseHandle(this.Handle);
                this.Handle = IntPtr.Zero;
            }

            protected virtual void Dispose(bool disposing)
            {
                if (this.Disposed)
                {
                    return;
                }

                if (disposing)
                {
                    // get rid of managed resources
                }

                // get rid of unmanaged resources
                this.Close();
                this.Disposed = true;
            }

            private static class NativeMethods
            {
                [DllImport("coredll.dll", SetLastError = true)]
                public static extern bool CloseHandle(IntPtr hObject);
            }
        }

        private class NativeFile : NativeObject
        {
            private const uint FileAccessGenericRead = 0x80000000;
            private const uint FileAccessGenericWrite = 0x40000000;

            private static readonly IntPtr InvalidHandle = new IntPtr(-1); // or uint.MaxValue ?

            public bool IsOpen
            {
                get
                {
                    return this.IsValid;
                }
            }

            protected static void CheckLastError(bool error, string method)
            {
                int errorCode = Marshal.GetLastWin32Error();

                // was: if (error || errorCode != 0), but at least for VersionDevice we get a 0x80000005
                // even thought the operation was successful
                if (error)
                {
                    Exception inner = Marshal.GetExceptionForHR(errorCode);
                    throw new IOException(string.Format("Could not {0}", method), inner);
                }
            }

            protected void CheckOpen()
            {
                if (!this.IsOpen)
                {
                    throw new IOException("File is not open");
                }
            }

            protected void Open(string fileName, FileAccess access, FileShare shareMode, FileFlag flags)
            {
                this.CreateFile(fileName, access, shareMode, FileMode.Open, flags);
            }

            private void CreateFile(
                string fileName, FileAccess access, FileShare shareMode, FileMode mode, FileFlag flags)
            {
                if (this.Disposed)
                {
                    throw new ObjectDisposedException("NativeFile");
                }

                if (this.IsOpen)
                {
                    throw new IOException("File already open");
                }

                // convert FileAccess enumeration
                uint fileAccess;
                switch (access)
                {
                    case FileAccess.Read:
                        fileAccess = FileAccessGenericRead;
                        break;
                    case FileAccess.Write:
                        fileAccess = FileAccessGenericWrite;
                        break;
                    case FileAccess.ReadWrite:
                        fileAccess = FileAccessGenericRead | FileAccessGenericWrite;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("access");
                }

                this.Handle = NativeMethods.CreateFile(fileName, fileAccess, shareMode, 0, mode, flags, IntPtr.Zero);

                int errorCode = Marshal.GetLastWin32Error();
                if (this.Handle == InvalidHandle || errorCode != 0)
                {
                    this.Handle = IntPtr.Zero;
                    if (errorCode == 2)
                    {
                        throw new FileNotFoundException(fileName + " not found.");
                    }

                    Exception inner = Marshal.GetExceptionForHR(errorCode);
                    throw new IOException("CreateFile returned error " + errorCode, inner);
                }
            }

            private static class NativeMethods
            {
                [DllImport("coredll.dll", SetLastError = true)]
                internal static extern IntPtr CreateFile(
                    string lpFileName,
                    uint dwDesiredAccess,
                    FileShare dwShareMode,
                    uint lpSecurityAttributes,
                    FileMode dwCreationDisposition,
                    FileFlag dwFlagsAndAttributes,
                    IntPtr hTemplateFile);
            }
        }

        /// <summary>
        ///   RS485 device used by StreamDriver
        /// </summary>
        private class CommDevice : NativeFile
        {
            /*
             * Block of constants for the serial port's DTR line
             */
            private const uint DtrControlDisable = 0x00;
            private const uint DtrControlEnable = 0x01;
            ////private const uint DTR_CONTROL_HANDSHAKE = 0x02;

            public void Open(string serialPortName)
            {
                this.Open(serialPortName, FileAccess.ReadWrite, FileShare.None, FileFlag.WriteThrough);
            }

            public virtual void Prepare()
            {
            }

            public void SetMask(CommMask mask)
            {
                this.CheckOpen();

                bool success = NativeMethods.SetCommMask(this.Handle, (uint)mask);
                NativeFile.CheckLastError(!success, "SetCommMask");
            }

            public void SetTimeouts(
                uint readIntervalTimeout,
                uint readTotalTimeout,
                uint readTotalMultiplierTimeout,
                uint writeTotalTimeout,
                uint writeMultiplierTimeout)
            {
                this.CheckOpen();

                NativeMethods.COMMTIMEOUTS comTimeouts; // structure to configure the RSXXX interface (timeouts)
                bool success = NativeMethods.GetCommTimeouts(this.Handle, out comTimeouts);
                NativeFile.CheckLastError(!success, "GetCommTimeouts");

                // it is needed to have a cleaned COMMTIMEOUTS
                // and now I configure the structure just created above as desired by the user...
                comTimeouts.ReadIntervalTimeout = readIntervalTimeout;
                comTimeouts.ReadTotalTimeoutConstant = readTotalTimeout;
                comTimeouts.ReadTotalTimeoutMultiplier = readTotalMultiplierTimeout;
                comTimeouts.WriteTotalTimeoutConstant = writeTotalTimeout;
                comTimeouts.WriteTotalTimeoutMultiplier = writeMultiplierTimeout;

                // and now set the timeout structure...
                success = NativeMethods.SetCommTimeouts(this.Handle, ref comTimeouts);

                // set the timeouts (nonzero on success)
                NativeFile.CheckLastError(!success, "SetCommTimeouts");

                // it seems that the SET operation was done with success.
                // but now, I will do this check:
                // - I get the COMMTIMEOUTS struct from the serial port
                // - I check each value with the values desired by the user
                // only after this check I consider ended this function.
                success = NativeMethods.GetCommTimeouts(this.Handle, out comTimeouts);

                // configure the timeouts on the interface
                NativeFile.CheckLastError(!success, "GetCommTimeouts");

                // ok, now I a check each value, one by one...
                if (comTimeouts.ReadIntervalTimeout != readIntervalTimeout ||
                    comTimeouts.ReadTotalTimeoutConstant != readTotalTimeout ||
                    comTimeouts.ReadTotalTimeoutMultiplier != readTotalMultiplierTimeout ||
                    comTimeouts.WriteTotalTimeoutConstant != writeTotalTimeout ||
                    comTimeouts.WriteTotalTimeoutMultiplier != writeMultiplierTimeout)
                {
                    // at least one of the values desired by the user is different
                    // from the actual state of the serial port's COMMTIMEOUTS structure.
                    // so, the previous "SetCommTimeouts" didn't worked well.
                    // for me, this function has failed.
                    // I cannot continue with this procedure.
                    throw new IOException("Timeouts were not set properly");
                }
            }

            public void SetState(
                BaudRate baudRate,
                ByteSize byteSize,
                Parity parity,
                bool parityCheckEnabled,
                StopBits stopBits,
                bool dtrEnabled,
                RtsControl rtsControl)
            {
                // Attention: .NET's StopBits enum is not the same as the native one
                byte nativeStopBits;
                switch (stopBits)
                {
                    case StopBits.One:
                        nativeStopBits = 0;
                        break;
                    case StopBits.OnePointFive:
                        nativeStopBits = 1;
                        break;
                    case StopBits.Two:
                        nativeStopBits = 2;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            "stopBits", "stopBits can only be: One, OnePointFive or Two. Was: " + stopBits);
                }

                this.CheckOpen();

                NativeMethods.DCB dcb;
                bool success = NativeMethods.GetCommState(this.Handle, out dcb); // <== do not remove this line.
                NativeFile.CheckLastError(!success, "GetCommState");

                dcb.BaudRate = (uint)baudRate;
                dcb.ByteSize = (byte)byteSize;
                dcb.fParity = (uint)(parityCheckEnabled ? 1 : 0);
                dcb.Parity = (byte)parity;
                dcb.StopBits = nativeStopBits;
                dcb.fDtrControl = dtrEnabled ? DtrControlEnable : DtrControlDisable;
                dcb.fRtsControl = (uint)rtsControl;

                // Attention !!!
                // not all the applications need all the parameters above

                // Attention !!!
                // probably I need also to set the "dcb.fDsrSensitivity"
                success = NativeMethods.SetCommState(this.Handle, ref dcb);
                NativeFile.CheckLastError(!success, "SetCommState");

                // it seems that the SET operation was done with success.
                // but now, I will do this check:
                // - I get the DCB struct from the serial port
                // - I check each value with the values desired by the user
                // only after this check I consider ended this function.
                success = NativeMethods.GetCommState(this.Handle, out dcb); // <== do not remove this line.
                NativeFile.CheckLastError(!success, "GetCommState");

                // ok. the DCB was get.
                // now I a check each value, one by one...
                if (dcb.BaudRate != (uint)baudRate ||
                    dcb.ByteSize != (byte)byteSize ||
                    dcb.fParity != (uint)(parityCheckEnabled ? 1 : 0) ||
                    dcb.Parity != (byte)parity ||
                    dcb.StopBits != nativeStopBits ||
                    dcb.fDtrControl != (dtrEnabled ? DtrControlEnable : DtrControlDisable) ||
                    dcb.fRtsControl != (uint)rtsControl)
                {
                    // at least one of the values desired by the user is different
                    // from the actual state of the serial port's DCB structure.
                    // so, the previous "SetCommTimeouts" didn't worked well.
                    // for me, this function has failed.
                    // I cannot continue with this procedure.
                    throw new IOException("Comm State was not set properly");
                }
            }

            public void RtsControl(RtsControl value)
            {
                this.CheckOpen();

                NativeMethods.DCB dcb;
                bool success = NativeMethods.GetCommState(this.Handle, out dcb); // <== do not remove this line.
                NativeFile.CheckLastError(!success, "GetCommState");

                dcb.fRtsControl = (uint)value;

                // Attention !!!
                // not all the applications need all the parameters above

                // Attention !!!
                // probably I need also to set the "dcb.fDsrSensitivity"
                success = NativeMethods.SetCommState(this.Handle, ref dcb);
                NativeFile.CheckLastError(!success, "SetCommState");

                // it seems that the SET operation was done with success.
                // but now, I will do this check:
                // - I get the DCB struct from the serial port
                // - I check each value with the values desired by the user
                // only after this check I consider ended this function.
                success = NativeMethods.GetCommState(this.Handle, out dcb); // <== do not remove this line.
                NativeFile.CheckLastError(!success, "GetCommState");

                // ok. the DCB was get.
                // now I a check each value, one by one...
                if (dcb.fRtsControl != (uint)value)
                {
                    // at least one of the values desired by the user is different
                    // from the actual state of the serial port's DCB structure.
                    // so, the previous "SetCommTimeouts" didn't worked well.
                    // for me, this function has failed.
                    // I cannot continue with this procedure.
                    throw new IOException("Comm State was not set properly");
                }
            }

            public void SetBufferSizes(int readBufferSize, int writeBufferSize)
            {
                this.CheckOpen();

                bool success = NativeMethods.SetupComm(this.Handle, (uint)readBufferSize, (uint)writeBufferSize);
                NativeFile.CheckLastError(!success, "SetupComm");
            }

            public virtual int Write(byte[] buffer, int bytesToWrite)
            {
                this.CheckOpen();

                int bytesWritten = 0;
                bool success = NativeMethods.WriteFile(
                    this.Handle,
                    buffer,
                    (uint)bytesToWrite,
                    ref bytesWritten,
                    IntPtr.Zero);
                NativeFile.CheckLastError(!success, "Write");
                return bytesWritten;
            }

            public int Read(byte[] bufferToFill, int bytesToRead)
            {
                this.CheckOpen();

                int bytesRead = 0;
                bool success = NativeMethods.ReadFile(
                                                     this.Handle,
                                                     bufferToFill,
                                                     (uint)bytesToRead,
                                                     ref bytesRead,
                                                     IntPtr.Zero);
                NativeFile.CheckLastError(!success, "Read");
                return bytesRead;
            }

            public int GetBytesToRead()
            {
                this.CheckOpen();

                uint errorStatus = 0;
                var comStat = new NativeMethods.COMSTAT();
                bool success = NativeMethods.ClearCommError(this.Handle, ref errorStatus, ref comStat);
                if (!success)
                {
                    return -1;
                }

                return (int)comStat.cbInQue;
            }

            private static class NativeMethods
            {
                // NativeMethods can violate ReSharper rules, let's disable some messages:
                // ReSharper disable InconsistentNaming
                // ReSharper disable UnusedMember.Local
                // ReSharper disable MemberCanBePrivate.Local
                // ReSharper disable FieldCanBeMadeReadOnly.Local
                [DllImport("coredll.dll", EntryPoint = "SetCommMask", SetLastError = true)]
                public static extern bool SetCommMask(IntPtr hFile, uint dwEvtMask);

                [DllImport("coredll.dll", EntryPoint = "WriteFile", SetLastError = true)]
                public static extern bool WriteFile(
                    IntPtr hFile,
                    byte[] lpBuffer,
                    uint nNumberOfBytesToWrite,
                    ref int lpNumberOfBytesWritten,
                    IntPtr lpOverlapped);

                [DllImport("coredll.dll", EntryPoint = "ReadFile", SetLastError = true)]
                public static extern bool ReadFile(
                    IntPtr hFile,
                    byte[] lpBuffer,
                    uint nNumberOfBytesToRead,
                    ref int lpNumberOfBytesRead,
                    IntPtr lpOverlapped);

                [DllImport("coredll.dll", EntryPoint = "PurgeComm", SetLastError = true)]
                public static extern bool PurgeComm(IntPtr hFile, uint dwFlags);

                [DllImport("coredll.dll", EntryPoint = "GetCommState", SetLastError = true)]
                public static extern bool GetCommState(IntPtr hFile, out DCB lpDcb);

                [DllImport("coredll.dll", EntryPoint = "SetCommState", SetLastError = true)]
                public static extern bool SetCommState(IntPtr hFile, ref DCB lpDcb);

                [DllImport("coredll.dll", EntryPoint = "GetCommTimeouts", SetLastError = true)]
                public static extern bool GetCommTimeouts(IntPtr hFile, out COMMTIMEOUTS lpCommTimeouts);

                [DllImport("coredll.dll", EntryPoint = "SetCommTimeouts", SetLastError = true)]
                public static extern bool SetCommTimeouts(IntPtr hFile, ref COMMTIMEOUTS lpCommTimeouts);

                [DllImport("coredll.dll", EntryPoint = "SetupComm", SetLastError = true)]
                public static extern bool SetupComm(IntPtr hFile, uint dwInQueue, uint dwOutQueue);

                [DllImport("coredll.dll", EntryPoint = "ClearCommError", SetLastError = true)]
                public static extern bool ClearCommError(IntPtr hFile, ref uint lpErrors, ref COMSTAT lpStat);

                [StructLayout(LayoutKind.Sequential)]
                public struct COMMTIMEOUTS
                {
                    /// <summary>
                    ///   DWORD->unsigned int
                    /// </summary>
                    public uint ReadIntervalTimeout;

                    /// <summary>
                    ///   DWORD->unsigned int
                    /// </summary>
                    public uint ReadTotalTimeoutMultiplier;

                    /// <summary>
                    ///   DWORD->unsigned int
                    /// </summary>
                    public uint ReadTotalTimeoutConstant;

                    /// <summary>
                    ///   DWORD->unsigned int
                    /// </summary>
                    public uint WriteTotalTimeoutMultiplier;

                    /// <summary>
                    ///   DWORD->unsigned int
                    /// </summary>
                    public uint WriteTotalTimeoutConstant;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct DCB
                {
                    // DWORD->unsigned int
                    public uint DCBlength;

                    // DWORD->unsigned int
                    public uint BaudRate;

                    // fBinary              : 1
                    // fParity              : 1
                    // fOutxCtsFlow         : 1
                    // fOutxDsrFlow         : 1
                    // fDtrControl          : 2
                    // fDsrSensitivity      : 1
                    // fTXContinueOnXoff    : 1
                    // fOutX                : 1
                    // fInX                 : 1
                    // fErrorChar           : 1
                    // fNull                : 1
                    // fRtsControl          : 2
                    // fAbortOnError        : 1
                    // fDummy2              : 17
                    public uint bitvector1;

                    // WORD->unsigned short
                    public ushort wReserved;

                    // WORD->unsigned short
                    public ushort XonLim;

                    // WORD->unsigned short
                    public ushort XoffLim;

                    // BYTE->unsigned char
                    public byte ByteSize;

                    // BYTE->unsigned char
                    public byte Parity;

                    // BYTE->unsigned char
                    public byte StopBits;

                    // char
                    public byte XonChar;

                    // char
                    public byte XoffChar;

                    // char
                    public byte ErrorChar;

                    // char
                    public byte EofChar;

                    // char
                    public byte EvtChar;

                    // WORD->unsigned short
                    public ushort wReserved1;

                    public uint fBinary
                    {
                        get
                        {
                            return this.bitvector1 & 1u;
                        }

                        set
                        {
                            this.bitvector1 &= ~1u;
                            this.bitvector1 = value | this.bitvector1;
                        }
                    }

                    public uint fParity
                    {
                        get
                        {
                            return (this.bitvector1 & 2u) / 2;
                        }

                        set
                        {
                            this.bitvector1 &= ~2u;
                            this.bitvector1 = (value * 2) | this.bitvector1;
                        }
                    }

                    public uint fOutxCtsFlow
                    {
                        get
                        {
                            return (this.bitvector1 & 4u) / 4;
                        }

                        set
                        {
                            this.bitvector1 &= ~4u;
                            this.bitvector1 = (value * 4) | this.bitvector1;
                        }
                    }

                    public uint fOutxDsrFlow
                    {
                        get
                        {
                            return (this.bitvector1 & 8u) / 8;
                        }

                        set
                        {
                            this.bitvector1 &= ~8u;
                            this.bitvector1 = (value * 8) | this.bitvector1;
                        }
                    }

                    public uint fDtrControl
                    {
                        get
                        {
                            return (this.bitvector1 & 48u) / 16;
                        }

                        set
                        {
                            this.bitvector1 &= ~48u;
                            this.bitvector1 = (value * 16) | this.bitvector1;
                        }
                    }

                    public uint fDsrSensitivity
                    {
                        get
                        {
                            return (this.bitvector1 & 64u) / 64;
                        }

                        set
                        {
                            this.bitvector1 &= ~64u;
                            this.bitvector1 = (value * 64) | this.bitvector1;
                        }
                    }

                    public uint fTXContinueOnXoff
                    {
                        get
                        {
                            return (this.bitvector1 & 128u) / 128;
                        }

                        set
                        {
                            this.bitvector1 &= ~128u;
                            this.bitvector1 = (value * 128) | this.bitvector1;
                        }
                    }

                    public uint fOutX
                    {
                        get
                        {
                            return (this.bitvector1 & 256u) / 256;
                        }

                        set
                        {
                            this.bitvector1 &= ~256u;
                            this.bitvector1 = (value * 256) | this.bitvector1;
                        }
                    }

                    public uint fInX
                    {
                        get
                        {
                            return (this.bitvector1 & 512u) / 512;
                        }

                        set
                        {
                            this.bitvector1 &= ~512u;
                            this.bitvector1 = (value * 512) | this.bitvector1;
                        }
                    }

                    public uint fErrorChar
                    {
                        get
                        {
                            return (this.bitvector1 & 1024u) / 1024;
                        }

                        set
                        {
                            this.bitvector1 &= ~1024u;
                            this.bitvector1 = (value * 1024) | this.bitvector1;
                        }
                    }

                    public uint fNull
                    {
                        get
                        {
                            return (this.bitvector1 & 2048u) / 2048;
                        }

                        set
                        {
                            this.bitvector1 &= ~2048u;
                            this.bitvector1 = (value * 2048) | this.bitvector1;
                        }
                    }

                    public uint fRtsControl
                    {
                        get
                        {
                            return (this.bitvector1 & 12288u) / 4096;
                        }

                        set
                        {
                            this.bitvector1 &= ~12288u;
                            this.bitvector1 = (value * 4096) | this.bitvector1;
                        }
                    }

                    public uint fAbortOnError
                    {
                        get
                        {
                            return (this.bitvector1 & 16384u) / 16384;
                        }

                        set
                        {
                            this.bitvector1 &= ~16384u;
                            this.bitvector1 = (value * 16384) | this.bitvector1;
                        }
                    }

                    public uint fDummy2
                    {
                        get
                        {
                            return (this.bitvector1 & 4294934528u) / 32768;
                        }

                        set
                        {
                            this.bitvector1 &= ~4294934528u;
                            this.bitvector1 = (value * 32768) | this.bitvector1;
                        }
                    }
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct COMSTAT
                {
                    // fCtsHold : 1
                    // fDsrHold : 1
                    // fRlsdHold : 1
                    // fXoffHold : 1
                    // fXoffSent : 1
                    // fEof : 1
                    // fTxim : 1
                    // fReserved : 25
                    public uint bitvector1;

                    // DWORD->unsigned int
                    public uint cbInQue;

                    // DWORD->unsigned int
                    public uint cbOutQue;

                    public uint fCtsHold
                    {
                        get { return this.bitvector1 & 1u; }
                        set { this.bitvector1 = value | this.bitvector1; }
                    }

                    public uint fDsrHold
                    {
                        get { return (this.bitvector1 & 2u) / 2; }
                        set { this.bitvector1 = (value * 2) | this.bitvector1; }
                    }

                    public uint fRlsdHold
                    {
                        get { return (this.bitvector1 & 4u) / 4; }
                        set { this.bitvector1 = (value * 4) | this.bitvector1; }
                    }

                    public uint fXoffHold
                    {
                        get { return (this.bitvector1 & 8u) / 8; }
                        set { this.bitvector1 = (value * 8) | this.bitvector1; }
                    }

                    public uint fXoffSent
                    {
                        get { return (this.bitvector1 & 16u) / 16; }
                        set { this.bitvector1 = (value * 16) | this.bitvector1; }
                    }

                    public uint fEof
                    {
                        get { return (this.bitvector1 & 32u) / 32; }
                        set { this.bitvector1 = (value * 32) | this.bitvector1; }
                    }

                    public uint fTxim
                    {
                        get { return (this.bitvector1 & 64u) / 64; }
                        set { this.bitvector1 = (value * 64) | this.bitvector1; }
                    }

                    public uint fReserved
                    {
                        get { return (this.bitvector1 & 4294967168u) / 128; }
                        set { this.bitvector1 = (value * 128) | this.bitvector1; }
                    }
                }
            }
        }
    }
}
