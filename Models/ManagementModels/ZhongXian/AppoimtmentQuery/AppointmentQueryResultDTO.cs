namespace test2.Models.ManagementModels.ZhongXian.AppoimtmentQuery
{
    public class AppointmentQueryResultDTO
    {
        public int appointmentId{get;set; }
        public string? bookCode { get; set; }
        public string? title { get; set; }
        public int cid { get; set; }
        public DateTime? appointmentDate { get; set; }
        public DateTime? appointmentDue { get; set; }
        public string? appointmentStatus { get; set; }
    }
}
