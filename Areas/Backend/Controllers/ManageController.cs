using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace test2.Areas.Backend.Controllers
{
    [Area("Backend")]
    public class ManageController : Controller
    {
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
        public IActionResult BorrowSend(string borrwoMode_UserID, string borrwoMode_BookNumber)
        {
            #region 測試回傳可刪
            //var mystatu = new BorrowModeSendClass();
            //if (borrwoMode_UserID != "1234")
            //{
            //    mystatu.IsSuccess = false;
            //    mystatu.MistakeMessag = "借閱者不存在";
            //    return PartialView("_BorrowModeContent", mystatu);
            //}
            //if (borrwoMode_BookNumber != "1234")
            //{
            //    mystatu.IsSuccess = false;
            //    mystatu.MistakeMessag = "書本不存在";
            //    return PartialView("_BorrowModeContent", mystatu);
            //}
            //Debug.WriteLine($"成功借書 ID:{borrwoMode_UserID} BookID: {borrwoMode_BookNumber}");
            //mystatu.UserId = borrwoMode_UserID;
            //mystatu.BookName = borrwoMode_BookNumber;
            #endregion 
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_borrowModeContent.cshtml");
        }
        // 預約模式_預約
        public IActionResult AppointmentSend(string borrwoMode_UserID, string borrwoMode_BookNumber)
        {
            Debug.WriteLine($"使用者: {borrwoMode_UserID} ；書籍ID {borrwoMode_BookNumber}");
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_borrowModeContent.cshtml");
        }
        // 借書模式_借書人資訊
        public IActionResult BorrowUserMessage(string userId)
        {
            // 之後要建立 ViewModel 用來裝搜尋到的 借書人資訊
            // 並回傳到 PartialView 上
            Debug.WriteLine(userId);
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_borrowModeUser.cshtml");
        }
        // 借書模式_書本資訊
        public IActionResult BorrowBookMessage(string bookId)
        {
            // 之後要建立 ViewModel 用來裝搜尋到的 書本資訊
            // 並回傳到 PartialView 上
            Debug.WriteLine(bookId);
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_borrowModeBook.cshtml");
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
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_returnBookContent.cshtml");
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

            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_appoimtmentContent.cshtml", appointmentMode_UserID);
        }
        public IActionResult AppointmentMode1Query(string keyWord, string state, string pageCount)
        {
            Debug.WriteLine($"預約書本查詢 載入成功....{keyWord}、{state}、{pageCount}");
            return PartialView("~/Areas/Backend/Views/Shared/_Partial/_appoimtmentModeQuery.cshtml");
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
    }
}