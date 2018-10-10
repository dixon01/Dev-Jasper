// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   AssemblyInfo.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("159d570b-4220-477f-b2d2-d7cbd714ebc7")]

[assembly: AssemblyTitle("Medi Resource Management")]
[assembly: AssemblyDescription("Message Dispatcher Resource Management Library")]

[assembly: InternalsVisibleTo("Medi.Core.Test")]
[assembly: InternalsVisibleTo("Gorba.Common.Medi.IntegrationTests")]
[assembly: InternalsVisibleTo("Gorba.Common.Medi.Resources.Tests")]