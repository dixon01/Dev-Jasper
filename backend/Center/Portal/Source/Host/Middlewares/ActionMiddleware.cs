// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionMiddleware.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ActionMiddleware type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Middlewares
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Portal.Host.Settings;
    using Gorba.Common.Utility.Files;

    using Microsoft.Owin;

    using NLog;

    using RazorEngine.Templating;

    /// <summary>
    /// Adds handling of actions to the <see cref="Owin"/> pipeline.
    /// An action is defined as a file in AppData/Views with html extension.
    /// </summary>
    public class ActionMiddleware : OwinMiddleware
    {
        private const string Extension = ".html";

        private const string RazorExtension = ".cshtml";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionMiddleware"/> class.
        /// </summary>
        /// <param name="next">
        /// The next.
        /// </param>
        /// <param name="appDataDirectoryInfo">
        /// The app data directory info.
        /// </param>
        public ActionMiddleware(OwinMiddleware next, IDirectoryInfo appDataDirectoryInfo)
            : base(next)
        {
            this.AppDataDirectoryInfo = appDataDirectoryInfo;
        }

        /// <summary>
        /// Gets the app data directory.
        /// </summary>
        public IDirectoryInfo AppDataDirectoryInfo { get; private set; }

        /// <summary>
        /// Process an individual request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        public override async Task Invoke(IOwinContext context)
        {
            var requestContext = context.GetRequestContext();
            Logger.Trace("Invoking ActionMiddleware for request {0}", requestContext.Id);
            if (!context.Request.Path.HasValue)
            {
                Logger.Trace("Action not valid. The request doesn't have a path");
                HandleInvalidRequest(context.Response);
                return;
            }

            ActionInfo actionInfo;
            if (this.IsActionValid(context.Request.Path.Value, out actionInfo))
            {
                Logger.Trace("Handling a valid action '{0}'", actionInfo);
                await this.HandleActionAsync(context, actionInfo);
                return;
            }

            await this.Next.Invoke(context);
        }

        private static void HandleInvalidRequest(IOwinResponse response, string path = "{empty}")
        {
            Logger.Warn("The requested path '{0}' can't be served", path);
            response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        private static bool TryGetApplicationUrl(string application, out CenterApplication centerApplication)
        {
            DataScope dataScope;
            if (Enum.TryParse(application, out dataScope))
            {
                switch (dataScope)
                {
                    case DataScope.AccessControl:
                    case DataScope.Tenant:
                    case DataScope.User:
                    case DataScope.ProductType:
                    case DataScope.Unit:
                    case DataScope.Resource:
                    case DataScope.Update:
                    case DataScope.Software:
                    case DataScope.UnitConfiguration:
                    case DataScope.MediaConfiguration:
                    case DataScope.Meta:
                        break;
                    case DataScope.CenterAdmin:
                        centerApplication = new CenterApplication("admin", "Admin", "Admin");
                        return true;
                    case DataScope.CenterDiag:
                        centerApplication = new CenterApplication("diag", "Diag", "Diag");
                        return true;
                    case DataScope.CenterMedia:
                        centerApplication = new CenterApplication("media", "Media", "Media");
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            centerApplication = null;
            return false;
        }

        private static string RunTemplate(string content, PageModel pageModel)
        {
            Logger.Trace("Compiling razor content");
            return RazorEngine.Engine.Razor.RunCompile(
                content,
                "key",
                typeof(PageModel),
                pageModel);
        }

        private static PageModel GetPageModel(IOwinContext context)
        {
            var requestContext = context.GetRequestContext();
            var pageModel = new PageModel();
            if (!requestContext.Session.ContainsKey(SessionMiddleware.ApplicationsKey))
            {
                Logger.Trace("Session doesn't contain The Applications key.");
                return pageModel;
            }

            var applicationsString = requestContext.Session[SessionMiddleware.ApplicationsKey];
            if (string.IsNullOrEmpty(applicationsString))
            {
                Logger.Trace("The Applications string is null or empty.");
                return pageModel;
            }

            var applicationValues = applicationsString.Split(',');
            foreach (var application in applicationValues)
            {
                CenterApplication centerApplication;
                if (TryGetApplicationUrl(application, out centerApplication))
                {
                    pageModel.CenterApplications.Add(centerApplication);
                }
            }

            var settings = PortalSettingsProvider.Current.GetSettings();
            pageModel.ClickOnceUseBeta = settings.ClickOnceUseBeta;
            Logger.Trace("Found {0} application(s)", pageModel.CenterApplications.Count);
            return pageModel;
        }

        private bool IsActionValid(string path, out ActionInfo actionInfo)
        {
            var actionName = path.Replace("/", string.Empty);
            IFileInfo fileInfo;
            if (actionName == string.Empty)
            {
                actionName = "Index";
            }

            if (this.TryGetRazorFileInfo(actionName, out fileInfo))
            {
                actionInfo = new ActionInfo(actionName, fileInfo, true);
                return true;
            }

            if (this.TryGetFileInfo(actionName, out fileInfo))
            {
                actionInfo = new ActionInfo(actionName, fileInfo, false);
                return true;
            }

            Logger.Warn("Couln't find file for action '{0}'", actionName);
            actionInfo = null;
            return false;
        }

        private bool TryGetRazorFileInfo(string actionName, out IFileInfo fileInfo)
        {
            Logger.Trace("Trying to get razor file for action '{0}'", actionName);
            var fileName = string.Concat(actionName, RazorExtension);
            var fullPath = Path.Combine(this.AppDataDirectoryInfo.FullName, "Views", "Actions", fileName);
            return this.AppDataDirectoryInfo.FileSystem.TryGetFile(fullPath, out fileInfo);
        }

        private bool TryGetFileInfo(string actionName, out IFileInfo fileInfo)
        {
            Logger.Trace("Trying to get file for action '{0}'", actionName);
            var fileName = string.Concat(actionName, Extension);
            var fullPath = Path.Combine(this.AppDataDirectoryInfo.FullName, "Views", "Actions", fileName);
            return this.AppDataDirectoryInfo.FileSystem.TryGetFile(fullPath, out fileInfo);
        }

        private async Task HandleActionAsync(IOwinContext context, ActionInfo actionInfo)
        {
            try
            {
                if (actionInfo.IsRazorTemplate)
                {
                    Logger.Trace("Processing razor file '{0}'", actionInfo.FileInfo.FullName);
                    using (var stream = actionInfo.FileInfo.OpenRead())
                    {
                        using (var streamReader = new StreamReader(stream))
                        {
                            var content = await streamReader.ReadToEndAsync();
                            var pageModel = GetPageModel(context);
                            content = RunTemplate(content, pageModel);
                            var authenticationError = !context.IsAuthenticated() && actionInfo.Name == "Login"
                                                      && !string.IsNullOrEmpty(context.Request.Query["Error"]);
                            content =
                                await
                                this.ReplacePartialsAsync(
                                    context.IsAuthenticated(),
                                    content,
                                    authenticationError,
                                    context.Request.Query["Details"],
                                    context.Request.Query["Error"]);
                            await context.Response.WriteAsync(content);
                            return;
                        }
                    }
                }

                Logger.Trace("Processing simple file '{0}'", actionInfo.FileInfo.FileSystem);
                using (var stream = actionInfo.FileInfo.OpenRead())
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        var content = await streamReader.ReadToEndAsync();
                        var authenticationError = !context.IsAuthenticated() && actionInfo.Name == "Login"
                                                  && !string.IsNullOrEmpty(context.Request.Query["Error"]);
                        content =
                            await
                            this.ReplacePartialsAsync(
                                context.IsAuthenticated(),
                                content,
                                authenticationError,
                                context.Request.Query["Details"],
                                context.Request.Query["Error"]);
                        await context.Response.WriteAsync(content);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async Task<string> ReplacePartialsAsync(
            bool isAuthenticated,
            string content,
            bool authenticationError,
            string details,
            string errorCode)
        {
            content = string.IsNullOrEmpty(details)
                          ? content.Replace("{Details}", string.Empty)
                          : content.Replace("{Details}", "<div>" + details + "</div>");

            var replacement = isAuthenticated ? "_Logout.cshtml" : "_Login.cshtml";
            IFileInfo fileInfo;
            var fullPath = Path.Combine(this.AppDataDirectoryInfo.FullName, "Views", "Shared", replacement);
            if (!this.AppDataDirectoryInfo.FileSystem.TryGetFile(fullPath, out fileInfo))
            {
                Logger.Debug("Partial login file not found");
                return content;
            }

            using (var stream = fileInfo.OpenRead())
            {
                using (var streamReader = new StreamReader(stream))
                {
                    var replace = await streamReader.ReadToEndAsync();
                    content = content.Replace("{Login}", replace);
                }
            }

            fullPath = Path.Combine(this.AppDataDirectoryInfo.FullName, "Views", "Shared", "_Meta.cshtml");
            if (!this.AppDataDirectoryInfo.FileSystem.TryGetFile(fullPath, out fileInfo))
            {
                Logger.Debug("Partial meta file not found");
                return content;
            }

            using (var stream = fileInfo.OpenRead())
            {
                using (var streamReader = new StreamReader(stream))
                {
                    var replace = await streamReader.ReadToEndAsync();
                    content = content.Replace("{Meta}", replace);
                }
            }

            var authenticationErrorContent = string.Empty;

            if (authenticationError)
            {
                authenticationErrorContent = await this.GetAuthenticationErrorContentAsync(details, errorCode);
                if (string.IsNullOrEmpty(authenticationErrorContent))
                {
                    authenticationErrorContent = "Login failed";
                }
            }

            content = content.Replace("{AuthenticationError}", authenticationErrorContent);
            return content;
        }

        private async Task<string> GetAuthenticationErrorContentAsync(string details, string errorCode)
        {
            IFileInfo fileInfo;
            var authenticationErrorContent = string.Empty;
            var fullPath = Path.Combine(
                this.AppDataDirectoryInfo.FullName,
                "Views",
                "Shared",
                errorCode == "true" ? "_AuthenticationError.cshtml" : "_MaintenanceMode.cshtml");

            if (this.AppDataDirectoryInfo.FileSystem.TryGetFile(fullPath, out fileInfo))
            {
                using (var stream = fileInfo.OpenRead())
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        authenticationErrorContent = await streamReader.ReadToEndAsync();
                    }
                }

                authenticationErrorContent = authenticationErrorContent.Replace(
                    "{reason}",
                    string.IsNullOrEmpty(details) ? string.Empty : details);
            }
            else
            {
                Logger.Debug("Authentication error file not found");
            }

            return authenticationErrorContent;
        }
    }
}