﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomerInformationServiceHandler.bugfix.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CustomerInformationServiceHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Vdv301.Handlers
{
    using Gorba.Common.Protocols.Vdv301.Messages;
    using Gorba.Common.Protocols.Ximple;

    /// <summary>
    /// Workaround partial class to support missing <see cref="StopSequenceStructure"/>.
    /// </summary>
    public partial class CustomerInformationServiceHandler
    {
        private partial class TripInformationStructureHandler
        {
            /// <summary>
            /// Method to work around issue that <see cref="StopSequenceStructure"/>
            /// is not automatically generated by XSD.exe.
            /// </summary>
            /// <param name="stopSequence">the stop sequence</param>
            /// <param name="row">the row number</param>
            /// <param name="dataContext">the data context</param>
            /// <param name="updated">a flag indicating if the stop sequence was updated</param>
            private void PrepareStopSequence(
                StopInformationStructure[] stopSequence, int row, DataContext dataContext, ref bool updated)
            {
                this.PrepareStopSequence(new StopSequenceStructure(stopSequence), row, dataContext, ref updated);
            }

            /// <summary>
            /// Method to work around issue that <see cref="StopSequenceStructure"/>
            /// is not automatically generated by XSD.exe.
            /// </summary>
            /// <param name="stopSequence">the stop sequence</param>
            /// <param name="ximple">the ximple object</param>
            /// <param name="row">the row number</param>
            /// <param name="dataContext">the data context</param>
            /// <param name="handled">a flag indicating if the stop sequence was handled</param>
            private void HandleStopSequence(
                StopInformationStructure[] stopSequence,
                Ximple ximple,
                int row,
                DataContext dataContext,
                ref bool handled)
            {
                this.HandleStopSequence(new StopSequenceStructure(stopSequence), ximple, row, dataContext, ref handled);
            }
        }

        private partial class StopSequenceStructureHandler
        {
            /// <summary>
            /// Method to work around issue that <see cref="StopSequenceStructure"/>
            /// is not automatically generated by XSD.exe.
            /// </summary>
            /// <param name="stopSequence">the stop sequence</param>
            /// <param name="row">the row number</param>
            /// <param name="dataContext">the data context</param>
            public void PrepareData(StopInformationStructure[] stopSequence, int row, DataContext dataContext)
            {
                this.PrepareData(new StopSequenceStructure(stopSequence), row, dataContext);
            }

            /// <summary>
            /// Method to work around issue that <see cref="StopSequenceStructure"/>
            /// is not automatically generated by XSD.exe.
            /// </summary>
            /// <param name="stopSequence">the stop sequence</param>
            /// <param name="ximple">the Ximple object</param>
            /// <param name="row">the row number</param>
            /// <param name="dataContext">the data context</param>
            public void HandleData(
                StopInformationStructure[] stopSequence, Ximple ximple, int row, DataContext dataContext)
            {
                this.HandleData(new StopSequenceStructure(stopSequence), ximple, row, dataContext);
            }
        }

        private class StopSequenceStructure
        {
            public StopSequenceStructure(StopInformationStructure[] stopSequence)
            {
                this.StopPoint = stopSequence;
            }

            public StopInformationStructure[] StopPoint { get; private set; }
        }
    }
}
