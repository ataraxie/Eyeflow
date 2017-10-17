using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Eyeflow.src.Entities
{
    class DwmRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Uuid { get; set; }

        public long Timestamp { get; set; }
        public string Event { get; set; }
        public int NumMonitors { get; set; }
    }
}