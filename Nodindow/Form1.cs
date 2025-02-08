using Nodindow.myPackages;
using Nodindow.myPackages.formTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Nodindow.myPackages.TreeViewAdvanced;
using static Nodindow.myPackages.windowsManager;
using static Nodindow.nodindowObjects.windowCategoryBD;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Nodindow.nodindowObjects;
using static Nodindow.nodindowObjects.defaultParameters;
using System.Xml.Linq;
using LiteDB;
namespace Nodindow
{
    public partial class Form1 : Form
    {
        public nodindowObjects.forms.configuration configuration { get; set; }
        string appPath = Directory.GetCurrentDirectory();
        /// <summary>
        /// espaço persistente onde será adicionado as novas janelas das aplicações quando o usuario abrir uma nova aplicação e não tiver nenhum space selecionado pelo usuario
        /// </summary>
        TreeNodeAdvanced mainTreeNodeSpace { get; set; }
        /// <summary>
        /// espaço selecionado pelo usuario onde as novas janelas das aplicações será exibido
        /// </summary>
        public TreeNodeAdvanced selectedTreeNodeSpace { get; set; }
        windowCategoryBD windowCategoryBD = new windowCategoryBD();
        List<MonitorConfig> monitors = new List<MonitorConfig>();
        defaultParameters defaultParameters { get; set; }
        System.Windows.Forms.Timer updateSelectedTreeNodeSpace = new System.Windows.Forms.Timer();
        public Form1()
        {
            setInitialValuesOfModifiedWindows();
            void setInitialValuesOfModifiedWindows()
            {
                List<windowCategoryBD.WindowInfoMore> modifiedWindows = JsonConvert.DeserializeObject<List<windowCategoryBD.WindowInfoMore>>(globalValues.liteDBManager.getData(globalValues.LiteDBManager.namesCollections.modifiedWindows.ToString()));
                if (modifiedWindows != null)
                {
                    foreach (windowCategoryBD.WindowInfoMore window in modifiedWindows)
                    {
                        window.windowInfo.InitialValues.setInitialValues();
                    }
                    globalValues.liteDBManager.updateData(globalValues.LiteDBManager.namesCollections.modifiedWindows.ToString(),"");
                }
            }
            globalValues.loadTranslations();
            if (globalValues.liteDBManager.LiteDatabase.CollectionExists(globalValues.LiteDBManager.namesCollections.currentLanguage.ToString()))
            {
                globalValues.language = globalValues.liteDBManager.getData(globalValues.LiteDBManager.namesCollections.currentLanguage.ToString());
            }
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mainLoad();
            void mainLoad()
            {
                loadMonitors();
                void loadMonitors()
                {
                    foreach (Screen screen in Screen.AllScreens)
                    {
                        this.monitors.Add(new MonitorInfo(screen.DeviceName, screen.DeviceName));
                    }
                }
            }
            mainUpdateTreeView();
            void mainUpdateTreeView()
            {
                treeViewAdvanced1.AllowDrop = true;
                this.defaultParameters = new defaultParameters(treeViewAdvanced1);
                #region loadAllNodes
                loadNodes();
                void loadNodes()
                {
                    #region loadNodesWorkspaceSpacesAndWindows
                    if (globalValues.liteDBManager.LiteDatabase.CollectionExists(globalValues.LiteDBManager.namesCollections.mainTreeViewAdvanced.ToString()))
                    {
                        loadSaveNodes();
                        this.selectedTreeNodeSpace = treeViewAdvanced1.TreeNodeAdvancedBD.Find(treeNodeAdvanced => treeNodeAdvanced.name == ((int)namesTreeNode.workspaceHub).ToString() && treeNodeAdvanced.locked == true).childrens[0].childrens[0];
                        this.mainTreeNodeSpace = treeViewAdvanced1.TreeNodeAdvancedBD.Find(treeNodeAdvanced => treeNodeAdvanced.name == ((int)namesTreeNode.workspaceHub).ToString() && treeNodeAdvanced.locked == true).childrens[0].childrens[0];
                        TreeNodeAdvanced monitorsT = treeViewAdvanced1.TreeNodeAdvancedBD.Find(treeNodeAdvanced => treeNodeAdvanced.name == ((int)namesTreeNode.config).ToString() && treeNodeAdvanced.locked == true).childrens.Find(treeNodeAdvanced => treeNodeAdvanced.name == ((int)namesTreeNode.configMonitors).ToString() && treeNodeAdvanced.locked == true);
                        foreach (TreeNodeAdvanced monitor in monitorsT.childrens)
                        {
                            this.monitors[this.monitors.FindIndex(monitorT => monitorT.nameSys == ((MonitorConfig)monitor.userObject).nameSys)] = (MonitorConfig)monitor.userObject;
                        }
                        TreeNodeAdvanced excludedForm = treeViewAdvanced1.addTreeNodeWithoutParent(new TreeNodeAdvanced(globalValues.translationsManager.mainFormTranslation.treeNodeAdvanced.ignoredForms, null, this.defaultParameters.ignoredForms));
                        foreach (windowCategoryBD.WindowInfoMore windowInfoMore in windowCategoryBD.windowExcluded)
                        {
                            if (windowInfoMore.windowInfo.Title != "")
                            {
                                loadWindowsNodes(excludedForm, windowInfoMore);
                            }
                        }
                    }
                    else
                    {
                        loadMainNodes();
                    }
                    void loadMainNodes()
                    {
                        loadConfig();
                        loadworkSpaceHub();
                        loadNotIncludeForm();
                        void loadConfig()
                        {
                            TreeNodeAdvanced config = treeViewAdvanced1.addTreeNodeWithoutParent(new TreeNodeAdvanced(globalValues.translationsManager.mainFormTranslation.treeNodeAdvanced.settings, null, this.defaultParameters.Settings));
                            TreeNodeAdvanced monitoresConfig = config.add(new TreeNodeAdvanced(globalValues.translationsManager.mainFormTranslation.treeNodeAdvanced.monitors, null, this.defaultParameters._Monitors));
                            foreach (MonitorInfo monitor in this.monitors)
                            {
                                monitoresConfig.add(new TreeNodeAdvanced(monitor.newName, monitor, this.defaultParameters._Monitor));
                            }
                        }
                        void loadworkSpaceHub()
                        {
                            TreeNodeAdvanced workSpaceHub = treeViewAdvanced1.addTreeNodeWithoutParent(new TreeNodeAdvanced(globalValues.translationsManager.mainFormTranslation.treeNodeAdvanced.sphereOfWork, null, this.defaultParameters.WorkspaceHub));
                            TreeNodeAdvanced mainWorkSpace = workSpaceHub.add(new TreeNodeAdvanced(globalValues.translationsManager.mainFormTranslation.treeNodeAdvanced.mainWorkspace, null, this.defaultParameters.MainWorkspace));
                            TreeNodeAdvanced mainSpace = mainWorkSpace.add(new TreeNodeAdvanced(globalValues.translationsManager.mainFormTranslation.treeNodeAdvanced.mainSpace, null, this.defaultParameters.MainSpace));
                            mainSpace.userObject = new Space(mainSpace, this);
                            this.selectedTreeNodeSpace = mainSpace;
                            mainTreeNodeSpace = selectedTreeNodeSpace;
                        }
                        void loadNotIncludeForm()
                        {
                            TreeNodeAdvanced ignoredForms = treeViewAdvanced1.addTreeNodeWithoutParent(new TreeNodeAdvanced(globalValues.translationsManager.mainFormTranslation.treeNodeAdvanced.ignoredForms, null, this.defaultParameters.ignoredForms));
                            foreach (windowCategoryBD.WindowInfoMore windowInfoMore in windowCategoryBD.windowExcluded)
                            {
                                if (windowInfoMore.windowInfo.Title != "")
                                {
                                    loadWindowsNodes(ignoredForms, windowInfoMore);
                                }
                            }
                        }
                    }
                    void loadSaveNodes()
                    {
                        List<TreeNodeAdvancedObjectParameter> objectParameter = new List<TreeNodeAdvancedObjectParameter>();
                        objectParameter.Add(new MonitorConfig.ObjectParameter(((int)namesTreeNode.ConfigMonitor).ToString()));
                        objectParameter.Add(new MonitorInfo.ObjectParameterMonitorInfo(((int)namesTreeNode.monitor).ToString()));
                        objectParameter.Add(new Monitors.ObjectParameter(((int)namesTreeNode.monitors).ToString(), this));
                        objectParameter.Add(new SpaceMonitor.ObjectParameter(((int)namesTreeNode.spaceMonitor).ToString(), this));
                        objectParameter.Add(new Space.ObjectParameter(((int)namesTreeNode.space).ToString(), this));
                        treeViewAdvanced1.loadJsonSaved(globalValues.liteDBManager.getData(globalValues.LiteDBManager.namesCollections.mainTreeViewAdvanced.ToString()), objectParameter);
                    }
                    #endregion
                }
                #endregion
                #region threadUpdateProcesses
                startUpdateProcesses();
                void startUpdateProcesses()
                {
                    updateSelectedTreeNodeSpace.Interval = 100;
                    updateSelectedTreeNodeSpace.Tick += updateProcess_Tick;
                    updateSelectedTreeNodeSpace.Enabled = true;
                    updateSelectedTreeNodeSpace.Start();
                    void updateProcess_Tick(object sender1, EventArgs e1)
                    {
                        lock (windowCategoryBD.lockObj)
                        {
                            if (this.selectedTreeNodeSpace != null && this.windowCategoryBD.windowNotInclude != null)
                            {
                                foreach (windowCategoryBD.WindowInfoMore windowInfoMore1 in this.windowCategoryBD.windowNotInclude)
                                {
                                    if ((windowsManager.IsWindowVisible(windowInfoMore1.windowInfo.Handle) || windowInfoMore1.hidedByUser) && windowInfoMore1.windowInfo.Title != "")
                                    {
                                        TraceFlow.references.getReferences(this.selectedTreeNodeSpace);
                                        loadWindowsNodes(this.selectedTreeNodeSpace, windowInfoMore1);
                                        this.windowCategoryBD.moveWindows(windowCategoryBD.windowActions.include, windowInfoMore1);
                                    }
                                }
                            }
                            foreach (TreeNodeAdvanced treeNodeAdvanced in this.windowCategoryBD.windowTreeNodeAdvanced)
                            {
                                WindowInfoMore window = (WindowInfoMore)treeNodeAdvanced.userObject;
                                moveFormsInMonitor();
                                RemoveForms();
                                void moveFormsInMonitor()
                                {
                                    if (treeNodeAdvanced.parent.name == ((int)namesTreeNode.spaceMonitor).ToString())
                                    {
                                        window.windowInfo.updateWindowInfo();
                                        if (window.windowInfo.IsVisible != false || window.hidedByUser == true)
                                        {
                                            TreeNodeAdvanced monitorWithLargestArea = GetMonitorWithLargestArea(window.windowInfo, treeNodeAdvanced.parent.parent.parent);
                                            if (monitorWithLargestArea != null)
                                            {
                                                TreeNodeAdvanced treeNodeMonitor = ((MonitorInfo)monitorWithLargestArea.userObject).selectedTreeNodeSpaceInMonitor;
                                                if (treeNodeMonitor.treeNode == null)
                                                {
                                                    ((MonitorInfo)monitorWithLargestArea.userObject).selectedTreeNodeSpaceInMonitor = monitorWithLargestArea.childrens[0];
                                                    treeNodeMonitor = ((MonitorInfo)monitorWithLargestArea.userObject).selectedTreeNodeSpaceInMonitor;
                                                }
                                                if (((MonitorInfo)monitorWithLargestArea.userObject).nameSys != ((MonitorInfo)treeNodeAdvanced.parent.parent.userObject).nameSys)
                                                {
                                                    ((MonitorInfo)monitorWithLargestArea.userObject).selectedTreeNodeSpaceInMonitor.add(treeNodeAdvanced);
                                                    this.windowCategoryBD.windowTreeNodeAdvanced = this.windowCategoryBD.windowTreeNodeAdvanced.Where(TreeViewAdvancedT => TreeViewAdvancedT != treeNodeAdvanced).ToList();
                                                    this.windowCategoryBD.windowTreeNodeAdvanced.Add(((MonitorInfo)monitorWithLargestArea.userObject).selectedTreeNodeSpaceInMonitor.childrens[((MonitorInfo)monitorWithLargestArea.userObject).selectedTreeNodeSpaceInMonitor.childrens.Count - 1]);
                                                    treeNodeAdvanced.remove();
                                                }
                                            }
                                        }
                                    }
                                }
                                void RemoveForms()
                                {
                                    if (this.windowCategoryBD.windowInfoBrute.FindIndex(windowT => windowT.Handle == window.windowInfo.Handle) == -1)
                                    {
                                        this.windowCategoryBD.windowTreeNodeAdvanced = this.windowCategoryBD.windowTreeNodeAdvanced.Where(TreeViewAdvancedT => TreeViewAdvancedT != treeNodeAdvanced).ToList();
                                        removeTreeNodeAdvanced(treeNodeAdvanced);
                                    }
                                    else
                                    {
                                        if (window.hidedByUser == false && !windowsManager.IsWindowVisible(window.windowInfo.Handle))
                                        {
                                            this.windowCategoryBD.windowTreeNodeAdvanced = this.windowCategoryBD.windowTreeNodeAdvanced.Where(TreeViewAdvancedT => TreeViewAdvancedT != treeNodeAdvanced).ToList();
                                            this.windowCategoryBD.moveWindows(windowCategoryBD.windowActions.notIncluded, (WindowInfoMore)treeNodeAdvanced.userObject);
                                            removeTreeNodeAdvanced(treeNodeAdvanced);
                                        }
                                    }
                                    void removeTreeNodeAdvanced(TreeNodeAdvanced treeNodeAdvanced1)
                                    {
                                        if (treeNodeAdvanced1.parent.name == ((int)defaultParameters.namesTreeNode.packageForm).ToString() && treeNodeAdvanced1.parent.childrens.Count == 1)
                                        {
                                            treeNodeAdvanced1.parent.remove();
                                        }
                                        treeNodeAdvanced1.remove();
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                #region globalHotkey
                loadGlobalHotkey();
                void loadGlobalHotkey()
                {
                    globalValues.hotkeyManager.allHotkeys = new Hotkey(this.Handle);
                    globalValues.LiteDBManager.namesCollections.globalHotkeys.ToString();
                    bool existsSave = globalValues.liteDBManager.LiteDatabase.CollectionExists(globalValues.LiteDBManager.namesCollections.globalHotkeys.ToString());
                    List<Hotkey.HotkeyO> hotkeys = new List<Hotkey.HotkeyO>();
                    if (existsSave)
                    {
                        hotkeys = JsonConvert.DeserializeObject<List<Hotkey.HotkeyO>>(globalValues.liteDBManager.getData(globalValues.LiteDBManager.namesCollections.globalHotkeys.ToString()));
                    }
                    Hotkey.HotkeyO nextSpaceSaved = existsSave ? hotkeys.Find(hotkey => hotkey.name == globalValues.hotkeyManager.defaultHotkeys.nexSpace.name) : null;
                    Hotkey.HotkeyO previousSpaceSaved = existsSave ? hotkeys.Find(hotkey => hotkey.name == globalValues.hotkeyManager.defaultHotkeys.previousSpace.name) : null;
                    Hotkey.HotkeyO showHideNodindowSaved = existsSave ? hotkeys.Find(hotkey => hotkey.name == globalValues.hotkeyManager.defaultHotkeys.showHideNodindow.name) : null;
                    Hotkey.HotkeyO previousSpacePerScreenSaved = existsSave ? hotkeys.Find(hotkey => hotkey.name == globalValues.hotkeyManager.defaultHotkeys.previousSpacePerScreen.name) : null;
                    Hotkey.HotkeyO nextSpacePerScreenSaved = existsSave ? hotkeys.Find(hotkey => hotkey.name == globalValues.hotkeyManager.defaultHotkeys.nextSpacePerScreen.name) : null;

                    globalValues.hotkeyManager.allHotkeys.Register(new Hotkey.HotkeyO(nextSpaceSaved == null ? globalValues.hotkeyManager.defaultHotkeys.nexSpace : nextSpaceSaved, (object sender1, EventArgs e1) =>
                    {
                        //nextSpace
                        ((ShowHideSpaces)locateSpaceParent(this.selectedTreeNodeSpace, 1).userObject).Show();
                    }));
                    globalValues.hotkeyManager.allHotkeys.Register(new Hotkey.HotkeyO(previousSpaceSaved == null ? globalValues.hotkeyManager.defaultHotkeys.previousSpace : previousSpaceSaved, (object sender1, EventArgs e1) =>
                    {
                        //previosSpace
                        ((ShowHideSpaces)locateSpaceParent(this.selectedTreeNodeSpace, -1).userObject).Show();
                    }));
                    globalValues.hotkeyManager.allHotkeys.Register(new Hotkey.HotkeyO(showHideNodindowSaved == null ? globalValues.hotkeyManager.defaultHotkeys.showHideNodindow : showHideNodindowSaved, (object sender1, EventArgs e1) =>
                    {
                        //exibe o formulario principal
                        Screen primaryScreen = Screen.PrimaryScreen;
                        if (this.Visible == false)
                        {
                            this.Show();
                            this.Location = new Point((primaryScreen.WorkingArea.Width - this.Width) / 2, (primaryScreen.WorkingArea.Height - this.Height) / 2);
                        }
                        else
                        {
                            this.Hide();
                        }
                    }));
                    globalValues.hotkeyManager.allHotkeys.Register(new Hotkey.HotkeyO(previousSpacePerScreenSaved == null ? globalValues.hotkeyManager.defaultHotkeys.previousSpacePerScreen : previousSpacePerScreenSaved, (object sender1, EventArgs e1) =>
                    {
                        if (this.selectedTreeNodeSpace.name == ((int)namesTreeNode.monitors).ToString())
                        {
                            TreeNodeAdvanced spaceMonitor = locateSpacePerScreenParent(this.selectedTreeNodeSpace, 1);
                            ((SpaceMonitor)spaceMonitor.userObject).Show();
                        }
                        //proxima área de cima no monitor onde está o cursor do mouse
                    }));
                    globalValues.hotkeyManager.allHotkeys.Register(new Hotkey.HotkeyO(nextSpacePerScreenSaved == null ? globalValues.hotkeyManager.defaultHotkeys.nextSpacePerScreen : nextSpacePerScreenSaved, (object sender1, EventArgs e1) =>
                    {
                        if (this.selectedTreeNodeSpace.name == ((int)namesTreeNode.monitors).ToString())
                        {
                            TreeNodeAdvanced spaceMonitor = locateSpacePerScreenParent(this.selectedTreeNodeSpace, -1);
                            ((SpaceMonitor)spaceMonitor.userObject).Show();
                        }
                        //proxima área de baixo no monitor onde está o cursor do mouse
                    }));
                    TreeNodeAdvanced locateSpaceParent(TreeNodeAdvanced treeNodeAdvanced, int operatorlogic = 1)
                    {
                        int indexStart = treeNodeAdvanced.parent.childrens.FindIndex(treeNodeAdvancedLocated => treeNodeAdvancedLocated == selectedTreeNodeSpace) + operatorlogic;
                        for (int i = indexStart; i < treeNodeAdvanced.parent.childrens.Count; i += operatorlogic)
                        {
                            if (i >= 0)
                            {
                                if ((treeNodeAdvanced.parent.childrens[i].name == ((int)namesTreeNode.space).ToString()) || (treeNodeAdvanced.parent.childrens[i].name == ((int)namesTreeNode.monitors).ToString()))
                                {
                                    return treeNodeAdvanced.parent.childrens[i];
                                }
                            }
                            else
                            {
                                return treeNodeAdvanced;
                            }
                        }
                        return treeNodeAdvanced;
                    }
                    TreeNodeAdvanced locateSpacePerScreenParent(TreeNodeAdvanced treeNodeAdvanced, int operatorlogic = 1)
                    {
                        TreeNodeAdvanced monitor = treeNodeAdvanced.childrens.Find(Monitor1 => ((MonitorInfo)Monitor1.userObject).nameSys == GetMonitorWithCursor().DeviceName);
                        int indexStart = monitor.childrens.FindIndex(treeNodeAdvancedLocated => treeNodeAdvancedLocated == ((MonitorInfo)monitor.userObject).selectedTreeNodeSpaceInMonitor) + operatorlogic;

                        for (int i = indexStart; i < monitor.childrens.Count; i += operatorlogic)
                        {
                            if (i >= 0)
                            {
                                if (monitor.childrens[i].name == ((int)namesTreeNode.spaceMonitor).ToString())
                                {
                                    return monitor.childrens[i];
                                }
                            }
                            else
                            {
                                return monitor.childrens.Find(treeNodeAdvancedLocated => treeNodeAdvancedLocated == ((MonitorInfo)monitor.userObject).selectedTreeNodeSpaceInMonitor);
                            }
                        }
                        return ((MonitorInfo)monitor.userObject).selectedTreeNodeSpaceInMonitor;
                        Screen GetMonitorWithCursor()
                        {
                            // Pega a posição atual do cursor do mouse
                            Point cursorPosition = Cursor.Position;

                            // Obtém o monitor que contém o ponto do cursor
                            Screen screenWithCursor = Screen.FromPoint(cursorPosition);

                            return screenWithCursor;
                        }
                    }
                }
                updateLanguage();
                #endregion
            }
        }
        public void updateLanguage()
        {
            toolStripDropDownButton1.Text = globalValues.translationsManager.mainFormTranslation.toolStrip.display;
            configurationsToolStripMenuItem.Text = globalValues.translationsManager.mainFormTranslation.toolStrip.configurations;

            foreach (TreeNodeAdvanced treeNodeAdvanced in treeViewAdvanced1.TreeNodeAdvancedBD)
            {
                if (treeNodeAdvanced.locked == true)
                {
                    if (treeNodeAdvanced.name == defaultParameters.Settings.name)
                    {
                        treeNodeAdvanced.text = globalValues.translationsManager.mainFormTranslation.treeNodeAdvanced.settings;
                        foreach (TreeNodeAdvanced child in treeNodeAdvanced.childrens)
                        {
                            if (child.name == defaultParameters._Monitors.name)
                            {
                                child.text = globalValues.translationsManager.mainFormTranslation.treeNodeAdvanced.monitors;
                            }
                        }
                    }
                    if (treeNodeAdvanced.name == defaultParameters.WorkspaceHub.name)
                    {
                        treeNodeAdvanced.text = globalValues.translationsManager.mainFormTranslation.treeNodeAdvanced.sphereOfWork;
                        foreach (TreeNodeAdvanced mainWorkspace in treeNodeAdvanced.childrens)
                        {
                            if (mainWorkspace.locked == true && mainWorkspace.name == defaultParameters.MainWorkspace.name)
                            {
                                mainWorkspace.text = globalValues.translationsManager.mainFormTranslation.treeNodeAdvanced.mainWorkspace;
                                foreach (TreeNodeAdvanced mainSpace in mainWorkspace.childrens)
                                {
                                    if (mainSpace.locked == true && mainSpace.name == defaultParameters.MainSpace.name)
                                    {
                                        mainSpace.text = globalValues.translationsManager.mainFormTranslation.treeNodeAdvanced.mainSpace;
                                    }
                                }
                            }
                        }
                    }
                    if (treeNodeAdvanced.name == defaultParameters.ignoredForms.name)
                    {
                        treeNodeAdvanced.text = globalValues.translationsManager.mainFormTranslation.treeNodeAdvanced.ignoredForms;
                    }
                }
            }
        }
        private void treeViewAdvanced1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if ((e.Node.IsExpanded && e.Location.X < e.Node.Bounds.Left - 19) || (!e.Node.IsExpanded && e.Location.X < e.Node.Bounds.Left - 19))
            {
                // O nó está sendo colapsado ou expandido, então não execute a ação
                return;
            }
            switch (e.Button)
            {
                case MouseButtons.Right:
                    treeViewAdvanced1.SelectedNode = e.Node;
                    if (treeViewAdvanced1.SelectedNode != null)
                    {
                        ContextMenuStrip contextMenuStrip = createContextMenuStrip(treeViewAdvanced1.SelectedNode);
                        if (contextMenuStrip != null)
                        {
                            contextMenuStrip.Show(Cursor.Position);
                        }
                    }
                    break;
                case MouseButtons.Left:
                    if (treeViewAdvanced1.SelectedNode != null)
                    {
                        TreeNodeAdvanced treeNode = ((TreeNodeAdvanced)e.Node.Tag);
                        if (treeNode.userObject != null && treeNode.userObject.GetType().IsSubclassOf(typeof(ShowHideSpaces)))
                        {
                            ((ShowHideSpaces)treeNode.userObject).Show();
                        }
                        if (((TreeNodeAdvanced)e.Node.Tag).name == ((int)namesTreeNode.ConfigMonitor).ToString() || ((TreeNodeAdvanced)e.Node.Tag).name == ((int)namesTreeNode.monitor).ToString())
                        {
                            createBoundsMonitor();
                        }
                        void createBoundsMonitor()
                        {
                            Screen monitor = getMonitorByName(((MonitorConfig)((TreeNodeAdvanced)e.Node.Tag).userObject).nameSys);
                            globalValues.showBoundsView(new Size(monitor.Bounds.Width, monitor.Bounds.Height), new Point(monitor.Bounds.X, monitor.Bounds.Y), Color.Green);
                        }
                    }
                    break;
            }
            // Ação a ser executada ao clicar no nó (fora do ícone de expandir/colapsar)

            //Otimize: é possivel delevar a parte da exibição para o objeto do treeNodeAdvancedArea a fazer
        }
        public Screen getMonitorByName(string nameSys)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.DeviceName.Equals(nameSys))
                {
                    return screen;
                }
            }
            return null;
        }
        public ContextMenuStrip createContextMenuStrip(TreeNode treeNode)
        {
            TreeNodeAdvanced treeNodeAdvanced = (TreeNodeAdvanced)treeNode.Tag;
            contextMenuStripAdvanced contextMenuStrip = new contextMenuStripAdvanced();
            if ((treeNode.Name == ((int)namesTreeNode.workspace).ToString()) || (treeNode.Name == ((int)namesTreeNode.workspaceHub).ToString()))
            {
                contextMenuStripAdvanced.ToolStripMenuItemAdvanced newStripMenuAddWorkspace = new contextMenuStripAdvanced.ToolStripMenuItemAdvanced(globalValues.translationsManager.mainFormTranslation.contextMenu.newNode, null, Color.White);
                newStripMenuAddWorkspace.addItem(globalValues.translationsManager.mainFormTranslation.contextMenu.workspace, Image.FromFile(globalValues.filePaths.workspace), Color.White, (object sender, EventArgs e) =>
                {
                    tools.Label1Textbox1Button2 label1Textbox1Button2 = new tools.Label1Textbox1Button2(globalValues.translationsManager.mainFormTranslation.contextMenu.tools.setNameWorkspace, "", globalValues.translationsManager.mainFormTranslation.contextMenu.tools.cancel, globalValues.translationsManager.mainFormTranslation.contextMenu.tools.ok);
                    tools tools = new tools(myPackages.formTools.tools.toolsType.Label1Textbox1Button2, label1Textbox1Button2);
                    tools.StartPosition = FormStartPosition.Manual;
                    tools.Location = Cursor.Position;
                    tools.ShowDialog();
                    if (label1Textbox1Button2.Ready)
                    {
                        label1Textbox1Button2.TextBox.Text.Split(',').ToList().ForEach(text =>
                        {
                            treeNodeAdvanced.add(new TreeNodeAdvanced(text, null, this.defaultParameters.Workspace));
                        });
                    }
                });
                newStripMenuAddWorkspace.addItem(globalValues.translationsManager.mainFormTranslation.contextMenu.Space, Image.FromFile(globalValues.filePaths.space), Color.White, (object sender, EventArgs e) =>
                {
                    tools.Label1Textbox1Button2 label1Textbox1Button2 = new tools.Label1Textbox1Button2(globalValues.translationsManager.mainFormTranslation.contextMenu.tools.setNameSpace, "", globalValues.translationsManager.mainFormTranslation.contextMenu.tools.cancel, globalValues.translationsManager.mainFormTranslation.contextMenu.tools.ok);
                    tools tools = new tools(myPackages.formTools.tools.toolsType.Label1Textbox1Button2, label1Textbox1Button2);
                    tools.StartPosition = FormStartPosition.Manual;
                    tools.Location = Cursor.Position;
                    tools.ShowDialog();
                    if (label1Textbox1Button2.Ready)
                    {
                        label1Textbox1Button2.TextBox.Text.Split(',').ToList().ForEach(text =>
                        {
                            TreeNodeAdvanced space = treeNodeAdvanced.add(new TreeNodeAdvanced(text, null, this.defaultParameters.Space));
                            space.userObject = new Space(space, this);
                        });
                    }
                });
                newStripMenuAddWorkspace.addItem(globalValues.translationsManager.mainFormTranslation.contextMenu.spacePerScreen, Image.FromFile(globalValues.filePaths.monitors), Color.White, (object sender, EventArgs e) =>
                {
                    tools.Label1Textbox1Button2 label1Textbox1Button2 = new tools.Label1Textbox1Button2(globalValues.translationsManager.mainFormTranslation.contextMenu.tools.setNameScreenPerSpace, "", globalValues.translationsManager.mainFormTranslation.contextMenu.tools.cancel, globalValues.translationsManager.mainFormTranslation.contextMenu.tools.ok);
                    tools tools = new tools(myPackages.formTools.tools.toolsType.Label1Textbox1Button2, label1Textbox1Button2);
                    tools.StartPosition = FormStartPosition.Manual;
                    tools.Location = Cursor.Position;
                    tools.ShowDialog();
                    if (label1Textbox1Button2.Ready)
                    {
                        label1Textbox1Button2.TextBox.Text.Split(',').ToList().ForEach(text =>
                        {
                            TreeNodeAdvanced treeNodeMonitors = treeNodeAdvanced.add(new TreeNodeAdvanced(text, null, this.defaultParameters.Monitors));
                            treeNodeMonitors.userObject = new Monitors(treeNodeMonitors, this);
                            foreach (MonitorConfig monitorConfig in monitors)
                            {
                                TreeNodeAdvanced treeNodeAdvancedMonitor = treeNodeMonitors.add(new TreeNodeAdvanced(monitorConfig.newName, new MonitorInfo(monitorConfig.nameSys, monitorConfig.newName, monitorConfig.splitMonitor), this.defaultParameters.Monitor));
                                TreeNodeAdvanced spaceMonitor = new TreeNodeAdvanced(globalValues.translationsManager.mainFormTranslation.treeNodeAdvanced.mainSpace, null, this.defaultParameters.mainSpaceMonitor);
                                treeNodeAdvancedMonitor.add(spaceMonitor);
                                treeNodeAdvancedMonitor.childrens[treeNodeAdvancedMonitor.childrens.Count - 1].userObject = new SpaceMonitor(treeNodeAdvancedMonitor.childrens[treeNodeAdvancedMonitor.childrens.Count - 1], this);
                                ((MonitorInfo)treeNodeAdvancedMonitor.userObject).selectedTreeNodeSpaceInMonitor = treeNodeAdvancedMonitor.childrens[treeNodeAdvancedMonitor.childrens.Count - 1];
                                //da para otimizar a criação dos spaces etc para chamar apenas uma função para criar um novo dessa forma economizando espaço de codigo
                            }
                        });
                    }
                });
                contextMenuStrip.Items.Add(newStripMenuAddWorkspace);
                if (treeNodeAdvanced.locked == false)
                {
                    contextMenuStrip.addItem(globalValues.translationsManager.mainFormTranslation.contextMenu.delete, null, Color.White, (object sender, EventArgs EventArgs) =>
                    {
                        if (DialogResult.Yes == MessageBox.Show(globalValues.translationsManager.mainFormTranslation.contextMenu.messageDialog.delete.Replace("(1)", treeNode.Text), globalValues.translationsManager.mainFormTranslation.contextMenu.messageDialog.deleteCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                        {
                            constructPackageForm(treeNodeAdvanced, treeNodeAdvanced.parent);
                            treeNodeAdvanced.remove();
                        }
                    });
                    contextMenuStrip.addItem(globalValues.translationsManager.mainFormTranslation.contextMenu.rename, null, Color.White, rename);
                }
            }
            else if (treeNode.Name == ((int)namesTreeNode.form).ToString())
            {
                contextMenuStrip.addItem(globalValues.translationsManager.mainFormTranslation.contextMenu.show, null, Color.White, showWindow);
                contextMenuStrip.addItem(globalValues.translationsManager.mainFormTranslation.contextMenu.hide, null, Color.White, hideWindow);
                contextMenuStrip.addItem(globalValues.translationsManager.mainFormTranslation.contextMenu.rename, null, Color.White, rename);
                void showWindow(object sender, EventArgs e)
                {
                    showHideWindow((WindowInfoMore)treeNodeAdvanced.userObject, false);
                }
                void hideWindow(object sender, EventArgs e)
                {
                    showHideWindow((WindowInfoMore)treeNodeAdvanced.userObject, true);
                }
            }
            else if (treeNode.Name == ((int)namesTreeNode.window).ToString())
            {
                /* o usuario não vai utilizar mas pretendo deixar se caso eu for utilizar no futuro
                    contextMenuStrip.addItem("Show", null, Color.White, showWindow);
                    contextMenuStrip.addItem("Hide", null, Color.White, hideWindow);
                    contextMenuStrip.addItem("Rename", null, Color.White, rename);
                    */
            }
            else if (treeNode.Name == ((int)namesTreeNode.space).ToString())
            {
                if (treeNodeAdvanced.locked == false)
                {
                    contextMenuStrip.addItem(globalValues.translationsManager.mainFormTranslation.contextMenu.delete, null, Color.White, (object sender, EventArgs e) =>
                    {
                        if (DialogResult.Yes == MessageBox.Show(globalValues.translationsManager.mainFormTranslation.contextMenu.messageDialog.delete.Replace("(1)", treeNode.Text), globalValues.translationsManager.mainFormTranslation.contextMenu.messageDialog.deleteCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                        {
                            this.selectedTreeNodeSpace = this.mainTreeNodeSpace;
                            constructPackageForm(treeNodeAdvanced, treeNodeAdvanced.parent);
                            treeNodeAdvanced.remove();
                        }
                    });
                    contextMenuStrip.addItem(globalValues.translationsManager.mainFormTranslation.contextMenu.rename, null, Color.White, rename);
                }
            }
            else if (treeNode.Name == ((int)namesTreeNode.spaceMonitor).ToString())
            {
                if (treeNodeAdvanced.locked == false)
                {
                    contextMenuStrip.addItem(globalValues.translationsManager.mainFormTranslation.contextMenu.delete, null, Color.White, (object sender, EventArgs e) =>
                    {
                        if (DialogResult.Yes == MessageBox.Show(globalValues.translationsManager.mainFormTranslation.contextMenu.messageDialog.delete.Replace("(1)", treeNode.Text), globalValues.translationsManager.mainFormTranslation.contextMenu.messageDialog.deleteCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                        {
                            foreach (TreeNodeAdvanced treeNodeAdvanced1 in treeNodeAdvanced.childrens)
                            {
                                showHideWindow((WindowInfoMore)treeNodeAdvanced1.userObject, true);
                            }
                            constructPackageForm(treeNodeAdvanced, treeNodeAdvanced.parent);
                            ((MonitorInfo)treeNodeAdvanced.parent.userObject).selectedTreeNodeSpaceInMonitor = treeNodeAdvanced.parent.childrens[treeNodeAdvanced.parent.childrens.FindIndex(treeNodeAdvancedT => treeNodeAdvancedT.text == treeNodeAdvanced.text && treeNodeAdvancedT.name == treeNodeAdvanced.name) - 1];
                            treeNodeAdvanced.remove();
                        }
                    });
                    contextMenuStrip.addItem(globalValues.translationsManager.mainFormTranslation.contextMenu.rename, null, Color.White, rename);
                }
            }
            else if (treeNode.Name == ((int)namesTreeNode.monitor).ToString())
            {
                contextMenuStripAdvanced.ToolStripMenuItemAdvanced newStripMenuAddMonitor = new contextMenuStripAdvanced.ToolStripMenuItemAdvanced(globalValues.translationsManager.mainFormTranslation.contextMenu.newNode, null, Color.White);
                newStripMenuAddMonitor.addItem(globalValues.translationsManager.mainFormTranslation.contextMenu.spacePerScreen, Image.FromFile(globalValues.filePaths.space), Color.White, (object sender, EventArgs e) =>
                {
                    tools.Label1Textbox1Button2 label1Textbox1Button2 = new tools.Label1Textbox1Button2(globalValues.translationsManager.mainFormTranslation.contextMenu.tools.setNameSpace, "", globalValues.translationsManager.mainFormTranslation.contextMenu.tools.cancel, globalValues.translationsManager.mainFormTranslation.contextMenu.tools.ok);
                    tools tools = new tools(myPackages.formTools.tools.toolsType.Label1Textbox1Button2, label1Textbox1Button2);
                    tools.StartPosition = FormStartPosition.Manual;
                    tools.Location = Cursor.Position;
                    tools.ShowDialog();
                    if (label1Textbox1Button2.Ready)
                    {
                        label1Textbox1Button2.TextBox.Text.Split(',').ToList().ForEach(text =>
                        {
                            treeNodeAdvanced.add(new TreeNodeAdvanced(text, null, this.defaultParameters.SpaceMonitor));
                            treeNodeAdvanced.childrens[treeNodeAdvanced.childrens.Count - 1].userObject = new SpaceMonitor(treeNodeAdvanced.childrens[treeNodeAdvanced.childrens.Count - 1], this);
                        });
                    }
                });
                contextMenuStrip.Items.Add(newStripMenuAddMonitor);
            }
            else if (treeNode.Name == ((int)namesTreeNode.ConfigMonitor).ToString())
            {
                contextMenuStrip.addItem(globalValues.translationsManager.mainFormTranslation.contextMenu.rename, null, Color.White, (object sender, EventArgs e) =>
                {
                    tools.Label1Textbox1Button2 label1Textbox1Button2 = new tools.Label1Textbox1Button2(globalValues.translationsManager.mainFormTranslation.contextMenu.tools.newName, treeViewAdvanced1.SelectedNode.Text, globalValues.translationsManager.mainFormTranslation.contextMenu.tools.cancel, globalValues.translationsManager.mainFormTranslation.contextMenu.tools.ok);
                    tools tools = new tools(tools.toolsType.Label1Textbox1Button2, label1Textbox1Button2);
                    tools.StartPosition = FormStartPosition.Manual;
                    tools.Location = Cursor.Position;
                    tools.ShowDialog();
                    if (label1Textbox1Button2.Ready)
                    {
                        treeNodeAdvanced.text = label1Textbox1Button2.TextBox.Text;
                        ((MonitorConfig)treeNodeAdvanced.userObject).newName = label1Textbox1Button2.TextBox.Text;
                    }
                });
            }
            else if (treeNode.Name == ((int)namesTreeNode.monitors).ToString())
            {
                if (treeNodeAdvanced.locked == false)
                {
                    contextMenuStrip.addItem(globalValues.translationsManager.mainFormTranslation.contextMenu.delete, null, Color.White, (object sender, EventArgs e) =>
                    {
                        if (DialogResult.Yes == MessageBox.Show(globalValues.translationsManager.mainFormTranslation.contextMenu.messageDialog.delete.Replace("(1)", treeNode.Text), globalValues.translationsManager.mainFormTranslation.contextMenu.messageDialog.deleteCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                        {
                            this.selectedTreeNodeSpace = this.mainTreeNodeSpace;
                            constructPackageForm(treeNodeAdvanced, treeNodeAdvanced.parent);
                            treeNodeAdvanced.remove();
                        }
                    });
                    contextMenuStrip.addItem(globalValues.translationsManager.mainFormTranslation.contextMenu.rename, null, Color.White, rename);
                }
            }
            return contextMenuStrip;
            void rename(object sender, EventArgs e)
            {
                tools.Label1Textbox1Button2 label1Textbox1Button2 = new tools.Label1Textbox1Button2(globalValues.translationsManager.mainFormTranslation.contextMenu.tools.newName, treeViewAdvanced1.SelectedNode.Text, globalValues.translationsManager.mainFormTranslation.contextMenu.tools.cancel, globalValues.translationsManager.mainFormTranslation.contextMenu.tools.ok);
                tools tools = new tools(tools.toolsType.Label1Textbox1Button2, label1Textbox1Button2);
                tools.StartPosition = FormStartPosition.Manual;
                tools.Location = Cursor.Position;
                tools.ShowDialog();
                if (label1Textbox1Button2.Ready)
                {
                    treeNodeAdvanced.text = label1Textbox1Button2.TextBox.Text;
                }
            }
        }
        void loadWindowsNodes(TreeNodeAdvanced treeNodeAdvancedParent, windowCategoryBD.WindowInfoMore windowInfoMore)
        {
            if (treeNodeAdvancedParent.name == ((int)namesTreeNode.space).ToString())
            {
                addWindowsInfoSpace();
                void addWindowsInfoSpace()
                {
                    TreeNodeAdvanced treeNodeAdvancedT = treeNodeAdvancedParent.add(new TreeNodeAdvanced($"{windowInfoMore.windowInfo.Title} Id:{windowInfoMore.windowInfo.Handle}", windowInfoMore, this.defaultParameters.Form));
                    foreach (WindowInfo windowInfoT in windowInfoMore.windowInfo.ChildWindows)
                    {
                        treeNodeAdvancedT.add(new TreeNodeAdvanced($"{windowInfoT.Title} Id:{windowInfoT.Handle}", new windowCategoryBD.WindowInfoMore(windowInfoT), this.defaultParameters.Window));
                    }
                    windowCategoryBD.addWindowTreeNodeAdvanced(treeNodeAdvancedT);
                }
            }
            else if (treeNodeAdvancedParent.name == ((int)namesTreeNode.monitors).ToString())
            {
                addWindowsInfoSpaceMonitor();
                void addWindowsInfoSpaceMonitor()
                {
                    TreeNodeAdvanced monitorWithLargestArea = GetMonitorWithLargestArea(windowInfoMore.windowInfo, treeNodeAdvancedParent);
                    if (monitorWithLargestArea != null)
                    {
                        TreeNodeAdvanced treeNodeMonitor = ((MonitorInfo)monitorWithLargestArea.userObject).selectedTreeNodeSpaceInMonitor;
                        if (treeNodeMonitor.treeNode == null)
                        {
                            ((MonitorInfo)monitorWithLargestArea.userObject).selectedTreeNodeSpaceInMonitor = monitorWithLargestArea.childrens[0];
                            treeNodeMonitor = ((MonitorInfo)monitorWithLargestArea.userObject).selectedTreeNodeSpaceInMonitor;
                        }
                        TreeNodeAdvanced treeNodeAdvancedT = treeNodeMonitor.add(new TreeNodeAdvanced($"{windowInfoMore.windowInfo.Title} Id:{windowInfoMore.windowInfo.Handle}", windowInfoMore, this.defaultParameters.Form));
                        foreach (WindowInfo windowInfoT in windowInfoMore.windowInfo.ChildWindows)
                        {
                            treeNodeAdvancedT.add(new TreeNodeAdvanced($"{windowInfoT.Title} Id:{windowInfoT.Handle}", new windowCategoryBD.WindowInfoMore(windowInfoT), this.defaultParameters.Window));
                        }
                        windowCategoryBD.addWindowTreeNodeAdvanced(treeNodeAdvancedT);
                    }
                }
            }
            else if (treeNodeAdvancedParent.name == ((int)namesTreeNode.ignoredForms).ToString())
            {
                addWindowsInfoExcludedForm();
                void addWindowsInfoExcludedForm()
                {
                    TreeNodeAdvanced form = treeNodeAdvancedParent.add(new TreeNodeAdvanced($"{windowInfoMore.windowInfo.Title} Id:{windowInfoMore.windowInfo.Handle}", windowInfoMore, this.defaultParameters.Form));
                    foreach (WindowInfo windowInfoT in windowInfoMore.windowInfo.ChildWindows)
                    {
                        form.add(new TreeNodeAdvanced($"{windowInfoT.Title} Id:{windowInfoT.Handle}", new windowCategoryBD.WindowInfoMore(windowInfoT), this.defaultParameters.Window));
                    }
                    windowCategoryBD.addWindowTreeNodeAdvanced(form);
                }
            }
        }
        public void showHideWindow(WindowInfoMore windowInfoMore, bool hide)
        {
            windowCategoryBD.addModifiedWindow(windowInfoMore);
            if (windowInfoMore.windowInfo.isWindow())
            {
                if (hide)
                {
                    updateSelectedTreeNodeSpace.Stop();
                    //aqui o timer está sendo pausado para não ocorrer dele excluir um processo que foi ocultado pelo usuario oque estava ocorrendo antes
                    windowInfoMore.hidedByUser = true;
                    updateSelectedTreeNodeSpace.Start();
                    windowInfoMore.windowInfo.hideWindow();
                }
                else
                {
                    updateSelectedTreeNodeSpace.Stop();
                    windowInfoMore.hidedByUser = false;
                    Thread.Sleep(10);
                    updateSelectedTreeNodeSpace.Start();
                    windowInfoMore.windowInfo.showWindow();
                }
            }
        }
        private TreeNodeAdvanced constructPackageForm(TreeNodeAdvanced treeNodeAdvanced, TreeNodeAdvanced parent)
        {
            List<TreeNodeAdvanced> allForms = getFormsFromTreeNodeAdvanced(treeNodeAdvanced);
            if (allForms.Count > 0)
            {
                TreeNodeAdvanced packageForm = parent.add(new TreeNodeAdvanced(getNameFree(), null, this.defaultParameters.PackageForm));
                foreach (TreeNodeAdvanced form in allForms)
                {
                    windowCategoryBD.windowTreeNodeAdvanced[windowCategoryBD.windowTreeNodeAdvanced.FindIndex(formNode => ((WindowInfoMore)formNode.userObject).windowInfo.Handle == ((WindowInfoMore)form.userObject).windowInfo.Handle)] = packageForm.add(form);
                }
                return packageForm;
            }
            else
            {
                return null;
            }

            List<TreeNodeAdvanced> getFormsFromTreeNodeAdvanced(TreeNodeAdvanced child, List<TreeNodeAdvanced> forms = null)
            {
                if (forms == null)
                {
                    forms = new List<TreeNodeAdvanced>();
                }
                if (child.name == ((int)namesTreeNode.workspace).ToString())
                {
                    foreach (TreeNodeAdvanced treeNodeAdvanced2 in child.childrens)
                    {
                        forms.AddRange(getFormsFromTreeNodeAdvanced(treeNodeAdvanced2, forms));
                    }
                }
                else if (child.name == ((int)namesTreeNode.space).ToString())
                {
                    foreach (TreeNodeAdvanced form in child.childrens)
                    {
                        forms.Add(form);
                    }
                }
                else if (child.name == ((int)namesTreeNode.spaceMonitor).ToString())
                {
                    foreach (TreeNodeAdvanced form in child.childrens)
                    {
                        forms.Add(form);
                    }
                }
                else if (child.name == ((int)namesTreeNode.monitor).ToString())
                {
                    foreach (TreeNodeAdvanced spaceMonitor in child.childrens)
                    {
                        forms.AddRange(getFormsFromTreeNodeAdvanced(spaceMonitor, forms));
                    }
                }
                else if (child.name == ((int)namesTreeNode.monitors).ToString())
                {
                    foreach (TreeNodeAdvanced monitor in child.childrens)
                    {
                        forms.AddRange(getFormsFromTreeNodeAdvanced(monitor, forms));
                    }
                }
                return forms;
            }
            string getNameFree()
            {
                string text = treeNodeAdvanced.text;
                for (int i = 0; treeNodeAdvanced.parent.childrens.FindIndex(treeNode => treeNode.text == text && treeNode.name == ((int)namesTreeNode.packageForm).ToString()) != -1; i++)
                {
                    text = $"{treeNodeAdvanced.text}({i})";
                }
                return text;
            }
        }
        /// <summary>
        /// Retorna o treeNodeAdvancedMonitor com maior area sendo usado no windowInfo
        /// </summary>
        /// <param name="windowInfo">Informações da janela</param>
        /// <param name="Monitors">TreeNodeAdvanced do monitors</param>
        /// <returns></returns>
        private TreeNodeAdvanced GetMonitorWithLargestArea(WindowInfo windowInfo, TreeNodeAdvanced Monitors)
        {
            Rectangle windowRect = new Rectangle(windowInfo.Position.X, windowInfo.Position.Y, windowInfo.Size.Width, windowInfo.Size.Height);
            string bestMonitor = null;
            int maxArea = 0;

            foreach (var screen in Screen.AllScreens)
            {
                Rectangle intersection = Rectangle.Intersect(windowRect, screen.Bounds);

                int area = intersection.Width * intersection.Height;
                if (area > maxArea)
                {
                    maxArea = area;
                    bestMonitor = screen.DeviceName;
                }
            }
            return Monitors.childrens.Find(monitor => ((MonitorInfo)monitor.userObject).nameSys == bestMonitor);
        }
        protected override void WndProc(ref Message m)
        {
            if (globalValues.hotkeyManager.allHotkeys != null)
            {
                globalValues.hotkeyManager.allHotkeys.ProcessMessage(m);
            }
            base.WndProc(ref m);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            globalValues.liteDBManager.updateData(globalValues.LiteDBManager.namesCollections.mainTreeViewAdvanced.ToString(), treeViewAdvanced1.getJsonSave());
            globalValues.liteDBManager.updateData(globalValues.LiteDBManager.namesCollections.currentLanguage.ToString(), globalValues.language);
            globalValues.liteDBManager.LiteDatabase.Dispose();
            foreach (WindowInfoMore windowInfoMore in windowCategoryBD.modifiedWindow)
            {
                windowInfoMore.windowInfo.InitialValues.setInitialValues();
            }
            windowCategoryBD.StopUpdating();
        }

        private void treeViewAdvanced1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void treeViewAdvanced1_AfterDragDropTreeViewAdvanced(object sender, DragDropTreeViewAdvancedEventArgs e)
        {
            TraceFlow.references.addRef(e.draggedNode);
            if (e.draggedNode.name == ((int)namesTreeNode.form).ToString())
            {
                constructForm(e.draggedNode);
            }
            if (e.draggedNode.name == ((int)namesTreeNode.packageForm).ToString())
            {
                foreach (TreeNodeAdvanced form in e.draggedNode.childrens)
                {
                    constructForm(e.targetNode.add(form));
                }
                e.draggedNode.remove();
            }
            if (e.draggedNode.name == ((int)namesTreeNode.monitors).ToString())
            {
                constructMonitors(e.draggedNode);
            }
            if (e.draggedNode.name == ((int)namesTreeNode.space).ToString())
            {
                constructSpace(e.draggedNode);
            }
            void constructForm(TreeNodeAdvanced formDragged)
            {
                TreeNodeAdvanced form1 = windowCategoryBD.windowTreeNodeAdvanced.Find(formNode => ((WindowInfoMore)formNode.userObject).windowInfo.Handle == ((WindowInfoMore)formDragged.userObject).windowInfo.Handle);
                if (form1 != null)
                {
                    if (formDragged.name == ((int)namesTreeNode.form).ToString() && ((WindowInfoMore)formDragged.userObject).windowInfo.Handle == ((WindowInfoMore)form1.userObject).windowInfo.Handle)
                    {
                        windowCategoryBD.windowTreeNodeAdvanced[windowCategoryBD.windowTreeNodeAdvanced.FindIndex(formNode => ((WindowInfoMore)formNode.userObject).windowInfo.Handle == ((WindowInfoMore)formDragged.userObject).windowInfo.Handle)] = formDragged;
                    }
                }
                if (e.targetNode.name == ((int)namesTreeNode.space).ToString())
                {
                    if (selectedTreeNodeSpace == e.targetNode)
                    {
                        showHideWindow((WindowInfoMore)formDragged.userObject, false);
                    }
                    else
                    {
                        showHideWindow((WindowInfoMore)formDragged.userObject, true);
                    }
                }
                if (e.targetNode.name == ((int)namesTreeNode.spaceMonitor).ToString())
                {
                    if (e.targetNode.parent.parent == selectedTreeNodeSpace && ((MonitorInfo)e.targetNode.parent.userObject).selectedTreeNodeSpaceInMonitor == e.targetNode)
                    {
                        showHideWindow((WindowInfoMore)formDragged.userObject, false);
                    }
                    else
                    {
                        showHideWindow((WindowInfoMore)formDragged.userObject, true);
                    }
                }
                if (e.targetNode.name == ((int)namesTreeNode.monitors).ToString())
                {
                    TreeNodeAdvanced monitorWithLargestArea = GetMonitorWithLargestArea(((WindowInfoMore)formDragged.userObject).windowInfo, e.targetNode);
                    TreeNodeAdvanced selectedSpaceMonitor = ((MonitorInfo)monitorWithLargestArea.userObject).selectedTreeNodeSpaceInMonitor;
                    if (selectedSpaceMonitor.treeNode == null)
                    {
                        ((MonitorInfo)monitorWithLargestArea.userObject).selectedTreeNodeSpaceInMonitor = monitorWithLargestArea.childrens[0];
                        selectedSpaceMonitor = ((MonitorInfo)monitorWithLargestArea.userObject).selectedTreeNodeSpaceInMonitor;
                        //tem que fazer esse processo porque após o carregamento dos nodes salvos o treeNode acaba ficando nulo do selectedTreeNodeSpaceInMonitor
                    }
                    windowCategoryBD.windowTreeNodeAdvanced[windowCategoryBD.windowTreeNodeAdvanced.FindIndex(formNode => ((WindowInfoMore)formNode.userObject).windowInfo.Handle == ((WindowInfoMore)formDragged.userObject).windowInfo.Handle)] = selectedSpaceMonitor.add(formDragged);
                    if (e.targetNode == selectedTreeNodeSpace)
                    {
                        showHideWindow((WindowInfoMore)formDragged.userObject, false);
                    }
                    else
                    {
                        showHideWindow((WindowInfoMore)formDragged.userObject, true);
                    }
                    this.selectedTreeNodeSpace = e.targetNode;
                    this.treeViewAdvanced1.SelectedNode = e.targetNode.treeNode;
                    formDragged.remove();
                }
            }
            void constructSpace(TreeNodeAdvanced space)
            {
                for (int i = 0; i < space.childrens.Count; i++)
                {
                    windowCategoryBD.windowTreeNodeAdvanced[windowCategoryBD.windowTreeNodeAdvanced.FindIndex(form1 => ((WindowInfoMore)form1.userObject).windowInfo.Handle == ((WindowInfoMore)space.childrens[i].userObject).windowInfo.Handle)] = space.childrens[i];
                }
                this.selectedTreeNodeSpace = space;
                this.treeViewAdvanced1.SelectedNode = space.treeNode;
                ((Space)space.userObject).treeNodeAdvanced = space;
            }
            void constructMonitors(TreeNodeAdvanced monitors)
            {
                foreach (TreeNodeAdvanced monitor in ((TreeNodeAdvanced)monitors).childrens)
                {
                    MonitorInfo monitorInfo = (MonitorInfo)monitor.userObject;
                    ((MonitorInfo)monitor.userObject).selectedTreeNodeSpaceInMonitor = monitor.childrens.Find(spacesMonitor => spacesMonitor.text == monitorInfo.selectedTreeNodeSpaceInMonitor.text && spacesMonitor.name == monitorInfo.selectedTreeNodeSpaceInMonitor.name);
                    foreach (TreeNodeAdvanced spaceMonitor in monitor.childrens)
                    {
                        for (int i = 0; i < spaceMonitor.childrens.Count; i++)
                        {
                            windowCategoryBD.windowTreeNodeAdvanced[windowCategoryBD.windowTreeNodeAdvanced.FindIndex(form1 => ((WindowInfoMore)form1.userObject).windowInfo.Handle == ((WindowInfoMore)spaceMonitor.childrens[i].userObject).windowInfo.Handle)] = spaceMonitor.childrens[i];
                        }
                        ((SpaceMonitor)spaceMonitor.userObject).treeNodeAdvanced = spaceMonitor;
                    }
                }
                ((Monitors)monitors.userObject).treeNodeAdvanced = monitors;
                this.selectedTreeNodeSpace = monitors;
                this.treeViewAdvanced1.SelectedNode = monitors.treeNode;
            }
            if (e.draggedNode.name == ((int)namesTreeNode.workspace).ToString())
            {
                constructChildrens(e.draggedNode);
                void constructChildrens(TreeNodeAdvanced treeNodeAdvanced)
                {
                    foreach (TreeNodeAdvanced child in treeNodeAdvanced.childrens)
                    {
                        if (child.name == ((int)namesTreeNode.workspace).ToString())
                        {
                            constructChildrens(child);
                        }
                        else if (child.name == ((int)namesTreeNode.space).ToString())
                        {
                            constructSpace(child);
                        }
                        else if (child.name == ((int)namesTreeNode.monitors).ToString())
                        {
                            constructMonitors(child);
                        }
                    }
                }
            }
        }

        private void treeViewAdvanced1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case (Keys.Apps):
                    ContextMenuStrip contextMenuStrip = createContextMenuStrip(treeViewAdvanced1.SelectedNode);
                    if (contextMenuStrip != null)
                    {
                        contextMenuStrip.Show(treeViewAdvanced1.PointToScreen(new Point(treeViewAdvanced1.SelectedNode.Bounds.X, treeViewAdvanced1.SelectedNode.Bounds.Y)));
                    }
                    break;
                case (Keys.Delete):
                    break;
            }
        }

        private void configurationsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (configuration == null || configuration.Visible == false)
            {
                configuration = new nodindowObjects.forms.configuration(new List<Form>() { this });
                configuration.Show();
            }
        }
    }
}
