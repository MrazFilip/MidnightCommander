using System;
using MidnightCommander.Managers;

namespace MidnightCommander.Components
{
    class CheckBox : IComponent
    {
        ColorManager CS = new ColorManager(ColorTransfer.colorScheme);
        private string colorScheme = ColorTransfer.colorScheme;

        public string Text { get; set; }
        public int Posx { get; set; }
        public int Posy { get; set; }
        public bool IsActive { get; set; }
        public int Type { get; set; }
        public string Message { get; set; }

        public CheckBox(int posx, int posy, bool activation, int type, string message,bool iserror = false)
        {
            Message = message;
            Posx = posx;
            Posy = posy;
            IsActive = activation;
            Type = type;
            SelectedType();
        }

        public void SelectedType()
        {
            if (Type == 1)
                Text = "[ ]" + Message;
            if (Type == 2)
                Text = "[X]" + Message;
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
                CS.SetColor(2, 1);
                CS.SetColor(3);
                
                Console.SetCursorPosition(Posx, Posy);
                Console.Write(Text);
            }
        }

        public void DrawActive()
        {
            Console.SetCursorPosition(Posx, Posy);

            CS.SetColor(11, 1);
            CS.SetColor(3);
            
            Console.Write(Text);
        }

        public void HandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.Enter)
            {
                if (Type == 1)
                    Type = 2;
                else
                    Type = 1;
                SelectedType();
                Draw();
            }
        }
    }
}
