using System.Data;

namespace EduPulse.Core.Database.Connection
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
