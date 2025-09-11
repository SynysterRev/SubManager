using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubManager.Application.DTO.Subscription
{
    public class SubscriptionCreateDto
    {
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        public float Price { get; set; }

        [Required]
        [Range(1, 31)]
        public int PaymentDay { get; set; }

        public string Category { get; set; }
    }
}
