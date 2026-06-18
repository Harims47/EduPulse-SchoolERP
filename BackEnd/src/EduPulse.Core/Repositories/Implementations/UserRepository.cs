using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EduPulse.Core.Database.Connection;
using EduPulse.Core.Domain.Entities;
using EduPulse.Core.Repositories.Interfaces;

namespace EduPulse.Core.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<UserWithRoles?> GetByEmailAsync(string email)
        {
            const string sqlUser = @"
                SELECT UserId, TenantId, Email, PasswordHash, IsActive, IsDeleted, CreatedOn
                FROM dbo.Users
                WHERE Email = @Email AND IsDeleted = 0;";

            const string sqlRoles = @"
                SELECT RoleId
                FROM dbo.UserRoles
                WHERE UserId = @UserId;";

            using var connection = _connectionFactory.CreateConnection();
            
            var user = await connection.QueryFirstOrDefaultAsync<User>(sqlUser, new { Email = email });
            if (user == null)
            {
                return null;
            }

            var roles = await connection.QueryAsync<string>(sqlRoles, new { UserId = user.UserId });

            return new UserWithRoles
            {
                User = user,
                Roles = roles.ToList()
            };
        }
    }
}
