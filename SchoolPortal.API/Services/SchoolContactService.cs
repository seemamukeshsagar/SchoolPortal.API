using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.Data;
using SchoolPortal.API.DTOs.School;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Interfaces.Services;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Services
{
    public class SchoolContactService : ISchoolContactService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SchoolContactService> _logger;

        public SchoolContactService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SchoolContactService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<SchoolContactDto>> GetContactsBySchoolIdAsync(Guid schoolId)
        {
            try
            {
                var contacts = await _unitOfWork.SchoolContactRepository
                    .GetBySchoolIdWithDetailsAsync(schoolId);
                return _mapper.Map<IEnumerable<SchoolContactDto>>(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contacts for school {SchoolId}", schoolId);
                throw;
            }
        }

        public async Task<SchoolContactDto> GetContactByIdAsync(Guid id)
        {
            try
            {
                var contact = await _unitOfWork.SchoolContactRepository.GetByIdWithDetailsAsync(id);
                if (contact == null)
                {
                    _logger.LogWarning("Contact {ContactId} not found", id);
                    return null;
                }
                return _mapper.Map<SchoolContactDto>(contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contact {ContactId}", id);
                throw;
            }
        }

        public async Task<SchoolContactDto> CreateContactAsync(SchoolContactRequest request)
        {
            try
            {
                var contact = _mapper.Map<SchoolContactMaster>(request);
                contact.Id = Guid.NewGuid();
                contact.CreatedDate = DateTime.UtcNow;
                contact.IsDeleted = false;

                await _unitOfWork.SchoolContactRepository.AddAsync(contact);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Created new contact {ContactId} for school {SchoolId}", 
                    contact.Id, contact.SchoolId);

                return _mapper.Map<SchoolContactDto>(contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contact for school {SchoolId}", request.SchoolId);
                throw;
            }
        }

        public async Task<SchoolContactDto> UpdateContactAsync(Guid id, SchoolContactRequest request)
        {
            try
            {
                var existingContact = await _unitOfWork.SchoolContactRepository.GetByIdAsync(id);
                if (existingContact == null)
                {
                    _logger.LogWarning("Contact {ContactId} not found for update", id);
                    return null;
                }

                _mapper.Map(request, existingContact);
                existingContact.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.SchoolContactRepository.Update(existingContact);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Updated contact {ContactId}", id);

                return _mapper.Map<SchoolContactDto>(existingContact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contact {ContactId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteContactAsync(Guid id)
        {
            try
            {
                var contact = await _unitOfWork.SchoolContactRepository.GetByIdAsync(id);
                if (contact == null)
                {
                    _logger.LogWarning("Contact {ContactId} not found for deletion", id);
                    return false;
                }

                contact.IsDeleted = true;
                contact.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.SchoolContactRepository.Update(contact);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Soft-deleted contact {ContactId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contact {ContactId}", id);
                throw;
            }
        }
    }
}
