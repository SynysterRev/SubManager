using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubManager.API.Extensions;
using SubManager.Application.DTO.Pagination;
using SubManager.Application.DTO.Subscription;
using SubManager.Application.Interfaces;

namespace SubManager.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/subscriptions")]
    public class MeController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<MeController> _logger;

        public MeController(ISubscriptionService subscriptionService, ILogger<MeController> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedSubscriptionsResponse>> GetSubscriptions([FromQuery] int pageNumber = 1)
        {
            var userId = User.GetUserId();

            var subscriptions = await _subscriptionService.GetSubscryptionsByUserAsync(userId, pageNumber);
            _logger.LogInformation("Get all subscriptions for user {id}", userId.ToString());
            return Ok(subscriptions);
        }

        [HttpPost]
        public async Task<ActionResult<SubscriptionDto>> CreateSubscription(SubscriptionCreateDto subscriptionCreate)
        {
            var userId = User.GetUserId();

            var newSub = await _subscriptionService.CreateSubscriptionAsync(subscriptionCreate, userId);
            _logger.LogInformation("Subscription created with ID {subID} for user {id}", newSub.Id, userId.ToString());
            return CreatedAtAction(nameof(GetSubscriptionDetails), new { subId = newSub.Id }, newSub);
        }

        [HttpGet("{subId}")]
        public async Task<ActionResult<SubscriptionDto>> GetSubscriptionDetails(int subId)
        {
            var userId = User.GetUserId();

            var subscriptions = await _subscriptionService.GetSubscriptionByIdAsync(subId, userId);
            _logger.LogInformation("Get subscription {subId} for user {id}", subId, userId.ToString());
            return Ok(subscriptions);
        }

        [HttpPut("{subId}")]
        public async Task<ActionResult<SubscriptionDto>> UpdateSubscription(int subId, SubscriptionUpdateDto subscriptionUpdate)
        {
            var userId = User.GetUserId();

            var updatedSub = await _subscriptionService.UpdateSubscriptionAsync(subId, subscriptionUpdate, userId);
            return Ok(updatedSub);
        }

        [HttpDelete("{subId}")]
        public async Task<IActionResult> DeleteSubscription(int subId)
        {
            var userId = User.GetUserId();

            await _subscriptionService.DeleteSubscriptionAsync(subId, userId);
            _logger.LogInformation("Subscription deleted with ID {subId} for user {userId}", subId, userId);

            return NoContent();
        }
    }
}
