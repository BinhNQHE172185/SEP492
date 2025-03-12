namespace LMCM_BE.DTOs.ShareDtos
{
    public class PagingRequest
    {
        public Guid? Id { get; set; }
        public string? SearchKey { get; set; } // Từ khóa tìm kiếm (có thể null)
        public int pageIndex { get; set; } = 1; // Trang mặc định là 1
        public int PageSize { get; set; } = 10; // Số lượng bản ghi mỗi trang
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
    }
}
