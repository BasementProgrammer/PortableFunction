using Fnproject.Fn.Fdk;
using Common.Support;
using Microsoft.Extensions.Configuration;
using Oci.Common.Auth;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Implementations;
[assembly:InternalsVisibleTo("Function.Tests")]

namespace OCIFunction {
	class Function {
		public Function() {
		}
		public Function(string name) {
		}
		public string FunctionHandler (string input)
		{
            IConfiguration config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

            Console.WriteLine("Function started");
			Console.WriteLine("ConnectionString - " + config["ConnectionString"]);
			Console.WriteLine("StorageNamespace - " + config["StorageNamespace"]);
			Console.WriteLine("StorageBucket - " + config["StorageBucket"]);
			Console.WriteLine("CompartmetId - " + config["CompartmetId"]);

            // Authenticate the function using Instance Principals
            IBasicAuthenticationDetailsProvider authenticationDetailsProvider = ResourcePrincipalAuthenticationDetailsProvider.GetProvider();

			// Instanciate the business logic handler, and pass in the OCI implementations
			// to handle platform code
			ISecretsManagement secrets = new OCIVaultSecretsManager(authenticationDetailsProvider, config);
            UniversalFunction universalFunction = new UniversalFunction
			(
				new OCIVisionImageLabelDetector(authenticationDetailsProvider, secrets),
				new OCIObjectTagging(authenticationDetailsProvider, secrets),
				new AutonomousJsonDbMetaDataRepository(authenticationDetailsProvider, secrets)
			);

			List<UniversalRecord> universalRecords = new List<UniversalRecord>();

			// Need to revise this to parse the OCI Object Storage event format
			var inputJson = JsonDocument.Parse(input);
			var data = inputJson.RootElement.GetProperty("data");
			Console.WriteLine("Got Data");
			var additionalDetails = data.GetProperty("additionalDetails");
			Console.WriteLine("Got additionalDetails");

			string bucketName = additionalDetails.GetProperty("bucketName").GetString();
			string objectKey = data.GetProperty("resourceName").GetString();
			Console.WriteLine($"Bucket Name: {bucketName}, Object Key: {objectKey}");

			universalRecords.Add(new UniversalRecord
			{
				BucketName = bucketName,
				ObjectKey = objectKey,
			});

			Console.WriteLine($"Received {universalRecords.Count} records from S3 event");
			universalFunction.ProcessImages(universalRecords);

			return string.Format("Hello {0}!",
				string.IsNullOrEmpty(input) ? "World" : input.Trim());
		}

		static void Main(string[] args)
		{
			Fdk.Handle(args[0]);
		}
	}
}
