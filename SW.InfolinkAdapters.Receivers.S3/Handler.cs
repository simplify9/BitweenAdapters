using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SW.CloudFiles;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;


namespace SW.InfolinkAdapters.Receivers.S3
{
    public class Handler : IInfolinkReceiver
    {
        private CloudFilesService cloudFiles;

        public Handler()
        {
            Runner.Expect(CommonProperties.AccessKeyId);
            Runner.Expect(CommonProperties.SecretAccessKey);
            Runner.Expect(CommonProperties.Url);
            Runner.Expect(CommonProperties.TargetPath);
            Runner.Expect(CommonProperties.FolderName, "");
            Runner.Expect(CommonProperties.BatchSize, "50");
            Runner.Expect(CommonProperties.ContentType, "base64");
            Runner.Expect(CommonProperties.DeleteMovesFileTo, null);
            Runner.Expect("toDictionaryResult", "false");
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
        }

        public async Task<IEnumerable<string>> ListFiles()
        {
            var key = Runner.StartupValueOf(CommonProperties.FolderName);
            var files = await cloudFiles.ListAsync(key);
            var batchSize = Convert.ToInt32(Runner.StartupValueOf(CommonProperties.BatchSize));
            var filesList = files.Where(f => !f.Key.EndsWith("/")).Select(f => f.Key).Take(batchSize).ToArray();

            return filesList;
        }

        public async Task<XchangeFile> GetFile(string fileId)
        {
            var file = await cloudFiles.OpenReadAsync(fileId);
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            var base64 = Convert.ToBase64String(bytes);
            var stringOutput = Encoding.UTF8.GetString(bytes);
            if (Runner.StartupValueOf("toDictionaryResult") == "true")
            {
                stringOutput = JsonConvert.SerializeObject(new Dictionary<string, string>()
                {
                    { "data", stringOutput }
                });
            }
            var xchangeFile = Runner.StartupValueOf(CommonProperties.ContentType) switch
            {
                "text/plain" => new XchangeFile(stringOutput, Path.GetFileName(fileId)),
                _ => new XchangeFile(base64, Path.GetFileName(fileId))
            };

            return xchangeFile;
        }

        public async Task DeleteFile(string fileId)
        {
            if (Runner.StartupValueOf(CommonProperties.DeleteMovesFileTo) != null)
            {
                var file = await cloudFiles.OpenReadAsync(fileId);
                var fileName = Path.GetFileName(fileId);
                await cloudFiles.WriteAsync(file, new WriteFileSettings()
                {
                    Key = Runner.StartupValueOf(CommonProperties.DeleteMovesFileTo) + fileName,
                    Public = false
                });
            }

            await cloudFiles.DeleteAsync(fileId);
        }
    }
}