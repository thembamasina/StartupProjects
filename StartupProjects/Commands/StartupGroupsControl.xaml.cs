﻿//------------------------------------------------------------------------------
// <copyright file="StartupGroupsControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows.Data;
using EnvDTE;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using StartupProjects.Shared;
using StartupProjects.ViewModels;

namespace StartupProjects.Commands
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for StartupGroupsControl.
    /// </summary>
    public partial class StartupGroupsControl : UserControl
    {
        private ProjectGroups _projectGroups;
        private CommandEvents _events;
        private StartupGroupViewModel _startupGroupViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupGroupsControl"/> class.
        /// </summary>
        public StartupGroupsControl()
        {
            this.InitializeComponent();
            var dte = ProjectHelpers.GetDetService();
            _events = dte.Events.CommandEvents;
            _events.AfterExecute += AfterExecute;
        }

        private void LoadProjects()
        {
            if (_projectGroups != null)
            {
                return;
            }

            _projectGroups = new ProjectGroups();
            _projectGroups.ReadStartupProjects();
            if (_projectGroups.Groups.Any())
            {
                PopulateStartupGroups();
            }
        }

        private void AfterExecute(string guid, int id, object customin, object customout)
        {
            if (!string.IsNullOrWhiteSpace(ProjectHelpers.Solution.FullName))
            {
                LoadProjects();

                mnuDelete.IsEnabled = true;
                mnuEdit.IsEnabled = true;
                mnuNew.IsEnabled = true;
            }
            else
            {
                _projectGroups = null;
                trvGroups.Items.Clear();
                mnuDelete.IsEnabled = false;
                mnuEdit.IsEnabled = false;
                mnuNew.IsEnabled = false;
            }
        }

        private void PopulateStartupGroups()
        {
            trvGroups.Items.Clear();
            var prop = ProjectHelpers.Solution.Properties.Item("Name");

            var solutionNode = new TreeViewItem
            {
                Header = prop.Value,
                Focusable = false,
            };

            foreach (var projectGroup in _projectGroups.Groups)
            {
                var projectNode = AddNode(projectGroup.GroupName, solutionNode);
                foreach (var projectName in projectGroup.SelectedProjects)
                {
                    var projectNameNode = AddNode(projectName, projectNode);
                    projectNameNode.Focusable = false;
                }
            }

            trvGroups.Items.Add(solutionNode);
        }

        private TreeViewItem AddNode(string text, TreeViewItem parent)
        {
            var treeViewItem = new TreeViewItem()
            {
                Header = text,
            };
            parent.Items.Add(treeViewItem);
            return treeViewItem;
        }

        private void DeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            var selectedNode = trvGroups.SelectedItem as TreeViewItem;
            System.Windows.Forms.MessageBox.Show(selectedNode.Header.ToString());
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var uiShell = ProjectHelpers.GetVsUiShell();
            var newGroup = new MyPopup(uiShell);
            //get the owner of this dialog
            IntPtr hwnd;
            uiShell.GetDialogOwnerHwnd(out hwnd);
            newGroup.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            uiShell.EnableModeless(0);
            try
            {
                _startupGroupViewModel = new StartupGroupViewModel(ProjectHelpers.GetAllProject());
                var content = new NewStartupProjectGroup {DataContext = _startupGroupViewModel};
                newGroup.Title = "New Startup Group";
                newGroup.Content = content;
                WindowHelper.ShowModal(newGroup, hwnd);

                var addedGroup = newGroup.Content as NewStartupProjectGroup;
                _projectGroups.Add(addedGroup.ProjectGroup);
                _projectGroups.SaveStartupProjects();

                PopulateStartupGroups();
            }
            finally
            {
                // This will take place after the window is closed.
                uiShell.EnableModeless(1);
            }
        }
    }
}