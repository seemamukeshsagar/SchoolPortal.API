using System;
using System.Collections.Generic;

namespace SchoolPortal.API.Models;

public partial class StudentReportCardMaster
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public int ClassId { get; set; }

    public int SectionId { get; set; }

    public string? Session { get; set; }

    public string? ReportCardType { get; set; }

    public string? ReportCardValue { get; set; }

    public string? Period { get; set; }

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
}
