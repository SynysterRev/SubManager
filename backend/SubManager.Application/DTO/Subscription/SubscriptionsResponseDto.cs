using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubManager.Application.DTO.Subscription
{
    public class SubscriptionsResponseDto
    {
        public List<SubscriptionDto> Subscriptions { get; set; } = new List<SubscriptionDto>();
        public float TotalCost { get; set; }
    }
}
