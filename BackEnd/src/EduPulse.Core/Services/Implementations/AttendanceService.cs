using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using EduPulse.Core.Common.Interfaces;
using EduPulse.Core.Domain.Entities;
using EduPulse.Core.Dtos.Attendance;
using EduPulse.Core.Repositories.Interfaces;
using EduPulse.Core.Services.Interfaces;

namespace EduPulse.Core.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _repository;
        private readonly IClassRepository _classRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITenantContext _tenantContext;
        private readonly IValidator<MarkAttendanceRequest> _validator;

        public AttendanceService(
            IAttendanceRepository repository,
            IClassRepository classRepository,
            ISectionRepository sectionRepository,
            IStudentRepository studentRepository,
            ITenantContext tenantContext,
            IValidator<MarkAttendanceRequest> validator)
        {
            _repository = repository;
            _classRepository = classRepository;
            _sectionRepository = sectionRepository;
            _studentRepository = studentRepository;
            _tenantContext = tenantContext;
            _validator = validator;
        }

        public async Task<bool> MarkAttendanceAsync(MarkAttendanceRequest request)
        {
            await _validator.ValidateAndThrowAsync(request);

            var tenantId = _tenantContext.TenantId;

            // 1. Verify Class and Section exist
            var classObj = await _classRepository.GetByIdAsync(tenantId, request.ClassId);
            if (classObj == null)
            {
                throw new ValidationException($"Class standard with ID '{request.ClassId}' was not found.");
            }

            var sectionObj = await _sectionRepository.GetByIdAsync(tenantId, request.SectionId);
            if (sectionObj == null)
            {
                throw new ValidationException($"Section with ID '{request.SectionId}' was not found.");
            }

            // 2. Fetch or create session
            var session = await _repository.GetSessionAsync(tenantId, request.ClassId, request.SectionId, request.Date);
            Guid sessionId;
            if (session == null)
            {
                var newSession = new AttendanceSession
                {
                    TenantId = tenantId,
                    ClassId = request.ClassId,
                    SectionId = request.SectionId,
                    Date = request.Date.Date,
                    IsDeleted = false,
                    CreatedOn = DateTime.UtcNow,
                    CreatedByUserId = _tenantContext.UserId
                };
                sessionId = await _repository.CreateSessionAsync(newSession);
            }
            else
            {
                sessionId = session.AttendanceSessionId;
            }

            // 3. Map entries
            var entryEntities = request.Entries.Select(e => new AttendanceEntry
            {
                AttendanceSessionId = sessionId,
                StudentId = e.StudentId,
                TenantId = tenantId,
                Status = e.Status,
                Remarks = e.Remarks,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedByUserId = _tenantContext.UserId
            }).ToList();

            return await _repository.UpsertEntriesAsync(tenantId, sessionId, entryEntities);
        }

        public async Task<DailyAttendanceReport?> GetDailyAttendanceAsync(Guid classId, Guid sectionId, DateTime date)
        {
            var tenantId = _tenantContext.TenantId;

            var session = await _repository.GetSessionAsync(tenantId, classId, sectionId, date);
            if (session == null) return null;

            var entries = await _repository.GetEntriesBySessionIdAsync(tenantId, session.AttendanceSessionId);
            return new DailyAttendanceReport(
                session.AttendanceSessionId,
                session.ClassId,
                session.SectionId,
                session.Date,
                entries.ToList()
            );
        }

        public async Task<IEnumerable<StudentAttendanceHistoryDto>> GetStudentHistoryAsync(Guid studentId)
        {
            var tenantId = _tenantContext.TenantId;

            // Verify student exists
            var student = await _studentRepository.GetByIdAsync(tenantId, studentId);
            if (student == null)
            {
                throw new ValidationException($"Student with ID '{studentId}' was not found.");
            }

            return await _repository.GetStudentHistoryAsync(tenantId, studentId);
        }

        public async Task<IEnumerable<ClassAttendanceSummaryDto>> GetClassSummaryAsync(Guid classId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var tenantId = _tenantContext.TenantId;

            // Verify Class exists
            var classObj = await _classRepository.GetByIdAsync(tenantId, classId);
            if (classObj == null)
            {
                throw new ValidationException($"Class standard with ID '{classId}' was not found.");
            }

            return await _repository.GetClassSummaryAsync(tenantId, classId, startDate, endDate);
        }
    }
}
