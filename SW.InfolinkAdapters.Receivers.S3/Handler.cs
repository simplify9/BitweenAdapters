using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SW.CloudFiles;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;


namespace SW.InfolinkAdapters.Receivers.S3
{
    public class Handler: IInfolinkReceiver
    {
        private CloudFilesService cloudFiles;
        public Handler()
        {
            Runner.Expect(CommonProperties.AccessKeyId);
            Runner.Expect(CommonProperties.SecretAccessKey);
            Runner.Expect(CommonProperties.Url);
            Runner.Expect(CommonProperties.TargetPath);
            Runner.Expect(CommonProperties.FolderName);
            Runner.Expect(CommonProperties.BatchSize, "50");
            Runner.Expect(CommonProperties.ContentType, "text/plain");
        }
        public async Task Initialize()
        {
            cloudFiles = new CloudFilesService(new CloudFilesOptions
            {

                AccessKeyId = Runner.StartupValueOf(CommonProperties.AccessKeyId),
                SecretAccessKey = Runner.StartupValueOf(CommonProperties.SecretAccessKey),
                ServiceUrl = Runner.StartupValueOf(CommonProperties.Url),
                BucketName = Runner.StartupValueOf(CommonProperties.TargetPath),

            });
        }

        public async Task Finalize()
        {
            //await _pop3.DisconnectAsync(false);
            //_pop3.Dispose();
        }

        public async Task<IEnumerable<string>> ListFiles()
        {
            var files = await cloudFiles.ListAsync("");
            var batchSize = Convert.ToInt32(Runner.StartupValueOf(CommonProperties.BatchSize));
            var filesList = files.Select(f => f.Key).Take(batchSize).ToArray();

            return filesList;
        }

        public async Task<XchangeFile> GetFile(string fileId)
        {


            return new XchangeFile("", fileId);
        }

        public Task DeleteFile(string fileId)
        {
            throw new NotImplementedException();
        }
    }
}