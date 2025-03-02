using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class Syllabus
{
    public Guid SyllabusId { get; set; }

    public string ProgramName { get; set; } = null!;

    public string? DecisionNo { get; set; }

    public string CourseName { get; set; } = null!;

    public string? CourseNameEnglish { get; set; }

    public string CourseCode { get; set; } = null!;

    public string? LearningTeachingMethod { get; set; }

    public int? NoOfCredits { get; set; }

    public string? DegreeLevel { get; set; }

    public string? TimeAllocation { get; set; }

    public string? PreRequisite { get; set; }

    public string? Description { get; set; }

    public string? StudentTask { get; set; }

    public string? Tools { get; set; }

    public string? Note { get; set; }

    public decimal? MinGpaToPass { get; set; }

    public decimal? ScoringScale { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Clo> Clos { get; set; } = new List<Clo>();

    public virtual ICollection<ConstructivistQuestion> ConstructivistQuestions { get; set; } = new List<ConstructivistQuestion>();

    public virtual ICollection<GradingStructure> GradingStructures { get; set; } = new List<GradingStructure>();

    public virtual ICollection<ImportedLearningMaterial> ImportedLearningMaterials { get; set; } = new List<ImportedLearningMaterial>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<SubjectsSyllabus> SubjectsSyllabi { get; set; } = new List<SubjectsSyllabus>();

    public virtual ICollection<SyllabusReferencedLearningMaterial> SyllabusReferencedLearningMaterials { get; set; } = new List<SyllabusReferencedLearningMaterial>();
}
