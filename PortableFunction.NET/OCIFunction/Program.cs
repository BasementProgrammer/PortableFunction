using Fnproject.Fn.Fdk;
using Common.Support;
using Microsoft.Extensions.Configuration;
using Oci.Common.Auth;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Implementations;
using Common.Support.ServiceInterfaces;
using Common.Support.Models;
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

            // Authenticate the function using Instance Principals
            IBasicAuthenticationDetailsProvider authenticationDetailsProvider = ResourcePrincipalAuthenticationDetailsProvider.GetProvider();

			// Instanciate the business logic handler, and pass in the OCI implementations
			// to handle platform code
			ISecretsManagement secrets = new OCIVaultSecretsManager(authenticationDetailsProvider, config);
            BusinessFunction universalFunction = new BusinessFunction
			(
				new OCIVisionImageLabelDetector(authenticationDetailsProvider, secrets),
				new OCIObjectTagging(authenticationDetailsProvider, secrets),
				new AutonomousJsonDbMetaDataRepository(authenticationDetailsProvider, secrets)
			);

			List<StorageObject> objectRecords = new List<StorageObject>();

			// Need to revise this to parse the OCI Object Storage event format
			var inputJson = JsonDocument.Parse(input);
			var data = inputJson.RootElement.GetProperty("data");
			Console.WriteLine("Got Data");
			var additionalDetails = data.GetProperty("additionalDetails");
			Console.WriteLine("Got additionalDetails");

			string bucketName = additionalDetails.GetProperty("bucketName").GetString();
			string objectKey = data.GetProperty("resourceName").GetString();
			Console.WriteLine($"Bucket Name: {bucketName}, Object Key: {objectKey}");

            objectRecords.Add(new StorageObject
			{
				BucketName = bucketName,
				ObjectKey = objectKey,
			});

			Console.WriteLine($"Received {objectRecords.Count} records from S3 event");
			universalFunction.ProcessImages(objectRecords);

			return string.Format("Hello {0}!",
				string.IsNullOrEmpty(input) ? "World" : input.Trim());
		}

		static void Main(string[] args)
		{
			Fdk.Handle(args[0]);
		}
	}
}
