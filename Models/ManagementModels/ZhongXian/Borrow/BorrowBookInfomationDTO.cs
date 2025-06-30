namespace test2.Models.ManagementModels.ZhongXian.Borrow
{
    public class BorrowBookInfomationDTO
    {
        public int bookId { get; set; }   
        public string title { get; set; } = null!;
        public string author { get; set; } = null!;
        public string translator { get; set; } = null!;
        public string? type { get; set; }
        public string publisher { get; set; } = null!;
        public DateTime publishDate { get; set; }
        public string? collectionImg { get; set; }
        public string bookStatus { get; set; } = null!;
    }
}
