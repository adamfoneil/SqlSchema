using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlSchema.Library.Models;
using SqlSchema.SqlServer;
using SqlSchema.SqlServer.Static;
using SqlServer.LocalDb;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using static SqlSchema.SqlServer.Static.SqlBuilder;

namespace Testing
{
    [TestClass]
    public class SqlServer
    {
        [TestMethod]
        public void Analyze()
        {
            using (var cn = LocalDb.GetConnection("CycleLog2"))
            {
                var a = new SqlServerAnalyzer();
                var objects = a.GetDbObjectsAsync(cn).Result;
                Assert.IsTrue(objects.Any());
            }          
        }

        [TestMethod]
        public void GetForeignKeys()
        {
            using (var cn = LocalDb.GetConnection("CycleLog2"))
            {
                var a = new SqlServerAnalyzer();
                var objects = a.GetDbObjectsAsync(cn).Result;

                var rideTable = objects.ToDictionary(row => $"{row.Schema}.{row.Name}")["dbo.Ride"] as Table;
                
                var parentFks = rideTable.GetParentForeignKeys(objects);
                Assert.IsTrue(parentFks.Count() == 2);

                Assert.IsTrue(parentFks.Select(fk => fk.Name).SequenceEqual(new string[] { "FK_Ride_BicycleID", "FK_Ride_LocationID" }));

                var fkDictionary = parentFks.ToDictionary(row => row.Name);
                Assert.IsTrue(fkDictionary["FK_Ride_BicycleID"].ReferencedTable.Equals(new Table() { Schema = "dbo", Name = "Bicycle" }));
                Assert.IsTrue(fkDictionary["FK_Ride_BicycleID"].ReferencingTable.Equals(new Table() { Schema = "dbo", Name = "Ride" }));

                Assert.IsTrue(fkDictionary["FK_Ride_LocationID"].Columns.First().ReferencedName.Equals("ID"));
                Assert.IsTrue(fkDictionary["FK_Ride_LocationID"].Columns.First().ReferencingName.Equals("LocationID"));

                var childFks = rideTable.GetChildForeignKeys(objects);
                Assert.IsTrue(childFks.Count() == 1);

                Assert.IsTrue(childFks.First().ReferencingTable.Equals(new Table() { Schema = "dbo", Name = "RideTag" }));
                Assert.IsTrue(childFks.First().ReferencedTable.Equals(new Table() { Schema = "dbo", Name = "Ride" }));

                var tableDictionary = objects.OfType<Table>().ToDictionary(item => item.Name);

                Assert.IsTrue(tableDictionary["RideTag"].UniqueConstraintColumns.SequenceEqual(new string[] { "RideID", "TagID" }));
            }
        }

        [TestMethod]
        public void GetViews()
        {
            using (var cn = LocalDb.GetConnection("Ginseng8"))
            {
                var a = new SqlServerAnalyzer();
                var views = a.GetDbObjectsAsync(cn).Result.OfType<View>();
                Assert.IsTrue(views.Any());
            }
        }

        [TestMethod]
        public void GetTableFunctions()
        {
            using (var cn = LocalDb.GetConnection("Ginseng8"))
            {
                var a = new SqlServerAnalyzer();
                var functions = a.GetDbObjectsAsync(cn).Result.OfType<TableFunction>();
                Assert.IsTrue(functions.Any());
            }
        }

        [TestMethod]
        public void ExploreFKs()
        {
            using (var cn = LocalDb.GetConnection("AerieHub4"))
            {
                var allObjects = new SqlServerAnalyzer().GetDbObjectsAsync(cn).Result;

                var rootTable = allObjects.OfType<Table>().ToDictionary(obj => obj.Name)["Field"];
                rootTable.EnumChildForeignKeys(allObjects,
                    ending:
                        (stack) => Debug.WriteLine(string.Join("\r\n",
                            stack.Select((fk, index) => new string(' ', index * 2) + $"{fk.ReferencingTable} -> {fk.ReferencedTable}")) + "\r\n"));

                /*
                this is for FromClaseSql test
                rootTable.EnumChildForeignKeys(allObjects,
                    ending: (stack) =>
                    {
                        var simplified = new
                        {
                            foreignKeys = stack.Select(fk => new
                            {
                                name = fk.Name,
                                referencedTable = fk.ReferencedTable.Name,
                                referencingTable = fk.ReferencingTable.Name,
                                columns = fk.Columns.Select(col => new
                                {
                                    referencedName = col.ReferencedName,
                                    referencingName = col.ReferencingName
                                })
                            })
                        };

                        string json = JsonSerializer.Serialize(simplified, new JsonSerializerOptions()
                        {
                            WriteIndented = true
                        });
                        Debug.Print(json);
                        Debug.Print("\r\n");
                    });*/
            }
        }

        [TestMethod]
        public void FromClauseSql()
        {
            var fks = GetResourceFKs();            

            string expectedResult =
                @$"FROM [DropdownValue] [dv] 
                INNER JOIN [DropdownOption] [do] ON [dv].[Value]=[do].[Id]
                INNER JOIN [Field] [f] ON [do].[FieldId]=[f].[Id]";

            var joinSql = SqlBuilder.JoinTables(fks);
            Assert.IsTrue(joinSql.Equals(expectedResult));
            
            IEnumerable<ForeignKey> GetResourceFKs()
            {
                var json = GetResource("foreignKeys.json");
                using (var doc = JsonDocument.Parse(json))
                {
                    var array = doc.RootElement.GetProperty("tables");
                    var tables = array.EnumerateArray().Select(element => new Table()
                    {
                        Name = element.GetProperty("name").ToString(),
                        Alias = element.GetProperty("alias").ToString()
                    }).ToDictionary(item => item.Name);
                    
                    array = doc.RootElement.GetProperty("foreignKeys");
                    foreach (var element in array.EnumerateArray())
                    {
                        yield return new ForeignKey()
                        {
                            Name = element.GetProperty("name").ToString(),
                            ReferencedTable = tables[element.GetProperty("referencedTable").ToString()],
                            ReferencingTable = tables[element.GetProperty("referencingTable").ToString()],
                            Columns = element.GetProperty("columns").EnumerateArray().Select(ele => new ForeignKeyColumn()
                            {
                                ReferencedName = ele.GetProperty("referencedName").ToString(),
                                ReferencingName = ele.GetProperty("referencingName").ToString()
                            }).ToArray()
                        };
                    }
                }
            }
        }        

        private string GetResource(string name)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Testing.Resources.{name}"))
            {
                return new StreamReader(stream).ReadToEnd();                
            }
        }
    }
}
