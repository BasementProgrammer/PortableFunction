using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Support
{
    public interface IObjectTagging
    {
        void ApplyTags(string bucketName, string objectKey, List<UniversalTag> tags);
        void GetObject(string bucketName, string objectKey);
    }
}
