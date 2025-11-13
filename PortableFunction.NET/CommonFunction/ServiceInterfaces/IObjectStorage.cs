using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Support.ServiceInterfaces
{
    public interface IObjectStorage
    {
        /// <summary>
        /// Get the list of images that have been stored in the object storage bucket.
        /// </summary>
        string[] ObjectList { get; }

        string GetObjectData(string imageName);
    }
}
