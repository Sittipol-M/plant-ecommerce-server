namespace plant_ecommerce_server.Responses
{
    public class RefreshTokenResponse
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }

    }
}