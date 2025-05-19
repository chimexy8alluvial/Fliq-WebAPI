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

        public PaginationResponse(IEnumerable<T> data, int totalCount, int pageNumber, int pageSize)
        {
            Data = data?.ToList() ?? new List<T>();
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
