using SW.Serverless.Sdk;
using System;
using System.Threading.Tasks;
using SW.PrimitiveTypes;
using System.IO;
using System.Collections.Generic;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage;
using System.Linq;

namespace SW.InfolinkAdapters.Receivers.AzureBlob
{
    public class Handler : IInfolinkReceiver
    {
        CloudBlobContainer container;
        public Handler()
        {
            Runner.Expect(CommonProperties.ConnectionString);
            Runner.Expect(CommonProperties.TargetPath);
        }

        public  async Task DeleteFile(string fileId)
        {
            var _blockBlob = container.GetBlockBlobReference(fileId);
            await _blockBlob.DeleteAsync();
            return;
        }

        public async Task Finalize()
        {
            return;
        }

        public async Task<XchangeFile> GetFile(string fileId)
        {
            CloudBlockBlob blobRef = container.GetBlockBlobReference(fileId);
            var data = await blobRef.DownloadTextAsync();
            return new XchangeFile(data, fileId);
        }

        public async Task Initialize()
        {
            var _storageAccount = CloudStorageAccount.Parse(Runner.StartupValueOf(CommonProperties.ConnectionString));
            var _client = _storageAccount.CreateCloudBlobClient();
            container = _client.GetContainerReference(Runner.StartupValueOf(CommonProperties.TargetPath));
          if (container == null)
                throw new Exception("Container not found");

        }

        public async Task<IEnumerable<string>> ListFiles()
        {
            var list = (await container.ListBlobsSegmentedAsync(null)).Results;
            string[] blobNames = list.OfType<CloudBlockBlob>().Select(b => b.Name).ToArray();
            return blobNames;
        }

    }
}
