using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;
using test2.Models.ManagementModels.ZhongXian.Normal;

namespace test2.Models.ManagementModels.ZhongXian.BookQuery
{
    public class BookQueryResult
    {
        private readonly Test2Context _context;
        public BookQueryResult(Test2Context context)
        {
            _context = context;
        }

        public async Task<BookQueryViewModel> BookQueryResultMethod(BookQueryFormModel BookForm)
        {
            Debug.WriteLine("進入BookQueryResult類別處理.......");
            var QueryResult = _context.Set<BookQueryDTO>().FromSqlInterpolated($"SELECT * FROM BookQueryResultView()").AsQueryable();

            bool BookEmptyFIlter()
            {
                return BookForm.book_ISBN == null &&
                    BookForm.book_KeyWord == null &&
                    BookForm.book_initDate == null &&
                    BookForm.book_lastDate == null;
            }
            Debug.WriteLine("isbm :" + BookForm.book_ISBN);
            Debug.WriteLine("關鍵字:" +BookForm.book_KeyWord);
            Debug.WriteLine("inidate:" +BookForm.book_initDate);
            Debug.WriteLine("lastdate:" +BookForm.book_lastDate);
            Debug.WriteLine("borrow_perpage:" +BookForm.borrow_perPage);
            Debug.WriteLine("page:" +BookForm.page);
            Debug.WriteLine("orderdate:" +BookForm.borrow_OrderDate);
            Debug.WriteLine("orderby:" + BookForm.borrow_orderBy);
            if (BookForm.book_ISBN != null) { QueryResult = QueryResult.Where(x => x.isbn  == BookForm.book_ISBN); }
            if (BookForm.book_KeyWord != null) { QueryResult = QueryResult.Where(x => x.title.Contains(BookForm.book_KeyWord) || x.author.Contains(BookForm.book_KeyWord)); }
            // 初始有 最終空
            if (BookForm.book_initDate != null && BookForm.book_lastDate == null) { QueryResult = QueryResult.Where(x => x.publishDate >= BookForm.book_initDate && x.publishDate <= DateTime.Now); }
            // 初始空 最終有
            else if (BookForm.book_initDate == null && BookForm.book_lastDate != null) { QueryResult = QueryResult.Where(x => x.publishDate <= BookForm.book_lastDate && x.publishDate <= DateTime.Now.AddYears(-1)); }
            // 初始小 最終大
            else if (BookForm.book_initDate <= BookForm.book_lastDate) { QueryResult = QueryResult.Where(x => x.publishDate <= BookForm.book_lastDate && x.publishDate >= BookForm.book_initDate); }
            // 初始大 最終小
            else if (BookForm.book_initDate >= BookForm.book_lastDate) { QueryResult = QueryResult.Where(x => x.publishDate >= BookForm.book_lastDate && x.publishDate <= BookForm.book_initDate); }
            if (BookForm.borrow_orderBy == "desc") { QueryResult = QueryResult.OrderByDescending(x => x.publishDate); }
            else { QueryResult = QueryResult.OrderBy(x => x.publishDate); }
            
            if (BookEmptyFIlter()) { QueryResult = QueryResult.Where(x => DateTime.Now.AddYears(-10) <= x.publishDate && DateTime.Now >= x.publishDate); }

            var dataCount = await QueryResult.CountAsync();

            var finalResult = await QueryResult.Skip((BookForm.page -1) * BookForm.borrow_perPage).Take( BookForm.borrow_perPage).ToListAsync();

            var finalViewModel = new BookQueryViewModel()
            {
                BookQueryDTOs = finalResult,
                PageCounts = new List<PageCount>() { new PageCount { TotalCount = dataCount, CurrentPage = BookForm.page, perPage = BookForm.borrow_perPage} }
            };
            return finalViewModel;
        }
    }
}
