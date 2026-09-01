SET XACT_ABORT ON;

BEGIN TRANSACTION;

IF COL_LENGTH('dbo.Makineler', 'sira_no') IS NULL
BEGIN
    ALTER TABLE dbo.Makineler
    ADD sira_no INT NULL;
END

GO

UPDATE dbo.Makineler
SET sira_no = CASE makine_no
    WHEN N'19' THEN 1
    WHEN N'20' THEN 2
    WHEN N'21' THEN 3
    WHEN N'22' THEN 4
    WHEN N'23' THEN 5
    WHEN N'24' THEN 6
    WHEN N'25' THEN 7
    WHEN N'26' THEN 8
    WHEN N'27' THEN 9
    WHEN N'18' THEN 10
    WHEN N'28' THEN 11
    WHEN N'29' THEN 12
    WHEN N'30' THEN 13
    WHEN N'31' THEN 14
    WHEN N'32' THEN 15
    WHEN N'33' THEN 16
    WHEN N'34' THEN 17
    WHEN N'35' THEN 18
    WHEN N'36' THEN 19
    WHEN N'17' THEN 20
    ELSE sira_no
END;

IF EXISTS
(
    SELECT 1
    FROM dbo.Makineler
    WHERE sira_no IS NULL
)
BEGIN
    THROW 50001, 'Sıra numarası atanmamış makine bulundu.', 1;
END

ALTER TABLE dbo.Makineler
ALTER COLUMN sira_no INT NOT NULL;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.Makineler')
      AND name = 'UX_Makineler_SiraNo'
)
BEGIN
    CREATE UNIQUE INDEX UX_Makineler_SiraNo
        ON dbo.Makineler(sira_no);
END

COMMIT TRANSACTION;
