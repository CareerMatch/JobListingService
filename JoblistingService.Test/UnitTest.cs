using JoblistingService.Models;
using JoblistingService.Repositories;
using JoblistingService.Services;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text.Json;
using JoblistingService.Enum;
using Xunit;

namespace JoblistingService.Tests
{
    public class UnitTest
    {
        private readonly Mock<IJobListingRepository> _repositoryMock;
        private readonly Mock<IDistributedCache> _cacheMock;
        private readonly IJobListingService _jobListingService;

        public UnitTest()
        {
            _repositoryMock = new Mock<IJobListingRepository>();
            _cacheMock = new Mock<IDistributedCache>();
            _jobListingService = new JobListingService(_repositoryMock.Object, _cacheMock.Object);
        }

        private byte[] SerializeToByteArray<T>(T obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }
        

        [Fact]
        public async Task GetAllAsync_ShouldReturnFromCache_WhenCacheIsAvailable()
        {
            // Arrange
            const string cacheKey = "jobListings";
            var cachedJobListings = new List<JobListing>
            {
                new JobListing { Id = Guid.NewGuid(), Title = "Software Engineer", Description = "Test Description" }
            };
            var serializedData = SerializeToByteArray(cachedJobListings);

            _cacheMock.Setup(cache => cache.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(serializedData);

            // Act
            var result = await _jobListingService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Software Engineer", result.First().Title);
            _repositoryMock.Verify(repo => repo.GetAllAsync(), Times.Never); // Ensure repository is not called
        }

        [Fact]
        public async Task GetAllAsync_ShouldFetchFromRepository_WhenCacheIsEmpty()
        {
            // Arrange
            const string cacheKey = "jobListings";
            var jobListings = new List<JobListing>
            {
                new JobListing { Id = Guid.NewGuid(), Title = "Software Engineer", Description = "Test Description" }
            };

            _cacheMock.Setup(cache => cache.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((byte[])null); // Simulating cache miss

            _repositoryMock.Setup(repo => repo.GetAllAsync())
                           .ReturnsAsync(jobListings);

            // Act
            var result = await _jobListingService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Software Engineer", result.First().Title);

            _cacheMock.Verify(cache => cache.SetAsync(
                cacheKey,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()
            ), Times.Once); // Ensure data is cached
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnFromCache_WhenCacheIsAvailable()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var cacheKey = $"jobListing:{jobId}";
            var cachedJobListing = new JobListing { Id = jobId, Title = "Software Engineer", Description = "Test Description" };
            var serializedData = SerializeToByteArray(cachedJobListing);

            _cacheMock.Setup(cache => cache.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(serializedData);

            // Act
            var result = await _jobListingService.GetByIdAsync(jobId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Software Engineer", result?.Title);
            _repositoryMock.Verify(repo => repo.GetByIdAsync(jobId), Times.Never); // Ensure repository is not called
        }

        [Fact]
        public async Task GetByIdAsync_ShouldFetchFromRepository_WhenCacheIsEmpty()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var cacheKey = $"jobListing:{jobId}";
            var jobListing = new JobListing { Id = jobId, Title = "Software Engineer", Description = "Test Description" };

            _cacheMock.Setup(cache => cache.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((byte[])null);

            _repositoryMock.Setup(repo => repo.GetByIdAsync(jobId))
                           .ReturnsAsync(jobListing);

            // Act
            var result = await _jobListingService.GetByIdAsync(jobId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Software Engineer", result?.Title);
            _cacheMock.Verify(cache => cache.SetAsync(
                cacheKey,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()
            ), Times.Once); // Ensure cache is updated
        }
        
        [Fact]
        public async Task AddAsync_ShouldInvalidateCache()
        {
            // Arrange
            var jobListing = new JobListing
            {
                Id = Guid.NewGuid(),
                Title = "Software Engineer",
                Description = "Develop and maintain software applications.",
                Location = "New York",
                PostedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                IsActive = true,
                EmployerId = Guid.NewGuid(),
                RequiredSkills = new List<Skill> { Skill.Programming, Skill.Communication },
                Salary = 75000.00,
                JobType = JobType.PartTime,
                Category = JobCategory.IT
            };

            // Act
            await _jobListingService.AddAsync(jobListing);

            // Assert
            _repositoryMock.Verify(repo => repo.AddAsync(jobListing), Times.Once); // Ensure repository AddAsync is called
            _cacheMock.Verify(cache => cache.RemoveAsync("jobListings", It.IsAny<CancellationToken>()), Times.Once); // Ensure cache is invalidated
        }

        [Fact]
        public async Task UpdateAsync_ShouldInvalidateCache()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var jobListing = new JobListing { Id = jobId, Title = "Software Engineer", Description = "Updated Description" };

            // Act
            await _jobListingService.UpdateAsync(jobId, jobListing);

            // Assert
            _repositoryMock.Verify(repo => repo.UpdateAsync(jobId, jobListing), Times.Once);
            _cacheMock.Verify(cache => cache.RemoveAsync($"jobListing:{jobId}", It.IsAny<CancellationToken>()), Times.Once);
            _cacheMock.Verify(cache => cache.RemoveAsync("jobListings", It.IsAny<CancellationToken>()), Times.Once); // Ensure cache is invalidated
        }

        [Fact]
        public async Task DeleteAsync_ShouldInvalidateCache()
        {
            // Arrange
            var jobId = Guid.NewGuid();

            // Act
            await _jobListingService.DeleteAsync(jobId);

            // Assert
            _repositoryMock.Verify(repo => repo.DeleteAsync(jobId), Times.Once);
            _cacheMock.Verify(cache => cache.RemoveAsync($"jobListing:{jobId}", It.IsAny<CancellationToken>()), Times.Once);
            _cacheMock.Verify(cache => cache.RemoveAsync("jobListings", It.IsAny<CancellationToken>()), Times.Once); // Ensure cache is invalidated
        }
    }
}