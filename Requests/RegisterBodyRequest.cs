namespace plant_ecommerce_server.Requests
{
    public class RegisterBodyRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}