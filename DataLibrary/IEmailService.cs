namespace DataLibrary;

public interface IEmailService
{
    Task SendEmailAsync(string eMailTo, string eSubject, string eBody);
}