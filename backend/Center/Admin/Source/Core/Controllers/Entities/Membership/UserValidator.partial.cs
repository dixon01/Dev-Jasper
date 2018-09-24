// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Membership
{
    using System;
    using System.Linq;
    using System.Net.Mail;

    using Gorba.Center.Admin.Core.DataViewModels.Membership;
    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// Specific implementation of <see cref="UserValidator"/>.
    /// </summary>
    public partial class UserValidator
    {
        partial void ValidateOwnerTenant(UserDataViewModel dvm)
        {
            dvm.ChangeError(
                "OwnerTenant", AdminStrings.Errors_NoItemSelected, dvm.OwnerTenant.SelectedEntity == null);
        }

        partial void ValidateUsername(UserDataViewModel dvm)
        {
            dvm.ChangeError("Username", AdminStrings.Errors_TextNotWhitespace, string.IsNullOrEmpty(dvm.Username));
            dvm.ChangeError(
                "Username",
                AdminStrings.Errors_DuplicateName,
                this.DataController.User.All.Any(u => u.Id != dvm.Id && u.Username.Equals(dvm.Username)));
        }

        partial void ValidateDomain(UserDataViewModel dvm)
        {
            this.ValidateDomainAndPassword(dvm);
        }

        partial void ValidateHashedPassword(UserDataViewModel dvm)
        {
            this.ValidateDomainAndPassword(dvm);
        }

        private void ValidateDomainAndPassword(UserDataViewModel dvm)
        {
            var hasError = (dvm.HashedPassword == null || dvm.HashedPassword.Length != 32)
                           && string.IsNullOrEmpty(dvm.Domain);
            dvm.ChangeError("Domain", AdminStrings.Errors_ProvidePasswordOrDomain, hasError);
            dvm.ChangeError("HashedPassword", AdminStrings.Errors_ProvidePasswordOrDomain, hasError);
        }

        partial void ValidateEmail(UserDataViewModel dvm)
        {
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new MailAddress(dvm.Email);
                dvm.RemoveError("Email", AdminStrings.Errors_ValidEmail);
            }
            catch (Exception)
            {
                dvm.AddError("Email", AdminStrings.Errors_ValidEmail);
            }
        }

        partial void ValidateCulture(UserDataViewModel dvm)
        {
            dvm.ChangeError(
                "CultureSelection", AdminStrings.Errors_NoItemSelected, dvm.CultureSelection.SelectedItem == null);
        }

        partial void ValidateTimeZone(UserDataViewModel dvm)
        {
            dvm.ChangeError(
                "TimeZoneSelection", AdminStrings.Errors_NoItemSelected, dvm.TimeZoneSelection.SelectedItem == null);
        }
    }
}