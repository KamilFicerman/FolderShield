using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace FolderShieldLib
{
    public static class Shield
    {
        public static bool _trigger =  true;
        public static EventLog _appLog;
        public static string _path;
        public static string _password;
        public static List<(Process, string)> _currentlyUsed;

        //start the main logic
        public static void Start(EventLog appLog, string path, string password)
        {
                _appLog = appLog;
                _path = path;
                _password = password;
                _currentlyUsed = new List<(Process, string)> ();
                ShieldWatcher.StartTheWatcher();
                var t = new Thread(() => ShieldProcessManager.UserWantsToOpenEncryptedFile());
                t.Start();
                var t2 = new Thread(() => ShieldProcessManager.CatchThePIDAndFileName());
                t2 .Start();
        }
        //end the main logic
        public static void JoinThreads()
        {
            _trigger = false;
        }
    }
}
