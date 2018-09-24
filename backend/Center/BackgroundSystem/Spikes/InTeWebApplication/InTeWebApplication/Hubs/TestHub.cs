// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestHub.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestHub type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.InTeWebApplication.Hubs
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Gorba.Center.BackgroundSystem.Spikes.InTeWebApplication.Models;

    using Microsoft.AspNet.SignalR;

    /// <summary>
    /// The test hub.
    /// </summary>
    public class TestHub : Hub<ITestHubClient>, ITestHubClient
    {
        /// <summary>
        /// Updates the result on clients.
        /// </summary>
        /// <param name="testResult">
        /// The test result.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task UpdateResultAsync(TestResult testResult)
        {
            await Clients.All.UpdateResultAsync(testResult);
        }

        /// <summary>
        /// Starts a test.
        /// </summary>
        /// <param name="name">The name of the test.</param>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        public async Task StartTestAsync(string name)
        {
            // Start test here
            var runId = Guid.NewGuid();
            var testResult = new TestResult(name, runId);
            await this.UpdateResultAsync(testResult);
        }

        private static string RenderPartialToString(string view, object model, ControllerContext context)
        {
            var viewData = new ViewDataDictionary { Model = model };
            var tempData = new TempDataDictionary();
            using (var stringWriter = new StringWriter())
            {
                var result = ViewEngines.Engines.FindPartialView(context, view);
                var viewContext = new ViewContext(context, result.View, viewData, tempData, stringWriter);
                result.View.Render(viewContext, stringWriter);
                return stringWriter.GetStringBuilder().ToString();
            }
        }
    }
}