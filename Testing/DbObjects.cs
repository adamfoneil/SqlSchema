using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlSchema.Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Testing
{
    [TestClass]
    public class DbObjects
    {
        [TestMethod]
        public void TablesShouldBeEqual()
        {
            var tbl1 = new Table() { Schema = "dbo", Name = "FuckYou" };
            var tbl2 = new Table() { Schema = "dbo", Name = "fuckyou" };
            Assert.IsTrue(tbl1.Equals(tbl2));
        }

        [TestMethod]
        public void TablesShouldBeNotEqual()
        {
            var tbl1 = new Table() { Schema = "dbo", Name = "Fuck_You" };
            var tbl2 = new Table() { Schema = "dbo", Name = "fuckyou" };
            Assert.IsTrue(!tbl1.Equals(tbl2));
        }
    }
}
