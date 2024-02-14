USE master;
GO
DROP DATABASE IF EXISTS InternalProject;
GO
CREATE DATABASE InternalProject;
GO
USE InternalProject;
GO
DROP TABLE IF EXISTS Review;
DROP TABLE IF EXISTS [User];
DROP TABLE IF EXISTS UserType;
DROP TABLE IF EXISTS OrderDetail;
DROP TABLE IF EXISTS ProductSku
DROP TABLE IF EXISTS Product
DROP TABLE IF EXISTS [Order];
DROP TABLE IF EXISTS OrderStatus;
DROP TABLE IF EXISTS Discount;
GO

------------------------------------------
--Create tables---------------------------
------------------------------------------
CREATE TABLE Discount(
    pkDiscountCode VARCHAR(15) PRIMARY KEY,
    discount FLOAT NOT NULL,
)

CREATE TABLE OrderStatus(
    pkOrderStatusId INT IDENTITY(1,1) PRIMARY KEY,
    orderStatus VARCHAR(20) NOT NULL,
    CHECK (orderStatus IN ('Pending','Paid','Shipped','Delivered'))
)

CREATE TABLE UserType(
    pkUserTypeId INT IDENTITY(1,1) PRIMARY KEY,
    userType VARCHAR(10) NOT NULL,
    CHECK (userType IN ('Registered','Admin'))
)

CREATE TABLE [User](
    pkUserId VARCHAR(30) PRIMARY KEY,
    fkUserTypeId INT,
    firstName VARCHAR(20) NOT NULL,
    lastName VARCHAR(20) NOT NULL,
    address VARCHAR(20) NOT NULL,
    address2 VARCHAR(20),
    city VARCHAR(20) NOT NULL,
    province VARCHAR(20) NOT NULL,
    country VARCHAR(20) NOT NULL,
    postalCode VARCHAR(10) NOT NULL,
    phoneNumber VARCHAR(20) NOT NULL,
    CONSTRAINT UserUserTypeFK 
        FOREIGN KEY(fkUserTypeId) REFERENCES UserType(pkUserTypeId)
        ON UPDATE CASCADE ON DELETE CASCADE
)

CREATE TABLE [Order](
    pkOrderId INT IDENTITY(1,1) PRIMARY KEY,
    fkUserId VARCHAR(30),
    fkOrderStatusId INT NOT NULL,
    fkDiscountCode VARCHAR(15),
    transactionId VARCHAR(30),
    buyerNote VARCHAR(100),
    orderDate Date NOT NULL,
    CONSTRAINT OrderOrderStatusFK 
        FOREIGN KEY(fkOrderStatusId) REFERENCES OrderStatus(pkOrderStatusId)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT OrderDiscountFK 
        FOREIGN KEY (fkDiscountCode) REFERENCES Discount(pkDiscountCode)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT OrderUserFK 
        FOREIGN KEY (fkUserId) REFERENCES [User](pkUserId)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT TransactionIdUnique UNIQUE(transactionId)
)

CREATE TABLE Product(
    pkProductId INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(30) NOT NULL,
    price FLOAT NOT NULL,
    description VARCHAR(200) NOT NULL,
    isActive Char(1) NOT NULL,
    image BINARY
)

CREATE TABLE ProductSku(
    pkSkuId INT IDENTITY(1,1) PRIMARY KEY,
    fKProductId INT,
    size VARCHAR(4) NOT NULL,
    CONSTRAINT ProductSkuProductFK
        FOREIGN KEY(fkProductId) REFERENCES Product(pkProductId)
        ON UPDATE CASCADE ON DELETE CASCADE
)

CREATE TABLE OrderDetail(
    fkOrderId INT,
    fkSkuId INT,
    quantity INT NOT NULL,
    PRIMARY KEY (fkOrderId,fkSkuId),
    CONSTRAINT OrderDetailOrderFK 
        FOREIGN KEY(fkOrderId) REFERENCES [Order](pkOrderId)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT OrderDetailProductSkuFK 
        FOREIGN KEY(fkSkuId) REFERENCES ProductSku(pkSkuId)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CHECK (quantity BETWEEN 0 AND 99)
)

CREATE TABLE Review(
    fkUserId VARCHAR(30),
    fkProductId INT,
    rating INT NOT NULL,
    comment VARCHAR(100),
    PRIMARY KEY (fkUserId,fkProductID),
    CONSTRAINT ReviewUserFK 
        FOREIGN KEY(fkUserId) REFERENCES [User](pkUserId)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT ReviewProductFK 
        FOREIGN KEY(fkProductId) REFERENCES Product(pkProductId)
        ON UPDATE CASCADE ON DELETE CASCADE,
    CHECK (rating BETWEEN 1 AND 5)
)
