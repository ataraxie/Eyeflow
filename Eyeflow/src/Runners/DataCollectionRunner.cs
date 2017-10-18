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
                gaze.ParentWindowPtr = parentWindowPtr;
                gaze.WindowTitle = windowTitle;
                gaze.WindowProcess = processName;

                DatabaseService.Instance.writeGazeRecord(gaze);
            }
        }

        private void onWindowEvent(object sender, WindowEventArgs e)
        {
            DwmRecord dwmRecord = new DwmRecord();
            dwmRecord.Timestamp = GazeLib.getTimestamp();
            dwmRecord.Event = "DUMMY_EVENT";
            dwmRecord.NumMonitors = Screen.AllScreens.Length;

            int dwmUuid = DatabaseService.Instance.writeDwmRecord(dwmRecord);
            foreach (IntPtr windowHandle in WinLib.getAllTopLevelWindows()) 
            {
                if (GazeLib.isTargetWindow(windowHandle))
                {
                    string processName = WinLib.getProcessName(windowHandle);
                    string windowTitle = WinLib.getWindowTitle(windowHandle);
                    // CONTINUE HERE
                    
                }
            }

            
            // create many Window records
            //WinLib.getAllWindows();
        }

    }
}