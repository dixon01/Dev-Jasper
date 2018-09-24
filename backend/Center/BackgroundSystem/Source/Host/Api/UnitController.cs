// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.BackgroundSystem.Host.Api
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel.Messages;

    using NLog;

    /// <summary>
    /// The configuration controller.
    /// </summary>
    [RoutePrefix("api/v1/unit")]
    public class UnitController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The GET operation returning the configuration for the specified unit.
        /// </summary>
        /// <param name="uid">
        /// The unique identifier for the unit.
        /// </param>
        /// <param name="hash">
        /// The hash currently known by the unit.
        /// </param>
        /// <param name="seq_num">
        /// The sequential number optionally provided by the unit.
        /// </param>
        /// <returns>
        /// The response to the request of the unit.
        /// The response will contain the <see cref="HttpStatusCode.NotFound"/> if the unit was not found (or any
        /// update command for it) was found.
        /// Otherwise, the unit will reply with status <see cref="HttpStatusCode.OK"/>. If the hash provided by the
        /// unit is the same as the current hash, then an empty content is sent; otherwise, the new content is sent as
        /// stream.
        /// </returns>
        [HttpGet]
        [Route("configuration")]
        public async Task<HttpResponseMessage> GetAsync(string uid, string hash = null, int? seq_num = null)
        {
            try
            {
                var unitService = DependencyResolver.Current.Get<IUnitChangeTrackingManager>();
                var query =
                    await
                    unitService.QueryAsync(
                        UnitQuery.Create()
                        .WithName(uid)
                        .IncludeUpdateGroup(UpdateGroupFilter.Create().IncludeUpdateParts())
                        .IncludeUpdateCommands());
                var unit = query.FirstOrDefault();
                if (unit == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                var currentConfigurationHash = await this.GetConfigurationHashAsync(uid);
                if (string.Equals(hash, currentConfigurationHash, StringComparison.OrdinalIgnoreCase))
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }

                if (unit.UpdateGroup == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                var updateCommand = unit.UpdateCommands.OrderByDescending(p => p.Id).FirstOrDefault();
                if (updateCommand == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                await updateCommand.LoadNavigationPropertiesAsync();
                await updateCommand.LoadXmlPropertiesAsync();
                var writable = updateCommand.ToChangeTrackingModel();
                writable.WasTransferred = true;
                var updateService = DependencyResolver.Current.Get<IUpdateCommandChangeTrackingManager>();
                await updateService.CommitAndVerifyAsync(writable);

                var stream = await this.GetUpdateCommandContent(updateCommand);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while serving unit configuration request");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        private async Task<Stream> GetUpdateCommandContent(UpdateCommandReadableModel updateCommand)
        {
            var updateFolder = (FolderUpdate)updateCommand.Command.Deserialize();
            var fileUpdate = updateFolder.Items.OfType<FileUpdate>().First();
            var resourceService = DependencyResolver.Current.Get<IContentResourceService>();
            var resource = resourceService.GetResource(fileUpdate.Hash, HashAlgorithmTypes.xxHash64);
            using (var stream = resource.OpenRead())
            {
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
        }

        private async Task<string> GetConfigurationHashAsync(string unitId)
        {
            var unitService = DependencyResolver.Current.Get<IUnitDataService>();

            // TODO: cache the value or use the unit and change tracking to easily find the current configuration hash
            // For the moment, returning empty, meaning that the configuration will always be sent to the unit
            return string.Empty;
        }
    }
}