using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Rekognition;
using Amazon.S3;
using Amazon.S3.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text.Json;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace S3EventSource;

public class Function
{
    IAmazonS3 S3Client { get; set; }

    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {
        S3Client = new AmazonS3Client();
    }

    /// <summary>
    /// Constructs an instance with a preconfigured S3 client. This can be used for testing the outside of the Lambda environment.
    /// </summary>
    /// <param name="s3Client"></param>
    public Function(IAmazonS3 s3Client)
    {
        this.S3Client = s3Client;
    }
    
    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
    /// to respond to S3 notifications.
    /// </summary>
    /// <param name="evnt"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(S3Event evnt, ILambdaContext context)
    {
        var s3Event = evnt.Records?[0].S3;
        if(s3Event == null)
        {
            return null;
        }
        AmazonRekognitionClient RekognitionClient = new AmazonRekognitionClient();
        var detectLabelsRequest = new Amazon.Rekognition.Model.DetectLabelsRequest
        {
            Image = new Amazon.Rekognition.Model.Image
            {
                S3Object = new Amazon.Rekognition.Model.S3Object
                {
                    Bucket = s3Event.Bucket.Name,
                    Name = s3Event.Object.Key
                }
            },
            MinConfidence = .75F
        };
        var response = RekognitionClient.DetectLabelsAsync(detectLabelsRequest).Result;
        foreach (var label in response.Labels)
        {
            Console.WriteLine($"Found label {label.Name} with confidence {label.Confidence}");
        }

        return null;
    }

    


}