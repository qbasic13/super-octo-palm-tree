/* Application critical records */

INSERT INTO [user_types] 
(
	[ut_id], 
	[ut_name]
) 
SELECT 
	* 
FROM 
(
	VALUES 
	('1', 'admin'), 
	('2', 'unverified'),
	('3', 'verified')
) 
AS [new]
(
	[ut_id],
 	[ut_name]
)
WHERE NOT EXISTS 
(
	SELECT
		*
	FROM 
		[user_types]
	WHERE [new].[ut_id] = [user_types].[ut_id]
		AND [new].[ut_name] = [user_types].[ut_name]
);


INSERT INTO [genres] 
(
	[g_name]
)
SELECT 
	[g_name] 
FROM 
(
	VALUES 
	('1','fantasy'), 
	('2','sci_fi'), 
	('3','mystery'), 
	('4','adventure'), 
	('5','triller'), 
	('6','romance'), 
	('7','dystopian'), 
	('8','contemporary'), 
	('9','computer_science'), 
	('10','memoir'), 
	('11','cookbook'), 
	('12','art'), 
	('13','personal_development'), 
	('14','health'), 
	('15','humor'), 
	('16','novel'),
	('17','psychological'),
	('18','literary_fiction'), 
	('19','poem')
) 
AS [new]
(
	[g_id], 
 	[g_name]
)
WHERE NOT EXISTS 
(
	SELECT
		*
	FROM 
		[genres]
	WHERE [new].[g_id] = [genres].[g_id]
	AND [new].[g_name] = [genres].[g_name]
);


INSERT INTO [order_statuses] 
(
	[os_id], 
	[os_name]
)
SELECT 
	* 
FROM 
(
	VALUES 
	('1', 'created'), 
	('2', 'being_delivered'), 
	('3', 'completed')
) 
AS [new] 
(
	[os_id], 
 	[os_name]
)
WHERE NOT EXISTS 
(
	SELECT
		*
	FROM 
		[order_statuses]
	WHERE [new].[os_id] = [order_statuses].[os_id]
		AND [new].[os_name] = [order_statuses].[os_name]
);

/* Sample data */

INSERT INTO [users] 
(
	[u_email], 
	[u_first_name], 
	[u_last_name], 
	[u_middle_name], 
	[u_password], 
	[u_phone], 
	[u_profile_file],
	[u_register_dt]
)
SELECT 
	[u_email], 
	[u_first_name], 
	[u_last_name], 
	[u_middle_name], 
	[u_password], 
	[u_phone], 
	[u_profile_file],
	[u_register_dt] 
FROM 
(
	VALUES 
	(
		'1',
		'admin@example.com', 
		'John', 
		'Smith', 
		null, /* testingPlatform01 using SHA3-512 hash */ 
		'0c00f61257a6ed0a9dff1d7d803c346472fe86ae7b6faef4b973b9678529d62c247edf2989ccb479226a7b862fb889cc0926475aa028c9d88ae5bd5673ceceff', 
		'+375000000000', 
		null, 
		CAST(N'2022-06-08 08:00:00' AS smalldatetime)
	), 
	(
		'2',
		'verified@example.com', 
		'Jane', 
		'Smith', 
		null, /* testingPlatform01 using SHA3-512 hash */ 
		'0c00f61257a6ed0a9dff1d7d803c346472fe86ae7b6faef4b973b9678529d62c247edf2989ccb479226a7b862fb889cc0926475aa028c9d88ae5bd5673ceceff', 
		'+3750170000000', 
		null, 
		CAST(N'2022-06-10 09:00:00' AS smalldatetime)
	), 
	(
		'3',
		'unverified@example.com', 
		'Jack', 
		'Smith', 
		null, /* testingPlatform01 using SHA3-512 hash */ 
		'0c00f61257a6ed0a9dff1d7d803c346472fe86ae7b6faef4b973b9678529d62c247edf2989ccb479226a7b862fb889cc0926475aa028c9d88ae5bd5673ceceff', 
		'+37501711111111', 
		null, 
		CAST(N'2022-06-10 09:00:00' AS smalldatetime)
	)
) 
AS [new]
(
	[u_id],
	[u_email], 
	[u_first_name], 
	[u_last_name], 
	[u_middle_name], 
	[u_password], 
	[u_phone], 
	[u_profile_file],
	[u_register_dt]
)
WHERE NOT EXISTS 
(
	SELECT
		*
	FROM 
		[users]
	WHERE [new].[u_id] = [users].[u_id]
		AND [new].[u_email] = [users].[u_email]
);


INSERT INTO [m2m_users_user_types] 
(
	[m2muut_u_id], 
	[m2muut_ut_id]
)
SELECT 
	* 
FROM 
(
	VALUES 
	(
		(
			SELECT 
				[u_id] 
			FROM 
				[users]
			WHERE [u_email] = 'admin@example.com'
		), 
		(
			SELECT 
				[ut_id] 
			FROM 
				[user_types] 
			WHERE [ut_name] = 'admin'
		)
	), 
	(
		(
			SELECT 
				[u_id] 
			FROM 
				[users] 
			WHERE [u_email] = 'verified@example.com'
		),
		(
			SELECT 
				[ut_id] 
			FROM 
				[user_types] 
			WHERE [ut_name] = 'verified'
		)
	), 
	(
		(
			SELECT 
				[u_id] 
			FROM 
				[users] 
			WHERE [u_email] = 'unverified@example.com'
		), 
		(
			SELECT 
				[ut_id] 
			FROM 
				[user_types] 
			WHERE [ut_name] = 'unverified'
		)
	)
) 
AS [new] 
(
	[m2muut_u_id], 
	[m2muut_ut_id]
)
WHERE NOT EXISTS 
(
	SELECT
		*
	FROM 
		[m2m_users_user_types]
	WHERE [new].[m2muut_u_id] = [m2m_users_user_types].[m2muut_u_id]
		AND [new].[m2muut_ut_id] = [m2m_users_user_types].[m2muut_ut_id]
);


INSERT INTO [books] 
(
	[b_isbn], 
	[b_title], 
	[b_genre], 
	[b_author], 
	[b_publish_year],
	[b_quantity], 
	[b_price], 
	[b_cover_file]
) 
SELECT 
	* 
FROM 
(
	VALUES 
	(
		'9780679783268', 
		'Pride and Prejudice', 
		(
			SELECT 
				[g_id] 
			FROM 
				[genres] 
			WHERE [g_name] = 'novel'
		), 
		'Jane Austen', 
		'2000', 
		'8', 
		'0.99',
		'9780679783268.jpg'
	), 
	(
		'9781782124207', 
		'1984', 
		(
			SELECT 
				[g_id] 
			FROM 
				[genres] 
			WHERE [g_name] = 'dystopian'
		), 
		'George Orwell', 
		'2013', 
		'16', 
		'9.99', 
		'9781782124207.jpg'
	), 
	(
		'9781405882620', 
		'Crime and Punishment', 
		(
			SELECT 
				[g_id] 
			FROM 
				[genres] 
			WHERE [g_name] = 'psychological'
		), 'Fyodor Dostoyevsky', 
		'2008', 
		'4', 
		'14.99', 
		'9781405882620.jpg' 
	), 
	(
		'9781451673319', 
		'Fahrenheit 451', 
		(
			SELECT 
				[g_id]
			FROM 
				[genres]
			WHERE [g_name] = 'dystopian'
		), 
		'Ray Bradbury', 
		'2012', 
		'0', 
		'12.99', 
		'9781451673319.jpg' 
	), 
	(
		'9780198800538', 
		'Anna Karenina', 
		(
			SELECT 
				[g_id] 
			FROM 
				[genres] 
			WHERE [g_name] = 'romance'
		), 
		'Leo Tolstoy', 
		'2017', 
		'6', 
		'29.99', 
		'9780198800538.jpg' 
	), 
	(
		'9780140449242', 
		'The Brothers Karamazov', 
		(
			SELECT 
				[g_id] 
			FROM 
				[genres] 
			WHERE [g_name] = 'mystery'
		), 
		'Fyodor Dostoyevsky', 
		'2003', 
		'2', 
		'24.99', 
		'9780140449242.jpg' 
	), 
	(
		'9780679727293', 
		'Lolita', 
		(
			SELECT 
				[g_id] 
			FROM 
				[genres] 
			WHERE [g_name] = 'novel'
		), 
		'Vladimir Nabokov', 
		'1991', 
		'3', 
		'14.99', 
		'9780679727293.jpg'
	), 
	(
		'9781476787848', 
		'The Old Man and the Sea', 
		(
			SELECT 
				[g_id] 
			FROM 
				[genres] 
			WHERE [g_name] = 'literary_fiction'
		), 
		'Ernest Hemingway', 
		'2020', 
		'5', 
		'29.99', 
		'9781476787848.jpg'
	), 
	(
		'9780199535644', 
		'The Divine Comedy', 
		(
			SELECT 
				[g_id] 
			FROM 
				[genres] 
			WHERE [g_name] = 'poem'
		), 
		'Dante Alighieri', 
		'2008', 
		'13', 
		'19.99', 
		'9780199535644.jpg'
	), 
	(
		'9780241265543', 
		'War And Peace', 
		(
			SELECT 
				[g_id] 
			FROM 
				[genres] 
			WHERE [g_name] = 'novel'
		), 
		'Leo Tolstoy', 
		'2017', 
		'7', 
		'44.99', 
		'9780241265543.jpg'
	)
) 
AS [new] 
(
	[b_isbn], 
	[b_title], 
	[b_genre], 
	[b_author], 
	[b_publish_year],
	[b_quantity], 
	[b_price], 
	[b_cover_file]
)
WHERE NOT EXISTS 
(
	SELECT
		*
	FROM 
		[books]
	WHERE [new].[b_isbn] = [books].[b_isbn]
);


INSERT INTO [orders] 
(
	[o_status], 
	[o_creator], 
	[o_total_price], 
	[o_creation_dt], 
	[o_completion_dt]
) 
SELECT 
	[o_status], 
	[o_creator], 
	[o_total_price], 
	[o_creation_dt], 
	[o_completion_dt] 
FROM 
(
	VALUES 
	(
		'1', 
		(
			SELECT 
				[os_id] 
			FROM 
				[order_statuses] 
			WHERE [os_name] = 'created'
		), 
		(
			SELECT 
				[u_id] 
			FROM 
				[users] 
			WHERE [u_email] = 'verified@example.com'
		), 
		'9.99', 
		CAST(N'2022-05-15 09:00:00' AS smalldatetime),
		null
	), 
	(
		'2', 
		(
			SELECT 
				[os_id] 
			FROM 
				[order_statuses] 
			WHERE [os_name] = 'being_delivered'
		),
		(
			SELECT 
				[u_id] 
			FROM 
				[users] 
			WHERE [u_email] = 'admin@example.com'
		),
		'39.98', 
		CAST(N'2022-06-05 05:13:02' AS smalldatetime), 
		null
	), 
	(
		'3', 
		(
			SELECT 
				[os_id] 
			FROM 
				[order_statuses] 
			WHERE [os_name] = 'completed'
		),
		(
			SELECT 
				[u_id] 
			FROM 
				[users] 
			WHERE [u_email] = 'verified@example.com'
		), 
		'69.98', 
		CAST(N'2022-06-08 15:57:00' AS smalldatetime), 
		CAST(N'2022-06-10 17:24:10' AS smalldatetime)
	)
) 
AS [new]
(
	[o_id], 
	[o_status], 
	[o_creator], 
	[o_total_price], 
	[o_creation_dt], 
	[o_completion_dt]
)
WHERE NOT EXISTS 
(
	SELECT
		*
	FROM 
		[orders]
	WHERE [new].[o_id] = [orders].[o_id]
);


INSERT INTO [m2m_orders_books] 
(
	[m2mob_o_id], 
	[m2mob_b_isbn], 
	[m2mob_price]
) 
SELECT 
	* 
FROM 
(
	VALUES 
	(
		(
			SELECT 
				[o_id] 
			FROM 
				[orders]
			JOIN [order_statuses]
				ON [orders].o_status = [order_statuses].[os_id] 
			WHERE [os_name] = 'created'
		), 
		'9781782124207', 
		'9.99'
	), 
	(
		(
			SELECT 
				[o_id] 
			FROM 
				[orders] 
			JOIN [order_statuses] 
				ON [orders].[o_status] = [order_statuses].[os_id] 
			WHERE [os_name] = 'being_delivered'
		), 
		'9781782124207', 
		'9.99'
	), 
	(
		(
			SELECT 
				[o_id] 
			FROM 
				[orders] 
			JOIN [order_statuses] 
				ON [orders].[o_status] = [order_statuses].[os_id] 
			WHERE [os_name] = 'being_delivered' 
		), 
		'9780198800538', 
		'29.99'
	), 
	(
		(
			SELECT 
				[o_id] 
			FROM 
				[orders] 
			JOIN [order_statuses] 
				ON [orders].[o_status] = [order_statuses].[os_id] 
			WHERE [os_name] = 'completed'
		), 
		'9780199535644', 
		'19.99'
	), 
	(
		(
			SELECT 
				[o_id] 
			FROM 
				[orders] 
			JOIN [order_statuses] 
				ON [orders].[o_status] = [order_statuses].[os_id] 
			WHERE [os_name] = 'completed'
		), 
		'9780241265543', 
		'39.99'
	),
	(
		(
			SELECT 
				[o_id] 
			FROM 
				[orders] 
			JOIN [order_statuses] 
				ON [orders].[o_status] = [order_statuses].[os_id] 
			WHERE [os_name] = 'completed'
		), 
		'9781405882620', 
		'10.00'
	)
) 
AS [new]
(
	[m2mob_o_id], 
	[m2mob_b_isbn], 
	[m2mob_price]
)
WHERE NOT EXISTS 
(
	SELECT
		*
	FROM 
		[m2m_orders_books]
	WHERE [new].[m2mob_o_id] = [m2m_orders_books].[m2mob_o_id] 
		AND [new].[m2mob_b_isbn] = [m2m_orders_books].[m2mob_b_isbn]
);