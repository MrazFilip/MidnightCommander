using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidnightCommander.Components;

namespace MidnightCommander
{
    class Application
    {
        public Window ActiveWindow { get; set; }
        public FileBrowserWindow SavedBrowser { get; set; }

        public Application()
        {
            SavedBrowser = new FileBrowserWindow(this);
            ActiveWindow = SavedBrowser;
        }

        public void HandleKey(ConsoleKeyInfo info)
        {
            ActiveWindow.HandleKey(info);
        }
    }
}
