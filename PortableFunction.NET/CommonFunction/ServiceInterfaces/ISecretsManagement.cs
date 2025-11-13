using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Support.ServiceInterfaces
{
    public interface ISecretsManagement
    {
        /// <summary>
        /// The connection string to the autonomous JSON database.
        /// </summary>
        string ConnectionString { get; }
        /// <summary>
        /// The Storage namespace for the OCI Object Storage.
        /// </summary>
        string StorageNamespace { get; }
        /// <summary>
        /// The Storage bucket for the OCI Object Storage that holds the images.
        /// </summary>
        string StorageBucket { get; }
        /// <summary>
        /// OCI Compartment for the storage 
        /// </summary>
        string CompartmetId { get; }
    }
}
