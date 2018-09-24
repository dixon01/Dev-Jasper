// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtocolStack.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Core
{
    using System;

    /// <summary>
    /// Handles the stack of the layers to make it easier to implement a protocol.
    /// </summary>
    public class ProtocolStack
    {
        private ProtocolLayer highestLayer;

        private ProtocolLayer lowestLayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolStack"/> class.
        /// </summary>
        public ProtocolStack()
        {
            this.highestLayer = null;
            this.lowestLayer = null;
        }

        /// <summary>
        /// Adds a protocol layer in the layer staks at the specified placement in the stack.
        /// </summary>
        /// <param name="layer">
        /// The layer.
        /// </param>
        /// <param name="placement">
        /// The placement.
        /// </param>
        /// <param name="existingLayer">
        /// The existing layer.
        /// </param>
        /// <exception cref="Exception">
        /// If <see cref="placement"/> equals Placement.Top then the <see cref="existingLayer"/> shouldn't be defined. 
        /// </exception>
        /// <exception cref="Exception">
        /// If <see cref="placement"/> equals Placement.Above then the <see cref="existingLayer"/> should be defined. 
        /// </exception>
        /// <exception cref="Exception">
        /// If <see cref="placement"/> equals Placement.Below then the <see cref="existingLayer"/> should be defined.  
        /// </exception>
        /// <exception cref="Exception">
        /// If the <see cref="highestLayer"/> is null then the <see cref="existingLayer"/> shouldn't be defeined.
        /// </exception>
        public void AddLayer(
            ProtocolLayer layer, Placement placement = Placement.Top, ProtocolLayer existingLayer = null)
        {
            // Start with a clean slate. Initialize the upper and lower protocol
            // layers to NULL. The pointers will be suitably initialized after insertion.
            layer.LowerLayer = null;
            layer.UpperLayer = null;

            // Check if some other layer is already present in the protocol stack. 
            // The placement processing applies only if this is not the first layer 
            // being added to the stack.
            if (this.highestLayer != null)
            {
                // This is not the first layer
                switch (placement)
                {
                    case Placement.Top: // Add the layer at the top
                        if (existingLayer != null)
                        {
                            throw new Exception("existingLayer must be null in that case !");
                        }

                        this.highestLayer.UpperLayer = layer;
                        layer.LowerLayer = this.highestLayer;
                        this.highestLayer = layer;
                        break;

                    case Placement.Above: // Place the layer above the existing layer
                        if (existingLayer == null)
                        {
                            throw new Exception("existingLayer must be not null to place the layer above it !");
                        }

                        // Linking up the new layer above the existing layer
                        ProtocolLayer previousUpperLayer = existingLayer.UpperLayer;
                        layer.UpperLayer = previousUpperLayer;
                        layer.LowerLayer = existingLayer;
                        existingLayer.UpperLayer = layer;

                        // Check if the existing layer was the highest layer
                        if (existingLayer == this.highestLayer)
                        {
                            // If it was, make the new layer the highest layer
                            this.highestLayer = layer;
                        }
                        else
                        {
                            // Change the pointer of the existing layer's upper layer
                            // to point to the newly inserted layer.
                            previousUpperLayer.LowerLayer = layer;
                        }

                            // else
                        break;

                    case Placement.Below: // Place the layer below the existing layer
                        if (existingLayer == null)
                        {
                            throw new Exception("existingLayer must be not null to place the layer below it !");
                        }

                        // Linking up the new layer below the existing layer
                        ProtocolLayer previousLowerLayer = existingLayer.LowerLayer;
                        layer.UpperLayer = existingLayer;
                        layer.LowerLayer = previousLowerLayer;
                        existingLayer.LowerLayer = layer;

                        // Check if the existing layer was the lowest layer
                        if (existingLayer == this.lowestLayer)
                        {
                            // If it was, make the new layer the lowest layer
                            this.lowestLayer = layer;
                        }
                        else
                        {
                            // Change the pointer of the existing layer's lower layer
                            // to point to the newly inserted layer.
                            previousLowerLayer.UpperLayer = layer;
                        }

                        break;
                }
            }
            else
            {
                // The highest layer is NULL
                // This means that this is the first layer in the protocol stack.
                if (existingLayer != null)
                {
                    throw new Exception("existingLayer must be null in that case !");
                }

                    // if
                this.highestLayer = layer;
                this.lowestLayer = layer;
            }
        }

        /// <summary>
        /// Transfer the given packet to the lowest protocol layer.
        /// </summary>
        /// <param name="packet">
        /// The packet to transfer
        /// </param>
        public void HandleReceive(ref ProtocolPacket packet)
        {
            if (this.lowestLayer != null)
            {
                this.lowestLayer.HandleReceive(ref packet);
            }
        }

        /// <summary>
        /// Transfer the given packet to the highest protocol layer.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        public void HandleTransmit(ref ProtocolPacket packet)
        {
            if (this.highestLayer != null)
            {
                this.highestLayer.Transmit(ref packet);
            }
        }

        /// <summary>
        /// The remove layer.
        /// </summary>
        /// <param name="layer">
        /// The layer.
        /// </param>
        public void RemoveLayer(ProtocolLayer layer)
        {
            // Check if the layer to be removed is the highest layer.
            if (layer == this.highestLayer)
            {
                // Yes it is, so set the removed layer's lower layer as the highest layer
                // in the protocol stack.
                this.highestLayer = layer.LowerLayer;

                // If this was not the only layer in the stack, set the
                // upper layer of this layer as NULL.
                if (this.highestLayer != null)
                {
                    this.highestLayer.UpperLayer = null;
                }

                    // if
            }
            else
            {
                // Not the highest layer 
                // Stitch the upper layer to lower layer link after the layer is removed.
                layer.UpperLayer.LowerLayer = layer.LowerLayer;
            }

                // else

            // Check if the layer to be removed is the lowest layer.
            if (layer == this.lowestLayer)
            {
                // Yes it is, so set the removed layer's upper layer as the lowest layer
                // in the protocol stack.
                this.lowestLayer = layer.UpperLayer;

                // If this was not the only layer in the stack, set the
                // lower layer of this layer as NULL.
                if (this.lowestLayer != null)
                {
                    this.lowestLayer.LowerLayer = null;
                }

                    // if
            }
            else
            {
                // Stitch the lower layer to upper layer link after the layer is removed.
                layer.LowerLayer.UpperLayer = layer.UpperLayer;
            }

                // else

            // Set the upper and lower layer pointers of the removed layer as NULL.
            // This is a safety measure.
            layer.LowerLayer = null;
            layer.UpperLayer = null;
        }
    }
}