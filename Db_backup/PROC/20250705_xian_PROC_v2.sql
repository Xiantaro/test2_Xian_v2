-- 憲 PROC
-- 1
CREATE PROC BookInfomationForBorrow
@bookCode NVARCHAR(500)
AS
BEGIN
		SELECT bok.bookCode, 
			col.title, col.author, 
			col.translator, 
			typ.[type], 
			col.publisher, 
			col.publishDate, 
			col.collectionImg, 
			bokstu.bookStatus
		FROM Book bok JOIN Collection col ON bok.collectionId = col.collectionId
		JOIN BookStatus bokstu ON bok.bookStatusId = bokstu.bookStatusId
		JOIN [Type] typ ON col.typeId = typ.typeId
		WHERE bok.bookCode = @bookCode
END
GO

-- 2
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
    LEFT JOIN Reservation re ON col.collectionId = re.collectionId
    WHERE (col.title LIKE '%'+ @keyword + '%' OR col.author LIKE '%' + @keyword + '%') 
    AND (
            @bookstatus = 'ALL' AND bok.bookStatusId IN (1,2, 3) OR
            (@bookstatus = 'IsLent' AND bok.bookStatusId IN (2)) OR
            (@bookstatus = 'Available' AND bok.bookStatusId = 1)
    )
    GROUP BY  col.collectionId, title, author, bokstu.bookStatus
END;
GO

--3
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
            SELECT 2 AS ResultCode, ('該本書不存在，請重新輸入!') AS Message;
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

--4
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

--5
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

        SET @message = N'親愛的 ' + @cName +
                       N' ，您所預約的書 [' + @title + 
                       N'] 已回到館內，請於3天內到本館借書，謝謝!!';

        INSERT INTO Notification (cid, [message], notificationDate) VALUES (@cid,  @message, GETDATE());
		SELECT 1 ResultCode, '通知成功' Message
    END TRY
    BEGIN CATCH
		SELECT 0 ResultCode, '通知失敗: ' + ERROR_MESSAGE() Message
        RETURN
    END CATCH
END
GO

--6
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

				-- 確認是否有被借閱
				IF EXISTS ( SELECT 1 FROM Borrow bor JOIN Book bok ON bor.bookId = bok.bookId WHERE bookCode= @BookCode  AND bookStatusId = 1)
				BEGIN 
					ROLLBACK
					SELECT 0 ResultCode, ('書本編號:' +@BookCode +' 並未被借閱。') Message
					RETURN
				END

				-- 如果書本逾期
				IF EXISTS ( SELECT 1 FROM Borrow bor JOIN Book bok ON bor.bookId = bok.bookId WHERE bookCode= @BookCode AND bookStatusId = 2 AND borrowStatusId = 3) 
				BEGIN 
					ROLLBACK
					SELECT 0 ResultCode, ('書本編號:' +@BookCode + ' 逾期歸還。' ) Message
					RETURN
				END
				
				-- 找出借閱紀錄
				SELECT TOP 1 @borrwid = borrowid, @collectionId = collectionId, @bookid = bok.bookid
				FROM Borrow bor JOIN Book bok ON bor.bookId  = bok.bookId
				WHERE bookCode = @BookCode AND bor.borrowStatusId = 2 
				ORDER BY borrowDate DESC


				-- 確認是該本書被借閱 => 要歸還
				IF  @borrwid IS NOT NULL 
				BEGIN
					DECLARE @affectedBorrow INT =0, @affectedBook INT = 0;
					-- 如果沒已借閱者
						-- 更新已歸還
					UPDATE Borrow WITH(ROWLOCK, UPDLOCK) SET borrowStatusId = 1, returnDate = GETDATE()
					WHERE borrowId = @borrwid SET @affectedBorrow =@@ROWCOUNT
					
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

--7
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

-- 8
CREATE FUNCTION [dbo].[GetEarliestReservation](@collectionId INT)
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

