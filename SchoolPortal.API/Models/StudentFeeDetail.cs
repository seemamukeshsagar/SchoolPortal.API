﻿using System;
using System.Collections.Generic;

namespace SchoolPortal.API.Models;

public partial class StudentFeeDetail
{
    public Guid Id { get; set; }

    public Guid StudentGuid { get; set; }

    public Guid ClassId { get; set; }

    public Guid SectionId { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? PaidDate { get; set; }

    public bool IsPaid { get; set; }

    public decimal? Amount { get; set; }

    public decimal? LateFeeAmount { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? FeeReceiptNumber { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public Guid SchoolId { get; set; }

    public Guid CompanyId { get; set; }

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

    public virtual UserDetail? ModifiedByNavigation { get; set; }

    public virtual SchoolMaster School { get; set; } = null!;

    public virtual SectionMaster Section { get; set; } = null!;

    public virtual StudentMaster Student { get; set; } = null!;
}
