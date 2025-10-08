using System;
using System.Collections.Generic;

namespace SchoolPortal.API.Models;

public partial class EmpSalaryStructureDetail
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid DesignationGradeId { get; set; }

    public Guid Session { get; set; }

    public decimal? Value { get; set; }

    public string SalaryType { get; set; } = null!;

    public bool IsDeductance { get; set; }

    public string? SalaryCode { get; set; }

    public string? Description { get; set; }

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

    public virtual DesigGradeDetail DesignationGrade { get; set; } = null!;

    public virtual EmpMaster Employee { get; set; } = null!;

    public virtual UserDetail? ModifiedByNavigation { get; set; }

    public virtual SchoolMaster School { get; set; } = null!;

    public virtual SessionMaster SessionNavigation { get; set; } = null!;
}
