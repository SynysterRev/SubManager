using AutoFixture;
using FluentAssertions;
using JuniorOnly.Application.Commons;
using Microsoft.Extensions.Options;
using MockQueryable;
using Moq;
using SubManager.Application.DTO.Subscription;
using SubManager.Application.Exceptions;
using SubManager.Application.Extensions;
using SubManager.Application.Interfaces;
using SubManager.Application.Services;
using SubManager.Domain.Entities;
using SubManager.Domain.Repositories;
using System;
using System.ComponentModel.DataAnnotations;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SubManager.Tests.Unit.Services
{
    public class SubscriptionServiceTests
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
        private readonly IFixture _fixture;

        public SubscriptionServiceTests()
        {
            _fixture = new Fixture();

            _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();

            _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var paginationOptions = new PaginationOptions { DefaultPageSize = 10 };
            IOptions<PaginationOptions> options = Options.Create(paginationOptions);

            _subscriptionService = new SubscriptionService(_subscriptionRepositoryMock.Object, options);
        }

        [Fact]
        public async Task CreateSubscriptionAsync_ShouldThrowValidationException_InvalidPrice()
        {
            var userId = Guid.NewGuid();
            var subcriptionCreate = _fixture.Build<SubscriptionCreateDto>()
                .With(temp => temp.Price, -1f)
                .Create();
            Func<Task> action = async () =>
            {
                await _subscriptionService.CreateSubscriptionAsync(subcriptionCreate, userId);
            };
            await action.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task CreateSubscriptionAsync_ShouldThrowValidationException_InvalidPaymentDay()
        {
            var userId = Guid.NewGuid();
            var subcriptionCreate = _fixture.Build<SubscriptionCreateDto>()
                .With(temp => temp.PaymentDay, 34)
                .Create();
            Func<Task> action = async () =>
            {
                await _subscriptionService.CreateSubscriptionAsync(subcriptionCreate, userId);
            };
            await action.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task CreateSubscriptionAsync_ShouldBeSuccessful()
        {
            var userId = Guid.NewGuid();
            var subcriptionCreate = _fixture.Build<SubscriptionCreateDto>()
                .With(temp => temp.PaymentDay, 15)
                .With(temp => temp.Price, 15f)
                .Create();

            var subEntity = subcriptionCreate.ToEntity();
            _subscriptionRepositoryMock.Setup(s => s.AddSubscriptionAsync(It.IsAny<Subscription>())).ReturnsAsync(subEntity);
            var expectedSub = subEntity.ToDto();
            var subscription = await _subscriptionService.CreateSubscriptionAsync(subcriptionCreate, userId);
            expectedSub.Id = subscription.Id;

            subscription.Should().NotBeNull();
            subscription.Should().BeEquivalentTo(expectedSub, options => options
            .Excluding(s => s.CreatedAt));
        }

        [Fact]
        public async Task UpdateSubscriptionAsync_ShouldThrowValidationException_InvalidPrice()
        {
            var subscription = new Subscription
            {
                Id = 1,
                UserId = Guid.NewGuid(),
                Name = "Netflix",
                Price = 9.99f,
                PaymentDay = 5,
                Category = "Streaming"
            };

            var subcriptionUpdate = _fixture.Build<SubscriptionUpdateDto>()
                .With(temp => temp.Price, -1f)
                .Create();

            _subscriptionRepositoryMock.Setup(s => s.GetSubscriptionByIdAsync(It.IsAny<int>())).ReturnsAsync(subscription);

            Func<Task> action = async () =>
            {
                await _subscriptionService.UpdateSubscriptionAsync(subscription.Id, subcriptionUpdate, subscription.UserId);
            };
            await action.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UpdateSubscriptionAsync_ShouldThrowValidationException_InvalidPaymentDay()
        {
            var subscription = new Subscription
            {
                Id = 1,
                UserId = Guid.NewGuid(),
                Name = "Netflix",
                Price = 9.99f,
                PaymentDay = 5,
                Category = "Streaming"
            };

            var subcriptionUpdate = _fixture.Build<SubscriptionUpdateDto>()
                .With(temp => temp.PaymentDay, 32)
                .Create();
            _subscriptionRepositoryMock.Setup(s => s.GetSubscriptionByIdAsync(It.IsAny<int>())).ReturnsAsync(subscription);

            Func<Task> action = async () =>
            {
                await _subscriptionService.UpdateSubscriptionAsync(subscription.Id, subcriptionUpdate, subscription.UserId);
            };
            await action.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UpdateSubscriptionAsync_ShouldThrowNotFoundException_InvalidId()
        {
            var subscription = new Subscription
            {
                Id = 1,
                UserId = Guid.NewGuid(),
                Name = "Netflix",
                Price = 9.99f,
                PaymentDay = 5,
                Category = "Streaming"
            };

            var subcriptionUpdate = _fixture.Build<SubscriptionUpdateDto>()
                .Create();

            _subscriptionRepositoryMock.Setup(s => s.GetSubscriptionByIdAsync(It.IsAny<int>())).ReturnsAsync(null as Subscription);

            Func<Task> action = async () =>
            {
                await _subscriptionService.UpdateSubscriptionAsync(subscription.Id, subcriptionUpdate, subscription.UserId);
            };
            await action.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task UpdateSubscriptionAsync_ShouldThrowUnauthorizedAccessException_InvalidUserId()
        {
            var subscription = new Subscription
            {
                Id = 1,
                UserId = Guid.NewGuid(),
                Name = "Netflix",
                Price = 9.99f,
                PaymentDay = 5,
                Category = "Streaming"
            };

            var subcriptionUpdate = _fixture.Build<SubscriptionUpdateDto>()
                .Create();

            _subscriptionRepositoryMock.Setup(s => s.GetSubscriptionByIdAsync(It.IsAny<int>())).ReturnsAsync(subscription);

            Func<Task> action = async () =>
            {
                await _subscriptionService.UpdateSubscriptionAsync(subscription.Id, subcriptionUpdate, Guid.NewGuid());
            };
            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task UpdateSubscriptionAsync_ShouldBeSuccessful()
        {
            var baseSub = new Subscription
            {
                Id = 1,
                UserId = Guid.NewGuid(),
                Name = "Netflix",
                Price = 9.99f,
                PaymentDay = 5,
                Category = "Streaming"
            };

            var subcriptionUpdate = _fixture.Build<SubscriptionUpdateDto>()
                .With(temp => temp.PaymentDay, 18)
                .With(temp => temp.Name, "Amazon")
                .With(temp => temp.Price, 9.99f)
                .With(temp => temp.Category, "Streaming")
                .Create();

            _subscriptionRepositoryMock.Setup(s => s.GetSubscriptionByIdAsync(It.IsAny<int>())).ReturnsAsync(baseSub);

            //_subscriptionRepositoryMock.Setup(s => s.UpdateSubscriptionAsync(It.IsAny<Subscription>())).ReturnsAsync((Subscription s) => s);

            var updatedSubscription = await _subscriptionService.UpdateSubscriptionAsync(baseSub.Id, subcriptionUpdate, baseSub.UserId);
            baseSub.PaymentDay = 18;
            var expectedSub = baseSub.ToDto();
            expectedSub.Name = "Amazon";

            updatedSubscription.Should().NotBeNull();
            updatedSubscription.Should().BeEquivalentTo(expectedSub, options => options
            .Excluding(s => s.CreatedAt));
        }

        [Fact]
        public async Task DeleteSubscriptionAsync_ShouldThrowNotFoundException_InvalidId()
        {
            var baseSub = new Subscription
            {
                Id = 1,
                UserId = Guid.NewGuid(),
                Name = "Netflix",
                Price = 9.99f,
                PaymentDay = 5,
                Category = "Streaming"
            };

            _subscriptionRepositoryMock.Setup(s => s.GetSubscriptionByIdAsync(It.IsAny<int>())).ReturnsAsync(null as Subscription);
            Func<Task> action = async () => await _subscriptionService.DeleteSubscriptionAsync(5, baseSub.UserId);

            await action.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task DeleteSubscriptionAsync_ShouldThrowUnauthorizedAccessException_InvalidId()
        {
            var baseSub = new Subscription
            {
                Id = 1,
                UserId = Guid.NewGuid(),
                Name = "Netflix",
                Price = 9.99f,
                PaymentDay = 5,
                Category = "Streaming"
            };

            _subscriptionRepositoryMock.Setup(s => s.GetSubscriptionByIdAsync(It.IsAny<int>())).ReturnsAsync(baseSub);
            Func<Task> action = async () => await _subscriptionService.DeleteSubscriptionAsync(baseSub.Id, Guid.NewGuid());

            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task DeleteSubscriptionAsync_ShouldBeSuccessful()
        {
            var baseSub = new Subscription
            {
                Id = 1,
                UserId = Guid.NewGuid(),
                Name = "Netflix",
                Price = 9.99f,
                PaymentDay = 5,
                Category = "Streaming"
            };

            _subscriptionRepositoryMock.Setup(s => s.GetSubscriptionByIdAsync(It.IsAny<int>())).ReturnsAsync(baseSub);
            Func<Task> action = async () => await _subscriptionService.DeleteSubscriptionAsync(baseSub.Id, baseSub.UserId);

            await action.Should().NotThrowAsync();
            _subscriptionRepositoryMock.Verify(r => r.DeleteSubscriptionAsync(baseSub), Times.Once);
        }

        [Fact]
        public async Task GetAllSubscriptionsAsync_ShouldReturnPaginatedList()
        {
            var subscriptions = _fixture.Build<Subscription>()
                .With(s => s.PaymentDay, 15)
                .CreateMany(15).ToList();

            var subscriptionsQueryable = subscriptions.BuildMock();

            _subscriptionRepositoryMock
                .Setup(r => r.GetAllSubscriptions())
                .Returns(subscriptionsQueryable);

            var result = await _subscriptionService.GetAllSubscryptionsAsync(1);

            result.Should().NotBeNull();
            result.Should().HaveCount(10);
        }

        [Fact]
        public async Task GetSubscriptionByIdAsync_ShouldReturnSubscription_WhenExists()
        {
            var subscription = _fixture.Create<Subscription>();
            _subscriptionRepositoryMock
                .Setup(r => r.GetSubscriptionByIdAsync(subscription.Id))
                .ReturnsAsync(subscription);

            var result = await _subscriptionService.GetSubscriptionByIdAsync(subscription.Id);

            result.Should().NotBeNull();
            result.Id.Should().Be(subscription.Id);
        }

        [Fact]
        public async Task GetSubscriptionByIdAsync_ShouldThrowNotFoundException_WhenNotExists()
        {
            _subscriptionRepositoryMock
                .Setup(r => r.GetSubscriptionByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(null as Subscription);

            Func<Task> action = async () =>
                await _subscriptionService.GetSubscriptionByIdAsync(999);

            await action.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Subscription with id 999 not found");
        }

        [Fact]
        public async Task GetSubscriptionsByUserAsync_ShouldReturnPaginatedList()
        {
            var userId = Guid.NewGuid();
            var subscriptions = _fixture.Build<Subscription>()
                .With(s => s.UserId, userId)
                .With(s => s.PaymentDay, 15)
                .CreateMany(12)
                .ToList();

            var subscriptionsQueryable = subscriptions.BuildMock();

            _subscriptionRepositoryMock
                .Setup(r => r.GetAllSubscriptionsByUser(userId))
                .Returns(subscriptionsQueryable);

            var result = await _subscriptionService.GetSubscryptionsByUserAsync(userId, 1);

            result.Should().NotBeNull();
            result.Should().HaveCount(10);
            result.All(s => s.UserId == userId).Should().BeTrue();
        }
    }
}
