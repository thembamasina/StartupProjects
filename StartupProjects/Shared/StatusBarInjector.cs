using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StartupProjects.Shared
{
    internal class StatusBarInjector
    {
        private Panel panel;

        public FrameworkElement statusBar;
        private readonly Window window;

        public StatusBarInjector(Window pWindow)
        {
            window = pWindow;
            window.Initialized += window_Initialized;

            FindStatusBar();
        }

        private static DependencyObject FindChild(DependencyObject parent, string childName)
        {
            if (parent == null)
            {
                return null;
            }
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var frameworkElement = child as FrameworkElement;
                if (frameworkElement != null && frameworkElement.Name == childName)
                {
                    return frameworkElement;
                }
                child = FindChild(child, childName);
                if (child != null)
                {
                    return child;
                }
            }
            return null;
        }

        private void FindStatusBar()
        {
            statusBar = FindChild(window, "StatusBarContainer") as FrameworkElement;
            panel = statusBar.Parent as DockPanel;
        }

        private static FrameworkElement FindStatusBarContainer(Panel panel)
        {
            FrameworkElement frameworkElement;
            var enumerator = panel.Children.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current as FrameworkElement;
                    if (current == null || !(current.Name == "StatusBarContainer"))
                    {
                        continue;
                    }
                    frameworkElement = current;
                    return frameworkElement;
                }
                return null;
            }
            finally
            {
                var disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        public void InjectControl(FrameworkElement pControl)
        {
            panel.Dispatcher.Invoke(() =>
            {
                pControl.SetValue(DockPanel.DockProperty, Dock.Right);
                panel.Children.Insert(1, pControl);
            });
        }

        public bool IsInjected(FrameworkElement pControl)
        {
            var flag2 = false;
            panel.Dispatcher.Invoke(() =>
            {
                var flag = panel.Children.Contains(pControl);
                var flag1 = flag;
                flag2 = flag;
                return flag1;
            });
            return flag2;
        }

        public void UninjectControl(FrameworkElement pControl)
        {
            panel.Dispatcher.Invoke(() => panel.Children.Remove(pControl));
        }

        private void window_Initialized(object sender, EventArgs e)
        {
        }
    }
}