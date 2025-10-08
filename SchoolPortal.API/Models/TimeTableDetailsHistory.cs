﻿using System;
using System.Collections.Generic;

namespace SchoolPortal.API.Models;

public partial class TimeTableDetailsHistory
{
    public Guid Id { get; set; }

    public Guid PeriodId { get; set; }

    public Guid TeacherId { get; set; }

    public Guid ClassId { get; set; }

    public Guid SectionId { get; set; }

    public Guid SubjectId { get; set; }

    public int DayOfWeek { get; set; }

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

    public virtual ClassMaster Class { get; set; } = null!;

    public virtual CompanyMaster Company { get; set; } = null!;

    public virtual UserDetail? CreatedByNavigation { get; set; }

    public virtual ICollection<TimeTableDetailsHistory> InversePeriod { get; set; } = new List<TimeTableDetailsHistory>();

    public virtual UserDetail? ModifiedByNavigation { get; set; }

    public virtual TimeTableDetailsHistory Period { get; set; } = null!;

    public virtual SchoolMaster School { get; set; } = null!;

    public virtual SectionMaster Section { get; set; } = null!;

    public virtual SessionMaster Session { get; set; } = null!;

    public virtual SubjectMaster Subject { get; set; } = null!;

    public virtual TeacherMaster Teacher { get; set; } = null!;
}
