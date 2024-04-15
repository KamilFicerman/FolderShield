using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace FolderShieldService
{
    [RunInstaller(true)]
    public partial class FShieldServiceInstaller : System.Configuration.Install.Installer
    {
        public FShieldServiceInstaller()
        {
            InitializeComponent();
        }
    }
}
