using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class CurriculumsSubject
    {
        public Guid CurriculumId { get; set; }
        public Guid SubjectId { get; set; }

        public virtual Curriculum Curriculum { get; set; } = null!;
        public virtual Subject Subject { get; set; } = null!;
    }
}
