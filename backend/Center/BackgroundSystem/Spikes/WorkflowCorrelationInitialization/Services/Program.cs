// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Services
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activities;

    /// <summary>
    /// Class containing the main logic.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            var dependentWorkflowServiceHost = new WorkflowServiceHost(
                new DependentService(), new Uri("http://localhost:9998/WorkflowCorrelationInitialization"));
            dependentWorkflowServiceHost.Open();
            var mainWorkflowServiceHost = new WorkflowServiceHost(
                new MainService(), new Uri("http://localhost:9999/WorkflowCorrelationInitializationMain"));
            mainWorkflowServiceHost.Open();
            var mainService = ChannelFactory<IMainService>.CreateChannel(
                new BasicHttpBinding(),
                new EndpointAddress("http://localhost:9999/WorkflowCorrelationInitializationMain"));
            mainService.Start();
            Console.WriteLine("Type any key to exit");
            Console.ReadKey();
        }
    }
}