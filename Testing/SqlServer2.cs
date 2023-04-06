using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlSchema.SqlServer;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Testing
{
   [TestClass]
   public class SqlServer2
   {
      [TestMethod]
      public async Task InspectSchemaAsync()
      {
         var connectionStrings = new[]
         {
            Config.GetConnectionString("Db1"),
            Config.GetConnectionString("Db2")
         };

         foreach (var connectionStr in connectionStrings)
         {
            using var cn = new SqlConnection(connectionStr);
            var a = new SqlServerAnalyzer();
            var results = await a.GetDbObjectsAsync(cn);
         }
      }


      private IConfiguration Config => new ConfigurationBuilder()
         .AddJsonFile("connections.json", optional: false)
         .Build();
   }
}
