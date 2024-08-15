using System.Data;

namespace ConnectVibe.Application.Common.Interfaces.Persistence
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
