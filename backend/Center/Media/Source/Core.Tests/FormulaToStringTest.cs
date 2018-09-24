// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaToStringTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for FormulaToStringTest and is intended
//   to contain all FormulaToStringTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests
{
    using System;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.Formulas;
    using Gorba.Center.Media.Core.Resources;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for FormulaToStringTest and is intended
    /// to contain all FormulaToStringTest Unit Tests
    /// </summary>
    [TestClass]
    public class FormulaToStringTest : FormulaTestBase
    {
        /// <summary>
        /// Initialize a test run. Create mockup.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.DictionaryDataViewModel = new DictionaryDataViewModel(this.GetDictionarySampleData());
            this.ShellMock = this.CreateMediaShell(this.DictionaryDataViewModel);
        }

        /// <summary>
        /// Tests if a given model returns a proper string representation
        /// </summary>
        [TestMethod]
        public void ConstantTest()
        {
            const string Formula = "= 2";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(ConstantEvalDataViewModel));
            var eval = (ConstantEvalDataViewModel)model;

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("2", formulaText);
        }

        /// <summary>
        /// Tests if a string with not escaped tic is valid.
        /// </summary>
        [TestMethod]
        public void LiteralNotEscapedTest()
        {
            const string Formula = "= 'a''bc'";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(ConstantEvalDataViewModel));
            var eval = (ConstantEvalDataViewModel)model;

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("'a'bc'", formulaText);
        }

        /// <summary>
        /// Tests if a given model returns a proper string representation
        /// </summary>
        [TestMethod]
        public void AndTest()
        {
            const string Formula = "= And ( 'true' ; 5>2;  3>=1; 6<7; 8<=9)";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(AndEvalDataViewModel));
            var eval = (AndEvalDataViewModel)model;

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("And ( 'true'; 5 > 2; 3 >= 1; 6 < 7; 8 <= 9 )", formulaText);
        }

        /// <summary>
        /// Tests if a given model returns a proper string representation
        /// </summary>
        [TestMethod]
        public void OrTest()
        {
            const string Formula =
                "= Or ( 'false' ; Not('true'); NOT ('FALSE'); Not( 4 <= 5))";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(OrEvalDataViewModel));
            var eval = (OrEvalDataViewModel)model;

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("Or ( 'false'; Not ( 'true' ); Not ( 'FALSE' ); Not ( 4 <= 5 ) )", formulaText);
        }

        /// <summary>
        /// Tests if a given model returns a proper string representation
        /// </summary>
        [TestMethod]
        public void EqualsTest()
        {
            const string Formula =
                "= Or ( 'a' = 'b'; 2<>3)";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(OrEvalDataViewModel));
            var eval = (OrEvalDataViewModel)model;

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("Or ( 'a' = 'b'; 2 <> 3 )", formulaText);
        }

        /// <summary>
        /// Tests if a given model returns a proper string representation
        /// </summary>
        [TestMethod]
        public void IfThenTest()
        {
            const string Formula =
                "= If('KA'; 6)";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(IfEvalDataViewModel));
            var eval = (IfEvalDataViewModel)model;

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("If ( 'KA'; 6 )", formulaText);
        }

        /// <summary>
        /// Tests if a given model returns a proper string representation
        /// </summary>
        [TestMethod]
        public void IfThenElseTest()
        {
            const string Formula = "= If('true'; 6; 5)";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(IfEvalDataViewModel));
            var eval = (IfEvalDataViewModel)model;

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("If ( 'true'; 6; 5 )", formulaText);
        }

        /// <summary>
        /// Tests if a given model returns a proper string representation
        /// </summary>
        [TestMethod]
        public void Format1Test()
        {
            const string Formula = "= Format('Bla Blub')";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(FormatEvalDataViewModel));
            var eval = (FormatEvalDataViewModel)model;

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("Format ( 'Bla Blub' )", formulaText);
        }

        /// <summary>
        /// Tests if a given model returns a proper string representation
        /// </summary>
        [TestMethod]
        public void Format2Test()
        {
            const string Formula = "= Format( \"Bla Blub {0} {1}\"; 'Blubber'; 4 )";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(FormatEvalDataViewModel));
            var eval = (FormatEvalDataViewModel)model;

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("Format ( 'Bla Blub {0} {1}'; 'Blubber'; 4 )", formulaText);
        }

        /// <summary>
        /// Tests if a given model returns a proper string representation
        /// </summary>
        [TestMethod]
        public void GenericTableTest()
        {
            const string Formula = "= $Stops3.StopName";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(GenericEvalDataViewModel));
            var eval = (GenericEvalDataViewModel)model;

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("$Stops3.StopName", formulaText);
        }

        /// <summary>
        /// Tests if a given model returns a proper string representation
        /// </summary>
        [TestMethod]
        public void GenericTableFailTest()
        {
            const string Formula = "= $Stops3.StopName";

            var model = this.ParseFormula(Formula);

            this.DictionaryDataViewModel.Tables.RemoveAll(t => t.Name == "Stops3");

            Assert.IsInstanceOfType(model, typeof(GenericEvalDataViewModel));
            var eval = (GenericEvalDataViewModel)model;

            var formulaText = eval.HumanReadable();

            var str = string.Format("$" + MediaStrings.FormulaValuesToStringTableNotFound + ".", "3");
            Assert.AreEqual(str, formulaText);
        }

        /// <summary>
        /// Tests if a given model returns a proper string representation
        /// </summary>
        [TestMethod]
        public void GenericColumnTest()
        {
            const string Formula = "= $Stops3.StopName";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(GenericEvalDataViewModel));
            var eval = (GenericEvalDataViewModel)model;

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("$Stops3.StopName", formulaText);
        }

        /// <summary>
        /// Tests if a given model returns a proper string representation
        /// </summary>
        [TestMethod]
        public void GenericColumnFailTest()
        {
            const string Formula = "= $Stops3.StopName";

            var model = this.ParseFormula(Formula);

            var table = this.DictionaryDataViewModel.Tables.FirstOrDefault(t => t.Name == "Stops3");
            Assert.IsNotNull(table);
            table.Columns.Clear();

            Assert.IsInstanceOfType(model, typeof(GenericEvalDataViewModel));
            var eval = (GenericEvalDataViewModel)model;

            var formulaText = eval.HumanReadable();

            var str = string.Format("$Stops3." + MediaStrings.FormulaValuesToStringColumnNotFound, "1");
            Assert.AreEqual(str, formulaText);
        }

        /// <summary>
        /// Tests if a given model returns a proper string representation
        /// </summary>
        [TestMethod]
        public void GenericRowTest()
        {
            const string Formula = "= $Stops2.StopName[1]";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(GenericEvalDataViewModel));
            var eval = (GenericEvalDataViewModel)model;

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("$Stops2.StopName[1]", formulaText);
        }

        /// <summary>
        /// Tests if a given model returns a proper string representation
        /// </summary>
        [TestMethod]
        public void GenericLanguageTest()
        {
            const string Formula = "= $Stops.StopName[1]{de}";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(GenericEvalDataViewModel));
            var eval = (GenericEvalDataViewModel)model;

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("$Stops.StopName[1]{de}", formulaText);
        }

        /// <summary>
        /// Tests if a given model returns a proper string representation
        /// </summary>
        [TestMethod]
        public void GenericLanguageFailTest()
        {
            const string Formula = "= $Stops.StopName[1]{de}";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(GenericEvalDataViewModel));
            var eval = (GenericEvalDataViewModel)model;

            eval.Language.Value = -1;

            var formulaText = eval.HumanReadable();

            var str = string.Format(
                "$Stops.StopName[1]{{" + MediaStrings.FormulaValuesToStringLanguageNotFound + "}}",
                "-1");
            Assert.AreEqual(str, formulaText);
        }

        /// <summary>
        /// Tests if a date format is valid.
        /// </summary>
        [TestMethod]
        public void DateTest()
        {
            const string Formula = "= Date ('01.02.2014'; '02.11.2014')";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(DateEvalDataViewModel));
            var eval = (DateEvalDataViewModel)model;

            Assert.IsInstanceOfType(eval.Begin, typeof(DataValue<DateTime>));
            Assert.IsInstanceOfType(eval.End, typeof(DataValue<DateTime>));

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("Date ( '01.02.2014'; '02.11.2014' )", formulaText);
        }

        /// <summary>
        /// Tests if a date format is valid on 29. February.
        /// </summary>
        [TestMethod]
        public void DateLeapYearTest()
        {
            const string Formula = "= Date ('29.02.2012'; '16.03.2012')";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(DateEvalDataViewModel));
            var eval = (DateEvalDataViewModel)model;

            Assert.IsInstanceOfType(eval.Begin, typeof(DataValue<DateTime>));
            Assert.IsInstanceOfType(eval.End, typeof(DataValue<DateTime>));

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("Date ( '29.02.2012'; '16.03.2012' )", formulaText);
        }

        /// <summary>
        /// Tests if a date format parsing throws an exception if the end date is less than the begin date.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void DateEndLessThanBeginTest()
        {
            const string Formula = "= Date ('03.02.2014'; '01.02.2014')";

            this.ParseFormula(Formula);
        }

        /// <summary>
        /// Tests if a time format is valid.
        /// </summary>
        [TestMethod]
        public void TimeTest()
        {
            const string Formula = "= Time ('18:30:55'; '19:32:42')";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(TimeEvalDataViewModel));
            var eval = (TimeEvalDataViewModel)model;

            Assert.IsInstanceOfType(eval.Begin, typeof(DataValue<TimeSpan>));
            Assert.IsInstanceOfType(eval.End, typeof(DataValue<TimeSpan>));

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("Time ( '18:30:55'; '19:32:42' )", formulaText);
        }

        /// <summary>
        /// Tests if a time format parsing throws an exception if the end time is less than the begin time.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void TimeEndLessThanBeginTest()
        {
            const string Formula = "= Time ('18:30:55'; '16:32:42')";

            this.ParseFormula(Formula);
        }

        /// <summary>
        /// Tests if a Switch format is valid.
        /// </summary>
        [TestMethod]
        public void SwitchTest()
        {
            const string Formula = "= Switch ($Stops3.StopName; 'KA':Not(3 < 4); 5)";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(SwitchEvalDataViewModel));
            var eval = (SwitchEvalDataViewModel)model;
            Assert.IsInstanceOfType(eval.Value, typeof(GenericEvalDataViewModel));
            Assert.IsInstanceOfType(eval.Default, typeof(ConstantEvalDataViewModel));
            Assert.AreEqual(1, eval.Cases.Count);
            Assert.IsInstanceOfType(eval.Cases[0], typeof(CaseDynamicPropertyDataViewModel));

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("Switch ( $Stops3.StopName; 'KA':Not ( 3 < 4 ); 5 )", formulaText);
        }

        /// <summary>
        /// Tests if a DayOfWeek format is valid.
        /// </summary>
        [TestMethod]
        public void DayOfWeekTest()
        {
            const string Formula = "= DayOfWeek ('Sun,Mon,Tue,Thu')";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(DayOfWeekEvalDataViewModel));
            var eval = (DayOfWeekEvalDataViewModel)model;

            var formulaText = eval.HumanReadable();

            Assert.AreEqual("DayOfWeek ( 'Mon, Tue, Thu, Sun' )", formulaText);
        }

        /// <summary>
        /// Tests if a text to image format is valid.
        /// </summary>
        [TestMethod]
        public void TextToImageTest()
        {
            const string Formula = "= TextToImage ($Stops3.StopName; 'Stop_{0}.jpg;Stop_{0}.png')";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(TextToImageEvalDataViewModel));
            var eval = (TextToImageEvalDataViewModel)model;
            Assert.IsInstanceOfType(eval.Evaluation, typeof(GenericEvalDataViewModel));
            Assert.IsInstanceOfType(eval.FilePatterns, typeof(DataValue<string>));
            var formulaText = eval.HumanReadable();

            Assert.AreEqual("TextToImage ( $Stops3.StopName; 'Stop_{0}.jpg;Stop_{0}.png' )", formulaText);
        }

        /// <summary>
        /// Tests if a string compare format is valid.
        /// </summary>
        [TestMethod]
        public void StringCompareTest()
        {
            const string Formula = "= StringCompare ($Stops3.StopName; 'KA' ; 'true' )";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(StringCompareEvalDataViewModel));
            var eval = (StringCompareEvalDataViewModel)model;
            Assert.IsInstanceOfType(eval.Evaluation, typeof(GenericEvalDataViewModel));
            Assert.IsInstanceOfType(eval.Value, typeof(DataValue<string>));

            var formulaText = eval.HumanReadable();
            Assert.AreEqual("StringCompare ( $Stops3.StopName; 'KA'; 'True' )", formulaText);
        }

        /// <summary>
        /// Tests if an integer compare format is valid.
        /// </summary>
        [TestMethod]
        public void IntegerCompareTest()
        {
            const string Formula = "= IntegerCompare ( $Stops3.StopName; 2; 6)";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(IntegerCompareEvalDataViewModel));
            var eval = (IntegerCompareEvalDataViewModel)model;
            Assert.IsInstanceOfType(eval.Evaluation, typeof(GenericEvalDataViewModel));
            Assert.IsInstanceOfType(eval.Begin, typeof(DataValue<int>));
            Assert.IsInstanceOfType(eval.End, typeof(DataValue<int>));

            var formulaText = eval.HumanReadable();
            Assert.AreEqual("IntegerCompare ( $Stops3.StopName; 2; 6 )", formulaText);
        }

        /// <summary>
        /// Tests if an integer compare format is valid.
        /// </summary>
        [TestMethod]
        public void RegexReplaceTest()
        {
            const string Formula = @"= RegexReplace ( $Stops3.StopName; '^0*(\d+)$'; '$1')";
            const string Expected = @"RegexReplace ( $Stops3.StopName; '^0*(\d+)$'; '$1' )";
            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(RegexReplaceEvalDataViewModel));
            var eval = (RegexReplaceEvalDataViewModel)model;
            Assert.IsInstanceOfType(eval.Evaluation, typeof(GenericEvalDataViewModel));
            Assert.IsInstanceOfType(eval.Pattern, typeof(DataValue<string>));
            Assert.IsInstanceOfType(eval.Replacement, typeof(DataValue<string>));

            var formulaText = eval.HumanReadable();
            Assert.AreEqual(Expected, formulaText);
        }

        /// <summary>
        /// Tests if a Match property is valid.
        /// </summary>
        [TestMethod]
        public void MatchDynamicPropertyTest()
        {
            const string ExpectedResult = "2:Not ( 5 = 4 )";
            var constantLeft = new ConstantEvalDataViewModel(this.ShellMock.Object) { Value = { Value = "5" } };
            var constantRight = new ConstantEvalDataViewModel(this.ShellMock.Object) { Value = { Value = "4" } };
            var equalsEval = new EqualsEvalDataViewModel(this.ShellMock.Object)
                                 {
                                     Left = constantLeft,
                                     Right = constantRight
                                 };
            var property = new MatchDynamicPropertyDataViewModel(this.ShellMock.Object)
                               {
                                   Column = { Value = 2 },
                                   Evaluation =
                                       new NotEvalDataViewModel(this.ShellMock.Object)
                                           {
                                               Evaluation = equalsEval
                                           }
                               };

            var text = property.HumanReadable();
            Assert.AreEqual(ExpectedResult, text);
        }

        /// <summary>
        /// Tests if a Case property is valid.
        /// </summary>
        [TestMethod]
        public void CaseDynamicPropertyTest()
        {
            const string ExpectedResult = "'KA':Not ( 5 > 4 )";

            var constantLeft = new ConstantEvalDataViewModel(this.ShellMock.Object) { Value = { Value = "5" } };
            var constantRight = new ConstantEvalDataViewModel(this.ShellMock.Object) { Value = { Value = "4" } };
            var greaterThanEval =
                new GreaterThanEvalDataViewModel(this.ShellMock.Object) { Left = constantLeft, Right = constantRight };
            var property = new CaseDynamicPropertyDataViewModel(this.ShellMock.Object)
            {
                Value = { Value = "KA" },
                Evaluation = new NotEvalDataViewModel(this.ShellMock.Object) { Evaluation = greaterThanEval }
            };

            var text = property.HumanReadable();
            Assert.AreEqual(ExpectedResult, text);
        }

        /// <summary>
        /// Tests if a CsvMapping format is valid.
        /// </summary>
        [TestMethod]
        public void CsvMappingTest()
        {
            const string Formula =
                "= CsvMapping ('file.csv'; '{0} bis {1}'; $Stops3.StopName; 2:NOT(3 = 4); 3:NOT(6 = 7))";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(CsvMappingEvalDataViewModel));
            var eval = (CsvMappingEvalDataViewModel)model;
            Assert.IsInstanceOfType(eval.FileName, typeof(DataValue<string>));
            Assert.IsInstanceOfType(eval.DefaultValue, typeof(GenericEvalDataViewModel));
            Assert.IsInstanceOfType(eval.OutputFormat, typeof(DataValue<string>));
            Assert.AreEqual(2, eval.Matches.Count);
            Assert.IsInstanceOfType(eval.Matches[0], typeof(MatchDynamicPropertyDataViewModel));

            var formulaText = eval.HumanReadable();

            Assert.AreEqual(
                "CsvMapping ( 'file.csv'; '{0} bis {1}'; $Stops3.StopName; 2:Not ( 3 = 4 ); 3:Not ( 6 = 7 ) )",
                formulaText);
        }
    }
}
