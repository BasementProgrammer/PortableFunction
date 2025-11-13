using Common.Support.ServiceInterfaces;
using Microsoft.Extensions.Configuration;
using Oci.Common;
using Oci.Common.Auth;
using Oci.SecretsService.Models;
using Oci.SecretsService.Requests;
using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementations
{
    public class OCIVaultSecretsManager : ISecretsManagement
    {
        public string ConnectionString { get; private set; }
        public string StorageNamespace { get; private set; }
        
        public string StorageBucket { get; private set; }

        public string CompartmetId { get; private set; }

        private string GetSecretValue (IBasicAuthenticationDetailsProvider provider, string ocid)
        {
            var client = new Oci.SecretsService.SecretsClient(provider);
            GetSecretBundleRequest getSecretBundleRequest = new GetSecretBundleRequest
            {
                SecretId = ocid
            };
            var secretResponce = client.GetSecretBundle(getSecretBundleRequest).Result;
            var secretBundle = secretResponce.SecretBundle;
            Base64SecretBundleContentDetails secretBundleContent = (Base64SecretBundleContentDetails)secretBundle.SecretBundleContent;
            var content = secretBundleContent.Content;
            var decodedBytes = Convert.FromBase64String(content);
            return Encoding.UTF8.GetString(decodedBytes);
        }
        public OCIVaultSecretsManager(IBasicAuthenticationDetailsProvider provider, IConfiguration configuration)
        {
            if (configuration == null)
            {
                ConnectionString = GetSecretValue(provider, "ocid1.vaultsecret.oc1.iad.amaaaaaafvqkqmqawww2ywbybn2nwslteiko7nqug5rceu766qtffa66q3cq");
                StorageNamespace = GetSecretValue(provider, "ocid1.vaultsecret.oc1.iad.amaaaaaafvqkqmqayddtsbhebyahq5jbt6wslsxsafrlchftmouartgpkouq");
                StorageBucket = GetSecretValue(provider, "ocid1.vaultsecret.oc1.iad.amaaaaaafvqkqmqargvy4yajmo22lg5wmuwedl5jtddiz7hsamqcqk7b5bta");
                CompartmetId = GetSecretValue(provider, "ocid1.vaultsecret.oc1.iad.amaaaaaafvqkqmqa463dhosnmwltwdqobu6h4yp4w4u37farbfsusqemgizq");
            }
            else
            {
                ConnectionString = GetSecretValue(provider, configuration["ConnectionString"]);
                StorageNamespace = GetSecretValue(provider, configuration["StorageNamespace"]);
                StorageBucket = GetSecretValue(provider, configuration["StorageBucket"]);
                CompartmetId = GetSecretValue(provider, configuration["CompartmetId"]);
            }
        }
    }
}
