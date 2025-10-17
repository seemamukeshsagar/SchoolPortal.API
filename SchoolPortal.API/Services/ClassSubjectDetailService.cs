// SchoolPortal.API/Services/ClassSubjectDetailService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using SchoolPortal.API.DTOs.ClassSubjectDetail;
using SchoolPortal.API.Interfaces;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Interfaces.Services;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Services
{
    public class ClassSubjectDetailService : IClassSubjectDetailService
    {
        private readonly IClassSubjectDetailRepository _classSubjectDetailRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ClassSubjectDetailService(
            IClassSubjectDetailRepository classSubjectDetailRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _classSubjectDetailRepository = classSubjectDetailRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClassSubjectDetailDto>> GetAllClassSubjectDetailsAsync()
        {
            var classSubjectDetails = await _classSubjectDetailRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ClassSubjectDetailDto>>(classSubjectDetails);
        }

        public async Task<ClassSubjectDetailDto> GetClassSubjectDetailByIdAsync(Guid id)
        {
            var classSubjectDetail = await _classSubjectDetailRepository.GetByIdAsync(id);
            if (classSubjectDetail == null)
            {
                throw new KeyNotFoundException($"ClassSubjectDetail with ID {id} not found");
            }
            return _mapper.Map<ClassSubjectDetailDto>(classSubjectDetail);
        }

        public async Task<ClassSubjectDetailDto> CreateClassSubjectDetailAsync(CreateClassSubjectDetailRequest request)
        {
            var classSubjectDetail = new ClassSubjectDetail
            {
                Id = Guid.NewGuid(),
                ClassId = request.ClassId,
                SubjectId = request.SubjectId,
                IsActive = request.IsActive,
                Status = request.Status,
                StatusMessage = request.StatusMessage,
                CreatedBy = request.CreatedBy,
                CreatedDate = DateTime.UtcNow
            };

            await _classSubjectDetailRepository.AddAsync(classSubjectDetail);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ClassSubjectDetailDto>(classSubjectDetail);
        }

        public async Task<ClassSubjectDetailDto> UpdateClassSubjectDetailAsync(Guid id, UpdateClassSubjectDetailRequest request)
        {
            var classSubjectDetail = await _classSubjectDetailRepository.GetByIdAsync(id);
            if (classSubjectDetail == null)
            {
                throw new KeyNotFoundException($"ClassSubjectDetail with ID {id} not found");
            }

            classSubjectDetail.IsActive = request.IsActive ?? classSubjectDetail.IsActive;
            classSubjectDetail.Status = request.Status ?? classSubjectDetail.Status;
            classSubjectDetail.StatusMessage = request.StatusMessage;
            classSubjectDetail.ModifiedBy = request.ModifiedBy;
            classSubjectDetail.ModifiedDate = DateTime.UtcNow;

            _classSubjectDetailRepository.Update(classSubjectDetail);
            await _unitOfWork.CommitAsync();

            // Map updated entity back to DTO
            // return _mapper.Map<ClassSubjectDetailDto>(classSubjectDetail);
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteClassSubjectDetailAsync(Guid id)
        {
            var classSubjectDetail = await _classSubjectDetailRepository.GetByIdAsync(id);
            if (classSubjectDetail == null)
            {
                throw new KeyNotFoundException($"ClassSubjectDetail with ID {id} not found");
            }

            _classSubjectDetailRepository.Delete(classSubjectDetail);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}