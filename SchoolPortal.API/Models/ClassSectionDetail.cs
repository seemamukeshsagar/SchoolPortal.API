using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolPortal.API.Models;

public partial class ClassSectionDetail
{
    public Guid Id { get; set; }

    public Guid ClassMasterId { get; set; }

    public Guid SectionMasterId { get; set; }

    public Guid LocationId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int MaxStudents { get; set; }

    public string AcademicYear { get; set; }

    public string? Remarks { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SchoolId { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? Status { get; set; }

    public string? StatusMessage { get; set; }

    [ForeignKey("ClassId")]
    public virtual ClassMaster ClassMaster { get; set; } = null!;

    public virtual CompanyMaster Company { get; set; } = null!;

    public virtual UserDetail? CreatedByNavigation { get; set; }

    public virtual LocationMaster Location { get; set; } = null!;

    public virtual UserDetail? ModifiedByNavigation { get; set; }

    public virtual SchoolMaster School { get; set; } = null!;

    [ForeignKey("SectionId")]
    public virtual SectionMaster SectionMaster { get; set; } = null!;

    [ForeignKey("ClassTeacherId")]
    public virtual EmpMaster ClassTeacher { get; set; }
    public Guid ClassId { get; internal set; }
}
