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

            var cloudStorageAccount = CloudStorageAccount.Parse(@"DefaultEndpointsProtocol=https;AccountName=12345azurestorage;AccountKey=p5QJUvojzpmLYYJZeKH1u4pFuT6SAIOuvENnZWK/Fbyinrvgzou1uEqx5+PF2UTQOBtDfg48c9GdLtA9y8z6nA==;EndpointSuffix=core.windows.net");
            CloudBlobClient cloudBlobClient
                = cloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer
                = cloudBlobClient.GetContainerReference(containerName);

            await cloudBlobContainer.CreateIfNotExistsAsync();

            CloudBlockBlob blockBlob =
                cloudBlobContainer.GetBlockBlobReference("uploadedfilename.png");

            MemoryStream memStream = new MemoryStream();

            await blockBlob.DownloadToStreamAsync(memStream);

            Console.WriteLine(memStream.Length);
        }

        private static async Task UploadFile()
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(@"DefaultEndpointsProtocol=https;AccountName=12345azurestorage;AccountKey=p5QJUvojzpmLYYJZeKH1u4pFuT6SAIOuvENnZWK/Fbyinrvgzou1uEqx5+PF2UTQOBtDfg48c9GdLtA9y8z6nA==;EndpointSuffix=core.windows.net");

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