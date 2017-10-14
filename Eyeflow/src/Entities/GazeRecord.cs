using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Eyeflow.Entities
{
    class GazeRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Uuid { get; set; }

        public long Timestamp { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int WindowPtr { get; set; }
        public int ParentWindowPtr { get; set; }
        public string WindowTitle { get; set; }
        public string WindowProcess { get; set; }
    }
}
