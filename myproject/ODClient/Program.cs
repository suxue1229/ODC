using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace ODClient
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            bool isnew = false;
            String appid = Assembly.LoadFile(Application.ExecutablePath).ManifestModule.ModuleVersionId.ToString();
            using (Mutex mutex = new Mutex(true, "Global\\" + appid, out isnew))
            {
                if (isnew)
                {
                    Form.CheckForIllegalCrossThreadCalls = false;
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new ODClient());
                    mutex.ReleaseMutex();
                }
                else if (args.Length == 0)
                {
                    MessageBox.Show("Don't run multiple instances on one host.", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
