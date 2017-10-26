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
    class EyeflowFormApp : EyeflowApp
    {
        private static Logger log = Logger.get(typeof(EyeflowFormApp));


        public override void execute()
        {
            base.execute();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            EyeflowFormContext appContext = new EyeflowFormContext((e) => this.exit());

            Application.Run(appContext);
        }

        public override void exit()
        {
            base.exit();
            Application.Exit();
        }
        
    }
}
