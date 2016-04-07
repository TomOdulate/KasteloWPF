using System;
using System.Configuration;
using System.Net;

namespace Tao.CredentialStore
{
    /// <summary>
    /// TODO: Add ability to load / save to FTP sites
    /// </summary>
    [Serializable]
    public class StoreManager : ApplicationStore
    {
        public StoreManager(string name, byte[] key, byte[] vector) : base(name, key, vector)
        {
        }
        public StoreManager(ApplicationStore newStore, byte[] key, byte[] vector) : base(newStore, key, vector)
        {
        }
        public bool UploadToFtp(Uri serverUri, string username, string password)
        {
            // Create communication (FTP) object
            var request = (FtpWebRequest)WebRequest.Create(serverUri);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            if (username == null) username = "anonymous";
            request.Credentials = new NetworkCredential(username, password);
            request.UseBinary = true;

            // Encrypt the current object (this) to a byte Array
            var encryptedBytes = EncryptObjectToBytes(this, Vector);

            // Write the encrypted files to the ftp server
            var requestStream = request.GetRequestStream();
            requestStream.Write(encryptedBytes, 0, encryptedBytes.Length);
            requestStream.Close();

            var response = (FtpWebResponse)request.GetResponse();
            response.Close();
            return true;
        }
        public bool DownloadFromFtp(Uri serverUri, string username, string password)
        {
            // The serverUri parameter should start with the ftp:// scheme. 
            if (serverUri.Scheme != Uri.UriSchemeFtp) return false;

            // Get the object used to communicate with the server.            
            var request = new WebClient { Credentials = new NetworkCredential(username, password) };

            try
            {
                byte[] newFileData = request.DownloadData(serverUri.ToString());
                // Decrypt the byte array
                var unencryptedBytes = DecryptObjectFromBytes(newFileData, Vector);
                // Store values from FTP locally.
                Name = unencryptedBytes.Name;
                Applications = unencryptedBytes.Applications;

            }
            catch (WebException e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }

            return true;
        }
    }
}
