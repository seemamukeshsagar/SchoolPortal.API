using System;
using System.Collections.Generic;

namespace SchoolPortal.API.Models;

public partial class ErrorType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Guid CompanyId { get; set; }

    public Guid SchoolId { get; set; }

    public virtual CompanyMaster Company { get; set; } = null!;

    public virtual ICollection<Error> Errors { get; set; } = new List<Error>();

    public virtual SchoolMaster School { get; set; } = null!;
}
