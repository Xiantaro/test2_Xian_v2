namespace test2.Models.ManagementModels.ZhongXian.BookQuery
{
    public class BookQueryDTO
    {
        public int collectionId { get; set; }
        public string? isbn { get; set; }
        public byte[] collectionImg { get; set; } = null!;
        public string title { get; set; } = null!;
        public string author { get; set; } = null!;
        public string? collectionDesc { get; set; }
        public string publisher { get; set; } = null!;
        public DateTime publishDate { get; set; }
        public int NumberOfBook { get; set; }
    }
}
