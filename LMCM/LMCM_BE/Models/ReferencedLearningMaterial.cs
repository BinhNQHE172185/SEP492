using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class ReferencedLearningMaterial
{
    public Guid MaterialId { get; set; }

    public string MaterialName { get; set; } = null!;

    public string? MaterialDescription { get; set; }

    public string? Isbn { get; set; }

    public string? Type { get; set; }

    public string? Note { get; set; }

    public string? Author { get; set; }

    public string? Publisher { get; set; }

    public DateTime? PublishedDate { get; set; }

    public string? Edition { get; set; }

    public string? Url { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<SyllabusReferencedLearningMaterial> SyllabusReferencedLearningMaterials { get; set; } = new List<SyllabusReferencedLearningMaterial>();
}
