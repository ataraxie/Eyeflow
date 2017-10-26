using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Eyeflow;
using Eyeflow.Runners;
using Eyeflow.Dispatchers;
using System.Windows.Forms;

using Eyeflow.Util;

namespace Eyeflow.Lifecycle
{
    public class EyeflowFormContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private Action<EventArgs> onExit;

        public EyeflowFormContext(Action<EventArgs> onExit)
        {
            this.onExit = onExit;
            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = new System.Drawing.Icon(Path.GetFullPath(@"eyeflow.ico")),
                ContextMenu = new ContextMenu(new MenuItem[] {
                new MenuItem("Exit", onExitClick)
            }),
                Visible = true
            };
        }

        void onExitClick(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;

            this.onExit(e);
        }
    }
}
