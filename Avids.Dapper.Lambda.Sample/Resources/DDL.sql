create table "Invoice" (
   "Id"							bigint							GENERATED ALWAYS AS IDENTITY NOT NULL,
   "No"							varchar(10)                     NOT NULL,
   "StatusId"					bigint                          NOT NULL,
   "PaymentStatusId"			bigint                          NOT NULL,
   "CashierId"					bigint                          NOT NULL,
   "UpdatedByUserId"			bigint                          NOT NULL,
   "CustomerId"					bigint                          NOT NULL,
   "CreatedDate"				timestamp without time zone     NOT NULL,
   "UpdatedDate"				timestamp without time zone		NULL,
   constraint PK_Invoice primary key ("Id")
);

create table "InvoiceStatus" (
   "Id"							bigint							NOT NULL,
   "Name"						varchar(10)                     NOT NULL,
   constraint PK_InvoiceStatus primary key ("Id")
);

create table "PaymentStatus" (
   "Id"							bigint							NOT NULL,
   "Name"						varchar(10)                     NOT NULL,
   constraint PK_PaymentStatus primary key ("Id")
);

create table "Cashier" (
   "Id"							bigint							GENERATED ALWAYS AS IDENTITY NOT NULL,
   "Name"						varchar(10)                     NOT NULL,
   "UserId"						bigint							NOT NULL,
   constraint PK_Cashier primary key ("Id")
);

create table "Customer" (
   "Id"							bigint							GENERATED ALWAYS AS IDENTITY NOT NULL,
   "Name"						varchar(10)                     NOT NULL,
   "UserId"						bigint							NOT NULL,
   constraint PK_Customer primary key ("Id")
);

create table "User" (
   "Id"							bigint							GENERATED ALWAYS AS IDENTITY NOT NULL,
   "Name"						varchar(10)                     NOT NULL,
   constraint PK_User primary key ("Id")
);