using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;

namespace FolderShieldLib
{
    public static class ShieldProcessManager
    {
        //method runs in the background, keeps track of the user's file
        //openings and saves the pair (a file name and a process) in the container to know later what to close
        public static void CatchThePIDAndFileName()
        {
            while (Shield._trigger)
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT ProcessId, CommandLine FROM Win32_Process");
                foreach (ManagementObject process in searcher.Get())
                {
                    int processId = Convert.ToInt32(process["ProcessId"]);
                    string commandLine = process["CommandLine"]?.ToString();
                    if (!string.IsNullOrEmpty(commandLine))
                    {
                        string path = Shield._path;
                        string[] fileNames = Directory.GetFiles(path);

                        foreach (string fileName in fileNames)
                        {
                            if (commandLine.Contains(fileName))
                            {
                                try
                                {
                                    if (!Shield._currentlyUsed.Exists(item => item.Item1.Id == processId && item.Item2 == fileName))
                                    {
                                        Process process1 = Process.GetProcessById(processId);
                                        Shield._currentlyUsed.Add((process1, fileName));
                                    }
                                }
                                catch (ArgumentException)
                                {
                                    //silent catch when the process no longer exists
                                    //in order not to enter the event log
                                }
                                catch (Exception ex)
                                {
                                    Shield._appLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                                }
                            }
                        }
                    }
                }
            }
        }
        //method runs in the background, tracks the user's opening of ".encrypted" files,
        //kills the process, decrypts and deletes the file
        public static void UserWantsToOpenEncryptedFile()
        {
            while (Shield._trigger)
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT ProcessId, CommandLine FROM Win32_Process WHERE Name='notepad.exe'");

                foreach (ManagementObject process in searcher.Get())
                {
                    int processId = Convert.ToInt32(process["ProcessId"]);
                    string commandLine = process["CommandLine"]?.ToString();

                    if (!string.IsNullOrEmpty(commandLine))
                    {
                        string path = Shield._path;
                        string[] fileNames = Directory.GetFiles(path, "*.encrypted");
                        foreach (string fileName in fileNames)
                        {
                            if (commandLine.Contains(fileName))
                            {
                                try
                                {
                                    Process process1 = Process.GetProcessById(processId);
                                    string outputFile = fileName.Substring(0, fileName.Length - 10);
                                    byte[] key = ShieldEncryptor.GenerateKeyToDecrypt(outputFile);
                                    process1.Kill();

                                    string blockedFilePath = outputFile + ".blocked";
                                    File.Create(blockedFilePath).Close();
                                    File.SetAttributes(blockedFilePath, File.GetAttributes(blockedFilePath) | FileAttributes.Hidden);

                                    ShieldEncryptor.DecryptFile(fileName, outputFile, key);
                                    ShieldEncryptor.DeleteSalt(outputFile);
                                    ShieldEncryptor.DeleteIV(outputFile);
                                    File.Delete(fileName);
                                    Shield._appLog.WriteEntry("Succesfully decrypted " + fileName,
                                        EventLogEntryType.Information);
                                }
                                catch (CryptographicException)
                                {
                                    Shield._appLog.WriteEntry("Decryption impossible!",
                                    EventLogEntryType.Error);
                                    string outputFile = fileName.Substring(0, fileName.Length - 10);
                                    string blockedFilePath = outputFile + ".blocked";
                                    File.Delete(blockedFilePath);
                                }
                                catch (Exception ex)
                                {
                                    Shield._appLog.WriteEntry(ex.Message,
                                    EventLogEntryType.Error);
                                }

                            }
                        }
                    }
                }
            }
        }
        public static void KillTheProcessOnSave(string file)
        {
            try
            {
                var result = Shield._currentlyUsed.FirstOrDefault(item => item.Item2 == file);
                try
                {
                    if (result != default)
                    {
                        result.Item1.Kill();
                        result.Item1.WaitForExit();

                        Shield._currentlyUsed.Remove(result);
                    }
                }
                catch (InvalidOperationException)
                {
                    Shield._currentlyUsed.Remove(result);
                    KillTheProcessOnSave(file);
                    //Recursive method call for next matching element to watch for "_currentlyUsed" values
                }
            }
            catch (Exception ex)
            {
                Shield._appLog.WriteEntry(ex.Message,
                EventLogEntryType.Error);
            }
        }
    }
}
