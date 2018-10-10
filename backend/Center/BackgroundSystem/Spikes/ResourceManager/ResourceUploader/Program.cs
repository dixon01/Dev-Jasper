// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ResourceManager.ResourceUploader
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.ServiceModel.Security;
    using Gorba.Common.Update.ServiceModel.Resources;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Usage ResourceUploader.exe username hashedPassword resourcePath [server]");
                Console.ResetColor();
                return;
            }

            if (string.IsNullOrEmpty(args[2]) || !File.Exists(args[2]))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Specified resource path is not valid");
                Console.WriteLine("Usage ResourceUploader.exe username hashedPassword resourcePath [server]");
                Console.ResetColor();
                return;
            }

            try
            {
                var backgroundSystemConfiguration = BackgroundSystemConfigurationProvider.Current.GetConfiguration();
                ChannelScopeFactoryUtility<IResourceService>.ConfigureAsFunctionalService(
                   backgroundSystemConfiguration.FunctionalServices, "Resources");
                var userCredentials = new UserCredentials(args[0], args[1]);
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                Console.WriteLine("Evaluating hash");
                var hash = ResourceHash.Create(args[2]);
                Console.WriteLine("Hash evaluated in {0}", stopwatch.Elapsed);
                stopwatch.Restart();
                var name = Path.GetFileName(args[2]);
                var resource = new Resource { OriginalFilename = name, Hash = hash };
                using (var channelScope = ChannelScopeFactory<IResourceService>.Current.Create(userCredentials))
                {
                    using (var stream = new FileStream(args[2], FileMode.Open))
                    {
                        var streamedResource = new ResourceUploadRequest { Content = stream, Resource = resource };
                        channelScope.Channel.UploadAsync(streamedResource).Wait();
                    }
                }

                stopwatch.Stop();
                Console.WriteLine("Resource uploaded in {0}", stopwatch.Elapsed);

                stopwatch.Restart();
                using (var channelScope = ChannelScopeFactory<IResourceService>.Current.Create(userCredentials))
                {
                    resource = channelScope.Channel.GetAsync(hash).Result;
                }

                Console.WriteLine("Resource info retrieved in {0}", stopwatch.Elapsed);

                stopwatch.Restart();
                using (var channelScope = ChannelScopeFactory<IResourceService>.Current.Create(userCredentials))
                {
                    var resourceDownloadRequest = new ResourceDownloadRequest
                                                      {
                                                          Hash = hash
                                                      };
                    using (var stream = channelScope.Channel.DownloadAsync(resourceDownloadRequest).Result.Content)
                    {
                    }
                }

                Console.WriteLine("Resource downloaded in {0}", stopwatch.Elapsed);
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception);
                Console.ResetColor();
            }
        }
    }
}