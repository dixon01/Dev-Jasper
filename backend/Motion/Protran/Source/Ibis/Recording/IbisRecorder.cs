// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisRecorder.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Recording
{
    using System;
    using System.IO;
    using System.Text;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Motion.Protran.Core.Buffers;

    using NLog;

    /// <summary>
    /// Object tasked to record each telegram
    /// received from the IBIS master.
    /// </summary>
    public abstract class IbisRecorder : IRecorder
    {
        #region VARIABLES

        /// <summary>
        /// Logger to be used by subclasses.
        /// </summary>
        protected readonly Logger Logger;

        /// <summary>
        /// Container of all the settings required
        /// to store a session with a specific IBIS master.
        /// </summary>
        private readonly RecordingConfig recordingConfigInfo;

        private readonly IbisConfig config;

        /// <summary>
        /// The instance to the object that really will
        /// write into the file.
        /// </summary>
        private StreamWriter streamWriter;
        #endregion VARIABLES

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisRecorder"/> class.
        /// </summary>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        protected IbisRecorder(IIbisConfigContext configContext)
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
            this.streamWriter = null;
            this.recordingConfigInfo = configContext.Config.Recording;
            this.config = configContext.Config;

            this.recordingConfigInfo.FileAbsPath = PathManager.Instance.CreatePath(
                FileType.Data, this.recordingConfigInfo.FileAbsPath);
        }

        #region PROPERTIES
        /// <summary>
        /// Gets a value indicating whether
        /// this recorder is recording or not.
        /// </summary>
        public bool IsRecording
        {
            get
            {
                return this.streamWriter != null;
            }
        }
        #endregion PROPERTIES

        /// <summary>
        /// Starts the recorder.
        /// <exception cref="UnauthorizedAccessException">If the software doesn't have the grants
        /// to create a new file.</exception>
        /// <exception cref="ArgumentException">If the settings used to create the file are invalid.</exception>
        /// <exception cref="IOException">If some error occurs on creating and opening the file.</exception>
        /// </summary>
        public void Start()
        {
            if (this.IsRecording)
            {
                // this recorder is already recording.
                // I avoid to start it twice.
                return;
            }

            this.streamWriter = new StreamWriter(this.recordingConfigInfo.FileAbsPath, false, Encoding.ASCII);
        }

        /// <summary>
        /// Stops the recorder.
        /// </summary>
        public void Stop()
        {
            if (!this.IsRecording)
            {
                // the recorder is already stopped.
                // I avoid to stop it twice.
                return;
            }

            this.streamWriter.Close();
            this.streamWriter = null;
        }

        /// <summary>
        /// Writes a buffer to the file.
        /// </summary>
        /// <param name="buffer">The buffer that has to be wrote.</param>
        public void Write(byte[] buffer)
        {
            if (buffer == null || buffer.Length <= 0)
            {
                // nothing to write
                return;
            }

            string text = BufferUtils.FromByteArrayToHexString(buffer);

            // if we write the clear text telegram in a new line immediately after the
            // one in hexadecimal format, then we cannot load anymore the produced log file
            // (and neither the ones produced by old versions of protran),
            // because line by line protran expects an hexadecimal telegram.
            // therefore I append the line in clear text after the hexadecimal telegram.
            // Also, appending the clear text at the end of the string doesn't affect the
            // file loading process, because it only cares about the time stamp (token 0 and 1)
            // and the hexadecimal telegram (token 2).
            string clearText = ToTelegramString(buffer, this.config.Behaviour.ByteType);
            this.WriteToFile(string.Format("{0} {1}", text, clearText));
        }

        /// <summary>
        /// Converts the incoming text to a text formatted
        /// as specified in the configuration file.
        /// </summary>
        /// <param name="text">The text to be formatted.</param>
        /// <returns>The new well formatted text.</returns>
        protected abstract string FormatText(string text);

        private static string ToTelegramString(byte[] data, ByteType byteType)
        {
            char[] chars;
            if (byteType == ByteType.Hengartner8)
            {
                chars = ArrayUtil.ConvertAll(data, b => (char)b);
            }
            else
            {
                var encoding = byteType == ByteType.UnicodeBigEndian ? Encoding.BigEndianUnicode : Encoding.ASCII;
                chars = encoding.GetChars(data);
            }

            var sb = new StringBuilder(data.Length * 2);
            foreach (var c in chars)
            {
                if (char.IsControl(c) || c >= 0x7F)
                {
                    sb.AppendFormat(byteType == ByteType.UnicodeBigEndian ? "<{0:X4}>" : "<{0:X2}>", (int)c);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Writes a text inside the file
        /// using a specific format.
        /// </summary>
        /// <param name="text">The text to be written.</param>
        private void WriteToFile(string text)
        {
            if (!this.IsRecording)
            {
                // this recorder was not started.
                // I cannot record nothing.
                return;
            }

            string realText = this.FormatText(text);
            try
            {
                this.streamWriter.WriteLine(realText);
                this.streamWriter.Flush();
            }
            catch (Exception)
            {
                // an error was occured on
                // writing a text into the file.
                this.Logger.Error("Error on recording text: " + text);
            }
        }
    }
}
