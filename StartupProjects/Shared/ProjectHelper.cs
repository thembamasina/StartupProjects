using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace StartupProjects.Shared
{
    internal static class ProjectHelpers
    {
        private static IServiceProvider _serviceProvider;

        public static Solution Solution
        {
            get
            {
                var dte2 = (DTE2) _serviceProvider.GetService(typeof(DTE));
                return dte2.Solution;
            }
        }

        public static int NumberOfStartupProjects => GetStartUpProjects().Count;

        public static string GetProjects()
        {
            var dte2 = (DTE2) _serviceProvider.GetService(typeof(DTE));
            return GetSelectedProject(dte2);
        }

        private static string GetSelectedProject(DTE2 dte2)
        {
            var items = (Array) dte2.ToolWindows.SolutionExplorer.SelectedItems;

            var selectedProjects = from item in items.Cast<UIHierarchyItem>()
                let project = item.Object as Project
                select project;
            var first = selectedProjects.First();
            return first.UniqueName;
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static List<object> GetStartUpProjects()
        {
            return ((object[]) Solution.SolutionBuild.StartupProjects ?? new List<object>().ToArray()).ToList();
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
            foreach (Project project in GetAllProjectsInSolution())
            {
                if (project.UniqueName == startupProject)
                {
                    string s = project.Name;
                    return s;
                }
            }
            return null;
        }

        private static string GetProjectByName(string startupProject)
        {
            foreach (Project project in GetAllProjectsInSolution())
            {
                if (project.Name == startupProject)
                {
                    string s = project.UniqueName;
                    return s;
                }
            }
            return null;
        }

        public static IEnumerable<string> GetStartUpProjectName()
        {
            var startUpProjects = GetStartUpProjects();
            return startUpProjects.Select(x => GetProjectByUniqueName(x.ToString()));
        }

        public static IEnumerable<string> GetAllProject()
        {
            return GetAllProjectsInSolution().Select(x => GetProjectByUniqueName(x.UniqueName));
        }

        public static IEnumerable<Project> GetAllProjectsInSolution()
        {
            var solution = (IVsSolution)_serviceProvider.GetService(typeof(IVsSolution));

            foreach (IVsHierarchy hier in GetProjectsInSolution(solution))
            {
                EnvDTE.Project project = GetDTEProject(hier);
                if (!string.IsNullOrWhiteSpace(project?.FileName))
                    yield return project;
            }
        }

        public static IEnumerable<IVsHierarchy> GetProjectsInSolution(IVsSolution solution)
        {
            return GetProjectsInSolution(solution, __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION);
        }

        public static IEnumerable<IVsHierarchy> GetProjectsInSolution(IVsSolution solution, __VSENUMPROJFLAGS flags)
        {
            if (solution == null)
                yield break;

            IEnumHierarchies enumHierarchies;
            Guid guid = Guid.Empty;
            solution.GetProjectEnum((uint)flags, ref guid, out enumHierarchies);
            if (enumHierarchies == null)
                yield break;

            IVsHierarchy[] hierarchy = new IVsHierarchy[1];
            uint fetched;
            while (enumHierarchies.Next(1, hierarchy, out fetched) == VSConstants.S_OK && fetched == 1)
            {
                if (hierarchy.Length > 0 && hierarchy[0] != null)
                    yield return hierarchy[0];
            }
        }

        public static Project GetDTEProject(IVsHierarchy hierarchy)
        {
            if (hierarchy == null)
                throw new ArgumentNullException(nameof(hierarchy));

            object obj;
            hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out obj);
            return obj as Project;
        }

        public static IVsUIShell GetVsUiShell()
        {
            return (IVsUIShell) _serviceProvider.GetService(typeof(SVsUIShell));
        }

        public static DTE2 GetDteService()
        {
            return (DTE2)_serviceProvider.GetService(typeof(DTE));
        }

        public static void SetStartupProjects(object[] selectedProjects)
        {
            if (selectedProjects.Length == 1)
            {
                SetStartupProject(selectedProjects[0].ToString());
            }
            else
            {
                Solution.SolutionBuild.StartupProjects = null;
                Solution.SaveAs(Solution.FileName);

                foreach (var selectedProject in selectedProjects)
                {

                    var projectName = GetProjectByName(selectedProject.ToString());
                    AddToStartupProjects(projectName);
                }
            }
        }
    }
}