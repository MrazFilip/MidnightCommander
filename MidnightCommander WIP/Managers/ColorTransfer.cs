using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightCommander.Managers
{
    public static class ColorTransfer
    {
        public static int index = 0;
        public static List<string> schemes = new() { "Basic", "Bee", "EliteCommander", "ElectricBlue" };
        public static string colorScheme = schemes[index];
    }
}
