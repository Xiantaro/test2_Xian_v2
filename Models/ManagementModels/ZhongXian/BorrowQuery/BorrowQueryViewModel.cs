using test2.Models.ManagementModels.ZhongXian.Appoimtment;
using test2.Models.ManagementModels.ZhongXian.AppoimtmentQuery;
using test2.Models.ManagementModels.ZhongXian.BookQuery;
using test2.Models.ManagementModels.ZhongXian.Normal;

namespace test2.Models.ManagementModels.ZhongXian.BorrowQuery
{
    public class BorrowQueryViewModel
    {
        public List<BorrowQueryResultDTO>? BorrowQueryDTOs { get; set; }
        public List<AppoimtmentKeywordDTO>? AppoimtmentKeywordShows { get; set; }
        public List<BookQueryDTO>? BookQueryDTOs { get; set; }
        public List<AppointmentQueryResultDTO>? AppointmentQueryResultDTOs { get; set; }
        public List<PageCount>? PageCounts { get; set; }
    }
}
