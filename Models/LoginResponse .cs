
namespace TaskManagerAPI.Models
{

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;

        public string TokenType { get; set; } = "Bearer";

        public DateTime ExpiresAt { get; set; }

        public UserDto User { get; set; } = null!;
    }
}