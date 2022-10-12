using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Configuration;
using FirebirdSql.Data.Services;

namespace DataLibrary;

public class EmailService : IEmailService
{
    private readonly string eMailUsr;
    private readonly string eMailPwd;

    public EmailService(IConfiguration config)
    {
        //Kullanildigi Project user secret den alinir.
        //eMailUsr = config.GetConnectionString("eMailUsr");
        //eMailPwd = config.GetConnectionString("eMailPwd");
        eMailUsr = config["MailKit:eMailUsr"];
        eMailPwd = config["MailKit:eMailPwd"];
    }

    //private async Task SendEmailAsync(string eMailTo, string eSubject, string eBody)
    public async Task SendEmailAsync(string eMailTo, string eSubject, string eBody)
    {
        //string eMailTo = "ismael.wiza44@ethereal.email";
        //string eSubject = "Deneme";
        //string eBody = "<h1>DENEME</h1>";
        //string host = "smtp.ethereal.email";
        //int port = 587;

        string host = "smtp.gmail.com";
        int port = 587; // Tls (465 for Ssl)

        MimeMessage msg = new MimeMessage();
        msg.From.Add(new MailboxAddress("LinkNoNet", "sener.demiral@gmail.com"));
        msg.To.Add(MailboxAddress.Parse(eMailTo));

        msg.Subject = eSubject;
        msg.Body = new TextPart(TextFormat.Html)
        {
            Text = eBody
        };

        var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls)
                .ConfigureAwait(false);

            // Note: since we don't have an OAuth2 token, disable the XOAUTH2 authentication mechanism.
            client.AuthenticationMechanisms.Remove("XOAUTH2");

            // Note: only needed if the SMTP server requires authentication.
            await client.AuthenticateAsync(eMailUsr, eMailPwd)
                    .ConfigureAwait(false);

            await client.SendAsync(msg).ConfigureAwait(false);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            await client.DisconnectAsync(true).ConfigureAwait(false);
            client.Dispose();
        }
    }
}
