using Microsoft.WindowsAzure.Storage;
using SW.Serverless.Sdk;
using System;
using System.Threading.Tasks;
using SW.PrimitiveTypes;

namespace SW.InfolinkAdapters.Handlers.AzureBlob
{
    public class Handler : IInfolinkHandler
    {
        public async Task<XchangeFile> Handle(XchangeFile xchangeFile)
        {
            var _storageAccount = CloudStorageAccount.Parse(Runner.StartupValueOf("BlobStorageAdaptor.ConnectionString"));
            var _client = _storageAccount.CreateCloudBlobClient();
            var _container = _client.GetContainerReference(Runner.StartupValueOf("BlobStorageAdaptor.ContainerName"));
            var _blockBlob = _container.GetBlockBlobReference(DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "." + Runner.StartupValueOf("BlobStorageAdaptor.FileExtension"));
            await _blockBlob.UploadTextAsync(xchangeFile.Data);
            return new XchangeFile(string.Empty);
        }

       
    }
}
