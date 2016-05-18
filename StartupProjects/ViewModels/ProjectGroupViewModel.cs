using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.PlatformUI;
using StartupProjects.Annotations;
using StartupProjects.Commands;
using MessageBox = System.Windows.Forms.MessageBox;

namespace StartupProjects.ViewModels
{
    public class ProjectGroupViewModel : ViewModel 
    {
        private string _groupName;
        private ObservableCollection<ProjectViewModel> _projects;

        public string GroupName
        {
            get { return _groupName; }
            set
            {
                if (value == _groupName) return;
                _groupName = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProjectViewModel> Projects
        {
            get { return _projects; }
            set
            {
                if (Equals(value, _projects)) return;
                _projects = value;
                OnPropertyChanged();
            }
        }

        public ProjectGroupViewModel(IEnumerable<string> projects)
        {
            Projects = new ObservableCollection<ProjectViewModel>(
                projects
                    .Select(x => new ProjectViewModel(x))
                    .OrderBy(x => x.ProjectName)
                );
        }

        public ProjectGroup ProjectGroup
        {
            get
            {
                var group = new ProjectGroup(_groupName, _projects.Where(x=>x.Selected).Select(x=>x.ProjectName));
                return group;
            }
        }

       private bool CanExecute(object o)
        {
            return !string.IsNullOrWhiteSpace(_groupName) && _projects.Any(x => x.Selected);
        }

        private void GetProjectGroup(object obj)
        {
            MessageBox.Show(string.Join(",", ProjectGroup.SelectedProjects));
        }
    }
}