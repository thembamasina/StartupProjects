using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using StartupProjects.Shared;

namespace StartupProjects.Commands
{
    sealed class NumberOfProjectCommandHandler
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly TextBlock _control = new TextBlock
        {
            Foreground = Brushes.White,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(5, 4, 10, 0)
        };

        private DTE2 _dte;
        private StatusBarInjector _injector;
        private CommandEvents _events;
        private SolutionEvents _solutionEvents;

        private NumberOfProjectCommandHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _dte = (DTE2)serviceProvider.GetService(typeof(DTE));
            _events = _dte.Events.CommandEvents;

            _solutionEvents = _dte.Events.SolutionEvents;
/*
            _solutionEvents.ProjectAdded += _solutionEvents_ProjectAdded;
            _solutionEvents.ProjectRemoved += _solutionEvents_ProjectAdded;
*/

            _events.AfterExecute += AfterExecute;
            _control.MouseLeftButtonUp += ShowStartupProjects;

            _injector = new StatusBarInjector(Application.Current.MainWindow);
            _injector.InjectControl(_control);
        }

        /*private void _solutionEvents_ProjectAdded(Project Project)
        {
            var numberOfStartupProjects = ProjectHelpers.NumberOfStartupProjects;
            _control.Dispatcher.Invoke(() =>
            {
                _control.Text = $"Startup Projects: {numberOfStartupProjects}";
            });
        }
*/
        private void ShowStartupProjects(object sender, MouseButtonEventArgs e)
        {
            var message = string.Join(Environment.NewLine, ProjectHelpers.GetStartUpProjectName());

            VsShellUtilities.ShowMessageBox(
                           _serviceProvider,
                           "",
                           message,
                           OLEMSGICON.OLEMSGICON_INFO,
                           OLEMSGBUTTON.OLEMSGBUTTON_OK,
                           OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        public static void Initialize(IServiceProvider provider)
        {
            Instance = new NumberOfProjectCommandHandler(provider);
        }

        private static NumberOfProjectCommandHandler Instance { get; set; }

        private void AfterExecute(string guid, int id, object customin, object customout)
        {
            var numberOfStartupProjects = ProjectHelpers.NumberOfStartupProjects;
                _control.Dispatcher.Invoke(() =>
                {
                    _control.Text = $"Startup Projects: {numberOfStartupProjects}";
                });
        }
    }
}