// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagShellFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DiagShellFactory.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels
{
    using System.ComponentModel.Composition;

    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Diag.Core.Views;

    /// <summary>
    /// Factory to create <see cref="DiagShellWindow"/>s.
    /// </summary>
    [Export]
    public class DiagShellFactory : WindowFactory<DiagShellWindow>
    {
    }
}