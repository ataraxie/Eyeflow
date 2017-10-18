using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Eyeflow.Entities
{
    class WindowRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Uuid { get; set; }

        public long Timestamp { get; set; }
        public int DwmUuid { get; set; }
        public int WindowPtr { get; set; }
        public int LeftPos { get; set; }
        public int RightPos { get; set; }
        public int BottomPos { get; set; }
        public int TopPos { get; set; }
        public int ParentWindowPtr { get; set; }
        public string WindowTitle { get; set; }
        public string WindowProcess { get; set; }
        public int ZIndex { get; set; }
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }
        public bool IsMinimized { get; set; }
        public bool IsMaximized { get; set; }
        public bool IsNormal { get; set; }
        public bool OnMonitor { get; set; }

    }
}
