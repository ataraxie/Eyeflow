using System;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Timers;
using Eyeflow.Util;
using Eyeflow.Dispatchers;
using Eyeflow.Entities;
using Eyeflow.Services;
using Eyeflow.Events;
using System.Windows.Forms;

namespace Eyeflow.Runners
{
    class DataCollectionRunner : Runner
    {
        private static Logger log = Logger.get(typeof(DataCollectionRunner));
        private static Config config = Config.Instance;
        
        private GazeDispatcher gazeDispatcher;
        private WindowDispatcher windowDispatcher;
        private int gazeCount = 0;

        private List<GazeRecord> gazeRecords = new List<GazeRecord>();
        private Dictionary<string, int> processNames = new Dictionary<string, int>();
        private Dictionary<string, int> windowTitles = new Dictionary<string, int>();

        protected HashSet<IntPtr> visibleWindows = new HashSet<IntPtr>();

        public DataCollectionRunner(GazeDispatcher gazeDispatcher, 
            WindowDispatcher windowDispatcher)
        {
            this.gazeDispatcher = gazeDispatcher;
            this.gazeDispatcher.addEventHandler(onGazeEvent);
            this.windowDispatcher = windowDispatcher;
            this.windowDispatcher.addEventHandler(onWindowEvent);
        }

        public override void start()
        {
            log.debug("DataCollectionRunner started");
            DatabaseService.Instance.createDatabase();
        }

        public override void stop()
        {
            DatabaseService.Instance.disconnect();
        }

        private void onGazeEvent(object sender, GazeEventArgs e)
        {
            this.gazeCount++;
            if (this.gazeCount % config.runOnEveryXGazeDispatch == 0)
            {
                GazeRecord gaze = new GazeRecord();
                int x = (int)e.x;
                int y = (int)e.y;
                Point point = new Point(x, y);
                IntPtr windowAtGaze = WinLib.WindowFromPoint(point);
                IntPtr topLevelWindowAtGaze = WinLib.getTopLevelWindow(windowAtGaze);
                int windowPtr = windowAtGaze.ToInt32();
                int parentWindowPtr = topLevelWindowAtGaze.ToInt32();
                String windowTitle = WinLib.getWindowTitle(windowAtGaze);
                String processName = WinLib.getProcessName(windowAtGaze);

                gaze.Timestamp = GazeLib.getTimestamp();
                gaze.X = x;
                gaze.Y = y;
                gaze.WindowPtr = windowPtr;
                gaze.WindowTitle = windowTitle;
                gaze.WindowProcess = processName;

                DatabaseService.Instance.writeGazeRecord(gaze);
            }
        }

        private void onWindowEvent(object sender, WindowEventArgs e)
        {
            log.trace("=== onWindowEvent ===");
            long timestamp = GazeLib.getTimestamp();
            List<Screen> allScreens = new List<Screen>(Screen.AllScreens);
            GazeLib.logProf(timestamp, "Screen.AllScreens");

            long timestamp2 = GazeLib.getTimestamp();
            DwmRecord dwmRecord = new DwmRecord();
            dwmRecord.Timestamp = timestamp;
            dwmRecord.Event = "DUMMY_EVENT";
            dwmRecord.NumScreens = allScreens.Count;
            GazeLib.logProf(timestamp2, "DwmRecord");

            DatabaseService.Instance.writeDwmRecord(dwmRecord);
            int dwmRecordId = dwmRecord.Id;

            long timestamp3 = GazeLib.getTimestamp();
            HashSet <IntPtr> windows = WinLib.getAllVisibleOrMinimizedWindows();
            GazeLib.logProf(timestamp3, "GetWindows");
            int index = windows.Count;
            foreach (IntPtr windowHandle in windows) 
            {
                index--;
                if (GazeLib.isTargetWindow(windowHandle))
                {
                    long timestamp4 = GazeLib.getTimestamp();
                    Rectangle rect = WinLib.getWindowRectangle(windowHandle);
                    IntPtr activeWindow = WinLib.GetForegroundWindow();
                    Screen screen = Screen.FromRectangle(rect);
                    GazeLib.logProf(timestamp4, "WindowInfo");

                    long timestamp5 = GazeLib.getTimestamp();
                    WindowRecord windowRecord = new WindowRecord()
                    {
                        DwmRecordId = dwmRecordId,
                        Timestamp = timestamp,
                        WindowPtr = windowHandle.ToInt32(),
                        TopPos = rect.Top,
                        RightPos = rect.Right,
                        BottomPos = rect.Bottom,
                        LeftPos = rect.Left,
                        ProcessName = WinLib.getProcessName(windowHandle),
                        Title = WinLib.getWindowTitle(windowHandle),
                        Status = WinLib.getWindowStatus(windowHandle),
                        IsForeground = windowHandle == activeWindow,
                        IsVisible = WinLib.IsWindowVisible(windowHandle),
                        IsIconic = WinLib.IsIconic(windowHandle),
                        Screen = allScreens.IndexOf(screen),
                        ZIndex = index
                    };
                    GazeLib.logProf(timestamp5, "WindowRecord");

                    DatabaseService.Instance.writeWindowRecord(windowRecord);
                    //log.debug(windowRecord.ToString());
                }
            }

            GazeLib.logProf(timestamp, "onWindowEvent");
            
        }

    }
}