using System;
using System.Collections.Generic;

namespace MidnightCommander.Managers
{
    class ColorManager
    {
        public List<ConsoleColor> colors = new();

        public ColorManager(string scheme)
        {
            SetColorScheme(scheme);
        }

        /* Color numbering
         * 0 - BG
         * 1 - Text
         * 2 - Path BG
         * 3 - Path text
         * 4 - Select BG
         * 5 - Select text
         * 6 - Button bg
         * 7 - Button text
         * 8 - Button Number BG
         * 9 - Button number
         * 10 - Info Color
         * 11 - Dialogue selected bg
         */

        public void SetColorScheme(string scheme)
        {
            if (scheme == "Basic")
            {
                colors.Add(ConsoleColor.DarkBlue);
                colors.Add(ConsoleColor.Gray);
                colors.Add(ConsoleColor.Gray);
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.DarkCyan);
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.DarkCyan);
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.DarkBlue);
                colors.Add(ConsoleColor.White);
                colors.Add(ConsoleColor.Gray);
                colors.Add(ConsoleColor.DarkCyan);
            }
            else if (scheme == "Bee")
            {
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.Gray);
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.DarkYellow);
                colors.Add(ConsoleColor.DarkYellow);
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.Yellow);
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.DarkYellow);
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.DarkYellow);
                colors.Add(ConsoleColor.Gray);
            }
            else if (scheme == "EliteCommander")
            {
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.DarkGray);
                colors.Add(ConsoleColor.Gray);
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.Red);
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.DarkGray);
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.DarkGray);
                colors.Add(ConsoleColor.Red);
                colors.Add(ConsoleColor.Red);
            }
            else if (scheme == "ElectricBlue")
            {
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.Gray);
                colors.Add(ConsoleColor.DarkCyan);
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.DarkGray);
                colors.Add(ConsoleColor.Gray);
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.DarkGray);
                colors.Add(ConsoleColor.Black);
                colors.Add(ConsoleColor.DarkGray);
                colors.Add(ConsoleColor.Blue);
                colors.Add(ConsoleColor.Blue);
            }
        }

        public void SetColor(int index, int type = 0)
        {
            /* 0 = FG color
             * 1 = BG color
             */

            if (type == 0)
            {
                Console.ForegroundColor = colors[index];
            }
            else
            {
                Console.BackgroundColor = colors[index];
            }
        }
    }
}
