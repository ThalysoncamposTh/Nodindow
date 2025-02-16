using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nodindow.myPackages.formTools
{
    public partial class boundsView : Form
    {
        Image image { get; set; }
        public boundsView(Size size, Point position, Color color, int timeVisible = 1000, int borderWidth = 2)
        {
            InitializeComponent();
            this.Width = size.Width;
            this.Height = size.Height;
            this.Location = position;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            pictureBox1.Image = imageManager.CreateImageWithBorder(pictureBox1.Width, pictureBox1.Height, borderWidth, color, Color.Fuchsia);
            await.Interval = timeVisible;
            await.Start();
        }

        private void await_Tick(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
            await.Stop();
        }
    }
}
