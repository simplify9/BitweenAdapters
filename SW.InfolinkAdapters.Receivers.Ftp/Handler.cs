using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rebex.Net;
using SW.Serverless.Sdk;
using SW.PrimitiveTypes;
using SW.InfolinkAdapters;

namespace SW.Infolink.FtpFileReceiver
{
    class Handler : IInfolinkReceiver
    {

        Ftp ftp;
        string dir = string.Empty;
        int batchSize;


        public Handler()
        {
            Runner.Expect(CommonProperties.LicenseKey);
            Runner.Expect(CommonProperties.Host);
            Runner.Expect(CommonProperties.Port, "21");
            Runner.Expect(CommonProperties.Username);
            Runner.Expect(CommonProperties.Password);
            Runner.Expect(CommonProperties.TargetPath, "");
            Runner.Expect(CommonProperties.BatchSize, "50");
            Runner.Expect(CommonProperties.DataReturnType, "");
            Runner.Expect(CommonProperties.DeleteMovesFileTo, null);

        }

        public async Task DeleteFile(string fileId)
        {
            if (await ftp.FileExistsAsync($"{dir}/{fileId}"))
                await ftp.DeleteFileAsync($"{dir}/{fileId}");
        }

        public async Task Finalize()
        {
            await ftp.DisconnectAsync();
            ftp.Dispose();
        }


        public async Task<XchangeFile> GetFile(string fileId)
        {

            // download a remote file into a memory stream
            await using var stream = new MemoryStream();
            await ftp.GetFileAsync($"{dir}/{fileId}", stream);

            // convert memory stream to data
            byte[] data = stream.ToArray();
            string strdata = Encoding.UTF8.GetString(data);

            return new XchangeFile(strdata, fileId);

        }

        public async Task Initialize()
        {

            ftp = new Ftp();

            Rebex.Licensing.Key = Runner.StartupValueOf(CommonProperties.LicenseKey);
            // connect to a server
            await ftp.ConnectAsync(Runner.StartupValueOf(CommonProperties.Host), Convert.ToInt32(Runner.StartupValueOf(CommonProperties.Port)));

            // authenticate
            await ftp.LoginAsync(Runner.StartupValueOf(CommonProperties.Username), Runner.StartupValueOf(CommonProperties.Password));

            dir = Runner.StartupValueOf(CommonProperties.TargetPath);
            batchSize = Convert.ToInt32(Runner.StartupValueOf(CommonProperties.BatchSize));
        }

        public async Task<IEnumerable<string>> ListFiles()
        {

            // get names of items within "/MyData
            string[] fileNames = await ftp.GetNameListAsync(dir);
            if (fileNames.Length >= batchSize)
            {
                fileNames = fileNames.Take(batchSize).ToArray();
            }
            return fileNames;
        }
    }
}
