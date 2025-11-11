using Common.Support;
using Oci.Common.Auth;
using Oci.ObjectstorageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementations
{
    public class OCIObjectStorage : IObjectStorage
    {
        private ISecretsManagement _secrets;
        IBasicAuthenticationDetailsProvider _provider;
        ObjectStorageClient _client;

        public OCIObjectStorage(IBasicAuthenticationDetailsProvider provider, ISecretsManagement secrets) 
        {
            _secrets = secrets;
            _provider = provider;
            _client = new ObjectStorageClient(_provider);
        }

        public string[] ImageList { 
            get 
            {
                List<string> ImageList = new List<string>();

                _client.ListObjects(new Oci.ObjectstorageService.Requests.ListObjectsRequest
                {
                    BucketName = _secrets.StorageBucket,
                    NamespaceName = _secrets.StorageNamespace
                }).Result.ListObjects.Objects.ToList().ForEach(o => ImageList.Add(o.Name));

                return ImageList.ToArray();
            }
        }

        public string GetImageData(string imageName)
        {
            var response = _client.GetObject(new Oci.ObjectstorageService.Requests.GetObjectRequest
            {
                BucketName = _secrets.StorageBucket,
                NamespaceName = _secrets.StorageNamespace,
                ObjectName = imageName
            }).Result;

            using (var reader = new System.IO.BinaryReader(response.InputStream))
            {
                byte[] bytes = reader.ReadBytes((int)response.ContentLength);
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
