using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Common.Support;
using Common.Support.Models;
using Implementations;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSImageFunction;

public class Function
{
    /// <summary>
    /// Default constructor used by AWS Lambda to construct the function. Credentials and Region information will
    /// be set by the running Lambda environment.
    /// 
    /// This constuctor will also search for the environment variable overriding the default minimum confidence level
    /// for label detection.
    /// </summary>
    public Function()
    {
    }


    /// <summary>
    /// A function for responding to S3 create events. It will determine if the object is an image and use Amazon Rekognition
    /// to detect labels and add the labels as tags on the S3 object.
    /// </summary>
    /// <param name="input">The S3 event to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task FunctionHandler(S3Event input, ILambdaContext context)
    {
        BusinessFunction universalFunction = new BusinessFunction
            (
                new RekognitionImageLabelDetector(),
                new S3ObjectTagging(),
                new DynamoDbMetaDataRepository()
            );
            
        List<StorageObject> objectRecords = new List<StorageObject>();

        foreach (var record in input.Records)
        {
            objectRecords.Add(new StorageObject
            {
                BucketName = record.S3.Bucket.Name,
                ObjectKey = record.S3.Object.Key,
                ObjectSize = record.S3.Object.Size.ToString(),
                EventTime = record.EventTime.ToString(),
                EventName = record.EventName,
                SourceIPAddress = record.RequestParameters?.SourceIPAddress
            });
        }
        Console.WriteLine($"Received {objectRecords.Count} records from S3 event");
        universalFunction.ProcessImages(objectRecords);

        return;
    }
}