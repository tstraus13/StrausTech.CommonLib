using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace StrausTech.CommonLib;
/// <summary>
/// Class to use the sFTP protocol to transfile files to
/// and from servers
/// </summary>
public class Sftp
{
    private SftpClient client;
    private string host;
    private string username;
    private string password;
    private int port;
    private string localPath;
    private string remotePath;
    private string privateKeyFile;

    /// <summary>
    /// The local path
    /// </summary>
    public string LocalPath
    {
        get { return localPath; }
        set { localPath = Tools.PathRemoveTrailingSlash(value); }
    }

    /// <summary>
    /// The local file name
    /// </summary>
    public string LocalFilename { get; set; }

    /// <summary>
    /// The remote server path
    /// </summary>
    public string RemotePath
    {
        get { return remotePath; }
        set { remotePath = Tools.PathRemoveTrailingSlash(value); }
    }

    /// <summary>
    /// The remote file name
    /// </summary>
    public string RemoteFilename { get; set; }

    /// <summary>
    /// The Private Key File path
    /// </summary>
    public string PrivateKeyFile
    {
        get { return privateKeyFile; }
        set { privateKeyFile = Tools.PathRemoveTrailingSlash(value); }
    }

    /// <summary>
    /// Initializes the class with the following parameters set
    /// </summary>
    /// <param name="host">The server to connect to</param>
    /// <param name="username">The username to connect with</param>
    /// <param name="password">The password for the username provided</param>
    /// <param name="port">The port to connect through</param>
    public Sftp(string host, string username, string password = null, int port = 22)
    {
        this.host = host;
        this.username = username;
        this.password = password;
        this.port = port;
    }

    /// <summary>
    /// Retrieves a file from the remote server
    /// </summary>
    public void Get()
    {
        Connect();

        try
        {
            using (Stream fileStream = File.OpenWrite($@"{LocalPath}\{LocalFilename}"))
            {
                client.DownloadFile($@"{RemotePath}/{RemoteFilename}", fileStream);
            }
        }

        catch (Exception ex)
        {
            throw ex;
        }

        Disconnect();
    }

    /// <summary>
    /// Places a file on the remote server
    /// </summary>
    public void Put()
    {
        Connect();

        try
        {
            using (Stream fileStream = File.OpenRead($@"{LocalPath}\{LocalFilename}"))
            {
                client.UploadFile(fileStream, $@"{RemotePath}/{RemoteFilename}");
            }
        }

        catch (Exception ex)
        {
            throw ex;
        }

        Disconnect();
    }

    /// <summary>
    /// Deletes a file on the remote server
    /// </summary>
    public void Delete()
    {
        Connect();

        try { client.Delete($@"{RemotePath}/{RemoteFilename}"); }
        catch (Exception ex) { throw ex; }

        Disconnect();
    }

    /// <summary>
    /// Retrieves the directory listing on the remote server
    /// </summary>
    /// <returns>A list of files with details</returns>
    public List<SftpFileDetails> ListDirectory()
    {
        Connect();

        IEnumerable<SftpFile> ifiles;
        List<SftpFileDetails> lfiles = new List<SftpFileDetails>();

        try
        {
            ifiles = client.ListDirectory($@"{RemotePath}");

            foreach (SftpFile file in ifiles)
            {
                SftpFileDetails lfile = new SftpFileDetails();
                lfile.FullName = file.FullName;
                lfile.Modified = file.Attributes.LastWriteTime;
                lfile.Name = file.Name;
                lfile.Size = file.Attributes.Size;

                lfiles.Add(lfile);
            }
        }

        catch (Exception ex)
        {
            throw ex;
        }

        Disconnect();

        return lfiles;
    }

    /// <summary>
    /// Establishes a connection to the remote server
    /// </summary>
    private void Connect()
    {
        try
        {
            ConnectionInfo connection;

            if (string.IsNullOrEmpty(PrivateKeyFile))
            {
                connection = new ConnectionInfo(host, port, username, authenticationMethods: new PasswordAuthenticationMethod(username, password));
            }

            else
            {
                connection = new ConnectionInfo(host, port, username, authenticationMethods: new PrivateKeyAuthenticationMethod(username, new PrivateKeyFile[] { new PrivateKeyFile(privateKeyFile) }));
            }

            client = new SftpClient(connection);

            client.Connect();

            if (!client.IsConnected)
            {
                throw new Exception($"SFTP Connection Error - Could not connect to {host} on {port} for user {username}.");
            }
        }

        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Releases connection to remote server
    /// </summary>
    private void Disconnect()
    {
        try
        {
            client.Disconnect();
        }

        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Struct based of the SftpFile data type from SSH.NET library
    /// </summary>
    public struct SftpFileDetails
    {
        public DateTime Modified;
        public string FullName;
        public string Name;
        public long Size;
    }
}
