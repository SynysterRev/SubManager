using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubManager.Application.DTO.Subscription
{
    public class SubscriptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal YearCost { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int DaysBeforeNextPayment { get; set; }
        public DateTime PaymentDate { get; set; }
        public Guid UserId { get; set; }
    }
}
