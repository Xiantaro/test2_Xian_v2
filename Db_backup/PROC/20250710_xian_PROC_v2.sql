-- 圖書管理系統: 借書、還書、預約、取消預約

--1. 借閱模式
--書本資訊
CREATE PROC BookInfomationForBorrow
@bookCode NVARCHAR(500)
AS
BEGIN
		SELECT bok.bookCode, 
			col.title, 
			auth.author, 
			col.translator, 
			typ.[type], 
			col.publisher, 
			col.publishDate, 
			col.collectionImg, 
			bokstu.bookStatus
		FROM Book bok JOIN Collection col ON bok.collectionId = col.collectionId
		JOIN BookStatus bokstu ON bok.bookStatusId = bokstu.bookStatusId
		JOIN [Type] typ ON col.typeId = typ.typeId
		JOIN Author auth ON auth.authorId = col.authorId
		WHERE bok.bookCode = @bookCode
END
GO

--借閱結果
CREATE PROC BorrowResult
    @cid INT,
    @bookCode NVARCHAR(500)
AS
BEGIN 
    BEGIN TRY
        SET TRANSACTION ISOLATION LEVEL REPEATABLE READ
        BEGIN TRAN
        -- 確定該借閱者是否存在
        IF NOT EXISTS( SELECT 1 FROM Client WHERE cId = @cid)
        BEGIN
            SELECT 1 AS ResultCode, ('該借閱者不存在，請重新輸入!') AS Message;
            ROLLBACK
            RETURN
        END
        
        -- 確認書是否存在
        IF NOT EXISTS ( SELECT 1 FROM Book WHERE bookCode = @bookCode)
        BEGIN
            ROLLBACK
            SELECT 2 AS ResultCode, ('該本書籍不存在，請重新輸入!') AS Message;
            RETURN
        END

        DECLARE @bookid INT;
		SELECT @bookid = bookid
            FROM Book
            WHERE bookCode = @bookCode
        -- 這本書在館內時 1.可借閱
        IF EXISTS ( SELECT 1 FROM Book WHERE bookCode = @bookCode AND bookStatusId = 1)
        BEGIN
			-- 直接更新為借出狀態(1)
            UPDATE Book WITH(ROWLOCK, UPDLOCK) SET bookStatusId = 2
            WHERE bookCode = @bookCode AND bookStatusId = 1;
						
			-- 如果沒有更新成功，代表被預約了....
            IF @@ROWCOUNT = 0
            BEGIN
                SELECT 3 AS ResultCode, '該本書已被其他人借走' Message;
                ROLLBACK;
                RETURN;
            END
			-- 插入借閱紀錄
            INSERT INTO Borrow (cId, bookid, borrowDate, dueDateB, borrowStatusId)
            VALUES(@cid, @bookid, GETDATE(), DATEADD(DAY, 14, GETDATE()), 2)

            SELECT 0 AS ResultCode, ('借閱成功') AS Message;
            COMMIT
            RETURN
        END
        -- 如果有在網路上預約(2)的話
        DECLARE @reservationId INT;
        IF EXISTS (
            SELECT 1 FROM Book WHERE bookCode = @bookCode AND bookStatusId = 2
        )
        BEGIN
            -- 判斷該人是否是 收到通知的預約者前來領書 => 狀態: 待取書中
            SELECT @reservationId = reservationId
            FROM Reservation WITH(UPDLOCK, ROWLOCK)
            WHERE cid = @cid AND reservationStatusId = 3
            AND bookId = @bookId

            IF @reservationId IS NULL
            BEGIN 
                SELECT 2 AS ResultCode, ('此書本已被借閱!') AS Message;
                ROLLBACK
            END

            --  改成預約狀態 已借出
            UPDATE Reservation SET reservationStatusId = 1
            WHERE reservationId = @reservationId AND cid = @cid

            IF @@ROWCOUNT = 0
            BEGIN 
                SELECT 3  ResultCode, ('書籍狀態已變更，無法完成借書') AS Message;
                ROLLBACK;
                RETURN;
            END

			-- 插入借閱紀錄
            INSERT INTO Borrow (cId,reservationId, bookId, borrowDate, dueDateB, borrowStatusId)
            VALUES(@cid, @reservationId , @bookId, GETDATE(), DATEADD(DAY, 14, GETDATE()), 2)

            SELECT 0 AS ResultCode, ('借閱成功') AS Message;
            COMMIT;
            RETURN;
        END
        -- 
        SELECT 2 AS ResultCode, ('該本書已借出，請重新輸入') AS Message;
        ROLLBACK
        RETURN
    END TRY
    BEGIN CATCH 
        PRINT '交易錯誤'
        ROLLBACK
        SELECT 500 ResultCode, '發生錯誤' Message
    END CATCH
END
GO

--2. 還書模式
--Main-還書
CREATE PROC ReturnBook
			@BookCode NVARCHAR(500)
AS
BEGIN
		BEGIN TRY
				SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
				BEGIN TRAN
				DECLARE @borrwid INT
				DECLARE @collectionId INT
				DECLARE @bookid INT

				-- 找出借閱紀錄
				SELECT TOP 1 @borrwid = borrowid, @collectionId = collectionId, @bookid = bok.bookid
				FROM Borrow bor JOIN Book bok ON bor.bookId  = bok.bookId
				WHERE bookCode = @BookCode AND bor.borrowStatusId IN ( 2,3) 
				ORDER BY borrowDate DESC

				-- 確認是否有被借閱
				IF @borrwid IS NULL
				BEGIN 
					ROLLBACK
					SELECT 0 ResultCode, ('書本編號:' +@BookCode +' 並未被借閱。') Message
					RETURN
				END

				-- 如果書本逾期
				IF EXISTS ( SELECT 1 FROM Borrow bor WHERE bor.borrowId = @borrwid AND returnDate IS NULL  AND borrowStatusId = 3) 
				BEGIN 
					UPDATE Borrow WITH(ROWLOCK,UPDLOCK) SET returnDate = GETDATE()
					WHERE borrowId = @borrwid 
					SELECT 0 ResultCode, ('書本編號:' +@BookCode + ' 逾期歸還。' ) Message
					COMMIT
					RETURN
				END
				
				-- 確認是該本書被借閱 => 要歸還
				IF  @borrwid IS NOT NULL 
				BEGIN
					DECLARE @affectedBorrow INT =0, @affectedBook INT = 0;
					-- 如果沒已借閱者
						-- 更新已歸還
					UPDATE Borrow WITH(ROWLOCK, UPDLOCK) SET borrowStatusId = 1, returnDate = GETDATE()
					WHERE borrowId = @borrwid SET @affectedBorrow = @@ROWCOUNT
					
					IF @affectedBorrow = 0 
					BEGIN 
						SELECT 0 ResultCode, '歸還失敗.....' Message
						ROLLBACK
						RETURN
					END

					
					--如果有其他借閱者
					IF EXISTS ( SELECT 1 FROM Reservation re WHERE collectionId = @collectionId AND reservationStatusId = 2 )
					BEGIN
						EXEC CheckBookIsReservation @collectionId, @bookid
						COMMIT 
						RETURN 
					END

					-- 更新可借閱
					UPDATE Book WITH(ROWLOCK, UPDLOCK) SET bookStatusId = 1
					WHERE bookCode = @BookCode  SET @affectedBook =@@ROWCOUNT

					IF @affectedBook = 0 
					BEGIN 
						SELECT 0 ResultCode, '歸還失敗.....' Message
						ROLLBACK
						RETURN
					END
					SELECT 1 ResultCode, ('書本編號:' +@BookCode + ' 歸還成功!' ) Message
					COMMIT
					RETURN
				END

				-- 該本書不存在
				ROLLBACK
				SELECT 0 ResultCode, '該本書不存在' Message
				RETURN
		END TRY
		BEGIN CATCH
				SELECT 0 ResultCode, ('發生未知錯誤: '+ ERROR_MESSAGE()) Message
				ROLLBACK
				RETURN
		END CATCH
END
GO

-- 檢查書本最早預約者
CREATE PROC CheckBookIsReservation
    @collectionId int,
	@bookid INT
AS 
BEGIN
    BEGIN TRY

        DECLARE @cid INT;
        DECLARE @reservationId INT;
        DECLARE @ResultCode INT;
		DECLARE @message NVARCHAR(500);
		CREATE TABLE #tmpNotifyResult (
				ResultCode INT,
				Message NVARCHAR(500)
		)

        --取的該本書最早預約的
        SELECT @reservationId = reservationId, @cid = cid
        FROM GetEarliestReservation(@collectionId)
		IF (@reservationId IS NULL OR @cid IS NULL)
		BEGIN 
				SELECT 0 ResultCode, '無有效預約紀錄' Message
				RETURN
		END
        -- 更新預約狀態"可取書"、三天取書時間
        UPDATE Reservation WITH(ROWLOCK, UPDLOCK)
        SET bookid = @bookid, reservationStatusId = 3, dueDateR = DATEADD(DAY, 3, GETDATE())
        WHERE reservationId = @reservationId AND cid = @cid
		AND reservationStatusId = 2


        IF @@ROWCOUNT = 0
        BEGIN 
            SELECT 0 ResultCode, '更改預約者狀態失敗.....' MESSAGE
            RETURN
        END

		-- 通知
		INSERT INTO #tmpNotifyResult
        EXEC NotificationBooker @cid, @collectionId

		SELECT @ResultCode = ResultCode, @message =  Message FROM #tmpNotifyResult

		IF @ResultCode = 0
		BEGIN 
				SELECT 0 ResultCode, '通知預約者失敗'+ @message Message
				RETURN
		END
 
        SELECT 1 ResultCode, '已成功更改預約者狀態: 待取書中。' Message 
		DROP TABLE #tmpNotifyResult
    END TRY
    BEGIN CATCH
        SELECT 0 ResultCode, '發生錯誤:' + ERROR_MESSAGE() Message
        RETURN
    END CATCH
END 
GO

--TVF  取得最早預約者
CREATE FUNCTION GetEarliestReservation(@collectionId INT)
RETURNS TABLE
AS 
RETURN
(
	WITH ReservationCTE AS (
			SELECT reservationId,  collectionId, cid, reservationDate, rs.reservationStatus,ROW_NUMBER() OVER(PARTITION BY collectionId  ORDER BY  reservationDate ASC) rk
			FROM Reservation re  JOIN ReservationStatus rs ON re.reservationStatusId =  rs.reservationStatusId
			WHERE collectionId = @collectionId AND  re.reservationStatusId = 2
	)
    SELECT reservationId, collectionId, cid,reservationDate, reservationStatus
    FROM ReservationCTE
    WHERE rk = 1
)
GO

--通知該名預約者取書
CREATE PROC NotificationBooker
    @cid INT,
    @collectionId INT
AS
BEGIN
    BEGIN TRY
        DECLARE @message NVARCHAR(500); 
		DECLARE @title NVARCHAR(500); 
		DECLARE @cName NVARCHAR(20);

        SELECT @cName = cName FROM Client WHERE cid = @cid
        SELECT @title = title FROM Collection WHERE collectionId = @collectionId
		IF (@cName IS NULL OR @title IS NULL)
		BEGIN 
			SELECT 0 ResultCode, '通知失敗: 找不到預約者或書籍' Message
			RETURN
		END

        SET @message = N'【取書通知】親愛的 ' + @cName +
                       N' ，您所預約的書 [' + @title + 
                       N'] 已可以借閱，請於3天內到本館借書，謝謝。圖書館管理系統 敬上。';

        INSERT INTO Notification (cid, [message], notificationDate) VALUES (@cid,  @message, GETDATE());
		SELECT 1 ResultCode, '通知成功' Message
    END TRY
    BEGIN CATCH
		SELECT 0 ResultCode, '通知失敗: ' + ERROR_MESSAGE() Message
        RETURN
    END CATCH
END
GO


--3.預約模式
--Main-預約
CREATE PROC RevervationMode
    @cid INT,
    @collectionId INT
AS 
BEGIN 
    BEGIN TRY
        BEGIN TRANSACTION
        -- 先確認使用者是否存在
        IF NOT EXISTS ( SELECT 1 FROM Client WHERE cid = @cid)
        BEGIN
            SELECT 0 ResultCode, '使用者不存在' Message 
			RETURN
        END
        -- 確認書本是否存在
        IF NOT EXISTS (SELECT 1 FROM Collection WHERE collectionId = @collectionId)
        BEGIN
            SELECT 0 ResultCode, '書本不存在' Message  
			RETURN
        END 
        -- 如果預約已經存在
        IF EXISTS (SELECT 1 FROM Reservation WHERE cid=@cid AND collectionId = @collectionId AND reservationStatusId  = 2 )
        BEGIN 
				SELECT 0 ResultCode, '重複預約' Message
				RETURN
		END
        -- 如果該本書 在館內 ok
        IF EXISTS (SELECT 1 FROM Book WITH(HOLDLOCK) WHERE collectionId = @collectionId AND  bookStatusId = 1)
        BEGIN 
                ROLLBACK 
                SELECT 0 ResultCode, '此本書目前在本館' Message
				RETURN
        END 
        -- 如果該本書有預約 ok
        IF EXISTS (SELECT 1 FROM Book WHERE bookStatusId  = 2 AND collectionId = @collectionId)
        BEGIN 
            INSERT INTO Reservation (cId, collectionId, reservationDate,reservationStatusId)
			VALUES (@cid, @collectionid, GETDATE(), 2)
			EXEC NotificationAppointmentSuccess @cid,@collectionid
            SELECT 1 ResultCode, '預約成功' Message 
            COMMIT 
            RETURN 
        END 

        ROLLBACK
		SELECT 0 ResultCode, '預約失敗' Message
		RETURN
    END TRY
    BEGIN CATCH 
    DECLARE @ErrMsg NVARCHAR(4000), @ErrSeverity INT
    SELECT @ErrMsg = ERROR_MESSAGE(), @ErrSeverity = ERROR_SEVERITY()
    ROLLBACK
    SELECT 0 ResultCode, '出現錯誤: ' + @ErrMsg Message 
    RETURN
END CATCH
END
GO

-- 書本借閱人數
CREATE PROC BookStatusDetail
    @keyword NVARCHAR(500),
    @bookstatus NVARCHAR(20)
AS 
BEGIN 
    SELECT col.collectionId ,title, author, bookstatus, 
	( SELECT COUNT(*) FROM Reservation r WHERE r.collectionId = col.collectionId AND reservationStatusId = 2) number
    FROM Collection col 
	JOIN Book bok ON col.collectionId = bok.collectionId
    JOIN BookStatus bokstu ON bok.bookStatusId = bokstu.bookStatusId
	JOIN Author auth ON auth.authorId = col.authorId
    LEFT JOIN Reservation re ON col.collectionId = re.collectionId
    WHERE (col.title LIKE '%'+ @keyword + '%' OR auth.author LIKE '%' + @keyword + '%') 
    AND (
            @bookstatus = 'ALL' AND bok.bookStatusId IN (1,2, 3) OR
            (@bookstatus = 'IsLent' AND bok.bookStatusId IN (2)) OR
            (@bookstatus = 'Available' AND bok.bookStatusId = 1)
    )
    GROUP BY  col.collectionId, title, author, bokstu.bookStatus
END;
GO

--成功預約通知
CREATE PROC NotificationAppointmentSuccess
    @cid INT,
    @collectionId INT
AS
BEGIN
    BEGIN TRY
        DECLARE @message NVARCHAR(500); 
		DECLARE @title NVARCHAR(500); 
		DECLARE @cName NVARCHAR(20);

        SELECT @cName = cName FROM Client WHERE cid = @cid
        SELECT @title = title FROM Collection WHERE collectionId = @collectionId
		IF (@cName IS NULL OR @title IS NULL)
		BEGIN 
			SELECT 0 ResultCode, '通知失敗: 找不到預約者或書籍' Message
			RETURN
		END

        SET @message = N'【預約成功通知】親愛的 ' + @cName +
                       N' ，您已於'  + CONVERT(NVARCHAR ,FORMAT(GETDATE(), 'yyyy-MM-dd hh-mm-ss'))+ N'預約了 ['+ @title + 
                       N'] 請耐心等候通知，謝謝!!';

        INSERT INTO Notification (cid, [message], notificationDate) VALUES (@cid,  @message, GETDATE());
		SELECT 1 ResultCode, '通知成功' Message
    END TRY
    BEGIN CATCH
		SELECT 0 ResultCode, '通知失敗: ' + ERROR_MESSAGE() Message
        RETURN
    END CATCH
END
GO

--TVP專區
CREATE TYPE OverDueTVP2 AS TABLE(
	[reservationId] [int] NULL,
	[borrowId] [int] NULL,
	[cid] [int] NULL,
	[collectionId] [int] NULL,
	[bookid] [int] NULL,
	[dueDateB] [datetime2](7) NULL
)
GO

--4.借閱逾期
-- Main-借閱預期PROC 
CREATE PROC LateReturn
AS
BEGIN
		SET NOCOUNT ON
		BEGIN TRY
			BEGIN TRANSACTION
			DECLARE @DueList OverDueTVP2;
			
			INSERT INTO @DueList ( cid, bookid,borrowId, dueDateB)
			SELECT  cid, bookid,borrowId, dueDateB
			FROM Borrow
			WHERE dueDateB < GETDATE() AND borrowStatusId = 2
			IF NOT EXISTS ( SELECT 1 FROM @DueList)
			BEGIN
				SELECT 0 ResultCode, '無逾期者' Message
				ROLLBACK
				RETURN 
			END

			UPDATE Borrow WITH(ROWLOCK,UPDLOCK) SET borrowStatusId = 3
			FROM Borrow bow
			JOIN @DueList due ON bow.borrowId = due.borrowId AND bow.cid = due.cid

			IF @@ROWCOUNT = 0
			BEGIN 
				SELECT 0 ResultCode, '無更新逾期' Message
				ROLLBACK
				RETURN 
			END

			--通知
			EXEC NotificationOverdueBorrow @OverDue = @DueList

			SELECT 1 ResultCode, '檢查逾期者結束...' Message
			COMMIT
		END TRY

		BEGIN CATCH
			IF @@TRANCOUNT > 0 
			ROLLBACK
			SELECT 0 ResultCode, '出現錯誤' + ERROR_MESSAGE() Message
		END CATCH
END
GO

--副-預期通知
CREATE PROC NotificationOverdueBorrow
    @OverDue OverDueTVP2 READONLY
AS
BEGIN
    BEGIN TRY
		INSERT INTO Notification (cid, message, notificationDate )
		SELECT
				od.cid,
				N'【逾期警告通知】親愛的 ' + cli.cName +
                       N' ，您所借閱的 [' + col.title + 
                       N'] 未於 { ' + CONVERT(NVARCHAR, od.dueDateB, 111) +' } 前還書，請盡速還書，並增加違規點數1點!!',
				GETDATE()
		FROM @OverDue od
		JOIN Client cli ON od.cid = cli.cid 
		JOIN Book bok ON od.bookid = bok.bookId
		JOIN Collection col ON bok.collectionId = col.collectionId

		SELECT 1 ResultCode, '所有逾通知已發放成功' Message
    END TRY
    BEGIN CATCH
		SELECT 0 ResultCode, '通知失敗: ' + ERROR_MESSAGE() Message
        RETURN
    END CATCH
END

GO

--5.取書逾期
GO
--通知預期者已取消取書
CREATE PROC NotificationOverdue
    @OverDue OverDueTVP2 READONLY
AS
BEGIN
    BEGIN TRY
		INSERT INTO Notification (cid, message, notificationDate )
		SELECT
				od.cid,
				N'【預約取消通知】親愛的 ' + cli.cName +
                       N' ，您所預約的 [' + col.title + 
                       N'] 未於 { ' + CONVERT(NVARCHAR, DATEADD(DAY, -1, GETDATE()), 111) +' } 前取書，系統已取消，如有需要請重新預約，謝謝!!',
				GETDATE()
		FROM @OverDue od
		JOIN Client cli ON od.cid = cli.cid 
		JOIN Collection col ON col.collectionId = od.collectionId

		SELECT 1 ResultCode, '所有逾通知已發放成功' Message
    END TRY
    BEGIN CATCH
		SELECT 0 ResultCode, '通知失敗: ' + ERROR_MESSAGE() Message
        RETURN
    END CATCH
END

GO

--通知預約者取書(TVP版)
CREATE PROC NotificationBookerTVPvserion
    @reservationer OverDueTVP2 READONLY
AS
BEGIN
    BEGIN TRY
			SELECT 'Test' Test, * FROM @reservationer
			INSERT INTO Notification (cid, [message], notificationDate)
			SELECT  
				cli.cid,
				N'【取書通知】親愛的 ' + cli.cName +
                       N' ，您所預約的書 [' + col.title + 
                       N'] 已可以借閱，請於3天內到本館借書，謝謝!!',
					   GETDATE()
			FROM @reservationer re 
			JOIN Client cli ON re.cid = cli.cid
			JOIN Collection col ON re.collectionId = col.collectionId
		
		SELECT COUNT(*) NotificationCount, '通知預約者取書(TVP版)成功' Message
    END TRY
    BEGIN CATCH
		SELECT 0 ResultCode, '通知失敗: ' + ERROR_MESSAGE() Message
        RETURN
    END CATCH
END
GO

--檢查其他預約者及書本狀態更新
CREATE PROC CheckReservationSchedule
		@DueBook OverDueTVP2 READONLY
AS 
BEGIN
    BEGIN TRY
		-- 用來存放-書籍有其他預約者的TVP
		DECLARE @reservationList OverDueTVP2;
		-- 用來存放-書籍沒有預約者的TVP
		DECLARE @bookList  OverDueTVP2;

		-- 1.取得沒有預約的書
		INSERT INTO @bookList (collectionId, bookid)
		SELECT  collectionId, bookid 
		FROM @DueBook bok 
		WHERE NOT EXISTS 
		( SELECT 1 FROM Reservation re WHERE re.collectionId = bok.collectionId   AND reservationStatusId IN ( 2, 3)  )

		-- 1.1 更新書本狀態為 1.可借閱
		IF EXISTS (SELECT 1 FROM @bookList)
		BEGIN
					UPDATE  Book SET bookStatusId = 1
					FROM Book bok JOIN @bookList boklist ON bok.bookid = boklist.bookid AND bok.collectionId = boklist.collectionId
					SELECT '更新書本狀態可借閱' message
		END

        --2.取得書籍最早預約的人 ok
		INSERT INTO @reservationList (reservationId, cid, bookid,collectionid )
        SELECT reservationId,  cid,bookid,collectionid 
        FROM GetEarliestReservationTVF(@DueBook)
		
        -- 2.2更新預約狀態"可取書"、三天取書時間 ok
        UPDATE Reservation WITH(ROWLOCK, UPDLOCK)
        SET bookid = rlist.bookid, reservationStatusId = 3, dueDateR = DATEADD(DAY, 3, GETDATE())
        FROM Reservation re JOIN @reservationList rlist ON re.reservationId = rlist.reservationId
		SELECT * FROM @reservationList
		--2.3通知預約者 ok
		IF EXISTS ( SELECT 1 FROM @reservationList)
		BEGIN
				EXEC NotificationBookerTVPvserion  @reservationer = @reservationList
				SELECT '已通知下一個預約者' message
		END

        SELECT 1 ResultCode, '已成功進行通知預約者及書本狀態' Message 

    END TRY
    BEGIN CATCH
        SELECT 0 ResultCode, '發生錯誤:' + ERROR_MESSAGE() Message
        RETURN
    END CATCH
END 
GO

--VTF取得書本最早預約
CREATE FUNCTION GetEarliestReservationTVF
(
	@InputTable OverDueTVP2 READONLY
)
RETURNS @RESULT TABLE (
		reservationId INT,
		collectionid INT,
		cid INT,
		bookid INT,
		reservationDate DATETIME,
		reservationStatus NVARCHAR(50)
)
AS 
BEGIN
		INSERT INTO @RESULT
		SELECT 
				re.reservationId,
				re.collectionid,
				re.cId,
				re.bookid,
				re.reservationDate,
				rs.reservationStatus
		FROM (
			SELECT 
				re.reservationId,  
				re.collectionid, 
				re.cid, 
				tvp.bookId,
				re.reservationDate, 
				re.reservationStatusId,
				ROW_NUMBER() OVER(PARTITION BY re.collectionId  ORDER BY  re.reservationDate ASC) rk
				FROM Reservation re
				JOIN @InputTable tvp ON re.collectionid = tvp.collectionid
				WHERE re.reservationStatusId = 2
			) AS re
			JOIN ReservationStatus rs ON re.reservationStatusId = rs.reservationStatusId
			WHERE re.rk = 1
			RETURN;
END
GO

--Main-取書逾期
CREATE PROC OverDue
AS
BEGIN
	BEGIN TRY
		
		DECLARE @OverDueList OverDueTVP2
		-- 插入逾期者
		INSERT INTO @OverDueList ( reservationId, cid, collectionId, bookId )
		SELECT reservationId, cid, collectionId, bookId
		FROM Reservation
		WHERE reservationStatusId = 3 AND dueDateR < GETDATE()
		-- 檢查是否有逾期者
		IF NOT EXISTS ( SELECT 1 FROM @OverDueList)
		BEGIN
			SELECT 0 ResultCode, '沒有逾期者!' Message
			RETURN
		END
		BEGIN TRAN
		UPDATE Reservation WITH(ROWLOCK,UPDLOCK) SET reservationStatusId = 4
		WHERE reservationId IN ( SELECT reservationId FROM @OverDueList);
		--
		IF (@@ROWCOUNT = 0)
		BEGIN 
			ROLLBACK
			SELECT 0 ResultCode, '無逾期紀錄。' Message
			RETURN
		END
		-- 以上OK

		-- 開始進行

		--1. 通知逾期者已取消取書狀態 測試OK  、 part2 ok
		EXEC NotificationOverdue @OverDue = @OverDueList

		--2. 檢查該本書是否有其他預約者或沒有其他預約並該改新書本狀態 OK、
		-- checkpart2 
		EXEC CheckReservationSchedule @DueBook= @OverDueList

		COMMIT
		SELECT 1 ResultCode, '每日逾期排成完成!' Message;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 ROLLBACK;
		SELECT 0 ResultCode, N'執行失敗' + ERROR_MESSAGE() Message;
	END CATCH
END
GO

---------------------------
-- 即將預期通知
-- SUP
---即將逾期通知
CREATE PROC NotificationAboutToExpire
    @OverDue OverDueTVP2 READONLY,
	@Day INT
AS
BEGIN
    BEGIN TRY
		INSERT INTO Notification (cid, message, notificationDate )
		SELECT
				od.cid,
				N'【即將逾期通知】親愛的 ' + cli.cName +
				N' ，您所借閱的 [' + col.title +
				N'] 將於 { ' + CONVERT(NVARCHAR, od.dueDateB, 111) +
				N' } 逾期，距離還書期限僅剩 ' + CONVERT(NVARCHAR, @Day) +
				N' 天。請儘速歸還。圖書館管理系統 敬上。',
				GETDATE()
		FROM @OverDue od
		JOIN Client cli ON od.cid = cli.cid 
		JOIN Book bok ON od.bookid = bok.bookId
		JOIN Collection col ON bok.collectionId = col.collectionId

		SELECT 1 ResultCode, '所有即將預期通知已發放成功' Message
    END TRY
    BEGIN CATCH
		SELECT 0 ResultCode, '通知失敗: ' + ERROR_MESSAGE() Message
        RETURN
    END CATCH
END
GO

-- Main_即將預期通知
CREATE PROC LateReturnOneToThree
AS
BEGIN 
	BEGIN TRY
		DECLARE @DueListOneDay OverDueTVP2;
		DECLARE @DueListThreeDay OverDueTVP2;


		-- 到期日前三天
		INSERT INTO @DueListThreeDay (cid, bookid, borrowId, dueDateB)
		SELECT cid, bookid, borrowId, dueDateB
		FROM Borrow
		WHERE FORMAT(GETDATE(), 'yyyy-MM-dd') = FORMAT(DATEADD(DAY, -3, dueDateB), 'yyyy-MM-dd') AND borrowStatusId = 2 AND returnDate IS NULL
		
		-- 寄發通知三天期
		IF EXISTS (SELECT 1 FROM @DueListThreeDay)
		BEGIN
			EXEC NotificationAboutToExpire @DueListThreeDay, 3
			SELECT 1 ResultCode, '發通知三天前的' Message
		END

		-- 到期日等前一天
		INSERT INTO @DueListOneDay (cid, bookid, borrowId, dueDateB)
		SELECT cid, bookid, borrowId, dueDateB
		FROM Borrow
		WHERE FORMAT(GETDATE(), 'yyyy-MM-dd') = FORMAT(DATEADD(DAY, -1, dueDateB), 'yyyy-MM-dd') AND borrowStatusId = 2 AND returnDate IS NULL
		
		-- 寄發通知一天前
		IF EXISTS (SELECT 1 FROM @DueListOneDay)
		BEGIN
			EXEC NotificationAboutToExpire @DueListOneDay, 1
			SELECT 1 ResultCode, '發通知一天前的' Message
		END

		SELECT 1 ResultCode, '即將逾期通已發放!' Message
	END TRY
	BEGIN CATCH
		SELECT 0 ResultCode, '發生錯誤:' + ERROR_MESSAGE() Message
		RETURN
	END CATCH
END
GO

