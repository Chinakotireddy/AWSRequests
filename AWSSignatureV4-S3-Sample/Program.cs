using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSSignatureV4_S3_Sample
{
    class Program
    {
        private static string bucketName = "mybucketname";
        private static string awsRegion = "eu-west-1";

        static void Main(string[] args)
        {


            //Console.WriteLine("\n\n************************************************$$$$");
            //GetS3ObjectSample.Run(awsRegion);

            //Console.WriteLine("\n\n************************************************");
            PostS3ObjectSample.Run(awsRegion);

        }
    }
}
