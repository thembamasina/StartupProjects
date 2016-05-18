using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.PlatformUI;
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
            ProjectGroup = vm.ProjectGroup;
            (Parent as DialogWindow).DialogResult = true;
        }
    }
}