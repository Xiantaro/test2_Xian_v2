using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Execution;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Server;
using NuGet.Protocol.Plugins;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using test2.Models;
using test2.Models.ManagementModels.ZhongXian.Appoimtment;
using test2.Models.ManagementModels.ZhongXian.AppoimtmentQuery;
using test2.Models.ManagementModels.ZhongXian.BookQuery;
using test2.Models.ManagementModels.ZhongXian.Borrow;
using test2.Models.ManagementModels.ZhongXian.BorrowQuery;
using test2.Models.ManagementModels.ZhongXian.Normal;
using test2.Models.ManagementModels.ZhongXian.RegisterBook;

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
        public async Task<IActionResult> AppointmentResult(int appointment_reservationNum, int appointment_UserID, string appointment_bookNum, DateTime? appointment_initDate = null, DateTime? appointment_lastDate = null, string appointment_state = "ALL", int appointment_perPage = 10, string appointment_orderDate = "desc", int page = 1)
        {
            Debug.WriteLine("++++++++++++++預約管理_查詢列表+++++++++++++++");
            Debug.WriteLine("測試載入:  預約ID:" + appointment_reservationNum + " 使用者ID:" + appointment_UserID + " 書本名稱:" + appointment_bookNum + " 開始日期:" + appointment_initDate + " 今天日期:" + appointment_lastDate + " 狀態:" + appointment_state + " 頁數:" + appointment_perPage + " 日期排序:" + appointment_orderDate + "頁數" + page);
            
            AppoimtmentQueryFilter filter = new AppoimtmentQueryFilter()
            {
                appointment_reservationId = appointment_reservationNum,
                appointment_UserID = appointment_UserID,
                appointment_bookCode = appointment_bookNum,
                appointment_initDate = appointment_initDate,
                appointment_lastDate = appointment_lastDate,
                appointment_state = appointment_state,
                appointment_perPage = appointment_perPage,
                appointment_orderDate = appointment_orderDate,
                page = page
            };

            var newclass = new AppointmentQueryFinalSearch(_context);
            var final = await newclass.AppointmentQuerySearch(filter);
            
            Debug.WriteLine("即將送出........預約搜尋結果!!");
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_appointmentResultPartial.cshtml", final);
        }
        #endregion

        #region 借閱查詢
        // 借閱查詢_搜尋排列_partial
        public IActionResult BorrowQuery()
        {
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_borrowQueryPartial.cshtml");
        }
        // 借閱查詢_查詢列表_partial
        public async Task<IActionResult> BorrowResult(int borrow_BorrowID, int borrow_UserID, string borrow_bookCode, string borrow_state = "All", DateTime? borrow_initDate = null, DateTime? borrow_lastDate = null, int borrow_perPage = 10, string borrow_OrderDate = "borrowDate", string borrow_orderBy = "desc", int page = 1)
        {
            var borrowqueryFiltervar = new BorrowQueryFilter()
            {
                borrow_BorrowID = borrow_BorrowID,
                borrow_bookCode = borrow_bookCode,
                borrow_UserID = borrow_UserID,
                borrow_state = borrow_state,
                borrow_initDate = borrow_initDate,
                borrow_lastDate = borrow_lastDate,
                borrow_perPage = borrow_perPage,
                borrow_OrderDate = borrow_OrderDate,
                borrow_orderBy = borrow_orderBy,
                page = page
            };
            var service = new BorrowQueryFinalSearch(_context);
            var BorrowQueryViewModels2 = await service.BorrowQuerySeach(borrowqueryFiltervar);

            
            Debug.WriteLine($"測試借閱載入 {borrow_BorrowID}+{borrow_UserID} + {borrow_bookCode} + {borrow_state}+日期 + {borrow_initDate}到 {borrow_lastDate}； {borrow_perPage} + {borrow_OrderDate} + {borrow_orderBy} + 頁數: {page}");
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_borrowResultPartial.cshtml", BorrowQueryViewModels2);
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

            var SqlResult = await _context.Set<MessageDTO>().FromSqlInterpolated($"EXEC BorrowResult {borrwoMode_UserID}, {borrwoMode_BookCode}").ToListAsync();
            var result = new ResultViewModel()
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
        public async Task<IActionResult> ReturnBookSend(string ReturnBookCode)
        {
            Debug.WriteLine($"還書編號: {ReturnBookCode}++++++++++++++++++++++++++++");

            var retrunBookIsReal = await _context.Borrows.Include(x => x.Book).ThenInclude(x => x.Collection).Include(x => x.CIdNavigation).Where(x => x.Book.BookCode == ReturnBookCode)
                .Select(re => new { re.CId,re.CIdNavigation.CName, re.Book.Collection.Title }).ToListAsync();
            if (retrunBookIsReal.IsNullOrEmpty()) { return Json(0); }

            Debug.WriteLine($"開始還書作業......");
            var resultMessage = await _context.Set<MessageDTO>().FromSqlInterpolated($"EXEC ReturnBook {ReturnBookCode}").ToListAsync();
            Debug.WriteLine($"錯誤代碼{resultMessage[0].ResultCode}: {resultMessage[0].Message}");
            var resultViewModel = new ResultViewModel()
            {
                ResultCode = resultMessage[0].ResultCode,
                Message = resultMessage[0].Message,
                Cid = retrunBookIsReal[0].CId,
                cName = retrunBookIsReal[0].CName,
                title = retrunBookIsReal[0].Title,
                bookcode = ReturnBookCode,
            };
            Debug.WriteLine($"借閱者:{resultViewModel.Cid}、{ReturnBookCode}還書成功");
            return PartialView("~/Areas/Backend/Views/Manage/ReturnBookContent.cshtml", resultViewModel);
        }
        #endregion 還書模式 END

        #region 預約模式
        public IActionResult AppointmentMode1()
        {
            Debug.WriteLine("預約模式載入成功...............");
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_appoimtmentPartial.cshtml");
        }
        public async Task<IActionResult> AppointmentMode1Send(int appointmentMode_UserID, int appointmentMode_BookId)
        {
            
            Debug.WriteLine("書本預約狀況 載入成功...." + "使用者ID: " + appointmentMode_UserID + "書籍ID: " + appointmentMode_BookId);
            var User = await _context.Clients.Where(x => x.CId == appointmentMode_UserID).ToListAsync();
            Debug.WriteLine($"使用者是否成功: {User.Count}");
            if(User.Count != 1) { return Json(0); }
            var bookId = await _context.Collections.Where(x => x.CollectionId == appointmentMode_BookId).Select(re => new { re.Title }).ToListAsync();
            Debug.WriteLine($"書本數量是否存在: {bookId.Count}");
            if (bookId.Count != 1) { return Json(1); }

            var ResultMessage = await _context.Set<MessageDTO>().FromSqlInterpolated($"EXEC RevervationMode {appointmentMode_UserID}, {appointmentMode_BookId}").ToListAsync();

            var result = new ResultViewModel()
            {
                ResultCode = ResultMessage[0].ResultCode,
                Message = ResultMessage[0].Message,
                Cid = appointmentMode_UserID,
                cName = User[0].CName,
                title = bookId[0].Title,
            };

            Debug.WriteLine("預約模式最後..........要發送了!");
            string ViewUrl = "~/Areas/Backend/Views/Manage/AppoimtmentContent.cshtml";
            return PartialView(ViewUrl, result);
        }
        public async Task<IActionResult> AppointmentMode1Query(string keyWord, string state, int pageCount, int  page = 1)
        {
            Debug.WriteLine($"{DateTime.Now}:預約書本查詢 載入成功....{keyWord}、{state}、{pageCount}、{page}");

            var result =  await _context.Set<AppoimtmentKeywordShow>().FromSqlInterpolated($"EXEC BookStatusDetail {keyWord}, {state}").ToListAsync();
            var totalcount = result.Count();
            if(totalcount == 0) { return Json(0); }
            var final =  result.Skip((page - 1) * pageCount).Take(pageCount).ToList();

            var FinalRestul = new AppoimtmentResult()
            {
                AppoimtmentKeywordShows = final,
                TotalCount = result.Count(),
                CurrentPage = page,
                perPage = pageCount,
                status = state
            };

            return PartialView("~/Areas/Backend/Views/Manage/AppoimtmentModeQuery.cshtml", FinalRestul);
        }

        // 關鍵字動態搜尋
        public async Task<IActionResult> KeyWordAuthorSearch(string keyword)
        {

            var keyWord = await _context.Collections.Where(x => x.Title.Contains(keyword)).Take(6).ToListAsync();
            return Json(keyWord);
        }
        #endregion

        #region 書籍登陸

        public async Task<IActionResult> BooksAdds()
        {
            var bookLanguages = await _context.Languages.ToListAsync();
            var bookTypes = await _context.Types.ToListAsync();
            LanguageAndTypeViewModel LanguageAndTypes = new LanguageAndTypeViewModel()
            {
                Language = bookLanguages,
                Type = bookTypes,
            };

            Debug.WriteLine("成功進入書籍登陸");
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_RegisterPartial.cshtml", LanguageAndTypes);
        }

        BookCodeListClass myBookCode = new BookCodeListClass();
        
        public async Task<IActionResult> BooksCreate(BookAddsClass formdata, IFormFile BookAdd_InputImg)
        {
            bool IsbnUNIQUE =  _context.Collections.Any(x => x.Isbn == formdata.BooksAdded_ISBM);
            if(IsbnUNIQUE == true) { return Json(new { ResltCode = 0, Message = "重複ISBM，請重新輸入" }); }
            if (BookAdd_InputImg == null && BookAdd_InputImg?.Length == 0) { return Json(new { ResltCode = 0, Message = "請放入封面!" }); }

            NewAuthor myNewAuthor = new NewAuthor(_context);
            var authorid = await myNewAuthor.CreateAuthor(formdata.BooksAdded_authorId, formdata.BooksAdded_authorName!);
            using var ms = new MemoryStream();
            BookAdd_InputImg!.CopyTo(ms);
            byte[] imageBytes = ms.ToArray();
            var newCollection = new Collection()
            {
                Title = formdata.BooksAdded_Title!,
                CollectionDesc = formdata.BooksAdded_Dec,
                TypeId = formdata.BooksAdded_Type,
                AuthorId = authorid,
                Translator = formdata.BooksAdded_translator,
                Publisher = formdata.BooksAdded_pushier!,
                LanguageId = formdata.BooksAdded_leng,
                Isbn = formdata.BooksAdded_ISBM!,
                PublishDate = formdata.BooksAdded_puDate,
                CollectionImg = imageBytes,
            };
            _context.Add(newCollection);
            await _context.SaveChangesAsync();

            var collectionId = newCollection.CollectionId;

            List<Book> bookList = myBookCode.BookCodeAddToList(formdata.BooksAdded_Type, collectionId, formdata.BooksAdded_inCount);
            _context.AddRange(bookList);
            await _context.SaveChangesAsync();
          
            Debug.WriteLine("回傳結果");
            return Json(new { ResltCode = 1, Message = "成功新增書籍!" });
        }
        // 作者AutoComplete
        public async Task<IActionResult> AuthorSearch(string authorLike)
        {
            Debug.WriteLine("作者書入關鍵字進入....");
            var author = await _context.Authors.Where(x => x.Author1.Contains(authorLike)).ToListAsync();
            return Json(author);
        }
        #endregion

        #region 書籍管理
        public IActionResult BooksQuerys()
        {
            Debug.WriteLine("成功進入書籍查詢");
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_SearchPartial.cshtml");
        }

        // 書籍管理查詢結果
        public async Task<IActionResult> BooksQueryResult(BookQueryModel borrowForm)
        {
            Debug.WriteLine("書籍管理進入....");
            Debug.WriteLine($"輸入結果: {borrowForm.book_ISBN}");
            Debug.WriteLine($"輸入結果: {borrowForm.book_KeyWord}");
            Debug.WriteLine($"輸入結果: {borrowForm.book_initDate}");
            Debug.WriteLine($"輸入結果: {borrowForm.book_lastDate}");
            Debug.WriteLine($"輸入結果: {borrowForm.borrow_perPage}");
            Debug.WriteLine($"輸入結果: {borrowForm.borrow_OrderDate}");
            Debug.WriteLine($"輸入結果: {borrowForm.borrow_orderBy}");

            // wait
            var BookQueryClass = new BookQueryResult(_context);
            var QueryResult = BookQueryClass.BookQueryResultMethod(borrowForm);

            Debug.WriteLine("準備送回去!!!");
            //var finalresult = await _context.Set<BookQueryDTO>().FromSqlInterpolated($"SELECT * FROM BookQueryResultView()").Take(5).ToListAsync();

            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_SearchResultPartial.cshtml", QueryResult);
        }
        #endregion

        #region 通用Action
        // 傳送通知
        public async Task<IActionResult> Notification(int NotificationId, string NotificationType, string NotificationTextarea)
        {
            Debug.WriteLine($"預約者編號: {NotificationId}、通知類型 {NotificationType}、內容 : {NotificationTextarea}");
            var user = await _context.Clients.FirstOrDefaultAsync(x => x.CId == NotificationId);
            if (user == null) { return Json("該預約者不存在"); }

            var NotificationSend = new Notification()
            {
                CId = NotificationId,
                Message = NotificationTextarea,
                NotificationDate = DateTime.UtcNow
            };
            await _context.AddAsync(NotificationSend);
            await _context.SaveChangesAsync();
            return Json(1);
        }
        // 取消預約
        public async Task<IActionResult> CancelAppointment(int NotificationAppointmentId, int NotificationUser, string NotificationTextarea)
        {
            Debug.WriteLine($"借閱編號:{NotificationAppointmentId}");
            Debug.WriteLine($"預約者id:{NotificationUser}");
            Debug.WriteLine($"文本內容:{NotificationTextarea}");
            var Noticaionl = new Notification
            {
                CId = NotificationUser,
                Message = NotificationTextarea,
                NotificationDate = DateTime.UtcNow
            };
            var text = await _context.Reservations.Where(x => x.ReservationId == NotificationUser && x.CId == NotificationUser).ToListAsync();
            if (text.IsNullOrEmpty()) { Json(0); }

            var cancelAppointment = await _context.Reservations.FirstOrDefaultAsync(x => x.ReservationId == NotificationAppointmentId && x.CId == NotificationUser);

            if (cancelAppointment == null) { Json(0); }
            cancelAppointment!.ReservationStatusId = 4;
            await _context.Notifications.AddAsync(Noticaionl);
            await _context.SaveChangesAsync();
            return Json(1);
        }
        #endregion
        //------------------------------------------------------------------------------------------
        #region DB連線測試
        public IActionResult TestDbContext()
        {
            Debug.WriteLine("Db連線測試開始.........");
            try
            {
                var canContext = _context.Database.CanConnect();
                Debug.WriteLine($"是否可以連線到資料庫:  {canContext}");
                return Json(canContext);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"連線失敗代碼: {ex}");
                return Json(0);
            }
        }
        #endregion
    }
}