create database DB_VanPhongPham
go

use DB_VanPhongPham
go

create table products 
(
	product_id varchar(10) not null,
	category_id varchar(10) not null,
	product_name nvarchar(255) not null unique,
	description nvarchar(max) default null,
	purchase_price float default 0,
	price_coefficient float default 0.5,
	price float default 0,
	promotion_price float default 0,
	stock_quantity int default 0,
	sold int default 0,
	avgRating float default 0,
	visited int default 0,
	status bit default 1,
	created_at datetime default getdate(),
	updated_at datetime default getdate()
)

create table promotions
(
	promotion_id VARCHAR(10) NOT NULL,
	promotion_name nvarchar(50) not null unique,
    discount_percent FLOAT NOT NULL,
    start_date DATETIME NOT NULL,
    end_date DATETIME NOT NULL,
	description nvarchar(500) default null,
    status BIT DEFAULT 1,
)

create table product_promotions
(
	promotion_id VARCHAR(10) NOT NULL,
    product_id VARCHAR(10) NOT NULL,
)

create table categories 
(
	category_id varchar(10) not null,	
	category_name nvarchar(150) not null unique,
	status bit default 1,
	created_at datetime default getdate(),
	updated_at datetime default getdate()
)

create table suppliers
(
	supplier_id varchar(10) not null,
	supplier_name nvarchar(200) not null unique,
	email varchar(50) default null,
	phone_number char(10) not null,
	status bit default 1,
	created_at datetime default getdate(),
	updated_at datetime default getdate()
)

create table attributes
(
	attribute_id varchar(10) not null,
	attribute_name nvarchar(50) not null unique,
	status bit default 1,
)

create table attribute_values
(
	attribute_value_id varchar(10) not null,
	attribute_id varchar(10) not null,
	value nvarchar(50) not null unique,
	status bit default 1,
)

create table product_attribute_values
(
	product_id varchar(10) not null,
	attribute_value_id varchar(10) not null,
)

create table images
(
	image_id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	product_id varchar(10) not null,
	image_url varchar(500) not null unique,
	is_primary bit default 0,
)

create table users
(
	user_id nvarchar(255) PRIMARY KEY NOT NULL, 
	full_name nvarchar(max),
	username varchar(max),
	email nvarchar(max),
	gender nvarchar(50),
	dob date,
	avt_url nvarchar(max),	
	password varchar(32),
	status bit default 0
)

create table roles 
(
	role_id int primary key identity (1,1),
	role_name nvarchar(50) not null unique,
	description nvarchar(200) default null,
)

create table user_roles
(
	user_id nvarchar(255) not null,
	role_id int not null,
	PRIMARY KEY (user_id, role_id),
    	FOREIGN KEY (user_id) REFERENCES users(user_id),
    	FOREIGN KEY (role_id) REFERENCES roles(role_id)
)

create table addresses
(
    address_id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    user_id NVARCHAR(255) NOT NULL,
    full_name NVARCHAR(MAX) NOT NULL,
    phone_number VARCHAR(10) NOT NULL,
    address_line NVARCHAR(255) NOT NULL,
    ward NVARCHAR(255) NOT NULL,
    district INT NOT NULL,
    province INT NOT NULL,
    isDefault BIT
);

create table order_status
(
	order_status_id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	order_status_name NVARCHAR(255) NOT NULL
)
create table payment_status
(
	payment_status_id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	payment_status_name NVARCHAR(255) NOT NULL
)
create table orders
(
	order_id varchar(10) not null,
	employee_id nvarchar(255),
	customer_id nvarchar(255) not null,
	info_address nvarchar(255) not null,
	ordernote nvarchar(255),
	ordercode nvarchar(255),
	method_id varchar(10) not null,
	delivery_date datetime default DATEADD(DAY, 7, GETDATE()),
	shipping_fee float,
	total_amount float default 0,
	order_status_id int not null,
	payment_status_id int not null,
	cancellation_requested int default 0,
	cancellation_reason nvarchar(255),
	created_at datetime default getdate(),
)

create table product_review
(
	review_id int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	user_id NVARCHAR(255) not null,
	product_id varchar(10) not null,
	rating int not null,
	review_content nvarchar(255) not null,
	created_at datetime default getdate(),
)

create table payment_methods 
(
	method_id varchar(10) not null,
	method_name nvarchar(30) not null,
)

create table order_details
(
	order_id varchar(10) not null,
	product_id varchar(10) not null,
	quantity int default 1,
	price float default 0,
	discountPrice float default 0,
	total_amount float default 0,
	isReviewed bit default 0
)

create table purchase_order
(
	purchase_order_id varchar(10) not null,
	supplier_id varchar(10) not null,
	employee_id nvarchar(255) not null,
	item_count int default 0,
	status nvarchar(50) default N'Đang giao',
	created_at datetime default getdate(),
)

create table purchase_order_detail
(
	purchase_order_id varchar(10) not null,
	product_id varchar(10) not null,
	quantity int default 1,
	price float default 0,
	quantity_received int default 0
)

create table receipts
(
	receipt_id varchar(10) not null,
	purchase_order_id varchar(10) not null,
	entry_count int default 1,
	created_at datetime default getdate(),
)

create table receipt_details 
(
	receipt_id varchar(10) not null,
	purchase_order_id varchar(10) not null,
	product_id varchar(10) not null,
	quantity int default 1,
)

create table cart_section
(
	cart_id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	user_id nvarchar(255) NOT NULL,
)

create table cart_details
(
	cart_id int NOT NULL,
	product_id varchar(10) NOT NULL,
	quantity int,
	total_amount float,
	isSelected INT DEFAULT 0,
	PRIMARY KEY (cart_id, product_id)
)

alter table products add primary key (product_id);
alter table categories add primary key (category_id);
alter table promotions add primary key (promotion_id);
alter table product_promotions add primary key (promotion_id, product_id);
alter table suppliers add primary key (supplier_id);
alter table attributes add primary key (attribute_id);
alter table attribute_values add primary key (attribute_value_id);
alter table product_attribute_values add primary key (product_id, attribute_value_id);
alter table payment_methods add primary key (method_id);
alter table orders add primary key (order_id);
alter table order_details add primary key (order_id, product_id);
alter table purchase_order add primary key (purchase_order_id);
alter table purchase_order_detail add primary key (purchase_order_id, product_id);
alter table receipts add primary key (receipt_id);
alter table receipt_details add primary key (receipt_id, purchase_order_id, product_id);

alter table product_promotions
add constraint FK_ProductPromotions_Products
foreign key (product_id) references products(product_id);

alter table product_promotions
add constraint FK_ProductPromotions_Promotions
foreign key (promotion_id) references promotions(promotion_id);

alter table products
add constraint FK_Products_Categories
foreign key (category_id) references categories(category_id);

alter table product_attribute_values
add constraint FK_ProductAttributeValues_Products
foreign key (product_id) references products(product_id);

alter table product_attribute_values
add constraint FK_ProductAttributeValues_AttributeValues
foreign key (attribute_value_id) references attribute_values(attribute_value_id);

alter table attribute_values
add constraint FK_AttributeValues_Attribute
foreign key (attribute_id) references attributes(attribute_id);

alter table images
add constraint FK_Images_Products
foreign key (product_id) references products(product_id);

alter table addresses
add constraint FK_Addresses_Users
foreign key (user_id) references users(user_id);

alter table orders
add constraint FK_Orders_Customers
foreign key (customer_id) references users(user_id);

alter table orders
add constraint FK_Orders_Employees
foreign key (employee_id) references users(user_id);

alter table orders
add constraint FK_Orders_PaymentMethods
foreign key (method_id) references payment_methods(method_id);

alter table orders
add constraint FK_Orders_OrderStatus
foreign key (order_status_id) references order_status(order_status_id);

alter table orders
add constraint FK_Orders_PaymentStatus
foreign key (payment_status_id) references payment_status(payment_status_id);

alter table order_details
add constraint FK_OrderDetails_Orders
foreign key (order_id) references orders(order_id);

alter table order_details
add constraint FK_OrderDetails_Products
foreign key (product_id) references products(product_id);

ALTER TABLE purchase_order
ADD CONSTRAINT FK_PurchaseOrder_Suppliers
FOREIGN KEY (supplier_id) REFERENCES suppliers(supplier_id);

ALTER TABLE purchase_order
ADD CONSTRAINT FK_PurchaseOrder_Employees
FOREIGN KEY (employee_id) REFERENCES users(user_id);

ALTER TABLE purchase_order_detail
ADD CONSTRAINT FK_PurchaseOrderDetail_PurchaseOrder
FOREIGN KEY (purchase_order_id) REFERENCES purchase_order(purchase_order_id);

ALTER TABLE purchase_order_detail
ADD CONSTRAINT FK_PurchaseOrderDetail_Products
FOREIGN KEY (product_id) REFERENCES products(product_id);

ALTER TABLE receipts
ADD CONSTRAINT FK_Receipts_PurchaseOrder
FOREIGN KEY (purchase_order_id) REFERENCES purchase_order(purchase_order_id);

alter table receipt_details
add constraint FK_Receipt_Details_Receipts
foreign key (receipt_id) references Receipts(receipt_id);

alter table receipt_details
add constraint FK_Receipt_Details_Purchase_Order_Detail
foreign key (purchase_order_id, product_id) references purchase_order_detail(purchase_order_id, product_id);

alter table product_review
add constraint FK_ProductReview_Users
foreign key (user_id) references users(user_id)

alter table product_review
add constraint FK_ProductReview_Product
foreign key (product_id) references products(product_id)

alter table cart_section
add constraint FK_CartSection_Users
foreign key (user_id) references users(user_id)

alter table cart_details
add constraint FK_CartDetails_CartSection
foreign key (cart_id) references cart_section(cart_id);

alter table cart_details
add constraint FK_CartDetails_Products
foreign key (product_id) references products(product_id);
go

-- Cập nhật trường updated_at trong bảng products mỗi khi có bản ghi bị cập nhật.
CREATE TRIGGER trg_UpdateProducts
ON products
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM inserted WHERE updated_at IS NULL OR updated_at < GETDATE())
    BEGIN
        UPDATE products
        SET updated_at = GETDATE()
        WHERE product_id IN (SELECT product_id FROM inserted);
    END
END;
GO

-- Cập nhật trường updated_at trong bảng categories mỗi khi có bản ghi bị cập nhật.
CREATE TRIGGER trg_UpdateCategories
ON categories
AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM inserted WHERE updated_at IS NULL OR updated_at < GETDATE())
    BEGIN
		UPDATE categories
		SET updated_at = GETDATE()
		WHERE category_id IN (SELECT category_id FROM inserted);
    END
END;
GO

-- Cập nhật trường updated_at trong bảng suppliers khi có thay đổi.
CREATE TRIGGER trg_UpdateSuppliers
ON suppliers
AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM inserted WHERE updated_at IS NULL OR updated_at < GETDATE())
    BEGIN
		UPDATE suppliers
		SET updated_at = GETDATE()
		WHERE supplier_id IN (SELECT supplier_id FROM inserted);
    END
END;
GO

-- Trigger tính toán giá bán sản phẩm dựa trên giá mua và hệ số.
CREATE TRIGGER trg_CalculateProductPrice
ON products
AFTER INSERT, UPDATE
AS
BEGIN
    UPDATE products
    SET price = ROUND(purchase_price * price_coefficient, 2)
    WHERE product_id IN (SELECT product_id FROM inserted);
END;
GO


-- Trigger tính toán giá khuyến mãi sản phẩm dựa bảng khuyến mãi.
CREATE TRIGGER trg_UpdatePromotionPrice
ON product_promotions
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    UPDATE p
    SET p.promotion_price = p.purchase_price * (1 - pr.discount_percent / 100)
    FROM products p
    INNER JOIN product_promotions pp ON p.product_id = pp.product_id
    INNER JOIN promotions pr ON pp.promotion_id = pr.promotion_id
    WHERE pr.status = 1
      AND GETDATE() BETWEEN pr.start_date AND pr.end_date;
END;
GO

-- Trigger cập nhật số lượng sản phẩm tồn kho sau khi sản phẩm được bán.
CREATE TRIGGER trg_UpdateProductStock
ON order_details
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE p
    SET p.stock_quantity = p.stock_quantity - inserted.quantity
    FROM products p
    INNER JOIN inserted ON p.product_id = inserted.product_id;
END;
GO

--Trigger cập nhật số lượng sản phẩm đã bán khi trạng thái đơn hàng thay đổi thành "Hoàn thành".
CREATE TRIGGER trg_UpdateProductSoldOnOrderStatus
ON orders
AFTER UPDATE
AS
BEGIN
    IF UPDATE(order_status_id)
    BEGIN
        SET NOCOUNT ON;
        UPDATE p
        SET p.sold = p.sold + od.quantity 
        FROM products p
        INNER JOIN order_details od ON p.product_id = od.product_id
        INNER JOIN inserted i ON od.order_id = i.order_id
        WHERE i.order_status_id = 3; 
    END
END;
GO

--Trigger cập nhật trạng thái "isSelected" trong giỏ hàng khi số lượng tồn kho của sản phẩm giảm xuống 0.
CREATE TRIGGER trg_UpdateCartDetails
ON products
AFTER UPDATE
AS
BEGIN
    UPDATE cd
    SET cd.isSelected = 0
    FROM cart_details cd
    INNER JOIN inserted i ON cd.product_id = i.product_id
    WHERE i.stock_quantity = 0;
END;

GO

--Trigger bỏ chọn sản phẩm trong giỏ hàng khi sản phẩm ngừng kinh doanh.
CREATE TRIGGER trg_UpdateCartDetailsOnStatus
ON products
AFTER UPDATE
AS
BEGIN
    UPDATE cd
    SET cd.isSelected = 0
    FROM cart_details cd
    INNER JOIN inserted i ON cd.product_id = i.product_id
    WHERE i.status = 0; 
END;

GO

--Trigger xóa sản phẩm khỏi giỏ hàng của người dùng sau khi sản phẩm đó được đặt hàng.
CREATE TRIGGER trg_ClearSpecificCartDetailsOnOrder
ON order_details
AFTER INSERT
AS
BEGIN
    DECLARE @order_id VARCHAR(10), @user_id NVARCHAR(255);

    SELECT @order_id = order_id
    FROM inserted;

    SELECT @user_id = customer_id
    FROM orders
    WHERE order_id = @order_id;

    DELETE cd
    FROM cart_details cd
    INNER JOIN cart_section cs ON cd.cart_id = cs.cart_id
    INNER JOIN inserted i ON cd.product_id = i.product_id
    WHERE cs.user_id = @user_id AND i.order_id = @order_id;
END

GO

-- Trigger khôi phục số lượng tồn kho của sản phẩm khi đơn hàng bị hủy.
CREATE TRIGGER trg_RestoreProductStockOnOrderCancel
ON orders
AFTER UPDATE
AS
BEGIN
    IF UPDATE(order_status_id)
    BEGIN
        SET NOCOUNT ON;

        UPDATE p
        SET p.stock_quantity = p.stock_quantity + od.quantity
        FROM products p
        INNER JOIN order_details od ON p.product_id = od.product_id
        INNER JOIN inserted i ON od.order_id = i.order_id
        WHERE i.order_status_id = 4;
    END
END;

GO

-- Trigger tính toán tổng số tiền của một đơn hàng.
CREATE TRIGGER trg_CalculateOrderTotalAmount
ON order_details
AFTER INSERT
AS
BEGIN
    UPDATE orders
    SET total_amount = (
        SELECT SUM(od.total_amount) + orders.shipping_fee
        FROM order_details od
        WHERE od.order_id = orders.order_id
        GROUP BY od.order_id
    )
    WHERE orders.order_id IN (SELECT DISTINCT order_id FROM inserted);
END;

GO

-- Trigger để cập nhật item_count sau khi thêm mới chi tiết đơn đặt hàng
CREATE TRIGGER trg_AfterInsertPurchaseOrderDetail
ON purchase_order_detail
AFTER INSERT
AS
BEGIN
    UPDATE purchase_order
    SET item_count = (
        SELECT COUNT(*)
        FROM purchase_order_detail
        WHERE purchase_order_detail.purchase_order_id = purchase_order.purchase_order_id
    )
    WHERE purchase_order.purchase_order_id IN (SELECT purchase_order_id FROM INSERTED);
END;
GO

-- Trigger cập nhật giá mua của sản phẩm nếu giá mua mới lớn hơn giá mua hiện tại.
CREATE TRIGGER trg_UpdatePurchasePrice
ON purchase_order_detail
AFTER INSERT
AS
BEGIN
    UPDATE products
    SET purchase_price = i.price
    FROM products p
    INNER JOIN inserted i ON p.product_id = i.product_id
    WHERE i.price > p.purchase_price;
END;
GO

-- Trigger để cập nhật stock_quantity trong bảng products khi có hàng nhập vào
CREATE TRIGGER trg_UpdateStockQuantityInReceipt
ON receipt_details
AFTER INSERT
AS
BEGIN
    UPDATE p
    SET p.stock_quantity = p.stock_quantity + i.quantity
    FROM products p
    INNER JOIN inserted i
        ON p.product_id = i.product_id;
END;
GO


-- Trigger cập nhật số lượng đã nhận trong bảng purchase_order_detail
CREATE TRIGGER trg_UpdateQuantityReceived
ON receipt_details
AFTER INSERT
AS
BEGIN
    UPDATE pod
    SET pod.quantity_received = pod.quantity_received + i.quantity
    FROM purchase_order_detail pod
    INNER JOIN inserted i
        ON pod.purchase_order_id = i.purchase_order_id
        AND pod.product_id = i.product_id;
END;
GO

-- Trigger để kiểm tra và cập nhật trạng thái đơn hàng nếu tất cả sản phẩm đã nhập đủ
CREATE TRIGGER trg_CheckAndUpdateOrderStatus
ON receipt_details
AFTER INSERT
AS
BEGIN
    UPDATE po
    SET po.status = N'Đã giao'
    FROM purchase_order po
    INNER JOIN purchase_order_detail pod ON po.purchase_order_id = pod.purchase_order_id
    INNER JOIN inserted i ON i.purchase_order_id = po.purchase_order_id
    WHERE NOT EXISTS (
        SELECT 1
        FROM purchase_order_detail pod2
        WHERE pod2.purchase_order_id = po.purchase_order_id
        AND pod2.quantity > pod2.quantity_received
    ); 
END;
GO

INSERT INTO categories (category_id, category_name)
VALUES 
('CAT001', N'Bút bi'),
('CAT002', N'Bút chì'),
('CAT003', N'Máy tính tay văn phòng');

INSERT INTO suppliers (supplier_id, supplier_name, email, phone_number, status)
VALUES 
('SUP001', N'Tổng Công ty Văn phòng phẩm Hòa Bình', 'info@vpphoabinh.com', '0245678910', 1),
('SUP002', N'Công ty TNHH Văn phòng phẩm Thành Đạt', 'contact@thanhdat.com', '0987654321', 1);

INSERT INTO products (product_id, category_id, product_name, description, purchase_price, price_coefficient, price, stock_quantity, status)
VALUES 
('PRO001', 'CAT001', N'Bút bi Thiên Long TL 023', N'<p>Bút có thiết kế đơn giản, thân tròn, dễ cầm nắm. Thân bút nhựa trong.</p><p><strong>Đặc điểm:</strong></p><p>- Đầu bi: 0.8 mm<br>- Thân bút thanh mảnh cơ chế bấm khế tiện dụng phù hợp cho mọi người.<br>- Thay ruột khi hết mực.</p><p><br>&nbsp;</p>', 0, 1.5, 0, 0, 1),
('PRO002', 'CAT001', N'Bút bi Thiên Long TL 025', N'<p>Bút có thiết kế đơn giản, thân tròn.Thân bút nhựa trong, tảm có đệm mềm (grip) giúp cầm êm tay và giảm trơn trợt khi viết.</p><p><strong>Đặc điểm:</strong></p><p>- Đầu bi: 0.8 mm<br>- Grip cùng màu mực<br>- Thân bút thanh mảnh cơ chế bấm khế tiện dụng phù hợp cho mọi người.<br>- Thay ruột khi hết mực.</p>', 0, 1.5, 0, 0, 1),
('PRO003', 'CAT002', N'Combo 20 Bút chì mỹ thuật Thiên Long 5B GP-024', N'<p>Bút chì mỹ thuật Thiên Long 5B GP-024 thích hợp cho các hoạt động như ghi chép, vẽ nháp, học tập.</p><p><strong>
Đặc điểm:</strong></p><p>- Ruột chì mềm, nét đậm, ít bột chì<br>- Thân gỗ mềm dễ chuốt<br>- 
Bền đẹp không gãy chì<br>- Bút dùng để viết, vẽ phác thảo trên giấy tập học sinh, sổ tay, giấy photocopy, gỗ hoặc giấy vẽ chuyên dụng<br>- Lướt rất nhẹ nhàng trên bề mặt viết<br>- Dùng để đánh bóng các bức vẽ, đạt đến nhiều mức độ sáng tối khác nhau. 
Ngoài ra khá hữu dụng trong việc tô đậm vào ô trả lời trắc nghiệm nhanh nhất.<br>- Thân lục giác, 5B.<br>- Thân bút được thiết kế hiện đại với họa tiết xoắn quanh bút cho cây bút sinh động và thu hút</p><p><strong>Bảo quản:</strong></p><p>- Tránh va đập mạnh làm gãy chì.<br>- Tránh xa nguồn nhiệt .</p>', 0, 1.5, 0, 0, 1),
('PRO004', 'CAT003', N'Máy tính văn phòng Thiên Long Flexio CAL-011', N'<p><strong>Đặc tính sản phẩm:</strong></p><p>- Máy tính văn phòng CAL-011 đa năng này phù hợp sử dụng tại nhà, trường học, văn phòng hoặc cửa hàng. Sự kết hợp chip xử lý và mạch điều khiển công nghệ hiện đại đưa ra những kết quả phép tính đáng tin cậy, nhanh chóng đáp ứng tốt cho mục đích cá nhân hoặc chuyên nghiệp.&nbsp;</p><p>- Thiết kế nhỏ gọn và di động, bạn có thể dễ dàng mang theo bất cứ mọi nơi.&nbsp;</p><p>- Bộ vỏ nguyên liệu ABS cao cấp, bền và sử dụng thiết kế thân thiện, máy tính văn phòng CAL-011 cung cấp sự tiện lợi và hiệu quả trong tất cả các nhu cầu tính toán.</p>', 0, 1.5, 0, 0, 1)

INSERT INTO attributes (attribute_id, attribute_name)
VALUES ('ATT001', N'Thương hiệu'),
('ATT002', N'Màu sắc'),
('ATT003', N'Đóng gói'),
('ATT004', N'Trọng lượng'),
('ATT005', N'Đường kính viên bi'),
('ATT006', N'Độ cứng ruột chì'),
('ATT007', N'Màn hình'),
('ATT008', N'Chất liệu'),
('ATT009', N'Loại pin');

INSERT INTO attribute_values (attribute_value_id, attribute_id, value)
VALUES ('VAL001', 'ATT001', N'Thiên Long'),
('VAL002', 'ATT002', 'Xanh'),
('VAL003', 'ATT002', N'Đỏ'),
('VAL004', 'ATT002', N'Đen'),
('VAL005', 'ATT003', N'20 cây/hộp'),
('VAL006', 'ATT004', '9 gram'),
('VAL007', 'ATT005', '0.8 mm'),
('VAL008', 'ATT004', '8 gram'),
('VAL009', 'ATT006', '5B'),
('VAL010', 'ATT003', N'10 cây/hộp'),
('VAL011', 'ATT001', N'Flexio'),
('VAL012', 'ATT007', N'LCD'),
('VAL013', 'ATT008', N'ABS'),
('VAL014', 'ATT009', N'AAA (1.55V)');

INSERT INTO product_attribute_values (product_id, attribute_value_id)
VALUES ('PRO001', 'VAL001'),
('PRO001', 'VAL002'),
('PRO001', 'VAL003'),
('PRO001', 'VAL004'),
('PRO001', 'VAL005'),
('PRO001', 'VAL006'),
('PRO001', 'VAL007'),
('PRO002', 'VAL001'),
('PRO002', 'VAL002'),
('PRO002', 'VAL003'),
('PRO002', 'VAL004'),
('PRO002', 'VAL005'),
('PRO002', 'VAL006'),
('PRO002', 'VAL007'),
('PRO003', 'VAL008'),
('PRO003', 'VAL009'),
('PRO003', 'VAL011'),
('PRO004', 'VAL008'),
('PRO004', 'VAL012'),
('PRO004', 'VAL013'),
('PRO004', 'VAL014');

INSERT INTO images (product_id, image_url, is_primary)
VALUES
('PRO001', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1731052395/xaxsqpih3wlkhn3svtzu.webp', 1),
('PRO001', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1731052396/e7f3ibtwciwnirlyrfy5.webp', 0),
('PRO001', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1731052397/acrcxyujqymmsmwjhptv.webp', 0),
('PRO001', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1731052428/ilsoqolpotlhrrz4lvgp.webp', 0),
('PRO002', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1731052512/ijhq94a3uxjmzsxxug1b.webp', 1),
('PRO002', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1731052513/crckzpsnajdaal5wxxjs.webp', 0),
('PRO002', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1731052513/jrh6zvjikf4e99cunqyr.webp', 0),
('PRO002', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1731052514/lwcgnvtxuxihrgxl42ln.webp', 0),
('PRO003', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1731053976/ookezmryr9dg2d6kwmsb.webp', 1),
('PRO003', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1731054041/cskuxusg6kbrllcsl6x9.jpg', 0),
('PRO004', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1731055234/u4oje3oyhrzadlw9glms.webp', 1),
('PRO004', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1731055235/sn45tefezfkholxonrip.webp', 0);

INSERT INTO roles (role_name, description)
VALUES
(N'Khách hàng', N'Khách mua hàng'),
(N'Quản lý', N'Quản lý toàn bộ hệ thống'),
(N'Nhân viên bán hàng', N'Nhân viên bán hàng'),
(N'Nhân viên nhập hàng', N'Nhân viên nhập hàng');

INSERT INTO payment_methods (method_id, method_name)
VALUES
('PAY001', N'Thanh toán khi nhận hàng'),
('PAY002', N'Thanh toán bằng chuyển khoản')

INSERT INTO order_status (order_status_name)
VALUES
(N'Chờ xác nhận'),
(N'Chờ giao hàng'),
(N'Hoàn thành'),
(N'Đã hủy')

INSERT INTO payment_status (payment_status_name)
VALUES
(N'Chưa thanh toán'),
(N'Đã thanh toán'),
(N'Đã hoàn tiền')

INSERT INTO users (user_id, full_name, username)
VALUES
('ADMIN001', N'Thành Đạt', 'qwe')

INSERT INTO user_roles
VALUES
('ADMIN001', 2)
GO

UPDATE users
SET password = CONVERT(VARCHAR(32), HASHBYTES('MD5', '123'), 2)
WHERE username = 'qwe';


INSERT INTO purchase_order (purchase_order_id, supplier_id, employee_id)
VALUES
('POD001', 'SUP001', 'ADMIN001')

INSERT INTO purchase_order_detail
VALUES
('POD001', 'PRO001', 100, 3500, 0),
('POD001', 'PRO002', 80, 4000, 0),
('POD001', 'PRO003', 70, 14000, 0),
('POD001', 'PRO004', 60, 55000, 0)

INSERT INTO receipts (receipt_id, purchase_order_id, entry_count)
VALUES 
('REC001', 'POD001', 1)

INSERT INTO receipt_details
VALUES
('REC001', 'POD001', 'PRO001', 100),
('REC001', 'POD001', 'PRO002', 80),
('REC001', 'POD001', 'PRO003', 70),
('REC001', 'POD001', 'PRO004', 60)
