// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportResource.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImportResource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.PowerShell.Resources
{
    using System;
    using System.IO;
    using System.Management.Automation;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel.Resources;

    /// <summary>
    /// Writes a new <see cref="Resource"/> object to the pipeline.
    /// </summary>
    [Cmdlet(VerbsData.Import, "Resource")]
    public sealed class ImportResource : ServiceCmdletBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportResource"/> class.
        /// </summary>
        public ImportResource()
        {
            this.MimeType = "application/octet-stream";
        }

        /// <summary>
        /// Gets or sets the mime type.
        /// </summary>
        [Parameter]
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the path to the resource to upload.
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Path { get; set; }

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            if (!File.Exists(this.Path))
            {
                throw new Exception("Path '" + this.Path + "' not found");
            }

            var fileInfo = new FileInfo(this.Path);
            using (var channel = this.CreateFunctionalChannelScope<IResourceService>("Resources"))
            {
                var resourceHash = ResourceHash.Create(this.Path);
                using (var fileStream = File.OpenRead(this.Path))
                {
                    var resourceRequest = new ResourceUploadRequest
                                              {
                                                  Content = fileStream,
                                                  Resource =
                                                      new Resource
                                                          {
                                                              Hash = resourceHash,
                                                              Length = fileStream.Length,
                                                              MimeType = this.MimeType,
                                                              OriginalFilename = fileInfo.Name
                                                          }
                                              };
                    var uploadAsync = channel.Channel.UploadAsync(resourceRequest).Result;
                    this.WriteObject(uploadAsync.Resource);
                }
            }
        }
    }
}