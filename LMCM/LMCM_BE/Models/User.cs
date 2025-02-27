using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class User : IdentityUser<Guid>
{
    public string Picture { get; set; }
    public string Name { get; set; }
    public User()
    {
        BudgetProposals = new HashSet<BudgetProposal>();
        DocumentTemplates = new HashSet<DocumentTemplate>();
        Notifications = new HashSet<Notification>();
    }

    public virtual ICollection<BudgetProposal> BudgetProposals { get; set; }
    public virtual ICollection<DocumentTemplate> DocumentTemplates { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; }
}
