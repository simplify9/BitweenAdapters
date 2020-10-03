using SW.Serverless.Sdk;
using System;
using System.Threading.Tasks;
using SW.PrimitiveTypes;
using System.Text;
using System.IO;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace SW.InfolinkAdapters.Handlers.AzureBlob
{
    public class Handler : IInfolinkHandler
    {
        public Handler()
        {
            Runner.Expect(CommonProperties.ConnectionString);
            Runner.Expect(CommonProperties.TargetPath);
            Runner.Expect(CommonProperties.FileName,"");
            Runner.Expect(CommonProperties.FileExtension, "");
        }
        public async Task<XchangeFile> Handle(XchangeFile xchangeFile)
        {

            var storageAccount = CloudStorageAccount.Parse(Runner.StartupValueOf(CommonProperties.ConnectionString));
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(Runner.StartupValueOf(CommonProperties.TargetPath));
            if (container == null)
                throw new Exception("Container not found");

            var fileName = "";
            if (Runner.StartupValueOf(CommonProperties.FileName) != "")
                fileName = Runner.StartupValueOf("FileName");
            else
                fileName = string.Concat(DateTime.UtcNow.ToString("yyyyMMddHHmmss"), ".", Runner.StartupValueOf(CommonProperties.FileExtension));

            var blobRef = container.GetBlockBlobReference(fileName);
            await blobRef.UploadTextAsync(xchangeFile.Data);
            return new XchangeFile(string.Empty);
        }

       
    }
}
