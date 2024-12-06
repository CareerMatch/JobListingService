using JoblistingService.Models;
using JoblistingService.Repositories;

namespace JoblistingService.Services;

public class JobListingService : IJobListingService
{
    private readonly IJobListingRepository _repository;

    public JobListingService(IJobListingRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<JobListing>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<JobListing?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task AddAsync(JobListing jobListing)
    {
        await _repository.AddAsync(jobListing);
    }

    public async Task UpdateAsync(Guid id, JobListing jobListing)
    {
        await _repository.UpdateAsync(id, jobListing);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}