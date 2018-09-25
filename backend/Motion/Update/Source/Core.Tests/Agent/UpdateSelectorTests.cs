// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSelectorTests.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateSelectorTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Tests.Agent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Update.Core.Agent;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="UpdateSelector"/>.
    /// </summary>
    [TestClass]
    public class UpdateSelectorTests
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using a single command in sequence.
        /// </summary>
        [TestMethod]
        public void TestSearch_Simple()
        {
            var guid = Guid.NewGuid().ToString();
            var command = new UpdateCommand { UpdateId = new UpdateId(guid, 5), UnitId = new UnitId("A") };

            var selector = new UpdateSelector(new[] { command });
            selector.Search(new UpdateId(guid, 4), new List<UpdateCommand>(), true);

            Assert.AreEqual(1, selector.CommandsToInstall.Count);
            Assert.AreSame(command, selector.CommandsToInstall[0]);

            Assert.AreEqual(0, selector.ValidatedParkedUpdateCommands.Count);

            Assert.AreEqual(1, selector.Feedback.Count);
            var feedback = selector.Feedback[0];
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(command.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using a single command with a higher ID than expected
        /// (still should install normally).
        /// </summary>
        [TestMethod]
        public void TestSearch_SkippedIndex()
        {
            var guid = Guid.NewGuid().ToString();
            var command = new UpdateCommand { UpdateId = new UpdateId(guid, 20), UnitId = new UnitId("A") };

            var selector = new UpdateSelector(new[] { command });
            selector.Search(new UpdateId(guid, 4), new List<UpdateCommand>(), true);

            Assert.AreEqual(1, selector.CommandsToInstall.Count);
            Assert.AreSame(command, selector.CommandsToInstall[0]);

            Assert.AreEqual(0, selector.ValidatedParkedUpdateCommands.Count);

            Assert.AreEqual(1, selector.Feedback.Count);
            var feedback = selector.Feedback[0];
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(command.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using a single command with the same ID as the last update.
        /// </summary>
        [TestMethod]
        public void TestSearch_Reinstall()
        {
            var guid = Guid.NewGuid().ToString();
            var command = new UpdateCommand { UpdateId = new UpdateId(guid, 5), UnitId = new UnitId("A") };

            var selector = new UpdateSelector(new[] { command });
            selector.Search(new UpdateId(guid, 5), new List<UpdateCommand>(), true);

            Assert.AreEqual(0, selector.CommandsToInstall.Count);

            Assert.AreEqual(0, selector.ValidatedParkedUpdateCommands.Count);

            Assert.AreEqual(1, selector.Feedback.Count);
            var feedback = selector.Feedback[0];
            Assert.AreEqual(UpdateState.Installed, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(command.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using a single command with a lower ID than the last update.
        /// </summary>
        [TestMethod]
        public void TestSearch_OldIgnored()
        {
            var guid = Guid.NewGuid().ToString();
            var command = new UpdateCommand { UpdateId = new UpdateId(guid, 4), UnitId = new UnitId("A") };

            var selector = new UpdateSelector(new[] { command });
            selector.Search(new UpdateId(guid, 5), new List<UpdateCommand>(), true);

            Assert.AreEqual(0, selector.CommandsToInstall.Count);

            Assert.AreEqual(0, selector.ValidatedParkedUpdateCommands.Count);

            Assert.AreEqual(1, selector.Feedback.Count);
            var feedback = selector.Feedback[0];
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(command.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using multiple commands in sequence.
        /// </summary>
        [TestMethod]
        public void TestSearch_Multiple()
        {
            var guid = Guid.NewGuid().ToString();
            var commandA = new UpdateCommand { UpdateId = new UpdateId(guid, 5), UnitId = new UnitId("A") };
            var commandB = new UpdateCommand { UpdateId = new UpdateId(guid, 6), UnitId = new UnitId("A") };
            var commandC = new UpdateCommand { UpdateId = new UpdateId(guid, 7), UnitId = new UnitId("A") };

            var selector = new UpdateSelector(new[] { commandA, commandB, commandC });
            selector.Search(new UpdateId(guid, 4), new List<UpdateCommand>(), true);

            Assert.AreEqual(1, selector.CommandsToInstall.Count);
            Assert.AreSame(commandC, selector.CommandsToInstall[0]);

            Assert.AreEqual(0, selector.ValidatedParkedUpdateCommands.Count);

            Assert.AreEqual(3, selector.Feedback.Count);

            var feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 5);
            Assert.IsNotNull(feedback);
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandA.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 6);
            Assert.IsNotNull(feedback);
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandB.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 7);
            Assert.IsNotNull(feedback);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandC.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using multiple commands
        /// in sequence, one with <see cref="RunCommands"/>.
        /// </summary>
        [TestMethod]
        public void TestSearch_MultipleWithRunCommands()
        {
            var guid = Guid.NewGuid().ToString();
            var commandA = new UpdateCommand
                               {
                                   UpdateId = new UpdateId(guid, 5),
                                   UnitId = new UnitId("A"),
                                   PreInstallation = new RunCommands { Items = { new RunApplication() } }
                               };
            var commandB = new UpdateCommand { UpdateId = new UpdateId(guid, 6), UnitId = new UnitId("A") };
            var commandC = new UpdateCommand { UpdateId = new UpdateId(guid, 7), UnitId = new UnitId("A") };

            var selector = new UpdateSelector(new[] { commandA, commandB, commandC });
            selector.Search(new UpdateId(guid, 4), new List<UpdateCommand>(), true);

            Assert.AreEqual(2, selector.CommandsToInstall.Count);
            Assert.AreSame(commandA, selector.CommandsToInstall[0]);
            Assert.AreSame(commandC, selector.CommandsToInstall[1]);

            Assert.AreEqual(0, selector.ValidatedParkedUpdateCommands.Count);

            Assert.AreEqual(3, selector.Feedback.Count);

            var feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 5);
            Assert.IsNotNull(feedback);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandA.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 6);
            Assert.IsNotNull(feedback);
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandB.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 7);
            Assert.IsNotNull(feedback);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandC.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using multiple commands
        /// from two different background systems.
        /// </summary>
        [TestMethod]
        public void TestSearch_MixedGuid()
        {
            var guidA = Guid.NewGuid().ToString();
            var guidB = Guid.NewGuid().ToString();
            var commandA = new UpdateCommand
                               {
                                   UpdateId = new UpdateId(guidA, 5),
                                   UnitId = new UnitId("A"),
                                   PreInstallation = new RunCommands { Items = { new RunApplication() } }
                               };
            var commandB = new UpdateCommand { UpdateId = new UpdateId(guidB, 1), UnitId = new UnitId("A") };
            var commandC = new UpdateCommand { UpdateId = new UpdateId(guidA, 6), UnitId = new UnitId("A") };

            var selector = new UpdateSelector(new[] { commandA, commandB, commandC });
            selector.Search(new UpdateId(guidA, 4), new List<UpdateCommand>(), true);

            Assert.AreEqual(1, selector.CommandsToInstall.Count);
            Assert.AreSame(commandB, selector.CommandsToInstall[0]);

            Assert.AreEqual(0, selector.ValidatedParkedUpdateCommands.Count);

            Assert.AreEqual(3, selector.Feedback.Count);

            var feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 5);
            Assert.IsNotNull(feedback);
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandA.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 1);
            Assert.IsNotNull(feedback);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandB.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 6);
            Assert.IsNotNull(feedback);
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandC.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using multiple commands
        /// from two different background systems.
        /// </summary>
        [TestMethod]
        public void TestSearch_MixedGuid2()
        {
            var guidA = Guid.NewGuid().ToString();
            var guidB = Guid.NewGuid().ToString();
            var commandA = new UpdateCommand { UpdateId = new UpdateId(guidA, 5), UnitId = new UnitId("A") };
            var commandB = new UpdateCommand { UpdateId = new UpdateId(guidB, 2), UnitId = new UnitId("A") };
            var commandC = new UpdateCommand { UpdateId = new UpdateId(guidA, 6), UnitId = new UnitId("A") };

            var selector = new UpdateSelector(new[] { commandA, commandB, commandC });
            selector.Search(new UpdateId(guidB, 1), new List<UpdateCommand>(), true);

            Assert.AreEqual(1, selector.CommandsToInstall.Count);
            Assert.AreSame(commandC, selector.CommandsToInstall[0]);

            Assert.AreEqual(0, selector.ValidatedParkedUpdateCommands.Count);

            Assert.AreEqual(3, selector.Feedback.Count);

            var feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 5);
            Assert.IsNotNull(feedback);
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandA.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 2);
            Assert.IsNotNull(feedback);
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandB.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 6);
            Assert.IsNotNull(feedback);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandC.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using a single parked command in sequence.
        /// </summary>
        [TestMethod]
        public void TestSearch_ParkedSimple()
        {
            var guid = Guid.NewGuid().ToString();
            var now = new DateTime(2010, 1, 25, 15, 00, 15);
            TimeProvider.Current = new ManualTimeProvider(now);
            var commandA = new UpdateCommand
                               {
                                   UpdateId = new UpdateId(guid, 5),
                                   UnitId = new UnitId("A"),
                                   ActivateTime = now.AddDays(-1)
                               };
            var commandB = new UpdateCommand
                              {
                                  UpdateId = new UpdateId(guid, 6),
                                  UnitId = new UnitId("A"),
                                  ActivateTime = now.AddDays(1)
                              };

            var selector = new UpdateSelector(new[] { commandA, commandB });
            selector.Search(new UpdateId(guid, 4), new List<UpdateCommand>(), true);

            Assert.AreEqual(1, selector.ValidatedParkedUpdateCommands.Count);
            Assert.AreSame(commandB, selector.ValidatedParkedUpdateCommands[0]);

            Assert.AreEqual(1, selector.CommandsToInstall.Count);
            Assert.AreSame(commandA, selector.CommandsToInstall[0]);

            Assert.AreEqual(2, selector.Feedback.Count);

            var feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 5);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandA.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 6);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandB.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using a single parked command.
        /// </summary>
        [TestMethod]
        public void TestSearch_ParkedSingle()
        {
            var guid = Guid.NewGuid().ToString();
            var now = new DateTime(2010, 1, 25, 15, 00, 15);
            TimeProvider.Current = new ManualTimeProvider(now);
            var commandB = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 6),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(1)
            };

            var selector = new UpdateSelector(new[] { commandB });
            selector.Search(new UpdateId(guid, 4), new List<UpdateCommand>(), true);

            Assert.AreEqual(1, selector.ValidatedParkedUpdateCommands.Count);
            Assert.AreSame(commandB, selector.ValidatedParkedUpdateCommands[0]);

            Assert.AreEqual(0, selector.CommandsToInstall.Count);

            Assert.AreEqual(1, selector.Feedback.Count);

            var feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 6);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandB.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using multiple parked commands in sequence.
        /// </summary>
        [TestMethod]
        public void TestSearch_ParkedMultiple()
        {
            var guid = Guid.NewGuid().ToString();
            var now = new DateTime(2010, 1, 25, 15, 00, 15);
            TimeProvider.Current = new ManualTimeProvider(now);
            var commandA = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 6),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(+3)
            };
            var commandB = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 5),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(-1)
            };
            var commandC = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 7),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(+2)
            };
            var selector = new UpdateSelector(new[] { commandA, commandB, commandC });
            selector.Search(new UpdateId(guid, 4), new List<UpdateCommand>(), true);

            Assert.AreEqual(1, selector.CommandsToInstall.Count);
            Assert.AreSame(commandB, selector.CommandsToInstall[0]);

            Assert.AreEqual(1, selector.ValidatedParkedUpdateCommands.Count);
            Assert.AreSame(commandC, selector.ValidatedParkedUpdateCommands[0]);

            Assert.AreEqual(3, selector.Feedback.Count);

            // #6 is ignored because #7 has an earlier activate time
            var feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 6);
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandA.UpdateId, feedback.UpdateId);

            // #5 will be installed right away since it is in the past
            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 5);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandB.UpdateId, feedback.UpdateId);

            // #7 will be parked
            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 7);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandC.UpdateId, feedback.UpdateId);

            var previouslyParkedUpdateCommands = new List<UpdateCommand>();
            previouslyParkedUpdateCommands.Add(commandC);
            var commandD = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 8),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(+3)
            };

            selector = new UpdateSelector(new[] { commandD });
            selector.Search(new UpdateId(guid, 5), previouslyParkedUpdateCommands, true);

            Assert.AreEqual(0, selector.CommandsToInstall.Count);

            Assert.AreEqual(1, selector.Feedback.Count);

            feedback = selector.Feedback[0];
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandD.UpdateId, feedback.UpdateId);

            Assert.AreEqual(2, selector.ValidatedParkedUpdateCommands.Count);
            Assert.AreSame(commandC, selector.ValidatedParkedUpdateCommands[0]);
            Assert.AreSame(commandD, selector.ValidatedParkedUpdateCommands[1]);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using multiple parked commands
        /// in sequence with flag InstallAfterBoot true and false.
        /// </summary>
        [TestMethod]
        public void TestSearch_ParkedMultipleWithInstallAfterBoot()
        {
            var guid = Guid.NewGuid().ToString();
            var now = new DateTime(2010, 1, 25, 15, 00, 15);
            TimeProvider.Current = new ManualTimeProvider(now);
            var commandA = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 6),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(+3),
                InstallAfterBoot = false
            };
            var commandB = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 5),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(-1),
                InstallAfterBoot = true
            };
            var commandC = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 7),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(+2),
                InstallAfterBoot = false
            };
            var commandD = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 8),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(+5),
                InstallAfterBoot = true
            };
            var selector = new UpdateSelector(new[] { commandA, commandB, commandC, commandD });
            selector.Search(new UpdateId(guid, 4), new List<UpdateCommand>(), true);
            Assert.AreEqual(0, selector.CommandsToInstall.Count);

            Assert.AreEqual(3, selector.ValidatedParkedUpdateCommands.Count);

            Assert.AreSame(commandD, selector.ValidatedParkedUpdateCommands[0]);
            Assert.AreSame(commandC, selector.ValidatedParkedUpdateCommands[1]);
            Assert.AreSame(commandB, selector.ValidatedParkedUpdateCommands[2]);

            Assert.AreEqual(4, selector.Feedback.Count);

            // #6 is ignored because #7 has an earlier activate time
            var feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 6);
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandA.UpdateId, feedback.UpdateId);

            // #5 will be installed right away since it is in the past
            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 5);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandB.UpdateId, feedback.UpdateId);

            // #7 will be parked
            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 7);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandC.UpdateId, feedback.UpdateId);

            // #8 will be parked
            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 8);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandD.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using two commands in sequence, one to be ignored.
        /// </summary>
        [TestMethod]
        public void TestSearch_OneIgnored()
        {
            var guid = Guid.NewGuid().ToString();
            var now = new DateTime(2010, 1, 25, 15, 00, 15);
            TimeProvider.Current = new ManualTimeProvider(now);
            var commandA = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 5),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(1)
            };
            var commandB = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 6),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(-1)
            };

            var selector = new UpdateSelector(new[] { commandA, commandB });
            selector.Search(new UpdateId(guid, 4), new List<UpdateCommand>(), true);

            Assert.AreEqual(0, selector.ValidatedParkedUpdateCommands.Count);

            Assert.AreEqual(1, selector.CommandsToInstall.Count);
            Assert.AreSame(commandB, selector.CommandsToInstall[0]);

            Assert.AreEqual(2, selector.Feedback.Count);

            var feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 5);
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandA.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 6);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandB.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using two commands in sequence, one to be ignored
        /// even if it has run commands configured.
        /// </summary>
        [TestMethod]
        public void TestSearch_NoParked_RunCommands()
        {
            var guid = Guid.NewGuid().ToString();
            var now = new DateTime(2010, 1, 25, 15, 00, 15);
            TimeProvider.Current = new ManualTimeProvider(now);
            var commandA = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 5),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(1),
                PreInstallation = new RunCommands { Items = { new RunApplication() } }
            };
            var commandB = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 6),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(-1)
            };

            var selector = new UpdateSelector(new[] { commandA, commandB });
            selector.Search(new UpdateId(guid, 4), new List<UpdateCommand>(), true);

            Assert.AreEqual(0, selector.ValidatedParkedUpdateCommands.Count);

            Assert.AreEqual(1, selector.CommandsToInstall.Count);
            Assert.AreSame(commandB, selector.CommandsToInstall[0]);

            Assert.AreEqual(2, selector.Feedback.Count);

            var feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 5);
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandA.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 6);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandB.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using multiple commands
        /// in sequence, one with <see cref="RunCommands"/> with index lower than parked update.
        /// </summary>
        [TestMethod]
        public void TestSearch_ParkedWithRunCommands()
        {
            var guid = Guid.NewGuid().ToString();
            var now = new DateTime(2010, 1, 25, 15, 00, 15);
            TimeProvider.Current = new ManualTimeProvider(now);
            var commandA = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 8),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(1)
            };
            var commandB = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 7),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(-1)
            };

            var commandC = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 5),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(1),
                PreInstallation = new RunCommands { Items = { new RunApplication() } }
            };

            var selector = new UpdateSelector(new[] { commandA, commandB, commandC });
            selector.Search(new UpdateId(guid, 4), new List<UpdateCommand>(), true);

            Assert.AreEqual(1, selector.ValidatedParkedUpdateCommands.Count);
            Assert.AreSame(commandA, selector.ValidatedParkedUpdateCommands[0]);

            Assert.AreEqual(1, selector.CommandsToInstall.Count);
            Assert.AreSame(commandB, selector.CommandsToInstall[0]);

            Assert.AreEqual(3, selector.Feedback.Count);

            var feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 8);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandA.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 7);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandB.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 5);
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandC.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using multiple commands
        /// in sequence, one with <see cref="RunCommands"/> with index higher than parked update.
        /// </summary>
        [TestMethod]
        public void TestSearch_ParkedWithRunCommandsHigherIndex()
        {
            var guid = Guid.NewGuid().ToString();
            var now = new DateTime(2010, 1, 25, 15, 00, 15);
            TimeProvider.Current = new ManualTimeProvider(now);
            var commandA = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 7),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(1)
            };
            var commandB = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 6),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(-1)
            };

            var commandC = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 8),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(1),
                PreInstallation = new RunCommands { Items = { new RunApplication() } }
            };

            var selector = new UpdateSelector(new[] { commandA, commandB, commandC });
            selector.Search(new UpdateId(guid, 4), new List<UpdateCommand>(), true);

            Assert.AreEqual(1, selector.ValidatedParkedUpdateCommands.Count);
            Assert.AreSame(commandC, selector.ValidatedParkedUpdateCommands[0]);

            Assert.AreEqual(1, selector.CommandsToInstall.Count);
            Assert.AreSame(commandB, selector.CommandsToInstall[0]);

            Assert.AreEqual(3, selector.Feedback.Count);

            var feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 7);
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandA.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 6);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandB.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 8);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandC.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using multiple parked commands in sequence
        /// with reception of a new command with higher index.
        /// </summary>
        [TestMethod]
        public void TestSearch_OldParkedWithNewHigherIndexedCommand()
        {
            var guid = Guid.NewGuid().ToString();
            var now = new DateTime(2010, 1, 25, 15, 00, 15);
            TimeProvider.Current = new ManualTimeProvider(now);

            var commandA = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 6),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(+3)
            };

            var commandB = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 7),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(+2)
            };

            var commandC = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 8),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(-1)
            };

            var oldParkedUpdates = new List<UpdateCommand>();
            oldParkedUpdates.Add(commandA);
            oldParkedUpdates.Add(commandB);

            var selector = new UpdateSelector(new[] { commandC });
            selector.Search(new UpdateId(guid, 4), oldParkedUpdates, true);

            Assert.AreEqual(0, selector.ValidatedParkedUpdateCommands.Count);

            Assert.AreEqual(1, selector.CommandsToInstall.Count);
            Assert.AreSame(commandC, selector.CommandsToInstall[0]);

            Assert.AreEqual(3, selector.Feedback.Count);

            var feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 6);
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandA.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 7);
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandB.UpdateId, feedback.UpdateId);

            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 8);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandC.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using multiple parked commands in sequence
        /// with reception of parked update with same index
        /// </summary>
        [TestMethod]
        public void TestSearch_OldParkedWithSameIndexedParkedCommand()
        {
            var guid = Guid.NewGuid().ToString();
            var now = new DateTime(2010, 1, 25, 15, 00, 15);
            TimeProvider.Current = new ManualTimeProvider(now);

            var commandA = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 6),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(+3)
            };

            var commandB = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 6),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(+3)
            };

            var oldParkedUpdates = new List<UpdateCommand>();
            oldParkedUpdates.Add(commandA);

            var selector = new UpdateSelector(new[] { commandB });
            selector.Search(new UpdateId(guid, 4), oldParkedUpdates, true);

            Assert.AreEqual(1, selector.ValidatedParkedUpdateCommands.Count);

            Assert.AreEqual(0, selector.CommandsToInstall.Count);

            Assert.AreEqual(1, selector.Feedback.Count);

            var feedback = selector.Feedback[0];
            Assert.AreEqual(UpdateState.Ignored, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandB.UpdateId, feedback.UpdateId);
        }

        /// <summary>
        /// Test for <see cref="UpdateSelector.Search"/> using multiple parked commands in sequence.
        /// </summary>
        [TestMethod]
        public void TestSearch_ParkedThreeWithIncreasingIndexes()
        {
            var guid = Guid.NewGuid().ToString();
            var now = new DateTime(2010, 1, 25, 15, 00, 15);
            TimeProvider.Current = new ManualTimeProvider(now);
            var commandA = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 2),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(+5)
            };
            var commandB = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 3),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(+10)
            };
            var commandC = new UpdateCommand
            {
                UpdateId = new UpdateId(guid, 4),
                UnitId = new UnitId("A"),
                ActivateTime = now.AddDays(+15)
            };
            var selector = new UpdateSelector(new[] { commandA, commandB, commandC });
            selector.Search(new UpdateId(guid, 1), new List<UpdateCommand>(), true);

            Assert.AreEqual(0, selector.CommandsToInstall.Count);

            Assert.AreEqual(3, selector.ValidatedParkedUpdateCommands.Count);
            Assert.AreSame(commandC, selector.ValidatedParkedUpdateCommands[0]);
            Assert.AreSame(commandB, selector.ValidatedParkedUpdateCommands[1]);
            Assert.AreSame(commandA, selector.ValidatedParkedUpdateCommands[2]);

            Assert.AreEqual(3, selector.Feedback.Count);

            // #6 is ignored because #7 has an earlier activate time
            var feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 2);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandA.UpdateId, feedback.UpdateId);

            // #5 will be installed right away since it is in the past
            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 3);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandB.UpdateId, feedback.UpdateId);

            // #7 will be parked
            feedback = selector.Feedback.First(i => i.UpdateId.UpdateIndex == 4);
            Assert.AreEqual(UpdateState.Transferred, feedback.State);
            Assert.AreEqual("A", feedback.UnitId.UnitName);
            Assert.AreEqual(commandC.UpdateId, feedback.UpdateId);
        }

        // ReSharper restore InconsistentNaming
    }
}
