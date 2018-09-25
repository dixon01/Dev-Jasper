// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XimpleDictionaryTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ximple.Tests
{
    using Gorba.Common.Protocols.Ximple.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for generic <see cref="Dictionary"/>
    /// </summary>
    [TestClass]
    public class XimpleDictionaryTest
    {
        #region Public Methods

        /// <summary>
        /// Simple test getting a column with a given name or number.
        /// </summary>
        [TestMethod]
        public void GetColumnForNameOrNumberTest()
        {
            var dictionary = CreateTestDictionary();
            var expected = dictionary.Tables[0].Columns[1];
            var actual = dictionary.Tables[0].GetColumnForNameOrNumber("Time");
            Assert.AreEqual(expected, actual);

            expected = dictionary.Tables[1].Columns[0];
            actual = dictionary.Tables[1].GetColumnForNameOrNumber("0");
            Assert.AreEqual(expected, actual);

            actual = dictionary.Tables[1].GetColumnForNameOrNumber("StopCity");
            Assert.AreEqual(expected, actual);

            actual = dictionary.Tables[0].GetColumnForNameOrNumber("123");
            Assert.IsNull(actual);
        }

        /// <summary>
        /// Simple test getting a table with a given name or number.
        /// </summary>
        [TestMethod]
        public void GetTableForNameOrNumberTest()
        {
            var dictionary = CreateTestDictionary();
            var expected = dictionary.Tables[0];
            var actual = dictionary.GetTableForNameOrNumber("System");
            Assert.AreEqual(expected, actual);

            expected = dictionary.Tables[1];
            actual = dictionary.GetTableForNameOrNumber("10");
            Assert.AreEqual(expected, actual);

            actual = dictionary.GetTableForNameOrNumber("Stops");
            Assert.AreEqual(expected, actual);

            actual = dictionary.GetTableForNameOrNumber("123");
            Assert.IsNull(actual);
        }

        #endregion

        #region Methods

        private static Dictionary CreateTestDictionary()
        {
            var dictionary = new Dictionary { VersionString = "2.0.0" };
            var english = new Language { Index = 0, Name = "English" };
            var german = new Language { Index = 1, Name = "German" };
            var table1 = new Table { Index = 0, Name = "System" };
            table1.Columns.Add(new Column { Index = 0, Name = "Date" });
            table1.Columns.Add(new Column { Index = 1, Name = "Time" });
            var table2 = new Table { Index = 10, Name = "Stops" };
            table2.Columns.Add(new Column { Index = 0, Name = "StopCity" });
            table2.Columns.Add(new Column { Index = 1, Name = "StopTime" });
            dictionary.Tables.Add(table1);
            dictionary.Tables.Add(table2);
            dictionary.Languages.Add(english);
            dictionary.Languages.Add(german);

            return dictionary;
        }

        #endregion
    }
}