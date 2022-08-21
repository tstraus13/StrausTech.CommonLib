using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net.Mime;

namespace StrausTech.CommonLib;

/// <summary>
/// Class for sending emails out in applications or
/// different processes
/// </summary>
public class Email
{
    /// <summary>
    /// The address of who the email is coming from
    /// </summary>
    public string FromAddress { get; set; } = "";
    /// <summary>
    /// The list of addresses to send the email message to
    /// </summary>
    public List<string> ToAddresses { get; set; } = new List<string>();
    /// <summary>
    /// The list of CC addresses to send the email message to
    /// </summary>
    public List<string> CcAddresses { get; set; } = new List<string>();
    /// <summary>
    /// The list of BCC addresses to send the email message to
    /// </summary>
    public List<string> BccAddresses { get; set; } = new List<string>();
    /// <summary>
    /// The list of Attachments to add to the email
    /// </summary>
    public List<string> Attachments { get; set; } = new List<string>();
    /// <summary>
    /// 
    /// </summary>
    public Dictionary<Stream, ContentType> AttachmentStreams { get; set; } = new Dictionary<Stream, ContentType>();
    /// <summary>
    /// The subject of the email message
    /// </summary>
    public string Subject { get; set; } = "";
    /// <summary>
    /// The body of the email message
    /// </summary>
    public string Body { get; set; } = "";
    /// <summary>
    /// Is the email message written in html?
    /// </summary>
    public bool IsHtml { get; set; } = false;

    /// <summary>
    /// The email server constant
    /// </summary>
    private readonly string Server = "";
    /// <summary>
    /// The email server port constant
    /// </summary>
    private readonly int Port = 0;

    public Email(string server, int port)
    {
        Server = server;
        Port = port;
    }

    /// <summary>
    /// Send the email message out through the Email Server
    /// </summary>
    public void Send()
    {
        try
        {
            using (SmtpClient client = new SmtpClient())
            {

                client.Host = Server;
                client.Port = Port;
                client.UseDefaultCredentials = false;

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(FromAddress);
                    mail.Subject = Subject;
                    mail.Body = Body;
                    mail.IsBodyHtml = IsHtml;

                    foreach (string address in ToAddresses)
                    {
                        mail.To.Add(address);
                    }

                    foreach (string address in CcAddresses)
                    {
                        mail.CC.Add(address);
                    }

                    foreach (string address in BccAddresses)
                    {
                        mail.Bcc.Add(address);
                    }

                    foreach (string attchment in Attachments)
                    {
                        mail.Attachments.Add(new Attachment(attchment));
                    }

                    foreach (KeyValuePair<Stream, ContentType> stream in AttachmentStreams)
                    {
                        mail.Attachments.Add(new Attachment(stream.Key, stream.Value));
                    }

                    client.Send(mail);
                }
            }
        }
        
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Determines whether a given email address
    /// is valid or not
    /// </summary>
    /// <param name="email">The email address to validate</param>
    /// <returns>True if the email address is valid and false if not</returns>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        return new EmailAddressAttribute().IsValid(email);
    }
}