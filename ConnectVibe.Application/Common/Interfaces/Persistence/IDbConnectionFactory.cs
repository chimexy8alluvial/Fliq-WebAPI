using System.Data;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
