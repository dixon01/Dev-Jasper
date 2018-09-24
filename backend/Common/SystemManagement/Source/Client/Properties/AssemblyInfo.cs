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
using System.Runtime.InteropServices;

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("6BFAE0A9-DFB2-4CB0-A283-B69FF5903048")]
[assembly: ComVisibleAttribute(false)]

[assembly: AssemblyTitle("System Management Client")]
[assembly: AssemblyDescription("System Management Client Library")]

[assembly: InternalsVisibleTo("Gorba.Common.SystemManagement.Core")]
[assembly: InternalsVisibleTo("Gorba.Common.SystemManagement.Core.CF20")]
[assembly: InternalsVisibleTo("Gorba.Common.SystemManagement.Core.CF35")]
[assembly: InternalsVisibleTo("Gorba.Motion.Update.Core.Tests")]
