
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Users_active]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Users] DROP CONSTRAINT [DF_Users_active]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Products_regdate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Products] DROP CONSTRAINT [DF_Products_regdate]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Products_likes]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Products] DROP CONSTRAINT [DF_Products_likes]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Products_quantity]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Products] DROP CONSTRAINT [DF_Products_quantity]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_LogUsersProducts_regdate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[LogUsersProducts] DROP CONSTRAINT [DF_LogUsersProducts_regdate]
END

GO
/****** Object:  Table [dbo].[Users]    Script Date: 07-May-19 10:02:13 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
DROP TABLE [dbo].[Users]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 07-May-19 10:02:13 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
DROP TABLE [dbo].[Products]
GO
/****** Object:  Table [dbo].[LogUsersProducts]    Script Date: 07-May-19 10:02:13 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LogUsersProducts]') AND type in (N'U'))
DROP TABLE [dbo].[LogUsersProducts]
GO
/****** Object:  StoredProcedure [dbo].[UPDATE_PRODUCT]    Script Date: 07-May-19 10:02:13 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UPDATE_PRODUCT]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UPDATE_PRODUCT]
GO
/****** Object:  StoredProcedure [dbo].[SET_PRODUCTS]    Script Date: 07-May-19 10:02:13 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SET_PRODUCTS]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SET_PRODUCTS]
GO
/****** Object:  StoredProcedure [dbo].[GET_PRODUCTS]    Script Date: 07-May-19 10:02:13 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GET_PRODUCTS]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GET_PRODUCTS]
GO
/****** Object:  StoredProcedure [dbo].[DELETE_PRODUCT]    Script Date: 07-May-19 10:02:13 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DELETE_PRODUCT]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DELETE_PRODUCT]
GO
/****** Object:  StoredProcedure [dbo].[CHECK_USER]    Script Date: 07-May-19 10:02:13 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CHECK_USER]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CHECK_USER]
GO
/****** Object:  StoredProcedure [dbo].[BUY_A_PRODUCT]    Script Date: 07-May-19 10:02:13 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BUY_A_PRODUCT]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BUY_A_PRODUCT]
GO
/****** Object:  StoredProcedure [dbo].[BUY_A_PRODUCT]    Script Date: 07-May-19 10:02:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BUY_A_PRODUCT]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[BUY_A_PRODUCT](@ProductId NUMERIC(18,0),
@UserdId VARCHAR(50),
@Quantity INT,
@Flag INT OUTPUT)
AS
	--user doesn''t exists
	SET @Flag = -1
	
	IF (SELECT COUNT(*) FROM [dbo].[Users] WHERE [user_name] = @UserdId) > 0
	BEGIN
		IF (SELECT COUNT(*) FROM Products WHERE product_id = @ProductId) > 0
		BEGIN
			IF (SELECT quantity FROM Products WHERE product_id = @ProductId) >= @Quantity
			BEGIN
				UPDATE [dbo].[Products]
				SET quantity = quantity - @Quantity
				WHERE product_id = @ProductId

				INSERT INTO [dbo].[LogUsersProducts]([user_name], product_id, quantity_purchased)
				VALUES(@UserdId, @ProductId, @Quantity)

				SET @Flag = @@IDENTITY
			END
			ELSE
			BEGIN
				--there is less product available to buy that the desired quantity
				SET @Flag = -2
			END
		END
		ELSE
		BEGIN
			--product doesn''t exist
			SET @Flag = -3
		END
	END
	

RETURN
' 
END
GO
/****** Object:  StoredProcedure [dbo].[CHECK_USER]    Script Date: 07-May-19 10:02:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CHECK_USER]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[CHECK_USER](@Name VARCHAR(50),
@Password VARCHAR(50),
@UserLevel INT,
@Exists INT OUTPUT)
AS
	SET @Exists = 0

	IF (SELECT COUNT(*) FROM [dbo].[Users]
		WHERE [user_name] = @Name AND [password] = @Password
		AND active = 1 AND user_level = @UserLevel)> 0
	BEGIN
		SET @Exists = 1
	END

RETURN' 
END
GO
/****** Object:  StoredProcedure [dbo].[DELETE_PRODUCT]    Script Date: 07-May-19 10:02:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DELETE_PRODUCT]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[DELETE_PRODUCT](@Id NUMERIC(18,0))
AS
	DELETE FROM [dbo].[Products]
	WHERE product_id = @Id

RETURN' 
END
GO
/****** Object:  StoredProcedure [dbo].[GET_PRODUCTS]    Script Date: 07-May-19 10:02:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GET_PRODUCTS]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[GET_PRODUCTS](@ById INT,
@PageIndex INT,
@PageSize INT,
@PagingEnabled INT,
@WordToSerach VARCHAR(50))
AS
	
	IF @ById > 0
	BEGIN
		SELECT * 
		FROM [dbo].[Products]
		WHERE product_id = @ById
	END
	ELSE
	BEGIN
		IF @WordToSerach <> ''''
		BEGIN
			IF @PagingEnabled = 1
			BEGIN
				WITH pagination AS
				(
					SELECT product_id
					FROM [dbo].[Products]
					ORDER BY product_id
					OFFSET @PageSize * ( @PageIndex - 1 ) ROWS
					FETCH NEXT @PageSize ROWS ONLY
				)

				SELECT products.* 
				FROM products
				INNER JOIN pagination
				ON products.product_id = pagination.product_id
			END
			ELSE
			BEGIN
				SELECT * 
				FROM [dbo].[Products]
			END
		END
		ELSE
		BEGIN
			IF @PagingEnabled = 1
			BEGIN
				WITH pagination AS
				(
					SELECT product_id
					FROM [dbo].[Products]
					WHERE name LIKE ''%''+ @WordToSerach + ''%''
					ORDER BY product_id
					OFFSET @PageSize * ( @PageIndex - 1 ) ROWS
					FETCH NEXT @PageSize ROWS ONLY
				)

				SELECT products.* 
				FROM products
				INNER JOIN pagination
				ON products.product_id = pagination.product_id

			END
			ELSE
			BEGIN
				SELECT * 
				FROM [dbo].[Products]
				WHERE name LIKE ''%''+ @WordToSerach + ''%''
			END
		END

	END

RETURN' 
END
GO
/****** Object:  StoredProcedure [dbo].[SET_PRODUCTS]    Script Date: 07-May-19 10:02:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SET_PRODUCTS]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SET_PRODUCTS](@Name VARCHAR(50),
@Quantity INT,
@Price NUMERIC(18,2),
@Id NUMERIC(18,0) OUTPUT)
AS

	INSERT INTO [dbo].[Products](name, quantity, price)
	VALUES(@Name, @Quantity, @Price)

	SET @Id = @@IDENTITY

RETURN' 
END
GO
/****** Object:  StoredProcedure [dbo].[UPDATE_PRODUCT]    Script Date: 07-May-19 10:02:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UPDATE_PRODUCT]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[UPDATE_PRODUCT](@Name VARCHAR(50),
@Quantity INT,
@Price NUMERIC(18,2),
@Id NUMERIC(18,0),
@UpdateType INT = 1)
AS
	--update type
	IF @UpdateType = 1
	BEGIN
		UPDATE [dbo].[Products]
		SET name = @Name,
		quantity = @Quantity,
		price = @Price,
		last_update = GETDATE()
		WHERE product_id = @Id
	END
	ELSE IF @UpdateType = 2
	BEGIN
		UPDATE [dbo].[Products]
		SET price = @Price,
		last_update = GETDATE()
		WHERE product_id = @Id
	END
	ELSE IF @UpdateType = 3
	BEGIN
		UPDATE [dbo].[Products]
		SET likes = likes + 1,
		last_update = GETDATE()
		WHERE product_id = @Id
	END

RETURN' 
END
GO
/****** Object:  Table [dbo].[LogUsersProducts]    Script Date: 07-May-19 10:02:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LogUsersProducts]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LogUsersProducts](
	[id] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[user_name] [varchar](50) NULL,
	[product_id] [numeric](18, 0) NULL,
	[quantity_purchased] [int] NULL,
	[regdate] [datetime] NULL,
 CONSTRAINT [PK_LogUsersProducts] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Products]    Script Date: 07-May-19 10:02:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Products](
	[product_id] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NULL,
	[quantity] [int] NULL,
	[likes] [int] NULL,
	[price] [numeric](18, 2) NULL,
	[regdate] [datetime] NULL,
	[last_update] [datetime] NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[product_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users]    Script Date: 07-May-19 10:02:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Users](
	[user_name] [varchar](50) NOT NULL,
	[password] [varchar](50) NULL,
	[active] [int] NULL,
	[user_level] [int] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[user_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[LogUsersProducts] ON 

GO
INSERT [dbo].[LogUsersProducts] ([id], [user_name], [product_id], [quantity_purchased], [regdate]) VALUES (CAST(1 AS Numeric(18, 0)), N'admin', CAST(1 AS Numeric(18, 0)), 1, CAST(0x0000AA4500A7A59A AS DateTime))
GO
INSERT [dbo].[LogUsersProducts] ([id], [user_name], [product_id], [quantity_purchased], [regdate]) VALUES (CAST(2 AS Numeric(18, 0)), N'admin', CAST(1 AS Numeric(18, 0)), 1, CAST(0x0000AA4500A7F4AD AS DateTime))
GO
INSERT [dbo].[LogUsersProducts] ([id], [user_name], [product_id], [quantity_purchased], [regdate]) VALUES (CAST(3 AS Numeric(18, 0)), N'user', CAST(1 AS Numeric(18, 0)), 1, CAST(0x0000AA4500A956B5 AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[LogUsersProducts] OFF
GO
SET IDENTITY_INSERT [dbo].[Products] ON 

GO
INSERT [dbo].[Products] ([product_id], [name], [quantity], [likes], [price], [regdate], [last_update]) VALUES (CAST(1 AS Numeric(18, 0)), N'Shoes', 7, 0, CAST(2.50 AS Numeric(18, 2)), CAST(0x0000AA3E00BA812E AS DateTime), NULL)
GO
INSERT [dbo].[Products] ([product_id], [name], [quantity], [likes], [price], [regdate], [last_update]) VALUES (CAST(2 AS Numeric(18, 0)), N'Apples', 11, 0, CAST(2.50 AS Numeric(18, 2)), CAST(0x0000AA3E00BA8AEB AS DateTime), NULL)
GO
INSERT [dbo].[Products] ([product_id], [name], [quantity], [likes], [price], [regdate], [last_update]) VALUES (CAST(3 AS Numeric(18, 0)), N'Computers', 12, 0, CAST(2.50 AS Numeric(18, 2)), CAST(0x0000AA3E00BA9591 AS DateTime), NULL)
GO
INSERT [dbo].[Products] ([product_id], [name], [quantity], [likes], [price], [regdate], [last_update]) VALUES (CAST(4 AS Numeric(18, 0)), N'Chocolate', 13, 0, CAST(2.50 AS Numeric(18, 2)), CAST(0x0000AA3E00BA9F2C AS DateTime), NULL)
GO
INSERT [dbo].[Products] ([product_id], [name], [quantity], [likes], [price], [regdate], [last_update]) VALUES (CAST(5 AS Numeric(18, 0)), N'Cars', 20, 0, CAST(2.50 AS Numeric(18, 2)), CAST(0x0000AA3E00BAA9C9 AS DateTime), NULL)
GO
INSERT [dbo].[Products] ([product_id], [name], [quantity], [likes], [price], [regdate], [last_update]) VALUES (CAST(6 AS Numeric(18, 0)), N'Bananas', 21, 2, CAST(50.00 AS Numeric(18, 2)), CAST(0x0000AA4000B4A682 AS DateTime), CAST(0x0000AA46016A9BB6 AS DateTime))
GO
INSERT [dbo].[Products] ([product_id], [name], [quantity], [likes], [price], [regdate], [last_update]) VALUES (CAST(8 AS Numeric(18, 0)), N'Modems', 55, 0, CAST(15.50 AS Numeric(18, 2)), CAST(0x0000AA46016A12E3 AS DateTime), NULL)
GO
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
INSERT [dbo].[Users] ([user_name], [password], [active], [user_level]) VALUES (N'admin', N'123456', 1, 1)
GO
INSERT [dbo].[Users] ([user_name], [password], [active], [user_level]) VALUES (N'user', N'123456', 1, 2)
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_LogUsersProducts_regdate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[LogUsersProducts] ADD  CONSTRAINT [DF_LogUsersProducts_regdate]  DEFAULT (getdate()) FOR [regdate]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Products_quantity]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_quantity]  DEFAULT ((0)) FOR [quantity]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Products_likes]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_likes]  DEFAULT ((0)) FOR [likes]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Products_regdate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_regdate]  DEFAULT (getdate()) FOR [regdate]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Users_active]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_active]  DEFAULT ((1)) FOR [active]
END

GO
