using SW.Serverless.Sdk;
using System;
using System.Threading.Tasks;
using SW.PrimitiveTypes;
using Azure.Storage.Blobs;
using System.Text;
using System.IO;

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
            var _client = new BlobContainerClient(Runner.StartupValueOf(CommonProperties.ConnectionString), Runner.StartupValueOf(CommonProperties.TargetPath));

            if (_client == null)
                throw new Exception("Container not found");  

            var fileName = "";
            if (Runner.StartupValueOf(CommonProperties.FileName) != "")
                fileName = Runner.StartupValueOf("FileName");
            else
                fileName = string.Concat(DateTime.UtcNow.ToString("yyyyMMddHHmmss"), ".", Runner.StartupValueOf(CommonProperties.FileExtension));

            var _blockBlob = _client.GetBlobClient(fileName);
            byte[] byteArray = Encoding.Unicode.GetBytes(xchangeFile.Data);
           using MemoryStream stream = new MemoryStream(byteArray);
            await _blockBlob.UploadAsync(stream);
            return new XchangeFile(string.Empty);
        }

       
    }
}
