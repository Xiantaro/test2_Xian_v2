using Microsoft.Identity.Client;
using test2.Models.ManagementModels.ZhongXian.BorrowQuery;

namespace test2.Models.ManagementModels.ZhongXian.AppoimtmentQuery
{
    public class BorrowQueryViewModel
    {
        public List<BorrowQueryResultDTO>? BorrowQueryResultDTOs { get; set; }
        // 總頁數
        public int TotalPage { get; set; }
        // 總筆數
        public int TotalCount { get; set; }
        // 目前頁數
        public int CurrentPage { get; set; }
        // 每頁數量
        public int perPage { get; set; }
        // 從0筆
        public int FromIndex { get; set; }
        // 到0筆
        public int ToIndex { get; set; }
    }
}
