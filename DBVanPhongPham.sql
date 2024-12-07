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

CREATE TABLE coupons
(
    coupon_id VARCHAR(10) PRIMARY KEY NOT NULL,
    coupon_code NVARCHAR(50) NOT NULL UNIQUE,
	coupon_imgurl varchar(500) not null unique,
    coupon_title NVARCHAR(255) NOT NULL UNIQUE,
    coupon_description NVARCHAR(MAX) DEFAULT NULL,
    coupon_percent INT NOT NULL CHECK (coupon_percent BETWEEN 1 AND 100),
    quantity INT DEFAULT 0,
    created_at DATETIME DEFAULT GETDATE(),
    expires_at DATETIME NOT NULL,
    updated_at DATETIME DEFAULT GETDATE(),
	status bit default 1
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
	discount_amount float default 0,
	total_amount float default 0,
	order_status_id int not null,
	payment_status_id int not null,
	coupon_applied VARCHAR(10),
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
    SET p.promotion_price = ROUND(p.price * (1 - pr.discount_percent / 100), 0)
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

--Trigger cập nhật số lượng coupon khi đơn hàng được đặt.
CREATE TRIGGER trg_UpdateCouponQuantity
ON orders
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra nếu coupon_applied có giá trị (nếu có coupon áp dụng)
    IF EXISTS (SELECT 1 FROM inserted WHERE coupon_applied IS NOT NULL)
    BEGIN
        -- Giảm số lượng coupon khi đơn hàng có coupon
        UPDATE c
        SET c.quantity = c.quantity - 1
        FROM coupons c
        INNER JOIN inserted i ON c.coupon_id = i.coupon_applied
        WHERE i.coupon_applied = c.coupon_id;
    END
END;

GO

--Trigger khôi phục số lượng coupon khi đơn hàng bị hủy.
CREATE TRIGGER trg_RestoreCouponQuantityOnOrderCancel
ON orders
AFTER UPDATE
AS
BEGIN
    IF UPDATE(order_status_id)
    BEGIN
        SET NOCOUNT ON;

        -- Kiểm tra nếu order_status_id đã thay đổi và đơn hàng bị hủy (status = 4)
        IF EXISTS (SELECT 1 FROM inserted WHERE order_status_id = 4)
        BEGIN
            -- Cộng lại số lượng coupon khi đơn hàng bị hủy
            UPDATE c
            SET c.quantity = c.quantity + 1
            FROM coupons c
            INNER JOIN inserted i ON c.coupon_id = i.coupon_applied
            WHERE i.coupon_applied = c.coupon_id AND i.order_status_id = 4;
        END
    END
END;

GO

-- Trigger tính avgRating cho products
CREATE TRIGGER trg_UpdateAvgRating
ON product_review
AFTER INSERT
AS
BEGIN
    -- Nếu xảy ra bất kỳ lỗi nào, bỏ qua trigger
    SET NOCOUNT ON;

    -- Cập nhật avgRating cho các sản phẩm bị đánh giá mới
    UPDATE products
    SET avgRating = (
        SELECT AVG(CAST(rating AS FLOAT))
        FROM product_review
        WHERE product_review.product_id = products.product_id
    )
    WHERE product_id IN (SELECT DISTINCT product_id FROM inserted);
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
        SELECT SUM(od.total_amount) - orders.discount_amount + orders.shipping_fee
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
('PRO004', 'CAT003', N'Máy tính văn phòng Thiên Long Flexio CAL-011', 
N'<p><strong>Đặc tính sản phẩm:</strong></p><p>- Máy tính văn phòng CAL-011 đa năng này phù hợp sử dụng tại nhà, trường học, văn phòng hoặc cửa hàng. Sự kết hợp chip xử lý và mạch điều khiển công nghệ hiện đại đưa ra những kết quả phép tính đáng tin cậy, nhanh chóng đáp ứng tốt cho mục đích cá nhân hoặc chuyên nghiệp.&nbsp;</p><p>- 
Thiết kế nhỏ gọn và di động, bạn có thể dễ dàng mang theo bất cứ mọi nơi.&nbsp;</p><p>- Bộ vỏ nguyên liệu ABS cao cấp, bền và sử dụng thiết kế thân thiện, máy tính văn phòng CAL-011 cung cấp sự tiện lợi và hiệu quả trong tất cả các nhu cầu tính toán.</p>', 0, 1.5, 0, 0, 1),
('PRO005', 'CAT001', N'Combo 20 bút bi FlexOffice FO-023 mực đen', N'<p>BÚT BI SNAPE FO-023 là sản phẩm do Tập đoàn Thiên Long sản xuất, mang nhãn hiệu FlexOffice. Sản phẩm được sản xuất theo công nghệ hiện đại, kiểu dáng phù hợp cho đối tượng học sinh, nhân viên văn phòng, sinh viên , giáo viên…</p><p>Những đặc điểm nổi bật của bút bi FO-023:</p><p>- Bút bi FO-023 được sản xuất theo công nghệ mới.<br>- Nét viết trơn, êm, mực ra đều và liên tục.<br>- Dạng bút bi cửa sổ bấm. Nút bấm và lò xo rất nhạy và bền, không bung, không kẹt, không tự rơi ra ngoài của sổ bấm.<br>- Công nghệ Smooth writing tiên tiến , viết trơn , êm , mực ra đều liên tục<br>- Đầu bi 0.7mm, viết trơn êm, màu mực đậm tươi, mực ra đều và liên tục.<br>- Thiết kế đẹp, đơn giản , rất chắc chắn , dễ cầm và chắc tay</p>', 0, 1.5, 0, 0, 1),
('PRO006', 'CAT001', N'Combo 20 Bút bi Thiên Long TL 048', N'<p>Bút có thiết kế thân tròn to, tạo cảm giác khỏe, chắc chắn.Thân bút làm từ nhựa trong có pha màu. Giắt bút bằng kim loại mạ crom sáng bóng. Tảm có đệm mềm (grip) giúp cầm êm tay và giảm trơn trợt khi viết.</p><p><strong>Đặc điểm:</strong></p><p>- Bút bi dạng bấm khế, có Grip.</p><p><strong>Lợi ích:</strong></p><p>- Đệm mềm (Grip) giúp êm tay và chống trơn tuột khi viết thường xuyên.<br>- Kiểu dáng hiện đại, sang trọng, sản phẩm phù hợp cho sinh viên, giáo viên và nhân viên văn phòng.<br>- Thay ruột khi hết mực.</p>', 0, 1.5, 0, 0, 1),
('PRO007', 'CAT001', N'Combo 20 Bút bi Thiên Long TL 093', N'<p>Bút có cấu tạo khác hoàn toàn với các dạng bút truyền thống. Mực được bơm thẳng vào thân bút để sử dụng.Bút đùn là dạng bút không ruột, mực được bơm thẳng vào vỏ bút (hay nói cách khác là ruột bút cũng chính là thân bút). Chính vì thế mà trọng lượng của bút nhẹ hơn, phù hợp những người cần viết nhiều, tốc ký, ít gây mỏi tay.</p><p>Thân bút bằng nhựa trắng đục pha màu dạng sọc thẳng.Nắp bút bằng nhựa trong. Nét chữ thanh mảnh, sắc nét, gọn gàng.&nbsp;</p>', 0, 1.5, 0, 0, 1),
('PRO008', 'CAT001', N'Combo 20 Bút Bi Thiên Long TL 036', N'<p>Bút sử dụng cơ cấu bấm, thân tròn và chắc chắn.Thân bút làm từ nhựa màu đục. Giắt bút bằng kim loại mạ crom sáng bóng. Tảm có đệm mềm (grip) giúp cầm êm tay và giảm trơn trượt khi viết. Đầu bút dạng cone , 0.7mm sản xuất tại Thụy Sĩ.</p><p>Đặc điểm:</p><p>- Bút bi dạng bấm khế, có grip.</p><p><strong>Lợi ích:</strong></p><p>- Grip giúp êm tay và giảm trơn tuột khi viết thường xuyên.<br>- Giắt được làm bằng kim loại, tảm và nút xi kim loại sáng bóng tạo sự sang trọng.<br>- Sản phẩm phù hợp cho sinh viên, giáo viên và giới văn phòng.<br>- Thiết kế trẻ trung kiểu dáng mạnh mẽ</p><p><strong>Bảo quản:</strong></p><p>- Nhiệt độ: 10 ~ 55º C.<br>- Độ ẩm: 55 ~ 95% RH.</p>', 0, 1.5, 0, 0, 1),
('PRO009', 'CAT002', N'Combo 20 Bút chì mỹ thuật Thiên Long 4B GP-023', N'<p>Bút chì mỹ thuật Thiên Long 4B GP-023 thích hợp cho các hoạt động như ghi chép, vẽ nháp, học tập.</p><p><strong>Đặc điểm:</strong></p><p>- Ruột chì mềm, nét đậm, ít bột chì<br>- Thân gỗ mềm dễ chuốt<br>- Bền đẹp không gãy chì<br>- Bút dùng để viết, vẽ phác thảo trên giấy tập học sinh, sổ tay, giấy photocopy, gỗ hoặc giấy vẽ chuyên dụng<br>- Lướt rất nhẹ nhàng trên bề mặt viết<br>- Dùng để đánh bóng các bức vẽ, đạt đến nhiều mức độ sáng tối khác nhau. Ngoài ra khá hữu dụng trong việc tô đậm vào ô trả lời trắc nghiệm nhanh nhất.<br>- Thân lục giác, 4B.<br>- Thân bút được thiết kế hiện đại với họa tiết xoắn quanh bút cho cây bút sinh động và thu hút</p><p><strong>Bảo quản:</strong></p><p>- Tránh va đập mạnh làm gãy chì.<br>- Tránh xa nguồn nhiệt .</p>', 0, 1.5, 0, 0, 1),
('PRO010', 'CAT002', N'Combo 20 Bút chì mỹ thuật Thiên Long 3B GP-022', N'<p>Bút chì mỹ thuật Thiên Long 3B GP-022 thích hợp cho các hoạt động như ghi chép, vẽ nháp, học tập.</p><p><strong>Đặc điểm:</strong></p><p>- Ruột chì mềm, nét đậm, ít bột chì<br>- Thân gỗ mềm dễ chuốt<br>- Bền đẹp không gãy chì<br>- Bút dùng để viết, vẽ phác thảo trên giấy tập học sinh, sổ tay, giấy photocopy, gỗ hoặc giấy vẽ chuyên dụng<br>- Lướt rất nhẹ nhàng trên bề mặt viết<br>- Dùng để đánh bóng các bức vẽ, đạt đến nhiều mức độ sáng tối khác nhau. Ngoài ra khá hữu dụng trong việc tô đậm vào ô trả lời trắc nghiệm nhanh nhất.<br>- Thân lục giác, 3B.<br>- Thân bút được thiết kế hiện đại với họa tiết xoắn quanh bút cho cây bút sinh động và thu hút</p><p><strong>Bảo quản:</strong></p><p>- Tránh va đập mạnh làm gãy chì.<br>- Tránh xa nguồn nhiệt .</p>', 0, 1.5, 0, 0, 1),
('PRO011', 'CAT002', N'Combo 20 Bút chì gỗ cao cấp Bizner BIZ-P03', N'<p>Hiện nay, Bút chì gỗ cao cấp Thiên Long - Bizner BIZ-P03 đang dần trở nên gần gũi đối với các bạn học sinh, nhất là dùng trong môn học vẽ, hình học, phác thảo, tốc ký,&nbsp; thi trắc nghiệm... Nó là dụng cụ không thể thiếu với các nhà thiết kế, nhân viên văn phòng chuyên về các hoạt động sáng tạo và cho nhiều mục đích cá nhân khác.</p><p><strong>Đặc điểm:</strong></p><p>- Bút chì thuộc dòng bút cao cấp Bizner</p><p>- Thiết kế đơn giản nhưng tinh tế và sang trọng.</p><p>- Độ cứng ruột chì: H - cho nét vẽ sắc nét, viết trơn êm, nét viết ra đều.</p><p>- Thân bút hình lục giác, dễ cầm, không trơn, không mỏi tay khi cầm viết lâu</p><p>- Thích hợp cho phác thảo và tốc ký</p><p><strong>Bảo quản:</strong></p><p>- Hạn chế rơi nhiều lần,&nbsp;va đập mạnh khi vận chuyển và sử dụng.<br>- Lưu trữ và trưng bày nơi thoáng mát.<br>- Tránh xa nguồn nhiệt và những nơi có ánh nắng trực tiếp chiếu vào.</p>', 0, 1.5, 0, 0, 1),
('PRO012', 'CAT002', N'Combo 20 Bút chì bấm Thiên Long PC-024', N'<p>Thuộc loại bút chì bấm mang thương hiệu Thiên Long. Bút chì bấm Thiên Long PC-024 cóchất lượng bền bỉ, đáng tin cậy. Thiết kế hiện đại, các chi tiết sắc xảo và tinh tế.</p><p><strong>Đặc điểm :</strong></p><p>- Bút có trang bị gôm ở đuôi bút, tiện lợi và dễ dàng sử dụng.<br>- Min chì cứng, khó gãy, nét viết thanh mảnh, sắc nét.<br>- Ngòi bút HB 0.5mm<br>- Giắt bút chắc chắn thuận tiện cho việc gài lên túi áo hoặc lên sổ tay</p><p><strong>Bảo quản:</strong></p><p>- Tránh va đập mạnh làm gãy chì.<br>- Tránh xa nguồn nhiệt.</p>', 0, 1.5, 0, 0, 1),
('PRO013', 'CAT002', N'Combo 20 Bút chì gỗ Thiên Long GP-03', N'<p>Bút chì gỗ Thiên Long TP GP-03 thích hợp cho các hoạt động như ghi chép, vẽ nháp, học tập.</p><p><strong>Đặc điểm:</strong></p><p>- Nét đậm, để lại nhiều than chì trên giấy<br>- Lướt rất nhẹ nhàng trên bề mặt viết<br>- Dùng để đánh bóng các bức vẽ, đạt đến nhiều mức độ sáng tối khác nhau. Ngoài ra khá hữu dụng trong việc tô đậm vào ô trả lời trắc nghiệm nhanh nhất.<br>- Thân lục giác, 2B.</p><p><strong>Lưu trữ &amp; bảo quản:</strong></p><p>- Hạn chế rơi nhiều lần,&nbsp;va đập mạnh khi vận chuyển và sử dụng.<br>- Lưu trữ và trưng bày nơi thoáng mát.<br>- Tránh xa nguồn nhiệt và những nơi có ánh nắng trực tiếp chiếu vào.</p>', 0, 1.5, 0, 0, 1),
('PRO014', 'CAT003', N'Máy tính văn phòng Thiên Long Flexio CAL-010', N'<p><strong>Đặc tính sản phẩm:</strong></p><p>- Máy tính văn phòng CAL-010 đa năng này phù hợp sử dụng tại nhà, trường học, văn phòng hoặc cửa hàng. Sự kết hợp chip xử lý và mạch điều khiển công nghệ hiện đại đưa ra những kết quả phép tính đáng tin cậy, nhanh chóng đáp ứng tốt cho mục đích cá nhân hoặc chuyên nghiệp.&nbsp;</p><p>- Ngoài phép tính cơ bản. máy tính văn phòng CAL-010 còn có chức năng tính Thuế (TAX -, TAX+).&nbsp;</p><p>- Kiểu dáng thiết kế hiện đại thân thiện người dùng.&nbsp;</p><p>- Bộ vỏ nguyên liệu ABS cao cấp, bền và sử dụng thiết kế thân thiện, máy tính văn phòng CAL-10 cung cấp sự tiện lợi và hiệu quả trong tất cả các nhu cầu tính toán.</p>', 0, 1.5, 0, 0, 1),
('PRO015', 'CAT003', N'Máy tính văn phòng Thiên Long Flexio CAL-009', N'<p><strong>Đặc tính sản phẩm:</strong></p><p>- Máy tính văn phòng CAL-009 đa năng này phù hợp sử dụng tại nhà, trường học, văn phòng hoặc cửa hàng. Sự kết hợp chip xử lý và mạch điều khiển công nghệ hiện đại đưa ra những kết quả phép tính đáng tin cậy, nhanh chóng đáp ứng tốt cho mục đích cá nhân hoặc chuyên nghiệp.&nbsp;</p><p>- Thiết kế nhỏ gọn và di động, bạn có thể dễ dàng mang theo bất cứ mọi nơi.&nbsp;</p><p>- Bộ vỏ nguyên liệu ABS cao cấp, bền và sử dụng thiết kế thân thiện, máy tính văn phòng CAL-009 cung cấp sự tiện lợi và hiệu quả trong tất cả các nhu cầu tính toán.</p>', 0, 1.5, 0, 0, 1),
('PRO016', 'CAT003', N'Máy tính văn phòng Thiên Long Flexio CAL-008', N'<p><strong>Đặc tính sản phẩm:</strong></p><p>- Máy tính văn phòng CAL-008 đa năng này phù hợp sử dụng tại nhà, trường học, văn phòng hoặc cửa hàng. Sự kết hợp chíp xử lý và mạch điều khiển công nghệ hiện đại đưa ra những kết quả phép tính đáng tin cậy, nhanh chóng đáp ứng tốt cho mục đích cá nhân hoặc chuyên nghiệp.&nbsp;</p><p>- Nhỏ gọn và di động, bạn có thể dễ dàng mang theo bất cứ mọi nơi.&nbsp;</p><p>- Bộ vỏ nguyên liệu ABS cao cấp, bền và sử dụng thiết kế thân thiện, máy tính văn phòng CAL-008 cung cấp sự tiện lợi và hiệu quả trong tất cả các nhu cầu tính toán.</p>', 0, 1.5, 0, 0, 1),
('PRO017', 'CAT003', N'Máy tính văn phòng Thiên Long Flexio CAL-007', N'<p><strong>Đặc tính sản phẩm:</strong></p><p>- Máy tính văn phòng CAL-007 đa năng này phù hợp sử dụng tại nhà, trường học, văn phòng hoặc cửa hàng.&nbsp;</p><p>- Sự kết hợp chip xử lý và mạch điều khiển công nghệ hiện đại đưa ra những kết quả phép tính đáng tin cậy, nhanh chóng đáp ứng tốt cho mục đích cá nhân hoặc chuyên nghiệp.&nbsp;</p><p>- Nhỏ gọn và di động, bạn có thể dễ dàng mang theo bất cứ mọi nơi.&nbsp;</p><p>- Bộ vỏ nguyên liệu ABS cao cấp, bền và sử dụng thiết kế thân thiện, máy tính văn phòng CAL-007 cung cấp sự tiện lợi và hiệu quả trong tất cả các nhu cầu tính toán.</p>', 0, 1.5, 0, 0, 1),
('PRO018', 'CAT003', N'Máy tính Flexoffice FLEXIO CAL-05P', N'<p><strong>Tính năng nổi bật:</strong></p><p>• Mẫu mã đẹp. Sản phẩm được thiết kế nhỏ gọn, vừa tay cầm tạo cảm giác vừa thoải mái vừa chắc chắn và thuận tiện khi sử dụng.</p><p>• Các phím bấm được thiết kế rộng, làm từ nhựa ABS giúp thao tác nhanh và chính xác.</p><p>• Bề mặt kim loại sáng bóng.</p><p>• Màn hình LCD rộng và sáng, hiển thị số rõ nét. Màn hình thiết kế nghiêng dễ nhìn, được làm bằng chất liệu Acrylic, hiển thị 12 chữ số.</p><p>• Bao bì làm bằng chất liệu giấy cao cấp, sang trọng, thiết kế đẹp.</p>', 0, 1.5, 0, 0, 1)

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
('VAL014', 'ATT009', N'AAA (1.55V)'),
('VAL015', 'ATT004', '218 gram'),
('VAL016', 'ATT001', 'Flexoffice'),
('VAL017', 'ATT004', '20 gram'),
('VAL018', 'ATT005', '1.0 mm'),
('VAL019', 'ATT005', '0.6 mm'),
('VAL020', 'ATT005', '0.7 mm'),
('VAL021', 'ATT004', '11 gram'),
('VAL022', 'ATT006', '4B'),
('VAL023', 'ATT006', '3B'),
('VAL024', 'ATT004', '7 gram'),
('VAL025', 'ATT001', N'Bizner'),
('VAL026', 'ATT004', '100 gram'),
('VAL027', 'ATT006', '2B'),
('VAL028', 'ATT004', '123 gram')

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
('PRO003', 'VAL001'),
('PRO004', 'VAL011'),
('PRO004', 'VAL012'),
('PRO004', 'VAL013'),
('PRO004', 'VAL014'),
('PRO004', 'VAL015'),
('PRO005', 'VAL016'),
('PRO005', 'VAL005'),
('PRO005', 'VAL008'),
('PRO006', 'VAL001'),
('PRO006', 'VAL018'),
('PRO006', 'VAL010'),
('PRO006', 'VAL017'),
('PRO007', 'VAL001'),
('PRO007', 'VAL008'),
('PRO007', 'VAL005'),
('PRO007', 'VAL019'),
('PRO008', 'VAL001'),
('PRO008', 'VAL020'),
('PRO008', 'VAL005'),
('PRO008', 'VAL021'),
('PRO009', 'VAL001'),
('PRO009', 'VAL022'),
('PRO009', 'VAL010'),
('PRO009', 'VAL008'),
('PRO010', 'VAL001'),
('PRO010', 'VAL023'),
('PRO010', 'VAL010'),
('PRO010', 'VAL008'),
('PRO011', 'VAL024'),
('PRO011', 'VAL025'),
('PRO011', 'VAL010'),
('PRO012', 'VAL001'),
('PRO012', 'VAL026'),
('PRO012', 'VAL010'),
('PRO013', 'VAL001'),
('PRO013', 'VAL024'),
('PRO013', 'VAL027'),
('PRO013', 'VAL010'),
('PRO014', 'VAL011'),
('PRO014', 'VAL012'),
('PRO014', 'VAL013'),
('PRO014', 'VAL015'),
('PRO015', 'VAL011'),
('PRO015', 'VAL012'),
('PRO015', 'VAL013'),
('PRO015', 'VAL015'),
('PRO016', 'VAL011'),
('PRO016', 'VAL012'),
('PRO016', 'VAL013'),
('PRO016', 'VAL015'),
('PRO016', 'VAL014'),
('PRO017', 'VAL011'),
('PRO017', 'VAL012'),
('PRO017', 'VAL013'),
('PRO017', 'VAL015'),
('PRO017', 'VAL014'),
('PRO018', 'VAL016'),
('PRO018', 'VAL028')

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
('PRO004', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1731055235/sn45tefezfkholxonrip.webp', 0),
('PRO005', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732432392/x1khnbva94fh1lz5fewb.webp', 1),
('PRO005', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732432394/k7dfx7j38sxok2fcelea.webp', 0),
('PRO006', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732434735/quvdfmps9w6ydwwsxmfy.webp', 1),
('PRO006', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732434737/ov3hm1izy6htszwdtoin.webp', 0),
('PRO007', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732435616/qdsbblbee0ceyprmlnox.webp', 1),
('PRO007', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732435620/jbe6oopdhuymwrwumiuq.webp', 0),
('PRO007', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732435622/u4vdoq1ojjt6u9wbohdj.webp', 0),
('PRO008', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732436894/xxmmwse886mssyaeap8b.jpg', 1),
('PRO008', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732436895/sjcuccvck97nua9iczn6.webp', 0),
('PRO008', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732436896/oficgzacncbinwzqqz60.webp', 0),
('PRO009', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732437884/ud7zjvlc7lbdlct3nomj.webp', 1),
('PRO009', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732437886/m8npz5jhnv3ejysegkya.webp', 0),
('PRO010', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732439040/aag5k487gtqutbltzcmk.webp', 1),
('PRO010', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732439041/pwdj13bo3buez84pid0y.webp', 0),
('PRO011', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732447857/u2hhdutjwtkqpjdgzrvi.webp', 1),
('PRO011', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732447858/eylw5u6ulepxqjs85oya.webp', 0),
('PRO012', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732449803/nuznmjusaqvcf3ib2pyk.webp', 1),
('PRO012', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732449804/m4eshjdhemkziankfkav.webp', 0),
('PRO013', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732450879/vv1nuzhyf6yykshtzjgd.webp', 1),
('PRO013', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732450881/on7us2ri5vyey4kjzf5s.webp', 0),
('PRO014', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732452189/nfalxf7n0zuqviiytirw.webp', 1),
('PRO014', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732452193/p1whx9nqr5arwnoafhmd.jpg', 0),
('PRO015', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732452947/ikmsxjxhpaabbx7s55co.webp', 1),
('PRO015', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732452948/k9dexvwyimrzsxc8y3tc.webp', 0),
('PRO016', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732454001/lrtc5saja3q541ebagtw.webp', 1),
('PRO016', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732454003/vzwkhdlz61tjaxo5pbs8.webp', 0),
('PRO017', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732456670/zc8abah7azzsmmrc2kt2.webp', 1),
('PRO017', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732456672/xpcppsc7khf84ptzpief.webp', 0),
('PRO018', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732456991/nyr8dca5uxtsjvtnw3nj.webp', 1),
('PRO018', 'https://res.cloudinary.com/dvpzullxc/image/upload/v1732456992/j1kx8dios3e3dsrseuph.webp', 0)

INSERT INTO roles (role_name, description)
VALUES
(N'Khách hàng', N'Khách mua hàng'),
(N'Quản lý', N'Quản lý toàn bộ hệ thống'),
(N'Nhân viên bán hàng', N'Nhân viên bán hàng');

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

INSERT INTO users (user_id, full_name, username, status)
VALUES
('USER001', N'Thành Đạt', 'qwe', 'true')

INSERT INTO user_roles
VALUES
('USER001', 2)
GO

UPDATE users
SET password = CONVERT(VARCHAR(32), HASHBYTES('MD5', '123'), 2)
WHERE username = 'qwe';

INSERT INTO purchase_order (purchase_order_id, supplier_id, employee_id)
VALUES
('POD001', 'SUP001', 'USER001'),
('POD002', 'SUP002', 'USER001')

INSERT INTO purchase_order_detail
VALUES
('POD001', 'PRO001', 100, 4500, 0),
('POD001', 'PRO002', 80, 4000, 0),
('POD001', 'PRO003', 70, 110000, 0),
('POD001', 'PRO004', 60, 95000, 0),
('POD001', 'PRO005', 50, 80000, 0),
('POD002', 'PRO006', 40, 400000, 0),
('POD002', 'PRO007', 50, 60000, 0),
('POD002', 'PRO008', 60, 200000, 0),
('POD002', 'PRO009', 60, 100000, 0),
('POD002', 'PRO010', 60, 100000, 0),
('POD002', 'PRO011', 60, 150000, 0),
('POD002', 'PRO012', 60, 260000, 0),
('POD002', 'PRO013', 60, 70000, 0),
('POD002', 'PRO014', 60, 160000, 0),
('POD002', 'PRO015', 60, 70000, 0),
('POD002', 'PRO016', 60, 100000, 0),
('POD002', 'PRO017', 60, 90000, 0),
('POD002', 'PRO018', 60, 160000, 0)

INSERT INTO receipts (receipt_id, purchase_order_id, entry_count)
VALUES 
('REC001', 'POD001', 1),
('REC002', 'POD002', 1),
('REC003', 'POD002', 2)

INSERT INTO receipt_details
VALUES
('REC001', 'POD001', 'PRO001', 100),
('REC001', 'POD001', 'PRO002', 80),
('REC001', 'POD001', 'PRO003', 70),
('REC001', 'POD001', 'PRO004', 60),
('REC001', 'POD001', 'PRO005', 50),
('REC002', 'POD002', 'PRO006', 20),
('REC002', 'POD002', 'PRO007', 30),
('REC002', 'POD002', 'PRO008', 30),
('REC002', 'POD002', 'PRO009', 30),
('REC002', 'POD002', 'PRO010', 30),
('REC002', 'POD002', 'PRO011', 30),
('REC002', 'POD002', 'PRO012', 30),
('REC002', 'POD002', 'PRO013', 30),
('REC002', 'POD002', 'PRO014', 30),
('REC002', 'POD002', 'PRO015', 30),
('REC002', 'POD002', 'PRO016', 30),
('REC002', 'POD002', 'PRO017', 30),
('REC002', 'POD002', 'PRO018', 30)

INSERT INTO receipt_details
VALUES
('REC003', 'POD002', 'PRO006', 20),
('REC003', 'POD002', 'PRO007', 20),
('REC003', 'POD002', 'PRO008', 30),
('REC003', 'POD002', 'PRO009', 30),
('REC003', 'POD002', 'PRO010', 30),
('REC003', 'POD002', 'PRO011', 30),
('REC003', 'POD002', 'PRO012', 30),
('REC003', 'POD002', 'PRO013', 30),
('REC003', 'POD002', 'PRO014', 30),
('REC003', 'POD002', 'PRO015', 30),
('REC003', 'POD002', 'PRO016', 30),
('REC003', 'POD002', 'PRO017', 30),
('REC003', 'POD002', 'PRO018', 30)