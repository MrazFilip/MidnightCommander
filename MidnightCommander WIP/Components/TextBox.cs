using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidnightCommander.Managers;

namespace MidnightCommander.Components
{
    public class TextBox : IComponent
    {
        ColorManager CS = new ColorManager(ColorTransfer.colorScheme);
        private string colorScheme = ColorTransfer.colorScheme;

        public string Text { get; set; }
        public int Posx { get; set; }
        public int Posy { get; set; }
        public bool IsActive { get; set; }
        public int Width { get; set; }
        public string PathStart { get; set; }
        public string PathDest { get; set; }
        public int Offest = 0;

        public TextBox(int posx, int posy, int dialoguewidth, string start = " ", string dest = " ")
        {
            Posx = posx;
            Posy = posy;
            Width = dialoguewidth;
            Text = "";
            IsActive = false;
        }

        public void Draw()
        {
            if (colorScheme != ColorTransfer.colorScheme)
            {
                CS = new ColorManager(ColorTransfer.colorScheme);
                colorScheme = ColorTransfer.colorScheme;
            }

            Console.SetCursorPosition(Posx, Posy);
            CS.SetColor(4, 1);
            Console.Write(new string (' ', Width - 4));

            Console.SetCursorPosition(Posx, Posy);
            if (IsActive)
            {
                CS.SetColor(5);
                if (Text == "")
                    Console.Write('*');
            }
            else
                CS.SetColor(1);


            if (Text.Length > Width - 4)
                Offest++;

            Console.Write(Text.Substring(Offest));
        }

        public void HandleKey(ConsoleKeyInfo info)
        {
            if(info.Key == ConsoleKey.Backspace)
            {
                if(Text.Length > 0)
                    Text = Text.Remove(Text.Length - 1);
            }
            else
            {
                Text += info.KeyChar.ToString();
            }

            Draw();
        }
    }
}
