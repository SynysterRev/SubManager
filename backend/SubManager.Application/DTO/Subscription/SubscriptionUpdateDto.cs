using System.ComponentModel.DataAnnotations;

namespace SubManager.Application.DTO.Subscription
{
    public class SubscriptionUpdateDto
    {
        [StringLength(150, MinimumLength = 3)]
        public string? Name { get; set; }
        public int? CategoryId { get; set; }

        [Range(typeof(decimal), "0", "100000")]
        public decimal? Price { get; set; }

        [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Invalid currency code")]
        public string? CurrencyCode { get; set; }
        public bool? IsActive { get; set; }

        [Range(1, 31)]
        public int? PaymentDay { get; set; }
    }
}
