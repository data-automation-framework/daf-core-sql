# Data Automation Framework - SQL Server plugin (Daf Sql)
**Note: This project is currently in an alpha state and should be considered unstable. Breaking changes to the public API will occur.**

Daf is a plugin-based data and integration automation framework primarily designed to facilitate data warehouse and ETL processes. Developers use this framework to programatically generate data integration objects using the Daf templating language.

This Daf plugin allow users to generate Microsoft SQL Server database definition files (tables, views, stored procedures etc).

## Installation
In the daf project file add a new ItemGroup containing a nuget package reference to the plugin:
```
<ItemGroup>
  <PackageReference Include="Daf.Core.Sql" Version="*" />
<ItemGroup>
```

## Usage
The root node of the SQL plugin is _Sql_. This root node must start on the first column in the daf template file.

Use <# #> to inject C# code, <#= #> to get variable string values from the C# code:
![Sql](https://user-images.githubusercontent.com/1073539/113327548-656d6400-931b-11eb-8be0-10fb4ecea378.png)

## Links
[Daf organization](https://github.com/data-automation-framework)

[Documentation](https://data-automation-framework.com/)
