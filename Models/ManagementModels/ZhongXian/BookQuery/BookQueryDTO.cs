namespace test2.Models.ManagementModels.ZhongXian.BookQuery
{
    public class BookQueryDTO
    {
        //id
        public int collectionId { get; set; }
        // ISBn
        public string? isbn { get; set; }
        //封面
        public byte[] collectionImg { get; set; } = null!;
        //書名
        public string title { get; set; } = null!;
        // 作者
        public string author { get; set; } = null!;
        // 簡介
        public string? collectionDesc { get; set; }
        // 出版社
        public string publisher { get; set; } = null!;
        // 出版年份
        public DateTime publishDate { get; set; }
        //  藏書量
        public int NumberOfBook { get; set; }
    }
}
