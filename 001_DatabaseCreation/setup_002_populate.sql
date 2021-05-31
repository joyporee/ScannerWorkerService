USE [ScannerWSDB]

SET NOCOUNT ON
GO

IF OBJECT_ID('dbo.ScanType') IS NOT NULL
BEGIN
    SELECT  *
    INTO    #scan_type
    FROM    dbo.ScanType
    WHERE   1 = 2

    INSERT	#scan_type
    SELECT	1, 'PROCESSING'		UNION
    SELECT	2, 'PICKUP' 		UNION
	SELECT	3, 'DELIVERY' 

    INSERT  dbo.ScanType 
    SELECT  a.*
    FROM    #scan_type a
    LEFT OUTER JOIN dbo.ScanType b
        ON  b.Id = a.Id
    WHERE   b.Id IS NULL

    DROP TABLE  #scan_type 
END

IF OBJECT_ID('dbo.sy_options') IS NOT NULL
BEGIN
    SELECT  *
    INTO    #sy_options
    FROM    dbo.sy_options
    WHERE   1 = 2

    INSERT	#sy_options
    SELECT	'delay_task_by_sec', '1', 'Delay the scan event by seconds'		UNION
    SELECT	'scan_repeat', '10', 'Number seconds duplicate scanning is considered a repeat.'	UNION
    SELECT	'limit_events_fetching', '100', 'The limit of fetching number of events'			UNION
    SELECT	'max_limit_events_fetching', '2000', 'The maximum limit of fetching number of events'

    INSERT  dbo.sy_options
    SELECT  a.*
    FROM    #sy_options a
    LEFT OUTER JOIN dbo.sy_options b
        ON  b.sy_option = a.sy_option
    WHERE   b.sy_option IS NULL

    DROP TABLE  #sy_options 
END

IF OBJECT_ID('dbo.Device') IS NOT NULL
BEGIN
    SELECT  *
    INTO    #devices
    FROM    dbo.Device
    WHERE   1 = 2

    INSERT	#devices
    SELECT	1, 'Device-001001'		UNION
    SELECT	2, 'Device-001002' 		UNION
	SELECT	3, 'Device-001003'		UNION
	SELECT	4, 'Device-001004'		UNION
	SELECT	5, 'Device-001005'		UNION
	SELECT	6, 'Device-001006'		UNION
	SELECT	7, 'Device-001007'		UNION
	SELECT	8, 'Device-001008'		UNION
	SELECT	9, 'Device-001009'		UNION
	SELECT	10, 'Device-001010'

    INSERT  dbo.Device 
    SELECT  a.*
    FROM    #devices a
    LEFT OUTER JOIN dbo.Device b
        ON  b.Id = a.Id
    WHERE   b.Id IS NULL

    DROP TABLE  #devices 
END

IF OBJECT_ID('dbo.Users') IS NOT NULL
BEGIN
    SELECT  *
    INTO    #usrs
    FROM    dbo.Users
    WHERE   1 = 2

    INSERT	#usrs
    SELECT	'NC1001', 'NC', 100		UNION
    SELECT	'NC1002', 'NC', 200 	UNION
	SELECT	'NC1003', 'NC', 300		UNION
	SELECT	'NC1004', 'NC', 400		UNION
	SELECT	'NC1005', 'NC', 500		UNION
	SELECT	'PH1001', 'PH', 101		UNION
	SELECT	'CP1001', 'CP', 102		UNION
	SELECT	'NW1001', 'NW', 103

    INSERT  dbo.Users 
    SELECT  a.*
    FROM    #usrs a
    LEFT OUTER JOIN dbo.Users b
        ON  b.Id = a.Id
    WHERE   b.Id IS NULL

    DROP TABLE  #usrs 
END

IF OBJECT_ID('dbo.event_tracker') IS NOT NULL
BEGIN
    SELECT  *
    INTO    #event_tracker
    FROM    dbo.event_tracker
    WHERE   1 = 2

    INSERT	#event_tracker
    SELECT	1, 0, GETDATE(), 'Initial system value'

    INSERT  dbo.event_tracker 
    SELECT  a.*
    FROM    #event_tracker a
    LEFT OUTER JOIN dbo.event_tracker b
        ON  b.LastEventId = a.LastEventId
    WHERE   b.LastEventId IS NULL

    DROP TABLE  #event_tracker 
END


PRINT '<< All initial data is populated under the tables >> on ' + DB_Name()

GO

SET NOCOUNT OFF
GO
