using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using EduPulse.Core.Database.Connection;
using EduPulse.Core.Domain.Entities;
using EduPulse.Core.Repositories.Interfaces;

namespace EduPulse.Core.Repositories.Implementations
{
    public class SectionRepository : ISectionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SectionRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Section>> GetAllAsync(Guid tenantId)
        {
            const string sql = @"
                SELECT SectionId, TenantId, Name, IsDeleted, CreatedOn, CreatedByUserId, ModifiedOn, ModifiedByUserId
                FROM dbo.Sections
                WHERE TenantId = @TenantId AND IsDeleted = 0
                ORDER BY Name ASC;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Section>(sql, new { TenantId = tenantId });
        }

        public async Task<Section?> GetByIdAsync(Guid tenantId, Guid sectionId)
        {
            const string sql = @"
                SELECT SectionId, TenantId, Name, IsDeleted, CreatedOn, CreatedByUserId, ModifiedOn, ModifiedByUserId
                FROM dbo.Sections
                WHERE TenantId = @TenantId AND SectionId = @SectionId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Section>(sql, new { TenantId = tenantId, SectionId = sectionId });
        }

        public async Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? excludeId = null)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.Sections
                WHERE TenantId = @TenantId 
                  AND Name = @Name 
                  AND IsDeleted = 0
                  AND (@ExcludeId IS NULL OR SectionId <> @ExcludeId);";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(sql, new 
            { 
                TenantId = tenantId, 
                Name = name, 
                ExcludeId = excludeId 
            });
            return count > 0;
        }

        public async Task<Guid> CreateAsync(Section entity)
        {
            const string sql = @"
                INSERT INTO dbo.Sections (SectionId, TenantId, Name, IsDeleted, CreatedOn, CreatedByUserId)
                OUTPUT INSERTED.SectionId
                VALUES (@SectionId, @TenantId, @Name, @IsDeleted, @CreatedOn, @CreatedByUserId);";

            if (entity.SectionId == Guid.Empty)
            {
                entity.SectionId = Guid.NewGuid();
            }

            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<Guid>(sql, entity);
        }

        public async Task<bool> UpdateAsync(Section entity)
        {
            const string sql = @"
                UPDATE dbo.Sections
                SET Name = @Name,
                    ModifiedOn = @ModifiedOn,
                    ModifiedByUserId = @ModifiedByUserId
                WHERE TenantId = @TenantId AND SectionId = @SectionId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, entity);
            return rowsAffected > 0;
        }

        public async Task<bool> SoftDeleteAsync(Guid tenantId, Guid sectionId, Guid deletedByUserId)
        {
            const string sql = @"
                UPDATE dbo.Sections
                SET IsDeleted = 1,
                    ModifiedOn = SYSUTCDATETIME(),
                    ModifiedByUserId = @DeletedByUserId
                WHERE TenantId = @TenantId AND SectionId = @SectionId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                TenantId = tenantId, 
                SectionId = sectionId, 
                DeletedByUserId = deletedByUserId 
            });
            return rowsAffected > 0;
        }
    }
}
