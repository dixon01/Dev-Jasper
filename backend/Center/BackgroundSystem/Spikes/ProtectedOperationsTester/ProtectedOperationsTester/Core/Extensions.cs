namespace ProtectedOperationsTester.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class Extensions
    {
        public static string ToMd5(this string text, bool useCapitalChars = false)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(text);
            var hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            string format;
            if (useCapitalChars)
            {
                format = "X2";
            }
            else
            {
                format = "x2";
            }

            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString(format));
            }

            var output = sb.ToString();
            return output;
        }

        public static void SetLogin<T>(this ChannelFactory<T> factory, string username, string password)
        {
            var defaultCredentials = factory.Endpoint.Behaviors.Find<ClientCredentials>();
            factory.Endpoint.Behaviors.Remove(defaultCredentials);

            // step two - instantiate your credentials
            var loginCredentials = new ClientCredentials();
            loginCredentials.ServiceCertificate.Authentication.CertificateValidationMode =
                X509CertificateValidationMode.None;
            loginCredentials.UserName.UserName = username;
            loginCredentials.UserName.Password = password;

            factory.Endpoint.Behaviors.Add(loginCredentials);
        }

        public static void SetCertificate<T>(this ChannelFactory<T> factory, string name)
        {
            var defaultCredentials = factory.Endpoint.Behaviors.Find<ClientCredentials>();
            factory.Endpoint.Behaviors.Remove(defaultCredentials);

            // step two - instantiate your credentials
            var loginCredentials = new ClientCredentials();
            loginCredentials.ServiceCertificate.Authentication.CertificateValidationMode =
                X509CertificateValidationMode.None;

            loginCredentials.ClientCertificate.SetCertificate(
                StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, name);
            factory.Endpoint.Behaviors.Add(loginCredentials);
        }
    }
}
