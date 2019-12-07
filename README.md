This is a library for inspecting relational database schemas that support `IDbConnection` so that you can present database objects in a UI of your choice. This is part of the internal tooling for a couple projects of mine [SqlChartify](https://sqlchartify.azurewebsites.net/) and [Postulate Query Helper](https://github.com/adamosoftware/Postulate.Zinger).

Nuget package: **SqlSchema.SqlServer**

This is currently in a pre-release state with minimal functionality at the moment. Currently, only [tables and foreign keys](https://github.com/adamosoftware/SqlSchema/blob/master/SqlSchema.SqlServer/SqlServerAnalyzer.cs#L12) are inspected.

Please check out the [unit tests](https://github.com/adamosoftware/SqlSchema/blob/master/Testing/SqlServer.cs) to get a sense of what this does.

I'm planning for this to target multiple database platforms (MySQL, Postgres), but I'm focusing exclusively on SQL Server for now since that's where I spend all my time, really.
