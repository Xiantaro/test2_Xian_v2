namespace test2.Models.ManagementModels.ZhongXian.BorrowQuery
{
    public class BorrowQueryFilter
    {
        // 借閱查詢篩選器
        public int? borrow_BorrowID {get;set;}
        public int? borrow_UserID { get; set; }
        public string? borrow_bookCode { get; set; }
        public string? borrow_state { get; set; } = "ALL";
        public DateTime? borrow_initDate { get; set; }
        public DateTime? borrow_lastDate { get; set; }
        public int borrow_perPage { get; set; } = 10;
        public string? borrow_OrderDate { get; set; } = "borrowDate";
        public string? borrow_orderBy { get; set; } = "desc";
        public int page { get; set; } = 1;
    }
}
