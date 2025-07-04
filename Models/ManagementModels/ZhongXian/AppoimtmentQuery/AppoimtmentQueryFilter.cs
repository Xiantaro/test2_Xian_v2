namespace test2.Models.ManagementModels.ZhongXian.AppoimtmentQuery
{
    public class AppoimtmentQueryFilter
    {
        public int? appointment_reservationId { get; set; }
        public int? appointment_UserID { get; set; }
        public string? appointment_bookCode { get; set; }
        public DateTime? appointment_initDate { get; set; }
        public DateTime? appointment_lastDate { get; set; }
        public string? appointment_state { get; set; }
        public int appointment_perPage { get; set; }
        public string? appointment_orderDate { get; set; }
        public int page { get; set; }
    }
}
