using Project_pac.UI;
using System;
using System.Windows.Forms;

namespace Project_pac
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GameLoad());
        }
    }
}
