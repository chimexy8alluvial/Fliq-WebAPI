using Fliq.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.Explore.Common
{
    public record ExploreEventsRequest(
        int PageNumber = 1, int PageSize = 5): PaginationRequest(PageNumber, PageSize);

}
