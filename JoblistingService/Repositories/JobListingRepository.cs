using JoblistingService.Config;
using JoblistingService.Models;
using MongoDB.Driver;

namespace JoblistingService.Repositories;

public class JobListingRepository : IJobListingRepository
{
    private readonly IMongoCollection<JobListing> _jobListings;

    public JobListingRepository(MongoDbContext context)
    {
        _jobListings = context.GetCollection<JobListing>("JobListings");
    }

    public async Task<IEnumerable<JobListing>> GetAllAsync()
    {
        return await _jobListings.Find(_ => true).ToListAsync();
    }

    public async Task<JobListing?> GetByIdAsync(Guid id)
    {
        return await _jobListings.Find(j => j.Id == id).FirstOrDefaultAsync();
    }

    public async Task AddAsync(JobListing jobListing)
    {
        await _jobListings.InsertOneAsync(jobListing);
    }

    public async Task UpdateAsync(Guid id, JobListing jobListing)
    {
        await _jobListings.ReplaceOneAsync(j => j.Id == id, jobListing);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _jobListings.DeleteOneAsync(j => j.Id == id);
    }
}