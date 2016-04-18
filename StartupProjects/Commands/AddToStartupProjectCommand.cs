//------------------------------------------------------------------------------
// <copyright file="AddToStartupProjectCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace StartupProjectss
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AddToStartupProjectCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6aab571c-daed-43f3-9581-0e86325885b7");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddToStartupProjectCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private AddToStartupProjectCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AddToStartupProjectCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new AddToStartupProjectCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var dte2 = (DTE2)ServiceProvider.GetService(typeof(DTE));
            var selectedProject = GetSelectedProjects(dte2);
            var solution = dte2.Solution;
            AddSelectedProjectToStartup(selectedProject, solution);
        }

        private void AddSelectedProjectToStartup(IEnumerable<string> selectedProject, Solution solution)
        {
            var startupProjects = ((object[]) solution.SolutionBuild.StartupProjects).ToList();
            startupProjects.AddRange(selectedProject);
            solution.SolutionBuild.StartupProjects = startupProjects.Distinct().ToArray(); 
            solution.SaveAs(solution.FileName);
        }

        private static IEnumerable<string> GetSelectedProjects(DTE2 dte2)
        {
            var items = (Array)dte2.ToolWindows.SolutionExplorer.SelectedItems;

            var selectedProjects = from item in items.Cast<UIHierarchyItem>()
                                   let project = item.Object as Project
                                   select project.UniqueName;
            return selectedProjects;
        }
    }
}
