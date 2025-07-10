using System;

namespace test2.Models.ManagementModels.ZhongXian.RegisterBook
{
    public class BookCodeListClass
    {
        List<string> BookCodeList = new();
        List<Book> BookList = new();
        string? _TypId { get; set; }
        string? _CollecionId { get; set; }

        public List<Book> BookCodeAddToList(int TypId, int CollecionId, int Quantity)
        {
            _TypId = TypId.ToString().PadLeft(3, '0');
            _CollecionId = CollecionId.ToString().PadLeft(4, '0');

            for (int x = 1; x <= Quantity; x++)
            {
                string numX = x.ToString().PadLeft(4, '0');
                string xBookcode = $"TYP{_TypId}-COL{_CollecionId}-CP{numX}";
                BookCodeList.Add(xBookcode);
            }
            foreach (string code in BookCodeList)
            {
                BookList.Add(new Book()
                {
                    CollectionId = CollecionId,
                    BookCode = code,
                    BookStatusId = 1,
                    AccessionDate = DateTime.Now
                });
            }
            return BookList;
        }
    }
}
