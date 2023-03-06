using System;
using System.Collections.Generic;
using System.Configuration;

using AWSSignatureV4_S3_Sample.Signers;
using AWSSignatureV4_S3_Sample.Util;

namespace AWSSignatureV4_S3_Sample
{
    /// <summary>
    /// Samples showing how to GET an object from Amazon S3 using Signature V4 authorization.
    /// </summary>
    public class GetS3ObjectSample
    {
        static readonly string AWSAccessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
        static readonly string AWSSecretKey = ConfigurationManager.AppSettings["AWSSecretKey"];

        /// <summary>
        /// Request the content of an object from the specified bucket using virtual hosted-style 
        /// object addressing.
        /// </summary>
        public static void Run(string region)
        {
            Console.WriteLine("GetS3ObjectSample");

            // Construct a virtual hosted style address with the bucket name part of the host address,
            // placing the region into the url if we're not using us-east-1.
            var regionUrlPart = string.Empty;
            if (!string.IsNullOrEmpty(region))
            {
                if (!region.Equals("eu-west-1", StringComparison.OrdinalIgnoreCase))
                    regionUrlPart = string.Format("-{0}", region);
            }

            var endpointUri = "https://api.staging.connectedvideo.tv/proximity/heartbeats/ping";
            var uri = new Uri(endpointUri);

            // for a simple GET, we have no body so supply the precomputed 'empty' hash
            var headers = new Dictionary<string, string>
            {
                //{AWS4SignerBase.X_Amz_Content_SHA256, AWS4SignerBase.EMPTY_BODY_SHA256}
                //{"content-type", "application/json"}
            };

            var signer = new AWS4SignerForAuthorizationHeader
            {
                EndpointUri = uri,
                HttpMethod = "GET",
                Service = "execute-api",
                Region = "eu-west-1"
            };

            var authorization = signer.ComputeSignature(headers,
                                                        "",   // no query parameters
                                                        AWS4SignerBase.EMPTY_BODY_SHA256,
                                                        AWSAccessKey,
                                                        AWSSecretKey);

            // place the computed signature into a formatted 'Authorization' header 
            // and call S3
            headers.Add("Authorization", authorization);

            HttpHelpers.InvokeHttpRequest(uri, "GET", headers, null);
        }
    }
}
