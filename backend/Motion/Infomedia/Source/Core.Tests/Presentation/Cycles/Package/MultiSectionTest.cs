// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiSectionTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Cycles.Package
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="MultiSection"/>.
    /// </summary>
    [TestClass]
    public class MultiSectionTest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Initializes MessageDispatcher before the tests
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig(), "U", "A"));
        }

        /// <summary>
        /// A test for Activate
        /// </summary>
        [TestMethod]
        public void ActivateTest()
        {
            var config = new MultiSectionConfig
                {
                    Duration = TimeSpan.FromSeconds(10),
                    Enabled = true,
                    Layout = "Layout",
                    Language = 1,
                    Table = 11,
                    MaxPages = 10,
                    RowsPerPage = 5,
                    Mode = PageMode.AllPages
                };

            var context = new PresentationContextMock(new LayoutConfig { Name = "Layout" });
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 0 }, "Hello");
            var target = new MultiSection(config, null, context);

            bool actual = target.Activate();

            Assert.IsTrue(actual);

            var page = target.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("Layout", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);
            Assert.AreEqual(0, page.PageIndex);
            Assert.AreEqual(1, page.TotalPages);
            Assert.AreEqual(0, page.RowOffset);
            Assert.AreEqual(1, page.LanguageIndex);
            Assert.AreEqual(11, page.TableIndex);

            target.Dispose();
        }

        /// <summary>
        /// A test for ShowNextObject
        /// </summary>
        [TestMethod]
        public void ShowNextPage_EmptyPage_Test()
        {
            var config = new MultiSectionConfig
            {
                Duration = TimeSpan.FromSeconds(10),
                Enabled = true,
                Layout = "Layout",
                Language = 1,
                Table = 11,
                MaxPages = 10,
                RowsPerPage = 5,
                Mode = PageMode.AllPages
            };

            var context = new PresentationContextMock(new LayoutConfig { Name = "Layout" });
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 0 }, "A");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 1 }, "B");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 2 }, "C");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 3 }, "D");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 4 }, "E");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 10 }, "F");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 11 }, "G");
            var target = new MultiSection(config, null, context);

            for (int i = 0; i < 2; i++)
            {
                bool actual = target.Activate();
                Assert.IsTrue(actual);

                var page = target.CurrentObject;
                Assert.IsNotNull(page);
                Assert.IsNotNull(page.Layout);
                Assert.AreEqual("Layout", page.Layout.Name);
                Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);
                Assert.AreEqual(0, page.PageIndex);
                Assert.AreEqual(2, page.TotalPages);
                Assert.AreEqual(0, page.RowOffset);
                Assert.AreEqual(1, page.LanguageIndex);
                Assert.AreEqual(11, page.TableIndex);

                actual = target.ShowNextObject();
                Assert.IsTrue(actual);

                page = target.CurrentObject;
                Assert.IsNotNull(page);
                Assert.IsNotNull(page.Layout);
                Assert.AreEqual("Layout", page.Layout.Name);
                Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);
                Assert.AreEqual(1, page.PageIndex);
                Assert.AreEqual(2, page.TotalPages);
                Assert.AreEqual(10, page.RowOffset);
                Assert.AreEqual(1, page.LanguageIndex);
                Assert.AreEqual(11, page.TableIndex);

                actual = target.ShowNextObject();
                Assert.IsFalse(actual);

                target.Deactivate();
            }

            target.Dispose();
        }

        /// <summary>
        /// A test for ShowNextObject using Mode=OnePage
        /// </summary>
        [TestMethod]
        public void ShowNextPage_OnePage_Test()
        {
            var config = new MultiSectionConfig
                {
                    Duration = TimeSpan.FromSeconds(10),
                    Enabled = true,
                    Layout = "Layout",
                    Language = 1,
                    Table = 11,
                    MaxPages = 10,
                    RowsPerPage = 5,
                    Mode = PageMode.OnePage
                };

            var context = new PresentationContextMock(new LayoutConfig { Name = "Layout" });
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 0 }, "A");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 1 }, "B");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 2 }, "C");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 3 }, "D");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 4 }, "E");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 5 }, "F");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 6 }, "G");
            var target = new MultiSection(config, null, context);

            for (int i = 0; i < 2; i++)
            {
                bool actual = target.Activate();
                Assert.IsTrue(actual, "Round " + i);

                var page = target.CurrentObject;
                Assert.IsNotNull(page);
                Assert.IsNotNull(page.Layout);
                Assert.AreEqual("Layout", page.Layout.Name);
                Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);
                Assert.AreEqual(0, page.PageIndex);
                Assert.AreEqual(2, page.TotalPages);
                Assert.AreEqual(0, page.RowOffset);
                Assert.AreEqual(1, page.LanguageIndex);
                Assert.AreEqual(11, page.TableIndex);

                actual = target.ShowNextObject();
                Assert.IsFalse(actual);

                target.Deactivate();
                actual = target.Activate();
                Assert.IsTrue(actual);

                page = target.CurrentObject;
                Assert.IsNotNull(page);
                Assert.IsNotNull(page.Layout);
                Assert.AreEqual("Layout", page.Layout.Name);
                Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);
                Assert.AreEqual(1, page.PageIndex);
                Assert.AreEqual(2, page.TotalPages);
                Assert.AreEqual(5, page.RowOffset);
                Assert.AreEqual(1, page.LanguageIndex);
                Assert.AreEqual(11, page.TableIndex);

                actual = target.ShowNextObject();
                Assert.IsFalse(actual);

                target.Deactivate();
            }

            target.Dispose();
        }

        /// <summary>
        /// A test for ShowNextObject using Mode=AllPages
        /// </summary>
        [TestMethod]
        public void ShowNextPage_AllPages_Test()
        {
            var config = new MultiSectionConfig
            {
                Duration = TimeSpan.FromSeconds(10),
                Enabled = true,
                Layout = "Layout",
                Language = 1,
                Table = 11,
                MaxPages = 10,
                RowsPerPage = 5,
                Mode = PageMode.AllPages
            };

            var context = new PresentationContextMock(new LayoutConfig { Name = "Layout" });
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 0 }, "A");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 1 }, "B");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 2 }, "C");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 3 }, "D");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 4 }, "E");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 5 }, "F");
            context.SetCellValue(new GenericCoordinate { Language = 1, Table = 11, Column = 0, Row = 6 }, "G");
            var target = new MultiSection(config, null, context);

            for (int i = 0; i < 2; i++)
            {
                bool actual = target.Activate();
                Assert.IsTrue(actual);

                var page = target.CurrentObject;
                Assert.IsNotNull(page);
                Assert.IsNotNull(page.Layout);
                Assert.AreEqual("Layout", page.Layout.Name);
                Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);
                Assert.AreEqual(0, page.PageIndex);
                Assert.AreEqual(2, page.TotalPages);
                Assert.AreEqual(0, page.RowOffset);
                Assert.AreEqual(1, page.LanguageIndex);
                Assert.AreEqual(11, page.TableIndex);

                actual = target.ShowNextObject();
                Assert.IsTrue(actual);

                page = target.CurrentObject;
                Assert.IsNotNull(page);
                Assert.IsNotNull(page.Layout);
                Assert.AreEqual("Layout", page.Layout.Name);
                Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);
                Assert.AreEqual(1, page.PageIndex);
                Assert.AreEqual(2, page.TotalPages);
                Assert.AreEqual(5, page.RowOffset);
                Assert.AreEqual(1, page.LanguageIndex);
                Assert.AreEqual(11, page.TableIndex);

                actual = target.ShowNextObject();
                Assert.IsFalse(actual);

                target.Deactivate();
            }

            target.Dispose();
        }

        // ReSharper restore InconsistentNaming
    }
}