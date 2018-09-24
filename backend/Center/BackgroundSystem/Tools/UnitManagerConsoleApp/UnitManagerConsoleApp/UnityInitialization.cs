// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnityInitialization.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnityInitialization type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnitManagerConsoleApp
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.Core.Communication;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.DTO.Units;

    using Moq;

    /// <summary>
    /// Assembly initialization and cleanup methods.
    /// </summary>
    public class UnityInitialization
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityInitialization"/> class.
        /// </summary>
        /// <param name="addressList">
        /// The address List containing network addresses of units.
        /// </param>
        public static void Initialise(IEnumerable<string> addressList)
        {
            var unitServiceMock = new Mock<IUnitService>();
            var unitServiceProviderMock = new Mock<ProxyProvider<IUnitService>>();

            var unitList = addressList.Select(address => new Unit { NetworkAddress = address }).ToList();
            var unit = unitList.FirstOrDefault();

            unitServiceMock.Setup(u => u.ListUnits(null)).Returns(unitList);
            if (unit != null)
            {
                unitServiceMock.Setup(u => u.ListUnits(It.IsAny<FilterBase>())).Returns(new List<Unit> { unit });
            }
            else
            {
                unitServiceMock.Setup(u => u.ListUnits(It.IsAny<FilterBase>())).Returns(new List<Unit>());
            }

            unitServiceProviderMock.Setup(p => p.Provide()).Returns(unitServiceMock.Object);
            ProxyProvider<IUnitService>.Current = unitServiceProviderMock.Object;
        }
    }
}