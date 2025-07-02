using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using test2.Models;

namespace test2.Models.ManagementModels.ZhongXian.BorrowQuery
{
    public class BorrowQueryFinalSearch
    {
        private readonly Test2Context _context;

        public BorrowQueryFinalSearch(Test2Context context)
        {
            _context = context;
        }

        public async Task<BorrowQueryViewModel> BorrowQuerySeach(BorrowQueryFilter filter)
        {
            bool BorrowIsEmptyFilter()
            {
                return filter.borrow_BorrowID == null &&
                    filter.borrow_bookCode == null &&
                    filter.borrow_UserID == null &&
                    filter.borrow_initDate == null &&
                    filter.borrow_lastDate == null &&
                    filter.borrow_state == "All";
            }

            var result = _context.Borrows.Include(x => x.Book).ThenInclude(x => x.Collection).Include(user => user.CIdNavigation).Include(statu => statu.BorrowStatus).Select(result => new BorrowQueryResultDTO
            {
                borrowId = result.BorrowId,
                bookCode = result.Book.BookCode,
                title = result.Book.Collection.Title,
                cId = result.CId,
                borrowDate = result.BorrowDate,
                dueDateB = result.DueDateB,
                returnDate = result.ReturnDate,
                borrowStatus = result.BorrowStatus.BorrowStatus1
            }).AsQueryable();

            // 各種條件的篩選
            if (filter.borrow_BorrowID != 0) { result = result.Where(id => id.borrowId == filter.borrow_BorrowID); }
            if (filter.borrow_UserID != 0) { result = result.Where(id => id.cId == filter.borrow_UserID); }
            if (filter.borrow_bookCode != null) { result = result.Where(id => id.bookCode == filter.borrow_bookCode); }

            if (filter.borrow_initDate != null && filter.borrow_lastDate == null) { result = result.Where(id => id.borrowDate > filter.borrow_initDate); }
            if (filter.borrow_initDate == null && filter.borrow_lastDate != null) { result = result.Where(id => id.borrowDate < filter.borrow_lastDate); }
            if (filter.borrow_initDate != null && filter.borrow_lastDate != null)
            {
                DateTime? StartTime = filter.borrow_initDate;
                if (filter.borrow_initDate > filter.borrow_lastDate) { StartTime = filter.borrow_lastDate; result = result.Where(id => id.borrowDate < filter.borrow_initDate && id.borrowDate > filter.borrow_lastDate); }
                else { result = result.Where(id => id.borrowDate > filter.borrow_initDate && id.borrowDate < filter.borrow_lastDate); }
            }
            if (filter.borrow_state != "ALL") result = result.Where(id => id.borrowStatus == filter.borrow_state);
            // 預設搜尋搜尋
            if (BorrowIsEmptyFilter()) { result = result.Where(re => re.borrowDate <= DateTime.Now && re.borrowDate >= DateTime.Now.AddMonths(-2)); }
            // 各種條件的篩選 END

            result = (filter.borrow_OrderDate, filter.borrow_orderBy) switch
            {
                ("borrowDate", "desc") => result.OrderByDescending(x => x.borrowDate),
                ("borrowDate", "asc") => result.OrderBy(x => x.borrowDate),
                ("dueDate", "desc") => result.OrderByDescending(x => x.borrowDate),
                ("dueDate", "asc") => result.OrderBy(x => x.borrowDate),
                ("returnDate", "desc") => result.OrderByDescending(x => x.borrowDate),
                ("returnDate", "asc") => result.OrderBy(x => x.borrowDate)
            };


            var totalCount = await result.CountAsync();
            //if (totalCount == 0) return Json(0);
            var Nextresult = await result.Skip((filter.page - 1) * filter.borrow_perPage).Take(filter.borrow_perPage).ToListAsync();

            var BorrowQueryViewModels = new BorrowQueryViewModel()
            {
                BorrowQueryDTOs = Nextresult,
                TotalCount = totalCount,
                CurrentPage = filter.page,
                PageSize = filter.borrow_perPage,
            };
            return BorrowQueryViewModels;
         }
    }
}

