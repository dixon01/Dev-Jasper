// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimationEditorPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The AnimationEditorPrompt.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Common;

    /// <summary>
    /// the Formula Editor Prompt
    /// </summary>
    public class AnimationEditorPrompt : PromptNotification
    {
        private readonly ICommandRegistry commandRegistry;

        private IMediaShell shell;

        private IAnimatedDataValue dataValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationEditorPrompt"/> class.
        /// </summary>
        /// <param name="shell">
        /// the shell
        /// </param>
        /// <param name="dataValue">
        /// the data value
        /// </param>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        public AnimationEditorPrompt(IMediaShell shell, IAnimatedDataValue dataValue, ICommandRegistry commandRegistry)
        {
            this.dataValue = dataValue;
            this.Shell = shell;
            this.commandRegistry = commandRegistry;
            this.Initialize();

            if (this.dataValue.Animation == null)
            {
                this.DataValue.Animation = new AnimationDataViewModel(shell)
                                           {
                                               Type = new DataValue<PropertyChangeAnimationType>(PropertyChangeAnimationType.Linear),
                                               Duration = new DataValue<TimeSpan>(TimeSpan.FromSeconds(1)),
                                           };
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationEditorPrompt"/> class.
        /// </summary>
        /// <param name="shell">
        /// the shell
        /// </param>
        /// <param name="evaluation">
        /// the evaluation
        /// </param>
        /// <param name="commandRegistry">
        /// The command Registry.
        /// </param>
        public AnimationEditorPrompt(IMediaShell shell, EvaluationConfigDataViewModel evaluation, ICommandRegistry commandRegistry)
            : this(shell, new AnimatedDynamicDataValue<string> { Formula = evaluation.Evaluation }, commandRegistry)
        {
        }

        /// <summary>
        /// Gets or sets the current DataValue
        /// </summary>
        public IAnimatedDataValue DataValue
        {
            get
            {
                return this.dataValue;
            }

            set
            {
                this.SetProperty(ref this.dataValue, value, () => this.DataValue);
            }
        }

        /// <summary>
        /// Gets the media shell
        /// </summary>
        public IMediaShell Shell { get; private set; }

        private void Initialize()
        {
            if (this.dataValue.Animation == null)
            {
                return;
            }
        }
    }
}