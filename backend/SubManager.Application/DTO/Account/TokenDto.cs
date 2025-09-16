namespace SubManager.Application.DTO.Account
{
    public class TokenDto
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public bool IsPremium { get; set; }
    }
}
