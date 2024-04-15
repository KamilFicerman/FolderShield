using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace FolderShieldLib
{
    public static class ShieldWatcher
    {
        public static void StartTheWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.Path = Shield._path;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Security;
            watcher.Filter = "*.*";
            watcher.Changed += OnChanged;

            watcher.EnableRaisingEvents = true;
        }

        //invoked after writing
        //kills the associated process
        //encrypts the file
        //deletes the file
        public static void OnChanged(object source, FileSystemEventArgs e)
        {
            string fname = e.FullPath;
            //a bunch of support files for encryption or support files used by other programs
            //these files could cause problems
            if (fname.Contains(".encrypted") || fname.Contains(".iv")
                || fname.Contains(".salt") || fname.Contains("~")
                || fname.Contains(".blocked") || fname.Contains(".tmp"))
            {
                return;
            }

            //File has just been created after decryption
            if (File.Exists(fname + ".blocked"))
            {
                File.Delete(fname + ".blocked");
                return;
            }

            string inputFile = e.FullPath;
            ShieldProcessManager.KillTheProcessOnSave(inputFile);
            string encryptedFile = inputFile + ".encrypted";
            Thread.Sleep(1000);

            try
            {
                byte[] key = ShieldEncryptor.GenerateKeyToEncrypt(inputFile);
                ShieldEncryptor.EncryptFile(inputFile, encryptedFile, key);
                File.Delete(inputFile);
                Shield._appLog.WriteEntry("Succesfully encrypted " + inputFile,
                EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                Shield._appLog.WriteEntry(ex.Message,
                EventLogEntryType.Error);
            }

        }

    }
}
