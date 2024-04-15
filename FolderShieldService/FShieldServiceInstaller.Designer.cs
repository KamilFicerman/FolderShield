namespace FolderShieldService
{
    partial class FShieldServiceInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FShieldServiceInstaller));
            this.fShieldProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.fShieldInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // fShieldProcessInstaller
            // 
            this.fShieldProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.fShieldProcessInstaller.Password = null;
            this.fShieldProcessInstaller.Username = null;
            // 
            // fShieldInstaller
            // 
            this.fShieldInstaller.Description = resources.GetString("fShieldInstaller.Description");
            this.fShieldInstaller.DisplayName = "FolderShield Service";
            this.fShieldInstaller.ServiceName = "FShield";
            // 
            // FShieldServiceInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.fShieldProcessInstaller,
            this.fShieldInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller fShieldProcessInstaller;
        private System.ServiceProcess.ServiceInstaller fShieldInstaller;
    }
}