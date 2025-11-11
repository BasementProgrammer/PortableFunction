using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Common.Support
{
    public class UniversalFunction
    {
        IObjectTagging _objectTagging;
        IImageLabelDetector _imageLabelDetector;
        IMetaDataRepository _metaDataRepository;

        public UniversalFunction(IImageLabelDetector imageLabelDetector, IObjectTagging objectTagging, IMetaDataRepository metaDataRepository) 
        {
            _imageLabelDetector = imageLabelDetector;
            _objectTagging = objectTagging;
            _metaDataRepository = metaDataRepository;
        }
        public void ProcessImages(List<UniversalRecord> universalRecords)
        {
            if (_imageLabelDetector == null)
            {
                Console.WriteLine("Image label detector is not initialized. Recreating!!!.");
                //_imageLabelDetector = new Common.Support.AWS.RekognitionImageLabelDetector();
            }
            if (_objectTagging == null)
            {
                Console.WriteLine("Object tagging is not initialized. Recreating!!!.");
                //_objectTagging = new Common.Support.AWS.S3ObjectTagging();
            }
            foreach (var record in universalRecords)
            {
                if (!_imageLabelDetector.SupportedImageTypes.Contains(Path.GetExtension(record.ObjectKey)))
                {
                    Console.WriteLine($"Object {record.BucketName}:{record.ObjectKey} is not a supported image type");
                    continue;
                }

                try
                {
                    // Attempt to read the object from object storage to test access
                    _objectTagging.GetObject(record.BucketName, record.ObjectKey);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accessing object {record.BucketName}:{record.ObjectKey} - {ex.Message}");
                }

                try
                {
                    Console.WriteLine($"Looking for labels in image {record.BucketName}:{record.ObjectKey}");
                    var detectResponses = _imageLabelDetector.DetectLabels(record.BucketName, record.ObjectKey);
                    Console.WriteLine($"Found {detectResponses.Count} labels in image {record.BucketName}:{record.ObjectKey}");
                    var tagList = _metaDataRepository.SaveMetadata(record.BucketName, record.ObjectKey, detectResponses);
                    Console.WriteLine($"Applying {tagList.Count} tags to object {record.BucketName}:{record.ObjectKey}");
                    _objectTagging.ApplyTags(record.BucketName, record.ObjectKey, tagList);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing object {record.BucketName}:{record.ObjectKey} - {ex.Message}");
                }



            }
        }
    }
}
