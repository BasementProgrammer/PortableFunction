using Amazon.DynamoDBv2.DataModel;


namespace Common.Support.AWS
{
    [DynamoDBTable("ImageMetadata")]
    public class ImageMetadataDataModel
    {
        [DynamoDBProperty("image")]
        public string Image { get; set; }

        public List<string> Labels { get; set; }
    }
}
