using System.ComponentModel;
using System.Runtime.CompilerServices;
using StartupProjects.Annotations;

namespace StartupProjects.ViewModels
{
    public class ProjectViewModel : ViewModel
    {
        public ProjectViewModel(string project)
        {
            ProjectName = project;
        }

        private string _projectName;
        private bool _selected;

        public string ProjectName
        {
            get { return _projectName; }
            set
            {
                if (value == _projectName) return;
                _projectName = value;
                OnPropertyChanged();
            }
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (value == _selected) return;
                _selected = value;
                OnPropertyChanged();
            }
        }

    }
}