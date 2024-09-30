using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.Common.Pagination
{
    public record PaginationResponse<T>(
        IEnumerable<T>? Data,
        int TotalCount,
        int PageNumber,
        int PageSize);
}
