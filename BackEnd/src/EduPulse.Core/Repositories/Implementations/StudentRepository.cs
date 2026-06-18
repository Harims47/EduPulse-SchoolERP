using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using EduPulse.Core.Database.Connection;
using EduPulse.Core.Domain.Entities;
using EduPulse.Core.Repositories.Interfaces;

namespace EduPulse.Core.Repositories.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public StudentRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Student>> GetAllAsync(Guid tenantId)
        {
            const string sql = @"
                SELECT StudentId, TenantId, AdmissionNo, FirstName, LastName, DateOfBirth, Gender, BloodGroup, GovernmentIdType, GovernmentIdNumber, SocialCategory, PhotoPath, AddressLine1, AddressLine2, City, State, Pincode, AdmissionDate, Status, ClassId, SectionId, IsDeleted, CreatedOn, CreatedByUserId, ModifiedOn, ModifiedByUserId
                FROM dbo.Students
                WHERE TenantId = @TenantId AND IsDeleted = 0
                ORDER BY FirstName ASC, LastName ASC;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Student>(sql, new { TenantId = tenantId });
        }

        public async Task<Student?> GetByIdAsync(Guid tenantId, Guid studentId)
        {
            const string sql = @"
                SELECT StudentId, TenantId, AdmissionNo, FirstName, LastName, DateOfBirth, Gender, BloodGroup, GovernmentIdType, GovernmentIdNumber, SocialCategory, PhotoPath, AddressLine1, AddressLine2, City, State, Pincode, AdmissionDate, Status, ClassId, SectionId, IsDeleted, CreatedOn, CreatedByUserId, ModifiedOn, ModifiedByUserId
                FROM dbo.Students
                WHERE TenantId = @TenantId AND StudentId = @StudentId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Student>(sql, new { TenantId = tenantId, StudentId = studentId });
        }

        public async Task<bool> ExistsByAdmissionNoAsync(Guid tenantId, string admissionNo, Guid? excludeId = null)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.Students
                WHERE TenantId = @TenantId 
                  AND AdmissionNo = @AdmissionNo 
                  AND IsDeleted = 0
                  AND (@ExcludeId IS NULL OR StudentId <> @ExcludeId);";

            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(sql, new 
            { 
                TenantId = tenantId, 
                AdmissionNo = admissionNo, 
                ExcludeId = excludeId 
            });
            return count > 0;
        }

        public async Task<Guid> CreateAsync(Student entity)
        {
            const string sql = @"
                INSERT INTO dbo.Students (
                    StudentId, TenantId, AdmissionNo, FirstName, LastName, DateOfBirth, Gender, BloodGroup, 
                    GovernmentIdType, GovernmentIdNumber, SocialCategory, PhotoPath, AddressLine1, AddressLine2, 
                    City, State, Pincode, AdmissionDate, Status, ClassId, SectionId, IsDeleted, CreatedOn, CreatedByUserId
                )
                OUTPUT INSERTED.StudentId
                VALUES (
                    @StudentId, @TenantId, @AdmissionNo, @FirstName, @LastName, @DateOfBirth, @Gender, @BloodGroup, 
                    @GovernmentIdType, @GovernmentIdNumber, @SocialCategory, @PhotoPath, @AddressLine1, @AddressLine2, 
                    @City, @State, @Pincode, @AdmissionDate, @Status, @ClassId, @SectionId, @IsDeleted, @CreatedOn, @CreatedByUserId
                );";

            if (entity.StudentId == Guid.Empty)
            {
                entity.StudentId = Guid.NewGuid();
            }

            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<Guid>(sql, entity);
        }

        public async Task<bool> UpdateAsync(Student entity)
        {
            const string sql = @"
                UPDATE dbo.Students
                SET AdmissionNo = @AdmissionNo,
                    FirstName = @FirstName,
                    LastName = @LastName,
                    DateOfBirth = @DateOfBirth,
                    Gender = @Gender,
                    BloodGroup = @BloodGroup,
                    GovernmentIdType = @GovernmentIdType,
                    GovernmentIdNumber = @GovernmentIdNumber,
                    SocialCategory = @SocialCategory,
                    PhotoPath = @PhotoPath,
                    AddressLine1 = @AddressLine1,
                    AddressLine2 = @AddressLine2,
                    City = @City,
                    State = @State,
                    Pincode = @Pincode,
                    AdmissionDate = @AdmissionDate,
                    Status = @Status,
                    ClassId = @ClassId,
                    SectionId = @SectionId,
                    ModifiedOn = @ModifiedOn,
                    ModifiedByUserId = @ModifiedByUserId
                WHERE TenantId = @TenantId AND StudentId = @StudentId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, entity);
            return rowsAffected > 0;
        }

        public async Task<bool> SoftDeleteAsync(Guid tenantId, Guid studentId, Guid deletedByUserId)
        {
            const string sql = @"
                UPDATE dbo.Students
                SET IsDeleted = 1,
                    ModifiedOn = SYSUTCDATETIME(),
                    ModifiedByUserId = @DeletedByUserId
                WHERE TenantId = @TenantId AND StudentId = @StudentId AND IsDeleted = 0;";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                TenantId = tenantId, 
                StudentId = studentId, 
                DeletedByUserId = deletedByUserId 
            });
            return rowsAffected > 0;
        }
    }
}
