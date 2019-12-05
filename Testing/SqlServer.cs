using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
