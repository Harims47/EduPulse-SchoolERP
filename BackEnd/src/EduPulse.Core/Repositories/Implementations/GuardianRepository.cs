using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using EduPulse.Core.Database.Connection;
using EduPulse.Core.Domain.Entities;
using EduPulse.Core.Repositories.Interfaces;

namespace EduPulse.Core.Repositories.Implementations
{
    public class GuardianRepository : IGuardianRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GuardianRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Guardian>> GetAllAsync(Guid tenantId)
        {
            const string sql = @"
                SELECT GuardianId, TenantId, UserId, FirstName, LastName, Phone, Email, IsDeleted, CreatedOn, CreatedByUserId, ModifiedOn, ModifiedByUserId
                FROM dbo.Guardians
                WHERE TenantId = @TenantId AND IsDeleted = 0
                ORDER BY FirstName ASC, LastName ASC;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Guardian>(sql, new { TenantId = tenantId });
        }

        public async Task<Guardian?> GetByIdAsync(Guid tenantId, Guid guardianId)
        {
            const string sql = @"
                SELECT GuardianId, TenantId, UserId, FirstName, LastName, Phone, Email, IsDeleted, CreatedOn, CreatedByUserId, ModifiedOn, ModifiedByUserId
                FROM dbo.Guardians
                WHERE TenantId = @TenantId AND GuardianId = @GuardianId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Guardian>(sql, new { TenantId = tenantId, GuardianId = guardianId });
        }

        public async Task<bool> ExistsByPhoneAsync(Guid tenantId, string phone, Guid? excludeId = null)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.Guardians
                WHERE TenantId = @TenantId 
                  AND Phone = @Phone 
                  AND IsDeleted = 0
                  AND (@ExcludeId IS NULL OR GuardianId <> @ExcludeId);";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(sql, new 
            { 
                TenantId = tenantId, 
                Phone = phone, 
                ExcludeId = excludeId 
            });
            return count > 0;
        }

        public async Task<Guid> CreateAsync(Guardian entity)
        {
            const string sql = @"
                INSERT INTO dbo.Guardians (
                    GuardianId, TenantId, UserId, FirstName, LastName, Phone, Email, IsDeleted, CreatedOn, CreatedByUserId
                )
                OUTPUT INSERTED.GuardianId
                VALUES (
                    @GuardianId, @TenantId, @UserId, @FirstName, @LastName, @Phone, @Email, @IsDeleted, @CreatedOn, @CreatedByUserId
                );";

            if (entity.GuardianId == Guid.Empty)
            {
                entity.GuardianId = Guid.NewGuid();
            }

            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<Guid>(sql, entity);
        }

        public async Task<bool> UpdateAsync(Guardian entity)
        {
            const string sql = @"
                UPDATE dbo.Guardians
                SET UserId = @UserId,
                    FirstName = @FirstName,
                    LastName = @LastName,
                    Phone = @Phone,
                    Email = @Email,
                    ModifiedOn = @ModifiedOn,
                    ModifiedByUserId = @ModifiedByUserId
                WHERE TenantId = @TenantId AND GuardianId = @GuardianId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, entity);
            return rowsAffected > 0;
        }

        public async Task<bool> SoftDeleteAsync(Guid tenantId, Guid guardianId, Guid deletedByUserId)
        {
            const string sql = @"
                UPDATE dbo.Guardians
                SET IsDeleted = 1,
                    ModifiedOn = SYSUTCDATETIME(),
                    ModifiedByUserId = @DeletedByUserId
                WHERE TenantId = @TenantId AND GuardianId = @GuardianId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                TenantId = tenantId, 
                GuardianId = guardianId, 
                DeletedByUserId = deletedByUserId 
            });
            return rowsAffected > 0;
        }
    }
}
