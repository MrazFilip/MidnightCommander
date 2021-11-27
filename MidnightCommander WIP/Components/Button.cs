using System;
using MidnightCommander.Managers;

namespace MidnightCommander.Components
{
    class Button : IComponent
    {
        ColorManager CS = new ColorManager(ColorTransfer.colorScheme);
        private string colorScheme = ColorTransfer.colorScheme;

        public event Action Press;
        public string Text { get; set; }
        public int Posx { get; set; }
        public int Posy { get; set; }
        public bool IsActive { get; set; }
        public int Type { get; set; }
        public int Error { get; set; }

        public Button(int posx, int posy, bool activation, int type, bool iserror = false)
        {
            Posx = posx;
            Posy = posy;
            IsActive = activation;
            Type = type;
            SelectedType();

            if (iserror)
                Error = 1;
        }

        public void SelectedType()
        {
            /* 1 = Close
             * 2 = Confirm Action
             */
            
            if (Type == 1)
                Text = "[Close]";
            if (Type == 2)
                Text = "[OK]";
        }

        public void Draw()
        {
            if (colorScheme != ColorTransfer.colorScheme)
            {
                CS = new ColorManager(ColorTransfer.colorScheme);
                colorScheme = ColorTransfer.colorScheme;
            }

            if (IsActive)
                DrawActive();
            else
            {
                if(Error == 1)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    CS.SetColor(2, 1);
                    CS.SetColor(3);
                }
                Console.SetCursorPosition(Posx, Posy);
                Console.Write(Text);
            }
        }

        public void DrawActive()
        {
            Console.SetCursorPosition(Posx, Posy);
            if(Error == 1)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                CS.SetColor(11, 1);
                CS.SetColor(3);
            }
            Console.Write(Text);
        }

        public void HandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.Enter)
                this.Press();
        }
    }
}
