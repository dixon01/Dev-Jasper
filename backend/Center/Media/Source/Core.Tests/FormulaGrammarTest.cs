// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaGrammarTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for FormulaGrammarTest and is intended
//   to contain all FormulaGrammarTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests
{
    using System;
    using System.Globalization;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Formulas;

    using Irony.Parsing;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for FormulaGrammarTest and is intended
    /// to contain all FormulaGrammarTest Unit Tests
    /// </summary>
    [TestClass]
    public class FormulaGrammarTest : FormulaTestBase
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
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void FormulaGrammarConstructorTest()
        {
            var target = new FormulaGrammar();

            const string Source = "= 5 > 4";
            var language = new LanguageData(target);
            var parser = new Parser(language);
            var parseTree = parser.Parse(Source);
            var root = parseTree.Root;

            Assert.IsNotNull(root);
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void GreaterThanTest()
        {
            this.TestBinaryFormula<GreaterThanEvalDataViewModel>("= 5 > 4", "5", "4");
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void GreaterThanOrEqualTest()
        {
            this.TestBinaryFormula<GreaterThanOrEqualEvalDataViewModel>("= 5 >= 4", "5", "4");
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void LessThanTest()
        {
            this.TestBinaryFormula<LessThanEvalDataViewModel>("= 5 < 4", "5", "4");
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void LessThanOrEqualTest()
        {
            this.TestBinaryFormula<LessThanOrEqualEvalDataViewModel>("= 5 <= 4", "5", "4");
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void EqualsTest()
        {
            this.TestBinaryFormula<EqualsEvalDataViewModel>("= 5 = 4", "5", "4");
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void NotEqualsTest()
        {
            this.TestBinaryFormula<NotEqualsEvalDataViewModel>("= 5 <> 4", "5", "4");
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "A missing Operator was allowed.")]
        public void MissingOperatorTest()
        {
            this.TestBinaryFormula<NotEqualsEvalDataViewModel>("= 5  4", "5", "4");
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "A missing Operant was allowed.")]
        public void MissingBinaryOperantTest()
        {
            this.TestBinaryFormula<NotEqualsEvalDataViewModel>("= 5 >", "5", "4");
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "A missing Operant was allowed.")]
        public void MissingBinaryOperantTest2Test()
        {
            this.TestBinaryFormula<NotEqualsEvalDataViewModel>("= > 5", "4", "5");
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Missing Equals sign at start of formula was allowed.")]
        public void MissingFormulaSignTest()
        {
            this.TestBinaryFormula<NotEqualsEvalDataViewModel>("4 > 5", "4", "5");
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestIf()
        {
            const string Formula = "= IF ( 4 > 5 ; '4 is bigger' ; '5 is bigger' )";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(IfEvalDataViewModel));
            var eval = (IfEvalDataViewModel)model;

            Assert.IsInstanceOfType(eval.Condition, typeof(GreaterThanEvalDataViewModel));
            Assert.IsInstanceOfType(eval.Then, typeof(ConstantEvalDataViewModel));
            Assert.IsInstanceOfType(eval.Else, typeof(ConstantEvalDataViewModel));
            Assert.AreEqual(((ConstantEvalDataViewModel)eval.Then).Value.Value, "4 is bigger");
            Assert.AreEqual(((ConstantEvalDataViewModel)eval.Else).Value.Value, "5 is bigger");
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ParseException), "Missing then was allowed.")]
        public void MethodTestIfMissingArgument1()
        {
            const string Formula = "= IF ( 4 > 5 )";

            this.ParseFormula(Formula);
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ParseException), "Missing condition and then was allowed.")]
        public void MethodTestIfMissingArgument2()
        {
            const string Formula = "= IF ( )";

            this.ParseFormula(Formula);
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestAnd()
        {
            const string Formula = "= AND ( 4 > 5 ; 5 <> 10 ; 5 = 3 )";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(AndEvalDataViewModel));
            var eval = (AndEvalDataViewModel)model;

            Assert.AreEqual(eval.Conditions.Count, 3);
            Assert.IsTrue(eval.Conditions.All(c => c is BinaryOperatorEvalDataViewModelBase));
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestOr()
        {
            const string Formula = "= OR ( 1 ; 5 ; 'abc' )";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(OrEvalDataViewModel));
            var eval = (OrEvalDataViewModel)model;

            Assert.AreEqual(eval.Conditions.Count, 3);
            Assert.IsTrue(eval.Conditions.All(c => c is ConstantEvalDataViewModel));
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestOrFunction()
        {
            const string Formula = "= OR ( NOT ( 4 <= 5 ) ; 5 ; 'abc' )";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(OrEvalDataViewModel));
            var eval = (OrEvalDataViewModel)model;

            Assert.AreEqual(eval.Conditions.Count, 3);
            Assert.IsInstanceOfType(eval.Conditions[0], typeof(NotEvalDataViewModel));
            Assert.IsInstanceOfType(eval.Conditions[1], typeof(ConstantEvalDataViewModel));
            Assert.IsInstanceOfType(eval.Conditions[2], typeof(ConstantEvalDataViewModel));
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestFormat()
        {
            const string Formula = "= FORMAT ( 'My first {0} playlists are {1}' ; 5 ; 'great' )";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(FormatEvalDataViewModel));
            var eval = (FormatEvalDataViewModel)model;

            Assert.AreEqual("My first {0} playlists are {1}", eval.Format.Value);
            Assert.AreEqual(2, eval.Arguments.Count);
            Assert.IsTrue(eval.Arguments.All(c => c is ConstantEvalDataViewModel));
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestGeneric()
        {
            const string Formula = "= $Stops.StopName[1]{de}";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(GenericEvalDataViewModel));
            var eval = (GenericEvalDataViewModel)model;

            Assert.AreEqual(1, eval.Table.Value);
            Assert.AreEqual(1, eval.Column.Value);
            Assert.AreEqual(1, eval.Row.Value);
            Assert.AreEqual(1, eval.Language.Value);
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestGeneric2()
        {
            const string Formula = "= $Stops2.StopName[1]";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(GenericEvalDataViewModel));
            var eval = (GenericEvalDataViewModel)model;

            Assert.AreEqual(2, eval.Table.Value);
            Assert.AreEqual(1, eval.Column.Value);
            Assert.AreEqual(1, eval.Row.Value);
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestGeneric3()
        {
            const string Formula = "= $Stops3.StopName";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(GenericEvalDataViewModel));
            var eval = (GenericEvalDataViewModel)model;

            Assert.AreEqual(3, eval.Table.Value);
            Assert.AreEqual(1, eval.Column.Value);
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestLiteral()
        {
            const string Formula = "= 'abc'";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(ConstantEvalDataViewModel));
            var eval = (ConstantEvalDataViewModel)model;

            Assert.AreEqual(eval.Value.Value, "abc");
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

            Assert.AreEqual(eval.Value.Value, "a'bc");
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestNot()
        {
            const string Formula = "= Not ( 5 > 4 )";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(NotEvalDataViewModel));
            var eval = (NotEvalDataViewModel)model;

            Assert.IsNotNull(eval.Evaluation);
            Assert.IsInstanceOfType(eval.Evaluation, typeof(GreaterThanEvalDataViewModel));
        }

        /// <summary>
        /// Tests if a simple formula is valid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ParseException), "Too many arguments were allowed.")]
        public void MethodTestNotTooManyArguments()
        {
            const string Formula = "= Not ( 5 > 4 ; 4 > 5 )";

            this.ParseFormula(Formula);
        }

        /// <summary>
        /// Tests if a date formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestDate()
        {
            const string Formula = "= DATE ('01.02.2014'; '02.02.2014')";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(DateEvalDataViewModel));
            var eval = (DateEvalDataViewModel)model;

            Assert.IsInstanceOfType(eval.Begin, typeof(DataValue<DateTime>));
            Assert.IsInstanceOfType(eval.End, typeof(DataValue<DateTime>));

            DateTime validationDate;
            DateTime.TryParseExact(
                "01.02.2014", "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out validationDate);
            Assert.AreEqual(eval.Begin.Value, validationDate);
            DateTime.TryParseExact(
                "02.02.2014", "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out validationDate);
            Assert.AreEqual(eval.End.Value, validationDate);
        }

        /// <summary>
        /// Tests if a date formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestTime()
        {
            const string Formula = "= TIME ('18:30:55'; '19:32:42')";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(TimeEvalDataViewModel));
            var eval = (TimeEvalDataViewModel)model;

            Assert.IsInstanceOfType(eval.Begin, typeof(DataValue<TimeSpan>));
            Assert.IsInstanceOfType(eval.End, typeof(DataValue<TimeSpan>));

            TimeSpan validationTime;
            TimeSpan.TryParse("18:30:55", out validationTime);
            Assert.AreEqual(eval.Begin.Value, validationTime);
            TimeSpan.TryParse("19:32:42", out validationTime);
            Assert.AreEqual(eval.End.Value, validationTime);
        }

        /// <summary>
        /// Tests if a switch formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestSwitchOnlyDefault()
        {
            const string Formula = "= SWiTcH ( $Stops3.StopName ; 'Earth')";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(SwitchEvalDataViewModel));
            var eval = (SwitchEvalDataViewModel)model;

            Assert.IsInstanceOfType(eval.Value, typeof(GenericEvalDataViewModel));
            Assert.IsInstanceOfType(eval.Default, typeof(ConstantEvalDataViewModel));

            Assert.AreEqual(0, eval.Cases.Count);

            Assert.AreEqual("$Stops3.StopName", eval.Value.HumanReadable());
            Assert.AreEqual("'Earth'", eval.Default.HumanReadable());
        }

        /// <summary>
        /// Tests if a switch formula is valid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException), "Syntax error, expected: ;")]
        public void MethodTestSwitchTooFew()
        {
            const string Formula = "= SWiTcH ($Stops3.StopName)";

            this.ParseFormula(Formula);
        }

        /// <summary>
        /// Tests if a switch formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestSwitch()
        {
            const string Formula = "= SWiTcH ( $Stops3.StopName ; 'KA': 'Karlsruhe' ; 'St' : 'Stuttgart'; 'Earth')";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(SwitchEvalDataViewModel));
            var eval = (SwitchEvalDataViewModel)model;

            Assert.IsInstanceOfType(eval.Value, typeof(GenericEvalDataViewModel));

            Assert.IsInstanceOfType(eval.Default, typeof(ConstantEvalDataViewModel));

            Assert.IsNotNull(eval.Cases);
            Assert.AreEqual(2, eval.Cases.Count);

            Assert.AreEqual("$Stops3.StopName", eval.Value.HumanReadable());
            Assert.AreEqual("'Earth'", eval.Default.HumanReadable());
            Assert.AreEqual("KA", eval.Cases[0].Value.Value);
            Assert.AreEqual("'Karlsruhe'", eval.Cases[0].Evaluation.HumanReadable());
            Assert.AreEqual("St", eval.Cases[1].Value.Value);
            Assert.AreEqual("'Stuttgart'", eval.Cases[1].Evaluation.HumanReadable());
        }

        /// <summary>
        /// Tests if a day of week formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestDayOfWeek()
        {
            const string Formula = "= DAYOFWEEK ('Sun,Mon,Tue,Thu')";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(DayOfWeekEvalDataViewModel));
            var eval = (DayOfWeekEvalDataViewModel)model;

            Assert.AreEqual(true, eval.Monday.Value);
            Assert.AreEqual(true, eval.Tuesday.Value);
            Assert.AreEqual(false, eval.Wednesday.Value);
            Assert.AreEqual(true, eval.Thursday.Value);
            Assert.AreEqual(false, eval.Friday.Value);
            Assert.AreEqual(false, eval.Saturday.Value);
            Assert.AreEqual(true, eval.Sunday.Value);
        }

        /// <summary>
        /// Tests if a integer compare formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestIntegerCompare()
        {
            const string Formula = "= INTEGERCOMPARE ( $Stops3.StopName; 2; 6)";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(IntegerCompareEvalDataViewModel));
            var eval = (IntegerCompareEvalDataViewModel)model;

            Assert.IsInstanceOfType(eval.Evaluation, typeof(GenericEvalDataViewModel));
            Assert.IsInstanceOfType(eval.Begin, typeof(DataValue<int>));
            Assert.IsInstanceOfType(eval.End, typeof(DataValue<int>));
            Assert.AreEqual(2, eval.Begin.Value);
            Assert.AreEqual(6, eval.End.Value);
        }

        /// <summary>
        /// Tests if a string compare formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestStringCompare()
        {
            const string Formula = "= STRINGCOMPARE ( $Stops3.StopName; 'KA'; 'True')";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(StringCompareEvalDataViewModel));
            var eval = (StringCompareEvalDataViewModel)model;

            Assert.IsInstanceOfType(eval.Evaluation, typeof(GenericEvalDataViewModel));
            Assert.IsInstanceOfType(eval.Value, typeof(DataValue<string>));
            Assert.IsInstanceOfType(eval.IgnoreCase, typeof(DataValue<bool>));
            Assert.AreEqual("KA", eval.Value.Value);
            Assert.IsTrue(eval.IgnoreCase.Value);
        }

        /// <summary>
        /// Tests if a regex replace formula is valid.
        /// </summary>
        [TestMethod]
        public void RegexReplaceTest()
        {
            const string Formula = @"= RegexReplace ( $Stops3.StopName; '^0*(\d+)$'; '$1')";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(RegexReplaceEvalDataViewModel));
            var eval = (RegexReplaceEvalDataViewModel)model;

            Assert.IsInstanceOfType(eval.Evaluation, typeof(GenericEvalDataViewModel));
            Assert.IsInstanceOfType(eval.Pattern, typeof(DataValue<string>));
            Assert.IsInstanceOfType(eval.Replacement, typeof(DataValue<string>));
            Assert.AreEqual(@"^0*(\d+)$", eval.Pattern.Value);
            Assert.AreEqual("$1", eval.Replacement.Value);
        }

        /// <summary>
        /// Tests if a text to image formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestTextToImage()
        {
            const string Formula = "= TEXTTOIMAGE ( $Stops3.StopName; 'Stop_{0}.jpg;Stop_{0}.png')";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(TextToImageEvalDataViewModel));
            var eval = (TextToImageEvalDataViewModel)model;

            Assert.IsInstanceOfType(eval.Evaluation, typeof(GenericEvalDataViewModel));
            Assert.IsInstanceOfType(eval.FilePatterns, typeof(DataValue<string>));
            Assert.AreEqual("Stop_{0}.jpg;Stop_{0}.png", eval.FilePatterns.Value);
        }

        /// <summary>
        /// Tests if a csv mapping formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestCsvMapping()
        {
            const string Formula = "= CsvMapping ( 'myfile.csv'; '{0} bis {1}'; $Stops3.StopName; 5:Not(3=4) )";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(CsvMappingEvalDataViewModel));
            var eval = (CsvMappingEvalDataViewModel)model;

            Assert.IsInstanceOfType(eval.FileName, typeof(DataValue<string>));
            Assert.IsInstanceOfType(eval.OutputFormat, typeof(DataValue<string>));
            Assert.IsInstanceOfType(eval.DefaultValue, typeof(EvalDataViewModelBase));
            Assert.IsInstanceOfType(
                eval.Matches, typeof(ExtendedObservableCollection<MatchDynamicPropertyDataViewModel>));

            Assert.AreEqual(1, eval.Matches.Count);
            Assert.IsInstanceOfType(eval.Matches[0], typeof(MatchDynamicPropertyDataViewModel));

            Assert.AreEqual("myfile.csv", eval.FileName.Value);
            Assert.AreEqual("{0} bis {1}", eval.OutputFormat.Value);
            Assert.AreEqual("$Stops3.StopName", eval.DefaultValue.HumanReadable());

            Assert.AreEqual(5, eval.Matches[0].Column.Value);
            Assert.AreEqual("Not ( 3 = 4 )", eval.Matches[0].Evaluation.HumanReadable());
        }

        /// <summary>
        /// Tests if a csv mapping formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestCsvMappingMultipleArgs()
        {
            const string Formula =
                "= CSVMAPPING ( 'myfile.csv'; '{0} bis {1}'; $Stops3.StopName; 5:NOT ( 5 = 5 ); 6:NOT ( 1 = 2 ) )";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(CsvMappingEvalDataViewModel));
            var eval = (CsvMappingEvalDataViewModel)model;

            Assert.IsInstanceOfType(eval.FileName, typeof(DataValue<string>));
            Assert.IsInstanceOfType(eval.OutputFormat, typeof(DataValue<string>));
            Assert.IsInstanceOfType(eval.DefaultValue, typeof(EvalDataViewModelBase));
            Assert.IsInstanceOfType(
                eval.Matches, typeof(ExtendedObservableCollection<MatchDynamicPropertyDataViewModel>));

            Assert.AreEqual(2, eval.Matches.Count);
            Assert.IsInstanceOfType(eval.Matches[0], typeof(MatchDynamicPropertyDataViewModel));
            Assert.IsInstanceOfType(eval.Matches[1], typeof(MatchDynamicPropertyDataViewModel));

            Assert.AreEqual("myfile.csv", eval.FileName.Value);
            Assert.AreEqual("{0} bis {1}", eval.OutputFormat.Value);
            Assert.AreEqual("$Stops3.StopName", eval.DefaultValue.HumanReadable());

            Assert.AreEqual(5, eval.Matches[0].Column.Value);
            Assert.AreEqual("Not ( 5 = 5 )", eval.Matches[0].Evaluation.HumanReadable());

            Assert.AreEqual(6, eval.Matches[1].Column.Value);
            Assert.AreEqual("Not ( 1 = 2 )", eval.Matches[1].Evaluation.HumanReadable());
        }

        /// <summary>
        /// Tests if a code conversion formula is valid.
        /// </summary>
        [TestMethod]
        public void MethodTestCodeConversion()
        {
            const string Formula = "= CodeConversion ( 'true' )";

            var model = this.ParseFormula(Formula);

            Assert.IsInstanceOfType(model, typeof(CodeConversionEvalDataViewModel));
            var eval = (CodeConversionEvalDataViewModel)model;

            Assert.IsInstanceOfType(eval.FileName, typeof(DataValue<string>));
            Assert.IsInstanceOfType(eval.UseImage, typeof(DataValue<bool>));

            Assert.IsTrue(eval.UseImage.Value);
        }

        private void TestBinaryFormula<T>(string formula, object expectedLeft, object expectedRight)
            where T : BinaryOperatorEvalDataViewModelBase
        {
            var model = this.ParseFormula(formula);

            Assert.IsInstanceOfType(model, typeof(T));
            var eval = (T)model;
            Assert.IsInstanceOfType(eval.Left, typeof(ConstantEvalDataViewModel));
            Assert.IsInstanceOfType(eval.Right, typeof(ConstantEvalDataViewModel));
            Assert.AreEqual(((ConstantEvalDataViewModel)eval.Left).Value.Value, expectedLeft);
            Assert.AreEqual(((ConstantEvalDataViewModel)eval.Right).Value.Value, expectedRight);
        }
    }
}
