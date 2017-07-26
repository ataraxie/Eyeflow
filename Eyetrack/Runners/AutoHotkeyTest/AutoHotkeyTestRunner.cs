using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHotkey.Interop;

namespace Eyetrack.Runners.AutoHotkeyTest
{
    class AutoHotkeyTestRunner : Runner
    {
        static String PATH = "C:\\Users\\IEUser\\Dropbox\\dev\\dotnet\\Eyetrack\\Eyetrack\\ahk\\functions.ahk";

        public override void run()
        {
            var ahk = AutoHotkeyEngine.Instance;
            ahk.LoadFile(PATH);
            //ahk.ExecFunction("MyFunction", "Hello", "World");
        }
    }
}
