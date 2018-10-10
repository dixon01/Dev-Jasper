// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   AssemblyInfo.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("Medi Core")]
[assembly: AssemblyDescription("Message Dispatcher Core Library")]

[assembly: InternalsVisibleTo("Gorba.Common.Medi.Bluetooth")]
[assembly: InternalsVisibleTo("Gorba.Common.Medi.Ports")]
[assembly: InternalsVisibleTo("Gorba.Common.Medi.Resources")]
[assembly: InternalsVisibleTo("Gorba.Common.Medi.Core.Tests")]
[assembly: InternalsVisibleTo("Gorba.Common.Medi.Resources.Tests")]
[assembly: InternalsVisibleTo("Gorba.Common.Medi.IntegrationTests")]

[assembly: InternalsVisibleTo("Gorba.Common.Medi.Ports.CF20")]
[assembly: InternalsVisibleTo("Gorba.Common.Medi.Resources.CF20")]

[assembly: InternalsVisibleTo("Gorba.Common.Medi.Ports.CF35")]
[assembly: InternalsVisibleTo("Gorba.Common.Medi.Resources.CF35")]
