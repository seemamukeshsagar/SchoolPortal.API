using System;
using System.Collections.Generic;

namespace SchoolPortal.API.Models;

public partial class ScholasticUnitDetail
{
    public int Id { get; set; }

    public int UnitId { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public virtual UserDetail? CreatedByNavigation { get; set; }

    public virtual UserDetail ModifiedByNavigation { get; set; } = null!;
}
