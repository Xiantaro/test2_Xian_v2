using Microsoft.Identity.Client;
using test2.Models.ManagementModels.ZhongXian.AppoimtmentQuery;

namespace test2.Models.ManagementModels.ZhongXian.AppoimtmentQuery
{
    public class AppointmentQueryViewModel
    {
        public List<AppointmentQueryResultDTO>? AppointmentQueryResultDTOs { get; set; }
        // 總頁數
        public int TotalPage => (int)Math.Ceiling((double)TotalCount / perPage);
        // 總筆數
        public int TotalCount { get; set; }
        // 目前頁數
        public int CurrentPage { get; set; }
        // 每頁數量
        public int perPage { get; set; }
        // 從0筆
        public int FromIndex => (CurrentPage - 1) * perPage + 1;
        // 到0筆
        public int ToIndex => Math.Min((CurrentPage * perPage), TotalCount);
    }
}
