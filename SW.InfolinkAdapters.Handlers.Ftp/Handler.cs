﻿using SW.Serverless.Sdk;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Rebex.Net;
using SW.PrimitiveTypes;

namespace SW.InfolinkAdapters.Handlers.Ftp
{
    public class Handler : IInfolinkHandler
    {
        public Handler()
        {
            Runner.Expect(CommonProperties.LicenseKey, null, true);
            Runner.Expect(CommonProperties.Host);
            Runner.Expect(CommonProperties.Port, null);
            Runner.Expect(CommonProperties.Username);
            Runner.Expect(CommonProperties.Password, false, true);
            Runner.Expect(CommonProperties.TargetPath, null);
            Runner.Expect(CommonProperties.FileNamePrefix, null);
            Runner.Expect(CommonProperties.Protocol, "sftp");
            Runner.Expect(CommonProperties.PrivateKey, null, true);
        }


        private IFtp ftpOrSftp;


        public async Task<XchangeFile> Handle(XchangeFile xchangeFile)
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

                    var privateKey = Runner.StartupValueOf(CommonProperties.PrivateKey);
                    var privateKeyEdited = Regex.Replace(privateKey, @"(?<!:)\s", "\r\n");
                    var keyBytes = Encoding.UTF8.GetBytes(privateKeyEdited);
                    var sshPrivateKey = new SshPrivateKey(keyBytes, password);
                    await sftpssh.LoginAsync(username, sshPrivateKey);

                    ftpOrSftp = sftpssh;
                    break;

                case "sftp":

                    var sftp = new Sftp();
                    var sftpPort = string.IsNullOrWhiteSpace(port) ? 22 : Convert.ToInt32(port);
                    await sftp.ConnectAsync(host, sftpPort);
                    ftpOrSftp = sftp;
                    await ftpOrSftp.LoginAsync(username, password);
                    break;

                case "ftp":

                    var ftp = new Rebex.Net.Ftp();
                    var ftpPort = string.IsNullOrWhiteSpace(port) ? 21 : Convert.ToInt32(port);
                    await ftp.ConnectAsync(host, ftpPort);
                    ftpOrSftp = ftp;
                    await ftpOrSftp.LoginAsync(username, password);
                    break;

                default:
                    throw new ArgumentException(
                        $"Unknown protocol '{Runner.StartupValueOf(CommonProperties.Protocol)}'");
            }


            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xchangeFile.Data));

            var filename = xchangeFile.Filename;

            if (string.IsNullOrWhiteSpace(filename))
            {
                var currentDate = DateTime.UtcNow;
                filename =
                    $"{currentDate.Year:0000}{currentDate.Month:00}{currentDate.Day:00}{currentDate.Hour:00}{currentDate.Minute:00}{currentDate.Second:00}{currentDate.Millisecond:000}";
            }

            //the logic below was changed because it looked like a suffix implementation not a prefix
            if (!string.IsNullOrWhiteSpace(Runner.StartupValueOf(CommonProperties.FileNamePrefix)))
            {
                var customPrefix = Runner.StartupValueOf(CommonProperties.FileNamePrefix) + "_";
                filename = $"{customPrefix}{filename}";
            }
            
            var targetPath = Runner.StartupValueOf(CommonProperties.TargetPath);
            await ftpOrSftp.PutFileAsync(stream, $"{targetPath}/{filename}");

            await ftpOrSftp.DisconnectAsync();
            return new XchangeFile(string.Empty);
        }
    }
}