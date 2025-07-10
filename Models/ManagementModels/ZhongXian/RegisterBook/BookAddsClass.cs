using System.ComponentModel.DataAnnotations;

namespace test2.Models.ManagementModels.ZhongXian.RegisterBook
{
    public class BookAddsClass
    {
        public string? BooksAdded_ISBM { get; set; }
        public int BooksAdded_Type { get; set; }
        public string? BooksAdded_Title { get; set; }
        public int BooksAdded_leng { get; set; }
        public string? BooksAdded_authorName { get; set; }
        public int BooksAdded_authorId { get; set; }
        public string? BooksAdded_translator { get; set; }
        public string? BooksAdded_pushier { get; set; }
        public DateTime BooksAdded_puDate { get; set; }
        public int BooksAdded_inCount { get; set; }
        public string? BooksAdded_Dec { get; set; }
    }
}
