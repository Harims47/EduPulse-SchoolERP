using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using EduPulse.Core.Database.Connection;
using EduPulse.Core.Domain.Entities;
using EduPulse.Core.Repositories.Interfaces;

namespace EduPulse.Core.Repositories.Implementations
{
    public class AcademicYearRepository : IAcademicYearRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AcademicYearRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<AcademicYear>> GetAllAsync(Guid tenantId)
        {
            const string sql = @"
                SELECT AcademicYearId, TenantId, Name, StartDate, EndDate, IsDeleted, CreatedOn, CreatedByUserId, ModifiedOn, ModifiedByUserId
                FROM dbo.AcademicYears
                WHERE TenantId = @TenantId AND IsDeleted = 0
                ORDER BY StartDate DESC;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<AcademicYear>(sql, new { TenantId = tenantId });
        }

        public async Task<AcademicYear?> GetByIdAsync(Guid tenantId, Guid academicYearId)
        {
            const string sql = @"
                SELECT AcademicYearId, TenantId, Name, StartDate, EndDate, IsDeleted, CreatedOn, CreatedByUserId, ModifiedOn, ModifiedByUserId
                FROM dbo.AcademicYears
                WHERE TenantId = @TenantId AND AcademicYearId = @AcademicYearId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<AcademicYear>(sql, new { TenantId = tenantId, AcademicYearId = academicYearId });
        }

        public async Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? excludeId = null)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.AcademicYears
                WHERE TenantId = @TenantId 
                  AND Name = @Name 
                  AND IsDeleted = 0
                  AND (@ExcludeId IS NULL OR AcademicYearId <> @ExcludeId);";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(sql, new 
            { 
                TenantId = tenantId, 
                Name = name, 
                ExcludeId = excludeId 
            });
            return count > 0;
        }

        public async Task<Guid> CreateAsync(AcademicYear entity)
        {
            const string sql = @"
                INSERT INTO dbo.AcademicYears (AcademicYearId, TenantId, Name, StartDate, EndDate, IsDeleted, CreatedOn, CreatedByUserId)
                OUTPUT INSERTED.AcademicYearId
                VALUES (@AcademicYearId, @TenantId, @Name, @StartDate, @EndDate, @IsDeleted, @CreatedOn, @CreatedByUserId);";

            if (entity.AcademicYearId == Guid.Empty)
            {
                entity.AcademicYearId = Guid.NewGuid();
            }

            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<Guid>(sql, entity);
        }

        public async Task<bool> UpdateAsync(AcademicYear entity)
        {
            const string sql = @"
                UPDATE dbo.AcademicYears
                SET Name = @Name,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    ModifiedOn = @ModifiedOn,
                    ModifiedByUserId = @ModifiedByUserId
                WHERE TenantId = @TenantId AND AcademicYearId = @AcademicYearId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, entity);
            return rowsAffected > 0;
        }

        public async Task<bool> SoftDeleteAsync(Guid tenantId, Guid academicYearId, Guid deletedByUserId)
        {
            const string sql = @"
                UPDATE dbo.AcademicYears
                SET IsDeleted = 1,
                    ModifiedOn = SYSUTCDATETIME(),
                    ModifiedByUserId = @DeletedByUserId
                WHERE TenantId = @TenantId AND AcademicYearId = @AcademicYearId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                TenantId = tenantId, 
                AcademicYearId = academicYearId, 
                DeletedByUserId = deletedByUserId 
            });
            return rowsAffected > 0;
        }
    }
}
