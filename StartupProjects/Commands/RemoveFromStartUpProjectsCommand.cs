//------------------------------------------------------------------------------
// <copyright file="RemoveFromStartUpProjects.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell;
using StartupProjects.Shared;

namespace StartupProjects.Commands
{
    /// <summary>
    ///     Command handler
    /// </summary>
    internal sealed class RemoveFromStartUpProjectsCommand
    {
        public const int CommandId = 0x0200;
        public static readonly Guid CommandSet = new Guid("6aab571c-daed-43f3-9581-0e86325885b7");
        private readonly Package package;

        private RemoveFromStartUpProjectsCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        public static RemoveFromStartUpProjectsCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return package; }
        }

        public static void Initialize(Package package)
        {
            Debug.WriteLine("Initializing Remove from startup project");
            Instance = new RemoveFromStartUpProjectsCommand(package);
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            ProjectHelpers.RemoveFromtartupProjects(ProjectHelpers.GetProjects());
        }
    }
}