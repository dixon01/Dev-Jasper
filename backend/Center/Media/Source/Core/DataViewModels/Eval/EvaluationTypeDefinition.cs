// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationTypeDefinition.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The EvaluationTypeDefinition.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Resources;

    /// <summary>
    /// the evaluation type definitions
    /// </summary>
    public class EvaluationTypeDefinition
    {
        private const string LocalizationPrefix = "FormulaEditor_Eval_";

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluationTypeDefinition"/> class.
        /// </summary>
        /// <param name="evaluationType">the evaluation Type</param>
        public EvaluationTypeDefinition(EvaluationType evaluationType)
        {
            this.EvaluationType = evaluationType;
            this.Name = MediaStrings.ResourceManager.GetString(LocalizationPrefix + evaluationType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluationTypeDefinition"/> class.
        /// </summary>
        /// <param name="evaluation">the evaluation</param>
        public EvaluationTypeDefinition(EvaluationConfigDataViewModel evaluation)
        {
            this.EvaluationType = EvaluationType.Evaluation;
            this.Name = evaluation.Name.Value;
            this.Evaluation = evaluation;
        }

        /// <summary>
        /// Gets or sets the Evaluation Type
        /// </summary>
        public EvaluationType EvaluationType { get; set; }

        /// <summary>
        /// Gets or sets the Evaluation type name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Evaluation
        /// </summary>
        public EvaluationConfigDataViewModel Evaluation { get; set; }
    }
}