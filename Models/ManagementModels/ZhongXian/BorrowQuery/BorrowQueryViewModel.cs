using test2.Models.ManagementModels.ZhongXian.Normal;

namespace test2.Models.ManagementModels.ZhongXian.BorrowQuery
{
    public class BorrowQueryViewModel
    {
        public List<BorrowQueryResultDTO>? BorrowQueryDTOs { get; set; }
        public List<PageCount>? PageCounts { get; set; }
        // 資料數量
        //public int TotalCount { get; set; }
        //// 總頁數
        //public int TotalPage => (int)Math.Ceiling((double)TotalCount / PageSize);
        //// 目前頁數
        //public int CurrentPage { get; set; }
        //// 每頁數量
        //public int PageSize { get; set; }
        //// 起點筆數
        //public int FromIndex => (CurrentPage - 1) * PageSize + 1;
        //// 到幾筆數
        //public int ToIndex => Math.Min(CurrentPage * PageSize, TotalCount);
    }
}
