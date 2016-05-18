//------------------------------------------------------------------------------
// <copyright file="StartupGroupsControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Linq;
using EnvDTE;
using Microsoft.Internal.VisualStudio.PlatformUI;
using StartupProjects.Shared;
using StartupProjects.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace StartupProjects.Commands
{
    /// <summary>
    /// Interaction logic for StartupGroupsControl.
    /// </summary>
    public partial class StartupGroupsControl : UserControl
    {
        private ProjectGroups _projectGroups;
        private SolutionEvents _solutionEvents;
        private ProjectGroupViewModel _projectGroupViewModel;
        /// <summary>
        /// Initializes a new instance of the <see cref="StartupGroupsControl"/> class.
        /// </summary>
        public StartupGroupsControl()
        {
            this.InitializeComponent();
            var dte = ProjectHelpers.GetDteService();
            _solutionEvents = dte.Events.SolutionEvents;
            _solutionEvents.Opened += _solutionEvents_Opened;
            _solutionEvents.BeforeClosing += SolutionEventsOnBeforeClosing;
        }

        private void SolutionEventsOnBeforeClosing()
        {
            _projectGroups = null;
            trvGroups.Items.Clear();
            mnuDelete.IsEnabled = false;
            mnuEdit.IsEnabled = false;
            mnuNew.IsEnabled = false;
            mnuSetAsStartupGroup.IsEnabled = false;
        }

        private void _solutionEvents_Opened()
        {
            LoadProjects();
            mnuNew.IsEnabled = true;
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

        private void PopulateStartupGroups()
        {
            trvGroups.Items.Clear();
            var prop = ProjectHelpers.Solution.Properties.Item("Name");

            var solutionNode = new TreeViewItem
            {
                Header = prop.Value,
                Focusable = false,
                IsExpanded = true
            };

            foreach (var projectGroup in _projectGroups.Groups.Where(x=>x != null))
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
            newGroup.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            uiShell.EnableModeless(0);
            try
            {
                _projectGroupViewModel = new ProjectGroupViewModel(ProjectHelpers.GetAllProject());
                var content = new NewStartupProjectGroup {DataContext = _projectGroupViewModel};
                newGroup.Title = "Project Group";
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

        private void SetAsStartUpGroup(object sender, RoutedEventArgs e)
        {
            var selectedProjects = _projectGroups.Groups.First(x => x.IsSelected).SelectedProjects.ToArray<object>();

            ProjectHelpers.SetStartupProjects(selectedProjects);
            var control = StatusBarInjector.FindChild(Application.Current.MainWindow, "txtNumberOfPRojects");
            var textblock = control as TextBlock;
            if (textblock == null) return;

            var numberOfStartupProjects = ProjectHelpers.NumberOfStartupProjects;
            textblock.Dispatcher.Invoke(() =>
            {
                textblock.Text = $"Startup Projects: {numberOfStartupProjects}";
            });
        }

        private void trvGroups_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selected = e.NewValue as TreeViewItem;
            mnuSetAsStartupGroup.IsEnabled = false;
            mnuDelete.IsEnabled = false;

            if (selected == null) return;

            mnuSetAsStartupGroup.IsEnabled = true;
            mnuDelete.IsEnabled = true;

            var vm = DataContext as ProjectGroupViewModel;
            var selectedGroup = _projectGroups.Groups.Where(x=> x!=null).First(x => x.GroupName == selected.Header.ToString());
            _projectGroups.Groups.Where(x => x != null).ToList().ForEach(p => p.IsSelected = false);

            selectedGroup.IsSelected = true;
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ProjectGroupViewModel;
            var selectedGroup = _projectGroups.Groups.First(x=>x.IsSelected);
            _projectGroups.Remove(selectedGroup);
            _projectGroups.SaveStartupProjects();
            PopulateStartupGroups();
        }
    }
}