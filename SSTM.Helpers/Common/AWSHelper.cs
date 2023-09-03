using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using SSTM.Models.AWS;
using System;
using System.IO;
using System.Net;

namespace SSTM.Helpers.Common
{
    public class AWSHelper
    {
        public static bool CreateDirectory(AWSModel model)
        {
            bool iscreate = false;

            IAmazonS3 client = new AmazonS3Client(model.AccessKey, model.SecreteKey, RegionEndpoint.APSoutheast1);

            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = model.BucketName,
                Key = model.BucketDirectory // <-- in S3 key represents a path  
            };
            //request.CannedACL = S3CannedACL.PublicRead;
            PutObjectResponse response = client.PutObject(request);
            iscreate = true;

            return iscreate;
        }

        public static bool DeleteDirectory(AWSModel model)
        {
            bool iscreate = false;

            IAmazonS3 client = new AmazonS3Client(model.AccessKey, model.SecreteKey, RegionEndpoint.APSoutheast1);

            DeleteObjectRequest request = new DeleteObjectRequest()
            {
                BucketName = model.BucketName,
                Key = model.BucketDirectory // <-- in S3 key represents a path  
            };

            DeleteObjectResponse response = client.DeleteObject(request);
            iscreate = true;

            return iscreate;
        }

        public static bool UploadFile(AWSModel model)
        {
            IAmazonS3 client = new AmazonS3Client(model.AccessKey, model.SecreteKey, RegionEndpoint.APSoutheast1);

            TransferUtility utility = new TransferUtility(client);

            TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();

            if (model.BucketDirectory == null || model.BucketDirectory == "")
                request.BucketName = model.BucketName; //no subdirectory just bucket name  
            else // subdirectory and bucket name  
                request.BucketName = model.BucketName + @"/" + model.BucketDirectory;

            request.Key = model.FileName; //file name up in S3  
            request.InputStream = model.LocalFileStream;
            //request.CannedACL = S3CannedACL.BucketOwnerRead;
            //request.CannedACL = S3CannedACL.PublicReadWrite;
           utility.Upload(request); //commensing the transfer  

            return true; //indicate that the file was sent  
        }

        public static void GetFiles(AWSModel model)
        {
            IAmazonS3 client = new AmazonS3Client(model.AccessKey, model.SecreteKey, RegionEndpoint.APSoutheast1);

            ListObjectsRequest request = new ListObjectsRequest
            {
                BucketName = model.BucketName,
                Prefix = model.BucketDirectory
            };

            ListObjectsResponse response = client.ListObjects(request);

            foreach (S3Object obj in response.S3Objects)
            {
                Console.WriteLine(obj.Key);
            }
        }

        public static bool GetSingleFile(AWSModel model)
        {
            GetFiles(model);

            IAmazonS3 client = new AmazonS3Client(model.AccessKey, model.SecreteKey, RegionEndpoint.APSoutheast1);

            GetObjectRequest request = new GetObjectRequest();
            request.BucketName = model.BucketName;
            request.Key = model.BucketDirectory + "/" + model.FileName;
            GetObjectResponse response = null;
            using (response = client.GetObject(request))
                response.WriteResponseStreamToFile(model.FilePath, true);
            if(response.HttpStatusCode== HttpStatusCode.OK)
                return true;
            else
                return false;
        }

        public static bool DeleteFile(AWSModel model)
        {
            IAmazonS3 client = new AmazonS3Client(model.AccessKey, model.SecreteKey, RegionEndpoint.APSoutheast1);

            bool isdelete = false;
            string folderPath = model.BucketDirectory+ "/"+ model.FileName;
            var deleteFileRequest = new DeleteObjectRequest
            {
                BucketName = model.BucketName,
                Key = folderPath
            };

            DeleteObjectResponse fileDeleteResponse = client.DeleteObject(deleteFileRequest);

            //if (fileDeleteResponse.DeleteMarker == "true")
                isdelete = true;

            return isdelete;
        }

        public static bool moveFile(AWSModel model)
        {
            IAmazonS3 client = new AmazonS3Client(model.AccessKey, model.SecreteKey, RegionEndpoint.APSoutheast1);

            bool isdelete = false;
            string res = "";
            PutObjectResponse response=new PutObjectResponse();
            try
            {
                FileInfo file = new FileInfo(model.FilePath);

                PutObjectRequest request = new PutObjectRequest()
                {
                    InputStream = file.OpenRead(),
                    BucketName = model.BucketName,
                    Key = model.BucketDirectory + "/" + model.FileName
                };

                 response = client.PutObject(request);
                res = response.HttpStatusCode.ToString();
            }
            catch (Exception ex)
            {
                string e = ex.Message;
                res = response.HttpStatusCode.ToString();
            }
            if (res == "OK")
            {
                string filePath = model.BucketDirectory+"/"+ model.FileName;
                var deleteFileRequest = new DeleteObjectRequest
                {
                    BucketName = model.BucketName,
                    Key = model.OldBucketDirectory + "/" + model.FileName
                };

                DeleteObjectResponse fileDeleteResponse = client.DeleteObject(deleteFileRequest);

                if (fileDeleteResponse.DeleteMarker == "true")
                    isdelete = true;
            }
            return isdelete;

           
        }
    }
}