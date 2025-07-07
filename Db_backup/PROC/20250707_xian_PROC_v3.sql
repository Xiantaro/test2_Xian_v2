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

				-- ��X�ɾ\����
				SELECT TOP 1 @borrwid = borrowid, @collectionId = collectionId, @bookid = bok.bookid
				FROM Borrow bor JOIN Book bok ON bor.bookId  = bok.bookId
				WHERE bookCode = @BookCode AND bor.borrowStatusId IN ( 2,3) 
				ORDER BY borrowDate DESC

				-- �T�{�O�_���Q�ɾ\
				IF @borrwid IS NULL
				BEGIN 
					ROLLBACK
					SELECT 0 ResultCode, ('�ѥ��s��:' +@BookCode +' �å��Q�ɾ\�C') Message
					RETURN
				END

				-- �p�G�ѥ��O��
				IF EXISTS ( SELECT 1 FROM Borrow bor WHERE bor.borrowId = @borrwid  AND borrowStatusId = 3) 
				BEGIN 
					UPDATE Borrow WITH(ROWLOCK,UPDLOCK) SET returnDate = GETDATE()
					WHERE borrowId = @borrwid 
					SELECT 0 ResultCode, ('�ѥ��s��:' +@BookCode + ' �O���k�١C' ) Message
					COMMIT
					RETURN
				END
				
				


				-- �T�{�O�ӥ��ѳQ�ɾ\ => �n�k��
				IF  @borrwid IS NOT NULL 
				BEGIN
					DECLARE @affectedBorrow INT =0, @affectedBook INT = 0;
					-- �p�G�S�w�ɾ\��
						-- ��s�w�k��
					UPDATE Borrow WITH(ROWLOCK, UPDLOCK) SET borrowStatusId = 1, returnDate = GETDATE()
					WHERE borrowId = @borrwid SET @affectedBorrow = @@ROWCOUNT
					
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

-- ���ѹO���q��
--9.0 �U�� TVP
CREATE TYPE OverDueTVP AS TABLE(
	[reservationId] [int] NULL,
	[cid] [int] NULL,
	[collectionId] [int] NULL,
	[bookid] [int] NULL
)
GO
--9. �ˬd�O�� Main
CREATE PROC OverDue
AS
BEGIN
	BEGIN TRY
		
		DECLARE @OverDueList OverDueTVP
		-- ���J�O����
		INSERT INTO @OverDueList
		SELECT reservationId, cid, collectionId, bookId
		FROM Reservation
		WHERE reservationStatusId = 3 AND dueDateR < GETDATE()
		-- �ˬd�O�_���O����
		IF NOT EXISTS ( SELECT 1 FROM @OverDueList)
		BEGIN
			SELECT 0 ResultCode, '�S���O����!' Message
			RETURN
		END
		BEGIN TRAN
		UPDATE Reservation SET reservationStatusId = 4
		WHERE reservationId IN ( SELECT reservationId FROM @OverDueList);
		--
		IF (@@ROWCOUNT = 0)
		BEGIN 
			ROLLBACK
			SELECT 0 ResultCode, '�L�O�������C' Message
			RETURN
		END
		-- �H�WOK

		-- �}�l�i��

		--1. �q���O���̤w�������Ѫ��A ����OK  �B part2 ok
		EXEC NotificationOverdue @OverDue = @OverDueList

		--2. �ˬd�ӥ��ѬO�_����L�w���̩ΨS����L�w���øӧ�s�ѥ����A OK�B
		-- checkpart2 
		EXEC CheckReservationSchedule @DueBook= @OverDueList

		COMMIT
		SELECT 1 ResultCode, '�C��O���Ʀ�����!' Message;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 ROLLBACK;
		SELECT 0 ResultCode, N'���楢��' + ERROR_MESSAGE() Message;
	END CATCH
END
GO

--9-1. �q���w���̭�
CREATE PROC NotificationOverdue
    @OverDue OverDueTVP READONLY
AS
BEGIN
    BEGIN TRY
		INSERT INTO Notification (cid, message, notificationDate )
		SELECT
				od.cid,
				N'�˷R�� ' + cli.cName +
                       N' �A�z�ҹw���� [' + col.title + 
                       N'] ���� { ' + CONVERT(NVARCHAR, DATEADD(DAY, -1, GETDATE()), 111) +' } �e���ѡA�t�Τw�����A�p���ݭn�Э��s�w���A����!!',
				GETDATE()
		FROM @OverDue od
		JOIN Client cli ON od.cid = cli.cid 
		JOIN Collection col ON col.collectionId = od.collectionId

		SELECT 1 ResultCode, '�Ҧ��O�q���w�o�񦨥\' Message
    END TRY
    BEGIN CATCH
		SELECT 0 ResultCode, '�q������: ' + ERROR_MESSAGE() Message
        RETURN
    END CATCH
END

GO

-- 9-2. �ˬd��L�w���̤ήѥ����A��s
CREATE PROC [dbo].[CheckReservationSchedule]
		@DueBook OverDueTVP READONLY
AS 
BEGIN
    BEGIN TRY
		-- �ΨӦs��-���y����L�w���̪�TVP
		DECLARE @reservationList OverDueTVP;
		-- �ΨӦs��-���y�S���w���̪�TVP
		DECLARE @bookList  OverDueTVP;

		-- 1.���o�S���w������
		INSERT INTO @bookList (collectionId, bookid)
		SELECT  collectionId, bookid 
		FROM @DueBook bok 
		WHERE NOT EXISTS 
		( SELECT 1 FROM Reservation re WHERE re.collectionId = bok.collectionId   AND reservationStatusId IN ( 2, 3)  )

		-- 1.1 ��s�ѥ����A�� 1.�i�ɾ\
		IF EXISTS (SELECT 1 FROM @bookList)
		BEGIN
					UPDATE  Book SET bookStatusId = 1
					FROM Book bok JOIN @bookList boklist ON bok.bookid = boklist.bookid AND bok.collectionId = boklist.collectionId
					SELECT '��s�ѥ����A�i�ɾ\' message
		END

        --2.���o���y�̦��w�����H ok
		INSERT INTO @reservationList (reservationId, cid, bookid,collectionid )
        SELECT reservationId,  cid,bookid,collectionid 
        FROM GetEarliestReservationTVF(@DueBook)
		
        -- 2.2��s�w�����A"�i����"�B�T�Ѩ��Ѯɶ� ok
        UPDATE Reservation WITH(ROWLOCK, UPDLOCK)
        SET bookid = rlist.bookid, reservationStatusId = 3, dueDateR = DATEADD(DAY, 3, GETDATE())
        FROM Reservation re JOIN @reservationList rlist ON re.reservationId = rlist.reservationId
		SELECT * FROM @reservationList
		--2.3�q���w���� ok
		IF EXISTS ( SELECT 1 FROM @reservationList)
		BEGIN
				EXEC NotificationBookerTVPvserion  @reservationer = @reservationList
				SELECT '�w�q���U�@�ӹw����' message
		END

        SELECT 1 ResultCode, '�w���\�i��q���w���̤ήѥ����A' Message 

    END TRY
    BEGIN CATCH
        SELECT 0 ResultCode, '�o�Ϳ��~:' + ERROR_MESSAGE() Message
        RETURN
    END CATCH
END 
GO

-- 9-2-1.���o�ӥ��ѹw���̦��w�����X�ιw����ID => VTF 
CREATE FUNCTION [dbo].[GetEarliestReservationTVF]
(
	@InputTable OverDueTVP READONLY
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

--9-3. �q���w���̨���(TVP��)
CREATE PROC NotificationBookerTVPvserion
    @reservationer OverDueTVP READONLY
AS
BEGIN
    BEGIN TRY
			INSERT INTO Notification (cid, [message], notificationDate)
			SELECT  
				cli.cid,
				N'�˷R�� ' + cli.cName +
                       N' �A�z�ҹw������ [' + col.title + 
                       N'] �w�i�H�ɾ\�A�Щ�3�Ѥ��쥻�]�ɮѡA����!!',
					   GETDATE()
			FROM @reservationer re 
			JOIN Client cli ON re.cid = cli.cid
			JOIN Collection col ON re.collectionId = col.collectionId
		
		SELECT COUNT(*) NotificationCount, '�q���w���̨���(TVP��)���\' Message
    END TRY
    BEGIN CATCH
		SELECT 0 ResultCode, '�q������: ' + ERROR_MESSAGE() Message
        RETURN
    END CATCH
END
GO
----
-- �ɾ\�O��
--10.0 
CREATE TYPE OverDueTVP2 AS TABLE(
	[reservationId] [int] NULL,
	[borrowId] [int] NULL,
	[cid] [int] NULL,
	[collectionId] [int] NULL,
	[bookid] [int] NULL,
	[dueDateB] [datetime2](7) NULL
)
GO
--10. Main Proc
CREATE PROC [dbo].[LateReturn]
AS
BEGIN
		BEGIN TRY
			BEGIN TRANSACTION
			DECLARE @DueList OverDueTVP2;
			
			INSERT INTO @DueList ( cid, bookid,borrowId, dueDateB)
			SELECT  cid, bookid,borrowId, dueDateB
			FROM Borrow
			WHERE dueDateB < GETDATE() AND borrowStatusId = 2
			IF NOT EXISTS ( SELECT 1 FROM @DueList)
			BEGIN
				SELECT 0 ResultCode, '�L�O����' message
				ROLLBACK
				RETURN 
			END

			UPDATE Borrow WITH(ROWLOCK,UPDLOCK) SET borrowStatusId = 3
			FROM Borrow bow
			JOIN @DueList due ON bow.borrowId = due.borrowId AND bow.cid = due.cid

			SELECT * FROM @DueList
			
			IF @@ROWCOUNT = 0
			BEGIN 
				SELECT 0 ResultCode, '�L��s�O��' message
				ROLLBACK
				RETURN 
			END
			--�q��
			EXEC NotificationOverdueBorrow @OverDue = @DueList
			COMMIT
			SELECT 1 ResultCode, '�ˬd�O���̵���,,,' message
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0 
			ROLLBACK
			SELECT 0 ResultCode, '�X�{���~' + ERROR_MESSAGE() message
		END CATCH
END
GO

-- 10.1
CREATE PROC NotificationOverdueBorrow
    @OverDue OverDueTVP2 READONLY
AS
BEGIN
    BEGIN TRY
		INSERT INTO Notification (cid, message, notificationDate )
		SELECT
				od.cid,
				N'�˷R�� ' + cli.cName +
                       N' �A�z�ҭɾ\�� [' + col.title + 
                       N'] ���� { ' + CONVERT(NVARCHAR, od.dueDateB, 111) +' } �e�ٮѡA�кɳt�ٮѡA�üW�[�H�W�I��1�I!!',
				GETDATE()
		FROM @OverDue od
		JOIN Client cli ON od.cid = cli.cid 
		JOIN Book bok ON od.bookid = bok.bookId
		JOIN Collection col ON bok.collectionId = col.collectionId

		SELECT 1 ResultCode, '�Ҧ��O�q���w�o�񦨥\' Message
    END TRY
    BEGIN CATCH
		SELECT 0 ResultCode, '�q������: ' + ERROR_MESSAGE() Message
        RETURN
    END CATCH
END
GO

