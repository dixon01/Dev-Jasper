// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagePresentationEngineTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PackagePresentationEngineTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation;
    using Gorba.Motion.Infomedia.Core.Presentation.Package;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="PackagePresentationEngine"/>.
    /// </summary>
    [TestClass]
    public class PackagePresentationEngineTest
    {
        /// <summary>
        /// Test for paging.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\paging.im2")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method.")]
        public void PagingTest()
        {
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig(), "U", "A"));

            var configContext = new PresentationConfigContext("paging.im2");
            var context = new PresentationContextMock(configContext.Config);

            for (int i = 0; i < 10; i++)
            {
                context.SetCellValue(new GenericCoordinate(0, 12, 1, i), "Stop " + i);
            }

            var target = new PackagePresentationEngine(configContext.Config.VirtualDisplays[0], null, context);

            target.Start();

            for (int i = 0; i < 2; i++)
            {
                var root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(1, root.Items.Count);

                var item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 0", item.Text);

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(4, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 0", item.Text);

                item = (TextItem)root.Items[1];
                Assert.AreEqual("Stop 1", item.Text);

                item = (TextItem)root.Items[2];
                Assert.AreEqual("Stop 2", item.Text);

                item = (TextItem)root.Items[3];
                Assert.AreEqual("1 / 4", item.Text);

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(4, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 3", item.Text);

                item = (TextItem)root.Items[1];
                Assert.AreEqual("Stop 4", item.Text);

                item = (TextItem)root.Items[2];
                Assert.AreEqual("Stop 5", item.Text);

                item = (TextItem)root.Items[3];
                Assert.AreEqual("2 / 4", item.Text);

                // replace one value and see what happens to the paging
                context.SetCellValue(new GenericCoordinate(0, 12, 1, 4), "4 Replaced");

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(4, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 3", item.Text);

                item = (TextItem)root.Items[1];
                Assert.AreEqual("4 Replaced", item.Text);

                item = (TextItem)root.Items[2];
                Assert.AreEqual("Stop 5", item.Text);

                item = (TextItem)root.Items[3];
                Assert.AreEqual("2 / 4", item.Text);

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(4, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 6", item.Text);

                item = (TextItem)root.Items[1];
                Assert.AreEqual("Stop 7", item.Text);

                item = (TextItem)root.Items[2];
                Assert.AreEqual("Stop 8", item.Text);

                item = (TextItem)root.Items[3];
                Assert.AreEqual("3 / 4", item.Text);

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(4, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 9", item.Text);

                item = (TextItem)root.Items[1];
                Assert.AreEqual(string.Empty, item.Text);

                item = (TextItem)root.Items[2];
                Assert.AreEqual(string.Empty, item.Text);

                item = (TextItem)root.Items[3];
                Assert.AreEqual("4 / 4", item.Text);

                // this shouldn't have any effect, but we need the old value back for the next iteration
                context.SetCellValue(new GenericCoordinate(0, 12, 1, 4), "Stop 4");

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(4, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 9", item.Text);

                item = (TextItem)root.Items[1];
                Assert.AreEqual(string.Empty, item.Text);

                item = (TextItem)root.Items[2];
                Assert.AreEqual(string.Empty, item.Text);

                item = (TextItem)root.Items[3];
                Assert.AreEqual("4 / 4", item.Text);

                context.AddTime(TimeSpan.FromSeconds(10));
            }
        }

        /// <summary>
        /// Test for paging where the paging gets deactivated because the data is cleared.
        /// This is using PageMode.AllPages
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\paging.im2")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method.")]
        public void PagingAllPagesWithClearTest()
        {
            const int NumRows = 10;
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig(), "U", "A"));

            var configContext = new PresentationConfigContext("paging.im2");
            var context = new PresentationContextMock(configContext.Config);

            for (int i = 0; i < NumRows; i++)
            {
                context.SetCellValue(new GenericCoordinate(0, 12, 1, i), "Stop " + i);
            }

            var target = new PackagePresentationEngine(configContext.Config.VirtualDisplays[0], null, context);

            target.Start();

            for (int j = 0; j < 2; j++)
            {
                var root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(1, root.Items.Count);

                var item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 0", item.Text);

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(4, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 0", item.Text);

                item = (TextItem)root.Items[1];
                Assert.AreEqual("Stop 1", item.Text);

                item = (TextItem)root.Items[2];
                Assert.AreEqual("Stop 2", item.Text);

                item = (TextItem)root.Items[3];
                Assert.AreEqual("1 / 4", item.Text);

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(4, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 3", item.Text);

                item = (TextItem)root.Items[1];
                Assert.AreEqual("Stop 4", item.Text);

                item = (TextItem)root.Items[2];
                Assert.AreEqual("Stop 5", item.Text);

                item = (TextItem)root.Items[3];
                Assert.AreEqual("2 / 4", item.Text);

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(4, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 6", item.Text);

                item = (TextItem)root.Items[1];
                Assert.AreEqual("Stop 7", item.Text);

                item = (TextItem)root.Items[2];
                Assert.AreEqual("Stop 8", item.Text);

                item = (TextItem)root.Items[3];
                Assert.AreEqual("3 / 4", item.Text);

                ///////////////////////////////////// Clear Table /////////////////////////////////////
                context.RaiseUpdating(EventArgs.Empty);
                for (int i = 0; i < NumRows; i++)
                {
                    context.SetCellValue(new GenericCoordinate(0, 12, 1, i), string.Empty);
                }

                context.RaiseUpdated(new PresentationUpdatedEventArgs());

                // back to the DefaultLayout
                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(1, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual(string.Empty, item.Text);

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(1, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual(string.Empty, item.Text);

                context.RaiseUpdating(EventArgs.Empty);
                for (int i = 0; i < NumRows; i++)
                {
                    context.SetCellValue(new GenericCoordinate(0, 12, 1, i), "Stop " + i);
                }

                context.RaiseUpdated(new PresentationUpdatedEventArgs());
            }
        }

        /// <summary>
        /// Test for paging where the paging gets deactivated because the data is cleared.
        /// This is using PageMode.OnePage
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\paging.im2")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test method.")]
        public void PagingOnePageWithClearTest()
        {
            const int NumRows = 10;
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig(), "U", "A"));

            var configContext = new PresentationConfigContext("paging.im2");
            var context = new PresentationContextMock(configContext.Config);

            for (int i = 0; i < NumRows; i++)
            {
                context.SetCellValue(new GenericCoordinate(0, 12, 1, i), "Stop " + i);
            }

            var target = new PackagePresentationEngine(configContext.Config.VirtualDisplays[1], null, context);

            target.Start();

            var root = target.CurrentRoot;
            Assert.IsNotNull(root);
            Assert.AreEqual(1, root.Items.Count);

            var item = (TextItem)root.Items[0];
            Assert.AreEqual("Stop 0", item.Text);

            for (int j = 0; j < 2; j++)
            {
                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(4, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 0", item.Text);

                item = (TextItem)root.Items[1];
                Assert.AreEqual("Stop 1", item.Text);

                item = (TextItem)root.Items[2];
                Assert.AreEqual("Stop 2", item.Text);

                item = (TextItem)root.Items[3];
                Assert.AreEqual("1 / 4", item.Text);

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(1, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 0", item.Text);

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(4, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 3", item.Text);

                item = (TextItem)root.Items[1];
                Assert.AreEqual("Stop 4", item.Text);

                item = (TextItem)root.Items[2];
                Assert.AreEqual("Stop 5", item.Text);

                item = (TextItem)root.Items[3];
                Assert.AreEqual("2 / 4", item.Text);

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(1, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 0", item.Text);

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(4, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 6", item.Text);

                item = (TextItem)root.Items[1];
                Assert.AreEqual("Stop 7", item.Text);

                item = (TextItem)root.Items[2];
                Assert.AreEqual("Stop 8", item.Text);

                item = (TextItem)root.Items[3];
                Assert.AreEqual("3 / 4", item.Text);

                ///////////////////////////////////// Clear Table /////////////////////////////////////
                context.RaiseUpdating(EventArgs.Empty);
                for (int i = 0; i < NumRows; i++)
                {
                    context.SetCellValue(new GenericCoordinate(0, 12, 1, i), string.Empty);
                }

                context.RaiseUpdated(new PresentationUpdatedEventArgs());

                // back to the DefaultLayout
                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(1, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual(string.Empty, item.Text);

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(1, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual(string.Empty, item.Text);

                context.RaiseUpdating(EventArgs.Empty);
                for (int i = 0; i < 3; i++)
                {
                    context.SetCellValue(new GenericCoordinate(0, 12, 1, i), "Stop " + i);
                }

                context.RaiseUpdated(new PresentationUpdatedEventArgs());

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(4, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 0", item.Text);

                item = (TextItem)root.Items[1];
                Assert.AreEqual("Stop 1", item.Text);

                item = (TextItem)root.Items[2];
                Assert.AreEqual("Stop 2", item.Text);

                item = (TextItem)root.Items[3];
                Assert.AreEqual("1 / 1", item.Text);

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(1, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 0", item.Text);

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(4, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 0", item.Text);

                item = (TextItem)root.Items[1];
                Assert.AreEqual("Stop 1", item.Text);

                item = (TextItem)root.Items[2];
                Assert.AreEqual("Stop 2", item.Text);

                item = (TextItem)root.Items[3];
                Assert.AreEqual("1 / 1", item.Text);

                ////////////////////////////////////// Next Step //////////////////////////////////////
                context.AddTime(TimeSpan.FromSeconds(10));

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(1, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 0", item.Text);

                context.RaiseUpdating(EventArgs.Empty);
                for (int i = 0; i < NumRows; i++)
                {
                    context.SetCellValue(new GenericCoordinate(0, 12, 1, i), string.Empty);
                }

                context.RaiseUpdated(new PresentationUpdatedEventArgs());

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(1, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual(string.Empty, item.Text);

                context.RaiseUpdating(EventArgs.Empty);
                for (int i = 0; i < NumRows; i++)
                {
                    context.SetCellValue(new GenericCoordinate(0, 12, 1, i), "Stop " + i);
                }

                context.RaiseUpdated(new PresentationUpdatedEventArgs());

                root = target.CurrentRoot;
                Assert.IsNotNull(root);
                Assert.AreEqual(1, root.Items.Count);

                item = (TextItem)root.Items[0];
                Assert.AreEqual("Stop 0", item.Text);
            }
        }
    }
}
