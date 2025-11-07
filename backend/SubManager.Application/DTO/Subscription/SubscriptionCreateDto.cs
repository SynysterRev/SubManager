using SubManager.Domain.Validation;
using System.ComponentModel.DataAnnotations;

namespace SubManager.Application.DTO.Subscription
{
    public class SubscriptionCreateDto
    {
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(typeof(decimal), "0", "100000")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 31)]
        public int PaymentDay { get; set; }

        public int? CategoryId { get; set; }
    }
}
