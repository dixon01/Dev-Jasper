// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionProject.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionProject type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.RepositoryStructureTests.Utility
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// Wrapper around Microsoft.Build.Construction.ProjectInSolution
    /// Thanks to http://stackoverflow.com/questions/707107/library-for-parsing-visual-studio-solution-files
    /// </summary>
    [DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectGuid}")]
    public class SolutionProject
    {
        private static readonly Type ProjectInSolutionType;

        private static readonly PropertyInfo ProjectNameProperty;
        private static readonly PropertyInfo ProjectTypeProperty;
        private static readonly PropertyInfo RelativePathProperty;
        private static readonly PropertyInfo ProjectGuidProperty;
        private static readonly PropertyInfo ParentProjectGuidProperty;

        static SolutionProject()
        {
            ProjectInSolutionType =
                Type.GetType("Microsoft.Build.Construction.ProjectInSolution, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true);
            if (ProjectInSolutionType != null)
            {
                ProjectNameProperty = ProjectInSolutionType.GetProperty(
                    "ProjectName", BindingFlags.NonPublic | BindingFlags.Instance);
                ProjectTypeProperty = ProjectInSolutionType.GetProperty(
                    "ProjectType", BindingFlags.NonPublic | BindingFlags.Instance);
                RelativePathProperty = ProjectInSolutionType.GetProperty(
                    "RelativePath", BindingFlags.NonPublic | BindingFlags.Instance);
                ProjectGuidProperty = ProjectInSolutionType.GetProperty(
                    "ProjectGuid", BindingFlags.NonPublic | BindingFlags.Instance);
                ParentProjectGuidProperty = ProjectInSolutionType.GetProperty(
                    "ParentProjectGuid", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionProject"/> class.
        /// </summary>
        /// <param name="solutionProject">
        /// The solution project, an instance of <see cref="Microsoft.Build.Construction.SolutionParser"/>.
        /// </param>
        public SolutionProject(object solutionProject)
        {
            this.ProjectName = ProjectNameProperty.GetValue(solutionProject, null) as string;
            this.ProjectType = (SolutionProjectType)Convert.ToInt32(ProjectTypeProperty.GetValue(solutionProject, null));
            this.RelativePath = RelativePathProperty.GetValue(solutionProject, null) as string;
            this.ProjectGuid = ProjectGuidProperty.GetValue(solutionProject, null) as string;
            this.ParentProjectGuid = ParentProjectGuidProperty.GetValue(solutionProject, null) as string;
        }

        /// <summary>
        /// Gets the project name.
        /// </summary>
        public string ProjectName { get; private set; }

        /// <summary>
        /// Gets the type of this project.
        /// </summary>
        public SolutionProjectType ProjectType { get; private set; }

        /// <summary>
        /// Gets the realtive path.
        /// </summary>
        public string RelativePath { get; private set; }

        /// <summary>
        /// Gets the project GUID.
        /// </summary>
        public string ProjectGuid { get; private set; }

        /// <summary>
        /// Gets the parent project GUID.
        /// </summary>
        public string ParentProjectGuid { get; private set; }
    }
}