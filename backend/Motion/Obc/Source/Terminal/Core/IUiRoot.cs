// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUiRoot.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IUiRoot type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    /// This is an AbstractFactory
    /// (GoF pattern - <see cref="http://www.dofactory.com/Patterns/PatternAbstract.aspx#_self1"/>)
    /// This Factory will create/provide all GUI references
    /// </summary>
    public interface IUiRoot
    {
        // [wes] todo: why are we handling key events in the factory?!

        /// <summary>
        /// The short key pressed event.
        /// </summary>
        event EventHandler<ShortKeyEventArgs> ShortKeyPressed;

        /// <summary>
        /// Gets the icon bar.
        /// </summary>
        IIconBar IconBar { get; }

        /// <summary>
        /// Gets the button bar.
        /// </summary>
        IButtonBar ButtonBar { get; }

        /// <summary>
        /// Gets the status field.
        /// </summary>
        IStatusField StatusField { get; }

        /// <summary>
        /// Gets the message field.
        /// </summary>
        IMessageField MessageField { get; }

        /// <summary>
        /// Gets the number input main field.
        /// </summary>
        INumberInput NumberInput { get; }

        /// <summary>
        /// Gets the driver block input main field.
        /// </summary>
        IDriverBlockInput DriverBlockInput { get; }

        /// <summary>
        /// Gets the login number input main field.
        /// </summary>
        ILoginNumberInput LoginNumberInput { get; }

        /// <summary>
        /// Gets the drive select main field.
        /// </summary>
        IDriveSelect DriveSelect { get; }

        /// <summary>
        /// Gets the list main field.
        /// </summary>
        IList List { get; }

        /// <summary>
        /// Gets the block drive wait main field.
        /// </summary>
        IBlockDriveWait BlockDriveWait { get; }

        /// <summary>
        /// Gets the special destination drive main field.
        /// </summary>
        ISpecialDestinationDrive SpecialDestinationDrive { get; }

        /// <summary>
        /// Gets the block drive main field.
        /// </summary>
        IBlockDrive BlockDrive { get; }

        /// <summary>
        /// Gets the main status main field.
        /// </summary>
        IStatusMainField MainStatus { get; }

        /// <summary>
        /// Gets the icon list main field.
        /// </summary>
        IMessageList IconList { get; }

        /// <summary>
        /// Gets the iqube radio main field.
        /// </summary>
        IIqubeRadio IqubeRadio { get; }

        /// <summary>
        /// Sets the active main field.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        void SetMainField(IMainField mainField);

        /// <summary>
        /// Runs the user interface.
        /// </summary>
        void Run();

        /// <summary>
        /// Stops running the user interface.
        /// </summary>
        void Stop();
    }
}