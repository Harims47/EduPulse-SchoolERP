using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using EduPulse.Core.Database.Connection;
using EduPulse.Core.Domain.Entities;
using EduPulse.Core.Repositories.Interfaces;

namespace EduPulse.Core.Repositories.Implementations
{
    public class StudentGuardianRepository : IStudentGuardianRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public StudentGuardianRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<StudentGuardian>> GetGuardiansByStudentIdAsync(Guid tenantId, Guid studentId)
        {
            const string sql = @"
                SELECT StudentId, GuardianId, TenantId, RelationshipType, IsPrimaryContact, IsBillingResponsible, IsDeleted, CreatedOn, CreatedByUserId, ModifiedOn, ModifiedByUserId
                FROM dbo.StudentGuardians
                WHERE TenantId = @TenantId AND StudentId = @StudentId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<StudentGuardian>(sql, new { TenantId = tenantId, StudentId = studentId });
        }

        public async Task<StudentGuardian?> GetRelationAsync(Guid tenantId, Guid studentId, Guid guardianId)
        {
            const string sql = @"
                SELECT StudentId, GuardianId, TenantId, RelationshipType, IsPrimaryContact, IsBillingResponsible, IsDeleted, CreatedOn, CreatedByUserId, ModifiedOn, ModifiedByUserId
                FROM dbo.StudentGuardians
                WHERE TenantId = @TenantId AND StudentId = @StudentId AND GuardianId = @GuardianId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<StudentGuardian>(sql, new { TenantId = tenantId, StudentId = studentId, GuardianId = guardianId });
        }

        public async Task<bool> CreateRelationAsync(StudentGuardian entity)
        {
            const string sql = @"
                INSERT INTO dbo.StudentGuardians (
                    StudentId, GuardianId, TenantId, RelationshipType, IsPrimaryContact, IsBillingResponsible, IsDeleted, CreatedOn, CreatedByUserId
                )
                VALUES (
                    @StudentId, @GuardianId, @TenantId, @RelationshipType, @IsPrimaryContact, @IsBillingResponsible, @IsDeleted, @CreatedOn, @CreatedByUserId
                );";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, entity);
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateRelationAsync(StudentGuardian entity)
        {
            const string sql = @"
                UPDATE dbo.StudentGuardians
                SET RelationshipType = @RelationshipType,
                    IsPrimaryContact = @IsPrimaryContact,
                    IsBillingResponsible = @IsBillingResponsible,
                    ModifiedOn = @ModifiedOn,
                    ModifiedByUserId = @ModifiedByUserId
                WHERE TenantId = @TenantId AND StudentId = @StudentId AND GuardianId = @GuardianId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, entity);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteRelationAsync(Guid tenantId, Guid studentId, Guid guardianId, Guid deletedByUserId)
        {
            const string sql = @"
                UPDATE dbo.StudentGuardians
                SET IsDeleted = 1,
                    ModifiedOn = SYSUTCDATETIME(),
                    ModifiedByUserId = @DeletedByUserId
                WHERE TenantId = @TenantId AND StudentId = @StudentId AND GuardianId = @GuardianId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                TenantId = tenantId, 
                StudentId = studentId, 
                GuardianId = guardianId, 
                DeletedByUserId = deletedByUserId 
            });
            return rowsAffected > 0;
        }
    }
}
