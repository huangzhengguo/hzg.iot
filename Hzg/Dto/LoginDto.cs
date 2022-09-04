namespace Ledinpro.Iot.Dto;

public class LoginDto
{
    public string Email
    {
        get
        {
            return Principal.Substring(Principal.IndexOf("@") + 1);
        }
    }
    public string Principal { get; set; }
    public string Credentials { get; set; }
}