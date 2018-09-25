using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProtectedOperationsTests.Tests
{
    using ProtectedOperationsTester.Core;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var bootstrapper = new Bootstrapper();
            var shell = bootstrapper.Bootstrap();
            Assert.IsNotNull(shell.GetUnitCertificateCommand);
            Assert.IsNotNull(shell.GetUnitLoginCommand);
        }
    }
}
