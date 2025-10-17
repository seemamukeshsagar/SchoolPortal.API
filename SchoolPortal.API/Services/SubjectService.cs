// Services/SubjectService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using SchoolPortal.API.DTOs.Subject;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Interfaces.Services;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IMapper _mapper;

        public SubjectService(ISubjectRepository subjectRepository, IMapper mapper)
        {
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync()
        {
            var subjects = await _subjectRepository.GetAllSubjectsAsync();
            return _mapper.Map<IEnumerable<SubjectDto>>(subjects);
        }

        public async Task<SubjectDto> GetSubjectByIdAsync(Guid id)
        {
            var subject = await _subjectRepository.GetSubjectByIdAsync(id);
            return _mapper.Map<SubjectDto>(subject);
        }

        public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectRequest request)
        {
            var subject = _mapper.Map<SubjectMaster>(request);
            var createdSubject = await _subjectRepository.CreateSubjectAsync(subject);
            return _mapper.Map<SubjectDto>(createdSubject);
        }

        public async Task<SubjectDto> UpdateSubjectAsync(UpdateSubjectRequest request)
        {
            var existingSubject = await _subjectRepository.GetSubjectByIdAsync(request.Id);
            _mapper.Map(request, existingSubject);
            var updatedSubject = await _subjectRepository.UpdateSubjectAsync(existingSubject);
            return _mapper.Map<SubjectDto>(updatedSubject);
        }

        public async Task<bool> DeleteSubjectAsync(Guid id)
        {
            return await _subjectRepository.DeleteSubjectAsync(id);
        }

        public async Task<IEnumerable<SubjectDto>> GetSubjectsBySchoolAsync(Guid schoolId)
        {
            var subjects = await _subjectRepository.GetSubjectsBySchoolAsync(schoolId);
            return _mapper.Map<IEnumerable<SubjectDto>>(subjects);
        }
    }
}