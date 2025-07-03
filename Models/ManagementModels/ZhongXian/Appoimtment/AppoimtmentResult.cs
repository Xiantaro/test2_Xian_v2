using Microsoft.Identity.Client;
namespace test2.Models.ManagementModels.ZhongXian.Appoimtment
{
    public class AppoimtmentResult
    {
        public List<AppoimtmentKeywordShow>? AppoimtmentKeywordShows { get; set; }
        // 總資料量
        public int TotalCount { get; set; }
        // 總頁數
        public int TotalPage => (int)Math.Ceiling((double)TotalCount / perPage);
        // 目前頁數
        public int CurrentPage { get; set; }
        // 每頁數量
        public int perPage { get; set; }
        // 第X幾筆
        public int FromIndex => (CurrentPage - 1)  * perPage + 1;
        // 到每頁最後一筆
        public int ToIndex => Math.Min(perPage * CurrentPage, TotalCount);
        // 狀態
        public string? status { get; set; }
    }
}
