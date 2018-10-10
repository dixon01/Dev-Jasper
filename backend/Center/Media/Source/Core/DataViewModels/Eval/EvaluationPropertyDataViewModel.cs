// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationPropertyDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The EvaluationPropertyDataViewModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;

    /// <summary>
    /// The EvaluationPropertyDataViewModel.
    /// </summary>
    public class EvaluationPropertyDataViewModel : DataViewModelBase
    {
        private string name;

        private ReflectionWrapper evaluation;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.SetProperty(ref this.name, value, () => this.Name);
            }
        }

        /// <summary>
        /// Gets or sets the Evaluation
        /// </summary>
        public ReflectionWrapper Evaluation
        {
            get
            {
                return this.evaluation;
            }

            set
            {
                this.SetProperty(ref this.evaluation, value, () => this.Evaluation);
            }
        }
    }
}