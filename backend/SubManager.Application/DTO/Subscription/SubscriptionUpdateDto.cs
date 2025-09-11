using System.ComponentModel.DataAnnotations;

namespace SubManager.Application.DTO.Subscription
{
    public class SubscriptionUpdateDto
    {
        [StringLength(150, MinimumLength = 3)]
        public string? Name { get; set; }
        public string? Category { get; set; }

        [Range(0, float.MaxValue)]
        public float? Price { get; set; }
        public bool? IsActive { get; set; }

        [Range(1, 31)]
        public int? PaymentDay { get; set; }
    }
}
