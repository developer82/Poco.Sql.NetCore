# Poco.Sql
##### Auto generate SQL statements from .NET objects

## Quick Start

Include the Poco.Sql extentions method namespace

```
using Poco.Sql.Extensions;
```

Generate SQL statements from ANY object

```
myObject.PocoSql().Select().ToString();
myObject.PocoSql().Insert().ToString();
myObject.PocoSql().Update().ToString();
myObject.PocoSql().Delete().ToString();
```

[Read the Quick Start guide](https://github.com/developer82/Poco.Sql/wiki/Quick-Start)

**It's that simple!**

## Configuration
Confgiration can be executed only once in the application lifecycle. Once configured, it doesn't allow changing the configuration later on.

Example configuration:
```
// PocoSql configuration
Configuration.Initialize(config =>
{
  config.PluralizeTableNames();
  config.SetStoreProcedurePrefix("stp_");
  config.ShowComments();
  config.InjectValuesInQueies();
  config.SelectFullGraph();
  config.AddMap(new EntityMap());
});
```
[Read more about Poco.Sql configuration](https://github.com/developer82/Poco.Sql/wiki/Configuration)

## Object Mapping

Poco.Sql works out-of-the-box on any .NET class and without the need to configure or map anything. However, there might be cases where we would like to apply mapping on objects - map relationships between objects, map object properties to database fields, ignore properties, set custom table name for object, define stored procedures, and more...

To allow such mapping, Poco.Sql has a mapping object that helps you define mappings between your objects and the database.

To create a mapping object, create a new class that inherits from `Poco.Sql.PocoSqlMapping<T>`

```
public class UserMap : PocoSqlMapping<User>
{
    public UserMap()
    {
        // mapping configuration
    }
}
```

[Read more about Poco.Sql mappings](https://github.com/developer82/Poco.Sql/wiki/Object-Mapping)

## Values Object Creator

To save the time and lines of code in writing these objects, Poco.Sql has an helper class that knows how to create an object that's combined of your domain objects, and is specific to SQL statement.

```
var sql = "insert into Users(FirstName, LastName, DateOfBirth, LuckyNumber, IsRegistered) values(@FirstName, @LastName, @DateOfBirth, @LuckyNumber, @IsRegistered)";
var data = ValuesObject.Create(sql, myUserObject /* , ... */);
```

[Read more about Poco.Sql Values Object Creator](https://github.com/developer82/Poco.Sql/wiki/Values-Object-Creator)

## Further Reading

You can have more information about this project (and other stuff I do) on my blog at http://developer82.webe.co.il
I'm also on twitter! follow me [@supervill_dev82](https://twitter.com/supervill_dev82)

## License

The MIT License (MIT)

Copyright (c) 2014 Ophir Oren

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
