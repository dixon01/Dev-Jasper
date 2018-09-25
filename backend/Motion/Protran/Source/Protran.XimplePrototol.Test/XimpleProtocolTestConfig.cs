namespace Protran.XimplePrototol.Test
{
    /// <summary>The ximple protocol test config.</summary>
    public class XimpleProtocolTestConfig
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="XimpleProtocolTestConfig"/> class.</summary>
        /// <param name="testExpectedXimpleCreatedCount">The test expected ximple created count.</param>
        /// <param name="testExpectedBadXmlCount">The test expected bad xml count.</param>
        /// <param name="repeatSocketWriteCount">The repeat socket write count.</param>
        /// <param name="waitForReply">The wait for reply.</param>
        /// <param name="port">The port.</param>
        public XimpleProtocolTestConfig(
            int testExpectedXimpleCreatedCount, 
            int testExpectedBadXmlCount, 
            int repeatSocketWriteCount = 1, 
            bool waitForReply = false, 
            int port = 0)
        {
            this.Port = port;
            this.TestExpectedXimpleCreatedCount = testExpectedXimpleCreatedCount;
            this.TestExpectedBadXmlCount = testExpectedBadXmlCount;
            this.RepeatSocketWriteCount = repeatSocketWriteCount;
            this.WaitForReply = waitForReply;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the expected bad xml count.</summary>
        public int TestExpectedBadXmlCount { get; set; }

        /// <summary>Gets or sets the expected ximple created count.</summary>
        public int TestExpectedXimpleCreatedCount { get; set; }

        /// <summary>Gets or sets the port.</summary>
        public int Port { get; set; }

        /// <summary>Gets or sets a value indicating whether save to file.</summary>
        public bool SaveToFile { get; set; }

        /// <summary>Gets or sets the total tcp client writes to the server for testing.</summary>
        public int RepeatSocketWriteCount { get; set; }

        /// <summary>Gets or sets a value indicating whether wait for socket reply from the server for testing.</summary>
        public bool WaitForReply { get; set; }

        #endregion
    }
}