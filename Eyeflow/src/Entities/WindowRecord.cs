using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.ComponentModel;

namespace Eyeflow.Entities
{
    class WindowRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public long Timestamp { get; set; }

        [Indexed]
        public int DwmRecordId { get; set; }

        public int WindowPtr { get; set; }
        public int LeftPos { get; set; }
        public int RightPos { get; set; }
        public int BottomPos { get; set; }
        public int TopPos { get; set; }
        public string Title { get; set; }
        public string ProcessName { get; set; }
        public int ZIndex { get; set; }
        public bool IsIconic { get; set; }
        public bool IsForeground { get; set; }
        public bool IsVisible { get; set; }
        public int Status { get; set; } // Hide = 0, Normal = 1, Minimized = 2, Maximized = 3
        public int Screen { get; set; }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append(this.ProcessName);
            b.Append(":::");
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(this))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(this);
                b.Append(String.Format("{0}={1};", name, value));
            }
            return b.ToString();
        }

    }
}
