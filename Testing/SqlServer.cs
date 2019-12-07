using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlSchema.Library.Models;
using SqlSchema.SqlServer;
using SqlServer.LocalDb;
using System.Linq;

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
                var fks = rideTable.GetForeignKeys(objects);
                Assert.IsTrue(fks.Count() == 2);

                Assert.IsTrue(fks.Select(fk => fk.Name).SequenceEqual(new string[] { "FK_Ride_BicycleID", "FK_Ride_LocationID" }));

                var fkDictionary = fks.ToDictionary(row => row.Name);
                Assert.IsTrue(fkDictionary["FK_Ride_BicycleID"].ReferencedTable.Equals(new Table() { Schema = "dbo", Name = "Bicycle" }));
                Assert.IsTrue(fkDictionary["FK_Ride_BicycleID"].ReferencingTable.Equals(new Table() { Schema = "dbo", Name = "Ride" }));

                Assert.IsTrue(fkDictionary["FK_Ride_LocationID"].Columns.First().ReferencedName.Equals("ID"));
                Assert.IsTrue(fkDictionary["FK_Ride_LocationID"].Columns.First().ReferencingName.Equals("LocationID"));
            }
        }
    }
}
