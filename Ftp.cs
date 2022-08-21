using FluentFTP;

namespace CommonLib;

/// <summary>
/// Class to use the FTP protocol to transfile files to
/// and from servers
/// </summary>
public class Ftp
{
    /// <summary>
    /// The Ftp client to use from FluentFTP
    /// </summary>
    private FtpClient client = new FtpClient();
    /// <summary>
    /// The local path of where on the local machine files are located
    /// </summary>
    private string localPath = "";
    /// <summary>
    /// The remote path of where on the server the files is located
    /// </summary>
    private string remotePath = "";

    /// <summary>
    /// The server/host address of the remote server
    /// </summary>
    public string Host
    {
        get { return client.Host; }
        set { client.Host = value; }
    }

    /// <summary>
    /// The username on the remote system to login with
    /// </summary>
    public string Username
    {
        get { return client.Credentials.UserName; }
        set { client.Credentials.UserName = value; }
    }

    /// <summary>
    /// The password on the remote system to login with
    /// </summary>
    public string Password
    {
        get { return client.Credentials.Password; }
        set { client.Credentials.Password = value; }
    }

    /// <summary>
    /// The port the remote server in which the FTP server is running
    /// </summary>
    public int Port
    {
        get { return client.Port; }
        set { client.Port = value; }
    }

    /// <summary>
    /// The number of times to attempt to retry in the even of a
    /// failed FTP transfer operation
    /// </summary>
    public int RetryAttempts
    {
        get { return client.RetryAttempts; }
        set { client.RetryAttempts = value; }
    }

    /// <summary>
    /// Set the local path and remove the trailing slash if provided
    /// </summary>
    public string LocalPath
    {
        get { return localPath; }
        set { localPath = Tools.PathRemoveTrailingSlash(value); }
    }

    /// <summary>
    /// The name of the local file
    /// Example: text.txt
    /// </summary>
    public string LocalFilename { get; set; } = "";

    /// <summary>
    /// Set the remote path and remove the trailing slash if provided
    /// </summary>
    public string RemotePath
    {
        get { return remotePath; }
        set { remotePath = Tools.PathRemoveTrailingSlash(value); }
    }

    /// <summary>
    /// The name of the remote file
    /// Example: text.txt
    /// </summary>
    public string RemoteFilename { get; set; } = "";

    /// <summary>
    /// Set the encryption mode of the FTP connection
    /// </summary>
    public EncryptionModes EncryptionMode
    {
        get { return (EncryptionModes) client.EncryptionMode; }
        set { client.EncryptionMode = (FtpEncryptionMode) value; }
    }
    
    /// <summary>
    /// The public enumeration of the different Encryption Modes
    /// taken from FluentFTP
    /// </summary>
    public enum EncryptionModes
    {
        Implicit = FtpEncryptionMode.Implicit,
        Explicit = FtpEncryptionMode.Explicit,
        None = FtpEncryptionMode.None
    }

    /// <summary>
    /// Get a file from the remote server
    /// </summary>
    /// <returns>Whether the retreival was successful or not</returns>
    public bool Get()
    {
        try
        {
            Connect();

            FtpStatus result = client.DownloadFile($@"{LocalPath}\{LocalFilename}", $@"{RemotePath}/{RemoteFilename}", existsMode: FtpLocalExists.Skip, verifyOptions: FtpVerify.Retry);

            if (result == FtpStatus.Failed)
            {
                client.Disconnect();
                return false;
            }

            Disconnect();
            return true;
        }
        
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Place a file on the remote server
    /// </summary>
    /// <returns>Whether the transfer was successful or not</returns>
    public bool Put()
    {
        try
        {
            Connect();

            FtpStatus result = client.UploadFile($@"{LocalPath}\{LocalFilename}", $@"{RemotePath}/{RemoteFilename}", existsMode: FtpRemoteExists.Skip, verifyOptions: FtpVerify.Retry);

            if (result == FtpStatus.Failed)
            {
                client.Disconnect();
                return false;
            }

            Disconnect();
            return true;
        }
        
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Delete a file off the remote server
    /// </summary>
    /// <returns>Whether the deletion was successful or not</returns>
    public bool Delete()
    {
        try
        {
            Connect();

            client.DeleteFile($@"{RemotePath}/{RemoteFilename}");

            if (client.FileExists($@"{RemotePath}/{RemoteFilename}"))
                return false;

            return true;
        }
        
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Get the list of files/folder on the current directory
    /// on the remote server
    /// </summary>
    /// <returns>The list of files/folders</returns>
    public List<FtpFileDetails> ListDirectory()
    {
        try
        {
            Connect();

            List<FtpFileDetails> lfiles = new List<FtpFileDetails>();

            var afiles = client.GetListing(RemotePath);

            foreach (FtpListItem file in afiles)
            {
                FtpFileDetails lfile = new FtpFileDetails();
                lfile.FullName = file.FullName;
                lfile.Created = file.Created;
                lfile.Modified = file.Modified;
                lfile.Name = file.Name;
                lfile.Size = file.Size;

                lfiles.Add(lfile);
            }

            return lfiles;
        }
        
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Connect to the remote server
    /// </summary>
    /// <returns></returns>
    private void Connect()
    {
        client.ValidateCertificate += new FtpSslValidation(OnValidateCert);
        client.Connect();

        if (!client.IsConnected)
        {
            client.Disconnect();
        }
    }

    /// <summary>
    /// Disconnect from the remote server
    /// </summary>
    private void Disconnect()
    {
        client.Disconnect();
    }

    /// <summary>
    /// Accept the certificate that the server is providing
    /// </summary>
    /// <param name="control">The FTP client</param>
    /// <param name="e">FTP SSL Validtion Event arguments</param>
    private void OnValidateCert(FtpClient control, FtpSslValidationEventArgs e)
    {
        e.Accept = true;
    }

    /// <summary>
    /// Struct based off the FtpListItem from FluentFTP library
    /// </summary>
    public struct FtpFileDetails
    {
        public DateTime Created;
        public DateTime Modified;
        public string FullName;
        public string Name;
        public long Size;
    }
}