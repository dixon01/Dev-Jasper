// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioStaticTTSLayoutElement.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.LayoutElements
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Interaction logic for AudioStaticTTSLayoutElement.xaml
    /// </summary>
    public partial class AudioStaticTtsLayoutElement
    {
        private List<LayoutElementDataViewModelBase> oldElements;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioStaticTtsLayoutElement"/> class.
        /// </summary>
        public AudioStaticTtsLayoutElement()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The enter edit mode.
        /// </summary>
        protected override void EnterEditMode()
        {
            base.EnterEditMode();
            var mediaShell = (MediaShell)ServiceLocator.Current.GetInstance(typeof(IMediaShell));
            var elements = mediaShell.Editor.CurrentAudioOutputElement.Elements;
            this.oldElements = new List<LayoutElementDataViewModelBase>();
            this.oldElements.AddRange(elements.Select(element => (LayoutElementDataViewModelBase)element.Clone()));
        }

        /// <summary>
        /// The exit edit mode.
        /// </summary>
        protected override void ExitEditMode()
        {
            base.ExitEditMode();

            var textbox = this.TextContainer;
            var dataconext = (TextToSpeechElementDataViewModel)this.DataContext;
            var mediaShell = (MediaShell)ServiceLocator.Current.GetInstance(typeof(IMediaShell));
            if (dataconext.Value.Value.Equals(textbox.Text))
            {
                return;
            }

            var elements = mediaShell.Editor.CurrentAudioOutputElement.Elements;
            var oldElement =
                (TextToSpeechElementDataViewModel)
                elements.FirstOrDefault(
                    e => ((TextToSpeechElementDataViewModel)e).Value.Value.Equals(dataconext.Value.Value));
            if (oldElement == null)
            {
                return;
            }

            var newElement = (LayoutElementDataViewModelBase)oldElement.Clone();
            ((TextToSpeechElementDataViewModel)newElement).Value.Value = textbox.Text;
            var newElements = new List<LayoutElementDataViewModelBase> { newElement };

            var editor = mediaShell.Editor as EditorViewModelBase;

            var parameters = new UpdateEntityParameters(
                this.oldElements,
                newElements,
                mediaShell.Editor.CurrentAudioOutputElement.Elements);
            if (editor != null)
            {
                editor.UpdateElementCommand.Execute(parameters);
            }
        }
    }
}
