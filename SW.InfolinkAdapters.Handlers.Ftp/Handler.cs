
using SW.Serverless.Sdk;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Rebex.Net;
using SW.PrimitiveTypes;

namespace SW.InfolinkAdapters.Handlers.Ftp
{
    class Handler : IInfolinkHandler
    {
        public Handler()
        {
            Runner.Expect(CommonProperties.LicenseKey);
            Runner.Expect(CommonProperties.Host);
            Runner.Expect(CommonProperties.Port, null);
            Runner.Expect(CommonProperties.Username);
            Runner.Expect(CommonProperties.Password);
            Runner.Expect(CommonProperties.TargetPath, "");
            //Runner.Expect(CommonProperties.FileNamePrefix, null);
            Runner.Expect(CommonProperties.Protocol, "sftp");
        }


        private IFtp ftpOrSftp;

        public async Task<XchangeFile> Handle(XchangeFile xchangeFile)
        {

            Rebex.Licensing.Key = Runner.StartupValueOf(CommonProperties.LicenseKey);

            switch (Runner.StartupValueOf(CommonProperties.Protocol).ToLower())
            {
                case "sftp":

                    var sftp = new Sftp();
                    int sftpPort = string.IsNullOrWhiteSpace(Runner.StartupValueOf(CommonProperties.Port)) ? 22 : Convert.ToInt32(Runner.StartupValueOf(CommonProperties.Port));
                    await sftp.ConnectAsync(Runner.StartupValueOf(CommonProperties.Host), sftpPort);
                    ftpOrSftp = sftp;
                    break;

                case "ftp":

                    var ftp = new Rebex.Net.Ftp();
                    int ftpPort = string.IsNullOrWhiteSpace(Runner.StartupValueOf(CommonProperties.Port)) ? 21 : Convert.ToInt32(Runner.StartupValueOf(CommonProperties.Port));
                    await ftp.ConnectAsync(Runner.StartupValueOf(CommonProperties.Host), ftpPort);
                    ftpOrSftp = ftp;
                    break;

                default:
                    throw new ArgumentException($"Unknown protocol '{Runner.StartupValueOf(CommonProperties.Protocol)}'");

            }

            await ftpOrSftp.LoginAsync(Runner.StartupValueOf(CommonProperties.Username), Runner.StartupValueOf(CommonProperties.Password));

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xchangeFile.Data));
            //Stream str = stream;

            var filename = xchangeFile.Filename;

            if (string.IsNullOrWhiteSpace(filename))
            {
                var currentDate = DateTime.UtcNow;
                filename = $"{currentDate.Year:0000}{currentDate.Month:00}{currentDate.Day:00}{currentDate.Hour:00}{currentDate.Minute:00}{currentDate.Second:00}{currentDate.Millisecond:000}";
            }

            //if (!string.IsNullOrWhiteSpace(Runner.StartupValueOf(CommonProperties.FileNamePrefix)))
            //    filename += Runner.StartupValueOf(CommonProperties.FileNamePrefix) + "_";
            //filename += Runner.StartupValueOf(CommonProperties.FileNamePrefix) + "_" + DateTime.UtcNow.Day + DateTime.UtcNow.Month + DateTime.UtcNow.Year + DateTime.UtcNow.Hour + DateTime.UtcNow.Minute + DateTime.UtcNow.Second + DateTime.UtcNow.Millisecond;

            await ftpOrSftp.PutFileAsync(stream, Runner.StartupValueOf($"{CommonProperties.TargetPath}/{filename}"));

            await ftpOrSftp.DisconnectAsync();
            return new XchangeFile(string.Empty);

        }
    }
}
