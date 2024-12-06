using System.ComponentModel.DataAnnotations;
using JoblistingService.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JoblistingService.Models;

public class JobListing
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; } 

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
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid EmployerId { get; set; } 

    public ICollection<Skill> RequiredSkills { get; set; } = new List<Skill>(); 

    [Required] 
    [Range(0, double.MaxValue)] 
    public double Salary { get; set; } 

    [Required] 
    public JobType JobType { get; set; } 

    [Required] 
    public JobCategory Category { get; set; }
    
}