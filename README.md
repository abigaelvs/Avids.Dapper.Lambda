Avids.Dapper.Lambda - a Lambda Dapper Extension
========================================

This is a Dapper Extension, supporting lambda expression, with chain style which allows developers to use more elegantly.

Install
------------
```
PM> Install-Package Avids.Dapper.Lambda
```

Features
---------
### 1.Base On Dapper

Avids.Dapper.Lambda is a library hosted in nuget. Right now it can be used on dotnet 5+.
More target frameworks will be added later (such as: net461, netstandard2.1)


The lambda expression encapsulation based on dapper is still an extension of `IDbConnection`Interface, and retains and opens the original `Execute`, `Query`, etc

### 2.Elegant Chain

#### Query
```c#
con.QuerySet<SysUser>().Where(a => a.Email == "287245177@qq.com")
                       .OrderBy(a => a.CreateDatetime)
                       .Select(a => new SysUser { Email = a.Email, CreateDatetime = a.CreateDatetime, SysUserid = a.SysUserid })
                       .PageList(1, 10);
```

#### Command
```c#
con.CommandSet<SysUser>().Where(a => a.Email == "287245177@qq.com").Update(a => new SysUser { Email = "123456789@qq.com" });
```
#### ExpressionBuilder
-----------------
```c#
var where = ExpressionBuilder.Init<SysUser>();

if (string.IsNullOrWhiteSpace(param.Email))
    where = where.And(a => a.Email == "287245177@qq.com");

if (string.IsNullOrWhiteSpace(param.Mobile))
    where = where.And(a => a.Mobile == "18988565556");

con.QuerySet<SysUser>().Where(where).OrderBy(b => b.Email).Top(10).Select(a => a.Email).ToList();
```
### 3.Support Async
```c#
ToListAsync
GetAsync
InsertAsync
DeleteAsync
UpdateSelectAsync
UpdateAsync
```
### 4.Faithful To Native Attribute
```c#
[Table("SYS_USER")]
[Key]
[Required]
[StringLength(32)]
[Display(Name = "??")]
[Column("SYS_USERID")]
[DatabaseGenerated]
```

Contribution
-------
Welcome to submit Pull Request for code changes. If you have any questions, you can open an issue for further discussion.

License
-------
[MIT](https://github.com/abigaelvs/Avids.Dapper.Lambda/blob/master/LICENSE)