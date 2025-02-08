using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static Nodindow.myPackages.TreeViewAdvanced;

namespace Nodindow.myPackages
{
    public class TreeViewAdvanced : TreeView
    {
        #region variableDeclarations
        public List<TreeNodeAdvanced> TreeNodeAdvancedBD = new List<TreeNodeAdvanced>();
        public TreeNodeAdvancedIcons TreeNodeAdvancedIconsBD { get; set; }
        public event EventHandler<DragDropTreeViewAdvancedEventArgs> AfterDragDropTreeViewAdvanced;
        public bool _allowDragDropInternalNodes = true;
        protected virtual void _afterDragDropTreeViewAdvanced(DragDropTreeViewAdvancedEventArgs e)
        {
            AfterDragDropTreeViewAdvanced?.Invoke(this, e);
        }
        [Category("Propriedade personalizada")]
        [Description("Indica se o usuario pode arrastar e soltar dentro de outro node")]
        public bool AllowDragDropInternalNodes
        {
            get { return _allowDragDropInternalNodes; }
            set { _allowDragDropInternalNodes = value; }
        }
        public TreeViewAdvanced()
        {
            TreeNodeAdvancedIconsBD = new TreeNodeAdvancedIcons(this);
            if (_allowDragDropInternalNodes)
            {
                this.ItemDrag += new ItemDragEventHandler((object sender, ItemDragEventArgs e) =>
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (((TreeNodeAdvanced)((TreeNode)e.Item).Tag).allowItemDrag)
                        {
                            DoDragDrop(e.Item, DragDropEffects.Move);
                        }
                    }
                });
                this.DragEnter += new DragEventHandler((object sender, DragEventArgs e) =>
                {
                    e.Effect = DragDropEffects.Move;
                });
                this.DragOver += new DragEventHandler((object sender, DragEventArgs e) =>
                {
                    Point targetPoint = this.PointToClient(new Point(e.X, e.Y));
                    TreeNode targetNode = this.GetNodeAt(targetPoint);
                    TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
                    if (targetNode != null && ((TreeNodeAdvanced)targetNode.Tag).allowItemNameDrop != null && ((TreeNodeAdvanced)targetNode.Tag).allowItemNameDrop.FindIndex(item => item == draggedNode.Name) != -1)
                    {
                        e.Effect = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                });
                this.DragDrop += new DragEventHandler((object sender, DragEventArgs e) =>
                {
                    Point targetPoint = this.PointToClient(new Point(e.X, e.Y));
                    TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
                    TreeNode targetNode = this.GetNodeAt(targetPoint);
                    if (targetNode != null && !draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode) && ((TreeNodeAdvanced)targetNode.Tag).allowItemNameDrop != null)
                    {
                        if ((((TreeNodeAdvanced)targetNode.Tag).allowItemNameDrop.FindIndex(item => item == draggedNode.Name)) != -1)
                        {
                            TreeNodeAdvanced treeNodeAdvancedDragged = (TreeNodeAdvanced)draggedNode.Tag;
                            TreeNodeAdvanced treeNodeAdvancedTarget = (TreeNodeAdvanced)targetNode.Tag;
                            if (treeNodeAdvancedTarget.childrens.FindIndex(node => node.text == treeNodeAdvancedDragged.text && node.name == treeNodeAdvancedDragged.name) == -1)
                            {
                                _afterDragDropTreeViewAdvanced(new DragDropTreeViewAdvancedEventArgs(treeNodeAdvancedTarget, treeNodeAdvancedTarget.add(treeNodeAdvancedDragged)));
                                treeNodeAdvancedDragged.remove();
                            }
                        }
                    }
                    bool ContainsNode(TreeNode node1, TreeNode node2)
                    {
                        if (node2.Parent == null) return false;
                        if (node2.Parent.Equals(node1)) return true;
                        return ContainsNode(node1, node2.Parent);
                    }
                });
            }
        }
        #endregion
        public TreeNodeAdvanced addTreeNodeWithoutParent(TreeNodeAdvanced treeNodeAdvancedChildren)
        {
            treeNodeAdvancedChildren.parent = null;
            treeNodeAdvancedChildren.TreeViewAdvanced = this;
            TreeNodeAdvancedBD.Add(treeNodeAdvancedChildren);
            Nodes.Add(treeNodeAdvancedChildren.text);
            Nodes[Nodes.Count - 1].Name = treeNodeAdvancedChildren.name;
            Nodes[Nodes.Count - 1].ImageIndex = TreeNodeAdvancedIconsBD.getIndex(treeNodeAdvancedChildren.icon);
            Nodes[Nodes.Count - 1].SelectedImageIndex = TreeNodeAdvancedIconsBD.getIndex(treeNodeAdvancedChildren.icon);
            Nodes[Nodes.Count - 1].Tag = TreeNodeAdvancedBD[TreeNodeAdvancedBD.Count - 1];
            ((TreeNodeAdvanced)Nodes[Nodes.Count - 1].Tag).treeNode = Nodes[Nodes.Count - 1];
            return TreeNodeAdvancedBD[TreeNodeAdvancedBD.Count - 1];
        }
        public TreeNodeAdvanced find(string pathTexts, string pathNames)
        {
            string[] texts = pathTexts.Split('\\');
            string[] names = pathNames.Split('\\');
            TreeNodeAdvanced treeNodeAdvancedLocated = null;
            if ((texts != null && names != null) && (texts.Length == names.Length) && (texts.Length > 0))
            {

                treeNodeAdvancedLocated = TreeNodeAdvancedBD.Find(i2 => i2.text == texts[0] && i2.name == names[0]);
                for (int i = 1; i < texts.Length; i++)
                {
                    if (treeNodeAdvancedLocated.childrens.Count > 0)
                    {
                        treeNodeAdvancedLocated = treeNodeAdvancedLocated.childrens.Find(i2 => i2.text == texts[i] && i2.name == names[i]);
                    }
                    else { break; }
                }
            }
            return treeNodeAdvancedLocated;
        }
        public void clear(bool clearIcons = false)
        {
            TreeNodeAdvancedBD.Clear();
            if (clearIcons)
            {
                TreeNodeAdvancedIconsBD.iconsAdvancedBD.Clear();
            }
            this.Nodes.Clear();
        }
        public void loadJsonSaved(string json, List<TreeNodeAdvancedObjectParameter> treeNodeAdvancedObjectParameter)
        {
            TreeNodeAdvancedBD.Clear();
            List<TreeNodeAdvanced> TreeNodeAdvancedBDT = JsonConvert.DeserializeObject<List<TreeNodeAdvanced>>(json);
            for (int i = 0; i < TreeNodeAdvancedBDT.Count; i++)
            {
                TreeNodeAdvanced newParent = addTreeNodeWithoutParent(new TreeNodeAdvanced(TreeNodeAdvancedBDT[i]));
                TreeNodeAdvancedObjectParameter treeNodeAdvancedObjectParameter1 = treeNodeAdvancedObjectParameter.Find(i2 => i2.nameParameter == TreeNodeAdvancedBDT[i].name);
                if (treeNodeAdvancedObjectParameter1 != null)
                {
                    newParent.userObject = TreeNodeAdvancedBDT[i].userObject == null ? TreeNodeAdvancedBDT[i].userObject : treeNodeAdvancedObjectParameter1.load(TreeNodeAdvancedBDT[i].userObject.ToString(), newParent);
                }
                foreach (TreeNodeAdvanced child in TreeNodeAdvancedBDT[i].childrens)
                {
                    replaceInternalUserObjects(child, newParent);
                }
            }
            void replaceInternalUserObjects(TreeNodeAdvanced child, TreeNodeAdvanced parent)
            {
                TreeNodeAdvanced newParent = parent.add(new TreeNodeAdvanced(child));
                TreeNodeAdvancedObjectParameter treeNodeAdvancedObjectParameter1 = treeNodeAdvancedObjectParameter.Find(i2 => i2.nameParameter == child.name);
                if (treeNodeAdvancedObjectParameter1 != null)
                {
                    newParent.userObject = child.userObject == null ? child.userObject : treeNodeAdvancedObjectParameter1.load(child.userObject.ToString(), newParent);
                }
                foreach (TreeNodeAdvanced child1 in child.childrens)
                {
                    replaceInternalUserObjects(child1, newParent);
                }
            }
        }
        public string getJsonSave()
        {
            List<TreeNodeAdvanced> TreeNodeAdvancedSave = new List<TreeNodeAdvanced>();
            foreach (TreeNodeAdvanced treeNodeAdvanced in TreeNodeAdvancedBD)
            {
                if (treeNodeAdvanced.saveTreeNodeAdvanced)
                {
                    TreeNodeAdvanced treeNodeAdvanced1 = new TreeNodeAdvanced(treeNodeAdvanced);
                    treeNodeAdvanced1.childrens.AddRange(getSavedNodeschildrens(treeNodeAdvanced));
                    TreeNodeAdvancedSave.Add(treeNodeAdvanced1);
                }
            }
            return JsonConvert.SerializeObject(TreeNodeAdvancedSave, Formatting.Indented);
            List<TreeNodeAdvanced> getSavedNodeschildrens(TreeNodeAdvanced treeNodeToSeparateToSave)
            {
                List<TreeNodeAdvanced> treeNodesAdvanced = new List<TreeNodeAdvanced>();
                foreach (TreeNodeAdvanced treeNodeAdvanced in treeNodeToSeparateToSave.childrens)
                {
                    if (treeNodeAdvanced.saveTreeNodeAdvanced)
                    {
                        TreeNodeAdvanced treeNodeAdvanced1 = new TreeNodeAdvanced(treeNodeAdvanced);
                        treeNodeAdvanced1.childrens.AddRange(getSavedNodeschildrens(treeNodeAdvanced));
                        treeNodesAdvanced.Add(treeNodeAdvanced1);
                    }
                }
                return treeNodesAdvanced;
            }
        }
        #region classUtil
        public class TreeNodeAdvanced
        {
            private string _text;
            public string text
            {
                get { return _text; }
                set
                {
                    _text = value;
                    if (treeNode != null)
                    {
                        treeNode.Text = value;
                    }
                }
            }
            public string _name { get; set; }
            public string name
            {
                get { return _name; }
                set
                {
                    _name = value;
                    if (treeNode != null)
                    {
                        treeNode.Name = value;
                    }
                }
            }
            public object userObject { get; set; }
            public bool locked { get; set; }
            public bool allowItemDrag { get; set; }
            public List<string> allowItemNameDrop { get; set; }
            public bool saveTreeNodeAdvanced { get; set; }
            [JsonIgnore]
            public TreeNode treeNode { get; set; }
            public TreeNodeAdvancedIcons.iconAdvanced icon { get; set; }
            public List<TreeNodeAdvanced> childrens = new List<TreeNodeAdvanced>();
            [JsonIgnore]
            public TreeNodeAdvanced parent { get; set; }
            [JsonIgnore]
            public TreeViewAdvanced TreeViewAdvanced { get; set; }
            #region comment
            /// <summary>
            /// classe utilizada para armazenar informações que serão consultadas e armazenadas
            /// </summary>
            /// <param name="text">texto a ser exibido do node</param>
            /// <param name="name">nome do node</param>
            /// <param name="userObject">objeto que o usuario desejar armazenar dentro do node</param>
            /// <param name="treeNode">node do TreeNode ao qual o TreeNodeAdvanced representa</param>
            /// <param name="icon">é onde vai ficar armazenado o icon do node</param>
            /// <param name="treeNodeAdvancedParent">pai do treeNodeAdvanced</param>
            /// <param name="treeNodeAdvancedChildrens">filhos do TreeNodeAdvanced</param>
            /// <param name="locked">representa se o treeNodeAdvanced será bloqueado de ser excluido</param>
            #endregion
            [JsonConstructor]
            public TreeNodeAdvanced(string text, string name, object userObject, TreeNodeAdvancedIcons.iconAdvanced icon, bool saveTreeNodeAdvanced = true, bool locked = false, bool allowItemDrag = false, List<string> allowItemNameDrop = null)
            {
                setAttributes(text, name, userObject, icon, saveTreeNodeAdvanced, locked, allowItemDrag, allowItemNameDrop);
            }
            public TreeNodeAdvanced(TreeNodeAdvanced treeNodeAdvanced)
            {
                setAttributes(treeNodeAdvanced.text, treeNodeAdvanced.name, treeNodeAdvanced.userObject, treeNodeAdvanced.icon, treeNodeAdvanced.saveTreeNodeAdvanced, treeNodeAdvanced.locked, treeNodeAdvanced.allowItemDrag, treeNodeAdvanced.allowItemNameDrop);
            }
            public TreeNodeAdvanced(string text,object userObject, defaultParameters defaultParameters)
            {
                setAttributes(text, defaultParameters.name, userObject, defaultParameters.icon, defaultParameters.saveTreeNodeAdvanced, defaultParameters.locked, defaultParameters.allowItemDrag, defaultParameters.allowItemNameDrop);
            }
            private void setAttributes(string text, string name, object userObject, TreeNodeAdvancedIcons.iconAdvanced icon, bool saveTreeNodeAdvanced = true, bool locked = false, bool allowItemDrag = false, List<string> allowItemNameDrop = null)
            {
                this.text = text;
                this.name = name;
                this.userObject = userObject;
                this.icon = icon;
                this.saveTreeNodeAdvanced = saveTreeNodeAdvanced;
                this.locked = locked;
                this.allowItemDrag = allowItemDrag;
                this.allowItemNameDrop = allowItemNameDrop;
            }
            public TreeNodeAdvanced add(TreeNodeAdvanced treeNodeAdvanced)
            {
                if (childrens.FindIndex(i => i.text == treeNodeAdvanced.text && i.name == treeNodeAdvanced.name) == -1)
                {
                    if (treeNodeAdvanced != null)
                    {
                        return addNewTreeNodeAdvanced(treeNodeAdvanced, this);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return childrens.Find(i => i.text == treeNodeAdvanced.text && i.name == treeNodeAdvanced.name);
                }
                TreeNodeAdvanced addNewTreeNodeAdvanced(TreeNodeAdvanced treeNodeAdvanced1, TreeNodeAdvanced parentTreeNodeAdvanced)
                {
                    TreeNodeAdvanced newTreeNodeAdvanced = new TreeNodeAdvanced(treeNodeAdvanced1);
                    newTreeNodeAdvanced.parent = parentTreeNodeAdvanced;
                    newTreeNodeAdvanced.TreeViewAdvanced = this.TreeViewAdvanced;
                    parentTreeNodeAdvanced.treeNode.Nodes.Add(newTreeNodeAdvanced.text);
                    parentTreeNodeAdvanced.treeNode.Nodes[parentTreeNodeAdvanced.treeNode.Nodes.Count - 1].Name = newTreeNodeAdvanced.name;
                    parentTreeNodeAdvanced.treeNode.Nodes[parentTreeNodeAdvanced.treeNode.Nodes.Count - 1].ImageIndex = TreeViewAdvanced.TreeNodeAdvancedIconsBD.getIndex(newTreeNodeAdvanced.icon);
                    parentTreeNodeAdvanced.treeNode.Nodes[parentTreeNodeAdvanced.treeNode.Nodes.Count - 1].SelectedImageIndex = TreeViewAdvanced.TreeNodeAdvancedIconsBD.getIndex(newTreeNodeAdvanced.icon);
                    parentTreeNodeAdvanced.treeNode.Nodes[parentTreeNodeAdvanced.treeNode.Nodes.Count - 1].Tag = newTreeNodeAdvanced;
                    newTreeNodeAdvanced.treeNode = parentTreeNodeAdvanced.treeNode.Nodes[parentTreeNodeAdvanced.treeNode.Nodes.Count - 1];
                    parentTreeNodeAdvanced.childrens.Add(newTreeNodeAdvanced);
                    foreach (TreeNodeAdvanced child in treeNodeAdvanced1.childrens)
                    {
                        addNewTreeNodeAdvanced(child, parentTreeNodeAdvanced.childrens[parentTreeNodeAdvanced.childrens.Count - 1]);
                    }
                    return parentTreeNodeAdvanced.childrens[parentTreeNodeAdvanced.childrens.Count - 1];
                }
            }
            public void remove()
            {
                if (!this.locked)
                {
                    this.treeNode.Remove();
                    parent.childrens = parent.childrens.Where(i => i.name != this.name || i.text != this.text).ToList();
                }
            }
            public void rename(string text, string name = null)
            {
                if (parent != null && !this.locked)
                {
                    if (parent.childrens.FindIndex(i => i.text == text && i.name == (name != null ? name : this.name)) == -1)
                    {
                        this.text = text != null ? text : this.text;
                        this.name = name != null ? name : this.name;
                        this.treeNode.Text = text != null ? text : this.text;
                        this.treeNode.Name = name != null ? name : this.name;
                    }
                }
            }
            public string getFullPathText()
            {
                if (parent != null)
                {
                    return $"{parent.getFullPathText()}\\{text}";
                }
                else
                {
                    return text;
                }
            }
            public string getFullPathName()
            {
                if (parent != null)
                {
                    return $"{parent.getFullPathName()}\\{name}";
                }
                else
                {
                    return name;
                }
            }
            public class defaultParameters
            {
                public string name { get; set; }
                public bool locked { get; set; }
                public bool allowItemDrag { get; set; }
                public List<string> allowItemNameDrop { get; set; }
                public bool saveTreeNodeAdvanced { get; set; }
                public TreeNodeAdvancedIcons.iconAdvanced icon { get; set; }
                public defaultParameters(string name,TreeNodeAdvancedIcons.iconAdvanced icon, bool saveTreeNodeAdvanced = true, bool locked = false, bool allowItemDrag = false, List<string> allowItemNameDrop = null)
                {
                    this.name = name;
                    this.icon = icon;
                    this.saveTreeNodeAdvanced = saveTreeNodeAdvanced;
                    this.locked = locked;
                    this.allowItemDrag = allowItemDrag;
                    this.allowItemNameDrop = allowItemNameDrop;
                }
            }
        }
        public class TreeNodeAdvancedIcons
        {
            private TreeViewAdvanced treeViewAdvanced { get; set; }
            public List<iconAdvanced> iconsAdvancedBD = new List<iconAdvanced>();
            public ImageList ImageList = new ImageList();
            public TreeNodeAdvancedIcons(TreeViewAdvanced treeViewAdvanced)
            {
                this.treeViewAdvanced = treeViewAdvanced;
                ImageList.ColorDepth = ColorDepth.Depth32Bit;
            }
            public void updateIconList()
            {
                ImageList.Images.Clear();
                foreach (iconAdvanced iconAdvanced in iconsAdvancedBD)
                {
                    ImageList.Images.Add(iconAdvanced.icon);
                }
                treeViewAdvanced.ImageList = ImageList;
            }
            public void add(iconAdvanced iconAdvanced)
            {
                if (iconsAdvancedBD.FindIndex(i => i.name == iconAdvanced.name) == -1)
                {
                    iconsAdvancedBD.Add(iconAdvanced);
                    updateIconList();
                }
            }
            public void remove(iconAdvanced iconAdvanced)
            {
                iconsAdvancedBD = iconsAdvancedBD.Where(i2 => i2.name != iconAdvanced.name).ToList();
                updateIconList();
            }
            public int getIndex(iconAdvanced iconAdvanced)
            {
                return iconsAdvancedBD.FindIndex(i => i.name == iconAdvanced.name);
            }
            public iconAdvanced getIconAdvanced(iconAdvanced iconAdvanced)
            {
                return iconsAdvancedBD[iconsAdvancedBD.FindIndex(i => i.name == iconAdvanced.name)];
            }
            public class iconAdvanced
            {
                public string name { get; set; }
                [JsonIgnore]
                public Icon icon { get; set; }
                public bool saveIcon { get; set; }
                public string iconRelactivePath { get; set; }
                #region comment
                /// <summary>
                /// utilizado para criar um novo icon para o treeNode
                /// </summary>
                /// <param name="name">nome dado ao icon</param>
                /// <param name="icon">icon que será utilizado no treeNode</param>
                /// <param name="saveIcon">se o icon será salvo junto ao arquivo do treeNodeAdvancedIcons</param>
                #endregion
                public iconAdvanced(string name, string iconPath = null, bool saveIcon = false)
                {
                    this.name = name;
                    this.icon = iconPath == null ? null : new Icon(iconPath);
                    this.saveIcon = saveIcon;
                    this.iconRelactivePath = iconPath;
                }
            }
        }
        public class TreeNodeAdvancedObjectParameter
        {
            public string nameParameter { get; set; }
            public object objectLoaded { get; set; }

            public virtual object load(string json, TreeNodeAdvanced treeNodeAdvanced)
            {
                return objectLoaded;
            }
        }
        public class DragDropTreeViewAdvancedEventArgs : EventArgs
        {
            public TreeNodeAdvanced targetNode { get; set; }
            public TreeNodeAdvanced draggedNode { get; set; }

            public DragDropTreeViewAdvancedEventArgs(TreeNodeAdvanced targetNode, TreeNodeAdvanced draggedNode)
            {
                this.targetNode = targetNode;
                this.draggedNode = draggedNode;
            }
        }
        #endregion
    }
}
