namespace test2.Models.ManagementModels.ZhongXian.AppoimtmentQuery
{
    public class appointmentQueryResultDTO
    {
        public int appointmentId{get;set; }
        public int cid { get; set; }
        public string? bookCode { get; set; }
        public DateTime? appointmentDate { get; set; }
        public DateTime? appointmentDue { get; set; }
        public string? appointmentStatus { get; set; }
    }
}
