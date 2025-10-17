using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.Models;

public partial class ClassSubjectDetail
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid ClassId { get; set; }
    
    [Required]
    public Guid SubjectId { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SchoolId { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Status Message")]
    public string? Status { get; set; }

    [StringLength(500)]
    public string? StatusMessage { get; set; }

    public virtual ClassMaster ClassMaster { get; set; } = null!;

    public virtual CompanyMaster Company { get; set; } = null!;

    public virtual UserDetail? CreatedByNavigation { get; set; }

    public virtual UserDetail? ModifiedByNavigation { get; set; }

    public virtual SchoolMaster School { get; set; } = null!;

    public virtual SubjectMaster Subject { get; set; } = null!;
}
