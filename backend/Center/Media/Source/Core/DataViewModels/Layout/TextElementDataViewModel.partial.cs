// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextElementDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Engines;
    using Gorba.Motion.Infomedia.BbCode;

    /// <summary>
    /// Partial implementation of the TextElementDataViewModel.
    /// </summary>
    public partial class TextElementDataViewModel
    {
        /// <summary>
        /// The unset media reference.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public override void UnsetMediaReference(ICommandRegistry commandRegistry)
        {
            if (this.FontFace == null || this.FontFace.Value == null)
            {
                return;
            }

            var userFont =
                this.mediaShell.MediaApplicationState.CurrentProject.Resources.FirstOrDefault(
                    r => r.Facename == this.FontFace.Value);
            if (userFont != null)
            {
                this.DecreaseMediaReferenceByHash(userFont.Hash, commandRegistry);
            }

            this.ResourceManager.TextElementManager.UnsetReferences(this);
        }

        /// <summary>
        /// The set media reference.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public override void SetMediaReference(ICommandRegistry commandRegistry)
        {
            if (this.FontFace == null || this.FontFace.Value == null)
            {
                return;
            }

            var userFont =
                this.mediaShell.MediaApplicationState.CurrentProject.Resources.FirstOrDefault(
                    r => r.Facename == this.FontFace.Value);
            if (userFont != null)
            {
                this.IncreaseMediaReferenceByHash(userFont.Hash, commandRegistry);
            }

            this.ResourceManager.TextElementManager.SetReferences(this);
        }

        /// <summary>
        /// Gets the component to be rendered.
        /// </summary>
        /// <returns>
        /// The <see cref="ComponentBase"/>.
        /// </returns>
        public override ComponentBase GetComponent()
        {
            var model = (TextElement)this.Export();

            if (this.Shell.MediaApplicationState.CurrentProject == null)
            {
                return null;
            }

            string text;
            if (model.ValueProperty != null && model.ValueProperty.Evaluation != null)
            {
                text = ((EvalDataViewModelBase)this.Value.Formula).HumanReadable();
            }
            else
            {
                text = model.Value ?? string.Empty;
            }

            text = BbParser.EscapeBbCode(text);

            return new TextComponent
            {
                Align = model.Align,
                VAlign = model.VAlign,
                Width = model.Width,
                Height = model.Height,
                Overflow = TextOverflow.Clip,
                Font = this.Font.Export(),
                ScrollSpeed = model.ScrollSpeed,
                Visible = model.Visible,
                ZIndex = model.ZIndex,
                X = model.X,
                Y = model.Y,
                Text = text
            };
        }

        partial void Initialize(TextElementDataModel dataModel)
        {
            if (dataModel == null)
            {
                this.ScrollSpeed.Value = -200;
            }
        }

        partial void FontChangedPartial(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.FontFace);
            this.RaisePropertyChanged(() => this.FontHeight);
            this.RaisePropertyChanged(() => this.FontWeight);
            this.RaisePropertyChanged(() => this.FontColor);
            this.RaisePropertyChanged(() => this.FontItalic);
        }
    }
}