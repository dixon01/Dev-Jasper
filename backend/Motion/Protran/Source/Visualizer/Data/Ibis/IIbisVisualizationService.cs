// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIbisVisualizationService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Protran.Visualizer.Data.Ibis
{
    using System;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.Visualizer.Controls.Ibis;

    /// <summary>
    /// Service used for IBIS visualization.
    /// </summary>
    public interface IIbisVisualizationService
    {
        /// <summary>
        /// Event that is fired every time a new Ximple is created by the underlying channel.
        /// </summary>
        event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Event that is fired if a handler starts handling a telegram.
        /// </summary>
        event EventHandler<TelegramHandlerEventArgs> HandlingTelegram;

        /// <summary>
        /// Event that is fired if a handler has finished handling a telegram.
        /// </summary>
        event EventHandler<TelegramHandlerEventArgs> HandledTelegram;

        /// <summary>
        /// Event that is fired if a handler emits a new Ximple.
        /// Use this event only to listen to the Ximple object created by
        /// a handler, otherwise use <see cref="XimpleCreated"/>.
        /// </summary>
        event EventHandler<TelegramHandlerXimpleEventArgs> HandlerCreatedXimple;

        /// <summary>
        /// Event that is fired every time a new telegram is being enqueued.
        /// </summary>
        event EventHandler TelegramParsing;

        /// <summary>
        /// Event that is fired when a telegram was successfully parsed,
        /// i.e. a <see cref="Telegram"/> was created from the binary data.
        /// </summary>
        event EventHandler<TelegramParsedEventArgs> TelegramParsed;

        /// <summary>
        /// Event that is fired when a telegram successfully passed transformation.
        /// </summary>
        event EventHandler<TransformationChainEventArgs> TelegramTransformed;

        /// <summary>
        /// Gets the generic view dictionary.
        /// </summary>
        Dictionary Dictionary { get; }

        /// <summary>
        /// Gets the IBIS config.
        /// </summary>
        IbisConfig Config { get; }

        /// <summary>
        /// Enqueue a new telegram into the channel of this service.
        /// </summary>
        /// <param name="telegram">
        /// The telegram data.
        /// </param>
        /// <param name="ignoreUnknown">
        /// If set to true, unknown telegrams (where no parser is found) will not be enqueued
        /// and false will be returned.
        /// </param>
        /// <returns>
        /// true if the telegram was enqueued successfully.
        /// </returns>
        bool EnqueueTelegram(byte[] telegram, bool ignoreUnknown);

        /// <summary>
        /// Enables or disabled the serial port.
        /// </summary>
        /// <param name="enableIbisBus">
        /// The enable ibis bus.
        /// </param>
        void EnableSerialPort(bool enableIbisBus);

        /// <summary>
        /// Send the telegram control into the channel of this service
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        void SetControl(TelegramControl controller);
    }
}