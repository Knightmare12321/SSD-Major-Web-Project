USE InternalProject;
GO

------------------------------------------
--Insert Data-----------------------------
------------------------------------------
INSERT INTO Discount (pkDiscountCode,discount)
VALUES ('BLACKFRIDAY15',0.15);
INSERT INTO Discount (pkDiscountCode,discount)
VALUES ('FLASH10',0.1);
INSERT INTO Discount (pkDiscountCode,discount)
VALUES ('BOXINGDAY',0.2);

INSERT INTO OrderStatus (orderStatus)
VALUES ('Pending');
INSERT INTO OrderStatus (orderStatus)
VALUES ('Paid');
INSERT INTO OrderStatus (orderStatus)
VALUES ('Shipped');
INSERT INTO OrderStatus (orderStatus)
VALUES ('Delivered');

INSERT INTO UserType(userType)
VALUES('Registered');
INSERT INTO UserType(userType)
VALUES('Admin');

INSERT INTO [User](pkUserId,fkUserTypeId,firstName,lastName,address,address2,city,province,country,postalCode,phoneNumber)
VALUES('alex@gmail.com',1,'Alex','Doe','123 Maple St.','Unit 456','Vancouver','BC', 'Canada','A1B 2C3','123-456-7890');
INSERT INTO [User](pkUserId,fkUserTypeId,firstName,lastName,address,city,province,country,postalCode,phoneNumber)
VALUES('amanda@gmail.com',2,'Amanda','Doe','111 Nut St.','Vancouver','BC', 'Canada','A1A 1A1','123-456-7890');
INSERT INTO [User](pkUserId,firstName,lastName,address,city,province,country,postalCode,phoneNumber)
VALUES('kenny@gmail.com','Kenny','Doe','111 Cashew Ave.','Seatle','WA', 'United State','12345-6789','1112223333');
INSERT INTO [User](pkUserId,firstName,lastName,address,city,province,country,postalCode,phoneNumber)
VALUES('charlotte@gmail.com','Charlotte','Doe','111 Tims Ave.','London','England', 'United Kingdom','T2X 2B3','+44(123)-456-7890');

INSERT INTO [Order](fkUserId,fkOrderStatusId,orderDate)
VALUES ('alex@gmail.com',1,'2023-12-08');
INSERT INTO [Order](fkUserId,fkOrderStatusId,fkDiscountCode,transactionId,buyerNote,orderDate)
VALUES ('amanda@gmail.com',2,'BLACKFRIDAY15','XDFLJWOFNM','','2023-12-08');
INSERT INTO [Order](fkUserId,fkOrderStatusId,transactionId,buyerNote,orderDate)
VALUES ('kenny@gmail.com',1,'JKLWOMZUENG','Please ship asap','2023-12-09');
INSERT INTO [Order](fkUserId,fkOrderStatusId,fkDiscountCode,transactionId,buyerNote,orderDate)
VALUES ('charlotte@gmail.com',4,'BOXINGDAY','WWMZOVNQOJ','Please include a gift receipt. This is a present for my daughter. Thank you!!!','2023-12-08');

INSERT INTO Product(name,price,description,isActive)
VALUES('Top 1',89.99,'Our most environmentally friendly top made with 100% post-consumer cottons.',1);
INSERT INTO Product(name,price,description,isActive)
VALUES('Top 2',69.99,'Affordable premium top made with long strand cottons',1);
INSERT INTO Product(name,price,description,isActive)
VALUES('Top 3',59.99,'it is a good jacket',0);
INSERT INTO Product(name,price,description,isActive)
VALUES('Top 4',39.99,'Great jacket with an amazing price tag',1);

INSERT INTO ProductSku(fKProductId,size)
VALUES(1,'S');
INSERT INTO ProductSku(fKProductId,size)
VALUES(1,'M');
INSERT INTO ProductSku(fKProductId,size)
VALUES(1,'L');
INSERT INTO ProductSku(fKProductId,size)
VALUES(2,'S');
INSERT INTO ProductSku(fKProductId,size)
VALUES(2,'M');
INSERT INTO ProductSku(fKProductId,size)
VALUES(2,'L');
INSERT INTO ProductSku(fKProductId,size)
VALUES(3,'S');
INSERT INTO ProductSku(fKProductId,size)
VALUES(3,'M');
INSERT INTO ProductSku(fKProductId,size)
VALUES(3,'L');
INSERT INTO ProductSku(fKProductId,size)
VALUES(4,'S');
INSERT INTO ProductSku(fKProductId,size)
VALUES(4,'M');
INSERT INTO ProductSku(fKProductId,size)
VALUES(4,'L');

INSERT INTO OrderDetail(fkOrderId,fkSkuId,quantity)
VALUES(1,1,1);
INSERT INTO OrderDetail(fkOrderId,fkSkuId,quantity)
VALUES(2,1,1);
INSERT INTO OrderDetail(fkOrderId,fkSkuId,quantity)
VALUES(2,2,3);
INSERT INTO OrderDetail(fkOrderId,fkSkuId,quantity)
VALUES(3,3,1);
INSERT INTO OrderDetail(fkOrderId,fkSkuId,quantity)
VALUES(4,4,2);

INSERT INTO Review(fkUserId,fkProductId,rating,comment)
VALUES('amanda@gmail.com',2,3,'Best shirt ever!');
INSERT INTO Review(fkUserId,fkProductId,rating,comment)
VALUES('alex@gmail.com',1,5,'A very nice top');
INSERT INTO Review(fkUserId,fkProductId,rating,comment)
VALUES('kenny@gmail.com',1,4,'quality is okay');
INSERT INTO Review(fkUserId,fkProductId,rating,comment)
VALUES('charlotte@gmail.com',4,3,'quality is poor. I rather spent it on Tim Horton');


USE master;
GO
