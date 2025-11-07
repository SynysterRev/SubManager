using SubManager.Domain.IdentityEntities;
using SubManager.Domain.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubManager.Domain.Entities
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public required string Name { get; set; }

        [ForeignKey(nameof(Category))]
        public int? CategoryId { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "100000")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 31)]
        public int PaymentDay { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Category? Category { get; set; }
    }
}
