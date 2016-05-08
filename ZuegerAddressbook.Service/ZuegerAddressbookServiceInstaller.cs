using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace ZuegerAddressbook.Service
{
    [RunInstaller(true)]
    public class ZuegerAddressbookServiceInstaller : Installer
    {
        public ZuegerAddressbookServiceInstaller()
        {
            var service = new ServiceInstaller
            {
                DisplayName = "Züger Addressbook Service",
                ServiceName = "ZuegerAddressbook.Service"
            };

            var serviceProcess = new ServiceProcessInstaller
            {
                Account = ServiceAccount.User
            };

            service.StartType = ServiceStartMode.Automatic;
            service.DelayedAutoStart = true;

            Installers.Add(serviceProcess);
            Installers.Add(service);
        }
    }
}