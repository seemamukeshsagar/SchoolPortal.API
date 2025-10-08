using System;
using System.Collections.Generic;

namespace SchoolPortal.API.Models;

public partial class TimeTablePeriodMasterHistory
{
    public Guid Id { get; set; }

    public Guid PeriodId { get; set; }

    public string Description { get; set; } = null!;

    public int PeriodNumber { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public Guid SessionId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SchoolId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? Status { get; set; }

    public string? StatusMessage { get; set; }

    public virtual CompanyMaster Company { get; set; } = null!;

    public virtual UserDetail? CreatedByNavigation { get; set; }

    public virtual UserDetail? ModifiedByNavigation { get; set; }

    public virtual TimeTablePeriodMaster Period { get; set; } = null!;

    public virtual SchoolMaster School { get; set; } = null!;

    public virtual SessionMaster Session { get; set; } = null!;
}
