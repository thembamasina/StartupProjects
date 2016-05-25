using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using StartupProjects.Shared;

namespace StartupProjects.ViewModels
{
    public class ProjectGroups
    {
        private readonly string _startupsJsonPath;

        public ProjectGroups()
        {
            Groups = new List<ProjectGroup>();
            var path = ProjectHelpers.Solution.FullName;
            var solutionName = ProjectHelpers.Solution.Properties.Item("Name").Value;
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var solutionFolder = Path.GetDirectoryName(path);
            _startupsJsonPath = Path.Combine(solutionFolder, string.Format("{0}-projects.groups", solutionName));

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

            if (!File.Exists(_startupsJsonPath))
            {
                return;
            }

            var groups = File.ReadAllText(_startupsJsonPath);
            if (string.IsNullOrWhiteSpace(groups))
            {
                return;
            }

            var startups = JsonConvert.DeserializeObject<List<ProjectGroup>>(groups);
            Groups = startups;
        }

        public void SaveStartupProjects()
        {
            var groups = JsonConvert.SerializeObject(Groups.Where(x => x != null));
            File.WriteAllText(_startupsJsonPath, groups);
        }

        public List<ProjectGroup> Groups { get; private set; }

        public void Add(ProjectGroup group)
        {
            Groups.Add(group);
        }

        public void Remove(ProjectGroup group)
        {
            Groups.Remove(group);
        }
    }
}