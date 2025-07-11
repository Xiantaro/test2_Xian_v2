using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;

namespace test2.Models.ManagementModels.ZhongXian.BookQuery
{
    public class BookQueryResult
    {
        private readonly Test2Context _context;
        public BookQueryResult(Test2Context contest)
        {
            _context = contest;
        }

        public async Task<BookQueryViewModel> BookQueryResultMethod(BookQueryModel borrowForm)
        {
            Debug.WriteLine("進入BookQueryResult類別處理.......");
            //var resutl = new List<BookQueryDTO>();
            bool BookEmptyFIlter()
            {
                return borrowForm.book_ISBN == null &&
                    borrowForm.book_KeyWord == null &&
                    borrowForm.book_initDate == null &&
                    borrowForm.book_lastDate == null;
            }

            var  resutl = await _context.Set<BookQueryDTO>().FromSqlInterpolated($"SELECT * FROM BookQueryResultView()").Take(5).ToListAsync();
            var finalResult = new BookQueryViewModel()
            {
                BookQueryModels = resutl
            };
            return resutl;
        }
    }
}
