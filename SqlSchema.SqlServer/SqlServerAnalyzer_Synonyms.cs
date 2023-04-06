using Dapper;
using SqlSchema.Library.Models;
using SqlSchema.SqlServer.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SqlSchema.SqlServer
{
    public partial class SqlServerAnalyzer
    {
        private async Task AddSynonymsAsync(IDbConnection connection, List<DbObject> results)
        {
            // splits the name parts of referenced objects,
            // so I can later get the schema names of thos objects since SCHEMA_NAME() function doesn't work on objects in other databases
            var referencedNameParts = (await connection.QueryAsync<ObjectNamePart>(
                @"SELECT
	                [syn].[object_id] AS [ObjectId],
                    OBJECT_ID([syn].[base_object_name]) AS [ReferencedObjectId],
	                [p].[value] AS [Name]
                FROM
	                [sys].[synonyms] [syn]
	                CROSS APPLY STRING_SPLIT([syn].[base_object_name], '.') AS [p]
                WHERE
                    OBJECT_ID([syn].[base_object_name]) IS NOT NULL"))
                    .GroupBy(row => row.ObjectId)
                    .Select(grp => grp.Select((item, index) => new ObjectNamePart()
                    {
                        ObjectId = item.ObjectId,
                        ReferencedObjectId = item.ReferencedObjectId,
                        Level = index,
                        Name = item.Name
                    })).SelectMany(names => names);

            var referencedSchemas = referencedNameParts
                .Where(part => part.Level == 1)
                .ToDictionary(row => row.ReferencedObjectId, row => Unbracket(row.Name));

            // all the synonyms in current connection
            var synonyms = await connection.QueryAsync<SynonymResult>(
                @"SELECT 
                    [syn].[object_id] AS [ObjectId],
                    [syn].[name] AS [Name],
                    SCHEMA_NAME([syn].[schema_id]) AS [Schema],
                    OBJECT_ID([syn].[base_object_name]) AS [TargetObjectId],
                    (SELECT TOP 1 [value] FROM STRING_SPLIT([syn].[base_object_name], '.')) AS [RawDbName],
                    [syn].[base_object_name] AS [ReferencedObjectName]
                FROM 
                    [sys].[synonyms] [syn]");

            // synonyms can point to a wide array of object types, so we need "discovery" methods for each type we care to support.
            // initially I'm interested in stored procedures and views
            var reflectors = new SynonymReflector[]
            {
                new SynonymReflector(async (dbName, cn) => await GetProcsInnerAsync(cn, dbName)),
                new SynonymReflector(async (dbName, cn) => await GetViewsInnerAsync(cn, dbName))
            };

            foreach (var db in synonyms.GroupBy(row => row.RawDbName))
            {
                if (!await connection.DatabaseExistsAsync(db.Key)) continue;

                foreach (var r in reflectors)
                {
                    var referencedObjects = await r.ObjectGetter.Invoke($"{db.Key}.", connection);
                    var output = referencedObjects.Join(synonyms, obj => obj.Id, syn => syn.TargetObjectId, (obj, syn) =>
                    {
                        // schema names don't come back in sys views due to limitation of SCHEMA_NAME function
                        if (string.IsNullOrEmpty(obj.Schema) && referencedSchemas.TryGetValue(obj.Id, out string schema))
                        {
                            obj.Schema = schema;
                        }

                        return new Synonym()
                        {
                            Name = syn.Name,
                            Schema = syn.Schema,
                            Id = syn.ObjectId,
                            ReferencedObject = obj,
                        };
                    });

                    results.AddRange(output);
                }
            }
        }

        private string Unbracket(string name)
        {
            var result = name;
            if (result.StartsWith("[")) result = result.Substring(1);
            if (result.EndsWith("]")) result = result.Substring(0, result.Length - 1);
            return result.Trim();
        }

        private class SynonymReflector
        {
            public SynonymReflector(Func<string, IDbConnection, Task<IEnumerable<DbObject>>> objectGetter)
            {
                ObjectGetter = objectGetter;
            }

            public Func<string, IDbConnection, Task<IEnumerable<DbObject>>> ObjectGetter { get; }
        }

        private class ObjectNamePart
        {
            public int ObjectId { get; set; }
            public int Level { get; set; }
            public string Name { get; set; }
            public int ReferencedObjectId { get; set; }
        }

        private class SynonymResult
        {
            public int ObjectId { get; set; }
            public string Name { get; set; }
            public string Schema { get; set; }
            public int TargetObjectId { get; set; }
            public string RawDbName { get; set; }
            public string ReferencedObjectName { get; set; }            
            public DbObjectType ObjectType { get; set; } // set as a second step
        }
    }
}
