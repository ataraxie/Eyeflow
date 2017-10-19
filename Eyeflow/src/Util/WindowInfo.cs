using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Eyeflow.Util
{
    class WindowInfo
    {
        public IntPtr handle { get; set; }
        public Rectangle rect { get; set; }
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }
        public bool IsMinimized { get; set; }
        public bool IsMaximized { get; set; }
        public bool IsNormal { get; set; }
    }
}
