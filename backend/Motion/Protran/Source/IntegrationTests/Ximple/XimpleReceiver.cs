// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XimpleReceiver.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XimpleReceiver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IntegrationTests.Ximple
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Utils;

    /// <summary>
    /// Possible meaning for a cell.
    /// </summary>
    public enum CellMeaning
    {
        /// <summary>
        /// Cell having a time as value.
        /// </summary>
        CellTime,

        /// <summary>
        /// Cell having a date as value.
        /// </summary>
        CellDate,

        /// <summary>
        /// Cell having the remote pc status as value.
        /// </summary>
        CellRemotePc,

        /// <summary>
        /// Cell having the stop request status as value.
        /// </summary>
        CellStopRequestStatus,

        /// <summary>
        /// Cell having the approaching status as value.
        /// </summary>
        CellApproachingStatus,

        /// <summary>
        /// Cell with stop info of absolute time.
        /// </summary>
        CellStopInfoAbsoluteTime
    }

    /// <summary>
    /// Class that receives Ximple from MessageDispatcher and checks it against
    /// defined rules.
    /// </summary>
    public class XimpleReceiver
    {
        private readonly List<Expectation> expectations = new List<Expectation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="XimpleReceiver"/> class.
        /// </summary>
        public XimpleReceiver()
        {
            this.ErroneousXimples = new List<Ximple>();
            this.SkippableCellsList = new List<CellMeaning>();
            this.MissingExpectations = new List<Expectation>();
        }

        /// <summary>
        /// Gets the number of Ximple objects received.
        /// </summary>
        public int ReceiveCount { get; private set; }

        /// <summary>
        /// Gets the number of erroneous Ximple objects received.
        /// </summary>
        public int ErrorCount { get; private set; }

        /// <summary>
        /// Gets or sets the list of cells that can be skipped.
        /// </summary>
        public List<CellMeaning> SkippableCellsList { get; set; }

        /// <summary>
        /// Gets or sets MissingExpectations.
        /// </summary>
        public List<Expectation> MissingExpectations { get; set; }

        /// <summary>
        /// Gets or sets the list of Ximple that are wrong.
        /// </summary>
        public List<Ximple> ErroneousXimples { get; set; }

        /// <summary>
        /// Starts this receiver by registering it to the
        /// <see cref="MessageDispatcher"/>.
        /// </summary>
        public void Start()
        {
            MessageDispatcher.Instance.Subscribe<Ximple>(this.HandleXimple);
        }

        /// <summary>
        /// Stops this receiver by unregistering it from the
        /// <see cref="MessageDispatcher"/>.
        /// </summary>
        public void Stop()
        {
            MessageDispatcher.Instance.Unsubscribe<Ximple>(this.HandleXimple);

            if (this.expectations.Count <= 0)
            {
                return;
            }

            this.ErrorCount++;
            foreach (var expectation in this.expectations)
            {
                this.MissingExpectations.Add(expectation);
            }

            Console.Error.WriteLine("{0} expected Ximple object(s) did not arrive", this.expectations.Count);
            this.DumpCurrentExpectations();
            this.expectations.Clear();
        }

        /// <summary>
        /// Adds an expectation to this receiver.
        /// An expectation is valid exactly once and will be removed after
        /// being valid for one Ximple .
        /// </summary>
        /// <returns>
        /// the expectation to which rules can be added.
        /// </returns>
        public Expectation ExpectXimple()
        {
            var expect = new Expectation();
            this.expectations.Add(expect);
            return expect;
        }

        /// <summary>
        /// Print on the console the current status of the
        /// list containing the expectations.
        /// </summary>
        public void DumpCurrentExpectations()
        {
            // TODO:
            // add the dump for each expectation.
            Console.Error.WriteLine(
                "{0} erreonous ximples, {1} expectations not arrived",
                this.ErroneousXimples.Count,
                this.MissingExpectations.Count);

            foreach (var ximple in this.ErroneousXimples)
            {
                Console.WriteLine("Erronous ximple:");
                Console.WriteLine(ximple.ToXmlString());
                Console.WriteLine();
            }

            foreach (var expectation in this.MissingExpectations)
            {
                Console.WriteLine("Expectation not arrived is: {0} ", expectation);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Handles the XIMPLE just arrived.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event with the XIMPLE</param>
        protected virtual void HandleXimple(object sender, MessageEventArgs<Ximple> e)
        {
            Expectation removedExpectation = null;
            lock (this.expectations)
            {
                this.RemoveSkippableCells(e.Message);
                this.ReceiveCount++;

                if (e.Message.Version == Constants.Version2)
                {
                    Console.WriteLine("Received Ximple2 with {0} cells.", e.Message.Cells.Count);
                }
                else
                {
                    Console.WriteLine("Received unsupported Ximple version: {0}", e.Message.Version);
                    return;
                }

                var converted = e.Message.ConvertTo(Constants.Version2);
                foreach (var cell in converted.Cells)
                {
                    Console.WriteLine(
                        "Cell with L={0} T={1} C={2} R={3} V=\"{4}\"",
                        cell.LanguageNumber,
                        cell.TableNumber,
                        cell.ColumnNumber,
                        cell.RowNumber,
                        cell.Value);
                }

                foreach (var expectation in this.expectations)
                {
                    if (expectation.Verify(e.Message))
                    {
                        // found the expectation.
                        Console.WriteLine("Removed expectation is: {0} ", expectation);
                        removedExpectation = expectation;
                        break;
                    }
                }

                if (removedExpectation != null)
                {
                    this.expectations.Remove(removedExpectation);
                    Console.WriteLine("Received expected Ximple #{0}", this.ReceiveCount);
                    return;
                }

                this.ErrorCount++;
                this.ErroneousXimples.Add(e.Message);
                Console.Error.WriteLine("Unexpected Ximple: {0}", e.Message.ToXmlString());
            }
        }

        private void RemoveSkippableCells(Ximple ximple)
        {
            foreach (var cellMeaning in this.SkippableCellsList)
            {
                switch (cellMeaning)
                {
                    case CellMeaning.CellTime:
                        {
                            ximple.Cells.RemoveAll(c => c.TableNumber == 0 && c.ColumnNumber == 1 && c.RowNumber == 0);
                        }

                        break;

                    case CellMeaning.CellDate:
                        {
                            ximple.Cells.RemoveAll(c => c.TableNumber == 0 && c.ColumnNumber == 0 && c.RowNumber == 0);
                        }

                        break;

                    case CellMeaning.CellRemotePc:
                        {
                            ximple.Cells.RemoveAll(c => c.TableNumber == 0 && c.ColumnNumber == 2 && c.RowNumber == 0);
                        }

                        break;

                    case CellMeaning.CellStopRequestStatus:
                        {
                            ximple.Cells.RemoveAll(c => c.TableNumber == 0 && c.ColumnNumber == 3 && c.RowNumber == 0);
                        }

                        break;

                    case CellMeaning.CellApproachingStatus:
                        {
                            ximple.Cells.RemoveAll(c => c.TableNumber == 10 && c.ColumnNumber == 5 && c.RowNumber == 0);
                        }

                        break;

                    case CellMeaning.CellStopInfoAbsoluteTime:
                        {
                            ximple.Cells.RemoveAll(c => c.TableNumber == 12 && c.ColumnNumber == 2);
                            ximple.Cells.RemoveAll(c => c.TableNumber == 11 && c.ColumnNumber == 2 && c.RowNumber == 0);
                        }

                        break;
                }
            }
        }
    }
}