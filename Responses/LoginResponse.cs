
namespace plant_ecommerce_server.Responses
{
    public class LoginUser
    {
        public required string Username { get; set; }
        public string? Role { get; set; }
    }
    public class LoginResponse
    {
        public required LoginUser User { get; set; }
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }

    }

}