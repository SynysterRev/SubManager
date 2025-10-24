using AutoFixture;
using FluentAssertions;
using JuniorOnly.Application.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SubManager.API.Controllers;
using SubManager.API.Extensions;
using SubManager.Application.DTO.Pagination;
using SubManager.Application.DTO.Subscription;
using SubManager.Application.Exceptions;
using SubManager.Application.Extensions;
using SubManager.Application.Interfaces;
using SubManager.Domain.Entities;
using SubManager.Domain.IdentityEntities;
using SubManager.Domain.Repositories;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Xunit;

namespace SubManager.Tests.Unit.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<ISubscriptionService> _subscriptionServiceMock;
        private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<ILogger<MeController>> _loggerMock;

        private readonly ISubscriptionService _subscriptionService;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ILogger<MeController> _logger;

        private readonly IFixture _fixture;

        public UserControllerTests()
        {
            _fixture = new Fixture();

            _subscriptionServiceMock = new Mock<ISubscriptionService>();
            _subscriptionService = _subscriptionServiceMock.Object;

            _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
            _subscriptionRepository = _subscriptionRepositoryMock.Object;

            var paginationOptions = new PaginationOptions { DefaultPageSize = 10 };
            IOptions<PaginationOptions> options = Options.Create(paginationOptions);

            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _loggerMock = new Mock<ILogger<MeController>>();
            _logger = _loggerMock.Object;
        }

        private void SetUserWithClaims(Controller controller, Guid? userId = null)
        {
            var id = userId ?? Guid.NewGuid();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString())
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task SubList_ShouldReturnViewWithSubscriptionsList()
        {
            var userId = Guid.NewGuid();

            var subscriptions = new List<Subscription>
            {
                new Subscription { Id = 1, Name = "Netflix", Price = 15, IsActive = true, PaymentDay = 5 },
                new Subscription { Id = 2, Name = "Spotify", Price = 10, IsActive = true, PaymentDay = 18 }
            }.AsQueryable();

            MeController userController = new MeController(_subscriptionService, _logger);
            SetUserWithClaims(userController, userId);

            var expectedResponse = new PaginatedSubscriptionsResponse
            {
                Items = subscriptions.Select(s => s.ToDto()).ToList(),
                TotalCostMonth = subscriptions.Sum(s => s.Price),
                TotalCostYear = subscriptions.Sum(s => s.Price) * 12,
                PageIndex = 1,
                TotalPages = 1,
                TotalCount = subscriptions.Count(),
                HasNextPage = false,
                HasPreviousPage = false
            };

            _subscriptionRepositoryMock.Setup(temp => temp.GetAllSubscriptionsByUser(It.IsAny<Guid>())).Returns(subscriptions);

            _subscriptionServiceMock.Setup(temp => temp.GetSubscryptionsByUserAsync(It.IsAny<Guid>(), 1)).ReturnsAsync(expectedResponse);

            var result = await userController.GetSubscriptions(1);

            var okResult = result.Result.As<OkObjectResult>();
            var model = okResult.Value.As<PaginatedSubscriptionsResponse>();

            model.TotalCostMonth.Should().Be(25);
            model.Items?.Should().HaveCount(2);
            model.Items?.Select(s => s.Name).Should().Contain(new[] { "Netflix", "Spotify" });
        }

        [Fact]
        public void GetUserId_ShouldThrowUnauthorized_WhenNoUserClaim()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            Action act = () => claimsPrincipal.GetUserId();

            // Assert
            act.Should()
               .Throw<UnauthorizedAccessException>()
               .WithMessage("User not authentified");
        }

        [Fact]
        public async Task SubList_UserNotAuth_ShouldReturnUnauthorizedAccessException()
        {
            var userId = Guid.NewGuid();

            MeController userController = new MeController(_subscriptionService, _logger);
            userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // no user
            };

            Func<Task> action = async () => await userController.GetSubscriptions(1);

            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task CreateSub_ShouldReturn201Created()
        {
            var userId = Guid.NewGuid();

            MeController userController = new MeController(_subscriptionService, _logger);
            SetUserWithClaims(userController, userId);

            var subcriptionCreate = _fixture.Build<SubscriptionCreateDto>()
                .With(temp => temp.PaymentDay, 15)
                .With(temp => temp.Price, 15m)
                .Create();

            var subscription = subcriptionCreate.ToEntity();
            var expectedResponse = subscription.ToDto();

            _subscriptionRepositoryMock.Setup(temp => temp.AddSubscriptionAsync(It.IsAny<Subscription>())).ReturnsAsync(subscription);

            _subscriptionServiceMock.Setup(temp => temp.CreateSubscriptionAsync(subcriptionCreate, userId)).ReturnsAsync(expectedResponse);

            var result = await userController.CreateSubscription(subcriptionCreate);
            var createdResult = result.Result.As<CreatedAtActionResult>();
            createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
            var model = createdResult.Value.As<SubscriptionDto>();

            model.Should().BeEquivalentTo(expectedResponse, options => options
                            .Excluding(x => x.Id));

            _subscriptionServiceMock.Verify(
        s => s.CreateSubscriptionAsync(subcriptionCreate, userId),
        Times.Once);
        }

        [Fact]
        public async Task CreateSubscription_ShouldReturnBadRequest_WhenModelInvalid()
        {
            var userId = Guid.NewGuid();
            var invalidDto = new SubscriptionCreateDto();
            var controller = new MeController(_subscriptionServiceMock.Object, _logger);
            SetUserWithClaims(controller, userId);

            controller.ModelState.AddModelError("Name", "Required");

            _subscriptionServiceMock
            .Setup(s => s.CreateSubscriptionAsync(invalidDto, userId))
            .ThrowsAsync(new ValidationException());

            Func<Task> act = async () => await controller.CreateSubscription(invalidDto);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task GetSubscriptionDetails_ShouldReturnOkWithSubscription()
        {
            var userId = Guid.NewGuid();
            var subId = 1;
            var expectedSubscription = new SubscriptionDto { Id = subId, Name = "Netflix", Price = 15 };

            _subscriptionServiceMock
                .Setup(s => s.GetSubscriptionByIdAsync(subId, userId))
                .ReturnsAsync(expectedSubscription);

            var controller = new MeController(_subscriptionServiceMock.Object, _logger);
            SetUserWithClaims(controller, userId);

            var result = await controller.GetSubscriptionDetails(subId);

            var okResult = result.Result.As<OkObjectResult>();
            okResult.Value.Should().BeEquivalentTo(expectedSubscription);

            _subscriptionServiceMock.Verify(s => s.GetSubscriptionByIdAsync(subId, userId), Times.Once);
        }

        [Fact]
        public async Task GetSubscriptionDetails_ShouldReturn404_WhenSubscriptionNotFound()
        {
            var userId = Guid.NewGuid();
            var subId = 999;

            _subscriptionServiceMock
                .Setup(s => s.GetSubscriptionByIdAsync(subId, userId))
                .ThrowsAsync(new NotFoundException($"Subscription {subId} not found"));

            var controller = new MeController(_subscriptionServiceMock.Object, _logger);
            SetUserWithClaims(controller, userId);

            Func<Task> act = async () => await controller.GetSubscriptionDetails(subId);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Subscription {subId} not found");
        }

        [Fact]
        public async Task UpdateSubscription_ShouldReturnOkWithUpdatedSubscription()
        {
            var userId = Guid.NewGuid();
            var subId = 1;
            var subscriptionUpdate = new SubscriptionUpdateDto { Price = 20 };
            var updatedSubscription = new SubscriptionDto { Id = subId, Name = "Netflix", Price = 20 };

            _subscriptionServiceMock
                .Setup(s => s.UpdateSubscriptionAsync(subId, subscriptionUpdate, userId))
                .ReturnsAsync(updatedSubscription);

            var controller = new MeController(_subscriptionServiceMock.Object, _logger);
            SetUserWithClaims(controller, userId);

            var result = await controller.UpdateSubscription(subId, subscriptionUpdate);

            var okResult = result.Result.As<OkObjectResult>();
            okResult.Value.Should().BeEquivalentTo(updatedSubscription);

            _subscriptionServiceMock.Verify(s => s.UpdateSubscriptionAsync(subId, subscriptionUpdate, userId), Times.Once);
        }

        [Fact]
        public async Task UpdateSubscription_ShouldThrowNotFound_WhenSubscriptionNotExist()
        {
            var userId = Guid.NewGuid();
            var subId = 999;
            var updateDto = new SubscriptionUpdateDto { Price = 20 };

            _subscriptionServiceMock
                .Setup(s => s.UpdateSubscriptionAsync(subId, updateDto, userId))
                .ThrowsAsync(new NotFoundException($"Subscription {subId} not found"));

            var controller = new MeController(_subscriptionServiceMock.Object, _logger);
            SetUserWithClaims(controller, userId);

            Func<Task> act = async () => await controller.UpdateSubscription(subId, updateDto);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Subscription {subId} not found");
        }

        [Fact]
        public async Task DeleteSubscription_ShouldReturnNoContent()
        {
            var userId = Guid.NewGuid();
            var subId = 1;

            _subscriptionServiceMock
                .Setup(s => s.DeleteSubscriptionAsync(subId, userId))
                .Returns(Task.CompletedTask);

            var controller = new MeController(_subscriptionServiceMock.Object, _logger);
            SetUserWithClaims(controller, userId);

            var result = await controller.DeleteSubscription(subId);

            result.Should().BeOfType<NoContentResult>();

            _subscriptionServiceMock.Verify(s => s.DeleteSubscriptionAsync(subId, userId), Times.Once);
        }

        [Fact]
        public async Task DeleteSubscription_ShouldThrowNotFound_WhenSubscriptionNotExist()
        {
            var userId = Guid.NewGuid();
            var subId = 999;

            _subscriptionServiceMock
                .Setup(s => s.DeleteSubscriptionAsync(subId, userId))
                .ThrowsAsync(new NotFoundException($"Subscription {subId} not found"));

            var controller = new MeController(_subscriptionServiceMock.Object, _logger);
            SetUserWithClaims(controller, userId);

            Func<Task> act = async () => await controller.DeleteSubscription(subId);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Subscription {subId} not found");
        }
    }
}
