//------------------------------------------------------------------------------
// <copyright file="AddToStartupProjectCommandPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

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
    public sealed class StartupProjectPackage : Package
    {
        #region Package Members
        /// <summary>
        ///     Initialization of the package; this method is called right after the package is sited, so this is the place
        ///     where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            ProjectHelpers.Initialize(this);
            AddToStartupProjectCommand.Initialize(this);
            RemoveFromStartUpProjectsCommand.Initialize(this);
            base.Initialize();

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                NumberOfProjectCommandHandler.Initialize(this);
            }), DispatcherPriority.ApplicationIdle, null);
        }

        public StartupProjectPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        #endregion
    }

/*
    internal class DteInitializer : IVsShellPropertyEvents
    {
        private IVsShell ShellService;
        private uint Cookie;
        private Action Callback;

        internal DteInitializer(IVsShell shellService, Action callback)
        {
            this.ShellService = shellService;
            this.Callback = callback;

            // Set an event handler to detect when the IDE is fully initialized
            int hr = this.ShellService.AdviseShellPropertyChanges(this, out this.Cookie);

            ErrorHandler.ThrowOnFailure(hr);
        }

        int IVsShellPropertyEvents.OnShellPropertyChange(int propid, object var)
        {
            if (propid != -9053)
            {
                return 0;
            }

            // Release the event handler to detect when the IDE is fully initialized
            int hr = this.ShellService.UnadviseShellPropertyChanges(this.Cookie);

            ErrorHandler.ThrowOnFailure(hr);

            this.Cookie = 0;

            this.Callback();

            return VSConstants.S_OK;
        }
    }
*/
}