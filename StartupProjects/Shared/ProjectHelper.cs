using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace StartupProjects.Shared
{
    internal static class ProjectHelpers
    {
        private static IServiceProvider _serviceProvider;

        public static Solution Solution
        {
            get
            {
                var dte2 = (DTE2)_serviceProvider.GetService(typeof(DTE));
                return dte2.Solution;
            }
        }

        public static string GetProjects()
        {
            var dte2 = (DTE2) _serviceProvider.GetService(typeof(DTE));
            return GetSelectedProject(dte2);
        }

        private static string GetSelectedProject(DTE2 dte2)
        {
            var items = (Array)dte2.ToolWindows.SolutionExplorer.SelectedItems;

            var selectedProjects = from item in items.Cast<UIHierarchyItem>()
                                   let project = item.Object as Project
                                   select project;
            Project first = selectedProjects.First();
            return first.UniqueName;
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private static List<object> GetStartUpProjects()
        {
            return (((object[]) Solution.SolutionBuild.StartupProjects) ?? new List<object>().ToArray()).ToList();
        }

        public static void AddToStartupProjects(string selectedProject)
        {
            var startupProjects = GetStartUpProjects();

            if (startupProjects.Contains(selectedProject))
            {
                return;
            }

            startupProjects.Add(selectedProject);
            Solution.SolutionBuild.StartupProjects = startupProjects.ToArray();
            Solution.SaveAs(Solution.FileName);
        }

        public static void RemoveFromtartupProjects(string selectedProject)
        {
            var startupProjects = GetStartUpProjects();

            if (!startupProjects.Contains(selectedProject))
            {
                return;
            }

            if (startupProjects.Count == 1 && startupProjects[0].ToString() == selectedProject)
            {
                return;
            }

            startupProjects.Remove(selectedProject);
            var list = new List<string>();
            foreach (Property property in Solution.Properties)
            {
               list.Add(property.Name); 
            }

            if (startupProjects.Count == 1)
            {
                SetStartupProject(startupProjects[0].ToString());
            }
            else
            {
                Solution.SolutionBuild.StartupProjects = startupProjects.ToArray();
            }
            Solution.SaveAs(Solution.FileName);
        }

        private static void SetStartupProject(string startupProjects)
        {
            var startup = Solution.Properties.Item("StartupProject");
            var project = GetProjectByUniqueName(startupProjects);
            startup.Value = project;
        }

        private static string GetProjectByUniqueName(string startupProject)
        {
            return (from Project project in Solution.Projects where project.UniqueName == startupProject select project.Name).FirstOrDefault();
        }
    }
}