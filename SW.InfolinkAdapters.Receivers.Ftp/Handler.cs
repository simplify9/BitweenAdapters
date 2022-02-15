
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Rebex.Net;
using Rebex.Security.Certificates;
using SW.PrimitiveTypes;
using SW.Serverless.Sdk;

namespace SW.InfolinkAdapters.Receivers.Ftp
{
    public class Handler : IInfolinkReceiver
    {
        private IFtp _ftpOrSftp;
        // private string _targetPath = string.Empty;

        public Handler()
        {
            Runner.Expect(CommonProperties.LicenseKey);
            Runner.Expect(CommonProperties.Host);
            Runner.Expect(CommonProperties.Port, null);
            Runner.Expect(CommonProperties.Username);
            Runner.Expect(CommonProperties.Password);
            Runner.Expect(CommonProperties.TargetPath, null);
            Runner.Expect(CommonProperties.BatchSize, "50");
            Runner.Expect(CommonProperties.ResponseEncoding, "utf8");
            Runner.Expect(CommonProperties.DeleteMovesFileTo, null);
            Runner.Expect(CommonProperties.Protocol, "sftp");
            Runner.Expect(CommonProperties.Certificate,null);
        }

        public async Task DeleteFile(string fileId)
        {
            if (!await _ftpOrSftp.FileExistsAsync(fileId)) return;
            var deleteMovesFileTo = Runner.StartupValueOf("DeleteMovesFileTo");
            if (string.IsNullOrWhiteSpace(deleteMovesFileTo))
            {
                await _ftpOrSftp.DeleteFileAsync(fileId);
            }
            else
            {
                await _ftpOrSftp.RenameAsync(fileId, deleteMovesFileTo + "/" + fileId);
            }

        }

        public async Task Finalize()
        {
            await _ftpOrSftp.DisconnectAsync();
            _ftpOrSftp.Dispose();
        }

        public async Task<XchangeFile> GetFile(string fileId)
        {

            // download a remote file into a memory stream
            await using var stream = new MemoryStream();
            await _ftpOrSftp.GetFileAsync(fileId, stream);

            // convert memory stream to data
            var data = stream.ToArray();

            return (Runner.StartupValueOf(CommonProperties.ResponseEncoding).ToLower()) switch
            {
                "base64" => new XchangeFile(Convert.ToBase64String(data), fileId),
                "utf8" => new XchangeFile(Encoding.UTF8.GetString(data), fileId),
                _ => throw new ArgumentException($"Unknown {nameof(CommonProperties.ResponseEncoding)} '{Runner.StartupValueOf(CommonProperties.ResponseEncoding)}'"),
            };
        }

        public async Task Initialize()
        {

            Rebex.Licensing.Key = Runner.StartupValueOf(CommonProperties.LicenseKey);
            var username = Runner.StartupValueOf(CommonProperties.Username);
            var password = Runner.StartupValueOf(CommonProperties.Password);
            var port = Runner.StartupValueOf(CommonProperties.Port);
            var host = Runner.StartupValueOf(CommonProperties.Host);

            switch (Runner.StartupValueOf(CommonProperties.Protocol).ToLower())
            {
                case "sftpssh":
                    var sftpssh = new Sftp();
                    var sftpsshPort = string.IsNullOrWhiteSpace(port) ? 22 : Convert.ToInt32(port);
                    await sftpssh.ConnectAsync(host, sftpsshPort);
                    
                    var certBytes =  Encoding.UTF8.GetBytes(Runner.StartupValueOf(CommonProperties.Certificate));
                    var signingCert = new X509Certificate2(certBytes,  password, X509KeyStorageFlags.Exportable);
                    var privateKey = new SshPrivateKey(signingCert);
                    await sftpssh.LoginAsync(username, privateKey);
                    
                    _ftpOrSftp = sftpssh;
                    break;
                
                case "sftp":
                    var sftp = new Sftp();
                    var sftpPort = string.IsNullOrWhiteSpace(port) ? 22 : Convert.ToInt32(port);
                    await sftp.ConnectAsync(host, sftpPort);
                    _ftpOrSftp = sftp;
                    
                    await _ftpOrSftp.LoginAsync(username, password);
                    break;

                case "ftp":

                    var ftp = new Rebex.Net.Ftp();
                    var ftpPort = string.IsNullOrWhiteSpace(port) ? 21 : Convert.ToInt32(port);
                    await ftp.ConnectAsync(host, ftpPort);
                    _ftpOrSftp = ftp;
                    
                    await _ftpOrSftp.LoginAsync(username, password);
                    break;

                default:
                    throw new ArgumentException($"Unknown protocol '{Runner.StartupValueOf(CommonProperties.Protocol)}'");

            }
            
            
            if (!string.IsNullOrEmpty(Runner.StartupValueOf(CommonProperties.TargetPath)))
                await _ftpOrSftp.ChangeDirectoryAsync(Runner.StartupValueOf(CommonProperties.TargetPath));
        }

        public async Task<IEnumerable<string>> ListFiles()
        {
            var files = await _ftpOrSftp.GetListAsync();
            //fileNames[0].
            var batchSize = Convert.ToInt32(Runner.StartupValueOf(CommonProperties.BatchSize));

            return files.Where(i => i.IsFile).Take(batchSize).Select(i => i.Name).ToArray();

        }
    }
}
