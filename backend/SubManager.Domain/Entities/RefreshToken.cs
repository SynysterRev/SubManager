using SubManager.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubManager.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime Expires { get; set; }

        public DateTime RevokedAt { get; set; }

        public bool IsRevoked { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}
