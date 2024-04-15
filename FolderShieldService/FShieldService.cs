using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using FolderShieldLib;

namespace FolderShieldService
{
    public partial class FShieldService : ServiceBase
    {
        private readonly string path = ConfigurationManager.AppSettings["Path"];
        private readonly string password = ConfigurationManager.AppSettings["Password"];
        private readonly string logName = ConfigurationManager.AppSettings["LogName"];
        private readonly string logSource = ConfigurationManager.AppSettings["LogSource"];
        Thread main;
        private EventLog appLog;
        public FShieldService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (!EventLog.SourceExists(logSource, "."))
            {
                EventLog.CreateEventSource(logSource, logName);
            }
            appLog = new EventLog(logName);
            appLog.Source = logSource;
            appLog.WriteEntry("Service has started!",
                EventLogEntryType.Information);
            main = new Thread(() => Shield.Start(appLog, path, password));
            main.Start();
        }

        protected override void OnStop()
        {
            appLog.WriteEntry("Service has stopped!",
                EventLogEntryType.Information);

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["Password"].Value = string.Empty;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            Shield.JoinThreads();
            main.Join();

            appLog?.Dispose();
        }
    }
}
