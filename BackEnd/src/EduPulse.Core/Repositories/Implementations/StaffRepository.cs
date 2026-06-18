using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using EduPulse.Core.Database.Connection;
using EduPulse.Core.Domain.Entities;
using EduPulse.Core.Repositories.Interfaces;

namespace EduPulse.Core.Repositories.Implementations
{
    public class StaffRepository : IStaffRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public StaffRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Staff>> GetAllAsync(Guid tenantId)
        {
            const string sql = @"
                SELECT StaffId, TenantId, UserId, EmployeeCode, FirstName, LastName, Phone, Designation, PhotoPath, IsActive, IsDeleted, CreatedOn, CreatedByUserId, ModifiedOn, ModifiedByUserId
                FROM dbo.Staff
                WHERE TenantId = @TenantId AND IsDeleted = 0
                ORDER BY FirstName ASC, LastName ASC;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Staff>(sql, new { TenantId = tenantId });
        }

        public async Task<Staff?> GetByIdAsync(Guid tenantId, Guid staffId)
        {
            const string sql = @"
                SELECT StaffId, TenantId, UserId, EmployeeCode, FirstName, LastName, Phone, Designation, PhotoPath, IsActive, IsDeleted, CreatedOn, CreatedByUserId, ModifiedOn, ModifiedByUserId
                FROM dbo.Staff
                WHERE TenantId = @TenantId AND StaffId = @StaffId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Staff>(sql, new { TenantId = tenantId, StaffId = staffId });
        }

        public async Task<bool> ExistsByEmployeeCodeAsync(Guid tenantId, string employeeCode, Guid? excludeId = null)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.Staff
                WHERE TenantId = @TenantId 
                  AND EmployeeCode = @EmployeeCode 
                  AND IsDeleted = 0
                  AND (@ExcludeId IS NULL OR StaffId <> @ExcludeId);";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(sql, new 
            { 
                TenantId = tenantId, 
                EmployeeCode = employeeCode, 
                ExcludeId = excludeId 
            });
            return count > 0;
        }

        public async Task<Guid> CreateAsync(Staff entity)
        {
            const string sql = @"
                INSERT INTO dbo.Staff (StaffId, TenantId, UserId, EmployeeCode, FirstName, LastName, Phone, Designation, PhotoPath, IsActive, IsDeleted, CreatedOn, CreatedByUserId)
                OUTPUT INSERTED.StaffId
                VALUES (@StaffId, @TenantId, @UserId, @EmployeeCode, @FirstName, @LastName, @Phone, @Designation, @PhotoPath, @IsActive, @IsDeleted, @CreatedOn, @CreatedByUserId);";

            if (entity.StaffId == Guid.Empty)
            {
                entity.StaffId = Guid.NewGuid();
            }

            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<Guid>(sql, entity);
        }

        public async Task<bool> UpdateAsync(Staff entity)
        {
            const string sql = @"
                UPDATE dbo.Staff
                SET UserId = @UserId,
                    EmployeeCode = @EmployeeCode,
                    FirstName = @FirstName,
                    LastName = @LastName,
                    Phone = @Phone,
                    Designation = @Designation,
                    PhotoPath = @PhotoPath,
                    IsActive = @IsActive,
                    ModifiedOn = @ModifiedOn,
                    ModifiedByUserId = @ModifiedByUserId
                WHERE TenantId = @TenantId AND StaffId = @StaffId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, entity);
            return rowsAffected > 0;
        }

        public async Task<bool> SoftDeleteAsync(Guid tenantId, Guid staffId, Guid deletedByUserId)
        {
            const string sql = @"
                UPDATE dbo.Staff
                SET IsDeleted = 1,
                    ModifiedOn = SYSUTCDATETIME(),
                    ModifiedByUserId = @DeletedByUserId
                WHERE TenantId = @TenantId AND StaffId = @StaffId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                TenantId = tenantId, 
                StaffId = staffId, 
                DeletedByUserId = deletedByUserId 
            });
            return rowsAffected > 0;
        }
    }
}
