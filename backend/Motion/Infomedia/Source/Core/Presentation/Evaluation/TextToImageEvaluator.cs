// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToImageEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Evaluation for an <see cref="TextToImageEval"/>.
    /// </summary>
    public partial class TextToImageEvaluator
    {
        private static readonly Regex SplitWordsRegex = new Regex(@"(\s+)");

        private string[] filePatterns;

        /// <summary>
        /// Evaluation method. Subclasses must implement this method so
        /// it returns the evaluated value.
        /// </summary>
        /// <returns>
        /// the evaluated value.
        /// </returns>
        protected override object CalculateValue()
        {
            var value = this.InnerEvaluator.StringValue;
            var texts = SplitWordsRegex.Split(this.InnerEvaluator.StringValue);
            var convertedText = new StringBuilder(value.Length * 2);
            foreach (var text in texts)
            {
                var part = text;
                foreach (var pattern in this.filePatterns)
                {
                    var fileName = string.Format(pattern, text);
                    fileName = this.Context.Config.GetAbsolutePathRelatedToConfig(fileName);
                    if (!File.Exists(fileName))
                    {
                        continue;
                    }

                    part = string.Format("[img={0}]", fileName);
                    break;
                }

                convertedText.Append(part);
            }

            return convertedText.ToString();
        }

        partial void Initialize()
        {
            this.filePatterns = this.TextToImageEval.FilePatterns.Split(';');
        }
    }
}
