using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace bach_hjalp.Databases
{
    public class RawDatabaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public double Time { get; set; }
        public double ECG_data { get; set; }
    }
}
