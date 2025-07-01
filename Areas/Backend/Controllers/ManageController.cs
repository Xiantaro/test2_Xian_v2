using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using test2.Models;
using test2.Models.ManagementModels.ZhongXian.Borrow;
using test2.Models.ManagementModels.ZhongXian.BorrowQuery;

namespace test2.Areas.Backend.Controllers
{
    [Area("Backend")]
    public class ManageController : Controller
    {
        private readonly Test2Context _context;
        public ManageController(Test2Context context)
        {
            _context = context;
        }
        #region view
        public IActionResult Index() { return View(); }
        #endregion

        //----葉忠憲處理部分------------------------------------------------------------------------------------------
        #region 預約管理&查詢
        // 預約管理_搜尋排列_partial
        public IActionResult AppointmentQuery()
        {
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_appointmentQueryPartial.cshtml");
        }
        //預約管理_查詢列表_partial
        public IActionResult AppointmentResult(string appointment_reservationNum = "All", string appointment_UserID = "我是ID", string appointment_bookNum = "持續買進", DateTime? appointment_initDate = null, DateTime? appointment_lastDate = null, string? appointment_state = "All", string appointment_perPage = "10", string appointment_orderDate = "desc", int page = 1)
        {
            Debug.WriteLine("測試載入:  預約ID:" + appointment_reservationNum + " 使用者ID:" + appointment_UserID + " 書本名稱:" + appointment_bookNum + " 開始日期:" + appointment_initDate + " 今天日期:" + appointment_lastDate + " 狀態:" + appointment_state + " 頁數:" + appointment_perPage + " 日期排序:" + appointment_orderDate + "頁數" + page);
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_appointmentResultPartial.cshtml");
        }
        #endregion

        #region 借閱查詢
        // 借閱查詢_搜尋排列_partial
        public IActionResult BorrowQuery()
        {
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_borrowQueryPartial.cshtml");
        }
        // 借閱查詢_查詢列表_partial
        public IActionResult BorrowResult(string borrow_BorrowID = "All", string borrow_UserID = "All", string borrow_bookNum = "All", string borrow_state = "All", DateTime? borrow_initDate = null, DateTime? borrow_lastDate = null, string borrow_perPage = "10", string borrow_date = "borrowDate", string borrow_order = "desc", int page = 1)
        {


            //Debug.WriteLine("已進入查詢!!!!!!!!!!!!!!!!!");
            //bool BorrowIsEmptyFilter()
            //{
            //    return borrow_BorrowID == null &&
            //        borrow_bookNum == null &&
            //        borrow_UserID == null &&
            //        borrow_initDate == null &&
            //        borrow_lastDate == null &&
            //        borrow_state == "ALL";
            //}
            //Debug.WriteLine($"測試借閱載入 {borrow_BorrowID}+{borrow_UserID} + {borrow_bookNum} + {borrow_state}+日期 + {borrow_initDate}到 {borrow_lastDate}； {borrow_perPage} + {borrow_OrderDate} + {borrow_orderBy} + 頁數: {page}");
            //Debug.WriteLine("開始進行查詢");
            //var result = _context.Set<Borrow>().Include(x => x.Collection).ThenInclude(x => x.Book).Include(user => user.CIdNavigation).Include(statu => statu.BorrowStatus).Select(result => new BorrowQueryDTO
            //{
            //    borrowId = result.BorrowId,
            //    collectionId = result.CollectionId,
            //    title = result.Collection.Book.Title,
            //    cId = result.CId,
            //    cName = result.CIdNavigation.CName,
            //    borrowDate = result.BorrowDate,
            //    dueDate = result.DueDate,
            //    returnDate = result.ReturnDate,
            //    borrowStatus = result.BorrowStatus.BorrowStatus1
            //}).AsQueryable();

            //// 各種條件的篩選
            //if (borrow_BorrowID != null) { result = result.Where(id => id.borrowId == borrow_BorrowID); }
            //if (borrow_UserID != null) { result = result.Where(id => id.cId == borrow_UserID); }
            //if (borrow_bookNum != null) { result = result.Where(id => id.collectionId == borrow_bookNum); }

            //if (borrow_initDate != null && borrow_lastDate == null) { result = result.Where(id => id.borrowDate > borrow_initDate); }
            //if (borrow_initDate == null && borrow_lastDate != null) { result = result.Where(id => id.borrowDate < borrow_lastDate); }
            //if (borrow_initDate != null && borrow_lastDate != null)
            //{
            //    DateTime? StartTime = borrow_initDate;
            //    if (borrow_initDate > borrow_lastDate) { StartTime = borrow_lastDate; result = result.Where(id => id.borrowDate < borrow_initDate && id.borrowDate > borrow_lastDate); }
            //    else { result = result.Where(id => id.borrowDate > borrow_initDate && id.borrowDate < borrow_lastDate); }
            //}
            //if (borrow_state != "ALL") result = result.Where(id => id.borrowStatus == borrow_state);
            //if (BorrowIsEmptyFilter()) { result = result.Where(re => re.borrowDate <= DateTime.Now && re.borrowDate >= DateTime.Now.AddMonths(-2)); }
            //// 各種條件的篩選 END

            //result = (borrow_OrderDate, borrow_orderBy) switch
            //{
            //    ("borrowDate", "desc") => result.OrderByDescending(x => x.borrowDate),
            //    ("borrowDate", "asc") => result.OrderBy(x => x.borrowDate),
            //    ("dueDate", "desc") => result.OrderByDescending(x => x.borrowDate),
            //    ("dueDate", "asc") => result.OrderBy(x => x.borrowDate),
            //    ("returnDate", "desc") => result.OrderByDescending(x => x.borrowDate),
            //    ("returnDate", "asc") => result.OrderBy(x => x.borrowDate)
            //};
            //var totalCount = await result.CountAsync();
            //if (totalCount == 0) return Json(0);


            //var BorrowResultPage = await result.Skip((page - 1) * borrow_perPage).Take(borrow_perPage).ToListAsync();

            //var BorrowQueryViewModels2 = new BorrowQueryViewModel()
            //{
            //    BorrowQueryDTOs = BorrowResultPage,
            //    TotalCount = totalCount,
            //    TotalPage = (int)Math.Ceiling((double)totalCount / borrow_perPage),
            //    CurrentPage = page,
            //    FromIndex = (page - 1) * borrow_perPage + 1,
            //    ToIndex = Math.Min(page * borrow_perPage, totalCount)
            //};



            Debug.WriteLine($"測試借閱載入 {borrow_BorrowID}+{borrow_UserID} + {borrow_bookNum} + {borrow_state}+日期 + {borrow_initDate}到 {borrow_lastDate}； {borrow_perPage} + {borrow_date} + {borrow_order} + 頁數: {page}");
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_borrowResultPartial.cshtml");
        }
        #endregion

        #region 借閱模式
        // 借書模式_partial
        public IActionResult BorrowMode()
        {
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_borrowModePartial.cshtml");
        }
        // 借書模式_借書
        public async Task<IActionResult> BorrowSend(int borrwoMode_UserID, string borrwoMode_BookCode)
        {
            Debug.WriteLine("借書開始" + borrwoMode_BookCode + "+++");
            var UserId = await _context.Clients.Where(x => x.CId == borrwoMode_UserID).Select(y => new { y.CId, y.CName }).FirstOrDefaultAsync();
            if (UserId == null) { return Json(0); };
            var BookInfo = await _context.Books.Join(_context.Collections, bok => bok.CollectionId, col => col.CollectionId, (bok, col) => new { bok, col }).Where(x => x.bok.BookCode == borrwoMode_BookCode).Select(result => new { result.col.Title}).FirstOrDefaultAsync();

            var SqlResult = await _context.Set<BorrwoMessageDTO>().FromSqlInterpolated($"EXEC BorrowResult {borrwoMode_UserID}, {borrwoMode_BookCode}").ToListAsync();
            var result = new BorrowResultViewModel()
            {
                ResultCode = SqlResult[0].ResultCode,
                Message = SqlResult[0].Message,
                Cid = UserId.CId,
                cName = UserId.CName ?? "查無此借閱者!!",
                title = BookInfo?.Title ?? "查無此書本!!",
                bookcode = borrwoMode_BookCode
            };
            return PartialView("~/Areas/Backend/Views/Manage/BorrowModeContent.cshtml", result);
        }
        // 借書模式_借書人資訊
        public async Task<IActionResult> BorrowUserMessage(int userId)
        {
            Debug.WriteLine("借書人資訊訊借書人資訊訊借書人資訊訊借書人資訊訊");
            var UserInfoamtion = await _context.Clients.Where(x => x.CId == userId).Select(result => new BorrowUser
            {
                cId = result.CId,
                cName = result.CName
            }).ToListAsync();
            Debug.WriteLine("++++++++++++++++++++++++++++++++++++");
            if (UserInfoamtion.Count != 1) { return Json(false); }
            
            Debug.WriteLine("哈哈哈"+UserInfoamtion);
            return PartialView("~/Areas/Backend/Views/Manage/BorrowModeUser.cshtml", UserInfoamtion);
        }
        // 借書模式_書本資訊
        public async Task<IActionResult> BorrowBookMessage(string bookId)
        {
            var BookInformation = await _context.Set<BorrowBookInfomationDTO>().FromSqlInterpolated($"EXEC BookInfomationForBorrow {bookId}").ToListAsync();
            if (BookInformation.Count != 1) { return Json(false); }
            Debug.WriteLine(bookId);
            return PartialView("~/Areas/Backend/Views/Manage/BorrowModeBook.cshtml", BookInformation);
        }
        #endregion 借閱模式END

        #region 還書模式
        public IActionResult ReturnBookMode()
        {
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_returnBookPartial.cshtml");
        }
        public IActionResult ReturnBookSend(string ReturnBookID)
        {
            Debug.WriteLine($"借閱者{ReturnBookID}還書成功");
            return PartialView("~/Areas/Backend/Views/Manage/ReturnBookContent.cshtml");
        }
        #endregion 還書模式 END

        #region 預約模式
        public IActionResult AppointmentMode1()
        {
            Debug.WriteLine("預約模式載入成功...............");
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_appoimtmentPartial.cshtml");
        }
        public IActionResult AppointmentMode1Send(string appointmentMode_UserID, string appointmentMode_BookNumber)
        {
            Debug.WriteLine("書本預約狀況 載入成功...." + "使用者ID: " + appointmentMode_UserID + "書籍ID: " + appointmentMode_BookNumber);

            return PartialView("~/Areas/Backend/Views/Manage/AppoimtmentContent.cshtml", appointmentMode_UserID);
        }
        public IActionResult AppointmentMode1Query(string keyWord, string state, string pageCount)
        {
            Debug.WriteLine($"預約書本查詢 載入成功....{keyWord}、{state}、{pageCount}");
            return PartialView("~/Areas/Backend/Views/Manage/AppoimtmentModeQuery.cshtml");
        }
        #endregion

        #region 通用Action
        // 回傳通知
        public IActionResult Notification(string NotificationUserInput, string NotificationType, string NotificationTextarea)
        {
            Debug.WriteLine($"預約者編號: {NotificationUserInput}、通知類型 {NotificationType}、內容 : {NotificationTextarea}");
            return Ok();
        }
        #endregion
        //------------------------------------------------------------------------------------------

        public IActionResult TestDbContext()
        {
            Debug.WriteLine("Db連線測試開始");
            try
            {
                var canContext = _context.Database.CanConnect();
                Debug.WriteLine($"是否可以連線到資料庫:  {canContext}");
                return Json(canContext);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"連線失敗: {ex}");
                return Json(0);
            }
        }
    }
}