using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nodindow.myPackages
{
    public class notifyIconManager
    {
        public NotifyIcon NotifyIcon = new NotifyIcon();
        public notifyIconManager(Icon icon,string text,bool visible) 
        {
            NotifyIcon.Icon = icon;
            NotifyIcon.Text = text;
            NotifyIcon.Visible = visible;
        }

        public void showDialog(string title, string content,int showTime = 3000)
        {
            NotifyIcon.ShowBalloonTip(showTime, title, content, ToolTipIcon.Info);
        }
        public void show()
        {
            NotifyIcon.Visible=true;
        }
        public void hide()
        {
            NotifyIcon.Visible=false;
        }
    }
}
