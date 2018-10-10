// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EPaperContentProviderTests.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests related to the EPaperContentProvider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Tests.Dynamic.EPaper
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Dynamic.EPaper;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Dynamic;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Common.Utility.Core;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests related to the EPaperContentProvider.
    /// </summary>
    [TestClass]
    public class EPaperContentProviderTests
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            DependencyResolver.Reset();
        }

        /// <summary>
        /// Cleans the test up.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
           DependencyResolver.Reset();
        }

        /// <summary>
        /// Tests that a persistent resource is added to the system.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Dynamic\Rss10.txt")]
        [SuppressMessage(
            "StyleCop.CSharp.ReadabilityRules",
            "SA1110:OpeningParenthesisMustBeOnDeclarationLine",
            Justification = "Reviewed. Suppression is OK here.")]
        public void PersistentFileTest()
        {
            TimerFactory.Current = new TestableTimerFactory();
            RegisterContentResourceDataServiceMock();
            RegisterConverterMock();

            var factory = new ProducerConsumerQueueFactory<MainUnitUpdateCommandUpdater.EnqueuedUpdate>
                .ImmediateProducerConsumerQueueFactory();

            ProducerConsumerQueueFactory<MainUnitUpdateCommandUpdater.EnqueuedUpdate>.Set(factory);

            var commandUpdater = RegisterMainUnitUpdateCommandUpdater();

            var contentResourceServiceMock = new Mock<IContentResourceDataService>();
            var result = new ContentResource();
            var resultTask = new TaskCompletionSource<ContentResource>();
            resultTask.SetResult(result);
            contentResourceServiceMock.Setup(
                service => service.AddAsync(It.IsAny<ContentResource>())).Returns(resultTask.Task);

            var queryResultTask = new TaskCompletionSource<IEnumerable<ContentResource>>();
            queryResultTask.SetResult(Enumerable.Empty<ContentResource>());
            contentResourceServiceMock.Setup(service => service.QueryAsync(It.IsAny<ContentResourceQuery>()))
                .Returns(queryResultTask.Task);

            DependencyResolver.Current.Register(contentResourceServiceMock.Object);
            var configPart = new EPaperDynamicContentPart
                                 {
                                     MainUnitId = 677,
                                     DisplayUnitIndex = 2,
                                     Url = Path.Combine(this.TestContext.DeploymentDirectory, "Rss10.txt"),
                                     IsPersistentFile = true,
                                     StaticFileSourceHash = "THISISSOMETHING"
                                 };
            var updateGroup = new UpdateGroup { Name = "TestGroup" };

            // ReSharper disable once UnusedVariable
            var provider = new EPaperContentProvider(new UpdateGroupReadableModel(updateGroup), configPart);
            provider.Start();
            provider.Stop();
            commandUpdater.Verify(s => s.EnqueueDisplayContent(677, 2, "THISISSOMETHING"), Times.Once());
        }

        private static Mock<IMainUnitUpdateCommandUpdater> RegisterMainUnitUpdateCommandUpdater()
        {
            var commandUpdater = new Mock<IMainUnitUpdateCommandUpdater>();
            DependencyResolver.Current.Register(commandUpdater.Object);
            return commandUpdater;
        }

        private static Mock<IEPaperConverter> RegisterConverterMock()
        {
            var converter = new Mock<IEPaperConverter>();
            DependencyResolver.Current.Register(converter.Object);
            return converter;
        }

        private static Mock<IContentResourceDataService> RegisterContentResourceDataServiceMock()
        {
            var contentResourceDataServiceMock = new Mock<IContentResourceDataService>();
            contentResourceDataServiceMock.Setup(s => s.QueryAsync(It.IsAny<ContentResourceQuery>()))
                .Returns<ContentResourceQuery>(
                    q => Task.FromResult((IEnumerable<ContentResource>)new List<ContentResource>()));
            DependencyResolver.Current.Register(contentResourceDataServiceMock.Object);
            return contentResourceDataServiceMock;
        }
    }
}