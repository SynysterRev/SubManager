using Microsoft.AspNetCore.Identity;
using SubManager.Domain.Entities;
using SubManager.Domain.Validation;
using System.ComponentModel.DataAnnotations;

namespace SubManager.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public bool IsPremium { get; set; }

        [Required]
        [ValidCurrency]
        [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Invalid currency code")]
        public string Currency { get; set; } = "EUR";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
