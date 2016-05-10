//------------------------------------------------------------------------------
// <copyright file="StartupGroupsControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Data;
using EnvDTE;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;
using StartupProjects.Shared;

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
        /// <summary>
        /// Initializes a new instance of the <see cref="StartupGroupsControl"/> class.
        /// </summary>
        public StartupGroupsControl()
        {
            this.InitializeComponent();
            var dte = ProjectHelpers.GetDetService();

            _events = dte.Events.CommandEvents;
            _events.AfterExecute += AfterExecute;
            //_projectGroups = new ProjectGroups();
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

            Property prop = ProjectHelpers.Solution.Properties.Item("Name");
        
            var solutionNode = new TreeViewItem
            {
                Header = prop.Value,
                Focusable = false,
            };

             
            foreach (var projectGroup in _projectGroups.Groups)
            {
                var projectNode = AddNode(projectGroup.GroupName, solutionNode);
                foreach (var projectName in projectGroup.ProjectNames)
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

        private void NewButtonClicked(object sender, RoutedEventArgs e)
        {
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
            var testDialog2 = new MyPopup(uiShell);
            //get the owner of this dialog
            IntPtr hwnd;
            uiShell.GetDialogOwnerHwnd(out hwnd);
            testDialog2.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            uiShell.EnableModeless(0);
            try
            {
                testDialog2.Content = new StartupGroupsControl();
                WindowHelper.ShowModal(testDialog2, hwnd);
            }
            finally
            {
                // This will take place after the window is closed.
                uiShell.EnableModeless(1);
            }
        }
    }


    public class ProjectGroup
    {
        public ProjectGroup(string groupName)
        {
            ProjectNames = new List<string>();
            GroupName = groupName;
        }

        public string GroupName { get; set; }
        public List<string> ProjectNames { get; }
        public bool IsSelected { get; set; }
        public void AddProject(string projectName)
        {
            ProjectNames.Add(projectName);
        }
    }

    public class ProjectGroups
    {
        private readonly string _startupsJsonPath;

        public ProjectGroups()
        {
            Groups = new List<ProjectGroup>();
            var path = ProjectHelpers.Solution.FullName;
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var solutionFolder = Path.GetDirectoryName(path);
            _startupsJsonPath = Path.Combine(solutionFolder, "startups.json");

            if (!File.Exists(_startupsJsonPath))
            {
                File.Create(_startupsJsonPath);
            }
        }

        public void ReadStartupProjects()
        {
            if (string.IsNullOrWhiteSpace(_startupsJsonPath))
            {
                return;
            }

            var groups = File.ReadAllText(_startupsJsonPath);
            var startups = JsonConvert.DeserializeObject<ProjectGroups>(groups);
            this.Groups = startups.Groups;
        }

        private void SaveStartupProjects()
        {
            var startups = JsonConvert.SerializeObject(_startupsJsonPath);
            File.WriteAllText(_startupsJsonPath, startups);
        }

        public List<ProjectGroup> Groups { get; private set; }

        public void Add(ProjectGroup group)
        {
            Groups.Add(group);
        }
    }
}