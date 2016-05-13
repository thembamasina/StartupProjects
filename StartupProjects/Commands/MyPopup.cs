using System.Windows;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell.Interop;

namespace StartupProjects.Commands
{
    public partial class MyPopup
        : DialogWindow
    {
        private readonly IVsUIShell _uiShell;

        public MyPopup(IVsUIShell uiShell)
        {
            _uiShell = uiShell;
/*
            Width = 400;
            Height = 400;
*/
/*
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle = WindowStyle.SingleBorderWindow;
*/

            
        }
    }
}