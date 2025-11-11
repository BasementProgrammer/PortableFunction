using Oci.Common.Auth;
using Oci.ObjectstorageService;
using Oci.ObjectstorageService.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Support.OCI
{
    public class OSObjectTagging : IObjectTagging
    {
        ObjectStorageClient _client;
        private IBasicAuthenticationDetailsProvider _authProvider;
        private ISecretsManagement _secrets;

        public OSObjectTagging(IBasicAuthenticationDetailsProvider authProvider, ISecretsManagement secrets) 
        {
            _secrets = secrets;
            _authProvider = authProvider;

            _client = new ObjectStorageClient(authProvider);
        }

        public void GetObject(string bucketName, string objectKey)
        {
            var getObjectRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                NamespaceName = _secrets.StorageNamespace,
                ObjectName = objectKey
            };

            var response = _client.GetObject(getObjectRequest);
            response.Wait();
            return;
        }
        public void ApplyTags(string bucketName, string objectKey, List<UniversalTag> tags)
        {

            //GetObjectRequest getObjectRequest = new GetObjectRequest();

            //var responce = _client.GetObject(getObjectRequest);
            //responce.Wait();

            //var objectStorageObject = responce.Result;


            return;
        }
    }
}
