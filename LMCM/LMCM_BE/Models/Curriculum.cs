using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Curriculum
    {
        public Guid CurriculumId { get; set; }
        public string CurriculumName { get; set; } = null!;
        public string? Status { get; set; }
    }
}
