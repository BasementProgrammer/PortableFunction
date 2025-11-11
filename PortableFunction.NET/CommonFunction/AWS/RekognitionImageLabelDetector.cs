using Amazon.Rekognition;
using Common.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Support.AWS
{
    public class RekognitionImageLabelDetector : IImageLabelDetector
    {

        IAmazonRekognition RekognitionClient { get; }

        float MinConfidence { get; set; } = IImageLabelDetector.DEFAULT_MIN_CONFIDENCE;

        public HashSet<string> SupportedImageTypes { get; } = new HashSet<string> { ".png", ".jpg", ".jpeg" };

        public RekognitionImageLabelDetector()
        {
            RekognitionClient = new AmazonRekognitionClient();

            var environmentMinConfidence = Environment.GetEnvironmentVariable(IImageLabelDetector.MIN_CONFIDENCE_ENVIRONMENT_VARIABLE_NAME);
            if (!string.IsNullOrWhiteSpace(environmentMinConfidence))
            {
                float value;
                if (float.TryParse(environmentMinConfidence, out value))
                {
                    MinConfidence = value;
                    Console.WriteLine($"Setting minimum confidence to {MinConfidence}");
                }
                else
                {
                    Console.WriteLine($"Failed to parse value {environmentMinConfidence} for minimum confidence. Reverting back to default of {MinConfidence}");
                }
            }
            else
            {
                Console.WriteLine($"Using default minimum confidence of {MinConfidence}");
            }
        }

        public List<Label> DetectLabels(string bucketName, string objectKey)
        {
            List<Label> labels = new List<Label>();
            var detectLabelsRequest = new Amazon.Rekognition.Model.DetectLabelsRequest
            {
                Image = new Amazon.Rekognition.Model.Image
                {
                    S3Object = new Amazon.Rekognition.Model.S3Object
                    {
                        Bucket = bucketName,
                        Name = objectKey
                    }
                },
                MinConfidence = MinConfidence
            };
            var response = RekognitionClient.DetectLabelsAsync(detectLabelsRequest).Result;
            foreach (var label in response.Labels)
            {
                Console.WriteLine($"Found label {label.Name} with confidence {label.Confidence}");
                labels.Add(new Label
                {
                    Name = label.Name,
                    Confidence = label.Confidence
                });
            }
            return labels;
        }
    }
}
