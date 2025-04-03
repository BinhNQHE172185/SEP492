using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class LearningMaterial
{
    public Guid MaterialId { get; set; } 

    public Guid SyllabusId { get; set; } 

    public string? LearningType { get; set; } 
    public bool? IsMainMaterial { get; set; } 
    public bool? IsImportedMaterial { get; set; } 

    public string? MaterialName { get; set; } 

    public string? Isbn { get; set; }


    public string? Author { get; set; }

    public string? Publisher { get; set; }

    public DateTime? PublishedDate { get; set; }

    public string? Edition { get; set; }

    public string? Url { get; set; } 

    public string? Purpose { get; set; } 

    public string? Note { get; set; } 

    public string? Status { get; set; } 

    public DateTime? CreatedAt { get; set; } 

    public DateTime? UpdatedAt { get; set; } 

    public virtual Syllabus Syllabus { get; set; } = null!; 
}
