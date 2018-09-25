// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IqubeCmdMsgStructTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Protocols.Qnet.Tests
{
    using System.Runtime.InteropServices;

    using Gorba.Common.Protocols.Qnet;
    using Gorba.Common.Protocols.Qnet.Structures;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for dosDateTimeTest and is intended
    /// to contain all dosDateTimeTest Unit Tests
    /// </summary>
    [TestClass]
    public class IqubeCmdMsgStructTest : BaseStructTest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods and Operators
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// A test for the size in bytes of ConditionStartStruct
        /// </summary>
        [TestMethod]
        public void ConditionStartStructLength_SizeOfStruct_Returns4()
        {
            const int ConditionStartStructSize = 4;
            this.TestStructLength<ConditionStartStruct>(ConditionStartStructSize);
        }

        /// <summary>
        /// A test for the size in bytes of ConditionStopStruct
        /// </summary>
        [TestMethod]
        public void ConditionStopStructLength_SizeOfStruct_Returns4()
        {
            const int ConditionStopStructSize = 4;
            this.TestStructLength<ConditionStopStruct>(ConditionStopStructSize);
        }

        /// <summary>
        /// A test for the size in bytes of ExeConditionStruct
        /// </summary>
        [TestMethod]
        public void ExeConditionStructLength_SizeOfStruct_Returns10()
        {
            const int ExeConditionStructSize = 10;
            this.TestStructLength<ExeConditionStruct>(ExeConditionStructSize);
        }

        /// <summary>
        /// A test for the size in bytes of InfoLineStruct
        /// </summary>
        [TestMethod]
        public void InfoLineStructLength_SizeOfStruct_Returns171()
        {
            const int InfoLineStructSize = 171;
            this.TestStructLength<InfoLineStruct>(InfoLineStructSize);
        }

        /// <summary>
        /// A test for the size in bytes of IqubeCmdMsgStruct
        /// </summary>
        [TestMethod]
        public void IqubeCmdMsgStructLength_SizeOfStruct_Returns188()
        {
            const int Expectedlen = 195;
            int actualLen = Marshal.SizeOf(new IqubeCmdMsgStruct());

            Assert.AreEqual(Expectedlen, actualLen);
        }

        /// <summary>
        /// A test for the size in bytes of IqubeTaskHdrStruct
        /// </summary>
        [TestMethod]
        public void IqubeTaskHdrStructLength_SizeOfStruct_Returns15()
        {
            const int TaskStructSize = 15;
            this.TestStructLength<IqubeTaskHdrStruct>(TaskStructSize);
        }

        /// <summary>
        /// A test for the size in bytes of TaskStruct
        /// </summary>
        [TestMethod]
        public void TaskStructLength_SizeOfStruct_Returns186()
        {
            const int Expectedlen = 186;
            int actualLen = Marshal.SizeOf(new ActivityStruct());

            Assert.AreEqual(Expectedlen, actualLen);
        }
        
        /// <summary>
        /// A test for the size in bytes of ActivityDataStruct
        /// Len = 38 bytes
        /// </summary>
        [TestMethod]
        public void ActivityDataStructLength_SizeOfStruct_Returns38()
        {            
            const int Expectedlen = 38;
            int actualLen = Marshal.SizeOf(new ActivityDataStruct());

            Assert.AreEqual(Expectedlen, actualLen);
        }
        
        /// <summary>
        /// A test for the size in bytes of DisplayStruct
        /// </summary>
        [TestMethod]
        public void DisplayStructLength_SizeOfStruct_Returns150()
        {            
            const int Expectedlen = 150;
            int actualLen = Marshal.SizeOf(new DisplayStruct());

            Assert.AreEqual(Expectedlen, actualLen);
        }

        /// <summary>
        /// A test for the size in bytes of VoiceTextStruct
        /// </summary>
        [TestMethod]
        public void VoiceTextStructLength_SizeOfStruct_Returns163()
        {            
            const int Expectedlen = 163;
            int actualLen = Marshal.SizeOf(new VoiceTextStruct());

            Assert.AreEqual(Expectedlen, actualLen);
        }        

        /// <summary>
        /// A test for the size in bytes of DeleteTripStruct
        /// </summary>
        [TestMethod]
        public void DeleteTripStructLength_SizeOfStruct_Returns12()
        {            
            const int Expectedlen = 12;
            int actualLen = Marshal.SizeOf(new DeleteTripStruct());

            Assert.AreEqual(Expectedlen, actualLen);
        }                

        /// <summary>
        /// A test for the size in bytes of DeleteTripStruct
        /// </summary>
        [TestMethod]
        public void ActivityIdsStructLength_SizeOfStruct_Returns49()
        {            
            const int Expectedlen = 193; // = 4*48 + 1 
            int actualLen = Marshal.SizeOf(new ActivityIdsStruct());

            Assert.AreEqual(Expectedlen, actualLen);
        }   
        // ReSharper restore InconsistentNaming
        #endregion
    }
}