using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using System.Collections.Generic;

namespace Eyeflow.Util
{
    class FixedSizeQueue<IntPtr> : Queue<IntPtr>
    {
        private static Logger log = Logger.get(typeof(FixedSizeQueue<IntPtr>));

        private int size;

        public FixedSizeQueue(int size) : base(size)
        {
            this.size = size;
        }

        public new void Enqueue(IntPtr handle)
        {
            if (base.Count == this.size)
            {
                base.Dequeue();
            }
            base.Enqueue(handle);
        }

    }
}
