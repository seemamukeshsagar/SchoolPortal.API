using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.Data;
using SchoolPortal.API.DTOs.Class;
using SchoolPortal.API.Interfaces;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Interfaces.Services;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Services
{
    public class ClassService : IClassService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ClassService> _logger;
        private readonly IRepository<ClassMaster> _repository;

        public ClassService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ClassService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _repository = _unitOfWork.GetRepository<ClassMaster>();
        }

        public async Task<IEnumerable<ClassListDto>> GetAllClassesAsync()
        {
            var classes = await _repository.GetQueryable()
                .Where(c => !c.IsDeleted)
                .Include(c => c.Company)
                .Include(c => c.School)
                .OrderBy(c => c.OrderBy)
                .Select(c => new ClassListDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ExamAssessment = c.ExamAssessment,
                    IsActive = c.IsActive,
                    OrderBy = c.OrderBy,
                    CompanyName = c.Company.CompanyName, // Assuming Company has a Name property
                    SchoolName = c.School.Name // Assuming School has a Name property
                })
                .ToListAsync();

            return classes;
        }

        public async Task<ClassDto> GetClassByIdAsync(Guid id)
        {
            var classMaster = await _repository.GetQueryable()
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (classMaster == null)
            {
                _logger.LogWarning("Class with ID {ClassId} not found", id);
                return null;
            }

            return _mapper.Map<ClassDto>(classMaster);
        }

        public async Task<ClassDto> CreateClassAsync(CreateClassRequest request, Guid userId)
        {
            var classMaster = _mapper.Map<ClassMaster>(request);
            classMaster.Id = Guid.NewGuid();
            classMaster.CreatedBy = userId;
            classMaster.CreatedDate = DateTime.UtcNow;
            classMaster.IsDeleted = false;

            await _repository.AddAsync(classMaster);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Created new class {ClassName} with ID {ClassId}", 
                classMaster.Name, classMaster.Id);

            return _mapper.Map<ClassDto>(classMaster);
        }

        public async Task<ClassDto> UpdateClassAsync(Guid id, UpdateClassRequest request, Guid userId)
        {
            var classMaster = await _repository.GetQueryable()
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (classMaster == null)
            {
                _logger.LogWarning("Class with ID {ClassId} not found for update", id);
                return null;
            }

            _mapper.Map(request, classMaster);
            classMaster.ModifiedBy = userId;
            classMaster.ModifiedDate = DateTime.UtcNow;

            _repository.Update(classMaster);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Updated class {ClassName} with ID {ClassId}", 
                classMaster.Name, classMaster.Id);

            return _mapper.Map<ClassDto>(classMaster);
        }

        public async Task<bool> DeleteClassAsync(Guid id, Guid userId)
        {
            var classMaster = await _repository.GetQueryable()
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (classMaster == null)
            {
                _logger.LogWarning("Class with ID {ClassId} not found for deletion", id);
                return false;
            }

            classMaster.IsDeleted = true;
            classMaster.ModifiedBy = userId;
            classMaster.ModifiedDate = DateTime.UtcNow;

            _repository.Update(classMaster);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Soft-deleted class with ID {ClassId}", id);
            return true;
        }
    }
}