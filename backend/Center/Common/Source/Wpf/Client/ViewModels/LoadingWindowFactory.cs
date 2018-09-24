// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadingWindowFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.ViewModels
{
    using System.ComponentModel.Composition;

    using Gorba.Center.Common.Wpf.Client.Views;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// The loading window factory.
    /// </summary>
    [Export]
    public class LoadingWindowFactory : WindowFactory<LoadingWindow>
    {
    }
}
