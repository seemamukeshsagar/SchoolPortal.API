﻿using System;
using System.Collections.Generic;

namespace SchoolPortal.API.Models;

public partial class VehicleExpenseDetail
{
    public Guid Id { get; set; }

    public Guid VehicleId { get; set; }

    public Guid VehicleTypeId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime? ExpenseDate { get; set; }

    public decimal? ExpenseAmount { get; set; }

    public Guid? CompanyId { get; set; }

    public Guid? SchoolId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? Status { get; set; }

    public string? StatusMessage { get; set; }

    public virtual CompanyMaster? Company { get; set; }

    public virtual UserDetail? CreatedByNavigation { get; set; }

    public virtual UserDetail? ModifiedByNavigation { get; set; }

    public virtual SchoolMaster? School { get; set; }

    public virtual VehicleMaster Vehicle { get; set; } = null!;

    public virtual VehicleTypeMaster VehicleType { get; set; } = null!;
}
