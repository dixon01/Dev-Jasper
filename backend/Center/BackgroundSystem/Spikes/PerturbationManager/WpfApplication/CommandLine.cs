// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLine.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   CommandLine.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WpfApplication
{
    using CommandLineParser.Arguments;

    public class CommandLine
    {
        [SwitchArgument('s', false, LongName = "start")]
        public bool Start { get; set; }
    }
}
