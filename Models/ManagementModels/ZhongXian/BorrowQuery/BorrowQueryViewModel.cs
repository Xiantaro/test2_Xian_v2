namespace test2.Models.ManagementModels.ZhongXian.BorrowQuery
{
    public class BorrowQueryViewModel
    {
        public List<BorrowQueryResultDTO>? BorrowQueryDTOs { get; set; }
        // 資料數量
        public int TotalCount { get; set; }
        // 總頁數
        public int TotalPage { get; set; }
        // 目前頁數
        public int CurrentPage { get; set; }
        // 起點筆數
        public int FromIndex { get; set; }
        // 到幾筆數
        public int ToIndex { get; set; }
    }
}
