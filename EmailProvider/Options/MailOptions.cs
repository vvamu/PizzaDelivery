namespace EmailProvider.Options;

public class MailOptions
{
    public const string OptionName = "EmailSettings";
    public string Email { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public string Displayname { get; set; }
    public int Port { get; set; }
}
