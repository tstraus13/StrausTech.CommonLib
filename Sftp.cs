using Renci.SshNet;

namespace StrausTech.CommonLib;

public class Ssh
{
    public string Host { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string PrivateKeyFile { get; set; } = "";
    public string PrivateKeyPassphrase { get; set; } = "";
    public int Port { get; set; } = 0;

    private ConnectionInfo connInfo;
    
    public Ssh(string host, string username, string password, int port = 22)
    {
        Host = host;
        Username = username;
        Password = password;
        Port = port;

        connInfo = new ConnectionInfo(Host, Port, Username, new AuthenticationMethod[] 
        {
            new PasswordAuthenticationMethod(Username, Password)
        });
    }

    public Ssh(string host, string username, string privateKeyFile, string privateKeyPassphrase = "", int port = 22)
    {
        Host = host;
        Username = username;
        Port = port;
        PrivateKeyFile = privateKeyFile;
        PrivateKeyPassphrase = privateKeyPassphrase;

        if (string.IsNullOrEmpty(PrivateKeyPassphrase))
        {
            connInfo = new ConnectionInfo(Host, Port, Username, new AuthenticationMethod[] {
                new PrivateKeyAuthenticationMethod(Username, new PrivateKeyFile[]
                {
                    new PrivateKeyFile(PrivateKeyFile)
                })
            });
        }

        else
        {
            connInfo = new ConnectionInfo(Host, Port, Username, new AuthenticationMethod[] {
                new PrivateKeyAuthenticationMethod(Username, new PrivateKeyFile[]
                {
                    new PrivateKeyFile(PrivateKeyFile, PrivateKeyPassphrase)
                })
            });
        }
    }

    public ReturnDetails ExecuteCommand(string command)
    {
        ReturnDetails details;

        try
        {
            ConfigureConnection();

            using (var client = new SshClient(connInfo))
            {
                client.Connect();

                using (var cmd = client.CreateCommand(command))
                {
                    cmd.Execute();
                    details.Result = cmd.Result;
                    details.Status = cmd.ExitStatus;
                    details.Error = cmd.Error;
                }

                client.Disconnect();
            }
        }
        
        catch (Exception ex)
        {
            throw ex;
        }

        return details;
    }

    private void ConfigureConnection()
    {
        if (!string.IsNullOrEmpty(Password))
        {
            connInfo = new ConnectionInfo(Host, Port, Username, new AuthenticationMethod[]
            {
                new PasswordAuthenticationMethod(Username, Password)
            });
        }

        else
        {
            if (string.IsNullOrEmpty(PrivateKeyPassphrase))
            {
                connInfo = new ConnectionInfo(Host, Port, Username, new AuthenticationMethod[] {
                new PrivateKeyAuthenticationMethod(Username, new PrivateKeyFile[]
                {
                    new PrivateKeyFile(PrivateKeyFile)
                })
            });
            }

            else
            {
                connInfo = new ConnectionInfo(Host, Port, Username, new AuthenticationMethod[] {
                new PrivateKeyAuthenticationMethod(Username, new PrivateKeyFile[]
                {
                    new PrivateKeyFile(PrivateKeyFile, PrivateKeyPassphrase)
                })
            });
            }
        }
    }

    public struct ReturnDetails
    {
        public string Result;
        public int Status;
        public string Error;
    }
}