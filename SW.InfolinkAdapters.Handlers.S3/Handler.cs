using SW.CloudFiles;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;
using System;
using System.Threading.Tasks;

namespace SW.InfolinkAdapters.Handlers.S3
{
  public  class Handler : IInfolinkHandler 
    {

        public async Task<XchangeFile> Handle(XchangeFile xchangeFile)
        {
            using var cloudFiles = new CloudFilesService(new CloudFilesOptions
            {

                AccessKeyId = Runner.StartupValueOf("S3StorageAdaptor.AccessKeyId"),
                SecretAccessKey = Runner.StartupValueOf("S3StorageAdaptor.SecretAccessKey"),
                ServiceUrl = Runner.StartupValueOf("S3StorageAdaptor.ServiceUrl"),
                BucketName = Runner.StartupValueOf("S3StorageAdaptor.BucketName"),

            });
            var key = string.Concat(Runner.StartupValueOf("S3StorageAdaptor.Folder"), "/", DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "." + Runner.StartupValueOf("S3StorageAdaptor.FileExtension"));
            await cloudFiles.WriteTextAsync(xchangeFile.Data, new WriteFileSettings
            {
                Key = key,
                ContentType = "text/plain"
            });

            return new XchangeFile(key, xchangeFile.Filename ); 
        }
    }
}
