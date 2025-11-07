using AutoFixture;
using FluentAssertions;
using JuniorOnly.Application.Commons;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockQueryable;
using Moq;
using SubManager.Application.DTO.Subscription;
using SubManager.Application.Exceptions;
using SubManager.Application.Extensions;
using SubManager.Application.Interfaces;
using SubManager.Application.Services;
using SubManager.Domain.Entities;
using SubManager.Domain.IdentityEntities;
using SubManager.Domain.Repositories;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace SubManager.Tests.Unit.Services
{
    public class SubscriptionServiceTests
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
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

            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<ApplicationUser>>().Object,
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

            _categoryRepositoryMock = new Mock<ICategoryRepository>();

            _subscriptionService = new SubscriptionService(_subscriptionRepositoryMock.Object, options, _userManagerMock.Object, _categoryRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateSubscriptionAsync_ShouldThrowValidationException_InvalidPrice()
        {
            var userId = Guid.NewGuid();
            var subcriptionCreate = _fixture.Build<SubscriptionCreateDto>()
                .With(temp => temp.Price, -1m)
                .Create();
            _userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString())).ReturnsAsync(new ApplicationUser { Id = userId });
            Func<Task> action = async () =>
            {
                await _subscriptionService.CreateSubscriptionAsync(subcriptionCreate, userId);
            };
            await action.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task CreateSubscriptionAsync_ShouldThrowNotFoundException_InvalidCategoryId()
        {
            var userId = Guid.NewGuid();
            var subcriptionCreate = _fixture.Build<SubscriptionCreateDto>()
                .With(temp => temp.CategoryId, 199)
                .Create();
            _userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString())).ReturnsAsync(new ApplicationUser { Id = userId });
            _categoryRepositoryMock.Setup(c => c.GetByIdAsync(199)).ReturnsAsync((Category?)null);
            Func<Task> action = async () =>
            {
                await _subscriptionService.CreateSubscriptionAsync(subcriptionCreate, userId);
            };
            await action.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task CreateSubscriptionAsync_ShouldThrowValidationException_InvalidPaymentDay()
        {
            var userId = Guid.NewGuid();
            var subcriptionCreate = _fixture.Build<SubscriptionCreateDto>()
                .With(temp => temp.PaymentDay, 34)
                .Create();
            _userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString())).ReturnsAsync(new ApplicationUser { Id = userId });
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
                .With(temp => temp.Price, 15m)
                .Create();

            var subEntity = subcriptionCreate.ToEntity();
            var Category = new Category { Name = "Test" };
            _userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString())).ReturnsAsync(new ApplicationUser { Id = userId });
            _subscriptionRepositoryMock.Setup(s => s.AddSubscriptionAsync(It.IsAny<Subscription>())).ReturnsAsync(subEntity);
            _categoryRepositoryMock.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(Category);
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
            var subscriptionId = 1;
            var userId = Guid.NewGuid();

            var subscription = new Subscription
            {
                Id = subscriptionId,
                UserId = userId,
                Name = "Netflix",
                Price = 9.99m,
                PaymentDay = 5,
            };

            var subcriptionUpdate = _fixture.Build<SubscriptionUpdateDto>()
                .With(temp => temp.Price, -1m)
                .Create();

            _subscriptionRepositoryMock.Setup(s => s.GetSubscriptionByIdAsync(It.IsAny<int>(), userId)).ReturnsAsync(subscription);

            Func<Task> action = async () =>
            {
                await _subscriptionService.UpdateSubscriptionAsync(subscriptionId, subcriptionUpdate, userId);
            };
            await action.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UpdateSubscriptionAsync_ShouldThrowValidationException_InvalidPaymentDay()
        {
            var subscriptionId = 1;
            var userId = Guid.NewGuid();

            var subscription = new Subscription
            {
                Id = subscriptionId,
                UserId = userId,
                Name = "Netflix",
                Price = 9.99m,
                PaymentDay = 5,
            };

            var subcriptionUpdate = _fixture.Build<SubscriptionUpdateDto>()
                .With(temp => temp.PaymentDay, 32)
                .Create();
            _subscriptionRepositoryMock.Setup(s => s.GetSubscriptionByIdAsync(It.IsAny<int>(), userId)).ReturnsAsync(subscription);

            Func<Task> action = async () =>
            {
                await _subscriptionService.UpdateSubscriptionAsync(subscriptionId, subcriptionUpdate, userId);
            };
            await action.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UpdateSubscriptionAsync_ShouldThrowNotFoundException_InvalidId()
        {
            var userId = Guid.NewGuid();

            var subcriptionUpdate = _fixture.Build<SubscriptionUpdateDto>()
                .Create();

            _subscriptionRepositoryMock.Setup(s => s.GetSubscriptionByIdAsync(It.IsAny<int>(), userId)).ReturnsAsync(null as Subscription);

            Func<Task> action = async () =>
            {
                await _subscriptionService.UpdateSubscriptionAsync(999, subcriptionUpdate, userId);
            };
            await action.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task UpdateSubscriptionAsync_ShouldThrowNotFoundException_InvalidUserId()
        {
            var subscriptionId = 1;
            var userId = Guid.NewGuid();
            var subscription = new Subscription
            {
                Id = subscriptionId,
                UserId = userId,
                Name = "Netflix",
                Price = 9.99m,
                PaymentDay = 5,
            };

            var subcriptionUpdate = _fixture.Build<SubscriptionUpdateDto>()
                .Create();

            _subscriptionRepositoryMock.Setup(s => s.GetSubscriptionByIdAsync(subscriptionId, userId)).ReturnsAsync(subscription);

            Func<Task> action = async () =>
            {
                await _subscriptionService.UpdateSubscriptionAsync(subscriptionId, subcriptionUpdate, Guid.NewGuid());
            };
            await action.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task UpdateSubscriptionAsync_ShouldBeSuccessful()
        {
            var subscriptionId = 1;
            var userId = Guid.NewGuid();
            var baseSub = new Subscription
            {
                Id = subscriptionId,
                UserId = userId,
                Name = "Netflix",
                Price = 9.99m,
                PaymentDay = 5,
            };

            var Category = new Category { Name = "Test" };
            var subcriptionUpdate = _fixture.Build<SubscriptionUpdateDto>()
                .With(temp => temp.PaymentDay, 18)
                .With(temp => temp.Name, "Amazon")
                .With(temp => temp.Price, 9.99m)
                .Create();

            _subscriptionRepositoryMock.Setup(s => s.GetSubscriptionByIdAsync(subscriptionId, userId)).ReturnsAsync(baseSub);
            _categoryRepositoryMock.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(Category);

            //_subscriptionRepositoryMock.Setup(s => s.UpdateSubscriptionAsync(It.IsAny<Subscription>())).ReturnsAsync((Subscription s) => s);

            var updatedSubscription = await _subscriptionService.UpdateSubscriptionAsync(subscriptionId, subcriptionUpdate, userId);
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
            var subscriptionId = 1;
            var userId = Guid.NewGuid();
            var baseSub = new Subscription
            {
                Id = subscriptionId,
                UserId = userId,
                Name = "Netflix",
                Price = 9.99m,
                PaymentDay = 5,
            };

            _subscriptionRepositoryMock.Setup(s => s.GetSubscriptionByIdAsync(It.IsAny<int>(), userId)).ReturnsAsync(null as Subscription);
            Func<Task> action = async () => await _subscriptionService.DeleteSubscriptionAsync(5, userId);

            await action.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task DeleteSubscriptionAsync_ShouldThrowNotFoundException_InvalidUserId()
        {
            var subscriptionId = 1;
            var userId = Guid.NewGuid();
            var baseSub = new Subscription
            {
                Id = subscriptionId,
                UserId = userId,
                Name = "Netflix",
                Price = 9.99m,
                PaymentDay = 5,
            };

            _subscriptionRepositoryMock.Setup(s => s.GetSubscriptionByIdAsync(subscriptionId, userId)).ReturnsAsync(baseSub);
            Func<Task> action = async () => await _subscriptionService.DeleteSubscriptionAsync(baseSub.Id, Guid.NewGuid());

            await action.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task DeleteSubscriptionAsync_ShouldBeSuccessful()
        {
            var subscriptionId = 1;
            var userId = Guid.NewGuid();
            var baseSub = new Subscription
            {
                Id = subscriptionId,
                UserId = userId,
                Name = "Netflix",
                Price = 9.99m,
                PaymentDay = 5,
            };

            _subscriptionRepositoryMock.Setup(s => s.GetSubscriptionByIdAsync(subscriptionId, userId)).ReturnsAsync(baseSub);
            Func<Task> action = async () => await _subscriptionService.DeleteSubscriptionAsync(subscriptionId, userId);

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
            var userId = Guid.NewGuid();
            //var user = new ApplicationUser
            //{
            //    Email = "test@test.com",
            //    Id = userId,
            //    Cur
            //}
            var subscription = _fixture.Build<Subscription>()
                .With(s => s.PaymentDay, 15)
                .Create();
            _subscriptionRepositoryMock
                .Setup(r => r.GetSubscriptionByIdAsync(subscription.Id, It.IsAny<Guid>()))
                .ReturnsAsync(subscription);

            var result = await _subscriptionService.GetSubscriptionByIdAsync(subscription.Id, userId);

            result.Should().NotBeNull();
            result.Id.Should().Be(subscription.Id);
        }

        [Fact]
        public async Task GetSubscriptionByIdAsync_ShouldThrowNotFoundException_WhenNotExists()
        {
            var userId = Guid.NewGuid();
            _subscriptionRepositoryMock
                .Setup(r => r.GetSubscriptionByIdAsync(It.IsAny<int>(), It.IsAny<Guid>()))
                .ReturnsAsync(null as Subscription);

            Func<Task> action = async () =>
                await _subscriptionService.GetSubscriptionByIdAsync(999, userId);

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
                .With(s => s.Price, 9.99m)
                .CreateMany(12)
                .ToList();

            var subscriptionsQueryable = subscriptions.BuildMock();

            _subscriptionRepositoryMock
                .Setup(r => r.GetAllSubscriptionsByUser(userId))
                .Returns(subscriptionsQueryable);

            var result = await _subscriptionService.GetSubscryptionsByUserAsync(userId, 1);

            result.Should().NotBeNull();
            result.TotalCount.Should().Be(12);
            result.TotalPages.Should().Be(2);
            result.Items?.Should().HaveCount(10);
            result.Items?.All(s => s.UserId == userId).Should().BeTrue();
        }
    }
}
