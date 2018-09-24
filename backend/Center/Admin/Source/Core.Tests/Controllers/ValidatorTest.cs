// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for all different validation types (unique string, item is null,...)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.Entities;
    using Gorba.Center.Admin.Core.Controllers.Entities.AccessControl;
    using Gorba.Center.Admin.Core.Controllers.Entities.Membership;
    using Gorba.Center.Admin.Core.Controllers.Entities.Units;
    using Gorba.Center.Admin.Core.Controllers.Entities.Update;
    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.DataViewModels.AccessControl;
    using Gorba.Center.Admin.Core.DataViewModels.Membership;
    using Gorba.Center.Admin.Core.DataViewModels.Units;
    using Gorba.Center.Admin.Core.DataViewModels.Update;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Common.Update.ServiceModel.Common;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for all different validation types (unique string, item is null,...)
    /// </summary>
    [TestClass]
    public class ValidatorTest
    {
        /// <summary>
        /// The initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Helpers.ResetServiceLocator();
        }

        /// <summary>
        /// The cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            Helpers.ResetServiceLocator();
        }

        /// <summary>
        /// The access control validators test.
        /// </summary>
        [TestMethod]
        public void AuthorizationValidatorSTest()
        {
            // init general
            var commandRegistry = new CommandRegistry();
            var factory = new DataViewModelFactory(commandRegistry);
            var dataController = new DataController(factory);

            // init specific
            var authorization = new Authorization();
            var readableModel = new AuthorizationReadableModel(authorization);

            var udpContext = Mock.Of<IUserRoleUdpContext>();

            var dataViewModel = dataController.Factory.Create(readableModel.ToChangeTrackingModel());

            var validator = new AuthorizationValidatorMock(dataViewModel, dataController);

            // test
            validator.TestValidation("UserRole");
            Assert.IsTrue(validator.DataViewModel.HasErrors);

            validator.DataViewModel.ClearErrors();
            var userRoleReadableModel = new UserRoleReadableModel(new UserRole());
            validator.DataViewModel.UserRole.SelectedEntity = new UserRoleReadOnlyDataViewModel(
                userRoleReadableModel,
                udpContext,
                factory);
            Assert.IsFalse(validator.DataViewModel.HasErrors);
        }

        /// <summary>
        /// The user role validator test.
        /// </summary>
        [TestMethod]
        public void UserRoleValidatorTest()
        {
            var initDataControllerSetup = InitDataController();

            // init specific
            var authorization = new UserRole();
            var readableModel = new UserRoleReadableModel(authorization);

            var dataViewModel =
                initDataControllerSetup.DataController.Factory.Create(readableModel.ToChangeTrackingModel());
            var validator = new UserRoleValidatorMock(dataViewModel, initDataControllerSetup.DataController);

            // test
            // null or empty
            validator.TestValidation("Name");
            validator.DataViewModel.Name = string.Empty;
            Assert.IsTrue(validator.DataViewModel.HasErrors);

            // valid name
            validator.DataViewModel.ClearErrors();
            validator.DataViewModel.Name = "John";
            validator.TestValidation("Name");
            Assert.IsFalse(validator.DataViewModel.HasErrors);
        }

        /// <summary>
        /// The user role with entries validator test.
        /// </summary>
        [TestMethod]
        public void UserRoleWithEntriesValidatorTest()
        {
            // init general
            var initDataController = InitDataController();
            var connectionControllerSetup = initDataController.ConnectionControllerSetup;
            var userList = new List<UserRoleReadableModel>
                           {
                               new UserRoleReadableModelMock(new UserRole { Id = 1, Name = "John" })
                           };

            connectionControllerSetup.UserRoleManagerMock.Setup(
                m => m.QueryAsync(null))
                .Returns(Task.FromResult((IEnumerable<UserRoleReadableModel>)userList));
            connectionControllerSetup.UserDefinedPropertyManagerMock.Setup(
                u => u.QueryAsync(It.IsAny<UserDefinedPropertyQuery>()))
                .Returns(Task.FromResult(Enumerable.Empty<UserDefinedPropertyReadableModel>()));

            // init specific
            var authorization = new UserRole { Id = 12 };
            var readableModel = new UserRoleReadableModel(authorization);

            var dataViewModel = initDataController.DataController.Factory.Create(readableModel.ToChangeTrackingModel());

            var validator = new UserRoleValidatorMock(dataViewModel, initDataController.DataController);

            // unique name
            validator.DataViewModel.ClearErrors();
            validator.DataViewModel.Name = "John";
            validator.TestValidation("Name");
            Assert.IsTrue(validator.DataViewModel.HasErrors);

            // ok
            validator.DataViewModel.ClearErrors();
            validator.DataViewModel.Name = "Tom";
            validator.TestValidation("Name");
            Assert.IsFalse(validator.DataViewModel.HasErrors);
        }

        /// <summary>
        /// The user role validator test.
        /// </summary>
        [TestMethod]
        public void UserValidatorTest()
        {
            // init general
            var initDataControllerSetup = InitDataController();

            // init specific
            var user = new User();
            var readableModel = new UserReadableModel(user);

            var dataViewModel =
                initDataControllerSetup.DataController.Factory.Create(readableModel.ToChangeTrackingModel());

            var validator = new UserValidatorMock(dataViewModel, initDataControllerSetup.DataController);

            // test
            // null
            validator.DataViewModel.Email = null;
            validator.TestValidation("Email");
            Assert.IsTrue(validator.DataViewModel.HasErrors);

            // empty
            validator.DataViewModel.ClearErrors();
            validator.DataViewModel.Email = string.Empty;
            validator.TestValidation("Email");
            Assert.IsTrue(validator.DataViewModel.HasErrors);

            // invalid mail format
            validator.DataViewModel.ClearErrors();
            validator.DataViewModel.Email = "NotValid.de";
            validator.TestValidation("Email");
            Assert.IsTrue(validator.DataViewModel.HasErrors);

            // valid
            validator.DataViewModel.ClearErrors();
            validator.DataViewModel.Email = "my@mail.de";
            validator.TestValidation("Email");
            Assert.IsFalse(validator.DataViewModel.HasErrors);

            // test taht a tenant is selected
            validator.DataViewModel.ClearErrors();
            validator.DataViewModel.OwnerTenant.SelectedEntity = null;
            validator.TestValidation("OwnerTenant");
            Assert.IsTrue(validator.DataViewModel.HasErrors);
        }

        /// <summary>
        /// The unit validator test.
        /// </summary>
        [TestMethod]
        public void UnitValidatorTest()
        {
            var initDataControllerSetup = InitDataController();

            // init specific
            var unit = new Unit();
            var readableModel = new UnitReadableModel(unit);

            var udpContext = new Mock<IUnitUdpContext>();
            var readOnlyDataViewModel = new UnitReadOnlyDataViewModel(
                readableModel,
                udpContext.Object,
                initDataControllerSetup.DataController.Factory);
            var dataViewModel = initDataControllerSetup.DataController.Factory.Create(readOnlyDataViewModel);

            var validator = new UnitValidatorMock(dataViewModel, initDataControllerSetup.DataController);

            // test
            // null
            validator.TestValidation("Name");
            validator.DataViewModel.Name = null;
            Assert.IsTrue(validator.DataViewModel.HasErrors);

            // empty
            validator.DataViewModel.ClearErrors();
            validator.DataViewModel.Name = string.Empty;
            validator.TestValidation("Name");
            Assert.IsTrue(validator.DataViewModel.HasErrors);

            // invalid name not HEX
            validator.DataViewModel.ClearErrors();
            validator.DataViewModel.Name = "TFT-aa-bb-cz";
            validator.TestValidation("Name");
            Assert.IsTrue(validator.DataViewModel.HasErrors);

            // invalid name not HEX upper case
            validator.DataViewModel.ClearErrors();
            validator.DataViewModel.Name = "TFT-01-02-0a";
            validator.TestValidation("Name");
            Assert.IsTrue(validator.DataViewModel.HasErrors);

            // invalid name format
            validator.DataViewModel.ClearErrors();
            validator.DataViewModel.Name = "TFT-0102-03";
            validator.TestValidation("Name");
            Assert.IsTrue(validator.DataViewModel.HasErrors);

            // valid name
            validator.DataViewModel.ClearErrors();
            validator.DataViewModel.Name = "TFT-01-02-0A";
            validator.TestValidation("Name");
            Assert.IsFalse(validator.DataViewModel.HasErrors);
        }

        /// <summary>
        /// The update part validator test.
        /// </summary>
        [TestMethod]
        public void UpdatePartValidatorTest()
        {
            var initDataControllerSetup = InitDataController();

            // init specific
            var updatePart = new UpdatePart();
            var readableModel = new UpdatePartReadableModel(updatePart);

            var dataViewModel =
                initDataControllerSetup.DataController.Factory.Create(readableModel.ToChangeTrackingModel());

            var validator = new UpdatePartValidatorMock(dataViewModel, initDataControllerSetup.DataController);

            // test
            // time set to default should be changed by user
            validator.DataViewModel.ClearErrors();
            validator.DataViewModel.Start = default(DateTime);
            validator.TestValidation("Start");
            Assert.IsTrue(validator.DataViewModel.HasErrors);

            validator.DataViewModel.ClearErrors();
            validator.DataViewModel.Start = DateTime.Now;

            validator.TestValidation("Start");
            Assert.IsFalse(validator.DataViewModel.HasErrors);

            // deserialize empty xml
            validator.DataViewModel.ClearErrors();
            validator.DataViewModel.Structure.XmlData = new XmlData();
            validator.TestValidation("Structure");
            Assert.IsTrue(validator.DataViewModel.HasErrors);

            // deserialize valid xml
            validator.DataViewModel.ClearErrors();
            var folderStructure = new UpdateFolderStructure();
            validator.DataViewModel.Structure.XmlData = new XmlData(folderStructure);
            validator.TestValidation("Structure");
            Assert.IsFalse(validator.DataViewModel.HasErrors);
        }

        private static InitDataControllerSetup InitDataController()
        {
            var tenant = new Tenant { Id = 1, Name = "TestTenant" };
            var container = Helpers.InitializeServiceLocator();
            Helpers.CreateApplicationStateMock(container, tenant);

            var connectionControllerSetup = Helpers.CreateConnectionControllerMock();
            connectionControllerSetup.UnitManagerMock.Setup(u => u.QueryAsync(It.IsAny<UnitQuery>()))
                .Returns(Task.FromResult(Enumerable.Empty<UnitReadableModel>()));
            var dataController = Helpers.SetupDataController(connectionControllerSetup.ConnectionControllerMock);

            var result = new InitDataControllerSetup
                             {
                                 DataController = dataController,
                                 ConnectionControllerSetup = connectionControllerSetup
                             };
            return result;
        }

        private class InitDataControllerSetup
        {
            public Helpers.ConnectionControllerResult ConnectionControllerSetup { get; set; }

            public DataController DataController { get; set; }
        }

        private class AuthorizationValidatorMock : AuthorizationValidator
        {
            public AuthorizationValidatorMock(AuthorizationDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
            }

            public void TestValidation(string propertyName)
            {
                this.HandleDataViewModelChange(propertyName);
            }
        }

        private class UpdatePartValidatorMock : UpdatePartValidator
        {
            public UpdatePartValidatorMock(UpdatePartDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
            }

            public void TestValidation(string propertyName)
            {
                this.HandleDataViewModelChange(propertyName);
            }
        }

        private class UserRoleReadableModelMock : UserRoleReadableModel
        {
            public UserRoleReadableModelMock(UserRole entity)
                : base(entity)
            {
                this.Populate();
            }
        }

        private class UserRoleValidatorMock : UserRoleValidator
        {
            public UserRoleValidatorMock(UserRoleDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
            }

            public void TestValidation(string propertyName)
            {
                this.HandleDataViewModelChange(propertyName);
            }
        }

        private class UserValidatorMock : UserValidator
        {
            public UserValidatorMock(UserDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
            }

            public void TestValidation(string propertyName)
            {
                this.HandleDataViewModelChange(propertyName);
            }
        }

        private class UnitValidatorMock : UnitValidator
        {
            public UnitValidatorMock(UnitDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
            }

            public void TestValidation(string propertyName)
            {
                this.HandleDataViewModelChange(propertyName);
            }
        }
    }
}
