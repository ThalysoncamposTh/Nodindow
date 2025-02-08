using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nodindow.myPackages
{
    public class contextMenuStripAdvanced : ContextMenuStrip
    {
        public ToolStripItem addItem(string text, Image image, Color color, EventHandler eventHandler)
        {
            Items.Add(text, image, eventHandler);
            Items[Items.Count - 1].BackColor = color;
            return Items[Items.Count - 1];
        }
        public ToolStripItem addItem(ToolStripMenuItem toolStripMenuItem)
        {
            Items.Add(toolStripMenuItem);
            return Items[Items.Count - 1];
        }
        public class ToolStripMenuItemAdvanced:ToolStripMenuItem
        {
            public ToolStripMenuItemAdvanced(string text, Image image, Color color)
            {
                this.Text = text;
                this.Image = image;
                this.BackColor = color;
            }
            public ToolStripItem addItem(string text, Image image, Color color, EventHandler eventHandler)
            {
                DropDownItems.Add(text, image, eventHandler);
                DropDownItems[DropDownItems.Count - 1].BackColor = color;
                return DropDownItems[DropDownItems.Count - 1];
            }
        }
    }
}
