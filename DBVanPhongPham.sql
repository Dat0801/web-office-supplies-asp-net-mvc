create database DB_VanPhongPham
go

use DB_VanPhongPham
go

create table products 
(
	product_id varchar(10) not null,
	category_id varchar(10) not null,
	product_name nvarchar(200) not null unique,
	description nvarchar(1000) default null,
	purchase_price float not null,
	price_coefficient float default 0.5,
	price float default null,
	promotion_price float default null,
	stock_quantity int not null,
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
	parent_category_id VARCHAR(10) default null,
	category_name nvarchar(50) not null unique,
	description nvarchar(200) default null,
	status bit default 1,
	created_at datetime default getdate(),
	updated_at datetime default getdate()
)

create table suppliers
(
	supplier_id varchar(10) not null,
	supplier_name nvarchar(50) not null unique,
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
	status bit default 1,
)

create table images
(
	image_id int identity (1,1) not null,
	product_id varchar(10) not null,
	image_url varchar(500) not null unique,
	description nvarchar(200) default null,
	is_primary bit default 0 not null,
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
    address_id INT PRIMARY KEY IDENTITY(1,1) NOT NULL, -- Thêm IDENTITY(1,1) để tăng tự động
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

create table orders
(
	order_id varchar(10) not null,
	employee_id nvarchar(255) not null,
	customer_id nvarchar(255) not null,
	info_address nvarchar(255) not null,
	method_id varchar(10) not null,
	delivery_date datetime default DATEADD(DAY, 7, GETDATE()),
	total_amount float default 0,
	order_status_id int not null,
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
	total_amount float default 0,
	isReviewed bit default 0
)

create table receipts
(
	receipt_id varchar(10) not null,
	employee_id nvarchar(255) not null,
	supplier_id varchar(10) not null,
	total_cost float default null,
	notes nvarchar(200) default null,
	receipt_status nvarchar(50) default N'Đã nhập hàng',
	created_at datetime default getdate(),
)

create table receipt_details 
(
	receipt_id varchar(10) not null,
	product_id varchar(10) not null,
	quantity int default 1,
	purchase_price float not null,
	total_amount float default 0,
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
alter table images add primary key (image_id);
alter table payment_methods add primary key (method_id);
alter table orders add primary key (order_id);
alter table order_details add primary key (order_id, product_id);
alter table receipts add primary key (receipt_id);
alter table receipt_details add primary key (receipt_id, product_id);

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

alter table order_details
add constraint FK_OrderDetails_Orders
foreign key (order_id) references orders(order_id);

alter table order_details
add constraint FK_OrderDetails_Products
foreign key (product_id) references products(product_id);

alter table receipts
add constraint FK_Receipts_Users
foreign key (employee_id) references users(user_id);

alter table receipts
add constraint FK_Receipts_Suppliers
foreign key (supplier_id) references suppliers(supplier_id);

alter table receipt_details
add constraint FK_Receipt_Details_Receipts
foreign key (receipt_id) references Receipts(receipt_id);

alter table receipt_details
add constraint FK_ReceiptDetails_Products
foreign key (product_id) references products(product_id);

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
    UPDATE categories
    SET updated_at = GETDATE()
    WHERE category_id IN (SELECT category_id FROM inserted);
END;
GO

-- Cập nhật trường updated_at trong bảng suppliers khi có thay đổi.
CREATE TRIGGER trg_UpdateSuppliers
ON suppliers
AFTER UPDATE
AS
BEGIN
    UPDATE suppliers
    SET updated_at = GETDATE()
    WHERE supplier_id IN (SELECT supplier_id FROM inserted);
END;
GO

-- Trigger tính toán giá bán sản phẩm dựa trên giá mua và hệ số giá.
CREATE TRIGGER trg_CalculateProductPrice
ON products
AFTER INSERT, UPDATE
AS
BEGIN
    UPDATE products
    SET price = purchase_price * (1 + price_coefficient)
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


-- Trigger cập nhật số lượng tồn kho trong bảng products khi một đơn hàng được đặt.
CREATE TRIGGER trg_UpdateStockQuantity
ON order_details
AFTER INSERT
AS
BEGIN
    UPDATE products
    SET stock_quantity = stock_quantity - inserted.quantity
    FROM products
    INNER JOIN inserted ON products.product_id = inserted.product_id
END;
GO	

-- Trigger tính toán tổng số tiền cho từng chi tiết đơn hàng.
CREATE TRIGGER trg_CalculateOrderDetailsTotalAmount
ON order_details
AFTER INSERT
AS
BEGIN
    UPDATE order_details
    SET total_amount = od.quantity * p.price
    FROM order_details od
    INNER JOIN products p ON od.product_id = p.product_id
    INNER JOIN inserted i ON od.order_id = i.order_id AND od.product_id = i.product_id;
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
        SELECT SUM(od.total_amount)
        FROM order_details od
        WHERE od.order_id = orders.order_id
    )
    FROM orders
    INNER JOIN (SELECT DISTINCT order_id FROM inserted) od 
    ON orders.order_id = od.order_id;
END;
GO

-- Trigger khôi phục số lượng tồn kho khi một đơn hàng bị hủy.
CREATE TRIGGER trg_RestoreStockOnOrderCancellation
ON orders
AFTER UPDATE
AS
BEGIN
    IF EXISTS (SELECT 1 FROM inserted WHERE order_status_id = 6)
    BEGIN
        UPDATE p
        SET p.stock_quantity = p.stock_quantity + od.quantity
        FROM products p
        INNER JOIN order_details od ON p.product_id = od.product_id
        INNER JOIN inserted i ON od.order_id = i.order_id
        WHERE i.order_status_id = 6;
    END;
END;
GO

-- Trigger cập nhật giá mua của sản phẩm nếu giá mua mới lớn hơn giá mua hiện tại.
CREATE TRIGGER trg_UpdatePurchasePrice
ON receipt_details
AFTER INSERT
AS
BEGIN
    UPDATE products
    SET purchase_price = i.purchase_price
    FROM products p
    INNER JOIN inserted i ON p.product_id = i.product_id
    WHERE i.purchase_price > p.purchase_price;
END;
GO

-- Trigger tính toán tổng số tiền cho từng chi tiết phiếu nhập.
CREATE TRIGGER trg_CalculateReceiptDetailsTotalAmount
ON receipt_details
AFTER INSERT
AS
BEGIN
    UPDATE rd
    SET total_amount = rd.purchase_price * rd.quantity
    FROM receipt_details rd
    INNER JOIN inserted i ON rd.receipt_id = i.receipt_id AND rd.product_id = i.product_id;
END;
GO

-- Trigger Tính toán tổng chi phí của một phiếu nhập.
CREATE TRIGGER trg_CalculateReceiptTotalCost
ON receipt_details
AFTER INSERT
AS
BEGIN
    UPDATE receipts
    SET total_cost = (
        SELECT SUM(rd.total_amount)
        FROM receipt_details rd
        WHERE rd.receipt_id = receipts.receipt_id
    )
    FROM receipts
    INNER JOIN (SELECT DISTINCT receipt_id FROM inserted) i
    ON receipts.receipt_id = i.receipt_id;
END;
GO

INSERT INTO categories (category_id, category_name, description, parent_category_id)
VALUES 
('CAT001', N'Bút viết', N'Bút viết', null),
('CAT002', N'Bút bi', N'Bút bi thông dụng, dễ sử dụng cho việc viết, ...','CAT001'),
('CAT003', N'Bút chì', N'Bút chì dùng để viết và vẽ, có thể tẩy được, ...','CAT001'),
('CAT004', N'Giấy photo', N'Giấy dùng để photo tài liệu, có độ bền cao, ...',null),
('CAT005', N'Máy tính cầm tay', N'Máy tính nhỏ gọn, tiện lợi cho việc tính toán, ...',null),
('CAT006', N'Vở viết', N'Vở dùng để ghi chép, có nhiều loại và kích thước khác nhau, ...',null);

INSERT INTO suppliers (supplier_id, supplier_name, email, phone_number, status)
VALUES 
('SUP001', N'Tổng Công ty Văn phòng phẩm Hòa Bình', 'info@vpphoabinh.com', '0245678910', 1),
('SUP002', N'Công ty TNHH Văn phòng phẩm Thành Đạt', 'contact@thanhdat.com', '0987654321', 1);

INSERT INTO products (product_id, category_id, product_name, description, purchase_price, price_coefficient, price, stock_quantity, status)
VALUES 
('PRO001', 'CAT001', N'Bút bi Thiên Long TL 023', N'Bút bi Thiên Long TL 023 được sử dụng rộng rãi trong các trường học, văn phòng, công sở,... Đây là một sản phẩm chất lượng cao, giá cả phải chăng, phù hợp với nhiều mục đích sử dụng.', 3500, 0.5, NULL, 100, 1),
('PRO002', 'CAT001', N'Bút bi Thiên Long TL 025', N'Bút bi Thiên Long TL 025 được thiết kế đơn giản và dễ sử dụng, phù hợp cho nhiều người. Đây là lựa chọn tuyệt vời cho công việc văn phòng và học tập.', 4000, 0.5, NULL, 80, 1),
('PRO003', 'CAT002', N'Bút chì Stabilo 2B 288', N'Bút chì Stabilo 2B 288 cho nét vẽ đậm dày, thích hợp dùng để tập viết chữ, vẽ phác thảo, vẽ bóng mờ, sáng tối hoặc tô trắc nghiệm. Chì có độ bền màu cao, lâu phai và dễ dàng xóa sạch bằng gôm tẩy khi sử dụng.', 14000, 0.5, NULL, 70, 1),
('PRO004', 'CAT003', N'Giấy a4 Double A 80gsm', N'Giấy photo a4 Double A DL80 GSM phù hợp với các nhu cầu in ấn văn phòng cơ bản như in ấn tài liệu, văn bản, hợp đồng, báo cáo,... với độ sắc nét nổi bật trong tầm giá cả hợp lý.', 55000, 0.5, NULL, 60, 1),
('PRO005', 'CAT004', N'Máy tính cầm tay Casio FX 580VN X new', N'Máy tính cầm tay Casio FX 580VN X new là một sản phẩm chất lượng cao, đáp ứng nhu cầu của nhiều đối tượng. Máy có màn hình lớn, rõ ràng, các nút bấm nhạy, dễ sử dụng.', 480000, 0.5, NULL, 40, 1),
('PRO006', 'CAT005', N'Vở Hồng Hà 300 trang A4 4532', N'Vở A4 300 trang Hồng Hà là sản phẩm chất lượng, phù hợp với nhu cầu của nhiều đối tượng sử dụng. Vở có giá thành hợp lý, phù hợp với túi tiền của học sinh, sinh viên.', 18000, 0.5, NULL, 50, 1);

INSERT INTO attributes (attribute_id, attribute_name)
VALUES ('ATT001', N'Thương hiệu'),
('ATT002', N'Màu sắc'),
('ATT003', N'Đóng gói'),
('ATT004', N'Trọng lượng'),
('ATT005', N'Đường kính viên bi')

INSERT INTO attribute_values (attribute_value_id, attribute_id, value)
VALUES ('VAL001', 'ATT001', N'Thiên Long'),
('VAL002', 'ATT002', 'Xanh'),
('VAL003', 'ATT002', N'Đỏ'),
('VAL004', 'ATT002', N'Đen'),
('VAL005', 'ATT003', N'20 cây / hộp'),
('VAL006', 'ATT004', '9 gram'),
('VAL007', 'ATT005', '0.8 mm');

INSERT INTO product_attribute_values (product_id, attribute_value_id)
VALUES ('PRO001', 'VAL001'),
('PRO001', 'VAL002'),
('PRO001', 'VAL003'),
('PRO001', 'VAL004'),
('PRO001', 'VAL005'),
('PRO001', 'VAL006'),
('PRO001', 'VAL007');

INSERT INTO images (product_id, image_url, description, is_primary)
VALUES
('PRO001', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1730185372/product_imgs/onumc2zaeyfnkrs3q92j.png', N'Test thôi', 1),
('PRO001', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1730185372/product_imgs/h6ntyymvhnx9nllpfz2q.jpg', N'Test thôi', 0),
('PRO001', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1730185372/product_imgs/bbdw0hhp9nkaegqfqmh6.jpg', N'Test thôi', 0),
('PRO002', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1730185372/product_imgs/cxyrbs1upydqmyxnoobj.png', N'Test thôi', 1),
('PRO002', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1730185372/product_imgs/fxkfvtzliwsp6th6xnuk.png', N'Test thôi', 0),
('PRO003', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1730185371/product_imgs/uzvymp9snxzaivs9kbyt.png', N'Test thôi', 1),
('PRO003', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1730185371/product_imgs/krpimoesikznpyo3czhh.png', N'Test thôi', 0)
--=======
--INSERT INTO images (image_id, product_id, image_url, description, is_primary)
--VALUES
--('IMG001', 'PRO001', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1730185372/product_imgs/onumc2zaeyfnkrs3q92j.png', N'Test thôi', 1),
--('IMG002', 'PRO001', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1730185372/product_imgs/h6ntyymvhnx9nllpfz2q.jpg', N'Test thôi', 0),
--('IMG003', 'PRO001', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1730185372/product_imgs/bbdw0hhp9nkaegqfqmh6.jpg', N'Test thôi', 0),
--('IMG004', 'PRO002', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1730185372/product_imgs/cxyrbs1upydqmyxnoobj.png', N'Test thôi', 1),
--('IMG005', 'PRO002', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1730185372/product_imgs/fxkfvtzliwsp6th6xnuk.png', N'Test thôi', 0),
--('IMG006', 'PRO003', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1730185371/product_imgs/uzvymp9snxzaivs9kbyt.png', N'Test thôi', 1),
--('IMG007', 'PRO003', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1730185371/product_imgs/krpimoesikznpyo3czhh.png', N'Test thôi', 0)
-->>>>>>> a28c84148908bcb4541dee1e75b401364df26814:DBVanPhongPham.sql

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
(N'Chờ thanh toán'),
(N'Chờ xác nhận'),
(N'Chờ lấy hàng'),
(N'Chờ giao hàng'),
(N'Hoàn thành'),
(N'Đã hủy'),
(N'Trả hàng/Hoàn tiền')

INSERT INTO users (user_id, full_name, username)
VALUES
('ADMIN002', N'Thành Đạt', 'qwe')

INSERT INTO user_roles
VALUES
('ADMIN002', 2)

GO

UPDATE users
SET password = CONVERT(VARCHAR(32), HASHBYTES('MD5', '123'), 2)
WHERE username = 'qwe';
