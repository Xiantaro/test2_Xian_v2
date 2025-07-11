namespace test2.Models.ManagementModels.ZhongXian.BookQuery
{
    public class BookQueryModel
    {
        public string? book_ISBN { get; set; }
        public string? book_KeyWord { get; set; }
        public DateTime? book_initDate { get; set; }
        public DateTime? book_lastDate { get; set; }
        public int borrow_perPage { get; set; }
        public string? borrow_OrderDate { get; set; }
        public string? borrow_orderBy { get; set; }
    }
}
