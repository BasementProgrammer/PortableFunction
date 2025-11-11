# PortableFunction
A project to build portable serverless applications in .NET.

Since I am at heart a .NET developer, this project is being developed in .NET for the moment. 

Currently supporting 
AWS-SAM 
OCI Functions

This repository is a sample project that is designed to help developers build serverless applications that can be deployed to multiple cloud providers.

Code structure as follows:
PortableFunction.NET                    - The serverless application code
   /AWSFunction                     - AWS SAM Project, this contains AWS Specific functionality to deploy as a Lambda Function
   /OCIFunction                     - Fn Project Project, This contains the OCI Specific functionality to deploy to OCI
   /CommonFunction                  - This contains the portable business logic. (The important Stuff)
   /support                         - This contains helpful files such as template commands and payloads for testing
   Dockerfile                       - Docker file for Fn Project to use to build your project. 
   func.yalm                        - fn Project function definition
   PortableFunctions.sln            - The Visual Studio Solution file






SampleImages        - These are royaltee free images that can be used as sample test data. 
