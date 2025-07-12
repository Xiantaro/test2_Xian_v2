using test2.Models.ManagementModels.ZhongXian.Appoimtment;
using test2.Models.ManagementModels.ZhongXian.AppoimtmentQuery;
using test2.Models.ManagementModels.ZhongXian.BookQuery;
using test2.Models.ManagementModels.ZhongXian.BorrowQuery;

namespace test2.Models.ManagementModels.ZhongXian.Normal
{
    public class QueryViewModel
    {
        public List<BorrowQueryResultDTO>? BorrowQueryDTOs { get; set; }
        public List<AppoimtmentKeywordDTO>? AppoimtmentKeywordDTOs { get; set; }
        public List<BookQueryDTO>? BookQueryDTOs { get; set; }
        public List<AppointmentQueryResultDTO>? AppointmentQueryResultDTOs { get; set; }
        public List<PageCount>? PageCounts { get; set; }
    }
}
