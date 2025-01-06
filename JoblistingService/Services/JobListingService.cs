using JoblistingService.Models;
using JoblistingService.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace JoblistingService.Services;

public class JobListingService : IJobListingService
{
    private readonly IJobListingRepository _repository;
    private readonly IDistributedCache _cache;

    public JobListingService(IJobListingRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IEnumerable<JobListing>> GetAllAsync()
    {
        const string cacheKey = "jobListings";
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            // Cache hit - deserialize and return cached data
            return JsonSerializer.Deserialize<IEnumerable<JobListing>>(cachedData);
        }

        // Cache miss - fetch data from repository and store in cache
        var jobListings = await _repository.GetAllAsync();

        var cacheEntryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache expiration time
        };

        var serializedData = JsonSerializer.Serialize(jobListings);
        await _cache.SetStringAsync(cacheKey, serializedData, cacheEntryOptions);

        return jobListings;
    }

    public async Task<JobListing?> GetByIdAsync(Guid id)
    {
        var cacheKey = $"jobListing:{id}";
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            // Cache hit - deserialize and return cached data
            return JsonSerializer.Deserialize<JobListing>(cachedData);
        }

        // Cache miss - fetch data from repository and store in cache
        var jobListing = await _repository.GetByIdAsync(id);

        if (jobListing != null)
        {
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache expiration time
            };

            var serializedData = JsonSerializer.Serialize(jobListing);
            await _cache.SetStringAsync(cacheKey, serializedData, cacheEntryOptions);
        }

        return jobListing;
    }

    public async Task AddAsync(JobListing jobListing)
    {
        await _repository.AddAsync(jobListing);

        // Invalidate the cache for all job listings
        const string cacheKey = "jobListings";
        await _cache.RemoveAsync(cacheKey);
    }

    public async Task UpdateAsync(Guid id, JobListing jobListing)
    {
        await _repository.UpdateAsync(id, jobListing);

        // Invalidate the cache for the specific job listing and all job listings
        var cacheKey = $"jobListing:{id}";
        await _cache.RemoveAsync(cacheKey);

        const string allCacheKey = "jobListings";
        await _cache.RemoveAsync(allCacheKey);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);

        // Invalidate the cache for the specific job listing and all job listings
        var cacheKey = $"jobListing:{id}";
        await _cache.RemoveAsync(cacheKey);

        const string allCacheKey = "jobListings";
        await _cache.RemoveAsync(allCacheKey);
    }
}