using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace APKEasyTool
{
    static class Program
    {
        public static readonly string Name = "APK Easy Tool";
        public static readonly string Version = "1.61";

        [DllImport("Shcore.dll")]
        static extern int SetProcessDpiAwareness(int PROCESS_DPI_AWARENESS);

        private enum DpiAwareness
        {
            None = 0,
            SystemAware = 1,
            PerMonitorAware = 2
        }

        [STAThread]
        static void Main(String[] arg)
        {
            if (Environment.OSVersion.Version.Major == 6)
            {
                SetProcessDPIAware();
                // SetProcessDpiAwareness((int)DpiAwareness.PerMonitorAware);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Load language/configuration.
            LoadConfig();
            Lang.LoadStr();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (ResourcesExist())
            {
                Application.Run(ModernUi.Apply(new MainForm()));
            }
            else
            {
                MessageBox.Show(Lang.RESOURCE_MISSING_NOTICE, Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        static void LoadConfig()
        {
            try
            {
                if (File.Exists(Variables.GetPath() + "config.xml"))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(APKEasyTool));
                    FileStream read = new FileStream(Variables.GetPath() + "config.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
                    APKEasyTool info = (APKEasyTool)xs.Deserialize(read);
                    Lang.LoadLocalization(Variables.RealPath("Language\\" + info.Language));
                    read.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        static bool ResourcesExist()
        {
            return Directory.Exists(Variables.RealPath("Apktool")) &&
                   Directory.Exists(Variables.RealPath("Resources"));
        }
    }
}
