// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TfsTestBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TfsTestBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.RepositoryStructureTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Base class for all TFS tests.
    /// </summary>
    [TestClass]
    public abstract class TfsTestBase
    {
        /// <summary>
        /// Gets the local main folder.
        /// </summary>
        protected DirectoryInfo LocalMainFolder { get; private set; }

        /// <summary>
        /// Initializes the test environment.
        /// </summary>
        [TestInitialize]
        public virtual void Initialize()
        {
            this.LocalMainFolder = GorbaTfs.GetCurrentLocalMainFolder();
            Assert.IsTrue(
                this.LocalMainFolder.Exists,
                "Local mapping of {0} was not found: {1}",
                GorbaTfs.MainPath,
                this.LocalMainFolder.FullName);
        }

        /// <summary>
        /// Gets all subfolders of a given directory that are not named "3rdParty".
        /// </summary>
        /// <param name="directory">
        /// The directory.
        /// </param>
        /// <returns>
        /// all subfolders except "3rdParty".
        /// </returns>
        protected static IEnumerable<DirectoryInfo> GetNonThirdPartyFolders(DirectoryInfo directory)
        {
            return
                directory.GetDirectories().Where(
                    d => !d.Name.Equals(GorbaTfs.ThirdPartyFolder, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Gets all product folders (except "3rdParty")
        /// </summary>
        /// <returns>
        /// all product folders (except "3rdParty").
        /// </returns>
        protected IEnumerable<DirectoryInfo> GetProductFolders()
        {
            return GetNonThirdPartyFolders(this.LocalMainFolder);
        }
    }
}
