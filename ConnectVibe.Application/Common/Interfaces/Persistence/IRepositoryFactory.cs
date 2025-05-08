using Fliq.Domain.Entities.Interfaces;
using System.Collections.Generic;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IRepositoryFactory
    {
        IGenericRepository<T>? GetRepository<T>() where T : class, IApprovableContent;
    }
}
