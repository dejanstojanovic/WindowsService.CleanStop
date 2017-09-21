using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService.CleanStop
{
    [RunInstaller(true)]
    public class ServiceInstaller : Installer
    {
        public ServiceInstaller()
        {

            var processInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            var serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            var assemblyDescription = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).OfType<AssemblyDescriptionAttribute>().FirstOrDefault();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.DisplayName = typeof(Service).FullName;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = typeof(Service).FullName;
            serviceInstaller.Description = assemblyDescription!=null ? assemblyDescription.Description : null;

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
