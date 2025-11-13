using Common.Support.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Support.ServiceInterfaces
{
    public interface IMetaDataRepository
    {
        List<Tag> SaveMetadata(string bucketName, string objectKey, List<Label> labels);
        List<Label> GetMetadata(string bucketName, string objectKey);
    }
}
