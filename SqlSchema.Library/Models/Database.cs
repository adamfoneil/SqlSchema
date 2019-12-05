using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSchema.Library.Models
{
    public class Database
    {
        public string Name { get; set; }

        public IEnumerable<DbObject> Tables { get; set; }
    }
}
