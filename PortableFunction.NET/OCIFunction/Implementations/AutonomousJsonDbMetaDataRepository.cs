using Common.Support.Models;
using Common.Support.ServiceInterfaces;
using crypto;
using Microsoft.Extensions.Configuration;
using Oci.Common.Auth;
using Oci.SecretsService;
using Oci.SecretsService.Models;
using Oci.SecretsService.Requests;
using Oci.SecretsService.Responses;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementations
{
    public class AutonomousJsonDbMetaDataRepository : IMetaDataRepository
    {
        private ISecretsManagement _secrets;

        public AutonomousJsonDbMetaDataRepository(IBasicAuthenticationDetailsProvider provider, ISecretsManagement secrets)
        {
            _secrets = secrets;
        }
        public List<Common.Support.Models.Tag> SaveMetadata(string bucketName, string objectKey, List<Label> labels)
        {
            List<Common.Support.Models.Tag> tagList = new List<Common.Support.Models.Tag>();
            ImageMetadataDataModel dataModel = new ImageMetadataDataModel
            {
                Image = objectKey,
                Labels = labels.Select(l => l.Name).ToList()
            };

            using (OracleConnection connection = new OracleConnection(_secrets.ConnectionString))
            {

                Console.WriteLine("Connecting to Oracle Autonomous Database...");
                connection.Open();
                Console.WriteLine("Connection successful!");

                foreach (var label in labels)
                {
                    tagList.Add(new Common.Support.Models.Tag
                    {
                        Key = label.Name,
                        Value = label.Confidence.Value.ToString("F2")
                    });
                }
                string testTableForValue = "SELECT COUNT(*) FROM ImageMetadata WHERE Image = :image";
                OracleCommand testCommand = new OracleCommand(testTableForValue);
                testCommand.Parameters.Add(new OracleParameter("image", dataModel.Image));
                testCommand.Connection = connection;
                int count = Convert.ToInt32(testCommand.ExecuteScalar());
                if (count > 0)
                {
                    string updateCommand = "UPDATE ImageMetadata SET Labels = :labels WHERE Image = :image";
                    OracleCommand updateCmd = new OracleCommand(updateCommand);
                    updateCmd.Parameters.Add(new OracleParameter("labels", dataModel.SerialiseLabels()));
                    updateCmd.Parameters.Add(new OracleParameter("image", dataModel.Image));
                    updateCmd.Connection = connection;
                    int rowsUpdated = updateCmd.ExecuteNonQuery();
                    return tagList;
                }

                string insertCommand = "INSERT INTO ImageMetadata (Image, Labels) VALUES (:image, :labels)";
                OracleCommand command = new OracleCommand(insertCommand);
                command.Parameters.Add(new OracleParameter("image", dataModel.Image));
                command.Parameters.Add(new OracleParameter("labels", dataModel.SerialiseLabels()));
                command.Connection = connection;
                int rowsInserted = command.ExecuteNonQuery();
            }

            return tagList;
        }

        public List<Label> GetMetadata(string bucketName, string objectKey)
        {
            List<Label> labels = new List<Label>();

            using (OracleConnection connection = new OracleConnection(_secrets.ConnectionString))
            {
                string testTableForValue = "SELECT * FROM ImageMetadata WHERE Image = :image";
                OracleCommand testCommand = new OracleCommand(testTableForValue);
                testCommand.Parameters.Add(new OracleParameter("image", objectKey));
                testCommand.Connection = connection;
                testCommand.Connection.Open();
                OracleDataReader reader = testCommand.ExecuteReader();
                ImageMetadataDataModel dataModel = null;
                if (reader.Read())
                {
                    dataModel = new ImageMetadataDataModel
                    {
                        Image = reader["Image"].ToString()
                    };
                    dataModel.DeserialiseLabels(reader["Labels"].ToString());
                    foreach (var label in dataModel.Labels)
                    {
                        labels.Add(new Label
                        {
                            Name = label,
                            Confidence = 100.0f
                        });
                    }
                }
                else
                {
                    return labels;
                }
            }
            return labels;
        }
    }
}
