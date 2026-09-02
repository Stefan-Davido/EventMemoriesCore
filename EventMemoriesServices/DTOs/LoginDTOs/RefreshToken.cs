using System.ComponentModel.DataAnnotations;

namespace EventMemoriesServices.DTOs
{
    /// <summary>
    /// DTO for refresh token request
    /// </summary>
    public class RefreshToken
    {
        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; } = string.Empty;
    }
}
