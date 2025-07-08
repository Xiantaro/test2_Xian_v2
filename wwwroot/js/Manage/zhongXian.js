// #region 載入parital => 預約查詢、借閱查詢、借書模式、還書模式、預約模式
$(() => {
    console.log("已綁定事件");
    $("#AppointmentQuery").on("click", AppointmentQueryModule)
    $("#BorrowQuery").on("click", BorrowQueryModule)
    $("#BorrowMode").on("click", BorrowModeMode)
    $("#ReturnMode").on("click", ReturnBookMode);
    $("#AppointmentMode").on("click", AppointmentMode);
    $("#dbConnect").on("click", DbContextText);
    console.log("綁定結束")
})
// #endregion

// #region 預約查詢&管理 Module
// 載入綁定_預約及借閱partial
function AppointmentQueryModule() {
    initAppointmentPage();
    console.log("已載入預約查詢欄parital");
};
// 預約管理(搜尋欄)_初始頁載入
function initAppointmentPage() {
    $("#content-panel").load("/Backend/Manage/AppointmentQuery", () => {
        appointment_queryEvent();
        console.log("已載入預設搜尋結果");
        // 預約查詢綁定
        $("#appointment_select").on("click", appointment_queryEvent);
        // 排列篩選綁定
        $(document).on("change", "#appointment_perPage, #appointment_orderDate ", appointment_queryEvent)
        $("#appointment_clear").on("click", appointment_clearEvent);
    });
};
// 搜尋、排列、分頁
function appointment_queryEvent() {
    $("#AppointmentContent").html("");
    $("#AppointmentContent").html(QueryWait);
    const value = $(this).data("page") || 1;
    let formData = $("#appointmenSearch").serialize() + `&page=${value}`;
    console.log("預約查詢進入" + formData);
    $.post("/Backend/Manage/AppointmentResult", formData, (result) => {
        $("#AppointmentContent").html(result);
        $(".page-link").on("click", appointment_queryEvent);
        // 取消按鈕設置
        $(".NotificationBtn").on("click", CancelAppointmentBtn);
        $("#NotificationClear").on("click", NotificationClearBtn);
        $("#CancelBox").on("click", appointmentNotificationClose);
        $("#NotificationSend").on("click", SendCancelAppointmentBtn);
    });
    console.log("查詢刷新~");
}
// 取消預約關閉按鈕
function appointmentNotificationClose() {
    console.log("關閉視窗按鈕成功")
    $('#notificationModal').modal("hide");
    $("#NotificationType").val("CancelNotification");
}
let booktitle;
let appointmentDate;
// 重新選擇
function CancelNotificationChange() {
    let cancelAppointmentText =
        `【取消預約通知】\n親愛的用戶您好，\n您於 {${appointmentDate}}\n所預約的書籍《 ${booktitle} 》\n已於 {${new Date().toLocaleString('zh-TW')}} 由本館管理員取消。\n取消原因： OOXX ，若您仍有借閱需求，歡迎重新進行預約。\n如有任何問題或需協助，敬請聯繫本館服務人員，我們將竭誠為您服務。感謝您的配合與理解！圖書館管理系統 敬上。`
    $("#NotificationTextarea").val(cancelAppointmentText);
}

// 取消預約按鈕
function CancelAppointmentBtn() {
    appointmentDate = $(this).closest("tr").find(".appointmentDate").text();
    booktitle = $(this).closest("tr").find(".booktitle").text();
    let appointmenId = $(this).closest("tr").find(".appointmentId").text();
    let cid = $(this).closest("tr").find(".appointmentcid").text();
    CancelNotificationChange();
    $("#NotificationType").on("click", CancelNotificationChange);
    $("#NotificationAppointmentId").val(appointmenId);
    $("#NotificationUser").val(cid);
}
// 送出取消預約
function SendCancelAppointmentBtn() {
    let CancelForm = $("#NotificationFom").serialize();
    console.log(`表單序列話 :${CancelForm}`)
    $.post("/Backend/Manage/CancelAppointment", CancelForm, (result) => {
        console.log("已回傳");
        if (result == 0) { alert("取消預約失敗...."); }
        if (result == 1) { alert("取消預約成功!!!!"); }
    })
    console.log("送出")
    $('#notificationModal').modal("hide");
    setTimeout(() => appointment_queryEvent(), 1000)
    appointment_queryEvent();
}

// #endregion 預約查詢Module "END""

// #region 借閱查詢 Module
function BorrowQueryModule() { initBorrowPage(); }
// 借閱查詢(搜尋欄)_初始載入
function initBorrowPage() {
    $("#content-panel").load("/Backend/Manage/BorrowQuery", () => {
        console.log("成功載入_借閱模式");
        borrow_queryEvent();
        $("#borrow_select").on("click", borrow_queryEvent);
        $(document).on("change", "#borrow_perPage,#borrow_date, #borrow_OrderDate, #borrow_orderBy", borrow_queryEvent);
        $("#borrow_clear").on("click", () => { $("#borrowForm")[0].reset(); });

    })
}
// 搜尋、分頁、排列
function borrow_queryEvent() {
    $("#BorrowContent").html("");
    $("#BorrowContent").html(QueryWait);
    let value = $(this).data("page") || 1;
    let borrowData = $("#borrowForm").serialize() + `&page=${value}`;
    $.post("/Backend/Manage/BorrowResult", borrowData, (result) => {
        $("#BorrowContent").html(result);
        $(".page-link").on("click", borrow_queryEvent);
        // 點擊按鈕
        $(".NotificationBtn").on("click", NotificationBtn);
        $("#NotificationSend").on("click", NotificationMessageSend);
        $("#NotificationClear").on("click", NotificationClearBtn);
        $("#CancelBox").on("click", NotificationClose);
    })
}
// #endregion 借閱查詢Module END

// #region 借閱模式 Module
function BorrowModeMode() {
    console.log("借書模式測試");
    $("#content-panel").load("/Backend/Manage/BorrowMode", () => {
        console.log("借書載入成功");
        $("#borrowSend").on("click", BorrowModeSend);
        $("#borrwoMode_UserID").on("input", BorrowModeModeUserDynamic)
        $("#borrwoMode_BookCode").on("input", BorrowModeModeBookDynamic);
        $("#borrwoMode_CancelUserIDBtn").on("click", CancelBtnUser)
        $("#borrwoMode_CancelBookIdBtn").on("click", CancelBtnBook)
    });
}
// 動態搜尋 借閱者
function BorrowModeModeUserDynamic() {
    $("#BorrowModeSuccessContent").html("");
    console.log("借閱者動態查詢");
    let userId = $("#borrwoMode_UserID").val().trim();
    if (userId === "") { $("#BorrowModeUser").html(pleaseInputUserId); return; }
    $.post("/Backend/Manage/BorrowUserMessage", { userId: userId }, (result) => {
        if (result === false) { $("#BorrowModeUser").html(pleaseInputUserId); return; }
        $("#BorrowModeUser").html(result);
    })
}
// 動態搜尋 書本資訊
function BorrowModeModeBookDynamic() {
    console.log("書本資訊");
    $("#BorrowModeSuccessContent").html("");
    let bookId = $("#borrwoMode_BookCode").val().trim();
    if (bookId === "") { $("#BorrowModeBook").html(pleaseInputBookId); return; }
    console.log("測試動態書本資訊: " + bookId);
    $.post("/Backend/Manage/BorrowBookMessage", { bookId: bookId }, (result) => {
        if (result === false) { $("#BorrowModeBook").html(pleaseInputBookId); return; }
        $("#BorrowModeBook").html(result);
    })
}
// 借閱書籍 發送POST
function BorrowModeSend() {

    let userId = $("#borrwoMode_UserID").val();
    let bookId = $("#borrwoMode_BookCode").val();
    if (userId === "" && bookId === "") { alert("請輸入借閱者ID和書籍編號"); return; }
    if (userId === "") { alert("請輸入借閱者ID!!"); return; }
    if (bookId === "") { alert("請輸入書籍編號!!"); return; }
    let formData = $("#borrwoModeForm").serialize();

    let btnValue = $(this).val();
    if (btnValue === "borrow") {
        $.post("/Backend/Manage/BorrowSend", formData, (result) => {
            if (result === 0) { $("#BorrowModeSuccessContent").html(pleaseInputUserId); return; }
            $("#BorrowModeSuccessContent").html(result);
        })
    }
    
}
function CancelBtnUser() {
    console.log("點擊清除按鈕")
    $(this).closest(".input-group").find(".form-control").val("");
    $("#BorrowModeSuccessContent").html("");
    $("#BorrowModeUser").html(pleaseInputUserId2);
    $
}
function CancelBtnBook() {
    console.log("點擊清除按鈕")
    $(this).closest(".input-group").find(".form-control").val("");
    $("#BorrowModeSuccessContent").html("");
    $("#BorrowModeBook").html(pleaseInputBookId2);
}
// #endregion 借書模式 END

// #region 還書模式 Module
function ReturnBookMode() {
    $("#content-panel").load("/Backend/Manage/ReturnBookMode", () => {
        $("#ReturnBookBtn").on("click", ReturnBookSend);
        $("#ReturnBook_CancelBookNumBtn").on("click", CancelBtn);
    })
}

// 還書送出
function ReturnBookSend() {
    let bookId = $("#ReturnBookCode").val();
    if (bookId === "") { alert("請輸入書籍編號!!"); return; }
    let data = $("#ReturnBookIdForm").serialize();
    $.post("/Backend/Manage/ReturnBookSend", data, (result) => {
        if (result == 0) { $("#ReturnBookContent").html(retrunFalse); return; }
        $("#ReturnBookContent").html(result);
        $("#ReturnBookCode").val("");
    })
}
// #endregion 還書模式 END Module

// #region 預約模式 Module
function AppointmentMode() {
    console.log("預約模式進入");
    $("#content-panel").load("/Backend/Manage/AppointmentMode1", () => {
        console.log("已進入Action")
        $("#appointmentSend").on("click", AppointmentModeSend);
        $("#appointmentMode_KeyWord").on("input", AppointmentModeBookDynamic);
        $("#appointmentMode_CancelUserIdBtn ,#appointmentMode_CancelBookNumBtn").on("click", CancelBtn);
        $("#appointmentMode_CancelKeyWordBtn").on("click", CancelBtn_AppointVersion);
        //$("#appointmentMode_status, #appointmentMode_perPage").on("change", AppointmentModeBookDynamic);
    })
}

// 關鍵字查詢
function AppointmentModeBookDynamic() {
    let keyWord = $("#appointmentMode_KeyWord").val();
    let state = $("#appointmentMode_status").val();
    let pageCount = $("#appointmentMode_perPage").val();
    let page = $(this).data("page") || 1;
    let obj = { keyWord: keyWord, state: state, pageCount: pageCount, page: page }
    console.log(`${keyWord}與${state}與${pageCount}與${page}`);
    if (keyWord === " ") {
        alert("請不要輸入空字串");
        $("#appointmentMode_KeyWord").val("");
        return
    }
    if (keyWord === "") { $("#appointmentQueryBook").remove; $("#appointmentQueryBook").html(appointmentQueryBookHtml); return }
    $.post("/Backend/Manage/AppointmentMode1Query", obj, (result) => {
        if (result == 0) {
            $("#appointmentQueryBook").html(appointmentQueryBookHtml);
            $("#appointmentMode_status").val(state);
            appointmentOnChange();
            return;
        }
        $("#appointmentQueryBook").html(result);
        $("#appointmentMode_status").val(state);
        $("#appointmentMode_perPage").val(pageCount);
        console.log("成功載入書本");
        $(".AppointmentMode_AddBookNumBtn").on("click", AppointmentModeAddBook);
        appointmentOnChange();
    });
};
function appointmentOnChange() {
    $("#appointmentMode_status").on("change", AppointmentModeBookDynamic);
    $("#appointmentMode_perPage").on("change", AppointmentModeBookDynamic);
    $(".page-link").on("click", AppointmentModeBookDynamic);
}
// 預約按鈕發送
function AppointmentModeSend() {
    let userId = $("#appointmentMode_UserID").val();
    let BookId = $("#appointmentMode_BookNumber").val();
    if (userId === "" && BookId === "") { alert("請輸入借閱者ID和書籍編號"); return; }
    if (userId === "") { alert("請輸入借閱者ID!!"); return; }
    if (BookId === "") { alert("請輸入書籍編號!!"); return; }
    let formData = $("#appointmentModeForm").serialize();
    console.log("測試輸入: " + formData);
    $.post("/Backend/Manage/AppointmentMode1Send", formData, (result) => {
        if (result === 0) { $("#appointmentSuccessContent").html(pleaseInputUserId); return; }
        if (result === 1) { $("#appointmentSuccessContent").html(pleaseInputBookId); return; }
        $("#appointmentSuccessContent").html(result);
        console.log("預約按鈕是否成功回傳，YEEEEEE")
        AppointmentModeBookDynamic();
    })
}
// 加入書籍編號到輸入框
function AppointmentModeAddBook() {
    let bookNumber = $(this).closest("tr").find("td").data("bookid");
    console.log("你的書本編號: " + bookNumber)
    $("#appointmentMode_BookId").val(bookNumber);
}
// 關鍵字專屬清潔按鈕
function CancelBtn_AppointVersion() {
    console.log("點擊清除按鈕");
    $(this).closest(".input-group").find(".form-control").val("");
    $("#appointmentQueryBook").remove; $("#appointmentQueryBook").html(appointmentQueryBookHtml);
}

// #endregion 預約模式 Module END

// #region 通用函數
let TempBookName;
let DueDate;


// 按鈕清除 
function CancelBtn() {
    console.log("點擊清除按鈕")
    $(this).closest(".input-group").find(".form-control").val("");
}
// ++++DROP ++++通知&取消預約按鈕
//function appointment_cancelEvent() {
//    let appointmentid = $(this).closest("tr").find(".appointmentid").data("appointmentid");
//    $.post("/Manage/AppointmentCancel", { appointmentid: appointmentid }, (result) => {
//        if (result == "") { alert(`成功取消預約，預約編號: ${appointmentid}`) }
//        else { alert("預約取消失敗"); }
//        appointment_queryEvent();
//    })
//    console.log("取消按鈕測試: " + appointmentid);
//}

// 清空搜尋資料
function appointment_clearEvent() {
    $("#appointmenSearch")[0].reset();
}
// 點擊通知
function NotificationBtn() {
    $("#NotificationType").on("change", ChageNotificationType);
    TempBookName = $(this).closest("tr").find(".BorrowBookTitle").text();
    DueDate = $(this).closest("tr").find(".BorrowDueDate").text();
    ChageNotificationType();
    let recipientId = $(this).closest("tr").find(".NotificationUserid").text();
    let recipientName = $(this).closest("tr").find(".NotificationUserName").text();

    let typeinput = $("#NotificationType").val();
    $("#NotificationInput").val(typeinput);
    $("#NotificationId").val(recipientId);
    $("#NotificationName").val(recipientName);
};

// 預設通知內容
function ChageNotificationType() {
    let NotificationType = $("#NotificationType").val();
    let UpcomingExpirationNoticeText = `【即將到期通知】\n親愛的用戶您好，\n您所借閱的書籍「《${TempBookName}》」\n即將於 { ${DueDate} } 到期 \n 請您於期限前歸還，謝謝。圖書館管理系統 敬上。`;
    let ExpirationNoticeWarningText = `【逾期警告通知】\n親愛的用戶您好，你所借閱的《${TempBookName}》已逾期\n請儘速歸還並聯繫館方補辦相關事宜，謝謝您的配合。`;

    if (NotificationType === "UpcomingExpirationNotice") { $("#NotificationTextarea").val(UpcomingExpirationNoticeText); }
    if (NotificationType === "ExpirationNoticeWarning") { $("#NotificationTextarea").val(ExpirationNoticeWarningText); }
    if (NotificationType === "Other") { $("#NotificationTextarea").val(""); }
}

// 送出按鈕
function NotificationMessageSend() {
    let myform = $("#NotificationFom").serialize();
    console.log(myform);
    $.post("/Backend/Manage/Notification", myform, (result) => {
        if (result === 1) { alert("成功送出!!"); }
        else if (result === 0) { alert("送出失敗...."); }
        else { alert(result); }
        NotificationClose();
    })
};
// 清除按鈕
function NotificationClearBtn() {
    $("#NotificationTextarea").val("");
}
// 關閉視窗
function NotificationClose() {
    console.log("關閉視窗按鈕成功")
    $('#notificationModal').modal("hide");
    $("#NotificationTextarea").val("");
    $("#NotificationType").val("UpcomingExpirationNotice");
}

// #endregion

// #region 可用的HTML
// 預約模式_顯示欄位
let appointmentQueryBookHtml = `
        <div class="d-flex justify-content-between align-items-end">
            <div class="d-flex flex-wrap gap-3 justify-content-end">
                <div>
                    <label for="appointmentMode_status" class="form-label mb-0">目前狀態：</label>
                    <select id="appointmentMode_status" name="appointmentMode_status" class="form-select form-select-sm">
                        <option value="IsLent">可借閱</option>
                        <option value="Available">借閱中</option>
                        <option value="ALL" selected>全部</option>
                    </select>
                </div>
                <div>
                    <label for="appointmentMode_perPage" class="form-label mb-0">每頁筆數：</label>
                    <select id="appointmentMode_perPage" name="appointmentMode_perPage" valus="10" class="form-select form-select-sm">
                        <option value="10" selected>10</option>
                        <option value="20">20</option>
                        <option value="30">30</option>
                    </select>
                </div>
            </div>
        </div><table class="table mt-2">
        <thead>
            <tr>
                <th scope="col">書籍名稱</th>
                <th scope="col">作者</th>
                <th scope="col">狀況</th>
                <th scope="col">預約人數</th>
                <th scope="col">操作</th>
            </tr>
        </thead>
    </table><h1 class="text-danger">查無書籍...</h1>`;

let pleaseInputUserId = `<div class="alert alert-danger fs-1">該名借閱者不存在，請重新輸入</div>`;
let pleaseInputBookId = `<div class="alert alert-danger fs-1 mt-5">該本書籍不存在，請重新輸入!</div>`;
let pleaseInputUserId2 = `<div class="alert alert-danger fs-1">請輸入借閱者ID</div>`
let pleaseInputBookId2 = `<div class="alert alert-danger fs-1">請輸入書本編號</div>`;
let QueryWait = `<div class="alert alert-danger fs-1 mt-5">請稍後....</div>`;
let QueryFalse = `<div class="alert alert-danger fs-1">查無資料</div>`;
let retrunFalse = `<div class="alert alert-danger fs-1">輸入錯誤，請重新輸入!</div>`;


// #endregion

//#region Db連線
function DbContextText() {
    console.log("DB連線測試");
    $.post("/Backend/Manage/TestDbContext", (restult) => {
        if (restult === true) { alert("連線成功!!!"); }
        else {alert("連線失敗...") }
    })
}
//#endregion