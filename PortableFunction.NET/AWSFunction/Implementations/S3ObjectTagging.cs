using Amazon.S3;
using Amazon.S3.Model;
using Common.Support.Models;
using Common.Support.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementations
{
    public class S3ObjectTagging : IObjectTagging
    {
        AmazonS3Client _S3Client;
        public S3ObjectTagging()
        {
        }
        public void ApplyTags(string bucketName, string objectKey, List<Common.Support.Models.Tag> tags)
        {
            _S3Client = new AmazonS3Client();

            var s3Tags = new List<Amazon.S3.Model.Tag>();
            foreach (var tag in tags)
            {
                s3Tags.Add(new Amazon.S3.Model.Tag { Key = tag.Key, Value = tag.Value });
            }

            var s3PutResonse = _S3Client.PutObjectTaggingAsync(new PutObjectTaggingRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                Tagging = new Tagging
                {
                    TagSet = s3Tags
                }
            });
            s3PutResonse.Wait();
        }

        public void GetObject(string bucketName, string objectKey)
        {
            _S3Client = new AmazonS3Client();
            var getObjectResponse = _S3Client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey
            });
            getObjectResponse.Wait();
            return;
        }
    }
}
