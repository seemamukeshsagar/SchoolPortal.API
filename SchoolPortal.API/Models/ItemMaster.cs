﻿using System;
using System.Collections.Generic;

namespace SchoolPortal.API.Models;

public partial class ItemMaster
{
    public Guid Id { get; set; }

    public string? ItemName { get; set; }

    public string? Description { get; set; }

    public Guid ItemTypeMasterId { get; set; }

    public bool? IsActive { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SchoolId { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public virtual CompanyMaster Company { get; set; } = null!;

    public virtual UserDetail CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<InventoryMaster> InventoryMasters { get; set; } = new List<InventoryMaster>();

    public virtual ItemTypeMaster ItemTypeMaster { get; set; } = null!;

    public virtual UserDetail ModifiedByNavigation { get; set; } = null!;

    public virtual SchoolMaster School { get; set; } = null!;
}
