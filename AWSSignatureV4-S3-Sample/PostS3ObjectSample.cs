using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI.WebControls;
using Amazon.EC2;
using AWSSignatureV4_S3_Sample.Signers;
using AWSSignatureV4_S3_Sample.Util;

namespace AWSSignatureV4_S3_Sample
{
    /// <summary>
    /// Samples showing how to POST an object from Amazon S3 using Signature V4 authorization.
    /// </summary>
    public class PostS3ObjectSample
    {
        static readonly string AWSAccessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
        static readonly string AWSSecretKey = ConfigurationManager.AppSettings["AWSSecretKey"];

        /// <summary>
        /// Request the content of an object from the specified bucket using virtual hosted-style 
        /// object addressing.
        /// </summary>
        public static void Run(string region)
        {
            Console.WriteLine("PostS3ObjectSample");

            // Construct a virtual hosted style address with the bucket name part of the host address,
            // placing the region into the url if we're not using us-east-1.
            var regionUrlPart = string.Empty;
            if (!string.IsNullOrEmpty(region))
            {
                if (!region.Equals("eu-west-1", StringComparison.OrdinalIgnoreCase))
                    regionUrlPart = string.Format("-{0}", region);
            }

            var endpointUri = "https://api.staging.connectedvideo.tv/proximity/heartbeats";
            var uri = new Uri(endpointUri);
            var content_X_Amz_Content_SHA256 = "beaead3198f7da1e70d03ab969765e0821b24fc913697e929e726aeaebf0eba3";

            var headers = new Dictionary<string, string>
            {
                {AWS4SignerBase.X_Amz_Content_SHA256, content_X_Amz_Content_SHA256}
            };

            var signer = new AWS4SignerForPOST
            {
                EndpointUri = uri,
                HttpMethod = "POST",
                Service = "execute-api",
                Region = "eu-west-1"
            };

            var authorization = signer.ComputeSignature(headers,
                                                        "",   // no query parameters
                                                        content_X_Amz_Content_SHA256,
                                                        AWSAccessKey,
                                                        AWSSecretKey);
            authorization = authorization.Replace("content-type;", string.Empty);
            Console.WriteLine($"Authorization: {authorization}");
            // place the computed signature into a formatted 'Authorization' header 
            // and call S3
            headers.Add("Authorization", authorization);
            headers.Add("content-type", "application/json");
            var requestBody = "{\r\n  \"amount\": 1, \r\n  \"heartbeats\": [\r\n    {\r\n      \"date_time_stamp\": \"2022-11-27T20:55:08Z\", \r\n      \"country_code\": \"ZAF\", \r\n      \"device_type\": \"STREAMA\",\r\n      \"connect_id\": \"b767662a-c0d3-46e0-b0b8-3fec9ffcb10a\",\r\n      \"agreement_id\": null, \r\n      \"device_id\": \"b767662a-c0d3-46e0-b0b8-3fec9ffcb10a\", \r\n      \"ip_address\": \"102.221.200.102\", \r\n      \"mac_address\": \"3C-7C-51-27-D9-BD\",\r\n      \"correlation_id\": \"b767662a-c0d3-46e0-b0b8-3fec9ffcb10a\"\r\n    }\r\n  ]\r\n}";
            //HttpHelpers.InvokeHttpRequest(uri, "POST", headers, null);
            HttpHelpers.InvokeHttpRequest(uri, "POST", headers, requestBody);
        }
    }
}
