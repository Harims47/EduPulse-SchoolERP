using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using EduPulse.Core.Database.Connection;
using EduPulse.Core.Domain.Entities;
using EduPulse.Core.Repositories.Interfaces;

namespace EduPulse.Core.Repositories.Implementations
{
    public class ClassRepository : IClassRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ClassRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Class>> GetAllAsync(Guid tenantId)
        {
            const string sql = @"
                SELECT ClassId, TenantId, Name, SortOrder, IsDeleted, CreatedOn, CreatedByUserId, ModifiedOn, ModifiedByUserId
                FROM dbo.Classes
                WHERE TenantId = @TenantId AND IsDeleted = 0
                ORDER BY SortOrder ASC, Name ASC;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Class>(sql, new { TenantId = tenantId });
        }

        public async Task<Class?> GetByIdAsync(Guid tenantId, Guid classId)
        {
            const string sql = @"
                SELECT ClassId, TenantId, Name, SortOrder, IsDeleted, CreatedOn, CreatedByUserId, ModifiedOn, ModifiedByUserId
                FROM dbo.Classes
                WHERE TenantId = @TenantId AND ClassId = @ClassId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Class>(sql, new { TenantId = tenantId, ClassId = classId });
        }

        public async Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? excludeId = null)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.Classes
                WHERE TenantId = @TenantId 
                  AND Name = @Name 
                  AND IsDeleted = 0
                  AND (@ExcludeId IS NULL OR ClassId <> @ExcludeId);";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(sql, new 
            { 
                TenantId = tenantId, 
                Name = name, 
                ExcludeId = excludeId 
            });
            return count > 0;
        }

        public async Task<Guid> CreateAsync(Class entity)
        {
            const string sql = @"
                INSERT INTO dbo.Classes (ClassId, TenantId, Name, SortOrder, IsDeleted, CreatedOn, CreatedByUserId)
                OUTPUT INSERTED.ClassId
                VALUES (@ClassId, @TenantId, @Name, @SortOrder, @IsDeleted, @CreatedOn, @CreatedByUserId);";

            if (entity.ClassId == Guid.Empty)
            {
                entity.ClassId = Guid.NewGuid();
            }

            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<Guid>(sql, entity);
        }

        public async Task<bool> UpdateAsync(Class entity)
        {
            const string sql = @"
                UPDATE dbo.Classes
                SET Name = @Name,
                    SortOrder = @SortOrder,
                    ModifiedOn = @ModifiedOn,
                    ModifiedByUserId = @ModifiedByUserId
                WHERE TenantId = @TenantId AND ClassId = @ClassId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, entity);
            return rowsAffected > 0;
        }

        public async Task<bool> SoftDeleteAsync(Guid tenantId, Guid classId, Guid deletedByUserId)
        {
            const string sql = @"
                UPDATE dbo.Classes
                SET IsDeleted = 1,
                    ModifiedOn = SYSUTCDATETIME(),
                    ModifiedByUserId = @DeletedByUserId
                WHERE TenantId = @TenantId AND ClassId = @ClassId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                TenantId = tenantId, 
                ClassId = classId, 
                DeletedByUserId = deletedByUserId 
            });
            return rowsAffected > 0;
        }
    }
}
