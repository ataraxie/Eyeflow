using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Eyeflow.Util;

namespace Eyeflow.Entities
{
    class DwmRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public long Timestamp { get; set; }
        public string Event { get; set; }
        public int NumScreens { get; set; }

        public override string ToString()
        {
            return this.Event + ":::" + GazeLib.objectDump(this);
        }
    }
}