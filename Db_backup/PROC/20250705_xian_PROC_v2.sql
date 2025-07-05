-- �� PROC
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
        -- �T�w�ӭɾ\�̬O�_�s�b
        IF NOT EXISTS( SELECT 1 FROM Client WHERE cId = @cid)
        BEGIN
            SELECT 1 AS ResultCode, ('�ӭɾ\�̤��s�b�A�Э��s��J!') AS Message;
            ROLLBACK
            RETURN
        END
        
        -- �T�{�ѬO�_�s�b
        IF NOT EXISTS ( SELECT 1 FROM Book WHERE bookCode = @bookCode)
        BEGIN
            ROLLBACK
            SELECT 2 AS ResultCode, ('�ӥ��Ѥ��s�b�A�Э��s��J!') AS Message;
            RETURN
        END

        DECLARE @bookid INT;
		SELECT @bookid = bookid
            FROM Book
            WHERE bookCode = @bookCode
        -- �o���Ѧb�]���� 1.�i�ɾ\
        IF EXISTS ( SELECT 1 FROM Book WHERE bookCode = @bookCode AND bookStatusId = 1)
        BEGIN
			-- ������s���ɥX���A(1)
            UPDATE Book WITH(ROWLOCK, UPDLOCK) SET bookStatusId = 2
            WHERE bookCode = @bookCode AND bookStatusId = 1;
						
			-- �p�G�S����s���\�A�N��Q�w���F....
            IF @@ROWCOUNT = 0
            BEGIN
                SELECT 3 AS ResultCode, '�ӥ��Ѥw�Q��L�H�ɨ�' Message;
                ROLLBACK;
                RETURN;
            END
			-- ���J�ɾ\����
            INSERT INTO Borrow (cId, bookid, borrowDate, dueDateB, borrowStatusId)
            VALUES(@cid, @bookid, GETDATE(), DATEADD(DAY, 14, GETDATE()), 2)

            SELECT 0 AS ResultCode, ('�ɾ\���\') AS Message;
            COMMIT
            RETURN
        END
        -- �p�G���b�����W�w��(2)����
        DECLARE @reservationId INT;
        IF EXISTS (
            SELECT 1 FROM Book WHERE bookCode = @bookCode AND bookStatusId = 2
        )
        BEGIN
            -- �P�_�ӤH�O�_�O ����q�����w���̫e�ӻ�� => ���A: �ݨ��Ѥ�
            SELECT @reservationId = reservationId
            FROM Reservation WITH(UPDLOCK, ROWLOCK)
            WHERE cid = @cid AND reservationStatusId = 3
            AND bookId = @bookId

            IF @reservationId IS NULL
            BEGIN 
                SELECT 2 AS ResultCode, ('���ѥ��w�Q�ɾ\!') AS Message;
                ROLLBACK
            END

            --  �令�w�����A �w�ɥX
            UPDATE Reservation SET reservationStatusId = 1
            WHERE reservationId = @reservationId AND cid = @cid

            IF @@ROWCOUNT = 0
            BEGIN 
                SELECT 3  ResultCode, ('���y���A�w�ܧ�A�L�k�����ɮ�') AS Message;
                ROLLBACK;
                RETURN;
            END

            INSERT INTO Borrow (cId,reservationId, bookId, borrowDate, dueDateB, borrowStatusId)
            VALUES(@cid, @reservationId , @bookId, GETDATE(), DATEADD(DAY, 14, GETDATE()), 2)

            SELECT 0 AS ResultCode, ('�ɾ\���\') AS Message;
            COMMIT;
            RETURN;
        END
        -- 
        SELECT 2 AS ResultCode, ('�ӥ��Ѥw�ɥX�A�Э��s��J') AS Message;
        ROLLBACK
        RETURN
    END TRY
    BEGIN CATCH 
        PRINT '������~'
        ROLLBACK
        SELECT 500 ResultCode, '�o�Ϳ��~' Message
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

        --�����ӥ��ѳ̦��w����
        SELECT @reservationId = reservationId, @cid = cid
        FROM GetEarliestReservation(@collectionId)
		IF (@reservationId IS NULL OR @cid IS NULL)
		BEGIN 
				SELECT 0 ResultCode, '�L���Ĺw������' Message
				RETURN
		END
        -- ��s�w�����A"�i����"�B�T�Ѩ��Ѯɶ�
        UPDATE Reservation WITH(ROWLOCK, UPDLOCK)
        SET bookid = @bookid, reservationStatusId = 3, dueDateR = DATEADD(DAY, 3, GETDATE())
        WHERE reservationId = @reservationId AND cid = @cid
		AND reservationStatusId = 2


        IF @@ROWCOUNT = 0
        BEGIN 
            SELECT 0 ResultCode, '���w���̪��A����.....' MESSAGE
            RETURN
        END
		-- �q��

		INSERT INTO #tmpNotifyResult
        EXEC NotificationBooker @cid, @collectionId

		SELECT @ResultCode = ResultCode, @message =  Message FROM #tmpNotifyResult

		IF @ResultCode = 0
		BEGIN 
				SELECT 0 ResultCode, '�q���w���̥���'+ @message Message
				RETURN
		END
 
        SELECT 1 ResultCode, '�w���\���w���̪��A: �ݨ��Ѥ��C' Message 
		DROP TABLE #tmpNotifyResult
    END TRY
    BEGIN CATCH
        SELECT 0 ResultCode, '�o�Ϳ��~:' + ERROR_MESSAGE() Message
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
			SELECT 0 ResultCode, '�q������: �䤣��w���̩ή��y' Message
			RETURN
		END

        SET @message = N'�˷R�� ' + @cName +
                       N' �A�z�ҹw������ [' + @title + 
                       N'] �w�^���]���A�Щ�3�Ѥ��쥻�]�ɮѡA����!!';

        INSERT INTO Notification (cid, [message], notificationDate) VALUES (@cid,  @message, GETDATE());
		SELECT 1 ResultCode, '�q�����\' Message
    END TRY
    BEGIN CATCH
		SELECT 0 ResultCode, '�q������: ' + ERROR_MESSAGE() Message
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

				-- �T�{�O�_���Q�ɾ\
				IF EXISTS ( SELECT 1 FROM Borrow bor JOIN Book bok ON bor.bookId = bok.bookId WHERE bookCode= @BookCode  AND bookStatusId = 1)
				BEGIN 
					ROLLBACK
					SELECT 0 ResultCode, ('�ѥ��s��:' +@BookCode +' �å��Q�ɾ\�C') Message
					RETURN
				END

				-- �p�G�ѥ��O��
				IF EXISTS ( SELECT 1 FROM Borrow bor JOIN Book bok ON bor.bookId = bok.bookId WHERE bookCode= @BookCode AND bookStatusId = 2 AND borrowStatusId = 3) 
				BEGIN 
					ROLLBACK
					SELECT 0 ResultCode, ('�ѥ��s��:' +@BookCode + ' �O���k�١C' ) Message
					RETURN
				END
				
				-- ��X�ɾ\����
				SELECT TOP 1 @borrwid = borrowid, @collectionId = collectionId, @bookid = bok.bookid
				FROM Borrow bor JOIN Book bok ON bor.bookId  = bok.bookId
				WHERE bookCode = @BookCode AND bor.borrowStatusId = 2 
				ORDER BY borrowDate DESC


				-- �T�{�O�ӥ��ѳQ�ɾ\ => �n�k��
				IF  @borrwid IS NOT NULL 
				BEGIN
					DECLARE @affectedBorrow INT =0, @affectedBook INT = 0;
					-- �p�G�S�w�ɾ\��
						-- ��s�w�k��
					UPDATE Borrow WITH(ROWLOCK, UPDLOCK) SET borrowStatusId = 1, returnDate = GETDATE()
					WHERE borrowId = @borrwid SET @affectedBorrow =@@ROWCOUNT
					
					IF @affectedBorrow = 0 
					BEGIN 
						SELECT 0 ResultCode, '�k�٥���.....' Message
						ROLLBACK
						RETURN
					END

					
					--�p�G����L�ɾ\��
					IF EXISTS ( SELECT 1 FROM Reservation re WHERE collectionId = @collectionId AND reservationStatusId = 2 )
					BEGIN
						EXEC CheckBookIsReservation @collectionId, @bookid
						COMMIT 
						RETURN 
					END

					-- ��s�i�ɾ\
					UPDATE Book WITH(ROWLOCK, UPDLOCK) SET bookStatusId = 1
					WHERE bookCode = @BookCode  SET @affectedBook =@@ROWCOUNT

					IF @affectedBook = 0 
					BEGIN 
						SELECT 0 ResultCode, '�k�٥���.....' Message
						ROLLBACK
						RETURN
					END
					SELECT 1 ResultCode, ('�ѥ��s��:' +@BookCode + ' �k�٦��\!' ) Message
					COMMIT
					RETURN
				END

				-- �ӥ��Ѥ��s�b
				ROLLBACK
				SELECT 0 ResultCode, '�ӥ��Ѥ��s�b' Message
				RETURN
		END TRY
		BEGIN CATCH
				SELECT 0 ResultCode, ('�o�ͥ������~: '+ ERROR_MESSAGE()) Message
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
        -- ���T�{�ϥΪ̬O�_�s�b
        IF NOT EXISTS ( SELECT 1 FROM Client WHERE cid = @cid)
        BEGIN
            SELECT 0 ResultCode, '�ϥΪ̤��s�b' Message 
			RETURN
        END
        -- �T�{�ѥ��O�_�s�b
        IF NOT EXISTS (SELECT 1 FROM Collection WHERE collectionId = @collectionId)
        BEGIN
            SELECT 0 ResultCode, '�ѥ����s�b' Message  
			RETURN
        END 
        -- �p�G�w���w�g�s�b
        IF EXISTS (SELECT 1 FROM Reservation WHERE cid=@cid AND collectionId = @collectionId AND reservationStatusId  = 2 )
        BEGIN 
				SELECT 0 ResultCode, '���ƹw��' Message
				RETURN
		END
        -- �p�G�ӥ��� �b�]�� ok
        IF EXISTS (SELECT 1 FROM Book WITH(HOLDLOCK) WHERE collectionId = @collectionId AND  bookStatusId = 1)
        BEGIN 
                ROLLBACK 
                SELECT 0 ResultCode, '�����ѥثe�b���]' Message
				RETURN
        END 
        -- �p�G�ӥ��Ѧ��w�� ok
        IF EXISTS (SELECT 1 FROM Book WHERE bookStatusId  = 2 AND collectionId = @collectionId)
        BEGIN 
            INSERT INTO Reservation (cId, collectionId, reservationDate,reservationStatusId)
			VALUES (@cid, @collectionid, GETDATE(), 2)

            SELECT 1 ResultCode, '�w�����\' Message 
            COMMIT 
            RETURN 
        END 

        ROLLBACK
		SELECT 0 ResultCode, '�w������' Message
		RETURN
    END TRY
    BEGIN CATCH 
    DECLARE @ErrMsg NVARCHAR(4000), @ErrSeverity INT
    SELECT @ErrMsg = ERROR_MESSAGE(), @ErrSeverity = ERROR_SEVERITY()
    ROLLBACK
    SELECT 0 ResultCode, '�X�{���~: ' + @ErrMsg Message 
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

