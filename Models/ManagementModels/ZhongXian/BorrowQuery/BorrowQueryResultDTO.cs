namespace test2.Models.ManagementModels.ZhongXian.BorrowQuery
{
    public class BorrowQueryResultDTO
    {
        public int borrowId { get; set; }
        public int bookId { get; set; }
        public string? title { get; set; }
        public int? cId { get; set; }
        public string? cName { get; set; }
        public DateTime? borrowDate { get; set; }
        public DateTime? dueDateB { get; set; }
        public DateTime? returnDate { get; set; }
        public string? borrowStatus {get;set;}
    }
}
