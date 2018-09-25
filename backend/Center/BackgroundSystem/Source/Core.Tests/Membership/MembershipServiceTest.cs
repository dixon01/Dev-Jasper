// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MembershipServiceTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MembershipServiceTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Tests.Membership
{
    using System.ServiceModel;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Membership;
    using Gorba.Center.BackgroundSystem.Data.Access;
    using Gorba.Center.BackgroundSystem.Data.Model.Membership;
    using Gorba.Center.Common.ServiceModel.Security;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for the <see cref="MembershipService"/>.
    /// </summary>
    [TestClass]
    public class MembershipServiceTest
    {
        /// <summary>
        /// Tests that no exception is thrown when the service is used locally (and the
        /// <see cref="OperationContext.Current"/> is not available.
        /// </summary>
        [TestMethod]
        public void TestRemoteAddressBehaviorInLocalContext()
        {
            var userInfo = new UserInfo(true, "login");
            var currentContextUserInfoProviderMock = new Mock<CurrentContextUserInfoProvider>();
            currentContextUserInfoProviderMock.Setup(provider => provider.GetUserInfo()).Returns(userInfo);
            CurrentContextUserInfoProvider.SetCurrent(currentContextUserInfoProviderMock.Object);

            var repositoryMock = new Mock<IRepository<User>>();
            var user = new User { Username = "login", HashedPassword = "password" };
            var users = new FakeDbSet<User> { user };
            repositoryMock.Setup(repository => repository.Query()).Returns(users);
            var userTask = new TaskCompletionSource<User>();
            userTask.SetResult(user);
            repositoryMock.Setup(repository => repository.UpdateAsync(user)).Returns(userTask.Task);

            var repositoryFactoryMock = new Mock<UserRepositoryFactory>();
            repositoryFactoryMock.Setup(factory => factory.Create()).Returns(repositoryMock.Object);
            UserRepositoryFactory.SetInstance(repositoryFactoryMock.Object);

            var membershipService = new MembershipService();
            var authenticatedUser = membershipService.AuthenticateUserAsync().Result;
            Assert.IsNotNull(authenticatedUser);
            currentContextUserInfoProviderMock.Verify(provider => provider.GetUserInfo(), Times.Once());
        }
    }
}