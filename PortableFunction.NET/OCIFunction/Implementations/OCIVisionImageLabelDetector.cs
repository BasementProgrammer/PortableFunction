using Common.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oci.Common.Auth;
using Oci.AivisionService;
using Oci.AivisionService.Models;
using Oci.AivisionService.Requests;


namespace Implementations
{
    public class OCIVisionImageLabelDetector : IImageLabelDetector
    {
        AIServiceVisionClient _client;
        ISecretsManagement _secrets;

        float MinConfidence { get; set; } = IImageLabelDetector.DEFAULT_MIN_CONFIDENCE;

        public HashSet<string> SupportedImageTypes { get; } = new HashSet<string> { ".png", ".jpg", ".jpeg" };

        public OCIVisionImageLabelDetector(IBasicAuthenticationDetailsProvider authProvider, ISecretsManagement secrets)
        {
            _client = new AIServiceVisionClient(authProvider);
            _secrets = secrets;

            NameSpace = _secrets.StorageNamespace;
            CompartmentId = _secrets.CompartmetId;
        }

        public string NameSpace { get;  set; }
        public string CompartmentId { get; set; }

        public List<Common.Support.Label> DetectLabels(string bucketName, string objectKey)
        {
            List<Common.Support.Label> labels = new List<Common.Support.Label>();

            var analyzeImageDetails = new AnalyzeImageDetails
            {
                Features = new List<ImageFeature>
                {
                    new ImageClassificationFeature()
                    {
                        MaxResults = 100
                    }
                },
                Image = new ObjectStorageImageDetails()
                {
                    NamespaceName = NameSpace,
                    BucketName = bucketName,
                    ObjectName = objectKey
                },
                CompartmentId = CompartmentId
            };

            var analyzeImageRequest = new AnalyzeImageRequest
            {
                AnalyzeImageDetails = analyzeImageDetails
            };

            var response = _client.AnalyzeImage(analyzeImageRequest);
            var results = response.Result;

            foreach (var classification in results.AnalyzeImageResult.Labels)
            {
                Console.WriteLine($"Label: {classification.Name}, Confidence: {classification.Confidence}");
                labels.Add(new Common.Support.Label
                {
                    Name = classification.Name,
                    Confidence = classification.Confidence.HasValue ? classification.Confidence.Value : 0
                });
            }

            return labels;
        }
    }
}
