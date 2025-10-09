using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SchoolPortal.API.DTOs.School;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Interfaces.Services;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Services
{
    public class SchoolService : ISchoolService
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly IMapper _mapper;

        public SchoolService(ISchoolRepository schoolRepository, IMapper mapper)
        {
            _schoolRepository = schoolRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SchoolDto>> GetAllSchoolsAsync()
        {
            var schools = await _schoolRepository.GetAllSchoolsAsync();
            return _mapper.Map<IEnumerable<SchoolDto>>(schools);
        }

        public async Task<SchoolDto> GetSchoolByIdAsync(Guid id)
        {
            var school = await _schoolRepository.GetSchoolByIdAsync(id);
            return _mapper.Map<SchoolDto>(school);
        }

        public async Task<SchoolDto> CreateSchoolAsync(CreateSchoolRequest request)
        {
            var school = _mapper.Map<SchoolMaster>(request);
            var createdSchool = await _schoolRepository.CreateSchoolAsync(school);
            return _mapper.Map<SchoolDto>(createdSchool);
        }

        public async Task<SchoolDto> UpdateSchoolAsync(UpdateSchoolRequest request)
        {
            var existingSchool = await _schoolRepository.GetSchoolByIdAsync(request.Id);
            if (existingSchool == null)
                throw new KeyNotFoundException("School not found");

            _mapper.Map(request, existingSchool);
            var updatedSchool = await _schoolRepository.UpdateSchoolAsync(existingSchool);
            return _mapper.Map<SchoolDto>(updatedSchool);
        }

        public async Task<bool> DeleteSchoolAsync(Guid id)
        {
            return await _schoolRepository.DeleteSchoolAsync(id);
        }

        public async Task<IEnumerable<SchoolDto>> GetSchoolsByCompanyIdAsync(Guid companyId)
        {
            var schools = await _schoolRepository.GetSchoolsByCompanyIdAsync(companyId);
            return _mapper.Map<IEnumerable<SchoolDto>>(schools);
        }
    }
}
