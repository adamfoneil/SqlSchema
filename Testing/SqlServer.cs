using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlSchema.Library.Models;
using SqlSchema.SqlServer;
using SqlServer.LocalDb;
using System.Linq;
using System.Threading.Tasks;
using SqlSchema.SqlServer.Extensions;

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
        public void GetProcedures()
        {
            using (var cn = LocalDb.GetConnection("Ginseng8"))
            {
                var a = new SqlServerAnalyzer();
                var procs = a.GetDbObjectsAsync(cn).Result.OfType<Procedure>();
                Assert.IsTrue(procs.Any());

                var dictionary = procs.ToDictionary(item => item.Name);

                Assert.IsTrue(dictionary["PostInvoice"].Arguments.Any(arg => arg.Name.Equals("@orgId")));
            }
        }

        [TestMethod]
        public async Task GetSynonyms()
        {
            using (var cn = LocalDb.GetConnection("ZingerSample"))
            {
                var a = new SqlServerAnalyzer();
                var synonyms = (await a.GetDbObjectsAsync(cn)).OfType<Synonym>();                
                Assert.IsTrue(synonyms.Count() == 3);
            }
        }

        [TestMethod]
        public async Task DbExists()
        {
            using var cn = LocalDb.GetConnection("ZingerSample");
            var exists = await cn.DatabaseExistsAsync("hello");
            Assert.IsFalse(exists);
            exists = await cn.DatabaseExistsAsync("ZingerSample");
            Assert.IsTrue(exists);
        }
    }
}
