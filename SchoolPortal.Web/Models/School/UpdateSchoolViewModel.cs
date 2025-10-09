using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Web.Models.School
{
    public class UpdateSchoolViewModel : SchoolBaseViewModel
    {
        [Required]
        public Guid Id { get; set; }
        
        // All other validation attributes are inherited from the base class
    }
}
