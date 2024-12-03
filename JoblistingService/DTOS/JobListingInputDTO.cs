using System.ComponentModel.DataAnnotations;
using JoblistingService.Enum;

namespace JoblistingService.DTOS;

public class JobListingInputDto
{
    [Required] 
    [MaxLength(255)] 
    public string Title { get; set; }

    [Required] 
    [MaxLength(5000)] 
    public string Description { get; set; }

    [Required] 
    [MaxLength(100)] 
    public string Location { get; set; }

    [Required] 
    public DateTime PostedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiryDate { get; set; }

    [Required] 
    public bool IsActive { get; set; } = true;

    [Required] 
    public Guid EmployerId { get; set; }

    [Required] 
    public ICollection<Skill> RequiredSkills { get; set; }

    [Required] 
    [Range(0, double.MaxValue)] 
    public double Salary { get; set; }

    [Required] 
    public JobType JobType { get; set; }

    [Required] 
    public JobCategory Category { get; set; }
}