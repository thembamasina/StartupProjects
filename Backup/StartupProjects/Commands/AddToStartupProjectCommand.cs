﻿//------------------------------------------------------------------------------
// <copyright file="AddToStartupProjectCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using StartupProjects.Shared;

namespace StartupProjectss
{
    /// <summary>
    ///     Command handler
    /// </summary>
    internal sealed class AddToStartupProjectCommand
    {
        /// <summary>
        ///     Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        ///     Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6aab571c-daed-43f3-9581-0e86325885b7");

//        public static readonly Guid CommandSet = new Guid(PackageGuids.guidAddToStartupProjectCommandPackageCmdSetString);

        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AddToStartupProjectCommand" /> class.
        ///     Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private AddToStartupProjectCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            var commandService =
                ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        ///     Gets the instance of the command.
        /// </summary>
        public static AddToStartupProjectCommand Instance { get; private set; }

        /// <summary>
        ///     Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get { return package; }
        }

        /// <summary>
        ///     Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new AddToStartupProjectCommand(package);
        }

        /// <summary>
        ///     This function is the callback used to execute the command when the menu item is clicked.
        ///     See the constructor to see how the menu item is associated with this function using
        ///     OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var selectedProject = ProjectHelpers.GetProjects();
            AddSelectedProjectToStartup(selectedProject, ProjectHelpers.Solution);
        }

        private void AddSelectedProjectToStartup(string selectedProject, Solution solution)
        {
            ProjectHelpers.AddToStartupProjects(selectedProject);
        }
    }
}