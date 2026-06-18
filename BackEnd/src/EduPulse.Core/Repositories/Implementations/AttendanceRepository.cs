using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using EduPulse.Core.Database.Connection;
using EduPulse.Core.Domain.Entities;
using EduPulse.Core.Dtos.Attendance;
using EduPulse.Core.Repositories.Interfaces;

namespace EduPulse.Core.Repositories.Implementations
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AttendanceRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<AttendanceSession?> GetSessionAsync(Guid tenantId, Guid classId, Guid sectionId, DateTime date)
        {
            const string sql = @"
                SELECT AttendanceSessionId, TenantId, ClassId, SectionId, Date, IsDeleted, CreatedOn, CreatedByUserId, ModifiedOn, ModifiedByUserId
                FROM dbo.AttendanceSessions
                WHERE TenantId = @TenantId AND ClassId = @ClassId AND SectionId = @SectionId AND Date = @Date AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<AttendanceSession>(sql, new 
            { 
                TenantId = tenantId, 
                ClassId = classId, 
                SectionId = sectionId, 
                Date = date.Date 
            });
        }

        public async Task<Guid> CreateSessionAsync(AttendanceSession session)
        {
            const string sql = @"
                INSERT INTO dbo.AttendanceSessions (AttendanceSessionId, TenantId, ClassId, SectionId, Date, IsDeleted, CreatedOn, CreatedByUserId)
                OUTPUT INSERTED.AttendanceSessionId
                VALUES (@AttendanceSessionId, @TenantId, @ClassId, @SectionId, @Date, @IsDeleted, @CreatedOn, @CreatedByUserId);";

            if (session.AttendanceSessionId == Guid.Empty)
            {
                session.AttendanceSessionId = Guid.NewGuid();
            }

            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<Guid>(sql, new 
            {
                session.AttendanceSessionId,
                session.TenantId,
                session.ClassId,
                session.SectionId,
                Date = session.Date.Date,
                session.IsDeleted,
                session.CreatedOn,
                session.CreatedByUserId
            });
        }

        public async Task<IEnumerable<AttendanceEntryDto>> GetEntriesBySessionIdAsync(Guid tenantId, Guid sessionId)
        {
            const string sql = @"
                SELECT e.StudentId, s.FirstName, s.LastName, s.AdmissionNo, e.Status, e.Remarks
                FROM dbo.AttendanceEntries e
                INNER JOIN dbo.Students s ON e.StudentId = s.StudentId
                WHERE e.TenantId = @TenantId AND e.AttendanceSessionId = @SessionId AND e.IsDeleted = 0 AND s.IsDeleted = 0
                ORDER BY s.FirstName ASC, s.LastName ASC;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<AttendanceEntryDto>(sql, new { TenantId = tenantId, SessionId = sessionId });
        }

        public async Task<bool> UpsertEntriesAsync(Guid tenantId, Guid sessionId, List<AttendanceEntry> entries)
        {
            const string sql = @"
                MERGE dbo.AttendanceEntries AS target
                USING (SELECT @AttendanceSessionId AS AttendanceSessionId, @StudentId AS StudentId) AS source
                ON (target.AttendanceSessionId = source.AttendanceSessionId AND target.StudentId = source.StudentId AND target.TenantId = @TenantId)
                WHEN MATCHED THEN
                    UPDATE SET Status = @Status, Remarks = @Remarks, IsDeleted = 0, ModifiedOn = @CreatedOn, ModifiedByUserId = @CreatedByUserId
                WHEN NOT MATCHED THEN
                    INSERT (AttendanceEntryId, AttendanceSessionId, StudentId, TenantId, Status, Remarks, IsDeleted, CreatedOn, CreatedByUserId)
                    VALUES (@AttendanceEntryId, @AttendanceSessionId, @StudentId, @TenantId, @Status, @Remarks, 0, @CreatedOn, @CreatedByUserId);";

            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open) connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var entry in entries)
                {
                    if (entry.AttendanceEntryId == Guid.Empty)
                    {
                        entry.AttendanceEntryId = Guid.NewGuid();
                    }

                    await connection.ExecuteAsync(sql, new 
                    {
                        entry.AttendanceEntryId,
                        AttendanceSessionId = sessionId,
                        entry.StudentId,
                        TenantId = tenantId,
                        entry.Status,
                        entry.Remarks,
                        entry.CreatedOn,
                        entry.CreatedByUserId
                    }, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<StudentAttendanceHistoryDto>> GetStudentHistoryAsync(Guid tenantId, Guid studentId)
        {
            const string sql = @"
                SELECT s.Date, s.ClassId, c.Name AS ClassName, s.SectionId, sec.Name AS SectionName, e.Status, e.Remarks
                FROM dbo.AttendanceEntries e
                INNER JOIN dbo.AttendanceSessions s ON e.AttendanceSessionId = s.AttendanceSessionId
                INNER JOIN dbo.Classes c ON s.ClassId = c.ClassId
                INNER JOIN dbo.Sections sec ON s.SectionId = sec.SectionId
                WHERE e.TenantId = @TenantId AND e.StudentId = @StudentId AND e.IsDeleted = 0 AND s.IsDeleted = 0
                ORDER BY s.Date DESC;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<StudentAttendanceHistoryDto>(sql, new { TenantId = tenantId, StudentId = studentId });
        }

        public async Task<IEnumerable<ClassAttendanceSummaryDto>> GetClassSummaryAsync(Guid tenantId, Guid classId, DateTime? startDate = null, DateTime? endDate = null)
        {
            const string sql = @"
                SELECT 
                    s.Date,
                    SUM(CASE WHEN e.Status = 'P' THEN 1 ELSE 0 END) AS TotalPresent,
                    SUM(CASE WHEN e.Status = 'A' THEN 1 ELSE 0 END) AS TotalAbsent,
                    SUM(CASE WHEN e.Status = 'L' THEN 1 ELSE 0 END) AS TotalLate,
                    SUM(CASE WHEN e.Status = 'T' THEN 1 ELSE 0 END) AS TotalHalfDay
                FROM dbo.AttendanceSessions s
                INNER JOIN dbo.AttendanceEntries e ON s.AttendanceSessionId = e.AttendanceSessionId
                WHERE s.TenantId = @TenantId 
                  AND s.ClassId = @ClassId 
                  AND s.IsDeleted = 0 
                  AND e.IsDeleted = 0
                  AND (@StartDate IS NULL OR s.Date >= @StartDate)
                  AND (@EndDate IS NULL OR s.Date <= @EndDate)
                GROUP BY s.Date
                ORDER BY s.Date DESC;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<ClassAttendanceSummaryDto>(sql, new 
            { 
                TenantId = tenantId, 
                ClassId = classId, 
                StartDate = startDate?.Date, 
                EndDate = endDate?.Date 
            });
        }
    }
}
