using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.S3;
using Amazon.S3.Model;

namespace TestLogViewer
{
    public class AwsHelper
    {
        private static readonly AmazonS3Config S3Config = new AmazonS3Config();
        private static readonly AmazonLambdaConfig LambdaConfig = new AmazonLambdaConfig();

        private readonly AmazonS3Client _s3Client;
        private readonly AmazonLambdaClient _lambdaClient;

        public AwsHelper()
        {
            var obj = JsonHelper.GetJsonFile("credentials.json");
            var accessKey = (string) obj.SelectToken("AccessKey");
            var secretKey = (string) obj.SelectToken("SecretKey");
            S3Config.RegionEndpoint = RegionEndpoint.EUCentral1;
            LambdaConfig.RegionEndpoint = RegionEndpoint.EUCentral1;
            _s3Client = new AmazonS3Client(accessKey, secretKey, S3Config);
            _lambdaClient = new AmazonLambdaClient(accessKey, secretKey, LambdaConfig);
        }

        public async Task<GetObjectResponse> S3GetObject(string bucket, string key)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucket,
                Key = key
            };
            Task<GetObjectResponse> response = _s3Client.GetObjectAsync(request);
            if (await Task.WhenAny(response, Task.Delay(90000)) == response)
            {
                return response.Result;
            }
            throw new Exception("Can't download file, timeout reached.");
        }

        public async Task<InvokeResponse> InvokeLambdaFunction(string funcName)
        {
            var task = new Task<InvokeResponse>(() =>
            {
                InvokeRequest request = new InvokeRequest() { FunctionName = funcName };
                var response = _lambdaClient.Invoke(request);
                return response;
            });
            task.Start();
            if (await Task.WhenAny(task, Task.Delay(90000)) == task)
            {
                return task.Result;
            }
            throw new Exception("Timeout reached for lambda function");
        }
    }
}
