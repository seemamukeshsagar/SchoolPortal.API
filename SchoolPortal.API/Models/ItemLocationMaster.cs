﻿using System;
using System.Collections.Generic;

namespace SchoolPortal.API.Models;

public partial class ItemLocationMaster
{
    public Guid Id { get; set; }

    public string? LocationName { get; set; }

    public string? Description { get; set; }

    public string? Building { get; set; }

    public string? LocationFloor { get; set; }

    public int? LocationNumber { get; set; }

    public int? Capacity { get; set; }

    public bool? IsActive { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SchoolId { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public virtual CompanyMaster Company { get; set; } = null!;

    public virtual UserDetail CreatedByNavigation { get; set; } = null!;

    public virtual UserDetail ModifiedByNavigation { get; set; } = null!;

    public virtual SchoolMaster School { get; set; } = null!;
}
