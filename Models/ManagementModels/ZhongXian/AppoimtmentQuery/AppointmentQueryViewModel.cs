using Microsoft.Identity.Client;
using test2.Models.ManagementModels.ZhongXian.AppoimtmentQuery;
using test2.Models.ManagementModels.ZhongXian.Normal;

namespace test2.Models.ManagementModels.ZhongXian.AppoimtmentQuery
{
    public class AppointmentQueryViewModel
    {
        public List<AppointmentQueryResultDTO>? AppointmentQueryResultDTOs { get; set; }
        public List<PageCount>? PageCounts { get; set; }
    }
}
