
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rebex.Net;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Receivers.SftpRec
{
    class Handler : IInfolinkReceiver
    {
        System.ComponentModel.ComponentCollection _container;
        Sftp sftp;
        string dir = string.Empty;
        string fileReturnType = string.Empty;
        int batchSize = 0;
        
        public Handler()
        {
            Runner.Expect("LicenseKey");
            Runner.Expect("Host");
            Runner.Expect("Port","22");
            Runner.Expect("Username");
            Runner.Expect("Password");
            Runner.Expect("TargetPath");
            Runner.Expect("BatchSize","50");
            Runner.Expect("DataReturnType","");
            Runner.Expect("DeleteMovesFileTo",null);

        }

        public async Task DeleteFile(string fileId)
        {
            if (!await sftp.FileExistsAsync(dir + "/" + fileId)) return;
            var deleteMovesFileTo = Runner.StartupValueOf("DeleteMovesFileTo");
            if (string.IsNullOrWhiteSpace(deleteMovesFileTo))
            {
                await sftp.DeleteFileAsync(dir + "/" + fileId);
            }
            else
            {
                await sftp.RenameAsync(dir + "/" + fileId,
                    deleteMovesFileTo + "/" + fileId);
            }

            return;
        }

        public async Task Finalize()
        {
            await sftp.DisconnectAsync();
            sftp.Dispose();
            return;
        }


        public async Task<XchangeFile> GetFile(string fileId)
        {

            // download a remote file into a memory stream
            var stream = new MemoryStream();
            await sftp.GetFileAsync(dir + "/" + fileId, stream);

            // convert memory stream to data
            byte[] data = stream.ToArray();
            string strdata = "";
            if (fileReturnType.ToLower() == "base64")
            {
                strdata = Convert.ToBase64String(data);
            }
            else
            {
                strdata = Encoding.UTF8.GetString(data);
            }
             
            
            return new XchangeFile(strdata,fileId);

        }

        public async Task Initialize()
        {

            sftp = new Sftp();

            Rebex.Licensing.Key = Runner.StartupValueOf("SftpLicenseKey");
            // connect to a server
            await sftp.ConnectAsync(Runner.StartupValueOf("Host"), Convert.ToInt32(Runner.StartupValueOf("Port")));

            // verify server's fingerprint
            // (see Security section for details)
            // ...

            // authenticate
            await sftp.LoginAsync(Runner.StartupValueOf("Username"), Runner.StartupValueOf("Password"));

            dir = Runner.StartupValueOf("TargetPath");
            batchSize = Convert.ToInt32(Runner.StartupValueOf("BatchSize"));
            fileReturnType = Runner.StartupValueOf("SftpFileReturnType");
            //sftp.Config("SSHAcceptAnyServerHostKey=true");
            //_container = sftp.Container.Components;


        }

        public async Task<IEnumerable<string>> ListFiles()
        {

            // get names of items within "/MyData
            var fileNames = await sftp.GetNameListAsync(dir);
            if (fileNames.Length >= batchSize)
            {
                fileNames = fileNames.Take(batchSize).ToArray();
            }
            return fileNames;


        }
    }
}
