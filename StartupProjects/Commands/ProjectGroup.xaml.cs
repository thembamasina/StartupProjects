using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using StartupProjects.Shared;
using StartupProjects.ViewModels;

namespace StartupProjects.Commands
{
    /// <summary>
    ///     Interaction logic for NewStartupProjectGroup.xaml
    /// </summary>
    public partial class NewStartupProjectGroup : UserControl
    {
        public NewStartupProjectGroup()
        {
            InitializeComponent();
        }
        public ProjectGroup ProjectGroup { get; private set; }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ProjectGroupViewModel;
            if (string.IsNullOrWhiteSpace(vm.GroupName))
            {
                VsShellUtilities.ShowMessageBox(
                                         ProjectHelpers.ServiceProvider,
                                         "Please Enter group name",
                                         "Error",
                                         OLEMSGICON.OLEMSGICON_INFO,
                                         OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                         OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                txtName.Focus();
                return;
            }

            if (vm.ProjectGroup.SelectedProjects.Count == 0)
            {
                VsShellUtilities.ShowMessageBox(
                                         ProjectHelpers.ServiceProvider,
                                         "Please select at lease 1 project",
                                         "Error",
                                         OLEMSGICON.OLEMSGICON_INFO,
                                         OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                         OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                return;
            }

            ProjectGroup = vm.ProjectGroup;
            (Parent as DialogWindow).DialogResult = true;
        }
    }
}