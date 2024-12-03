using JoblistingService.Models;

namespace JoblistingService.Services;

public interface IJobListingService
{
    Task<IEnumerable<JobListing>> GetAllAsync();
    Task<JobListing?> GetByIdAsync(Guid id);
    Task AddAsync(JobListing jobListing);
    Task UpdateAsync(Guid id, JobListing jobListing);
    Task DeleteAsync(Guid id);
}