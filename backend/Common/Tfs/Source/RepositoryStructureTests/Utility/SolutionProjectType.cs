// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionProjectType.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionProjectType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.RepositoryStructureTests.Utility
{
    /// <summary>
    /// Public "wrapper" for the internal <see cref="Microsoft.Build.Construction.SolutionProjectType"/> enum.
    /// </summary>
    public enum SolutionProjectType
    {
        /// <summary>
        /// Unknown project type
        /// </summary>
        Unknown,

        /// <summary>
        /// MS build compatible project type
        /// </summary>
        KnownToBeMSBuildFormat,

        /// <summary>
        /// Solution folder
        /// </summary>
        SolutionFolder,

        /// <summary>
        /// Web project
        /// </summary>
        WebProject,

        /// <summary>
        /// Web deployment project
        /// </summary>
        WebDeploymentProject,

        /// <summary>
        /// ETP sub-project
        /// </summary>
        EtpSubProject
    }
}