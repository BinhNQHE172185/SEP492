using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Syllabus
    {
        public Guid SyllabusId { get; set; }
        public string SyllabusCode { get; set; } = null!;
        public string SyllabusName { get; set; } = null!;
        public string? Status { get; set; }
    }
}
