// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationViewModelTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ValidationViewModelTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Core.Tests
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Gorba.Center.Common.Wpf.Core.Validation;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the <see cref="ValidationViewModelBase"/>.
    /// </summary>
    [TestClass]
    public class ValidationViewModelTest
    {
        private const string CountError = "CountError";

        private const string CustomValidationError = "CustomValidationError";

        private const string MixCountError = "MixCountError";

        private const string MixNameError = "MixNameError";

        private const string RequiredError = "RequiredError";

        /// <summary>
        /// Tests a class with predefined (<see cref="System.ComponentModel.DataAnnotations"/> namespace)
        /// validation attributes.
        /// </summary>
        [TestMethod]
        public void PredefinedValidationAttributesTest()
        {
            var validation = new PredefinedValidationAttributes();
            var dataErrorInfo = validation as IDataErrorInfo;

            // object doesn't have a name. We expect that error, but the Count is valid.
            Assert.AreEqual("Name", dataErrorInfo.Error);
            Assert.AreEqual(ValidationViewModelTest.RequiredError, dataErrorInfo["Name"]);
            Assert.AreEqual(string.Empty, dataErrorInfo["Count"]);
            validation.Count = -1;

            // now both Name and Count contain validation errors.
            Assert.AreEqual("Count, Name", dataErrorInfo.Error);
            Assert.AreEqual(ValidationViewModelTest.CountError, dataErrorInfo["Count"]);
            validation.Name = "Name";

            // now the Name is valid, but the Count is still not valid.
            Assert.AreEqual("Count", dataErrorInfo.Error);
            Assert.AreEqual(string.Empty, dataErrorInfo["Name"]);
            Assert.AreEqual(ValidationViewModelTest.CountError, dataErrorInfo["Count"]);
            Assert.AreEqual("Count", dataErrorInfo.Error);
            validation.Count = 0;

            // now everything should be valid.
            Assert.AreEqual(string.Empty, dataErrorInfo.Error);
            Assert.AreEqual(string.Empty, dataErrorInfo["Count"]);
        }

        /// <summary>
        /// Tests custom validation.
        /// </summary>
        [TestMethod]
        public void CustomValidationTest()
        {
            var validation = new CustomValidation();
            var dataErrorInfo = validation as IDataErrorInfo;

            // the default Name is valid.
            Assert.AreEqual(string.Empty, dataErrorInfo.Error);
            Assert.AreEqual(string.Empty, dataErrorInfo["Value"]);
            validation.Value = "Valid";

            // the Name is still valid.
            Assert.AreEqual(string.Empty, dataErrorInfo.Error);
            Assert.AreEqual(string.Empty, dataErrorInfo["Value"]);
            validation.Value = "Forbidden";

            // The name is not valid anymore.
            Assert.AreEqual("Value", dataErrorInfo.Error);
            Assert.AreEqual(ValidationViewModelTest.CustomValidationError, dataErrorInfo["Value"]);
        }

        /// <summary>
        /// Tests mixed validation
        /// </summary>
        [TestMethod]
        public void MixValidationTest()
        {
            var validation = new MixValidation();
            var dataErrorInfo = validation as IDataErrorInfo;

            // by default, the Name isn't valid.
            Assert.AreEqual("Name", dataErrorInfo.Error);
            Assert.AreEqual(ValidationViewModelTest.MixNameError, dataErrorInfo["Name"]);
            Assert.AreEqual(string.Empty, dataErrorInfo["Count"]);

            validation.Count = -1;

            // now also the Count is not valid anymore.
            Assert.AreEqual("Count, Name", dataErrorInfo.Error);
            Assert.AreEqual(ValidationViewModelTest.MixNameError, dataErrorInfo["Name"]);
            Assert.AreEqual(ValidationViewModelTest.MixCountError, dataErrorInfo["Count"]);

            validation.Name = "Name";

            // now only the Count is not valid.
            Assert.AreEqual("Count", dataErrorInfo.Error);
            Assert.AreEqual(string.Empty, dataErrorInfo["Name"]);
            Assert.AreEqual(ValidationViewModelTest.MixCountError, dataErrorInfo["Count"]);

            validation.Count = 0;

            // everything is valid.
            Assert.AreEqual(string.Empty, dataErrorInfo.Error);
            Assert.AreEqual(string.Empty, dataErrorInfo["Name"]);
            Assert.AreEqual(string.Empty, dataErrorInfo["Count"]);
        }

        private class PredefinedValidationAttributes : ValidationViewModelBase
        {
            [Range(0, int.MaxValue, ErrorMessage = ValidationViewModelTest.CountError)]
            public int Count { get; set; }

            [Required(ErrorMessage = ValidationViewModelTest.RequiredError)]
            public string Name { get; set; }
        }

        private class CustomValidation : ValidationViewModelBase
        {
            public string Value { get; set; }

            protected override IEnumerable<string> Validate(string propertyName)
            {
                switch (propertyName)
                {
                    case "Value":
                        return new[] { this.ValidateValue() };
                }

                return base.Validate(propertyName);
            }

            private string ValidateValue()
            {
                if (string.Equals("Forbidden", this.Value))
                {
                    return ValidationViewModelTest.CustomValidationError;
                }

                return string.Empty;
            }
        }

        private class MixValidation : ValidationViewModelBase
        {
            [Range(0, int.MaxValue, ErrorMessage = ValidationViewModelTest.MixCountError)]
            public int Count { get; set; }

            public string Name { get; set; }

            protected override IEnumerable<string> Validate(string propertyName)
            {
                switch (propertyName)
                {
                    case "Name":
                        return new[] { this.ValidateName() };
                }

                return base.Validate(propertyName);
            }

            private string ValidateName()
            {
                if (string.IsNullOrWhiteSpace(this.Name))
                {
                    return ValidationViewModelTest.MixNameError;
                }

                return string.Empty;
            }
        }
    }
}
