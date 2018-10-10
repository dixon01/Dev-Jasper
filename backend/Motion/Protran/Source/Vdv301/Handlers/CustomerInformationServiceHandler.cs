// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomerInformationServiceHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CustomerInformationServiceHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Vdv301.Handlers
{
    using System;

    using Gorba.Common.Protocols.Vdv301.Messages;
    using Gorba.Common.Protocols.Ximple;

    /// <summary>
    /// Manual implementation of the special functionality needed by <see cref="CustomerInformationServiceHandler"/>:
    /// - keeping track of the current stop index and the current stop list
    /// - updating the stop list depending on the current stop index (including clearing unused cells)
    /// </summary>
    public partial class CustomerInformationServiceHandler
    {
        // R# has some issues with partial classes, let's disable some warnings
        // ReSharper disable RedundantAssignment
        // ReSharper disable UnusedParameter.Local
        // ReSharper disable ParameterTypeCanBeEnumerable.Local
        private partial class DataContext
        {
            public int CurrentStopIndex { get; private set; }

            /// <summary>
            /// Gets or sets the last stops "displayed" (i.e. only current and next stops)
            /// </summary>
            public StopInformationStructure[] LastStopList { get; set; }

            /// <summary>
            /// Gets or sets the last connection list
            /// </summary>
            public ConnectionStructure[] LastConnectionList { get; set; }

            public void SetCurrentStopIndex(IBISIPint value)
            {
                if (value != null && !value.ErrorCodeSpecified)
                {
                    this.CurrentStopIndex = value.Value;
                }
            }
        }

        private partial class GetAllDataHandler
        {
            partial void PrepareCurrentStopIndex(IBISIPint item, int row, DataContext dataContext)
            {
                dataContext.SetCurrentStopIndex(item);
            }
        }

        private partial class GetCurrentStopIndexHandler
        {
            partial void PrepareCurrentStopIndex(IBISIPint item, int row, DataContext dataContext)
            {
                dataContext.SetCurrentStopIndex(item);
            }
        }

        private partial class GetTripDataHandler
        {
            partial void PrepareCurrentStopIndex(IBISIPint item, int row, DataContext dataContext)
            {
                dataContext.SetCurrentStopIndex(item);
            }
        }

        private partial class StopSequenceStructureHandler
        {
            partial void HandleStopPoint(
                StopInformationStructure[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled)
            {
                handled = true;
                var maxRow = -1;
                if (item != null)
                {
                    foreach (var info in item)
                    {
                        if (info.StopIndex.ErrorCodeSpecified || info.StopIndex.Value < dataContext.CurrentStopIndex)
                        {
                            continue;
                        }

                        var stopRow = info.StopIndex.Value - dataContext.CurrentStopIndex;
                        maxRow = Math.Max(maxRow, stopRow);
                        this.handlerStopPoint.HandleData(info, ximple, stopRow, dataContext);
                    }
                }

                if (dataContext.LastStopList != null)
                {
                    // clear all additional rows:
                    // 1. we create the same cells as last time (i.e. with the saved data from the context)
                    // 2. we change the cell value to an empty string
                    // 3. we copy all cells from the dummy structure to the real Ximple object
                    for (int stopRow = maxRow + 1; stopRow < dataContext.LastStopList.Length; stopRow++)
                    {
                        var dummyXimple = new Ximple();
                        var info = dataContext.LastStopList[stopRow];
                        this.handlerStopPoint.HandleData(info, dummyXimple, stopRow, dataContext);
                        foreach (var cell in dummyXimple.Cells)
                        {
                            cell.Value = string.Empty;
                        }

                        ximple.Cells.AddRange(dummyXimple.Cells);
                    }
                }

                dataContext.LastStopList = item;
            }
        }

        private partial class StopInformationStructureHandler
        {
            // TODO: do we need to do something with the stop announcement?
            partial void HandleConnection(
                ConnectionStructure[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled)
            {
                handled = true;

                // TODO: we might want to enable checking for the next stop with connections
                // (instead of just the next stop - that might not have any connections)
                if (row != 0)
                {
                    // we are only interested in connections from the current stop (row == 0)
                    return;
                }

                var nextRow = 0;
                if (item != null)
                {
                    for (int connectionRow = 0; connectionRow < item.Length; connectionRow++)
                    {
                        var connection = item[connectionRow];

                        this.handlerConnection.HandleData(connection, ximple, connectionRow, dataContext);
                    }

                    nextRow = item.Length;
                }

                if (dataContext.LastConnectionList != null)
                {
                    // clear all additional rows:
                    // 1. we create the same cells as last time (i.e. with the saved data from the context)
                    // 2. we change the cell value to an empty string
                    // 3. we copy all cells from the dummy structure to the real Ximple object
                    for (int connectionRow = nextRow;
                         connectionRow < dataContext.LastConnectionList.Length;
                         connectionRow++)
                    {
                        var dummyXimple = new Ximple();
                        var connection = dataContext.LastConnectionList[connectionRow];
                        this.handlerConnection.HandleData(connection, dummyXimple, connectionRow, dataContext);
                        foreach (var cell in dummyXimple.Cells)
                        {
                            cell.Value = string.Empty;
                        }

                        ximple.Cells.AddRange(dummyXimple.Cells);
                    }
                }

                dataContext.LastConnectionList = item;
            }

            partial void HandleDisplayContent(
                DisplayContentStructure[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled)
            {
                // we are only interested in the display content of the current stop (row == 0)
                handled = row != 0;
            }
        }

        // ReSharper restore ParameterTypeCanBeEnumerable.Local
        // ReSharper restore UnusedParameter.Local
        // ReSharper restore RedundantAssignment
    }
}
