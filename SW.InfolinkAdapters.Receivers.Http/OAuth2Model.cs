namespace SW.InfolinkAdapters.Receivers.Http;

public class OAuth2Response
{
    public string access_token { get; set; }
}
public class UserLoginModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
}


public class LoginResponse
{
    public string Jwt { get; set; }
    public string Refresh { get; set; }
}
