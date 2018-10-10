// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Shell.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Shell type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProtectedOperationsTester.ViewModels
{
    using System;
    using System.ComponentModel.Composition;
    using System.Windows.Input;

    using ProtectedOperationsTester.Contracts;

    [Export]
    public class Shell
    {
        private readonly Lazy<ICommand> getUnitLoginCommand;

        private readonly Lazy<ICommand> getUnitLegacyCommand;

        private readonly Lazy<ICommand> getUnitCertificateCommand;

        [Import(Commands.GetUnitLogin)]
        private Func<ICommand> getUnitLoginCommandFactory;

        [Import(Commands.GetUnitLegacy)]
        private Func<ICommand> getUnitLegacyCommandFactory;

        [Import(Commands.GetUnitCertificate)]
        private Func<ICommand> getUnitCertificateCommandFactory;

        [ImportingConstructor]
        public Shell()
        {
            this.getUnitLoginCommand = new Lazy<ICommand>(this.CreateGetUnitLoginCommand);
            this.getUnitLegacyCommand = new Lazy<ICommand>(this.CreateGetUnitLegacyCommand);
            this.getUnitCertificateCommand = new Lazy<ICommand>(this.CreateGetUnitCertificateCommand);
            this.Username = "gorba";
            this.Password = "gorba";
            this.DnsIdentity = "BackgroundSystem";
            this.Certificate = "CenterOnline";
        }

        public string Certificate { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public ICommand GetUnitLegacyCommand
        {
            get
            {
                return this.getUnitLegacyCommand.Value;
            }
        }

        public ICommand GetUnitLoginCommand
        {
            get
            {
                return this.getUnitLoginCommand.Value;
            }
        }

        public ICommand GetUnitCertificateCommand
        {
            get
            {
                return this.getUnitCertificateCommand.Value;
            }
        }

        public string DnsIdentity { get; set; }

        private ICommand CreateGetUnitLegacyCommand()
        {
            return this.getUnitLegacyCommandFactory();
        }

        private ICommand CreateGetUnitLoginCommand()
        {
            return this.getUnitLoginCommandFactory();
        }

        private ICommand CreateGetUnitCertificateCommand()
        {
            return this.getUnitCertificateCommandFactory();
        }
    }
}
