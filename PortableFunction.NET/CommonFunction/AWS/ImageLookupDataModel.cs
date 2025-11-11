using Amazon.DynamoDBv2.DataModel;


namespace Common.Support.AWS
{
    [DynamoDBTable("ImageLookups")]
    public class ImageLookupDataModel
    {
        [DynamoDBProperty("label")]
        public string Label { get; set; }

        public List<string> Images { get; set; }

    }
}
