// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.Controllers
{
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Motion.Infomedia.Core.Presentation.Evaluation;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator;

    /// <summary>
    /// Base class for the controllers
    /// </summary>
    public class ControllerBase
    {
        /// <summary>
        /// Retrieves the evaluations from the evaluator
        /// </summary>
        /// <param name="evaluator">
        /// The evaluator.
        /// </param>
        /// <returns>
        /// The <see cref="EvaluatorBaseDataViewModel"/>.
        /// </returns>
        public EvaluatorBaseDataViewModel RetrieveEvaluators(EvaluatorBase evaluator)
        {
            var formatEvaluator = evaluator as FormatEvaluator;
            if (formatEvaluator != null)
            {
                return this.CreateFormatEvaluatorDataViewModel(formatEvaluator);
            }

            var genericEvaluator = evaluator as GenericEvaluator;
            if (genericEvaluator != null)
            {
                return this.CreateGenericEvaluatorDataViewModel(genericEvaluator);
            }

            var ifEvaluator = evaluator as IfEvaluator;
            if (ifEvaluator != null)
            {
                return this.CreateIfEvaluatorDataViewModel(ifEvaluator);
            }

            var integerCompareEvaluator = evaluator as IntegerCompareEvaluator;
            if (integerCompareEvaluator != null)
            {
                return this.CreateIntegerCompareEvaluatorDataViewModel(integerCompareEvaluator);
            }

            var constantEvaluator = evaluator as ConstantEvaluator;
            if (constantEvaluator != null)
            {
                var eval = new ConstantEvaluatorDataViewModel();
                eval.Value = constantEvaluator.Value;
                return eval;
            }

            var andEvaluator = evaluator as AndEvaluator;
            if (andEvaluator != null)
            {
                var eval = new AndEvaluatorDataViewModel();
                return this.CreateAndEvaluatorDataViewModel(eval, andEvaluator);
            }

            var evaluatorOr = evaluator as OrEvaluator;
            if (evaluatorOr != null)
            {
                var eval = new OrEvaluatorDataViewModel();
                return this.CreateOrEvaluatorDataViewModel(eval, evaluatorOr);
            }

            var notEvaluator = evaluator as NotEvaluator;
            if (notEvaluator != null)
            {
                var eval = new NotEvaluatorDataViewModel();
                eval.Value = notEvaluator.Value;
                eval.Not =
                (NotEvalDataViewModel)this.RetrieveEvaluations(notEvaluator.NotEval);
                eval.Not.Evaluation = this.RetrieveEvaluations(notEvaluator.NotEval.Evaluation);
                eval.Not.Evaluation.EvalValue = this.RetrieveEvaluators(notEvaluator.EvalEvaluation);
                eval.Not.EvalValue = notEvaluator.Value;
                return eval;
            }

            var stringCompareEvaluator = evaluator as StringCompareEvaluator;
            if (stringCompareEvaluator != null)
            {
                return this.CreateStringCompareEvaluatorDataViewModel(stringCompareEvaluator);
            }

            var csvMappingEvaluator = evaluator as CsvMappingEvaluator;
            if (csvMappingEvaluator != null)
            {
                var eval = new CsvMappingEvaluatorDataViewModel();
                return this.CreateCsvMappingEvaluatorDataViewModel(eval, csvMappingEvaluator);
            }

            var switchEvaluator = evaluator as SwitchEvaluator;
            if (switchEvaluator != null)
            {
                var eval = new SwitchEvaluatorDataViewModel();
                return this.CreateSwitchEvaluatorDataViewModel(eval, switchEvaluator);
            }

            var textToImageEvaluator = evaluator as TextToImageEvaluator;
            if (textToImageEvaluator != null)
            {
                return this.CreateTextToImageEvaluatorDataViewModel(textToImageEvaluator);
            }

            var dateEvaluator = evaluator as DateEvaluator;
            if (dateEvaluator != null)
            {
                return this.CreateDateEvaluatorDataViewModel(dateEvaluator);
            }

            var dayOfWeekEvaluator = evaluator as DayOfWeekEvaluator;
            if (dayOfWeekEvaluator != null)
            {
                var eval = new DayOfWeekEvaluatorDataViewModel();
                eval.Value = dayOfWeekEvaluator.Value;
                eval.DayOfWeek = (DayOfWeekEvalDataViewModel)
                    this.RetrieveEvaluations(dayOfWeekEvaluator.DayOfWeekEval);
                return eval;
            }

            var timeEvaluator = evaluator as TimeEvaluator;
            if (timeEvaluator != null)
            {
                var eval = new TimeEvaluatorDataViewModel();
                eval.Value = timeEvaluator.Value;
                eval.Time = (TimeEvalDataViewModel)this.RetrieveEvaluations(timeEvaluator.TimeEval);
                return eval;
            }

            var equalsEvaluator = evaluator as EqualsEvaluator;
            if (equalsEvaluator != null)
            {
                var eval = new EqualsEvaluatorDataViewModel();
                return this.CreateBinaryOperatorEvaluatorBaseDataViewModel(eval, equalsEvaluator);
            }

            var greaterThanEvaluator = evaluator as GreaterThanEvaluator;
            if (greaterThanEvaluator != null)
            {
                var eval = new GreaterThanEvaluatorDataViewModel();
                return this.CreateBinaryOperatorEvaluatorBaseDataViewModel(eval, greaterThanEvaluator);
            }

            var greaterThanOrEqualEvaluator = evaluator as GreaterThanOrEqualEvaluator;
            if (greaterThanOrEqualEvaluator != null)
            {
                var eval = new GreaterThanOrEqualEvaluatorDataViewModel();
                return this.CreateBinaryOperatorEvaluatorBaseDataViewModel(eval, greaterThanOrEqualEvaluator);
            }

            var lessThanEvaluator = evaluator as LessThanEvaluator;
            if (lessThanEvaluator != null)
            {
                var eval = new LessThanEvaluatorDataViewModel();
                return this.CreateBinaryOperatorEvaluatorBaseDataViewModel(eval, lessThanEvaluator);
            }

            var lessThanOrEqualEvaluator = evaluator as LessThanOrEqualEvaluator;
            if (lessThanOrEqualEvaluator != null)
            {
                var eval = new LessThanOrEqualEvaluatorDataViewModel();
                return this.CreateBinaryOperatorEvaluatorBaseDataViewModel(eval, lessThanOrEqualEvaluator);
            }

            var notEqualsEvaluator = evaluator as NotEqualsEvaluator;
            if (notEqualsEvaluator != null)
            {
                var eval = new NotEqualsEvaluatorDataViewModel();
                return this.CreateBinaryOperatorEvaluatorBaseDataViewModel(eval, notEqualsEvaluator);
            }

            return new EvaluatorBaseDataViewModel();
        }

        private EvaluatorBaseDataViewModel CreateDateEvaluatorDataViewModel(DateEvaluator dateEvaluator)
        {
            var eval = new DateEvaluatorDataViewModel();
            eval.Value = dateEvaluator.Value;
            eval.Date = (DateEvalDataViewModel)this.RetrieveEvaluations(dateEvaluator.DateEval);
            return eval;
        }

        private EvaluatorBaseDataViewModel CreateTextToImageEvaluatorDataViewModel(
            TextToImageEvaluator textToImageEvaluator)
        {
            var eval = new TextToImageEvaluatorDataViewModel();
            eval.Value = textToImageEvaluator.Value;
            eval.TextToImageEval = (TextToImageEvalDataViewModel)this.RetrieveEvaluations(
                textToImageEvaluator.TextToImageEval);
            return eval;
        }

        private EvaluatorBaseDataViewModel CreateStringCompareEvaluatorDataViewModel(
            StringCompareEvaluator stringCompareEvaluator)
        {
            var eval = new StringCompareEvaluatorDataViewModel();
            eval.Value = stringCompareEvaluator.Value;
            eval.StringCompare =
                (StringCompareEvalDataViewModel)this.RetrieveEvaluations(stringCompareEvaluator.StringCompareEval);
            eval.StringCompare.EvalValue = stringCompareEvaluator.Value;
            eval.StringCompare.Evaluation.EvalValue = this.RetrieveEvaluators(stringCompareEvaluator.EvalEvaluation);
            return eval;
        }

        private EvaluatorBaseDataViewModel CreateIntegerCompareEvaluatorDataViewModel(
            IntegerCompareEvaluator integerCompareEvaluator)
        {
            var eval = new IntegerCompareEvaluatorDataViewModel();
            eval.IntegerCompare =
                (IntegerCompareEvalDataViewModel)this.RetrieveEvaluations(integerCompareEvaluator.IntegerCompareEval);
            eval.Value = integerCompareEvaluator.Value;
            eval.IntegerCompare.Evaluation.EvalValue = this.RetrieveEvaluators(integerCompareEvaluator.EvalEvaluation);
            return eval;
        }

        private EvaluatorBaseDataViewModel CreateGenericEvaluatorDataViewModel(GenericEvaluator genericEvaluator)
        {
            var eval = new GenericEvaluatorDataViewModel();
            eval.Value = genericEvaluator.Value;
            eval.Generic = (GenericEvalDataViewModel)this.RetrieveEvaluations(genericEvaluator.GenericEval);
            eval.Generic.EvalValue = genericEvaluator.Value;
            return eval;
        }

        private EvaluatorBaseDataViewModel CreateBinaryOperatorEvaluatorBaseDataViewModel(
            BinaryOperatorEvaluatorBaseDataViewModel eval, BinaryOperatorEvaluatorBase evaluator)
        {
            eval.LeftComparator = this.RetrieveEvaluators(evaluator.HandlerLeft.Evaluator);
            eval.LeftComparator.Value = evaluator.HandlerLeft.Evaluator.Value;
            eval.RightComparator = this.RetrieveEvaluators(evaluator.HandlerRight.Evaluator);
            eval.RightComparator.Value = evaluator.HandlerRight.Evaluator.Value;
            eval.Value = evaluator.Value;
            return eval;
        }

        private EvaluatorBaseDataViewModel CreateSwitchEvaluatorDataViewModel(
            SwitchEvaluatorDataViewModel eval, SwitchEvaluator switchEvaluator)
        {
            eval.Default = this.RetrieveEvaluators(switchEvaluator.HandlerDefault.Evaluator);
            eval.SwitchValue = this.RetrieveEvaluators(switchEvaluator.HandlerValue.Evaluator);
            foreach (var cases in switchEvaluator.HandlersCases)
            {
                eval.Cases.Add(this.RetrieveEvaluators(cases.Evaluator));
            }

            eval.Value = switchEvaluator.Value;
            return eval;
        }

        private EvaluatorBaseDataViewModel CreateCsvMappingEvaluatorDataViewModel(
            CsvMappingEvaluatorDataViewModel eval, CsvMappingEvaluator csvMappingEvaluator)
        {
            eval.DefaultValue = this.RetrieveEvaluators(csvMappingEvaluator.HandlerDefaultValue.Evaluator);
            foreach (var match in csvMappingEvaluator.HandlersMatches)
            {
                eval.Matches.Add(this.RetrieveEvaluators(match.Evaluator));
            }

            eval.Value = csvMappingEvaluator.Value;
            return eval;
        }

        private EvaluatorBaseDataViewModel CreateOrEvaluatorDataViewModel(
            OrEvaluatorDataViewModel eval, CollectionEvaluatorBase evaluatorvaluator)
        {
            foreach (var evalsCondition in evaluatorvaluator.EvalsConditions)
            {
                var condition = this.RetrieveEvaluators(evalsCondition);
                condition.Value = evalsCondition.Value;
                eval.OrConditions.Add(condition);
            }

            eval.Value = evaluatorvaluator.Value;

            return eval;
        }

        private EvaluatorBaseDataViewModel CreateAndEvaluatorDataViewModel(
    AndEvaluatorDataViewModel eval, CollectionEvaluatorBase evaluatorvaluator)
        {
            foreach (var evalsCondition in evaluatorvaluator.EvalsConditions)
            {
                var condition = this.RetrieveEvaluators(evalsCondition);
                condition.Value = evalsCondition.Value;
                eval.AndConditions.Add(condition);
            }

            eval.Value = evaluatorvaluator.Value;

            return eval;
        }

        private EvaluatorBaseDataViewModel CreateFormatEvaluatorDataViewModel(FormatEvaluator formatEvaluator)
        {
            var eval = new FormatEvaluatorDataViewModel();
            foreach (var arg in formatEvaluator.EvalsArguments)
            {
                var argument = this.RetrieveEvaluators(arg);
                argument.Value = this.RetrieveEvaluators(arg);
                eval.FormatArguments.Add(argument);
            }

            eval.Value = formatEvaluator.Value;
            return eval;
        }

        private EvaluatorBaseDataViewModel CreateIfEvaluatorDataViewModel(IfEvaluator ifEvaluator)
        {
            var eval = new IfEvaluatorDataViewModel();
            eval.Condition = this.RetrieveEvaluators(ifEvaluator.HandlerCondition.Evaluator);
            eval.Else = this.RetrieveEvaluators(ifEvaluator.HandlerElse.Evaluator);
            eval.Then = this.RetrieveEvaluators(ifEvaluator.HandlerThen.Evaluator);
            eval.Value = ifEvaluator.Value;

            return eval;
        }

        private EvalBaseDataViewModel RetrieveEvaluations(EvalBase eval)
        {
            var andEval = eval as AndEval;
            if (andEval != null)
            {
                return this.CreateAndEvalDataViewModel(andEval);
            }

            var evalOr = eval as OrEval;
            if (evalOr != null)
            {
                return this.CreateorOrEvalDataViewModel(evalOr);
            }

            var ifEval = eval as IfEval;
            if (ifEval != null)
            {
                return this.CreateIfEvalDataViewModel(ifEval);
            }

            var formatEval = eval as FormatEval;
            if (formatEval != null)
            {
                return this.CreateFormatEvalDataViewModel(formatEval);
            }

            var integerCompareEval = eval as IntegerCompareEval;
            if (integerCompareEval != null)
            {
                return this.CreateIntegerCompareEvalDataViewModel(integerCompareEval);
            }

            var constantEval = eval as ConstantEval;
            if (constantEval != null)
            {
                return this.CreateConstantEvalDataViewModel(constantEval);
            }

            var genericEval = eval as GenericEval;
            if (genericEval != null)
            {
                return this.CreateGenericEvalDataViewModel(genericEval);
            }

            var csvMappingEval = eval as CsvMappingEval;
            if (csvMappingEval != null)
            {
                return this.CreateCsvMappingEvalDataViewModel(csvMappingEval);
            }

            var dateEval = eval as DateEval;
            if (dateEval != null)
            {
                return this.CreatedDateEvalDataViewModel(dateEval);
            }

            var dayOfWeekEval = eval as DayOfWeekEval;
            if (dayOfWeekEval != null)
            {
                return this.CreateDayOfWeekEvalDataViewModel(dayOfWeekEval);
            }

            var evaluationEval = eval as EvaluationEval;
            if (evaluationEval != null)
            {
                return this.CreateEvaluationEvalDataViewModel(evaluationEval);
            }

            var notEval = eval as NotEval;
            if (notEval != null)
            {
                return this.CreatenNotEvalDataViewModel(notEval);
            }

            var regexReplaceEval = eval as RegexReplaceEval;
            if (regexReplaceEval != null)
            {
                return this.CreaterRegexReplaceEvalDataViewModel(regexReplaceEval);
            }

            var stringCompareEval = eval as StringCompareEval;
            if (stringCompareEval != null)
            {
                return this.CreatestriStringCompareEvalDataViewModel(stringCompareEval);
            }

            var switchEval = eval as SwitchEval;
            if (switchEval != null)
            {
                return this.CreateswSwitchEvalDataViewModel(switchEval);
            }

            var textToImageEval = eval as TextToImageEval;
            if (textToImageEval != null)
            {
                return this.CreateTextToImageEvalDataViewModel(textToImageEval);
            }

            var timeEval = eval as TimeEval;
            if (timeEval != null)
            {
                return this.CreateTimeEvalDataViewModel(timeEval);
            }

            var equalsEval = eval as EqualsEval;
            if (equalsEval != null)
            {
                return this.CreateqEqualsEvalDataViewModel(equalsEval);
            }

            var notEqualsEval = eval as NotEqualsEval;
            if (notEqualsEval != null)
            {
                return this.CreateNotEqualsEvalDataViewModel(notEqualsEval);
            }

            var greaterThanEval = eval as GreaterThanEval;
            if (greaterThanEval != null)
            {
                return this.CreategGreaterThanEvalDataViewModel(greaterThanEval);
            }

            var greaterThanOrEqualEval = eval as GreaterThanOrEqualEval;
            if (greaterThanOrEqualEval != null)
            {
                return this.CreategreGreaterThanOrEqualEvalDataViewModel(greaterThanOrEqualEval);
            }

            var lessThanEval = eval as LessThanEval;
            if (lessThanEval != null)
            {
                return this.CreateLessThanEvalDataViewModel(lessThanEval);
            }

            var lessThanOrEqualEval = eval as LessThanOrEqualEval;
            if (lessThanOrEqualEval != null)
            {
                return this.CreateLessThanOrEqualEvalDataViewModel(lessThanOrEqualEval);
            }

            return new EvalBaseDataViewModel();
        }

        private EvalBaseDataViewModel CreateLessThanOrEqualEvalDataViewModel(LessThanOrEqualEval lessThanOrEqualEval)
        {
            var lessThanOrEqualEvalDataViewModel = new LessThanOrEqualEvalDataViewModel();
            lessThanOrEqualEvalDataViewModel.Left = this.RetrieveEvaluations(lessThanOrEqualEval.Left.Evaluation);
            lessThanOrEqualEvalDataViewModel.Right = this.RetrieveEvaluations(lessThanOrEqualEval.Right.Evaluation);
            return lessThanOrEqualEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreateLessThanEvalDataViewModel(LessThanEval lessThanEval)
        {
            var lessThanEvalDataViewModel = new LessThanEvalDataViewModel();
            lessThanEvalDataViewModel.Left = this.RetrieveEvaluations(lessThanEval.Left.Evaluation);
            lessThanEvalDataViewModel.Right = this.RetrieveEvaluations(lessThanEval.Right.Evaluation);
            return lessThanEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreategreGreaterThanOrEqualEvalDataViewModel(
            GreaterThanOrEqualEval greaterThanOrEqualEval)
        {
            var greaterThanOrEqualEvalDataViewModel = new GreaterThanOrEqualEvalDataViewModel();
            greaterThanOrEqualEvalDataViewModel.Left = this.RetrieveEvaluations(greaterThanOrEqualEval.Left.Evaluation);
            greaterThanOrEqualEvalDataViewModel.Right =
                this.RetrieveEvaluations(greaterThanOrEqualEval.Right.Evaluation);
            return greaterThanOrEqualEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreategGreaterThanEvalDataViewModel(GreaterThanEval greaterThanEval)
        {
            var greaterThanEvalDataViewModel = new GreaterThanEvalDataViewModel();
            greaterThanEvalDataViewModel.Left = this.RetrieveEvaluations(greaterThanEval.Left.Evaluation);
            greaterThanEvalDataViewModel.Right = this.RetrieveEvaluations(greaterThanEval.Right.Evaluation);
            return greaterThanEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreateNotEqualsEvalDataViewModel(NotEqualsEval notEqualsEval)
        {
            var notEqualsEvalDataViewModel = new NotEqualsEvalDataViewModel();
            notEqualsEvalDataViewModel.Left = this.RetrieveEvaluations(notEqualsEval.Left.Evaluation);
            notEqualsEvalDataViewModel.Right = this.RetrieveEvaluations(notEqualsEval.Right.Evaluation);
            return notEqualsEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreateqEqualsEvalDataViewModel(EqualsEval equalsEval)
        {
            var equalsEvalDataViewModel = new EqualsEvalDataViewModel();
            equalsEvalDataViewModel.Left = this.RetrieveEvaluations(equalsEval.Left.Evaluation);
            equalsEvalDataViewModel.Right = this.RetrieveEvaluations(equalsEval.Right.Evaluation);
            return equalsEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreateTimeEvalDataViewModel(TimeEval timeEval)
        {
            var timeEvalDataViewModel = new TimeEvalDataViewModel();
            timeEvalDataViewModel.Begin = timeEval.Begin;
            return timeEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreateTextToImageEvalDataViewModel(TextToImageEval textToImageEval)
        {
            var textToImageEvalDataViewModel = new TextToImageEvalDataViewModel();
            textToImageEvalDataViewModel.FilePatterns = textToImageEval.FilePatterns;
            textToImageEvalDataViewModel.Evaluation = this.RetrieveEvaluations(textToImageEval.Evaluation);
            return textToImageEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreateswSwitchEvalDataViewModel(SwitchEval switchEval)
        {
            var switchEvalDataViewModel = new SwitchEvalDataViewModel();
            switchEvalDataViewModel.Default = this.RetrieveEvaluations(switchEval.Default.Evaluation);
            foreach (var caseDynamicProperty in switchEval.Cases)
            {
                var caseEvalBaseDataViewModel = new CaseEvalBaseDataViewModel();
                caseEvalBaseDataViewModel.Value = caseDynamicProperty.Value;
                caseEvalBaseDataViewModel.Evaluation = this.RetrieveEvaluations(caseDynamicProperty.Evaluation);
                switchEvalDataViewModel.Cases.Add(caseEvalBaseDataViewModel);
            }

            switchEvalDataViewModel.Value = this.RetrieveEvaluations(switchEval.Value.Evaluation);
            return switchEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreatestriStringCompareEvalDataViewModel(StringCompareEval stringCompareEval)
        {
            var stringCompareEvalDataViewModel = new StringCompareEvalDataViewModel();
            stringCompareEvalDataViewModel.IgnoreCase = stringCompareEval.IgnoreCase;
            stringCompareEvalDataViewModel.EvalValue = stringCompareEval.Value;
            stringCompareEvalDataViewModel.Evaluation = this.RetrieveEvaluations(stringCompareEval.Evaluation);
            return stringCompareEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreaterRegexReplaceEvalDataViewModel(RegexReplaceEval regexReplaceEval)
        {
            var regexReplaceEvalDataViewModel = new RegexReplaceEvalDataViewModel();
            regexReplaceEvalDataViewModel.Pattern = regexReplaceEval.Pattern;
            regexReplaceEvalDataViewModel.Replacement = regexReplaceEval.Replacement;
            regexReplaceEvalDataViewModel.Evaluation = this.RetrieveEvaluations(regexReplaceEval.Evaluation);
            return regexReplaceEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreatenNotEvalDataViewModel(NotEval notEval)
        {
            var notEvalDataViewModel = new NotEvalDataViewModel();
            notEvalDataViewModel.Evaluation = this.RetrieveEvaluations(notEval.Evaluation);
            notEvalDataViewModel.EvalValue = notEval.Evaluation;
            return notEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreateEvaluationEvalDataViewModel(EvaluationEval evaluationEval)
        {
            var evaluationEvalDataViewModel = new EvaluationEvalDataViewModel();
            evaluationEvalDataViewModel.Reference = evaluationEval.Reference;
            return evaluationEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreateDayOfWeekEvalDataViewModel(DayOfWeekEval dayOfWeekEval)
        {
            var dayOfWeekEvalDataViewModel = new DayOfWeekEvalDataViewModel();
            dayOfWeekEvalDataViewModel.Monday = dayOfWeekEval.Monday;
            dayOfWeekEvalDataViewModel.Tuesday = dayOfWeekEval.Tuesday;
            dayOfWeekEvalDataViewModel.Wednesday = dayOfWeekEval.Wednesday;
            dayOfWeekEvalDataViewModel.Thursday = dayOfWeekEval.Thursday;
            dayOfWeekEvalDataViewModel.Friday = dayOfWeekEval.Friday;
            dayOfWeekEvalDataViewModel.Saturday = dayOfWeekEval.Saturday;
            dayOfWeekEvalDataViewModel.Sunday = dayOfWeekEval.Sunday;
            return dayOfWeekEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreatedDateEvalDataViewModel(DateEval dateEval)
        {
            var dateEvalDataViewModel = new DateEvalDataViewModel();
            dateEvalDataViewModel.Begin = dateEval.Begin;
            return dateEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreateCsvMappingEvalDataViewModel(CsvMappingEval csvMappingEval)
        {
            var csvMappingEvalDataViewModel = new CsvMappingEvalDataViewModel();
            csvMappingEvalDataViewModel.DefaultValue = this.RetrieveEvaluations(csvMappingEval.DefaultValue.Evaluation);
            csvMappingEvalDataViewModel.FileName = csvMappingEval.FileName;
            foreach (var matchDynamicProperty in csvMappingEval.Matches)
            {
                var matchEvalBaseDataViewModel = new MatchEvalBaseDataViewModel();
                matchEvalBaseDataViewModel.Column = matchDynamicProperty.Column;
                matchEvalBaseDataViewModel.Evaluation = this.RetrieveEvaluations(matchDynamicProperty.Evaluation);
                csvMappingEvalDataViewModel.Matches.Add(matchEvalBaseDataViewModel);
            }

            csvMappingEvalDataViewModel.OutputFormat = csvMappingEval.OutputFormat;
            return csvMappingEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreateGenericEvalDataViewModel(GenericEval genericEval)
        {
            var genericEvalDataViewModel = new GenericEvalDataViewModel();
            genericEvalDataViewModel.Language = genericEval.Language;
            genericEvalDataViewModel.Table = genericEval.Table;
            genericEvalDataViewModel.Column = genericEval.Column;
            genericEvalDataViewModel.Row = genericEval.Row;

            return genericEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreateConstantEvalDataViewModel(ConstantEval constantEval)
        {
            var constantEvalDataViewModel = new ConstantEvalDataViewModel();
            constantEvalDataViewModel.Value = constantEval.Value;
            return constantEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreateIntegerCompareEvalDataViewModel(IntegerCompareEval integerCompareEval)
        {
            var integerCompareEvalDataViewModel = new IntegerCompareEvalDataViewModel();
            integerCompareEvalDataViewModel.Begin = integerCompareEval.Begin;
            integerCompareEvalDataViewModel.End = integerCompareEval.End;
            integerCompareEvalDataViewModel.Evaluation = this.RetrieveEvaluations(integerCompareEval.Evaluation);
            return integerCompareEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreateFormatEvalDataViewModel(FormatEval formatEval)
        {
            var formatEvalDataViewModel = new FormatEvalDataViewModel();
            formatEvalDataViewModel.Format = formatEval.Format;
            foreach (var argument in formatEval.Arguments)
            {
                formatEvalDataViewModel.Arguments.Add(this.RetrieveEvaluations(argument));
            }

            return formatEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreateIfEvalDataViewModel(IfEval ifEval)
        {
            var ifEvalDataViewModel = new IfEvalDataViewModel();
            ifEvalDataViewModel.Condition = this.RetrieveEvaluations(ifEval.Condition.Evaluation);
            ifEvalDataViewModel.Then = this.RetrieveEvaluations(ifEval.Then.Evaluation);
            ifEvalDataViewModel.Else = this.RetrieveEvaluations(ifEval.Else.Evaluation);
            return ifEvalDataViewModel;
        }

        private EvalBaseDataViewModel CreateorOrEvalDataViewModel(OrEval evalOr)
        {
            var evalOrDataViewModel = new OrEvalDataViewModel();
            foreach (var condition in evalOr.Conditions)
            {
                evalOrDataViewModel.Conditions.Add(this.RetrieveEvaluations(condition));
            }

            return evalOrDataViewModel;
        }

        private EvalBaseDataViewModel CreateAndEvalDataViewModel(AndEval andEval)
        {
            var andEvalDataViewModel = new AndEvalDataViewModel();
            foreach (var condition in andEval.Conditions)
            {
                andEvalDataViewModel.Conditions.Add(this.RetrieveEvaluations(condition));
            }

            return andEvalDataViewModel;
        }
    }
}
