using Newtonsoft.Json;
using Nodindow.myPackages;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nodindow.myPackages.TreeViewAdvanced;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using static Nodindow.myPackages.windowsManager;
using static Nodindow.nodindowObjects.windowCategoryBD;
using Nodindow.myPackages.formTools;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;
using System.Resources;

namespace Nodindow.nodindowObjects
{
    public class MonitorInfo : MonitorConfig
    {
        public TreeNodeAdvanced selectedTreeNodeSpaceInMonitor { get; set; }
        public MonitorInfo(string nameSys, string newName, SplitMonitor splitMonitor = null) : base(nameSys, newName, splitMonitor)
        {
            this.nameSys = nameSys;
            this.newName = newName;
            this.splitMonitor = splitMonitor;
        }
        public class ObjectParameterMonitorInfo : TreeViewAdvanced.TreeNodeAdvancedObjectParameter
        {
            public override object load(string json, TreeNodeAdvanced treeNodeAdvanced)
            {
                objectLoaded = JsonConvert.DeserializeObject<MonitorInfo>(json);
                ((MonitorInfo)objectLoaded).selectedTreeNodeSpaceInMonitor = null;
                return objectLoaded;
            }
            public ObjectParameterMonitorInfo(string nameParameter)
            {
                this.nameParameter = nameParameter;
            }
        }
    }
    public class MonitorConfig
    {
        public string nameSys { get; set; }
        public string newName { get; set; }
        public SplitMonitor splitMonitor { get; set; }
        public MonitorConfig(string nameSys, string newName, SplitMonitor splitMonitor = null)
        {
            this.nameSys = nameSys;
            this.newName = newName;
            this.splitMonitor = splitMonitor;
        }
        public Screen getCurrentMonitor()
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.DeviceName == this.nameSys)
                {
                    return screen;
                }
            }
            return null;
        }
        public class ObjectParameter : TreeViewAdvanced.TreeNodeAdvancedObjectParameter
        {
            public override object load(string json, TreeNodeAdvanced treeNodeAdvanced)
            {
                objectLoaded = JsonConvert.DeserializeObject<MonitorConfig>(json);
                return objectLoaded;
            }
            public ObjectParameter(string nameParameter)
            {
                this.nameParameter = nameParameter;
            }
        }
    }
    public class Monitors : ShowHideSpaces
    {
        public override void Show()
        {
            showBounds();
            void showBounds()
            {
                foreach (Screen monitor in Screen.AllScreens)
                {
                    globalValues.showBoundsView(new Size(monitor.Bounds.Width, monitor.Bounds.Height), new Point(monitor.Bounds.X, monitor.Bounds.Y), Color.Green);
                }
            }

            showSelected();
            if (this.form1.selectedTreeNodeSpace != this.treeNodeAdvanced)
            {
                ((ShowHideSpaces)this.form1.selectedTreeNodeSpace.userObject).Hide();
                selectTreeNode(this.treeNodeAdvanced);
            }
            void showSelected()
            {
                foreach (TreeNodeAdvanced monitor in this.treeNodeAdvanced.childrens)
                {
                    MonitorInfo monitorInfo = (MonitorInfo)monitor.userObject;
                    if (monitorInfo.selectedTreeNodeSpaceInMonitor == null)
                    {
                        monitorInfo.selectedTreeNodeSpaceInMonitor = monitor.childrens[0];
                    }
                    foreach (TreeNodeAdvanced form in monitorInfo.selectedTreeNodeSpaceInMonitor.childrens)
                    {
                        if (!((WindowInfoMore)form.userObject).windowInfo.getVisible())
                        {
                            this.form1.showHideWindow((WindowInfoMore)form.userObject, false);
                        }
                    }
                }
            }
        }
        public override void Hide()
        {
            foreach (TreeNodeAdvanced monitor in this.treeNodeAdvanced.childrens)
            {
                MonitorInfo monitorInfo = (MonitorInfo)monitor.userObject;
                foreach (TreeNodeAdvanced form in monitorInfo.selectedTreeNodeSpaceInMonitor.childrens)
                {
                    this.form1.showHideWindow((WindowInfoMore)form.userObject, true);
                }
            }
        }
        public Monitors(TreeNodeAdvanced treeNodeAdvanced, Form1 form1)
        {
            this.treeNodeAdvanced = treeNodeAdvanced;
            this.form1 = form1;
        }
        public class ObjectParameter : TreeViewAdvanced.TreeNodeAdvancedObjectParameter
        {
            Form1 form1 { get; set; }
            public override object load(string json, TreeNodeAdvanced treeNodeAdvanced)
            {
                return new Monitors(treeNodeAdvanced, form1);
            }
            public ObjectParameter(string nameParameter, Form1 form1)
            {
                this.nameParameter = nameParameter;
                this.form1 = form1;
            }
        }
    }

    public class SpaceMonitor : ShowHideSpaces
    {
        public override void Show()
        {
            MonitorInfo monitorInfo = (MonitorInfo)this.treeNodeAdvanced.parent.userObject;
            showBounds();
            void showBounds()
            {
                Screen monitor = form1.getMonitorByName(monitorInfo.nameSys);
                globalValues.showBoundsView(new Size(monitor.Bounds.Width, monitor.Bounds.Height), new Point(monitor.Bounds.X, monitor.Bounds.Y), Color.Green);
            }
            TreeNodeAdvanced monitors = this.treeNodeAdvanced.parent.parent;
            if (monitors != this.form1.selectedTreeNodeSpace)
            {
                ((Monitors)monitors.userObject).Show();
            }
            else
            {
                showSelected();
                if (monitorInfo.selectedTreeNodeSpaceInMonitor != this.treeNodeAdvanced)
                {
                    ((SpaceMonitor)monitorInfo.selectedTreeNodeSpaceInMonitor.userObject).Hide();
                    monitorInfo.selectedTreeNodeSpaceInMonitor = this.treeNodeAdvanced;
                }
                void showSelected()
                {
                    foreach (TreeNodeAdvanced form in this.treeNodeAdvanced.childrens)
                    {
                        if (!((WindowInfoMore)form.userObject).windowInfo.getVisible())
                        {
                            this.form1.showHideWindow((WindowInfoMore)form.userObject, false);
                        }
                    }
                }
            }
            selectTreeNode(monitors);
        }
        public override void Hide()
        {
            foreach (TreeNodeAdvanced form in this.treeNodeAdvanced.childrens)
            {
                form1.showHideWindow((WindowInfoMore)form.userObject, true);
            }
        }
        public SpaceMonitor(TreeNodeAdvanced treeNodeAdvanced, Form1 form1)
        {
            this.treeNodeAdvanced = treeNodeAdvanced;
            this.form1 = form1;
        }
        public class ObjectParameter : TreeViewAdvanced.TreeNodeAdvancedObjectParameter
        {
            Form1 form1 { get; set; }
            public override object load(string json, TreeNodeAdvanced treeNodeAdvanced)
            {
                return new SpaceMonitor(treeNodeAdvanced, form1);
            }
            public ObjectParameter(string nameParameter, Form1 form1)
            {
                this.nameParameter = nameParameter;
                this.form1 = form1;
            }
        }
    }
    public class Space : ShowHideSpaces
    {
        public override void Show()
        {
            showBounds();
            void showBounds()
            {
                foreach (Screen monitor in Screen.AllScreens)
                {
                    globalValues.showBoundsView(new Size(monitor.Bounds.Width, monitor.Bounds.Height), new Point(monitor.Bounds.X, monitor.Bounds.Y), Color.Blue);
                }
            }
            showSelected();
            if (this.treeNodeAdvanced != form1.selectedTreeNodeSpace)
            {
                ((ShowHideSpaces)this.form1.selectedTreeNodeSpace.userObject).Hide();
                selectTreeNode(this.treeNodeAdvanced);
            }
            void showSelected()
            {
                foreach (TreeNodeAdvanced form in this.treeNodeAdvanced.childrens)
                {
                    if (!((WindowInfoMore)form.userObject).windowInfo.getVisible())
                    {
                        form1.showHideWindow((WindowInfoMore)form.userObject, false);
                    }
                }
            }
        }
        public override void Hide()
        {
            foreach (TreeNodeAdvanced form in this.treeNodeAdvanced.childrens)
            {
                form1.showHideWindow((WindowInfoMore)form.userObject, true);
            }
        }
        public Space(TreeNodeAdvanced treeNodeAdvanced, Form1 form1)
        {
            this.treeNodeAdvanced = treeNodeAdvanced;
            this.form1 = form1;
        }
        public class ObjectParameter : TreeViewAdvanced.TreeNodeAdvancedObjectParameter
        {
            Form1 form1 { get; set; }
            public override object load(string json, TreeNodeAdvanced treeNodeAdvanced)
            {
                return new Space(treeNodeAdvanced, form1);
            }
            public ObjectParameter(string nameParameter, Form1 form1)
            {
                this.nameParameter = nameParameter;
                this.form1 = form1;
            }
        }
    }
    public class ShowHideSpaces
    {
        [JsonIgnore]
        public TreeNodeAdvanced treeNodeAdvanced { get; set; }
        [JsonIgnore]
        public Form1 form1 { get; set; }
        public ShowHideSpaces()
        {
        }
        public virtual void Show()
        {

        }
        public virtual void Hide()
        {

        }
        public void selectTreeNode(TreeNodeAdvanced treeNodeAdvanced)
        {
            this.form1.selectedTreeNodeSpace = treeNodeAdvanced;
            this.form1.treeViewAdvanced1.SelectedNode = treeNodeAdvanced.treeNode;
        }
    }

    public class SplitMonitor
    {
        public string name { get; set; }
        public Size size { get; set; }
        public Point point { get; set; }
    }
    /// <summary>
    /// Used to categorize windows based on the application's needs
    /// </summary>
    public class windowCategoryBD
    {
        /// <summary>
        /// all windows of all applications
        /// </summary>
        public List<WindowInfo> windowInfoBrute = new List<WindowInfo>();
        /// <summary>
        /// Windows not assigned to any user node.
        /// </summary>
        public List<WindowInfoMore> windowExcluded = new List<WindowInfoMore>();
        /// <summary>
        /// Windows awaiting inclusion.
        /// </summary>
        public List<WindowInfoMore> windowNotInclude = new List<WindowInfoMore>();
        /// <summary>
        /// Windows already assigned to a field in the user's nodes
        /// </summary>
        public List<WindowInfoMore> windowIncluded = new List<WindowInfoMore>();
        /// <summary>
        /// Prevents concurrent access by other threads.
        /// </summary>
        public readonly object lockObj = new object();
        private List<TreeNodeAdvanced> _windowTreeNodeAdvanced = new List<TreeNodeAdvanced>();
        public List<TreeNodeAdvanced> windowTreeNodeAdvanced
        {
            get { return this._windowTreeNodeAdvanced; }
            set
            {
                lock (lockObj)
                {
                    this._windowTreeNodeAdvanced = value;
                }
            }
        }
        public List<WindowInfoMore> modifiedWindow = new List<WindowInfoMore>();
        public Thread updateWindowsThread;

        public windowCategoryBD()
        {
            windowInfoBrute = windowsManager.GetAllWindowsInfo();
            foreach (WindowInfo windowInfo in windowInfoBrute)
            {
                windowExcluded.Add(new WindowInfoMore(windowInfo));
            }
            updateWindowsThread = new Thread(UpdateWindows);
            updateWindowsThread.Start();
        }
        public void addWindowTreeNodeAdvanced(TreeNodeAdvanced treeNodeAdvanced)
        {
            lock (lockObj)
            {
                windowTreeNodeAdvanced.Add(treeNodeAdvanced);
            }
        }
        public void addModifiedWindow(WindowInfoMore windowInfoMore)
        {
            lock (lockObj)
            {
                if (modifiedWindow.FindIndex(windowInfoMore1 => windowInfoMore1.windowInfo.Handle == windowInfoMore.windowInfo.Handle) == -1)
                {
                    modifiedWindow.Add(windowInfoMore);
                }
                globalValues.liteDBManager.updateData(globalValues.LiteDBManager.namesCollections.modifiedWindows.ToString(), JsonConvert.SerializeObject(modifiedWindow));
            }
        }
        /// <summary>
        /// Used to check for new windows that need to be categorized and subsequently added.
        /// </summary>
        private void UpdateWindows()
        {
            while (true)
            {
                lock (lockObj)
                {
                    windowInfoBrute = windowsManager.GetAllWindowsInfo();
                    foreach (WindowInfo windowInfo in windowInfoBrute)
                    {
                        if (windowsManager.IsWindowVisible(windowInfo.Handle) && (windowInfo.Size.Width != 0 && windowInfo.Size.Height != 0))
                        {
                            bool processExcludeNotExists = windowExcluded.FindIndex(windowExcluded => windowExcluded.windowInfo.Handle == windowInfo.Handle) == -1;
                            bool processIncludedNotExists = windowIncluded.FindIndex(windowIncluded => windowIncluded.windowInfo.Handle == windowInfo.Handle) == -1;
                            if (processExcludeNotExists && processIncludedNotExists)
                            {
                                if (getAllWindowHandlesAssociatedThisApplication().FindIndex(handle => handle == windowInfo.Handle) == -1)
                                {
                                    windowNotInclude.Add(new WindowInfoMore(windowInfo));
                                }
                                else
                                {
                                    windowExcluded.Add(new WindowInfoMore(windowInfo));
                                }
                            }
                        }
                    }
                    removeProcessesClosed();
                    void removeProcessesClosed()
                    {
                        windowExcluded = removeProcesses(windowExcluded);
                        windowNotInclude = removeProcesses(windowNotInclude);
                        windowIncluded = removeProcesses(windowIncluded);
                        List<WindowInfoMore> removeProcesses(List<WindowInfoMore> windows)
                        {
                            foreach (WindowInfoMore windowInfoMore1 in windows)
                            {
                                if (windowInfoMore1.windowInfo.Handle == IntPtr.Zero || windowInfoMore1.windowInfo.Handle == null)
                                {
                                    windows = windows.Where(windowT1 => windowT1.windowInfo.Handle != windowInfoMore1.windowInfo.Handle).ToList();
                                }
                            }
                            return windows;
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }
        /// <summary>
        /// This step is responsible for moving windows from one field to another, helping to facilitate the movement
        /// </summary>
        /// <param name="windowActions">Indicates where the window should be moved</param>
        /// <param name="windowInfoMore">Indicates the window that should be moved</param>
        public void moveWindows(windowActions windowActions, WindowInfoMore windowInfoMore)
        {
            lock (lockObj)
            {
                switch (windowActions)
                {
                    case windowActions.include:
                        windowExcluded = windowExcluded.Where(windowInfoT => windowInfoT.windowInfo != windowInfoMore.windowInfo).ToList();
                        windowNotInclude = windowNotInclude.Where(windowInfoT => windowInfoT.windowInfo != windowInfoMore.windowInfo).ToList();
                        windowIncluded.Add(new WindowInfoMore(windowInfoMore.windowInfo, windowInfoMore.hidedByUser));
                        break;
                    case windowActions.exclude:
                        windowNotInclude = windowNotInclude.Where(windowInfoT => windowInfoT.windowInfo != windowInfoMore.windowInfo).ToList();
                        windowIncluded = windowIncluded.Where(windowInfoT => windowInfoT.windowInfo != windowInfoMore.windowInfo).ToList();
                        windowExcluded.Add(new WindowInfoMore(windowInfoMore.windowInfo, windowInfoMore.hidedByUser));
                        break;
                    case windowActions.notIncluded:
                        windowExcluded = windowExcluded.Where(windowInfoT => windowInfoT.windowInfo != windowInfoMore.windowInfo).ToList();
                        windowIncluded = windowIncluded.Where(windowInfoT => windowInfoT.windowInfo != windowInfoMore.windowInfo).ToList();
                        windowNotInclude.Add(new WindowInfoMore(windowInfoMore.windowInfo, windowInfoMore.hidedByUser));
                        break;
                }
            }
        }
        public enum windowActions
        {
            include,
            exclude,
            notIncluded
        }
        public void StopUpdating()
        {
            updateWindowsThread.Abort();
        }
        public class WindowInfoMore
        {
            public WindowInfo windowInfo { get; set; }
            public bool hidedByUser { get; set; }
            public WindowInfoMore(WindowInfo windowInfo, bool hidedByUser = false)
            {
                this.windowInfo = windowInfo;
                this.hidedByUser = hidedByUser;
            }
        }
    }
    public class defaultParameters
    {
        public TreeNodeAdvanced.defaultParameters Space { get; set; }
        public TreeNodeAdvanced.defaultParameters Workspace { get; set; }
        public TreeNodeAdvanced.defaultParameters Monitors { get; set; }
        public TreeNodeAdvanced.defaultParameters Monitor { get; set; }
        public TreeNodeAdvanced.defaultParameters mainSpaceMonitor { get; set; }
        public TreeNodeAdvanced.defaultParameters SpaceMonitor { get; set; }
        public TreeNodeAdvanced.defaultParameters Form { get; set; }
        public TreeNodeAdvanced.defaultParameters Window { get; set; }
        public TreeNodeAdvanced.defaultParameters PackageForm { get; set; }

        //app
        public TreeNodeAdvanced.defaultParameters WorkspaceHub { get; set; }
        public TreeNodeAdvanced.defaultParameters MainWorkspace { get; set; }
        public TreeNodeAdvanced.defaultParameters MainSpace { get; set; }

        public TreeNodeAdvanced.defaultParameters Settings { get; set; }
        public TreeNodeAdvanced.defaultParameters _Monitors { get; set; }
        public TreeNodeAdvanced.defaultParameters _Monitor { get; set; }

        public TreeNodeAdvanced.defaultParameters ignoredForms { get; set; }

        public defaultParameters(TreeViewAdvanced treeViewAdvanced)
        {

            loadIcons();
            this.Space = new TreeNodeAdvanced.defaultParameters(((int)namesTreeNode.space).ToString(), treeViewAdvanced.TreeNodeAdvancedIconsBD.getIconAdvanced(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.space).ToString())), true, false, true, new List<string> { ((int)namesTreeNode.form).ToString(), ((int)namesTreeNode.packageForm).ToString() });
            this.Workspace = new TreeNodeAdvanced.defaultParameters(((int)namesTreeNode.workspace).ToString(), treeViewAdvanced.TreeNodeAdvancedIconsBD.getIconAdvanced(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.workspace).ToString())), true, false, true, new List<string> { ((int)namesTreeNode.workspace).ToString(), ((int)namesTreeNode.space).ToString(), ((int)namesTreeNode.monitors).ToString() });
            this.Monitors = new TreeNodeAdvanced.defaultParameters(((int)namesTreeNode.monitors).ToString(), treeViewAdvanced.TreeNodeAdvancedIconsBD.getIconAdvanced(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.monitors).ToString())), true, false, true, new List<string> { ((int)namesTreeNode.form).ToString(), ((int)namesTreeNode.packageForm).ToString() });
            this.Monitor = new TreeNodeAdvanced.defaultParameters(((int)namesTreeNode.monitor).ToString(), treeViewAdvanced.TreeNodeAdvancedIconsBD.getIconAdvanced(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.monitor).ToString())), true, false, false, new List<string> { ((int)namesTreeNode.spaceMonitor).ToString(), ((int)namesTreeNode.packageForm).ToString() });
            this.mainSpaceMonitor = new TreeNodeAdvanced.defaultParameters(((int)namesTreeNode.spaceMonitor).ToString(), treeViewAdvanced.TreeNodeAdvancedIconsBD.getIconAdvanced(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.space).ToString())), true, true, false, new List<string> { ((int)namesTreeNode.form).ToString(), ((int)namesTreeNode.packageForm).ToString() });
            this.SpaceMonitor = new TreeNodeAdvanced.defaultParameters(((int)namesTreeNode.spaceMonitor).ToString(), treeViewAdvanced.TreeNodeAdvancedIconsBD.getIconAdvanced(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.space).ToString())), true, false, true, new List<string> { ((int)namesTreeNode.form).ToString(), ((int)namesTreeNode.packageForm).ToString() });
            this.Form = new TreeNodeAdvanced.defaultParameters(((int)namesTreeNode.form).ToString(), treeViewAdvanced.TreeNodeAdvancedIconsBD.getIconAdvanced(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.form).ToString())), false, false, true);
            this.Window = new TreeNodeAdvanced.defaultParameters(((int)namesTreeNode.window).ToString(), treeViewAdvanced.TreeNodeAdvancedIconsBD.getIconAdvanced(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.window).ToString())), false);
            this.PackageForm = new TreeNodeAdvanced.defaultParameters(((int)namesTreeNode.packageForm).ToString(), treeViewAdvanced.TreeNodeAdvancedIconsBD.getIconAdvanced(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.packageForm).ToString())), false, false, true);

            //app
            this.WorkspaceHub = new TreeNodeAdvanced.defaultParameters(((int)namesTreeNode.workspaceHub).ToString(), treeViewAdvanced.TreeNodeAdvancedIconsBD.getIconAdvanced(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.workspaceHub).ToString())), true, true, false, new List<string> { ((int)namesTreeNode.workspace).ToString(), ((int)namesTreeNode.space).ToString(), ((int)namesTreeNode.monitors).ToString() });
            this.MainWorkspace = new TreeNodeAdvanced.defaultParameters(((int)namesTreeNode.workspace).ToString(), treeViewAdvanced.TreeNodeAdvancedIconsBD.getIconAdvanced(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.workspace).ToString())), true, true, false, new List<string> { ((int)namesTreeNode.workspace).ToString(), ((int)namesTreeNode.space).ToString(), ((int)namesTreeNode.monitors).ToString() });
            this.MainSpace = new TreeNodeAdvanced.defaultParameters(((int)namesTreeNode.space).ToString(), treeViewAdvanced.TreeNodeAdvancedIconsBD.getIconAdvanced(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.space).ToString())), true, true, false, new List<string> { ((int)namesTreeNode.form).ToString(), ((int)namesTreeNode.packageForm).ToString() });

            this.Settings = new TreeNodeAdvanced.defaultParameters(((int)namesTreeNode.config).ToString(), treeViewAdvanced.TreeNodeAdvancedIconsBD.getIconAdvanced(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.config).ToString())), true, true);
            this._Monitors = new TreeNodeAdvanced.defaultParameters(((int)namesTreeNode.configMonitors).ToString(), treeViewAdvanced.TreeNodeAdvancedIconsBD.getIconAdvanced(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.monitor).ToString())), true, true);
            this._Monitor = new TreeNodeAdvanced.defaultParameters(((int)namesTreeNode.ConfigMonitor).ToString(), treeViewAdvanced.TreeNodeAdvancedIconsBD.getIconAdvanced(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.monitor).ToString())), true, true);

            this.ignoredForms = new TreeNodeAdvanced.defaultParameters(((int)namesTreeNode.ignoredForms).ToString(), treeViewAdvanced.TreeNodeAdvancedIconsBD.getIconAdvanced(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.excludedForm).ToString())), false, true, false, new List<string> { ((int)namesTreeNode.form).ToString() });
            void loadIcons()
            {
                string appPath = Directory.GetCurrentDirectory();
                treeViewAdvanced.TreeNodeAdvancedIconsBD.add(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.workspaceHub).ToString(), globalValues.filePaths.appIcon));
                treeViewAdvanced.TreeNodeAdvancedIconsBD.add(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.excludedForm).ToString(), globalValues.filePaths.excludedFormIcon));
                treeViewAdvanced.TreeNodeAdvancedIconsBD.add(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.workspace).ToString(), globalValues.filePaths.workspaceIcon));
                treeViewAdvanced.TreeNodeAdvancedIconsBD.add(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.space).ToString(), globalValues.filePaths.spaceIcon));
                treeViewAdvanced.TreeNodeAdvancedIconsBD.add(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.form).ToString(), globalValues.filePaths.formIcon));
                treeViewAdvanced.TreeNodeAdvancedIconsBD.add(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.window).ToString(), globalValues.filePaths.windowIcon));
                treeViewAdvanced.TreeNodeAdvancedIconsBD.add(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.monitor).ToString(), globalValues.filePaths.monitorIcon));
                treeViewAdvanced.TreeNodeAdvancedIconsBD.add(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.monitors).ToString(), globalValues.filePaths.monitorsIcon));
                treeViewAdvanced.TreeNodeAdvancedIconsBD.add(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.config).ToString(), globalValues.filePaths.configIcon));
                treeViewAdvanced.TreeNodeAdvancedIconsBD.add(new TreeNodeAdvancedIcons.iconAdvanced(((int)namesTreeNode.packageForm).ToString(), globalValues.filePaths.packageFormIcon));
            }
        }
        public enum namesTreeNode
        {
            space,
            workspace,
            workspaceHub,
            monitors,
            monitor,
            spaceMonitor,
            form,
            window,
            packageForm,
            ignoredForms,
            config,
            configMonitors,
            ConfigMonitor,
            excludedForm
        }
    }
}
