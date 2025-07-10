using System.ComponentModel.DataAnnotations;

namespace test2.Models.ManagementModels.ZhongXian.RegisterBook
{
    public class BookAddsClass
    {
        [Required(ErrorMessage = "請輸入ISNBM!")]
        public string? BooksAdded_ISBM { get; set; }

        public int BooksAdded_Type { get; set; }

        [Required(ErrorMessage = "請輸入書本名稱!")]
        public string? BooksAdded_Title { get; set; }

        public int BooksAdded_leng { get; set; }

        [Required(ErrorMessage = "請輸入作者名稱!")]
        public string? BooksAdded_author { get; set; }

        public string? BooksAdded_translator { get; set; }

        [Required(ErrorMessage = "請輸入出版商!")]
        public string? BooksAdded_pushier { get; set; }

        [Required(ErrorMessage = "請輸入出版日期")]
        public DateTime BooksAdded_puDate { get; set; }

        public int BooksAdded_inCount { get; set; }
        public string? BooksAdded_Dec { get; set; }
    }
}
