namespace PizzaDelivery.Application.Options;

public class JwtOptions
{
    public const string OptionName = "Jwt";
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}