using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Support.AWS
{
    public class DynamoDbMetaDataRepository : IMetaDataRepository
    {
        private AmazonDynamoDBClient _dynamoDBclient;

        public DynamoDbMetaDataRepository ()
        {
            _dynamoDBclient = new AmazonDynamoDBClient();
        }

        public List<Label> GetMetadata(string bucketName, string objectKey)
        {
            throw new NotImplementedException();
        }

        public List<UniversalTag> SaveMetadata(string bucketName, string objectKey, List<Label> labels)
        {
            var tags = new List<UniversalTag>();
            foreach (var label in labels)
            {
                ImageMetadataDataModel imageMetadata = new ImageMetadataDataModel()
                {
                    Image = objectKey,
                    Labels = labels.Select(x => x.Name).ToList()
                };

                DynamoDBContext context = new DynamoDBContext(_dynamoDBclient);
                var dynamoResult = context.SaveAsync(imageMetadata);
                dynamoResult.Wait();

                var lookupRequest = context.LoadAsync<ImageLookupDataModel>(label.Name);
                lookupRequest.Wait();
                var lookupData = lookupRequest.Result;
                if (lookupData == null)
                {
                    lookupData = new ImageLookupDataModel()
                    {
                        Label = label.Name,
                        Images = new List<string>()
                    };
                }
                ;
                lookupData.Images.Add(objectKey);
                context.SaveAsync(lookupData);
                dynamoResult.Wait();
            }
            return tags;
        }
    }
}
