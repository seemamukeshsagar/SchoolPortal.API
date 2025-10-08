﻿using System;
using System.Collections.Generic;

namespace SchoolPortal.API.Models;

public partial class InventoryMaster
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public Guid ItemId { get; set; }

    public Guid LocationId { get; set; }

    public int? Quantity { get; set; }

    public decimal? CostPerItem { get; set; }

    public bool? IsActive { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SchoolId { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public virtual CompanyMaster Company { get; set; } = null!;

    public virtual UserDetail CreatedByNavigation { get; set; } = null!;

    public virtual ItemMaster Item { get; set; } = null!;

    public virtual UserDetail ModifiedByNavigation { get; set; } = null!;

    public virtual SchoolMaster School { get; set; } = null!;
}
