using System.Linq;
using System.Windows;

namespace Kastelo
{
    public partial class App
    {
        public static string[] Args;
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            Args = e.Args.Length > 0 ? e.Args : new string[0];
        }
    }
}
