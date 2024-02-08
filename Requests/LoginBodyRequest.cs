namespace plant_ecommerce_server.Requests
{
    public class LoginBodyRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}