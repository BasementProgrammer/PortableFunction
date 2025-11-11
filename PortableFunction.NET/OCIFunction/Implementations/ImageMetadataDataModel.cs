using System.Collections.Generic;

namespace Implementations
{
    public class ImageMetadataDataModel
    {
        public string Image { get; set; }

        public List<string> Labels { get; set; }

        public string SerialiseLabels()
        {
            return System.Text.Json.JsonSerializer.Serialize(Labels);
        }
        public void DeserialiseLabels(string labels)
        {
            Labels = System.Text.Json.JsonSerializer.Deserialize<List<string>>(labels);
        }
    }
}
