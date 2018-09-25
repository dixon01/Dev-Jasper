// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The FormulaEditor.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Navigation;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Media.Core.Converters;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.ViewModels;

    using NLog;

    /// <summary>
    /// Interaction logic for FormulaEditor.xaml
    /// </summary>
    public partial class FormulaEditor
    {
        /// <summary>
        /// The dictionary selector command property.
        /// </summary>
        public static readonly DependencyProperty DictionarySelectorCommandProperty =
            DependencyProperty.Register(
                "DictionarySelectorCommand",
                typeof(ICommand),
                typeof(FormulaEditor),
                new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// specifies the max height of the formula selection pop down
        /// </summary>
        public static readonly DependencyProperty MaxFormulaDropDownHeightProperty =
            DependencyProperty.Register(
                "MaxFormulaDropDownHeight",
                typeof(double),
                typeof(FormulaEditor),
                new PropertyMetadata(default(double)));

        /// <summary>
        /// The PredefinedEvaluationsProperty
        /// </summary>
        public static readonly DependencyProperty PredefinedEvaluationsProperty =
            DependencyProperty.Register(
                "PredefinedEvaluations",
                typeof(ExtendedObservableCollection<EvaluationConfigDataViewModel>),
                typeof(FormulaEditor),
                new PropertyMetadata(
                    default(ExtendedObservableCollection<EvaluationConfigDataViewModel>),
                    OnPredefinedEvaluationsChanged));

        /// <summary>
        /// the FormulaTypeListProperty
        /// </summary>
        public static readonly DependencyProperty FormulaTypeListProperty = DependencyProperty.Register(
            "FormulaTypeList",
            typeof(ExtendedObservableCollection<EvaluationTypeDefinition>),
            typeof(FormulaEditor),
            new PropertyMetadata(default(ExtendedObservableCollection<EvaluationTypeDefinition>)));

        /// <summary>
        /// The csv mapping file names property.
        /// </summary>
        public static readonly DependencyProperty CsvMappingFileNamesProperty =
            DependencyProperty.Register(
                "CsvMappingFileNames",
                typeof(ExtendedObservableCollection<string>),
                typeof(FormulaEditor),
                new PropertyMetadata(new ExtendedObservableCollection<string>()));

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Lazy<IEnumerable<EvaluationTypeDefinition>> evaluationTypeDefinitions;

        private readonly List<EvaluationType> evaluationTypesToIgnore = new List<EvaluationType>
                                                                        {
                                                                            EvaluationType.Constant,
                                                                            EvaluationType.Evaluation,
                                                                            EvaluationType.Equals,
                                                                            EvaluationType.GreaterThan,
                                                                            EvaluationType.GreaterThanOrEqual,
                                                                            EvaluationType.LessThan,
                                                                            EvaluationType.LessThanOrEqual,
                                                                            EvaluationType.NotEquals,
                                                                            EvaluationType.GreaterThan,
                                                                        };

        private bool initialized;
        private bool isUpdatingFormulaTypeList;

        private bool isUpdatingCsvMappings;

        private TextBox pendingFocusElement;

        private string previousSelectedCsvMappingFileName;

        private ComboBox csvMappingFileNameComboBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaEditor"/> class.
        /// </summary>
        public FormulaEditor()
        {
            this.InitializeComponent();
            this.Loaded += this.OnLoaded;
            this.DataContextChanged += this.OnDataContextChanged;

            this.Unloaded += this.OnUnload;

            this.evaluationTypeDefinitions = new Lazy<IEnumerable<EvaluationTypeDefinition>>(
                () =>
                {
                    var evaluationTypes = Enum.GetValues(typeof(EvaluationType)).Cast<EvaluationType>();
                    return from n in evaluationTypes
                           where !this.evaluationTypesToIgnore.Contains(n)
                           select new EvaluationTypeDefinition(n);
                });
            this.UpdateFormulaTypeList();
            this.UpdateCsvMappingFileNames();
        }

        /// <summary>
        /// Gets or sets the MaxFormulaDropDownHeight
        /// </summary>
        public double MaxFormulaDropDownHeight
        {
            get
            {
                return (double)this.GetValue(MaxFormulaDropDownHeightProperty);
            }

            set
            {
                this.SetValue(MaxFormulaDropDownHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets the Add operant to property list command
        /// </summary>
        public ICommand AddOperantToPropertyListCommand
        {
            get
            {
                return new RelayCommand(this.AddOperantToPropertyList);
            }
        }

        /// <summary>
        /// Gets the remove property from list command
        /// </summary>
        public ICommand RemovePropertyFromListCommand
        {
            get
            {
                return new RelayCommand(this.RemovePropertyFromList);
            }
        }

        /// <summary>
        /// Gets or sets the dictionary selector command.
        /// </summary>
        public ICommand DictionarySelectorCommand
        {
            get
            {
                return (ICommand)this.GetValue(DictionarySelectorCommandProperty);
            }

            set
            {
                this.SetValue(DictionarySelectorCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the predefined evaluations
        /// </summary>
        public ExtendedObservableCollection<EvaluationConfigDataViewModel> PredefinedEvaluations
        {
            get
            {
                return
                    (ExtendedObservableCollection<EvaluationConfigDataViewModel>)
                    this.GetValue(PredefinedEvaluationsProperty);
            }

            set
            {
                this.SetValue(PredefinedEvaluationsProperty, value);
            }
        }

        /// <summary>
        /// Gets the Formula type list
        /// </summary>
        public ExtendedObservableCollection<EvaluationTypeDefinition> FormulaTypeList
        {
            get
            {
                return (ExtendedObservableCollection<EvaluationTypeDefinition>)this.GetValue(FormulaTypeListProperty);
            }

            private set
            {
                this.SetValue(FormulaTypeListProperty, value);
            }
        }

        /// <summary>
        /// Gets the get time format culture for time picker control.
        /// </summary>
        public CultureInfo GetTimeEvalFormatCulture
        {
            get
            {
                var tempCultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                tempCultureInfo.DateTimeFormat.ShortTimePattern = Settings.Default.TimeEvalConvertTimeSpanFormat;
                return tempCultureInfo;
            }
        }

        /// <summary>
        /// Gets the csv mapping file names.
        /// </summary>
        public ExtendedObservableCollection<string> CsvMappingFileNames
        {
            get
            {
                return (ExtendedObservableCollection<string>)this.GetValue(CsvMappingFileNamesProperty);
            }

            private set
            {
                this.SetValue(CsvMappingFileNamesProperty, value);
            }
        }

        /// <summary>
        /// The refresh function
        /// </summary>
        public void Refresh()
        {
            this.UpdateFormulaTypeList();
            this.UpdateCsvMappingFileNames();
            if (this.DataContext != null)
            {
                var prompt = (FormulaEditorPrompt)this.DataContext;
                this.initialized = this.FormulaComboBox.SelectedItem != null
                    && ((EvaluationTypeDefinition)this.FormulaComboBox.SelectedItem).EvaluationType
                                   == prompt.SelectedEvaluationType;

                if (prompt.SelectedPredefinedFormula != null)
                {
                    this.FormulaComboBox.SelectedItem =
                        this.FormulaTypeList.FirstOrDefault(
                            etd =>
                            etd.EvaluationType == EvaluationType.Evaluation
                            && etd.Name == prompt.SelectedPredefinedFormula.Name.Value);
                }
                else
                {
                    this.FormulaComboBox.SelectedItem =
                        this.FormulaTypeList.FirstOrDefault(etd => etd.EvaluationType == prompt.SelectedEvaluationType);
                }

                prompt.HasPendingChanges = false;
            }
        }

        /// <summary>
        /// handles key up events
        /// </summary>
        /// <param name="e">the key event</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            // only close formular editor on escape
            if (e.Key != Key.Escape)
            {
                e.Handled = true;
            }
        }

        private static void OnPredefinedEvaluationsChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            var editor = (FormulaEditor)sender;

            var oldValue = (ExtendedObservableCollection<EvaluationConfigDataViewModel>)e.OldValue;
            var newValue = (ExtendedObservableCollection<EvaluationConfigDataViewModel>)e.NewValue;
            if (oldValue != null)
            {
                oldValue.CollectionChanged -= editor.OnPredefinedEvaluationsCollectionChanged;
            }

            editor.UpdateFormulaTypeList();

            if (newValue != null)
            {
                newValue.CollectionChanged += editor.OnPredefinedEvaluationsCollectionChanged;
            }
        }

        private static EvalDataViewModelBase CreateEvaluationInstance(
            EvaluationType selectedEvaluationType,
            IMediaShell shell)
        {
            EvalDataViewModelBase result = null;

            var dataViewModelTypeName = string.Empty;
            try
            {
                var evalTypeName = selectedEvaluationType.ToString();

                dataViewModelTypeName = "Gorba.Center.Media.Core.DataViewModels.Eval." + evalTypeName
                                            + "EvalDataViewModel";

                var type = Type.GetType(dataViewModelTypeName);

                if (type != null)
                {
                    result = (EvalDataViewModelBase)Activator.CreateInstance(type, shell, null);
                }
            }
            catch (Exception exception)
            {
                var message = string.Format("Error while trying to create an instance of {0}", dataViewModelTypeName);
                Logger.ErrorException(message, exception);
            }

            return result;
        }

        private void UpdateCsvMappingFileNames()
        {
            var context = (FormulaEditorPrompt)this.DataContext;
            if (context == null)
            {
                return;
            }

            this.isUpdatingCsvMappings = true;
            this.CsvMappingFileNames.Clear();
            foreach (var csvMappingDataViewModel in context.Shell.MediaApplicationState.CurrentProject.CsvMappings)
            {
                this.CsvMappingFileNames.Add(Path.GetFileNameWithoutExtension(csvMappingDataViewModel.Filename.Value));
            }

            // This is needed to be backwards compatible
            this.CsvMappingFileNames.Add("codeconversion");

            this.isUpdatingCsvMappings = false;
            if (!string.IsNullOrEmpty(this.previousSelectedCsvMappingFileName)
                && this.csvMappingFileNameComboBox != null)
            {
                this.csvMappingFileNameComboBox.SelectedItem =
                    this.CsvMappingFileNames.FirstOrDefault(name => name == this.previousSelectedCsvMappingFileName);
            }

            this.previousSelectedCsvMappingFileName = null;
            this.csvMappingFileNameComboBox = null;
        }

        private void UpdateFormulaTypeList()
        {
            EvaluationTypeDefinition previousDefinition = null;
            this.isUpdatingFormulaTypeList = true;
            if (this.FormulaTypeList == null)
            {
                this.FormulaTypeList = new ExtendedObservableCollection<EvaluationTypeDefinition>();
            }
            else
            {
                previousDefinition = (EvaluationTypeDefinition)this.FormulaComboBox.SelectedItem;
                this.FormulaTypeList.Clear();
            }

            foreach (var definition in this.evaluationTypeDefinitions.Value)
            {
                this.FormulaTypeList.Add(definition);
            }

            if (this.PredefinedEvaluations != null)
            {
                foreach (var definition in this.PredefinedEvaluations.Select(pe => new EvaluationTypeDefinition(pe)))
                {
                    this.FormulaTypeList.Add(definition);
                }
            }

            this.FormulaComboBox.SelectedItem = previousDefinition;
            this.isUpdatingFormulaTypeList = false;
        }

        private void OnPredefinedEvaluationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateFormulaTypeList();
        }

        private void OnDataContextChanged(
            object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            this.Refresh();
        }

        /// <summary>
        /// sets the index on load time
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="routedEventArgs">the routed event args</param>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this.Refresh();
            var context = (FormulaEditorPrompt)this.DataContext;
            context.Shell.MediaApplicationState.CurrentProject.CsvMappings.CollectionChanged +=
                this.OnCsvMappingsChanged;
        }

        private void OnCsvMappingsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateCsvMappingFileNames();
        }

        private void OnEvaluationTypeSelected(object sender, RoutedEventArgs e)
        {
            var isFormulaEmpty = false;
            if (this.DataContext == null || this.isUpdatingFormulaTypeList)
            {
                return;
            }

            var prompt = (FormulaEditorPrompt)this.DataContext;
            var selectedEnumerationMember = (EvaluationTypeDefinition)this.FormulaComboBox.SelectedItem;

            if (selectedEnumerationMember != null)
            {
                Logger.Debug(
                    "Changing Formula from '{0}' to '{1}'.",
                    prompt.SelectedEvaluationType,
                    selectedEnumerationMember.EvaluationType);

                prompt.SelectedEvaluationType = selectedEnumerationMember.EvaluationType;

                if (!this.initialized)
                {
                    this.initialized = true;
                    return;
                }

                EvalDataViewModelBase eval = null;

                if (
                    selectedEnumerationMember.Evaluation !=
                  null && selectedEnumerationMember.Evaluation.Evaluation == null)
                {
                    prompt.SelectedEvaluationType = EvaluationType.None;
                    this.FormulaComboBox.SelectedItem =
                        this.FormulaTypeList.FirstOrDefault(f => f.EvaluationType == EvaluationType.None);
                    isFormulaEmpty = true;
                    Logger.Debug(
                   "Predefined Formula '{0}' is empty; changing it back to none.",
                   selectedEnumerationMember.Name);
                }

                if (prompt.SelectedEvaluationType == EvaluationType.Evaluation)
                {
                    eval = new EvaluationEvalDataViewModel(prompt.Shell)
                               {
                                   Reference =
                                       selectedEnumerationMember.Evaluation
                               };
                    prompt.SelectedPredefinedFormula = selectedEnumerationMember.Evaluation;
                }
                else if (prompt.SelectedEvaluationType != EvaluationType.None)
                {
                    eval = CreateEvaluationInstance(prompt.SelectedEvaluationType, prompt.Shell);
                    prompt.SelectedPredefinedFormula = null;
                }
                else
                {
                    prompt.SelectedPredefinedFormula = null;
                }

                prompt.SetEvaluation(eval, isFormulaEmpty);
            }
            else
            {
                prompt.SelectedEvaluationType = EvaluationType.None;
                prompt.DataValue.Formula = null;
            }

            prompt.HasPendingChanges = true;
        }

        private void RemovePropertyFromList(object parameter)
        {
            var aggregation = parameter as ListAndItemAggregationConverter.ListAndItemAggregation;

            if (aggregation != null)
            {
                aggregation.List.Remove(aggregation.Item);
                this.UpdateResultTextBox();
            }
        }

        private void AddOperantToPropertyList(object parameter)
        {
            var prompt = (FormulaEditorPrompt)this.DataContext;
            var list = parameter as IList;
            if (list != null)
            {
                var type = list.GetType().GetGenericArguments()[0];
                switch (type.Name)
                {
                    case "MatchDynamicPropertyDataViewModel":
                        Logger.Trace("Adding a MatchDynamicProperty to formula {0}", prompt.SelectedEvaluationType);
                        list.Add(new MatchDynamicPropertyDataViewModel(prompt.Shell));
                        break;
                    case "CaseDynamicPropertyDataViewModel":
                        Logger.Trace("Adding a CaseDynamicProperty to formula {0}", prompt.SelectedEvaluationType);
                        list.Add(new CaseDynamicPropertyDataViewModel(prompt.Shell));
                        break;
                    default:
                        Logger.Trace("Adding a parameter to formula {0}", prompt.SelectedEvaluationType);
                        list.Add(new EvaluationConfigDataViewModel(prompt.Shell));
                        break;
                }
            }

            prompt.HasPendingChanges = true;
        }

        private void UpdateResultTextBox()
        {
            var bindingExpression =
              BindingOperations.GetBindingExpression(this.ResultTextBlock, TextBlock.TextProperty);
            if (bindingExpression != null)
            {
                bindingExpression.UpdateTarget();
            }

            var prompt = (FormulaEditorPrompt)this.DataContext;
            if (prompt != null)
            {
                prompt.HasPendingChanges = true;
            }
        }

        private void OnArgumentTextBoxKeyUp(object sender, RoutedEventArgs e)
        {
            this.UpdateResultTextBox();
        }

        private void OnOperandGotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                this.pendingFocusElement = textBox;
            }
        }

        private void OnOperandLostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var be = textBox.GetBindingExpression(TextBox.TextProperty);
                if (be != null)
                {
                    be.UpdateSource();
                    this.UpdateResultTextBox();
                }

                this.pendingFocusElement = null;
            }
        }

        private void OnUnload(object sender, RoutedEventArgs routedEventArgs)
        {
            // be sure to trigger focus lost on element so source is updated
            if (this.pendingFocusElement != null)
            {
                this.OnOperandLostFocus(this.pendingFocusElement, new RoutedEventArgs());
            }

            var prompt = (FormulaEditorPrompt)this.DataContext;
            if (prompt == null)
            {
                return;
            }

            var collectionEval = prompt.DataValue.Formula as CollectionEvalDataViewModelBase;
            if (collectionEval != null)
            {
                collectionEval.ClearEmptyConditions();
            }

            var switchEval = prompt.DataValue.Formula as SwitchEvalDataViewModel;
            if (switchEval != null)
            {
                switchEval.ClearEmptyCases();
            }

            var formatEval = prompt.DataValue.Formula as FormatEvalDataViewModel;
            if (formatEval != null)
            {
                formatEval.ClearEmptyArguments();
            }
        }

        private void OnNavigateToMoreInformation(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void OnHelpTextCollapsed(object sender, RoutedEventArgs e)
        {
            this.LastRowDefinition.Height = GridLength.Auto;
        }

        private void OnCheckBoxClicked(object sender, RoutedEventArgs e)
        {
            this.UpdateResultTextBox();
        }

        private void OnTabItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                var tabItem = e.AddedItems.OfType<TabItem>().FirstOrDefault();

                if (tabItem == null)
                {
                    return;
                }

                if (tabItem.Name == "SimpleTabItem")
                {
                    this.Refresh();
                    return;
                }

                var context = (FormulaEditorPrompt)this.DataContext;
                if (context == null)
                {
                    return;
                }

                var evaluationConfig = context.DataValue.Formula as EvaluationConfigDataViewModel;
                if (evaluationConfig != null)
                {
                    context.ExpertEvaluationPart = evaluationConfig.HumanReadable();
                }
                else
                {
                    if (context.DataValue.Formula != null)
                    {
                        context.ExpertEvaluationPart =
                            ((EvalDataViewModelBase)context.DataValue.Formula).HumanReadable();
                    }
                }

                if (tabItem.Name == "ExpertTabItem")
                {
                    if (context.DataValue.Formula != null)
                    {
                        context.DataValue.Formula.IsValid();
                    }
                }
            }
        }

        private void OnExpertTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            var context = (FormulaEditorPrompt)this.DataContext;
            if (context == null)
            {
                return;
            }

            context.HasChangedInExpertMode = true;
        }

        private void OnDateTimeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateResultTextBox();
        }

        private void CsvMappingComboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.isUpdatingCsvMappings && sender is ComboBox)
            {
                this.csvMappingFileNameComboBox = (ComboBox)sender;
                this.previousSelectedCsvMappingFileName = (string)e.RemovedItems[0];
            }
        }
    }
}
