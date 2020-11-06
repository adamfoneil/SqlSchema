[![Nuget](https://img.shields.io/nuget/v/SqlSchema.SqlServer)](https://www.nuget.org/packages/SqlSchema.SqlServer/)

This is a library for inspecting relational database schemas that support `IDbConnection` so that you can present database objects in a UI of your choice. This is part of the internal tooling for a couple projects of mine [SqlChartify](https://sqlchartify.azurewebsites.net/) and [Postulate Query Helper](https://github.com/adamosoftware/Postulate.Zinger). For example, this library powers this UI in Zinger:

![img](https://adamosoftware.blob.core.windows.net:443/images/sqlschema.png)

This UI is populated in this [LoadObjects](https://github.com/adamfoneil/Postulate.Zinger/blob/master/Zinger/Controls/SchemaBrowser.cs#L87) method. The database objects themselves are retrieved in this short [block](https://github.com/adamfoneil/Postulate.Zinger/blob/master/Zinger/Controls/SchemaBrowser.cs#L77):

```csharp
private async Task RefreshAsync()
{
    using (var cn = _getConnection.Invoke())
    {
        _objects = await Analyzers[_providerType].GetDbObjectsAsync(cn);
    }

    LoadObjects();
}
```

Please check out the [unit tests](https://github.com/adamosoftware/SqlSchema/blob/master/Testing/SqlServer.cs) to get a sense of what this does.

I'm planning for this to target multiple database platforms (MySQL, PostgreSQL), but I'm focusing exclusively on SQL Server for now since that's where I spend all my time, really.
