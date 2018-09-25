// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionParser.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionParser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.RepositoryStructureTests.Utility
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Wrapper around the internal class Microsoft.Build.Construction.SolutionParser
    /// Thanks to http://stackoverflow.com/questions/707107/library-for-parsing-visual-studio-solution-files
    /// </summary>
    public class SolutionParser
    {
        private static readonly Type SolutionParserType;

        private static readonly PropertyInfo SolutionReaderProperty;

        private static readonly PropertyInfo ProjectsProperty;

        private static readonly MethodInfo ParseSolutionMethod;

        private readonly object wrappedInstance;

        static SolutionParser()
        {
            SolutionParserType =
                Type.GetType("Microsoft.Build.Construction.SolutionParser, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true);
            SolutionReaderProperty = SolutionParserType.GetProperty(
                "SolutionReader", BindingFlags.NonPublic | BindingFlags.Instance);
            ProjectsProperty = SolutionParserType.GetProperty(
                "Projects", BindingFlags.NonPublic | BindingFlags.Instance);
            ParseSolutionMethod = SolutionParserType.GetMethod(
                "ParseSolution", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionParser"/> class.
        /// </summary>
        public SolutionParser()
        {
            this.wrappedInstance =
                SolutionParserType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);
        }

        /// <summary>
        /// Gets all projects.
        /// </summary>
        public SolutionProject[] Projects { get; private set; }

        /// <summary>
        /// Loads the solution from the given reader.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        public void Load(StreamReader reader)
        {
            SolutionReaderProperty.SetValue(this.wrappedInstance, reader, null);
            ParseSolutionMethod.Invoke(this.wrappedInstance, null);

            var array = (Array)ProjectsProperty.GetValue(this.wrappedInstance, null);
            var projects = new SolutionProject[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                projects[i] = new SolutionProject(array.GetValue(i));
            }

            this.Projects = projects;
        }
    }
}
