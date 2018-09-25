// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportContentResource.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Writes a new <see cref="ContentResource" /> object to the pipeline.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.PowerShell.Resources
{
    using System;
    using System.IO;
    using System.Management.Automation;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.Utils;

    /// <summary>
    /// Uploads a <see cref="ContentResource"/> to the system.
    /// </summary>
    [Cmdlet(VerbsData.Import, "ContentResource")]
    public class ImportContentResource : ServiceCmdletBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportContentResource"/> class.
        /// </summary>
        public ImportContentResource()
        {
            this.MimeType = "application/octet-stream";
        }

        /// <summary>
        /// Gets or sets the mime type.
        /// </summary>
        [Parameter(Mandatory = true)]
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
            using (var channel = this.CreateFunctionalChannelScope<IContentResourceService>("ContentResources"))
            {
                var resourceHash = ContentResourceHash.Create(this.Path, HashAlgorithmTypes.xxHash64);
                using (var fileStream = File.OpenRead(this.Path))
                {
                    var resourceRequest = new ContentResourceUploadRequest
                                              {
                                                  Content = fileStream,
                                                  Resource =
                                                      new ContentResource
                                                          {
                                                              Hash = resourceHash,
                                                              Length = fileStream.Length,
                                                              MimeType = this.MimeType,
                                                              OriginalFilename = fileInfo.Name,
                                                              HashAlgorithmType = HashAlgorithmTypes.xxHash64
                                                          }
                                              };
                    var uploadAsync = channel.Channel.UploadAsync(resourceRequest).Result;
                    this.WriteObject(uploadAsync.Resource);
                }
            }
        }
    }
}