using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureStorage.Blobs
{
    internal class Program
    {
        private const string StorageConnection = "";

        private static void Main(string[] args)
        {
            MainAsync()
                .GetAwaiter()
                .GetResult();

            Console.ReadLine();
        }

        private static async Task MainAsync()
        {
            await UploadFile();

            //await ReadFile();
        }

        private static async Task ReadFile()
        {
            var containerName = "images";

            CloudStorageAccount cloudStorageAccount 
                = new CloudStorageAccount(new StorageCredentials(StorageConnection), true);

            CloudBlobClient cloudBlobClient 
                = cloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer 
                = cloudBlobClient.GetContainerReference(containerName);

            CloudBlockBlob blockBlob =
                cloudBlobContainer.GetBlockBlobReference("uploadedfilename.png");


            MemoryStream memStream = new MemoryStream();

            await blockBlob.DownloadToStreamAsync(memStream);

            Console.WriteLine(memStream.Length);
        }

        private static async Task UploadFile()
        {

            const string cs =
                "DefaultEndpointsProtocol=https;AccountName=storage12345david;AccountKey=Hug7WhDLbYl1dojX4OABsxteoAIjtMsi9B92W2QBIFuM+iQsZsuKrzedQC04vSFDx3WcTVB/oMFOvexTA9lwig==;EndpointSuffix=core.windows.net";

            var credentials = new StorageCredentials("storage12345david",
                @"Hug7WhDLbYl1dojX4OABsxteoAIjtMsi9B92W2QBIFuM+iQsZsuKrzedQC04vSFDx3WcTVB/oMFOvexTA9lwig==");

            var cloudStorageAccount = new CloudStorageAccount(credentials, true);

            //create a block blob
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("images2");

            //create a container if it is not already exists
            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                //await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions
                //{
                //    PublicAccess = BlobContainerPublicAccessType.Blob
                //});
            }

            FileStream fileStream = File.OpenRead(@"C:\Users\Deivit\Desktop\Screenshot_2.png"); // or

            CloudBlockBlob cloudBlockBlob =
                cloudBlobContainer.GetBlockBlobReference($"{Guid.NewGuid()}image.png");
            cloudBlockBlob.Properties.ContentType = "application/octet-stream";
            cloudBlockBlob.Metadata.Add("name", "david");

            await cloudBlockBlob.UploadFromStreamAsync(fileStream);
        }
    }
}