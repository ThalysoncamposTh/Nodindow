using System;
using System.Windows.Forms;

namespace Nodindow
{
    internal static class Program
    {
        /// <summary>
        /// Main entry point to the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
