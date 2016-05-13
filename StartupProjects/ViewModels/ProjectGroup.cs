using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace StartupProjects.ViewModels
{
    public class ProjectGroup
    {
        [JsonConstructor]
        public ProjectGroup()
        {
            
        }

        public ProjectGroup(string name,IEnumerable<string> projects) : this(name)
        {
            SelectedProjects = projects.ToList();
        }

        public ProjectGroup(string groupName)
        {
            SelectedProjects = new List<string>();
            GroupName = groupName;
        }

        public string GroupName { get; set; }
        public List<string> SelectedProjects { get; set; }
        public bool IsSelected { get; set; }
        public void AddProject(string projectName)
        {
            SelectedProjects.Add(projectName);
        }
    }
}