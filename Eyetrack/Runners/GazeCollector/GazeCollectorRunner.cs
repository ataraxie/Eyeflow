using System;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using Tobii.Interaction;
using System.Runtime.InteropServices;

using System.Collections.Generic;

using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;
using Eyetrack.Runners;
using System.Timers;
using AutoHotkey.Interop;

namespace Eyetrack.Runners.GazeCollector
{
    class GazeCollectorRunner : Runner
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool IsIconic(IntPtr handle);

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(System.Drawing.Point p);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

        private static String PATH = "Resources\\ahk\\functions.ahk"; // must be in bin/debug
        private static AutoHotkeyEngine AHK = AutoHotkeyEngine.Instance;

        private static List<string> IGNORED_PROCESSES = new List<string>(new string[] {
            /*"explorer",*/ "Eyetrack"
        });

        private static long TIME_THRESHOLD_MS = 3000;
        private static long TIMER_INTERVAL = 1000;

        private GazeCollector gazeCollector;

        private HashSet<int> visibleProcesses = new HashSet<int>();
        private HashSet<int> hiddenProcesses = new HashSet<int>();

        public override void run()
        {
            AHK.LoadFile(PATH);
            showAllProcesses(false);
            this.gazeCollector = new GazeCollector();
            this.gazeCollector.GazeEvent += onGazeEvent;
            this.gazeCollector.start();
            initInactiveProcessHider();
            Console.ReadKey();
            this.gazeCollector.stop();
            showAllProcesses(true);
        }

        private void onGazeEvent(object sender, EventArgs e)
        {
            showProcess(this.gazeCollector.lastGazedProcessId);
        }

        private void showAllProcesses(bool shallShow)
        {
            foreach (Process process in Process.GetProcesses())
            {
                if (isTargetProcess(process))
                {
                    if (shallShow)
                    {
                        showProcess(process.Id);
                    } else
                    {
                        hideProcess(process.Id);
                    }
                   
                }
            }
        }

        private void showProcess(int processId) 
        {
            if (!this.visibleProcesses.Contains(processId))
            {
                this.visibleProcesses.Add(processId);
                this.hiddenProcesses.Remove(processId);
                Console.WriteLine("Show process: " + getProcessName(processId));
                AHK.ExecFunction("ShowWindow", processId.ToString());
            }

        }

        private void hideProcess(int processId)
        {
            if (!this.hiddenProcesses.Contains(processId))
            {
                this.visibleProcesses.Remove(processId);
                this.hiddenProcesses.Add(processId);
                Console.WriteLine("Hiding process: " + getProcessName(processId));
                AHK.ExecFunction("HideWindow", processId.ToString());
            }
        }

        private bool isTargetProcess(Process process)
        {
            bool ret = false;
            IntPtr handle = process.MainWindowHandle;
            if (handle != IntPtr.Zero && !IsIconic(handle) && IsWindowVisible(handle)
                && !IGNORED_PROCESSES.Contains(process.ProcessName))
            {
                Rectangle rect = new Rectangle();
                GetWindowRect(handle, out rect);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    ret = true;
                }
            }
            return ret;
        }

        private String getProcessName(int processId)
        {
            Process process = Process.GetProcessById(processId);
            return process.ProcessName;
        }

        private void onTimerHideInactiveProcesses(object source, ElapsedEventArgs e)
        {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            foreach (int processId in this.gazeCollector.processTimestamps.Keys.ToList())
            {
                if (now - this.gazeCollector.processTimestamps[processId] > TIME_THRESHOLD_MS)
                {
                    hideProcess(processId);
                }
            }
        }

        private void initInactiveProcessHider()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(onTimerHideInactiveProcesses);
            aTimer.Interval = TIMER_INTERVAL;
            aTimer.Enabled = true;
        }

        //private static async Task SetInterval(Action action, TimeSpan timeout)
        //{
        //    Console.WriteLine("NEW INTERVAL");
        //    await Task.Delay(timeout).ConfigureAwait(false);

        //    action();

        //    SetInterval(action, timeout);
        //}

        private bool isAboveThreshold(int processId, long timestampNow)
        {
            long processTimestamp = this.gazeCollector.processTimestamps[processId];
            bool isAbove = timestampNow - processTimestamp > TIME_THRESHOLD_MS;
            string processName = getProcessName(processId);
            Console.WriteLine("Checking: " + processName + " = " + isAbove);
            return isAbove;
        }

    }
}