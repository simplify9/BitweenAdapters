using nsoftware.InEDI;
using SW.Serverless.Sdk;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SW.PrimitiveTypes;

namespace SW.Infolink.SftpFileHandler
{
    class Handler : IInfolinkHandler
    {
        public Handler()
        {
            Runner.Expect("SftpLicenseKey");
            Runner.Expect("Host");
            Runner.Expect("Port","22");
            Runner.Expect("Username");
            Runner.Expect("Password");
            Runner.Expect("DestinationPath");
            Runner.Expect("FileNamePrefix");

        }

        static Random _random = new Random();
        public event Sftp.OnListDirectoryCompletedHandler OnDirList;

        public delegate void OnListDirectoryCompletedHandler(object sender, SftpAsyncCompletedEventArgs e);
        int x = 0;
        private nsoftware.InEDI.Sftp sftp;

        public async Task<XchangeFile> Handle(XchangeFile xchangeFile)
        {
            using (var sftp = new Rebex.Net.Sftp())
            {
                Rebex.Licensing.Key = Runner.StartupValueOf("SftpLicenseKey");
                // connect to a server
                await sftp.ConnectAsync(Runner.StartupValueOf("Host"), Convert.ToInt32(Runner.StartupValueOf("Port")));

                // verify server's fingerprint
                // (see Security section for details)
                // ...

                // authenticate
                await sftp.LoginAsync(Runner.StartupValueOf("Username"), Runner.StartupValueOf("Password"));

                //upload

                byte[] byteArray = Encoding.ASCII.GetBytes(xchangeFile.Data);
                MemoryStream stream = new MemoryStream(byteArray);
                Stream str = stream;
                var filename = Runner.StartupValueOf("FileNamePrefix") + "_" + DateTime.UtcNow.Day.ToString() + DateTime.UtcNow.Month.ToString() + DateTime.UtcNow.Year.ToString() + DateTime.UtcNow.Hour.ToString() + DateTime.UtcNow.Minute.ToString() + DateTime.UtcNow.Second.ToString() + DateTime.UtcNow.Millisecond.ToString();  //request.File.Filename;
                await sftp.PutFileAsync(str, Runner.StartupValueOf("DestinationPath") + "/" + filename);


                //sftp.Config("SSHAcceptAnyServerHostKey=true");


                // download a file
                //sftp.Download("/MyData/file2.txt", @"C:\MyData");

                // disconnect (not required, but polite)
                await sftp.DisconnectAsync();
                return new XchangeFile( string.Empty);
            }
        }
    }
}
