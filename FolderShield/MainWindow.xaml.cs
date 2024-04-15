using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using System.Configuration;
using System.Threading;

namespace FolderShield
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string serviceName = "FShield";
        private ServiceController sc;
        private EventLog fShieldLog;
        private const string fShieldLogName = "FSLog";
        private string chosenDirectory;
        public MainWindow()
        {
            InitializeComponent();
            sc = new ServiceController(serviceName);
            serviceStatusTextBox.Text = sc.Status.ToString();
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(100);
                    ServiceController serviceController = new ServiceController(serviceName);
                    ServiceControllerStatus status = serviceController.Status;
                    UpdateUI(status);
                }
            });
        }
        private void UpdateUI(ServiceControllerStatus status)
        {
            Dispatcher.Invoke(() =>
            {
                if (fShieldLog != null)
                {
                    List<string> messages = new List<string>();
                    serviceLogBox.Clear();
                    foreach (EventLogEntry someEntry in fShieldLog.Entries)
                    {
                        string message = someEntry.Source + " : " +
                            someEntry.Message + " : " + someEntry.TimeWritten;
                        messages.Add(message);
                    }
                    messages.Reverse();
                    foreach (string msg in messages)
                    {
                        serviceLogBox.Text += msg;
                        serviceLogBox.Text += "\n";
                    }
                }
                if (status == ServiceControllerStatus.Running)
                {
                    startButton.IsEnabled = false;
                    endButton.IsEnabled = true;
                }
                else
                {
                    endButton.IsEnabled = false;
                    startButton.IsEnabled = true;
                }
                serviceStatusTextBox.Text = status.ToString();
            });
        }
        
        private void StartServiceButton_Click(object sender, RoutedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(@"FolderShieldService.exe");
            if (config.AppSettings.Settings["Password"].Value == String.Empty)
            {
                MessageBox.Show("Fill the password box!");
                return;
            }
            if (chosenDirectoryBox.Text == String.Empty)
            {
                MessageBox.Show("Set the directory!");
                return;
            }
            try
            {
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running);
                serviceStatusTextBox.Text = sc.Status.ToString();
                fShieldLog = new EventLog(fShieldLogName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void StopServiceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped);
                serviceStatusTextBox.Text = sc.Status.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void ClearLogsButton_Click(object sender, RoutedEventArgs e)
        {
            if(fShieldLog!= null)
            {
                fShieldLog.Clear();
                serviceLogBox.Clear();
            }
        }

        private void changeDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    string selectedFolder = dialog.SelectedPath;
                    chosenDirectory = selectedFolder;
                    chosenDirectoryBox.Text = chosenDirectory;
                    try
                    {
                        Configuration config = ConfigurationManager.OpenExeConfiguration(@"FolderShieldService.exe");
                        config.AppSettings.Settings["Path"].Value = chosenDirectory;
                        config.Save(ConfigurationSaveMode.Full);
                        ConfigurationManager.RefreshSection("appSettings");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    
                }
            }
        }

        private void changePassButton_Click(object sender, RoutedEventArgs e)
        {
            string password = passBox.Password;
            if (!string.IsNullOrWhiteSpace(password))
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(@"FolderShieldService.exe");
                config.AppSettings.Settings["Password"].Value = password;
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }
    }
}
