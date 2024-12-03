using JoblistingService.DTOS;
using JoblistingService.Models;
using JoblistingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace JoblistingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobListingsController : ControllerBase
{
    private readonly IJobListingService _jobListingService;

    public JobListingsController(IJobListingService jobListingService)
    {
        _jobListingService = jobListingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var jobListings = await _jobListingService.GetAllAsync();
        return Ok(jobListings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var jobListing = await _jobListingService.GetByIdAsync(id);
        if (jobListing == null)
        {
            return NotFound();
        }
        return Ok(jobListing);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] JobListingInputDto jobListingInput)
    {
        // Map DTO to the model
        var jobListing = new JobListing
        {
            Title = jobListingInput.Title,
            Description = jobListingInput.Description,
            Location = jobListingInput.Location,
            PostedDate = jobListingInput.PostedDate,
            ExpiryDate = jobListingInput.ExpiryDate,
            IsActive = jobListingInput.IsActive,
            EmployerId = jobListingInput.EmployerId,
            RequiredSkills = jobListingInput.RequiredSkills,
            Salary = jobListingInput.Salary,
            JobType = jobListingInput.JobType,
            Category = jobListingInput.Category
        };

        await _jobListingService.AddAsync(jobListing);
        return CreatedAtAction(nameof(GetById), new { id = jobListing.Id }, jobListing);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] JobListingInputDto jobListingInput)
    {
        var existingJob = await _jobListingService.GetByIdAsync(id);
        if (existingJob == null)
        {
            return NotFound();
        }

        // Map DTO to the model
        existingJob.Title = jobListingInput.Title;
        existingJob.Description = jobListingInput.Description;
        existingJob.Location = jobListingInput.Location;
        existingJob.PostedDate = jobListingInput.PostedDate;
        existingJob.ExpiryDate = jobListingInput.ExpiryDate;
        existingJob.IsActive = jobListingInput.IsActive;
        existingJob.EmployerId = jobListingInput.EmployerId;
        existingJob.RequiredSkills = jobListingInput.RequiredSkills;
        existingJob.Salary = jobListingInput.Salary;
        existingJob.JobType = jobListingInput.JobType;
        existingJob.Category = jobListingInput.Category;

        await _jobListingService.UpdateAsync(id, existingJob);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existingJob = await _jobListingService.GetByIdAsync(id);
        if (existingJob == null)
        {
            return NotFound();
        }

        await _jobListingService.DeleteAsync(id);
        return NoContent();
    }
}
