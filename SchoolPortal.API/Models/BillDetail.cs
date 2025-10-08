﻿using System;
using System.Collections.Generic;

namespace SchoolPortal.API.Models;

public partial class BillDetail
{
    public Guid Id { get; set; }

    public Guid BillId { get; set; }

    public Guid ExpenseCategoryId { get; set; }

    public string? Description { get; set; }

    public int? Quantity { get; set; }

    public decimal? Amount { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SchoolId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string Status { get; set; } = null!;

    public string? StatusMessage { get; set; }

    public virtual BillMaster Bill { get; set; } = null!;

    public virtual CompanyMaster Company { get; set; } = null!;

    public virtual UserDetail? CreatedByNavigation { get; set; }

    public virtual ExpenseCategoryMaster ExpenseCategory { get; set; } = null!;

    public virtual UserDetail? ModifiedByNavigation { get; set; }

    public virtual SchoolMaster School { get; set; } = null!;
}
