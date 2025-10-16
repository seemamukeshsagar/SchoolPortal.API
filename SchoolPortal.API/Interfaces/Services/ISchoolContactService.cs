using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolPortal.API.DTOs.School;

namespace SchoolPortal.API.Interfaces.Services
{
    /// <summary>
    /// Service interface for school contact operations
    /// </summary>
    public interface ISchoolContactService
    {
        /// <summary>
        /// Gets all contacts for a school
        /// </summary>
        /// <param name="schoolId">The school ID</param>
        /// <returns>List of school contacts</returns>
        Task<IEnumerable<SchoolContactDto>> GetContactsBySchoolIdAsync(Guid schoolId);

        /// <summary>
        /// Gets a school contact by ID
        /// </summary>
        /// <param name="id">The contact ID</param>
        /// <returns>The school contact if found</returns>
        Task<SchoolContactDto> GetContactByIdAsync(Guid id);

        /// <summary>
        /// Creates a new school contact
        /// </summary>
        /// <param name="request">The contact creation request</param>
        /// <returns>The created contact</returns>
        Task<SchoolContactDto> CreateContactAsync(SchoolContactRequest request);

        /// <summary>
        /// Updates an existing school contact
        /// </summary>
        /// <param name="id">The contact ID</param>
        /// <param name="request">The contact update request</param>
        /// <returns>The updated contact</returns>
        Task<SchoolContactDto> UpdateContactAsync(Guid id, SchoolContactRequest request);

        /// <summary>
        /// Deletes a school contact
        /// </summary>
        /// <param name="id">The contact ID</param>
        /// <returns>True if deleted successfully</returns>
        Task<bool> DeleteContactAsync(Guid id);
    }
}
