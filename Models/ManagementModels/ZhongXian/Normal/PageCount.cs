namespace test2.Models.ManagementModels.ZhongXian.Normal
{
    public class PageCount
    {
        public int TotalPage => (int)Math.Ceiling((double)TotalCount / perPage);
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int perPage { get; set; }
        public int FromIndex => (CurrentPage - 1) * perPage + 1;
        public int ToIndex => Math.Min((CurrentPage * perPage), TotalCount);

    }
}
