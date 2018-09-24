namespace Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;

    using Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication.ViewModels;

    using Microsoft.WindowsAzure.Storage;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {

        private static readonly string StorageAccount = "devstoreaccount1";
        //private static readonly string StorageAccount = "azurestoragespike";

        private static readonly string LocalStorageKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";
        protected override void OnStartup(StartupEventArgs e)
        {
            var shell = new Shell(this.Dispatcher);
            var window = new MainWindow { DataContext = shell };
            window.Show();
        }
        private static IEnumerable<ResourceEntity> GetResources()
        {
            var s = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;"
                    + "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;"
                    + "BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;"
                    + "TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;"
                    + "QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1";
            var now = DateTime.UtcNow;
            var url = "http://azurestoragespike.table.core.windows.net";
            url = "http://127.0.0.1:10002";
            var 
            uri = new Uri("http://127.0.0.1:10002/devstoreaccount1/Tables");
            var request = (HttpWebRequest)HttpWebRequest.Create(uri);
            //request.Accept = "application/json;odata=fullmetadata";
            //request.ContentType = "application/json;odata=fullmetadata";
            request.Method = "GET";
            //request.Headers.Add("x-ms-version", "2014-02-14");
            //request.Headers.Add("x-ms-date", now.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
            //request.Headers.Add("x-ms-version", "2014-02-14");
            //var authorizationHeader = AuthorizationHeader("GET", now, request, string.Empty, string.Empty);
            //request.Headers.Add("Authorization", authorizationHeader);
            AddAuthorizationHeader(request, now);
            var response = (HttpWebResponse)request.GetResponse();
            yield break;
        }

        public static void AddAuthorizationHeader(HttpWebRequest request, DateTime utcNow)
        {
            string signature = "GET\n";

            // Content-MD5
            signature += "\n";

            // Content-Type
            signature += "\n";

            // Date
            //var date = request.Headers.Get("Date");//
            var dateString = utcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
            request.Date = utcNow;
            signature += dateString + "\n";

            // Canonicalized Resource
            // Format is /{0}/{1} where 0 is name of the account and 1 is resources URI path
            signature += "/" + StorageAccount + "/Tables";//"('" + TableName + "')";// + TableName;
            signature = string.Format("GET\n\n\n{0}\n/{1}/Tables", dateString, "devstoreaccount1");
            // Hash-based Message Authentication Code (HMAC) using SHA256 hash
            System.Security.Cryptography.HMACSHA256 hasher =
                     new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(LocalStorageKey));

            // Authorization header
            string authH = "SharedKey " + StorageAccount + ":"
                           + System.Convert.ToBase64String(
                               hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signature)));
            //authH = AuthorizationHeader("GET", utcNow, request, StorageAccount, AzureStorageKey, canonicalizedResource: "/" + StorageAccount + "/Tables", ifMatch: string.Empty, md5: string.Empty);
            request.Headers.Add("Authorization", authH);
        }
    }
}
