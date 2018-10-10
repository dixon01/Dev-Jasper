// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaVisitor.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The FormulaVisitor.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Formulas
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;

    using Irony.Interpreter.Ast;

    /// <summary>
    /// the formula visitor
    /// </summary>
    public class FormulaVisitor : IAstVisitor
    {
        private readonly IMediaShell shell;

        private readonly Func<string, string, string, string, GenericEvalDataViewModel> genericEvalFactory;

        private readonly Stack<NodeStateEvalDataViewModelBase> evalStack;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaVisitor"/> class.
        /// </summary>
        /// <param name="shell">the shell</param>
        /// <param name="genericEvalFactory">the generic evaluation factory</param>
        public FormulaVisitor(
            IMediaShell shell, Func<string, string, string, string, GenericEvalDataViewModel> genericEvalFactory)
        {
            this.shell = shell;
            this.genericEvalFactory = genericEvalFactory;
            this.evalStack = new Stack<NodeStateEvalDataViewModelBase>();
        }

        /// <summary>
        /// Gets or sets the model, this is the resulting Formula
        /// </summary>
        public EvalDataViewModelBase Model { get; set; }

        /// <summary>
        /// the begin visit method
        /// </summary>
        /// <param name="node">the node</param>
        public void BeginVisit(IVisitableNode node)
        {
            var eval = this.Visit(node);

            if (eval != null)
            {
                if (this.evalStack.Count > 0)
                {
                    var parent = this.GetFirstNonNullParent();
                    if (parent != null)
                    {
                        AddChild(parent, eval, this.shell);
                    }
                }
            }

            this.evalStack.Push(new NodeStateEvalDataViewModelBase(eval));
        }

        /// <summary>
        /// the end visit method
        /// </summary>
        /// <param name="node">the node</param>
        public void EndVisit(IVisitableNode node)
        {
            var nodestate = this.evalStack.Pop();

            if (nodestate.Model != null)
            {
                this.Validate(nodestate.Model);
                this.Model = nodestate.Model;
            }
        }

        #region AddChild

        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private static void AddChild(
            NodeStateEvalDataViewModelBase parentnode,
            EvalDataViewModelBase eval,
            IMediaShell shell)
        {
            var parent = parentnode.Model;
            if (parent is BinaryOperatorEvalDataViewModelBase)
            {
                var model = parent as BinaryOperatorEvalDataViewModelBase;
                AddChild(model, eval, parentnode);
            }
            else if (parent is IfEvalDataViewModel)
            {
                var model = parent as IfEvalDataViewModel;
                AddChild(model, eval, parentnode);
            }
            else if (parent is CollectionEvalDataViewModelBase)
            {
                var model = parent as CollectionEvalDataViewModelBase;
                AddChild(model, eval);
            }
            else if (parent is IntegerCompareEvalDataViewModel)
            {
                var model = parent as IntegerCompareEvalDataViewModel;
                AddChild(model, eval, parentnode);
            }
            else if (parent is StringCompareEvalDataViewModel)
            {
                var model = parent as StringCompareEvalDataViewModel;
                AddChild(model, eval, parentnode);
            }
            else if (parent is TextToImageEvalDataViewModel)
            {
                var model = parent as TextToImageEvalDataViewModel;
                AddChild(model, eval, parentnode);
            }
            else if (parent is RegexReplaceEvalDataViewModel)
            {
                var model = parent as RegexReplaceEvalDataViewModel;
                AddChild(model, eval, parentnode);
            }
            else if (parent is ContainerEvalDataViewModelBase)
            {
                var model = parent as ContainerEvalDataViewModelBase;
                AddChild(model, eval, parentnode);
            }
            else if (parent is FormatEvalDataViewModel)
            {
                var model = parent as FormatEvalDataViewModel;
                AddChild(model, eval, parentnode);
            }
            else if (parent is CsvMappingEvalDataViewModel)
            {
                var model = parent as CsvMappingEvalDataViewModel;
                AddChild(model, eval, parentnode, shell);
            }
            else if (parent is DateEvalDataViewModel)
            {
                var model = parent as DateEvalDataViewModel;
                AddChild(model, eval, parentnode);
            }
            else if (parent is DayOfWeekEvalDataViewModel)
            {
                var model = parent as DayOfWeekEvalDataViewModel;
                AddChild(model, eval);
            }
            else if (parent is TimeEvalDataViewModel)
            {
                var model = parent as TimeEvalDataViewModel;
                AddChild(model, eval, parentnode);
            }
            else if (parent is SwitchEvalDataViewModel)
            {
                var model = parent as SwitchEvalDataViewModel;
                AddChild(model, eval, parentnode, shell);
            }
            else if (parent is CodeConversionEvalDataViewModel)
            {
                var model = parent as CodeConversionEvalDataViewModel;
                AddChild(model, eval);
            }
            else
            {
                throw new InvalidOperationException(
                    "tsnh. A Child is not valid in this context. (" + parent.GetType() + ")");
            }

            parentnode.ArgumentIndex++;
        }

        private static void AddChild(
            CodeConversionEvalDataViewModel model,
            EvalDataViewModelBase eval)
        {
            var viewModel = eval as ConstantEvalDataViewModel;
            if (viewModel == null)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_DayOfWeek_ArgumentsNeedToBeBool);
            }

            var dateValue = viewModel.Value.Value;

            bool value;
            bool.TryParse(dateValue, out value);
            model.UseImage.Value = value;
        }

        private static void AddChild(
            BinaryOperatorEvalDataViewModelBase model,
            EvalDataViewModelBase eval,
            NodeStateEvalDataViewModelBase parentnode)
        {
            switch (parentnode.ArgumentIndex)
            {
                case 0:
                    model.Left = eval;
                    break;
                case 1:
                    model.Right = eval;
                    break;
                default:
                    throw new InvalidOperationException(
                        "tsnh. Error in grammar, binary expressions can never have 3 children.");
            }
        }

        private static void AddChild(
        IntegerCompareEvalDataViewModel model,
        EvalDataViewModelBase eval,
            NodeStateEvalDataViewModelBase parentnode)
        {
            var viewModel = eval as ConstantEvalDataViewModel;
            switch (parentnode.ArgumentIndex)
            {
                case 0:
                    model.Evaluation = eval;
                    break;
                case 1:
                    if (viewModel == null)
                    {
                        throw new ParseException(MediaStrings.FormulaParser_Error_IntegerCompare_SecondArgumentNotInt);
                    }

                    int begin;
                    if (int.TryParse(viewModel.Value.Value, out begin))
                    {
                        model.Begin.Value = begin;
                    }
                    else
                    {
                        throw new ParseException(MediaStrings.FormulaParser_Error_IntegerCompare_SecondArgumentNotInt);
                    }

                    break;
                case 2:
                    if (viewModel == null)
                    {
                        throw new ParseException(MediaStrings.FormulaParser_Error_IntegerCompare_ThirdArgumentNotInt);
                    }

                    int end;
                    if (int.TryParse(viewModel.Value.Value, out end))
                    {
                        model.End.Value = end;
                    }
                    else
                    {
                        throw new ParseException(MediaStrings.FormulaParser_Error_IntegerCompare_ThirdArgumentNotInt);
                    }

                    break;
                default:
                    throw new ParseException(MediaStrings.FormulaParser_Error_IntegerCompare_TooManyArguments);
            }
        }

        private static void AddChild(
            StringCompareEvalDataViewModel model, EvalDataViewModelBase eval, NodeStateEvalDataViewModelBase parentnode)
        {
            var viewModel = eval as ConstantEvalDataViewModel;
            switch (parentnode.ArgumentIndex)
            {
                case 0:
                    model.Evaluation = eval;
                    break;
                case 1:
                    if (viewModel == null)
                    {
                        throw new ParseException(
                            MediaStrings.FormulaParser_Error_StringCompare_SecondArgumentNotConstant);
                    }

                    model.Value.Value = viewModel.Value.Value;
                    break;
                case 2:
                    if (viewModel == null)
                    {
                        throw new ParseException(MediaStrings.FormulaParser_Error_StringCompare_ThirdArgumentNotBool);
                    }

                    bool ignoreCase;
                    if (bool.TryParse(viewModel.Value.Value, out ignoreCase))
                    {
                        model.IgnoreCase.Value = ignoreCase;
                    }
                    else
                    {
                        throw new ParseException(MediaStrings.FormulaParser_Error_StringCompare_ThirdArgumentNotBool);
                    }

                    break;
                default:
                    throw new ParseException(MediaStrings.FormulaParser_Error_StringCompare_TooManyArguments);
            }
        }

        private static void AddChild(
            IfEvalDataViewModel model,
            EvalDataViewModelBase eval,
            NodeStateEvalDataViewModelBase parentnode)
        {
            switch (parentnode.ArgumentIndex)
            {
                case 0:
                    model.Condition = eval;
                    break;
                case 1:
                    model.Then = eval;
                    break;
                case 2:
                    model.Else = eval;
                    break;
                default:
                    throw new ParseException(MediaStrings.FormulaParser_Error_IF_TooManyArguments);
            }
        }

        private static void AddChild(
       RegexReplaceEvalDataViewModel model,
       EvalDataViewModelBase eval,
       NodeStateEvalDataViewModelBase parentnode)
        {
            var viewModel = eval as ConstantEvalDataViewModel;
            switch (parentnode.ArgumentIndex)
            {
                case 0:
                    model.Evaluation = eval;
                    break;
                case 1:
                    if (viewModel == null)
                    {
                        throw new ParseException(MediaStrings.FormulaParser_Error_RegexReplace_SecondArgumentNotString);
                    }

                    model.Pattern.Value = viewModel.Value.Value;
                    break;
                case 2:
                    if (viewModel == null)
                    {
                        throw new ParseException(MediaStrings.FormulaParser_Error_RegexReplace_ThirdArgumentNotString);
                    }

                    model.Replacement.Value = viewModel.Value.Value;
                    break;
                default:
                    throw new ParseException(MediaStrings.FormulaParser_Error_RegexReplace_TooManyArguments);
            }
        }

        private static void AddChild(
            CollectionEvalDataViewModelBase model,
            EvalDataViewModelBase eval)
        {
            model.Conditions.Add(eval);
        }

        private static void AddChild(
            TextToImageEvalDataViewModel model, EvalDataViewModelBase eval, NodeStateEvalDataViewModelBase parentnode)
        {
            switch (parentnode.ArgumentIndex)
            {
                case 0:
                    model.Evaluation = eval;
                    break;
                case 1:
                    var viewModel = eval as ConstantEvalDataViewModel;
                    if (viewModel == null)
                    {
                        throw new ParseException(MediaStrings.FormulaParser_Error_TextToImage_SecondArgumentNotString);
                    }

                    model.FilePatterns.Value = viewModel.Value.Value;

                    break;
                default:
                    throw new ParseException(MediaStrings.FormulaParser_Error_TextToImage_TooManyArguments);
            }
        }

        private static void AddChild(
            ContainerEvalDataViewModelBase model,
            EvalDataViewModelBase eval,
// ReSharper disable UnusedParameter.Local
            NodeStateEvalDataViewModelBase parentnode)
// ReSharper restore UnusedParameter.Local
        {
            if (parentnode.ArgumentIndex != 0)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_Container_TooManyArguments);
            }

            model.Evaluation = eval;
        }

        private static void AddChild(
            FormatEvalDataViewModel model,
            EvalDataViewModelBase eval,
            NodeStateEvalDataViewModelBase parentnode)
        {
            if (parentnode.ArgumentIndex == 0)
            {
                var viewModel = eval as ConstantEvalDataViewModel;
                if (viewModel == null)
                {
                    throw new ParseException(MediaStrings.FormulaParser_Error_FORMAT_FirstArgumentNotString);
                }

                model.Format = viewModel.Value;
            }
            else
            {
                model.Arguments.Add(eval);
            }
        }

        private static void AddChild(
            CsvMappingEvalDataViewModel model,
            EvalDataViewModelBase eval,
            NodeStateEvalDataViewModelBase parentnode,
            IMediaShell shell)
        {
            switch (parentnode.ArgumentIndex)
            {
                case 0:
                    {
                        var viewModel = eval as ConstantEvalDataViewModel;
                        if (viewModel == null)
                        {
                            throw new ParseException(
                                MediaStrings.FormulaParser_Error_CSVMAPPING_FirstArgumentNotString);
                        }

                        model.FileName = viewModel.Value;
                    }

                    break;
                case 1:
                    {
                        var viewModel = eval as ConstantEvalDataViewModel;
                        if (viewModel == null)
                        {
                            throw new ParseException(
                                MediaStrings.FormulaParser_Error_CSVMAPPING_SecondArgumentNotString);
                        }

                        model.OutputFormat = viewModel.Value;
                    }

                    break;
                case 2:
                    model.DefaultValue = eval;
                    break;
                default:
                    if (parentnode.ArgumentIndex % 2 != 0)
                    {
                        var constantViewModel = eval as ConstantEvalDataViewModel;
                        if (constantViewModel == null)
                        {
                            throw new ParseException(MediaStrings.FormulaParser_Error_Case_FirstArgumentNotConstant);
                        }

                        int intValue;
                        if (!int.TryParse(constantViewModel.Value.Value, out intValue))
                        {
                            throw new ParseException(
                                string.Format(
                                    MediaStrings.FormulaParser_Error_CsvMapping_LeftSideOfCaseIsNoInteger,
                                    (parentnode.ArgumentIndex - 1) / 2));
                        }

                        model.Matches.Add(new MatchDynamicPropertyDataViewModel(shell)
                                            {
                                                Column = new DataValue<int>(intValue)
                                            });
                    }
                    else
                    {
                        model.Matches.Last().Evaluation = eval;
                    }

                    break;
            }
        }

        private static void AddChild(
            DateEvalDataViewModel model,
            EvalDataViewModelBase eval,
            NodeStateEvalDataViewModelBase parentnode)
        {
            switch (parentnode.ArgumentIndex)
            {
                case 0:
                    {
                        var viewModel = eval as ConstantEvalDataViewModel;
                        if (viewModel != null)
                        {
                            DateTime dateValue;
                            if (DateTime.TryParseExact(
                                viewModel.Value.Value,
                                Settings.Default.DateEvalFormat,
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out dateValue))
                            {
                                model.Begin = new DataValue<DateTime>(dateValue);
                            }
                            else
                            {
                                throw new ParseException(MediaStrings.FormulaParser_Error_Date_FirstArgumentNotDate);
                            }
                        }
                        else
                        {
                            throw new ParseException(MediaStrings.FormulaParser_Error_Date_FirstArgumentNotString);
                        }
                    }

                    break;
                case 1:
                    {
                        var viewModel = eval as ConstantEvalDataViewModel;
                        if (viewModel != null)
                        {
                            DateTime dateValue;
                            if (DateTime.TryParseExact(
                                viewModel.Value.Value,
                                Settings.Default.DateEvalFormat,
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out dateValue))
                            {
                                model.End = new DataValue<DateTime>(dateValue);
                            }
                            else
                            {
                                throw new ParseException(MediaStrings.FormulaParser_Error_Date_SecondArgumentNotDate);
                            }
                        }
                        else
                        {
                            throw new ParseException(MediaStrings.FormulaParser_Error_Date_SecondArgumentNotString);
                        }
                    }

                    break;
                default:
                    throw new ParseException(MediaStrings.FormulaParser_Error_Date_TooManyArguments);
            }
        }

        private static void AddChild(
            DayOfWeekEvalDataViewModel model,
            EvalDataViewModelBase eval)
        {
            var viewModel = eval as ConstantEvalDataViewModel;
            if (viewModel == null)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_DayOfWeek_ArgumentsNeedToBeBool);
            }

            var dateValue = viewModel.Value.Value;

            // Set all day values to false
            model.ClearValues();

            var days = dateValue.Split(new[] { ',' });
            foreach (var day in days)
            {
                switch (day.Trim().ToLower())
                {
                    case "mon":
                        model.Monday.Value = true;
                        break;
                    case "tue":
                        model.Tuesday.Value = true;
                        break;
                    case "wed":
                        model.Wednesday.Value = true;
                        break;
                    case "thu":
                        model.Thursday.Value = true;
                        break;
                    case "fri":
                        model.Friday.Value = true;
                        break;
                    case "sat":
                        model.Saturday.Value = true;
                        break;
                    case "sun":
                        model.Sunday.Value = true;
                        break;
                    default:
                        var exception = string.Format(MediaStrings.FormulaParser_Error_DayOfWeek_UnknownDay, day);
                        throw new ParseException(exception);
                }
            }
        }

        private static void AddChild(
            TimeEvalDataViewModel model,
            EvalDataViewModelBase eval,
            NodeStateEvalDataViewModelBase parentnode)
        {
            switch (parentnode.ArgumentIndex)
            {
                case 0:
                    {
                        var viewModel = eval as ConstantEvalDataViewModel;
                        if (viewModel == null)
                        {
                            throw new ParseException(MediaStrings.FormulaParser_Error_Time_FirstArgumentNotString);
                        }

                        TimeSpan timeValue;
                        if (TimeSpan.TryParse(viewModel.Value.Value, out timeValue))
                        {
                            model.Begin = new DataValue<TimeSpan>(timeValue);
                        }
                    }

                    break;
                case 1:
                    {
                        var viewModel = eval as ConstantEvalDataViewModel;
                        if (viewModel == null)
                        {
                            throw new ParseException(MediaStrings.FormulaParser_Error_Time_SecondArgumentNotString);
                        }

                        TimeSpan timeValue;
                        if (TimeSpan.TryParse(viewModel.Value.Value, out timeValue))
                        {
                            model.End = new DataValue<TimeSpan>(timeValue);
                        }
                    }

                    break;
            }
        }

        private static void AddChild(
            SwitchEvalDataViewModel model,
            EvalDataViewModelBase eval,
            NodeStateEvalDataViewModelBase parentnode,
            IMediaShell shell)
        {
            switch (parentnode.ArgumentIndex)
            {
                case 0:
                    model.Value = eval;
                    break;
                case 1:
                    model.Default = eval;
                    break;
                default:
                    if (parentnode.ArgumentIndex % 2 == 0)
                    {
                        var constantViewModel = eval as ConstantEvalDataViewModel;
                        if (constantViewModel == null)
                        {
                            throw new ParseException(MediaStrings.FormulaParser_Error_Case_FirstArgumentNotConstant);
                        }

                        model.Cases.Add(new CaseDynamicPropertyDataViewModel(shell)
                                            {
                                                Value = constantViewModel.Value
                                            });
                    }
                    else
                    {
                        model.Cases.Last().Evaluation = eval;
                    }

                    break;
            }
        }

        #endregion

        #region Validate

        private void Validate(EvalDataViewModelBase eval)
        {
            if (eval is IfEvalDataViewModel)
            {
                this.Validate((IfEvalDataViewModel)eval);
            }
            else if (eval is CollectionEvalDataViewModelBase)
            {
                this.Validate(eval as CollectionEvalDataViewModelBase);
            }
            else if (eval is BinaryOperatorEvalDataViewModelBase)
            {
                this.Validate((BinaryOperatorEvalDataViewModelBase)eval);
            }
            else if (eval is TextToImageEvalDataViewModel)
            {
                this.Validate((TextToImageEvalDataViewModel)eval);
            }
            else if (eval is ContainerEvalDataViewModelBase)
            {
                this.Validate((ContainerEvalDataViewModelBase)eval);
            }
            else if (eval is FormatEvalDataViewModel)
            {
                this.Validate((FormatEvalDataViewModel)eval);
            }
            else if (eval is DateEvalDataViewModel)
            {
                this.Validate((DateEvalDataViewModel)eval);
            }
            else if (eval is DayOfWeekEvalDataViewModel)
            {
                this.Validate((DayOfWeekEvalDataViewModel)eval);
            }
            else if (eval is TimeEvalDataViewModel)
            {
                this.Validate((TimeEvalDataViewModel)eval);
            }
            else if (eval is SwitchEvalDataViewModel)
            {
                this.Validate((SwitchEvalDataViewModel)eval);
            }
            else if (eval is CsvMappingEvalDataViewModel)
            {
                this.Validate(eval as CsvMappingEvalDataViewModel);
            }
        }

        // ReSharper disable UnusedParameter.Local
        private void Validate(IfEvalDataViewModel eval)
        {
            if (eval.Condition == null)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_IF_MissingConditionArgument);
            }

            if (eval.Then == null)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_IF_MissingThenArgument);
            }
        }

        private void Validate(CollectionEvalDataViewModelBase eval)
        {
        }

        private void Validate(CsvMappingEvalDataViewModel eval)
        {
            if (string.IsNullOrEmpty(eval.OutputFormat.Value))
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_Csv_MissingOutputFormat);
            }

            if (string.IsNullOrEmpty(eval.FileName.Value))
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_Csv_MissingFilename);
            }
        }

        private void Validate(SwitchEvalDataViewModel eval)
        {
            if (eval.Value == null)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_Switch_MissingValueArgument);
            }

            if (eval.Default == null)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_Switch_MissingDefaultArgument);
            }
        }

        private void Validate(TextToImageEvalDataViewModel eval)
        {
            if (eval.Evaluation == null)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_TextToImage_MissingEvaluationArgument);
            }
        }

        private void Validate(TimeEvalDataViewModel eval)
        {
            if (eval.Begin == null)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_Time_MissingBeginArgument);
            }

            if (eval.End == null)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_Time_MissingEndArgument);
            }

            if (eval.End.Value < eval.Begin.Value)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_Time_EndLessThanBegin);
            }
        }

        private void Validate(DayOfWeekEvalDataViewModel eval)
        {
        }

        private void Validate(DateEvalDataViewModel eval)
        {
            if (eval.Begin == null)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_Date_MissingBeginArgument);
            }

            if (eval.End == null)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_Date_MissingEndArgument);
            }

            if (eval.End.Value < eval.Begin.Value)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_Date_EndLessThanBegin);
            }
        }

        private void Validate(FormatEvalDataViewModel eval)
        {
        }

        private void Validate(ContainerEvalDataViewModelBase eval)
        {
            if (eval.Evaluation == null && !(eval is EvaluationConfigDataViewModel))
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_Container_MissingEvaluationArgument);
            }
        }

        private void Validate(BinaryOperatorEvalDataViewModelBase eval)
        {
            if (eval.Left == null)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_Binary_MissingLeftArgument);
            }

            if (eval.Right == null)
            {
                throw new ParseException(MediaStrings.FormulaParser_Error_Binary_MissingRightArgument);
            }
        }

        // ReSharper restore UnusedParameter.Local
        #endregion

        #region Visit

        private EvalDataViewModelBase Visit(IVisitableNode node)
        {
            EvalDataViewModelBase eval = null;

            if (node is LiteralValueNode)
            {
                eval = this.Visit(node as LiteralValueNode);
            }
            else if (node is BinaryExpressionNode)
            {
                eval = this.Visit(node as BinaryExpressionNode);
            }
            else if (node is MethodCallNode)
            {
                eval = this.Visit(node as MethodCallNode);
            }
            else if (node is GenericNode)
            {
                eval = this.Visit(node as GenericNode);
            }
            else if (node is SwitchCallNode)
            {
                eval = new SwitchEvalDataViewModel(this.shell);
            }
            else if (node is CsvMappingCallNode)
            {
                eval = new CsvMappingEvalDataViewModel(this.shell);
            }

            return eval;
        }

        private EvalDataViewModelBase Visit(MethodCallNode functionCallNode)
        {
            EvalDataViewModelBase result = null;

            if (functionCallNode.TargetName.Equals("if", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new IfEvalDataViewModel(this.shell);
            }
            else if (functionCallNode.TargetName.Equals("and", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new AndEvalDataViewModel(this.shell);
            }
            else if (functionCallNode.TargetName.Equals("or", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new OrEvalDataViewModel(this.shell);
            }
            else if (functionCallNode.TargetName.Equals("not", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new NotEvalDataViewModel(this.shell);
            }
            else if (functionCallNode.TargetName.Equals("format", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new FormatEvalDataViewModel(this.shell);
            }
            else if (functionCallNode.TargetName.Equals("date", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new DateEvalDataViewModel(this.shell);
            }
            else if (functionCallNode.TargetName.Equals("dayofweek", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new DayOfWeekEvalDataViewModel(this.shell);
            }
            else if (functionCallNode.TargetName.Equals("time", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new TimeEvalDataViewModel(this.shell);
            }
            else if (functionCallNode.TargetName.Equals("switch", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new SwitchEvalDataViewModel(this.shell);
            }
            else if (functionCallNode.TargetName.Equals("integercompare", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new IntegerCompareEvalDataViewModel(this.shell);
            }
            else if (functionCallNode.TargetName.Equals("stringcompare", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new StringCompareEvalDataViewModel(this.shell);
            }
            else if (functionCallNode.TargetName.Equals("texttoimage", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new TextToImageEvalDataViewModel(this.shell);
            }
            else if (functionCallNode.TargetName.Equals("regexreplace", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new RegexReplaceEvalDataViewModel(this.shell);
            }
            else if (functionCallNode.TargetName.Equals("codeconversion", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new CodeConversionEvalDataViewModel(this.shell);
            }
            else
            {
                // look up custom formula in the project
                foreach (
                    var predefinedFormula in
                    this.shell.MediaApplicationState.CurrentProject.InfomediaConfig.Evaluations)
                {
                    if (functionCallNode.TargetName.Equals(
                        predefinedFormula.Name.Value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = new EvaluationEvalDataViewModel(this.shell)
                        {
                            Reference = predefinedFormula
                        };
                        break;
                    }
                }
            }

            return result;
        }

        private EvalDataViewModelBase Visit(BinaryExpressionNode binaryExpressionNode)
        {
            EvalDataViewModelBase result = null;

            if (binaryExpressionNode.OperatorNode.GetOperatorText() == BinaryOperation.GreaterThan.Operator)
            {
                result = new GreaterThanEvalDataViewModel(this.shell);
            }
            else if (binaryExpressionNode.OperatorNode.GetOperatorText() == BinaryOperation.GreaterThanOrEqual.Operator)
            {
                result = new GreaterThanOrEqualEvalDataViewModel(this.shell);
            }
            else if (binaryExpressionNode.OperatorNode.GetOperatorText() == BinaryOperation.LessThan.Operator)
            {
                result = new LessThanEvalDataViewModel(this.shell);
            }
            else if (binaryExpressionNode.OperatorNode.GetOperatorText() == BinaryOperation.LessThanOrEqual.Operator)
            {
                result = new LessThanOrEqualEvalDataViewModel(this.shell);
            }
            else if (binaryExpressionNode.OperatorNode.GetOperatorText() == BinaryOperation.IsEqual.Operator)
            {
                result = new EqualsEvalDataViewModel(this.shell);
            }
            else if (binaryExpressionNode.OperatorNode.GetOperatorText() == BinaryOperation.NotEquals.Operator)
            {
                result = new NotEqualsEvalDataViewModel(this.shell);
            }
            else if (binaryExpressionNode.OperatorNode.GetOperatorText() == ":")
            {
                // children of this node will be added to CaseDynamicProperty directly
            }

            return result;
        }

        private EvalDataViewModelBase Visit(LiteralValueNode literalValueNode)
        {
            return new ConstantEvalDataViewModel(this.shell)
                   {
                       Value = new DataValue<string>(literalValueNode.Value.ToString())
                   };
        }

        private EvalDataViewModelBase Visit(GenericNode genericNode)
        {
            return this.genericEvalFactory(
                genericNode.Table, genericNode.Column, genericNode.Row, genericNode.Language);
        }

        #endregion

        private NodeStateEvalDataViewModelBase GetFirstNonNullParent()
        {
            return this.evalStack.FirstOrDefault(element => element.Model != null);
        }

        private class NodeStateEvalDataViewModelBase
        {
            public NodeStateEvalDataViewModelBase(EvalDataViewModelBase model)
            {
                this.Model = model;
            }

            public EvalDataViewModelBase Model { get; private set; }

            public int ArgumentIndex { get; set; }
        }
    }
}