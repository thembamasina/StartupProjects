using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using StartupProjects.Commands;
using StartupProjects.Shared;
using StartupProjectss;

namespace StartupProjects
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid("a6f62261-b200-41f4-8827-3ce7ff087848")]
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    [ProvideToolWindow(typeof(StartupProjects.Commands.StartupGroups))]
    public sealed class StartupProjectPackage : Package
    {
        protected override void Initialize()
        {
            ProjectHelpers.Initialize(this);
            AddToStartupProjectCommand.Initialize(this);
            RemoveFromStartUpProjectsCommand.Initialize(this);
            base.Initialize();

            NumberOfProjectCommandHandler.Initialize(this);
/*
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
            }), DispatcherPriority.ApplicationIdle, null);
*/
            StartupGroupsCommand.Initialize(this);
        }

        public StartupProjectPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }
    }
}