using Microsoft.WindowsAzure.Storage;
using SW.Serverless.Sdk;
using System;
using System.Threading.Tasks;

namespace SW.Infolink.AzureBlobFileHandler
{
    public class Handler : IInfolinkHandler
    {
        public async Task<object> Handle(XchangeFile xchangeFile)
        {
            var _storageAccount = CloudStorageAccount.Parse(Runner.StartupValues.GetOrDefault("BlobStorageAdaptor.ConnectionString", "DefaultEndpointsProtocol=https;AccountName=bcrm1;AccountKey=n+taOdDuZf9nJxp13qO/C/YF1tMH23ulnDrKIHzKZa+/W0kQztWFhNQs1K0Zvq+21YSVu/IQAtxi+Wzuju0TAg==;EndpointSuffix=core.windows.net"));
            var _client = _storageAccount.CreateCloudBlobClient();
            var _container = _client.GetContainerReference(Runner.StartupValues.GetOrDefault("BlobStorageAdaptor.ContainerName", "customers-out"));
            var _blockBlob = _container.GetBlockBlobReference(DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "." + Runner.StartupValues.GetOrDefault("BlobStorageAdaptor.FileExtension", "csv"));
            await _blockBlob.UploadTextAsync(xchangeFile.Data);
            return new XchangeFile(string.Empty);
        }

       
    }
}
