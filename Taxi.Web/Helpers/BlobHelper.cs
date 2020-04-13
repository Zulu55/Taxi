using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Taxi.Web.Helpers
{
    public class BlobHelper : IBlobHelper
    {
        private readonly CloudBlobClient _blobClient;

        public BlobHelper(IConfiguration configuration)
        {
            string keys = configuration["Blob:ConnectionString"];
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public string GetBlobPath(string containerName, string name)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
            return blockBlob.Uri.AbsoluteUri;
        }

        public async Task<string> UploadBlobAsync(byte[] file, string containerName)
        {
            MemoryStream stream = new MemoryStream(file);
            string name = $"{Guid.NewGuid()}";
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
            await blockBlob.UploadFromStreamAsync(stream);
            return name;
        }

        public async Task<string> UploadBlobAsync(IFormFile file, string containerName)
        {
            Stream stream = file.OpenReadStream();
            string name = $"{Guid.NewGuid()}";
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
            await blockBlob.UploadFromStreamAsync(stream);
            return name;
        }

        public async Task<string> UploadBlobAsync(string image, string containerName)
        {
            Stream stream = File.OpenRead(image);
            string name = $"{Guid.NewGuid()}";
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
            await blockBlob.UploadFromStreamAsync(stream);
            return name;
        }
    }
}
