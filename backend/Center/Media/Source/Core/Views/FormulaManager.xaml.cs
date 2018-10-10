// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaManager.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The FormulaManager.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// Interaction logic for FormulaManager.xaml
    /// </summary>
    public partial class FormulaManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaManager"/> class.
        /// </summary>
        public FormulaManager()
        {
            this.InitializeComponent();
            this.Unloaded += (sender, args) =>
                {
                    if (!(this.DataContext is FormulaManagerPrompt context))
                    {
                        return;
                    }

                    if (context.CurrentFormulaPrompt != null && context.CurrentFormulaPrompt.HasPendingChanges)
                    {
                        this.ExecuteUpdateCommand(context);
                    }
                };
        }

        private void OnSelectedPredefinedFormulaChanged(object sender, SelectionChangedEventArgs e)
        {
            // FormulaEditor.Refresh();
        }

        private void PredefinedFormulaListOnFocus(object sender, RoutedEventArgs e)
        {
            var context = (FormulaManagerPrompt)this.DataContext;
            if (context != null)
            {
                if (context.CurrentFormulaPrompt != null
                    && (context.CurrentFormulaPrompt.HasPendingChanges
                        || context.CurrentFormulaPrompt.HasChangedInExpertMode))
                {
                    this.ExecuteUpdateCommand(context);
                }
            }
        }

        private void ExecuteUpdateCommand(FormulaManagerPrompt context)
        {
            var oldElements = new List<DataViewModelBase> { context.CurrentUnchangedEvaluation };
            var newElements = new List<DataViewModelBase>
                                  {
                                      (EvaluationConfigDataViewModel)
                                      context.CurrentEvaluation.Clone()
                                  };
            if (oldElements.First().EqualsViewModel(newElements.First()))
            {
                if (context.CurrentUnchangedEvaluation.Evaluation == null &&
                    context.CurrentEvaluation.Evaluation == null)
                {
                    return;
                }

                if (context.CurrentUnchangedEvaluation.Evaluation != null &&
                    context.CurrentEvaluation.Evaluation != null)
                {
                    if (
                        context.CurrentUnchangedEvaluation.Evaluation.EqualsViewModel(
                            context.CurrentEvaluation.Evaluation))
                    {
                        return;
                    }
                }
            }

            var parameters = new UpdateEntityParameters(oldElements, newElements, context.PredefinedFormulas);
            context.UpdatePredefinedFormula.Execute(parameters);
            if (context.CurrentFormulaPrompt != null)
            {
                context.CurrentFormulaPrompt.HasPendingChanges = false;
                context.CurrentFormulaPrompt.HasChangedInExpertMode = false;
            }
        }
    }
}
