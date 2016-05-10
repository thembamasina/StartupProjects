using System.Windows;
using Microsoft.VisualStudio.Shell.Interop;

namespace StartupProjects.Commands
{
    public partial class MyPopup
        : Window
    {
        private readonly IVsUIShell _uiShell;

        public MyPopup(IVsUIShell uiShell)
        {
            _uiShell = uiShell;
        }
    }
}