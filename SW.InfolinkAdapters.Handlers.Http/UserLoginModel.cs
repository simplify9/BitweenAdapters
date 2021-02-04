namespace SW.InfolinkAdapters.Handlers.Http
{
    public class UserLoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Jwt { get; set; }
        public string RefreshToken { get; set; }
    }
}