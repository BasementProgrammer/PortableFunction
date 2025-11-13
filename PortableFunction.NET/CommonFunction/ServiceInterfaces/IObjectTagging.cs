using Common.Support.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Support.ServiceInterfaces
{
    public interface IObjectTagging
    {
        void ApplyTags(string bucketName, string objectKey, List<Common.Support.Models.Tag> tags);
        void GetObject(string bucketName, string objectKey);
    }
}
