

namespace Fliq.Application.Common.Pagination
{
    public class PaginationResponse<T>
    {
        public IEnumerable<T> Data { get; }
        public int TotalCount { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;

        public PaginationResponse(IEnumerable<T> Data, int totalCount, int pageNumber, int pageSize)
        {
            Data = Data ?? throw new ArgumentNullException(nameof(Data));
            TotalCount = totalCount >= 0 ? totalCount : throw new ArgumentException("TotalCount cannot be negative.");
            PageNumber = pageNumber >= 1 ? pageNumber : throw new ArgumentException("PageNumber must be at least 1.");
            PageSize = pageSize >= 1 ? pageSize : throw new ArgumentException("PageSize must be at least 1.");
        }
    }
}
