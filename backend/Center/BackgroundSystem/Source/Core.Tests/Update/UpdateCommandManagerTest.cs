// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateCommandManagerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateControllerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Tests.Update
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Update;
    using Gorba.Center.BackgroundSystem.Data.Access;
    using Gorba.Center.BackgroundSystem.Data.Model;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Center.Common.ServiceModel.Meta;
    using Gorba.Center.Common.ServiceModel.Settings;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Common.Configuration.Update.Providers;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Core;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using UpdateCommandDto = Gorba.Center.Common.ServiceModel.Update.UpdateCommand;
    using UpdateCommandMsg = Gorba.Common.Update.ServiceModel.Messages.UpdateCommand;
    using UpdateStateDto = Gorba.Center.Common.ServiceModel.Update.UpdateState;
    using UpdateStateMsg = Gorba.Common.Update.ServiceModel.Messages.UpdateState;
    using XmlData = Gorba.Center.Common.ServiceModel.XmlData;

    /// <summary>
    /// Unit tests for <see cref="UpdateCommandManager"/>.
    /// </summary>
    [TestClass]
    public class UpdateCommandManagerTest
    {
        private static readonly TimeSpan AsyncTimeout = TimeSpan.FromMilliseconds(500);

        private readonly List<UpdateCommandDto> commands = new List<UpdateCommandDto>();

        private readonly List<UpdateFeedback> feedbacks = new List<UpdateFeedback>();

        private Mock<IUpdateCommandDataService> updateCommandDataServiceMock;

        private Mock<IUpdateFeedbackDataService> updateFeedbackDataServiceMock;

        /// <summary>
        /// Initializes the test resetting the <see cref="DependencyResolver"/> and the services.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            DependencyResolver.Reset();
            this.commands.Clear();
            this.updateCommandDataServiceMock = this.CreateUpdateCommandDataService();
            DependencyResolver.Current.Register(this.updateCommandDataServiceMock.Object);

            this.updateFeedbackDataServiceMock = this.CreateUpdateFeedbackDataService();
            DependencyResolver.Current.Register(this.updateFeedbackDataServiceMock.Object);

            var systemConfigDataService = this.CreateSystemConfigDataService();
            DependencyResolver.Current.Register(systemConfigDataService.Object);
        }

        /// <summary>
        /// Cleans the system up after each test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            DependencyResolver.Reset();
            this.commands.Clear();
            this.feedbacks.Clear();
        }

            /// <summary>
        /// Test that adds a single update part and makes sure it creates commands for the right units.
        /// </summary>
        [TestMethod]
        public void TestAddUpdatePart()
        {
            var now = new DateTime(2012, 12, 12, 15, 23, 17, DateTimeKind.Utc);
            var timeProvider = new Mock<TimeProvider>();
            timeProvider.SetupGet(t => t.UtcNow).Returns(now);
            TimeProvider.Current = timeProvider.Object;

            var units = new List<Unit>
                            {
                                new Unit { Name = "A", Id = 1 },
                                new Unit { Name = "B", Id = 2 },
                                new Unit { Name = "C", Id = 3 }
                            };
                var groups = new List<UpdateGroup>
                                 {
                                     new UpdateGroup
                                         {
                                             Name = "Test",
                                             Units = new List<Unit>(),
                                             UpdateParts = new List<UpdatePart>()
                                         }
                                 };
            foreach (var unit in units.Take(2))
            {
                groups[0].Units.Add(unit);
                unit.UpdateGroup = groups[0];
            }

            var target = new UpdateCommandManager();

            var newPart = new UpdatePart
                              {
                                  Start = now.AddDays(-1),
                                  End = now.AddDays(1),
                                  UpdateGroup = groups[0],
                                  Type = UpdatePartType.Setup,
                                  Structure =
                                          new XmlData(CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe"))
                              };
            groups[0].UpdateParts.Add(newPart);

            target.CreateUpdateCommandsAsync(groups[0]).Wait(AsyncTimeout);

            Assert.AreEqual(2, this.commands.Count);
            var commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A");
            Assert.IsNotNull(commandA);

            var commandB = this.commands.FirstOrDefault(c => c.Unit.Name == "B");
            Assert.IsNotNull(commandB);
        }

        /// <summary>
        /// Test that adds two distinct update parts and makes sure it creates commands for the right units.
        /// </summary>
        [TestMethod]
        public void TestAddTwoUpdateParts()
        {
            var now = new DateTime(2012, 12, 12, 15, 23, 17, DateTimeKind.Utc);
            var timeProvider = new Mock<TimeProvider>();
            timeProvider.SetupGet(t => t.UtcNow).Returns(now);
            TimeProvider.Current = timeProvider.Object;

            var units = new List<Unit>
                            {
                                new Unit { Name = "A", Id = 1 },
                                new Unit { Name = "B", Id = 2 },
                                new Unit { Name = "C", Id = 3 }
                            };
            var groups = new List<UpdateGroup>
                             {
                                 new UpdateGroup
                                     {
                                         Name = "Test",
                                         Units = new List<Unit>(),
                                         UpdateParts = new List<UpdatePart>()
                                     }
                             };
            foreach (var unit in units.Take(2))
            {
                groups[0].Units.Add(unit);
                unit.UpdateGroup = groups[0];
            }

            var target = new UpdateCommandManager();

            var newPart = new UpdatePart
                              {
                                  Start = now.AddDays(-1),
                                  End = now.AddDays(1),
                                  UpdateGroup = groups[0],
                                  Type = UpdatePartType.Setup,
                                  Structure =
                                          new XmlData(CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe"))
                              };
            groups[0].UpdateParts.Add(newPart);

            newPart = new UpdatePart
                          {
                              Start = now.AddDays(-1),
                              End = now.AddDays(1),
                              UpdateGroup = groups[0],
                              Type = UpdatePartType.Presentation,
                              Structure = new XmlData(CreateFolderStructure(@"Progs\Protran\Protran.exe"))
                          };
            groups[0].UpdateParts.Add(newPart);

            target.CreateUpdateCommandsAsync(groups[0]).Wait(AsyncTimeout);

            var expectedStructure = CreateFolderStructure(
                @"Progs\SystemManager\SystemManager.exe",
                @"Progs\Protran\Protran.exe");
            Assert.AreEqual(2, this.commands.Count);
            var commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A");
            Assert.IsNotNull(commandA);
            Assert.AreEqual(expectedStructure, DeserializeFolderStructureFromCommand(commandA.Command));

            var commandB = this.commands.FirstOrDefault(c => c.Unit.Name == "B");
            Assert.IsNotNull(commandB);
            Assert.AreEqual(expectedStructure, DeserializeFolderStructureFromCommand(commandB.Command));
        }

        /// <summary>
        /// Test that adds two distinct update parts and makes sure it creates commands for the right units.
        /// In a second try, the update commands shouldn't be resent to the units.
        /// </summary>
        [TestMethod]
        public void TestAddTwoUpdatePartsNoResend()
        {
            var now = new DateTime(2012, 12, 12, 15, 23, 17, DateTimeKind.Utc);
            var timeProvider = new Mock<TimeProvider>();
            timeProvider.SetupGet(t => t.UtcNow).Returns(now);
            TimeProvider.Current = timeProvider.Object;

            var units = new List<Unit>
                            {
                                new Unit { Name = "A", Id = 1 },
                                new Unit { Name = "B", Id = 2 },
                                new Unit { Name = "C", Id = 3 }
                            };
            var groups = new List<UpdateGroup>
                             {
                                 new UpdateGroup
                                     {
                                         Name = "Test",
                                         Units = new List<Unit>(),
                                         UpdateParts = new List<UpdatePart>()
                                     }
                             };
            foreach (var unit in units.Take(2))
            {
                groups[0].Units.Add(unit);
                unit.UpdateGroup = groups[0];
            }

            var target = new UpdateCommandManager();

            var newPart = new UpdatePart
            {
                Start = now.AddDays(-1),
                End = now.AddDays(1),
                UpdateGroup = groups[0],
                Type = UpdatePartType.Setup,
                Structure =
                        new XmlData(CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe"))
            };
            groups[0].UpdateParts.Add(newPart);

            newPart = new UpdatePart
            {
                Start = now.AddDays(-1),
                End = now.AddDays(1),
                UpdateGroup = groups[0],
                Type = UpdatePartType.Presentation,
                Structure = new XmlData(CreateFolderStructure(@"Progs\Protran\Protran.exe"))
            };
            groups[0].UpdateParts.Add(newPart);

            target.CreateUpdateCommandsAsync(groups[0]).Wait(AsyncTimeout);

            var expectedStructure = CreateFolderStructure(
                @"Progs\SystemManager\SystemManager.exe",
                @"Progs\Protran\Protran.exe");
            Assert.AreEqual(2, this.commands.Count);
            var commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A");
            Assert.IsNotNull(commandA);
            Assert.AreEqual(expectedStructure, DeserializeFolderStructureFromCommand(commandA.Command));

            var commandB = this.commands.FirstOrDefault(c => c.Unit.Name == "B");
            Assert.IsNotNull(commandB);
            Assert.AreEqual(expectedStructure, DeserializeFolderStructureFromCommand(commandB.Command));

            // now, nothing should be resent
            target.CreateUpdateCommandsAsync(groups[0]).Wait(AsyncTimeout);
            Assert.AreEqual(2, this.commands.Count);
        }

        /// <summary>
        /// Test that adds two distinct update parts with different times and
        /// makes sure it creates commands for the right units.
        /// </summary>
        [TestMethod]
        public void TestAddTwoUpdatePartsParking()
        {
            var now = new DateTime(2012, 12, 12, 15, 23, 17, DateTimeKind.Utc);
            var timeProvider = new Mock<TimeProvider>();
            timeProvider.SetupGet(t => t.UtcNow).Returns(now);
            TimeProvider.Current = timeProvider.Object;

            var units = CreateUnitsForTestAddTwoUpdatePartsParking();
            var groups = CreateUpdateGroupsForTestAddTwoUpdatePartsParking();
            foreach (var unit in units.Take(2))
            {
                groups[0].Units.Add(unit);
                unit.UpdateGroup = groups[0];
            }

            var target = new UpdateCommandManager();

            var newPart = new UpdatePart
            {
                Start = now.AddDays(-1),
                End = now.AddDays(2),
                UpdateGroup = groups[0],
                Type = UpdatePartType.Setup,
                Structure =
                        new XmlData(CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe"))
            };
            groups[0].UpdateParts.Add(newPart);

            newPart = new UpdatePart
            {
                Start = now.AddDays(-2),
                End = now.AddDays(1),
                UpdateGroup = groups[0],
                Type = UpdatePartType.Presentation,
                Structure = new XmlData(CreateFolderStructure(@"Progs\Protran\Protran.exe"))
            };
            groups[0].UpdateParts.Add(newPart);

            target.CreateUpdateCommandsAsync(groups[0]).Wait(AsyncTimeout);

            // expected outcome:
            // t - 2: only Protran (ignored since in the past)
            // t - 1: Protran + System Manager
            // t + 1: only System Manager
            // t + 2: nothing left (ignored)
            var expectedStructure = CreateFolderStructure(
                @"Progs\SystemManager\SystemManager.exe",
                @"Progs\Protran\Protran.exe");
            Assert.AreEqual(4, this.commands.Count);
            var commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 1);
            Assert.IsNotNull(commandA);
            var cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(-1), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            var commandB = this.commands.FirstOrDefault(c => c.Unit.Name == "B" && c.UpdateIndex == 1);
            Assert.IsNotNull(commandB);
            var cmdMsgB = (UpdateCommandMsg)commandB.Command.Deserialize();
            Assert.AreEqual(now.AddDays(-1), cmdMsgB.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgB.Folders });

            expectedStructure = CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 2);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(1), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            commandB = this.commands.FirstOrDefault(c => c.Unit.Name == "B" && c.UpdateIndex == 2);
            Assert.IsNotNull(commandB);
            cmdMsgB = (UpdateCommandMsg)commandB.Command.Deserialize();
            Assert.AreEqual(now.AddDays(1), cmdMsgB.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgB.Folders });
        }

        /// <summary>
        /// Test that an update part of the same "Type" overwrites another older update part.
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
                Justification = "Reviewed. Suppression is OK here.")]
        public void TestOverwriteUpdateParts()
        {
            var now1 = new DateTime(2012, 12, 12, 15, 23, 17, DateTimeKind.Utc);
            var timeProvider = new Mock<TimeProvider>();
            // ReSharper disable AccessToModifiedClosure --> this is intentional
            timeProvider.SetupGet(t => t.UtcNow).Returns(() => now1);
            // ReSharper restore AccessToModifiedClosure
            TimeProvider.Current = timeProvider.Object;

            var units = new List<Unit> { new Unit { Name = "A", Id = 1 } };
            var groups = new List<UpdateGroup>
                             {
                                 new UpdateGroup
                                     {
                                         Name = "Test",
                                         Units = new List<Unit>(),
                                         UpdateParts = new List<UpdatePart>()
                                     }
                             };
            groups[0].Units.Add(units[0]);

            var target = new UpdateCommandManager();

            var newPart = new UpdatePart
                              {
                                  Id = 1,
                                  Start = now1.AddDays(-2),
                                  End = now1.AddDays(6),
                                  UpdateGroup = groups[0],
                                  Type = UpdatePartType.Setup,
                                  Structure =
                                      new XmlData(CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe"))
                              };
            groups[0].UpdateParts.Add(newPart);

            newPart = new UpdatePart
                          {
                              Id = 2,
                              Start = now1.AddDays(-1),
                              End = now1.AddDays(2),
                              UpdateGroup = groups[0],
                              Type = UpdatePartType.Presentation,
                              Structure = new XmlData(CreateFolderStructure(@"Progs\Protran\Protran.exe"))
                          };
            groups[0].UpdateParts.Add(newPart);

            newPart = new UpdatePart
                          {
                              Id = 3,
                              Start = now1.AddDays(1),
                              End = now1.AddDays(4),
                              UpdateGroup = groups[0],
                              Type = UpdatePartType.Setup,
                              Structure = new XmlData(CreateFolderStructure(@"Progs\Update\Update.exe"))
                          };
            groups[0].UpdateParts.Add(newPart);

            newPart = new UpdatePart
                          {
                              Id = 4,
                              Start = now1.AddDays(3),
                              End = now1.AddDays(5),
                              UpdateGroup = groups[0],
                              Type = UpdatePartType.Presentation,
                              Structure = new XmlData(CreateFolderStructure(@"Progs\Protran\Protran.exe"))
                          };
            groups[0].UpdateParts.Add(newPart);

            target.CreateUpdateCommandsAsync(groups[0]).Wait(AsyncTimeout);
            var now = now1;

            // expected outcome:
            // t - 2: only System Manager (ignored since in the past)
            // t - 1: Protran + System Manager
            // t + 1: Protran + Update (System Manager is overwritten by Update)
            // t + 2: only Update (System Manager is overwritten by Update)
            // t + 3: Protran[2] + Update (System Manager is overwritten by Update)
            // t + 4: Protran[2] + System Manager
            // t + 5: only System Manager
            // t + 6: nothing left (ignored)
            Assert.AreEqual(6, this.commands.Count);

            var expectedStructure = CreateFolderStructure(
                @"Progs\SystemManager\SystemManager.exe",
                @"Progs\Protran\Protran.exe");
            var commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 1);
            Assert.IsNotNull(commandA);
            var cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(-1), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            expectedStructure = CreateFolderStructure(@"Progs\Protran\Protran.exe", @"Progs\Update\Update.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 2);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(1), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            expectedStructure = CreateFolderStructure(@"Progs\Update\Update.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 3);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(2), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            expectedStructure = CreateFolderStructure(@"Progs\Protran\Protran.exe", @"Progs\Update\Update.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 4);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(3), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            expectedStructure = CreateFolderStructure(
                @"Progs\Protran\Protran.exe",
                @"Progs\SystemManager\SystemManager.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 5);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(4), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            expectedStructure = CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 6);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(5), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });
        }

        /// <summary>
        /// Test that removes an update part and makes sure it creates commands for the right units.
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
                Justification = "Reviewed. Suppression is OK here.")]
        public void TestChangeUpdateGroupOfUnit()
        {
            var now = new DateTime(2012, 12, 12, 15, 23, 17, DateTimeKind.Utc);
            var timeProvider = new Mock<TimeProvider>();
            timeProvider.SetupGet(t => t.UtcNow).Returns(now);
            TimeProvider.Current = timeProvider.Object;

            var units = new List<Unit>
                            {
                                new Unit { Name = "A", Id = 1 },
                                new Unit { Name = "B", Id = 2 },
                                new Unit { Name = "C", Id = 3 }
                            };
            var groups = new List<UpdateGroup>
                             {
                                 new UpdateGroup
                                     {
                                         Name = "Test 1",
                                         Units = new List<Unit>(),
                                         UpdateParts = new List<UpdatePart>()
                                     },
                                 new UpdateGroup
                                     {
                                         Name = "Test 2",
                                         Units = new List<Unit>(),
                                         UpdateParts = new List<UpdatePart>()
                                     }
                             };
            foreach (var unit in units.Take(2))
            {
                groups[0].Units.Add(unit);
                unit.UpdateGroup = groups[0];
            }

            foreach (var unit in units.Skip(2))
            {
                groups[1].Units.Add(unit);
                unit.UpdateGroup = groups[1];
            }

            var target = new UpdateCommandManager();

            var newPart = new UpdatePart
                              {
                                  Start = now.AddDays(-1),
                                  End = now.AddDays(1),
                                  UpdateGroup = groups[0],
                                  Type = UpdatePartType.Setup,
                                  Structure =
                                          new XmlData(CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe"))
                              };
            groups[0].UpdateParts.Add(newPart);

            newPart = new UpdatePart
                          {
                              Start = now.AddDays(-2),
                              End = now.AddDays(2),
                              UpdateGroup = groups[1],
                              Type = UpdatePartType.Presentation,
                              Structure = new XmlData(CreateFolderStructure(@"Progs\Protran\Protran.exe"))
                          };
            groups[1].UpdateParts.Add(newPart);

            target.CreateUpdateCommandsAsync(groups[0]).Wait(AsyncTimeout);

            var expectedStructure = CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe");
            Assert.AreEqual(2, this.commands.Count);
            var commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 1);
            Assert.IsNotNull(commandA);
            Assert.AreEqual(expectedStructure, DeserializeFolderStructureFromCommand(commandA.Command));

            var commandB = this.commands.FirstOrDefault(c => c.Unit.Name == "B" && c.UpdateIndex == 1);
            Assert.IsNotNull(commandB);
            Assert.AreEqual(expectedStructure, DeserializeFolderStructureFromCommand(commandB.Command));

            target.CreateUpdateCommandsAsync(groups[1]).Wait(AsyncTimeout);

            expectedStructure = CreateFolderStructure(@"Progs\Protran\Protran.exe");
            Assert.AreEqual(3, this.commands.Count);
            var commandC = this.commands.FirstOrDefault(c => c.Unit.Name == "C" && c.UpdateIndex == 1);
            Assert.IsNotNull(commandC);
            Assert.AreEqual(expectedStructure, DeserializeFolderStructureFromCommand(commandC.Command));

            groups[0].Units.Remove(units[1]);
            groups[1].Units.Add(units[1]);
            units[1].UpdateGroup = groups[1];

            // this is the only unit to update
            target.CreateUpdateCommandsAsync(units[1]).Wait(AsyncTimeout);

            expectedStructure = CreateFolderStructure(@"Progs\Protran\Protran.exe");
            Assert.AreEqual(4, this.commands.Count);
            commandB = this.commands.FirstOrDefault(c => c.Unit.Name == "B" && c.UpdateIndex == 2);
            Assert.IsNotNull(commandB);
            Assert.AreEqual(expectedStructure, DeserializeFolderStructureFromCommand(commandB.Command));
        }

        /// <summary>
        /// Test that removes an update part and makes sure it creates commands for the right units.
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void TestRemoveUpdatePart()
        {
            var now = new DateTime(2012, 12, 12, 15, 23, 17, DateTimeKind.Utc);
            var timeProvider = new Mock<TimeProvider>();
            timeProvider.SetupGet(t => t.UtcNow).Returns(now);
            TimeProvider.Current = timeProvider.Object;

            var units = new List<Unit>
                            {
                                new Unit { Name = "A", Id = 1 },
                                new Unit { Name = "B", Id = 2 },
                                new Unit { Name = "C", Id = 3 }
                            };
            var groups = new List<UpdateGroup>
                             {
                                 new UpdateGroup
                                     {
                                         Name = "Test",
                                         Units = new List<Unit>(),
                                         UpdateParts = new List<UpdatePart>()
                                     }
                             };
            foreach (var unit in units.Take(2))
            {
                groups[0].Units.Add(unit);
                unit.UpdateGroup = groups[0];
            }

            var target = new UpdateCommandManager();

            var newPart = new UpdatePart
                              {
                                  Start = now.AddDays(-1),
                                  End = now.AddDays(1),
                                  UpdateGroup = groups[0],
                                  Type = UpdatePartType.Setup,
                                  Structure =
                                          new XmlData(CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe"))
                              };
            groups[0].UpdateParts.Add(newPart);

            newPart = new UpdatePart
                          {
                              Start = now.AddDays(-1),
                              End = now.AddDays(1),
                              UpdateGroup = groups[0],
                              Type = UpdatePartType.Presentation,
                              Structure = new XmlData(CreateFolderStructure(@"Progs\Protran\Protran.exe"))
                          };
            groups[0].UpdateParts.Add(newPart);

            target.CreateUpdateCommandsAsync(groups[0]).Wait(AsyncTimeout);

            var expectedStructure = CreateFolderStructure(
                @"Progs\SystemManager\SystemManager.exe",
                @"Progs\Protran\Protran.exe");
            Assert.AreEqual(2, this.commands.Count);
            var commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 1);
            Assert.IsNotNull(commandA);
            Assert.AreEqual(expectedStructure, DeserializeFolderStructureFromCommand(commandA.Command));

            var commandB = this.commands.FirstOrDefault(c => c.Unit.Name == "B" && c.UpdateIndex == 1);
            Assert.IsNotNull(commandB);
            Assert.AreEqual(expectedStructure, DeserializeFolderStructureFromCommand(commandB.Command));

            groups[0].UpdateParts.Remove(groups[0].UpdateParts.First());

            target.CreateUpdateCommandsAsync(groups[0]).Wait(AsyncTimeout);

            expectedStructure = CreateFolderStructure(@"Progs\Protran\Protran.exe");
            Assert.AreEqual(4, this.commands.Count);
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 2);
            Assert.IsNotNull(commandA);
            Assert.AreEqual(expectedStructure, DeserializeFolderStructureFromCommand(commandA.Command));

            commandB = this.commands.FirstOrDefault(c => c.Unit.Name == "B" && c.UpdateIndex == 2);
            Assert.IsNotNull(commandB);
            Assert.AreEqual(expectedStructure, DeserializeFolderStructureFromCommand(commandB.Command));
        }

        /// <summary>
        /// Test that we can change parked updates without resending all commands (but with the necessary ones).
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void TestAddParkedUpdateChanges()
        {
            var now = new DateTime(2012, 12, 12, 15, 23, 17, DateTimeKind.Utc);
            var timeProvider = new Mock<TimeProvider>();
            // ReSharper disable AccessToModifiedClosure --> this is intentional
            timeProvider.SetupGet(t => t.UtcNow).Returns(() => now);
            // ReSharper restore AccessToModifiedClosure
            TimeProvider.Current = timeProvider.Object;

            var units = new List<Unit>
                            {
                                new Unit { Name = "A", Id = 1 },
                                new Unit { Name = "B", Id = 2 },
                                new Unit { Name = "C", Id = 3 }
                            };
            var groups = new List<UpdateGroup>
                             {
                                 new UpdateGroup
                                     {
                                         Name = "Test",
                                         Units = new List<Unit>(),
                                         UpdateParts = new List<UpdatePart>()
                                     }
                             };
            foreach (var unit in units.Take(2))
            {
                groups[0].Units.Add(unit);
                unit.UpdateGroup = groups[0];
            }

            var target = new UpdateCommandManager();

            var newPart = new UpdatePart
            {
                Start = now.AddDays(-1),
                End = now.AddDays(3),
                UpdateGroup = groups[0],
                Type = UpdatePartType.Setup,
                Structure =
                        new XmlData(CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe"))
            };
            groups[0].UpdateParts.Add(newPart);

            newPart = new UpdatePart
            {
                Start = now.AddDays(-2),
                End = now.AddDays(2),
                UpdateGroup = groups[0],
                Type = UpdatePartType.Presentation,
                Structure = new XmlData(CreateFolderStructure(@"Progs\Protran\Protran.exe"))
            };
            groups[0].UpdateParts.Add(newPart);

            target.CreateUpdateCommandsAsync(groups[0]).Wait(AsyncTimeout);

            // expected outcome:
            // t - 2: only Protran (ignored since in the past)
            // t - 1: Protran + System Manager
            // t + 2: only System Manager
            // t + 3: nothing left (ignored)
            var expectedStructure = CreateFolderStructure(
                @"Progs\SystemManager\SystemManager.exe",
                @"Progs\Protran\Protran.exe");
            Assert.AreEqual(4, this.commands.Count);
            var commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 1);
            Assert.IsNotNull(commandA);
            var cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(-1), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            var commandB = this.commands.FirstOrDefault(c => c.Unit.Name == "B" && c.UpdateIndex == 1);
            Assert.IsNotNull(commandB);
            var cmdMsgB = (UpdateCommandMsg)commandB.Command.Deserialize();
            Assert.AreEqual(now.AddDays(-1), cmdMsgB.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgB.Folders });

            expectedStructure = CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 2);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(2), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            commandB = this.commands.FirstOrDefault(c => c.Unit.Name == "B" && c.UpdateIndex == 2);
            Assert.IsNotNull(commandB);
            cmdMsgB = (UpdateCommandMsg)commandB.Command.Deserialize();
            Assert.AreEqual(now.AddDays(2), cmdMsgB.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgB.Folders });

            // now, let's change the second part
            var part = groups[0].UpdateParts.Last();
            part.End = now.AddDays(1.5);

            now = now.AddDays(0.5);
            target.CreateUpdateCommandsAsync(groups[0]).Wait(AsyncTimeout);

            // expected outcome:
            // t - 2.5: only Protran (ignored since in the past)
            // t - 1.5: Protran + System Manager (ignored, same as last time [index=1])
            // t + 1: only System Manager (modified with new activation time)
            // t + 2.5: nothing left (ignored)
            Assert.AreEqual(6, this.commands.Count);

            expectedStructure = CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 3);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(1), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            commandB = this.commands.FirstOrDefault(c => c.Unit.Name == "B" && c.UpdateIndex == 3);
            Assert.IsNotNull(commandB);
            cmdMsgB = (UpdateCommandMsg)commandB.Command.Deserialize();
            Assert.AreEqual(now.AddDays(1), cmdMsgB.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgB.Folders });
        }

        /// <summary>
        /// Test that we can change parked updates without resending all commands (but with the necessary ones).
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void TestRemoveParkedUpdates()
        {
            var now = new DateTime(2012, 12, 12, 15, 23, 17, DateTimeKind.Utc);
            var timeProvider = new Mock<TimeProvider>();
            // ReSharper disable AccessToModifiedClosure --> this is intentional
            timeProvider.SetupGet(t => t.UtcNow).Returns(() => now);
            // ReSharper restore AccessToModifiedClosure
            TimeProvider.Current = timeProvider.Object;

            var units = new List<Unit>
                            {
                                new Unit { Name = "A", Id = 1 }
                            };
            var groups = new List<UpdateGroup>
                             {
                                 new UpdateGroup
                                     {
                                         Name = "Test",
                                         Units = new List<Unit>(),
                                         UpdateParts = new List<UpdatePart>()
                                     }
                             };
            groups[0].Units.Add(units[0]);

            var target = new UpdateCommandManager();

            var newPart = new UpdatePart
            {
                Id = 1,
                Start = now.AddDays(-2),
                End = now.AddDays(4),
                UpdateGroup = groups[0],
                Type = UpdatePartType.Setup,
                Structure =
                        new XmlData(CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe"))
            };
            groups[0].UpdateParts.Add(newPart);

            newPart = new UpdatePart
            {
                Id = 2,
                Start = now.AddDays(-1),
                End = now.AddDays(2),
                UpdateGroup = groups[0],
                Type = UpdatePartType.Presentation,
                Structure = new XmlData(CreateFolderStructure(@"Progs\Protran\Protran.exe"))
            };
            groups[0].UpdateParts.Add(newPart);

            newPart = new UpdatePart
            {
                Id = 3,
                Start = now.AddDays(1),
                End = now.AddDays(6),
                UpdateGroup = groups[0],
                Type = (UpdatePartType)3, // little trick to allow more types
                Structure = new XmlData(CreateFolderStructure(@"Progs\Update\Update.exe"))
            };
            groups[0].UpdateParts.Add(newPart);

            newPart = new UpdatePart
            {
                Id = 4,
                Start = now.AddDays(3),
                End = now.AddDays(5),
                UpdateGroup = groups[0],
                Type = UpdatePartType.Presentation,
                Structure = new XmlData(CreateFolderStructure(@"Progs\Protran\Protran.exe"))
            };
            groups[0].UpdateParts.Add(newPart);

            target.CreateUpdateCommandsAsync(groups[0]).Wait(AsyncTimeout);

            // expected outcome:
            // t - 2: only System Manager (ignored since in the past)
            // t - 1: Protran + System Manager
            // t + 1: Protran + System Manager + Update
            // t + 2: System Manager + Update
            // t + 3: Protran[2] + System Manager + Update
            // t + 4: Protran[2] + Update
            // t + 5: only Update
            // t + 6: nothing left (ignored)
            Assert.AreEqual(6, this.commands.Count);

            var expectedStructure = CreateFolderStructure(
                @"Progs\SystemManager\SystemManager.exe",
                @"Progs\Protran\Protran.exe");
            var commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 1);
            Assert.IsNotNull(commandA);
            var cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(-1), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            expectedStructure = CreateFolderStructure(
                @"Progs\SystemManager\SystemManager.exe",
                @"Progs\Protran\Protran.exe",
                @"Progs\Update\Update.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 2);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(1), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            expectedStructure = CreateFolderStructure(
                @"Progs\SystemManager\SystemManager.exe",
                @"Progs\Update\Update.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 3);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(2), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            expectedStructure = CreateFolderStructure(
                @"Progs\SystemManager\SystemManager.exe",
                @"Progs\Protran\Protran.exe",
                @"Progs\Update\Update.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 4);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(3), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            expectedStructure = CreateFolderStructure(
                @"Progs\Protran\Protran.exe",
                @"Progs\Update\Update.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 5);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(4), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            expectedStructure = CreateFolderStructure(
                @"Progs\Update\Update.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 6);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(5), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            // now, let's remove the last part (Protran[2])
            var part = groups[0].UpdateParts.Last();
            groups[0].UpdateParts.Remove(part);
            part.UpdateGroup = null;

            now = now.AddDays(0.5);
            target.CreateUpdateCommandsAsync(groups[0]).Wait(AsyncTimeout);

            // expected outcome:
            // t - 2.5: only System Manager (ignored since in the past)
            // t - 1.5: Protran + System Manager (ignored, same as last time [index=1])
            // t + 0.5: Protran + System Manager + Update (ignored, same as last time [index=2])
            // t + 1.5: System Manager + Update (ignored, same as last time [index=3])
            // t + 2.5: System Manager + Update
            //          (now without Protran[2], but since this is unknown to the manager, we will send t + 1.5 again)
            // t + 3.5: only Update (now without Protran[2])
            // t + 4.5: only Update (not needed, same as previous)
            // t + 5.5: nothing left (ignored)
            Assert.AreEqual(8, this.commands.Count);

            expectedStructure = CreateFolderStructure(
                @"Progs\SystemManager\SystemManager.exe",
                @"Progs\Update\Update.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 7);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(1.5), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            expectedStructure = CreateFolderStructure(
                @"Progs\Update\Update.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 8);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(3.5), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });
        }

        /// <summary>
        /// Test that we can change parked updates without resending all commands (but with the necessary ones).
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void TestAddParkedUpdateRemoveLast()
        {
            var now = new DateTime(2012, 12, 12, 15, 23, 17, DateTimeKind.Utc);
            var timeProvider = new Mock<TimeProvider>();
            // ReSharper disable AccessToModifiedClosure --> this is intentional
            timeProvider.SetupGet(t => t.UtcNow).Returns(() => now);
            // ReSharper restore AccessToModifiedClosure
            TimeProvider.Current = timeProvider.Object;

            var units = new List<Unit>
                            {
                                new Unit { Name = "A", Id = 1 },
                                new Unit { Name = "B", Id = 2 },
                                new Unit { Name = "C", Id = 3 }
                            };
            var groups = new List<UpdateGroup>
                             {
                                 new UpdateGroup
                                     {
                                         Name = "Test",
                                         Units = new List<Unit>(),
                                         UpdateParts = new List<UpdatePart>()
                                     }
                             };
            foreach (var unit in units.Take(2))
            {
                groups[0].Units.Add(unit);
                unit.UpdateGroup = groups[0];
            }

            var target = new UpdateCommandManager();

            var newPart = new UpdatePart
            {
                Start = now.AddDays(-1),
                End = now.AddDays(2),
                UpdateGroup = groups[0],
                Type = UpdatePartType.Setup,
                Structure =
                        new XmlData(CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe"))
            };
            groups[0].UpdateParts.Add(newPart);

            newPart = new UpdatePart
            {
                Start = now.AddDays(-2),
                End = now.AddDays(1),
                UpdateGroup = groups[0],
                Type = UpdatePartType.Presentation,
                Structure = new XmlData(CreateFolderStructure(@"Progs\Protran\Protran.exe"))
            };
            groups[0].UpdateParts.Add(newPart);

            target.CreateUpdateCommandsAsync(groups[0]).Wait(AsyncTimeout);

            // expected outcome:
            // t - 2: only Protran (ignored since in the past)
            // t - 1: Protran + System Manager
            // t + 1: only System Manager
            // t + 2: nothing left (ignored)
            var expectedStructure = CreateFolderStructure(
                @"Progs\SystemManager\SystemManager.exe",
                @"Progs\Protran\Protran.exe");
            Assert.AreEqual(4, this.commands.Count);
            var commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 1);
            Assert.IsNotNull(commandA);
            var cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(-1), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            var commandB = this.commands.FirstOrDefault(c => c.Unit.Name == "B" && c.UpdateIndex == 1);
            Assert.IsNotNull(commandB);
            var cmdMsgB = (UpdateCommandMsg)commandB.Command.Deserialize();
            Assert.AreEqual(now.AddDays(-1), cmdMsgB.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgB.Folders });

            expectedStructure = CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 2);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(1), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            commandB = this.commands.FirstOrDefault(c => c.Unit.Name == "B" && c.UpdateIndex == 2);
            Assert.IsNotNull(commandB);
            cmdMsgB = (UpdateCommandMsg)commandB.Command.Deserialize();
            Assert.AreEqual(now.AddDays(1), cmdMsgB.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgB.Folders });

            // now, let's change the second part
            var part = groups[0].UpdateParts.Last();
            part.End = now.AddDays(2);

            now = now.AddDays(0.5);
            target.CreateUpdateCommandsAsync(groups[0]).Wait(AsyncTimeout);

            // expected outcome:
            // t - 2.5: only Protran (ignored since in the past)
            // t - 1.5: Protran + System Manager (ignored, same as last time [index=1])
            // t + 1: NOT only System Manager
            //        (no longer there, must be deleted with a higher update index but contents of t - 1.5)
            // t + 1.5: nothing left (ignored)
            Assert.AreEqual(6, this.commands.Count);

            expectedStructure = CreateFolderStructure(
                @"Progs\SystemManager\SystemManager.exe",
                @"Progs\Protran\Protran.exe");
            commandA = this.commands.FirstOrDefault(c => c.Unit.Name == "A" && c.UpdateIndex == 3);
            Assert.IsNotNull(commandA);
            cmdMsgA = (UpdateCommandMsg)commandA.Command.Deserialize();
            Assert.AreEqual(now.AddDays(-1.5), cmdMsgA.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgA.Folders });

            commandB = this.commands.FirstOrDefault(c => c.Unit.Name == "B" && c.UpdateIndex == 3);
            Assert.IsNotNull(commandB);
            cmdMsgB = (UpdateCommandMsg)commandB.Command.Deserialize();
            Assert.AreEqual(now.AddDays(-1.5), cmdMsgB.ActivateTime);
            Assert.AreEqual(expectedStructure, new UpdateFolderStructure { Folders = cmdMsgB.Folders });
        }

        /// <summary>
        /// Test that update commands get assigned the right feedback.
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void TestAddFeedback()
        {
            var now = new DateTime(2012, 12, 12, 15, 23, 17, DateTimeKind.Utc);
            var timeProvider = new Mock<TimeProvider>();
            // ReSharper disable AccessToModifiedClosure --> this is intentional
            timeProvider.SetupGet(t => t.UtcNow).Returns(() => now);
            // ReSharper restore AccessToModifiedClosure
            TimeProvider.Current = timeProvider.Object;

            var units = new List<Unit>
                            {
                                new Unit { Name = "A", Id = 1 },
                                new Unit { Name = "B", Id = 2 },
                                new Unit { Name = "C", Id = 3 }
                            };
            var groups = new List<UpdateGroup>
                             {
                                 new UpdateGroup
                                     {
                                         Name = "Test",
                                         Units = new List<Unit>(),
                                         UpdateParts = new List<UpdatePart>()
                                     }
                             };
            foreach (var unit in units.Take(2))
            {
                groups[0].Units.Add(unit);
            }

            var target = new UpdateCommandManager();

            var newPart = new UpdatePart
            {
                Id = 1,
                Start = now.AddDays(-2),
                End = now.AddDays(6),
                UpdateGroup = groups[0],
                Type = UpdatePartType.Setup,
                Structure =
                        new XmlData(CreateFolderStructure(@"Progs\SystemManager\SystemManager.exe"))
            };

            groups[0].UpdateParts.Add(newPart);

            newPart = new UpdatePart
            {
                Id = 2,
                Start = now.AddDays(-1),
                End = now.AddDays(2),
                UpdateGroup = groups[0],
                Type = UpdatePartType.Presentation,
                Structure = new XmlData(CreateFolderStructure(@"Progs\Protran\Protran.exe"))
            };
            groups[0].UpdateParts.Add(newPart);

            newPart = new UpdatePart
            {
                Id = 3,
                Start = now.AddDays(1),
                End = now.AddDays(4),
                UpdateGroup = groups[0],
                Type = UpdatePartType.Setup,
                Structure = new XmlData(CreateFolderStructure(@"Progs\Update\Update.exe"))
            };
            groups[0].UpdateParts.Add(newPart);

            newPart = new UpdatePart
            {
                Id = 4,
                Start = now.AddDays(3),
                End = now.AddDays(5),
                UpdateGroup = groups[0],
                Type = UpdatePartType.Presentation,
                Structure = new XmlData(CreateFolderStructure(@"Progs\Protran\Protran.exe"))
            };
            groups[0].UpdateParts.Add(newPart);

            var task = target.CreateUpdateCommandsAsync(groups[0]);
            task.Wait(AsyncTimeout);
            var commandMsgs = task.Result.ToList();

            // expected outcome:
            // t - 2: only System Manager (ignored since in the past)
            // t - 1: Protran + System Manager
            // t + 1: Protran + Update (System Manager is overwritten by Update)
            // t + 2: only Update (System Manager is overwritten by Update)
            // t + 3: Protran[2] + Update (System Manager is overwritten by Update)
            // t + 4: Protran[2] + System Manager
            // t + 5: only System Manager
            // t + 6: nothing left (ignored)
            Assert.AreEqual(12, this.commands.Count);
            Assert.AreEqual(12, commandMsgs.Count);

            var transferredTime = now = now.AddMinutes(5);
            var states =
                commandMsgs.Select(
                    command =>
                    new UpdateStateInfo
                        {
                            State = UpdateStateMsg.Transferred,
                            TimeStamp = now,
                            UnitId = command.UnitId,
                            UpdateId = command.UpdateId
                        }).ToList();

            target.AddFeedbacksAsync(states.ToArray()).Wait(AsyncTimeout);
            Assert.AreEqual(12, this.feedbacks.Count);

            var installingTime = now = now.AddSeconds(20);
            states.Clear();
            states =
                commandMsgs.Where(c => c.UpdateId.UpdateIndex < 2)
                    .Select(
                        command =>
                        new UpdateStateInfo
                            {
                                State = UpdateStateMsg.Installing,
                                TimeStamp = now,
                                UnitId = command.UnitId,
                                UpdateId = command.UpdateId
                            }).ToList();

            target.AddFeedbacksAsync(states.ToArray()).Wait(AsyncTimeout);
            Assert.AreEqual(14, this.feedbacks.Count);

            var installedTime = now = now.AddMinutes(1);
            states.Clear();
            states =
                commandMsgs.Where(c => c.UpdateId.UpdateIndex < 2)
                    .Select(
                        command =>
                        new UpdateStateInfo
                            {
                                Folders = command.Folders.ConvertAll(ToFeedback),
                                State = UpdateStateMsg.Installed,
                                TimeStamp = now,
                                UnitId = command.UnitId,
                                UpdateId = command.UpdateId
                            }).ToList();

            target.AddFeedbacksAsync(states.ToArray()).Wait(AsyncTimeout);
            Assert.AreEqual(16, this.feedbacks.Count);

            var installed = this.commands.Where(c => c.WasInstalled).ToList();
            Assert.AreEqual(2, installed.Count);

            var commandA = installed.FirstOrDefault(c => c.Unit.Name == "A");
            Assert.IsNotNull(commandA);
            Assert.AreEqual(1, commandA.UpdateIndex);
            Assert.AreEqual(3, commandA.Feedbacks.Count);

            var commandB = installed.FirstOrDefault(c => c.Unit.Name == "B");
            Assert.IsNotNull(commandB);
            Assert.AreEqual(1, commandB.UpdateIndex);
            Assert.AreEqual(3, commandB.Feedbacks.Count);

            Assert.IsTrue(
                this.feedbacks.Where(feedback => feedback.State == UpdateStateDto.Transferred)
                    .All(f => f.Timestamp == transferredTime));
            Assert.IsTrue(
                this.feedbacks.Where(feedback => feedback.State == UpdateStateDto.Installing)
                    .All(f => f.Timestamp == installingTime));
            Assert.IsTrue(
                this.feedbacks.Where(feedback => feedback.State == UpdateStateDto.Installed)
                    .All(f => f.Timestamp == installedTime));
        }

        private static List<UpdateGroup> CreateUpdateGroupsForTestAddTwoUpdatePartsParking()
        {
            var groups = new List<UpdateGroup>
                             {
                                 new UpdateGroup
                                     {
                                         Name = "Test",
                                         Units = new List<Unit>(),
                                         UpdateParts = new List<UpdatePart>()
                                     }
                             };
            return groups;
        }

        private static List<Unit> CreateUnitsForTestAddTwoUpdatePartsParking()
        {
            var units = new List<Unit>
                            {
                                new Unit { Name = "A", Id = 1 },
                                new Unit { Name = "B", Id = 2 },
                                new Unit { Name = "C", Id = 3 }
                            };
            return units;
        }

        private static UpdateFolderStructure DeserializeFolderStructureFromCommand(XmlData updateCommand)
        {
            return new UpdateFolderStructure { Folders = ((UpdateCommandMsg)updateCommand.Deserialize()).Folders };
        }

        private static UpdateFolderStructure CreateFolderStructure(params string[] files)
        {
            var structure = new UpdateFolderStructure();
            foreach (var file in files)
            {
                var path = file.Replace('/', '\\').Trim('\\').Split('\\');
                if (path.Length < 2)
                {
                    throw new ArgumentException("Can't have files in the root: " + file);
                }

                string name;
                var folder = GetFolder(structure.Folders, path[0], structure.Folders.Add);
                for (int index = 1; index < path.Length - 1; index++)
                {
                    name = path[index];
                    folder = GetFolder(folder.Items.OfType<FolderUpdate>(), name, folder.Items.Add);
                }

                name = path.Last();
                var data = Encoding.UTF8.GetBytes(name);
                folder.Items.Add(new FileUpdate { Name = name, Hash = ResourceHash.Create(data, 0, data.Length) });
            }

            return structure;
        }

        private static FolderUpdate GetFolder(
            IEnumerable<FolderUpdate> folders, string name, Action<FolderUpdate> addMethod)
        {
            var folder = folders.FirstOrDefault(f => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (folder == null)
            {
                folder = new FolderUpdate { Name = name };
                addMethod(folder);
            }

            return folder;
        }

        private static FolderUpdateInfo ToFeedback(FolderUpdate update)
        {
            var feedback = new FolderUpdateInfo { Name = update.Name, State = ItemUpdateState.UpToDate };
            foreach (var item in update.Items)
            {
                var folder = item as FolderUpdate;
                if (folder != null)
                {
                    feedback.Items.Add(ToFeedback(folder));
                    continue;
                }

                var file = item as FileUpdate;
                if (file != null)
                {
                    feedback.Items.Add(ToFeedback(file));
                }
            }

            return feedback;
        }

        private static FileUpdateInfo ToFeedback(FileUpdate update)
        {
            return new FileUpdateInfo
            {
                Name = update.Name,
                ExpectedHash = update.Hash,
                Hash = update.Hash,
                State = ItemUpdateState.UpToDate
            };
        }

        private Mock<ISystemConfigDataService> CreateSystemConfigDataService()
        {
            var systemConfigDataService = new Mock<ISystemConfigDataService>();
            var systemConfig = new SystemConfig
                                   {
                                       Id = 1,
                                       SystemId = Guid.NewGuid(),
                                       Settings =
                                           new XmlData(
                                           new BackgroundSystemSettings
                                               {
                                                   FtpUpdateProviders = new List<FtpUpdateProviderConfig>()
                                               })
                                   };
            systemConfigDataService.Setup(s => s.QueryAsync(It.IsAny<SystemConfigQuery>()))
                .Returns(Task.FromResult(Enumerable.Repeat(systemConfig, 1)));
            return systemConfigDataService;
        }

        private Mock<IUpdateCommandDataService> CreateUpdateCommandDataService()
        {
            int nextId = 0;
            var updateCommandDataService = new Mock<IUpdateCommandDataService>();
            updateCommandDataService.Setup(s => s.AddAsync(It.IsAny<UpdateCommandDto>()))
                .Returns<UpdateCommandDto>(
                    command =>
                        {
                            command = (UpdateCommandDto)command.Clone();
                            command.Id = nextId++;
                            this.commands.Add(command);
                            return Task.FromResult((UpdateCommandDto)command.Clone());
                        });
            updateCommandDataService.Setup(s => s.QueryAsync(It.IsAny<UpdateCommandQuery>()))
                .Returns<UpdateCommandQuery>(
                    filter =>
                        {
                            var query = commands.Select(c => c.ToDatabase()).AsQueryable();
                            if (filter != null)
                            {
                                query = query.Apply(filter);
                            }

                            return Task.FromResult((IEnumerable<UpdateCommandDto>)query.Select(c => c.ToDto(filter)));
                        });
            updateCommandDataService.Setup(s => s.UpdateAsync(It.IsAny<UpdateCommandDto>()))
                .Returns<UpdateCommandDto>(
                    command =>
                        {
                            var index = this.commands.FindIndex(c => c.Id == command.Id);
                            command = (UpdateCommandDto)command.Clone();
                            if (command.Feedbacks == null)
                            {
                                command.Feedbacks = this.commands[index].Feedbacks;
                            }

                            this.commands[index] = command;
                            return Task.FromResult((UpdateCommandDto)command.Clone());
                        });
            return updateCommandDataService;
        }

        private Mock<IUpdateFeedbackDataService> CreateUpdateFeedbackDataService()
        {
            int nextId = 0;
            var updateCommand = new Action<UpdateFeedback>(
                feedback =>
                    {
                        if (feedback.UpdateCommand == null)
                        {
                            return;
                        }

                        var command = this.commands.FirstOrDefault(c => c.Id == feedback.UpdateCommand.Id);
                        if (command != null && command.Feedbacks != null
                            && command.Feedbacks.All(f => f.Id != feedback.Id))
                        {
                            command.Feedbacks.Add(feedback);
                        }
                    });
            var updateFeedbackDataService = new Mock<IUpdateFeedbackDataService>();
            updateFeedbackDataService.Setup(s => s.AddAsync(It.IsAny<UpdateFeedback>()))
                .Returns<UpdateFeedback>(
                    feedback =>
                        {
                            feedback = (UpdateFeedback)feedback.Clone();
                            feedback.Id = nextId++;
                            feedbacks.Add(feedback);
                            updateCommand(feedback);
                            return Task.FromResult((UpdateFeedback)feedback.Clone());
                        });
            updateFeedbackDataService.Setup(s => s.QueryAsync(It.IsAny<UpdateFeedbackQuery>()))
                .Returns<UpdateFeedbackQuery>(
                    filter =>
                        {
                            var query = feedbacks.Select(f => f.ToDatabase()).AsQueryable();
                            if (filter != null)
                            {
                                query = query.Apply(filter);
                            }

                            return Task.FromResult((IEnumerable<UpdateFeedback>)query.Select(f => f.ToDto(filter)));
                        });
            updateFeedbackDataService.Setup(s => s.UpdateAsync(It.IsAny<UpdateFeedback>()))
                .Returns<UpdateFeedback>(
                    feedback =>
                        {
                            var index = feedbacks.FindIndex(f => f.Id == feedback.Id);
                            feedbacks[index] = (UpdateFeedback)feedback.Clone();
                            updateCommand(feedbacks[index]);
                            return Task.FromResult((UpdateFeedback)feedback.Clone());
                        });
            return updateFeedbackDataService;
        }
    }
}
