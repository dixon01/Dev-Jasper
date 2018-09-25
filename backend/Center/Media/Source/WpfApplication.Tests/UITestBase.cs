// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UITestBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The UITestBase.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.WpfApplication.Tests
{
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The LayoutElementTests.
    /// </summary>
    [CodedUITest]
    [Ignore]
    public class UITestBase
    {
        /// <summary>
        /// The application under test
        /// </summary>
        private ApplicationUnderTest application;

        private UIMap map;

        /// <summary>
        /// Gets the UI Map
        /// </summary>
        public UIMap UIMap
        {
            get
            {
                if (this.map == null)
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        /// <summary>
        /// Gets or sets the test context
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Gets or sets the application under test
        /// </summary>
        protected ApplicationUnderTest Application
        {
            get
            {
                return this.application;
            }

            set
            {
                this.application = value;
            }
        }

        /// <summary>
        /// The test initialization that starts the application
        /// </summary>
        [TestInitialize]
        [Ignore]
        public void Initialize()
        {
            this.application = ApplicationUnderTest.Launch(
                Path.Combine(TestContext.DeploymentDirectory, @"Gorba.Center.Media.WpfApplication.exe"));

            this.application.WaitForControlReady(2000);
        }

        /// <summary>
        /// The test cleanup function
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            this.application.Close();
        }
    }
}