// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublishDocumentWritableModelParameters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The create project parameters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    using System;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;

    /// <summary>
    /// The create project parameters.
    /// </summary>
    public class PublishDocumentWritableModelParameters
    {
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public DocumentWritableModel Model { get; set; }

        /// <summary>
        /// Gets or sets the set busy action. This provides feedback for the UI.
        /// </summary>
        public Action<bool, object, object> OnFinished { get; set; }

        /// <summary>
        /// Gets or sets the error callback action. UI can react on errors.
        /// </summary>
        public Action<Exception> ErrorCallbackAction { get; set; }

        /// <summary>
        /// Gets or sets the old values.
        /// </summary>
        public object OldValues { get; set; }

        /// <summary>
        /// Gets or sets the new values.
        /// </summary>
        public object NewValues { get; set; }
    }
}
