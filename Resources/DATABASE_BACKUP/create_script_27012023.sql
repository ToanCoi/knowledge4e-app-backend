-- 
-- Set character set the client will use to send SQL statements to the server
--
SET NAMES 'utf8';

--
-- Set default database
--
USE katestappa_library;

--
-- Create table `category`
--
CREATE TABLE category (
  CategoryID char(36) NOT NULL DEFAULT '',
  Title varchar(255) DEFAULT NULL,
  ParentID char(36) DEFAULT NULL,
  Note varchar(255) DEFAULT NULL,
  CreatedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  CreatedBy varchar(255) DEFAULT NULL,
  ModifiedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  ModifiedBy varchar(255) DEFAULT NULL,
  Status bit(1) DEFAULT b'1',
  IsDeleted bit(1) DEFAULT b'0',
  PRIMARY KEY (CategoryID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 4096,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

--
-- Create table `book`
--
CREATE TABLE book (
  BookID char(36) NOT NULL DEFAULT '',
  CategoryID char(36) DEFAULT NULL,
  BookCode char(20) DEFAULT NULL,
  BookName varchar(255) DEFAULT NULL,
  Publisher varchar(100) DEFAULT NULL,
  IsPrivate bit(1) DEFAULT NULL,
  Author varchar(100) DEFAULT NULL,
  LanguageCode char(10) DEFAULT NULL,
  Price decimal(15, 2) DEFAULT NULL,
  Description text DEFAULT NULL,
  BookFormat int(11) DEFAULT NULL,
  Image text DEFAULT NULL,
  File text DEFAULT NULL,
  BookType int(11) NOT NULL DEFAULT 0 COMMENT '0-giáo trình,1-sách tham khảo',
  Available int(11) NOT NULL DEFAULT 0,
  Loaned int(11) NOT NULL DEFAULT 0,
  Reserved int(11) NOT NULL DEFAULT 0,
  Amount int(11) NOT NULL DEFAULT 0,
  PublicationDate timestamp NULL DEFAULT NULL,
  CreatedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  CreatedBy varchar(255) DEFAULT NULL,
  ModifiedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  ModifiedBy varchar(255) DEFAULT NULL,
  Status bit(1) DEFAULT b'0',
  IsDeleted bit(1) DEFAULT b'0',
  PRIMARY KEY (BookID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 16384,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

--
-- Create foreign key
--
ALTER TABLE book
ADD CONSTRAINT FK_book_category_CategoryID FOREIGN KEY (CategoryID)
REFERENCES category (CategoryID) ON DELETE SET NULL ON UPDATE NO ACTION;

--
-- Create table `role`
--
CREATE TABLE role (
  RoleID char(36) NOT NULL DEFAULT 'UUID',
  RoleName varchar(255) DEFAULT NULL,
  RoleType int(11) NOT NULL,
  Description varchar(255) NOT NULL DEFAULT '',
  IsDeleted bit(1) DEFAULT b'0',
  PRIMARY KEY (RoleID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 4096,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

--
-- Create table `account`
--
CREATE TABLE account (
  AccountID char(36) NOT NULL DEFAULT '',
  UserName varchar(255) NOT NULL,
  Email varchar(100) DEFAULT NULL,
  Address varchar(255) DEFAULT NULL,
  PhoneNumber varchar(100) DEFAULT NULL,
  FullName varchar(255) DEFAULT NULL,
  Password varchar(255) DEFAULT '',
  Avatar text DEFAULT NULL,
  CreatedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  CreatedBy varchar(255) DEFAULT NULL,
  ModifiedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  ModifiedBy varchar(255) DEFAULT NULL,
  Status int(11) DEFAULT NULL,
  IsDeleted bit(1) DEFAULT b'0',
  PRIMARY KEY (AccountID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 3640,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

--
-- Create table `notification`
--
CREATE TABLE notification (
  NotificationID char(36) NOT NULL DEFAULT '',
  Content varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `From` char(36) DEFAULT NULL,
  `To` char(36) DEFAULT NULL,
  ToEmail varchar(255) DEFAULT NULL,
  IsReaded bit(1) DEFAULT b'1',
  CreatedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  CreatedBy varchar(255) DEFAULT NULL,
  ModifiedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  ModifiedBy varchar(255) DEFAULT NULL,
  IsDeleted bit(1) DEFAULT b'0',
  Status bit(1) DEFAULT b'1',
  PRIMARY KEY (NotificationID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 315,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

--
-- Create foreign key
--
ALTER TABLE notification
ADD CONSTRAINT FK_notification_account_AccountID FOREIGN KEY (`From`)
REFERENCES account (AccountID) ON DELETE SET NULL ON UPDATE NO ACTION;

--
-- Create table `book_order`
--
CREATE TABLE book_order (
  BookOrderID char(36) NOT NULL DEFAULT '',
  BookOrderCode char(20) DEFAULT NULL,
  AccountID char(36) DEFAULT NULL,
  BookOrderInformation longtext DEFAULT NULL,
  Note varchar(255) DEFAULT NULL,
  OrderStatus int(11) DEFAULT NULL,
  FromDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  DueDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  CreatedBy varchar(255) DEFAULT NULL,
  CreatedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  ModifiedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  ModifiedBy varchar(255) DEFAULT NULL,
  Status bit(1) DEFAULT b'0',
  IsDeleted bit(1) DEFAULT b'0',
  PRIMARY KEY (BookOrderID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 1365,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

--
-- Create foreign key
--
ALTER TABLE book_order
ADD CONSTRAINT FK_book_order_account_AccountID FOREIGN KEY (AccountID)
REFERENCES account (AccountID) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Create table `account_roles`
--
CREATE TABLE account_roles (
  AccountRoleID char(36) NOT NULL DEFAULT 'UUID',
  AccountID char(36) NOT NULL DEFAULT 'UUID',
  RoleID char(36) NOT NULL DEFAULT 'UUID',
  IsDeleted bit(1) DEFAULT b'0',
  PRIMARY KEY (AccountRoleID, AccountID, RoleID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 8192,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

--
-- Create foreign key
--
ALTER TABLE account_roles
ADD CONSTRAINT FK_account_roles_account_AccountID FOREIGN KEY (AccountID)
REFERENCES account (AccountID) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Create foreign key
--
ALTER TABLE account_roles
ADD CONSTRAINT FK_account_roles_role_RoleID FOREIGN KEY (RoleID)
REFERENCES role (RoleID) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Create table `technology_introduction`
--
CREATE TABLE technology_introduction (
  TechID char(36) NOT NULL DEFAULT 'UUID',
  Image varchar(255) DEFAULT NULL,
  Content text DEFAULT NULL,
  IsShow bit(1) DEFAULT NULL,
  `Order` int(11) DEFAULT 0,
  Title text DEFAULT NULL,
  CreatedDate timestamp NULL DEFAULT NULL,
  ModifiedDate timestamp NULL DEFAULT NULL,
  PRIMARY KEY (TechID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 16384,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

--
-- Create index `IDX_technology_introduction_title` on table `technology_introduction`
--
ALTER TABLE technology_introduction
ADD INDEX IDX_technology_introduction_title (Title (30));

--
-- Create table `safe_address`
--
CREATE TABLE safe_address (
  SafeAddressID char(36) NOT NULL DEFAULT 'UUID',
  SafeAddressValue varchar(255) DEFAULT NULL,
  Type int(11) DEFAULT NULL,
  DeviceName varchar(255) DEFAULT NULL,
  DeviceCode varchar(255) DEFAULT NULL,
  CreatedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  CreatedBy varchar(255) DEFAULT NULL,
  ModifiedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  ModifiedBy varchar(255) DEFAULT NULL,
  Status bit(1) DEFAULT b'1',
  IsDeleted bit(1) DEFAULT b'0',
  PRIMARY KEY (SafeAddressID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 2730,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

--
-- Create table `post`
--
CREATE TABLE post (
  PostID char(36) NOT NULL DEFAULT 'UUID',
  MenuID char(36) NOT NULL,
  Title varchar(255) DEFAULT NULL,
  Slug varchar(255) DEFAULT NULL,
  Description text DEFAULT NULL,
  Content longtext DEFAULT NULL,
  Image text DEFAULT NULL,
  ViewCount int(11) DEFAULT NULL,
  Type int(11) DEFAULT NULL,
  CreatedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  CreatedBy varchar(255) DEFAULT NULL,
  ModifiedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  ModifiedBy varchar(255) DEFAULT NULL,
  Status bit(1) DEFAULT b'1',
  IsDeleted bit(1) DEFAULT b'0',
  Price varchar(255) DEFAULT NULL,
  PRIMARY KEY (PostID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 862,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

--
-- Create table `partner`
--
CREATE TABLE partner (
  PartnerID char(36) NOT NULL DEFAULT 'UUID',
  Image varchar(255) DEFAULT NULL,
  Description varchar(500) NOT NULL DEFAULT '',
  PRIMARY KEY (PartnerID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 2730,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

--
-- Create table `menu`
--
CREATE TABLE menu (
  MenuID char(36) NOT NULL DEFAULT 'UUID',
  Title varchar(255) DEFAULT NULL,
  Slug varchar(255) DEFAULT NULL,
  ParentID char(36) DEFAULT '00000000-0000-0000-0000-000000000000',
  IsShowHome bit(1) DEFAULT b'1',
  Link text DEFAULT NULL,
  DisplayOrder int(11) DEFAULT NULL,
  Type int(11) DEFAULT NULL,
  CreatedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  CreatedBy varchar(255) DEFAULT NULL,
  ModifiedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  ModifiedBy varchar(255) DEFAULT NULL,
  Status bit(1) DEFAULT b'1',
  IsDeleted bit(1) DEFAULT b'0',
  IsPrivate bit(1) DEFAULT NULL,
  PRIMARY KEY (MenuID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 1560,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

--
-- Create table `image_manager`
--
CREATE TABLE image_manager (
  ImageID char(36) NOT NULL DEFAULT 'UUID',
  ImageName varchar(255) NOT NULL,
  Url text DEFAULT NULL,
  FolderID char(36) DEFAULT 'UUID',
  CreateDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  CreateBy varchar(255) DEFAULT NULL,
  ModifiedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  ModifiedBy varchar(255) DEFAULT NULL,
  FromFile varbinary(255) DEFAULT NULL
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 16384,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

--
-- Create index `UK_image_manager_ImageName` on table `image_manager`
--
ALTER TABLE image_manager
ADD UNIQUE INDEX UK_image_manager_ImageName (ImageName);

--
-- Create table `html_section`
--
CREATE TABLE html_section (
  SectionID char(36) NOT NULL DEFAULT 'UUID',
  Content text DEFAULT NULL,
  HtmlSectionType int(11) DEFAULT NULL,
  PRIMARY KEY (SectionID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 16384,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

--
-- Create table `folder`
--
CREATE TABLE folder (
  FolderID char(36) NOT NULL DEFAULT 'UUID',
  FolderName varchar(255) NOT NULL,
  ParentID char(36) DEFAULT NULL,
  ModuleType int(11) DEFAULT 0,
  CreatedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  CreatedBy varchar(255) DEFAULT NULL,
  ModifiedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  ModifiedBy varchar(255) DEFAULT NULL,
  IsDeleted bit(1) DEFAULT b'0',
  Status bit(1) DEFAULT NULL,
  PRIMARY KEY (FolderID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 4096,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci,
COMMENT = 'bảng quản lý folder';

--
-- Create table `contact_submit`
--
CREATE TABLE contact_submit (
  ContactSubmitID char(36) NOT NULL DEFAULT 'UUID',
  Content text DEFAULT 'UUID',
  Email varchar(50) DEFAULT NULL,
  PhoneNumber varchar(50) DEFAULT 'current_timestamp()',
  Company varchar(255) DEFAULT 'current_timestamp()',
  CreatedDate timestamp NULL DEFAULT CURRENT_TIMESTAMP(),
  PRIMARY KEY (ContactSubmitID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 10922,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;

--
-- Create table `carousel`
--
CREATE TABLE carousel (
  CarouselID char(36) NOT NULL DEFAULT 'UUID',
  CarouselSection int(11) DEFAULT NULL,
  CarouselContent text DEFAULT NULL,
  CarouselText text DEFAULT NULL,
  IsShow bit(1) DEFAULT b'1',
  PRIMARY KEY (CarouselID)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 16384,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_general_ci;