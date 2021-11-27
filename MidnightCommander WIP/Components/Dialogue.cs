using System;
using System.Collections.Generic;
using MidnightCommander.Managers;
using MidnightCommander.Components.Subcomponents;

namespace MidnightCommander.Components
{
    class Dialogue : IComponent
    {
        ColorManager CS = new ColorManager(ColorTransfer.colorScheme);
        private string colorScheme = ColorTransfer.colorScheme;

        private int width;
        private int height;

        private List<string> msgs = new List<string>();
        private string checkboxText;
        private List<Button> buttons = new List<Button>();
        private List<TextBox> textboxes = new List<TextBox>();
        private List<CheckBox> checkboxes = new List<CheckBox>();
        private List<string> headers = new List<string>() { " Chyba ", " Kopírovat ", " Mazat ", " Vytvářet ", " Přesunout ", " Přejmenovat ", " Schéma " };

        public string Path = " ";
        public string Dest = "";

        public int Code;
        public int LastSelectedIndex = 0;
        public int offset = 2;

        public Application ParentWindow { get; set; }

        public bool IsActive { get; set; }

        public Dialogue(Application parentWindow = null)
        {
            ParentWindow = parentWindow;
        }

        public void Draw()
        {
            if(colorScheme != ColorTransfer.colorScheme)
            {
                CS = new ColorManager(ColorTransfer.colorScheme);
                colorScheme = ColorTransfer.colorScheme;
            }

            CodeHandle();

            DrawShadow();
            DrawBG();
            CreateActions();

            if(msgs.Count > 0)
                msgs = new List<string>();
        }

        public void DrawShadow()
        {
            CS.SetColor(3, 1);
            for (int i = 0; i <= height; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2 - width / 2 + 1, Console.WindowHeight / 2 - height / 2 + i + 1);
                Console.Write(" ".PadRight(width));
            }
        }

        public void DrawBG()
        {
            if(Code == 1)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                CS.SetColor(2, 1);
                CS.SetColor(3);
            }


            for (int j = 0; j <= height; j++)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2 - width / 2, Console.WindowHeight / 2 - height / 2 + j);

                if (j == 1)
                {
                    CS.SetColor(3);
                    Console.Write(" ┌" + "─".PadRight(width - 4, '─') + "┐ ");

                    if (Code == 1)
                    {
                        Console.SetCursorPosition(Console.WindowWidth / 2 - headers[0].Length / 2, Console.WindowHeight / 2 - height / 2 + j);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(headers[0]);
                    }
                    else
                    {
                        Console.SetCursorPosition(Console.WindowWidth / 2 - headers[Code - 1].Length / 2, Console.WindowHeight / 2 - height / 2 + j);
                        CS.SetColor(4);
                        Console.Write(headers[Code - 1]);
                    }

                    CS.SetColor(3);
                }
                else if (j == 2)
                {
                    if (Code == 1 || Code == 3)
                    {
                        if(msgs.Count == 0)
                            msgs.Add("CHYBA: ");
                        string temp = "";
                        if (Code == 3)
                            temp = "  ";
                        Console.Write(" │ " + msgs[0] + Path + temp + " │ ");
                    }
                    else if (Code == 2 || Code == 5)
                        Console.Write(" │ " + Path + " -> " + Dest + " │ ");
                    else
                        Console.Write(" │ ".PadRight(width - 3) + " │ ");
                }
                else if (j == 3)
                    Console.Write(" ├" + "─".PadRight(width - 4, '─') + "┤ ");
                else if (j == height - 1)
                    Console.Write(" └" + "─".PadRight(width - 4, '─') + "┘ ");
                else if (j == 0 || j == height)
                    Console.Write(" ".PadRight(width));
                else
                    Console.Write(" │" + " ".PadRight(width - 4) + "│ ");
            }
        }

        public void CreateActions()
        {
            if (height % 2 != 0)
                offset = 1;
            else
                offset = 2;

            buttons = new List<Button>();

            if (Code == 1)
            {
                buttons.Add(new Button(Console.WindowWidth / 2 - 3, Console.WindowHeight / 2 + height / 2 - offset, true, 1, true));
                buttons[0].Draw();
                buttons[0].Press += CloseDialogue;
            }
            else if (Code != 1)
            {
                buttons.Add(new Button(Console.WindowWidth / 2 - 5, Console.WindowHeight / 2 + height / 2 - offset, true, 2));
                buttons[0].Draw();
                buttons[0].Press += FileActions;

                buttons.Add(new Button(Console.WindowWidth / 2, Console.WindowHeight / 2 + height / 2 - offset, false, 1));
                buttons[1].Draw();
                buttons[1].Press += CloseDialogue;
            }


            textboxes = new List<TextBox>();

            if (Code != 1 && Code != 3)
            {
                if(Code == 2 || Code == 5)
                {
                    textboxes.Add(new TextBox(Console.WindowWidth / 2 - width / 2 + 2, Console.WindowHeight / 2 - 1, width));
                    textboxes[0].Text = Dest;
                    textboxes[0].Draw();
                }
                else
                {
                    textboxes.Add(new TextBox(Console.WindowWidth / 2 - width / 2 + 2, Console.WindowHeight / 2 - 1, width));
                    textboxes[0].Draw();
                }
            }

            checkboxes = new List<CheckBox>();

            if (Code == 6 || Code == 4)
            {
                checkboxes.Add(new CheckBox(Console.WindowWidth / 2 - checkboxText.Length / 2 - 2, Console.WindowHeight / 2 + height / 2 - offset - 1, false, 2, checkboxText));
                checkboxes[0].Draw();
            }
        }

        public void CodeHandle()
        {
            /* 1 - Error
             * 2 - Copy
             * 3 - Delete
             * 4 - Create
             * 5 - Move
             * 6 - Rename
             */

            if (Code == 1)
            {
                // v případě chyby se dá přepsat path na chybovou hlášku = nemusí se vytvářet extra proměnná

                msgs.Add("CHYBA: ");

                if (Path == " ")
                    Path = ParentWindow.Path;
                width = msgs[0].Length + Path.Length + 6;
                height = 6;
            }
            else if (Code == 2)
            {
                if (ParentWindow.PathDest == null)
                    Error("SLOŽKA PRO KOPII");
                else
                {
                    Path = ParentWindow.Path;
                    Dest = ParentWindow.PathDest;
                    width = Path.Length + Dest.Length + 10;
                    height = 6;
                }
            }
            else if (Code == 3)
            {
                msgs.Add("Chcete odstranit: ");
                msgs.Add(" ?");
                Path = ParentWindow.Path;
                width = msgs[0].Length + msgs[1].Length + Path.Length + 6;
                height = 6;
            }
            else if (Code == 4)
            {
                checkboxText = "Je soubor?";
                Path = ParentWindow.Path;
                msgs.Add("");
                width = Console.WindowWidth / 3;
                height = 7;
            }
            else if (Code == 5)
            {
                if (ParentWindow.PathDest == null)
                    Error("SLOŽKA PRO PŘESUN");
                else
                {
                    Path = ParentWindow.Path;
                    Dest = ParentWindow.PathDest;
                    width = Path.Length + Dest.Length + 10;
                    height = 6;
                }
            }
            else if (Code == 6)
            {
                checkboxText = "Zachovat typ souboru?";
                Path = ParentWindow.Path;
                msgs.Add("");
                width = Console.WindowWidth / 3;
                height = 7;
            }
        }

        public void Error(string message)
        {
            Path = message;
            Code = 1;
            Draw();
        }

        public void CloseDialogue()
        {
            foreach(Table t in ParentWindow.tables)
            {
                if (t.ERROR)
                {
                    t.ERROR = false;
                    t.HandleKey(new ConsoleKeyInfo('e', ConsoleKey.Enter, false, false, false));
                }    
            }

            ParentWindow.DialogActive = false;
            foreach(Table t in ParentWindow.tables)
            {
                t.Draw();
            }
            LastSelectedIndex = 0;
        }

        public void FileActions()
        {
            if (Code == 2)
            {
                Dest = textboxes[0].Text;
                FileManager fm = new FileManager(Path, Dest);
                try
                {
                    fm.Copy();
                }
                catch(System.IO.IOException a)
                {
                    ExceptionHandler eh = new ExceptionHandler(a);
                    ExceptionDraw(eh.message);
                }
            }
            else if (Code == 3)
            {
                FileManager fm = new FileManager(Path);
                try
                {
                    fm.Delete();
                }
                catch (System.IO.IOException a)
                {
                    ExceptionHandler eh = new ExceptionHandler(a);
                    ExceptionDraw(eh.message);
                }
            }
            else if (Code == 4)
            {
                Dest = Path.Remove(Path.LastIndexOf(@"\")) + @"\" + textboxes[0].Text;
                FileManager fm = new FileManager(Dest);
                try
                {
                    if (checkboxes[0].Type == 2)
                        fm.Create(true);
                    else
                        fm.Create();
                }
                catch (System.IO.IOException a)
                {
                    ExceptionHandler eh = new ExceptionHandler(a);
                    ExceptionDraw(eh.message);
                }
            }
            else if (Code == 5)
            {
                Dest = textboxes[0].Text + Path.Remove(0, Path.LastIndexOf(@"\"));
                FileManager fm = new FileManager(Path, Dest);
                try
                {
                    fm.Move();
                }
                catch (System.IO.IOException a)
                {
                    ExceptionHandler eh = new ExceptionHandler(a);
                    ExceptionDraw(eh.message);
                }
            }
            else if (Code == 6)
            {
                Dest = Path.Remove(Path.LastIndexOf(@"\")) + @"\" + textboxes[0].Text;
                FileManager fm = new FileManager(Path, Dest);
                try
                {
                    if (checkboxes[0].Type == 2)
                        fm.Rename();
                    else
                        fm.Rename(false);
                }
                catch (System.IO.IOException a)
                {
                    ExceptionHandler eh = new ExceptionHandler(a);
                    ExceptionDraw(eh.message);
                }
            }

            CloseDialogue();
            ParentWindow.Refresh();
        }

        public void ExceptionDraw(string message)
        {
            Error(message);
            while (Console.ReadKey(true).Key != ConsoleKey.Enter)
            { }
        }

        public void HandleKey(ConsoleKeyInfo info)
            {
            if(info.Key == ConsoleKey.Tab)
            {
                foreach (Button b in buttons)
                {
                    b.IsActive = false;
                }
                foreach (TextBox t in textboxes)
                {
                    t.IsActive = false;
                }
                foreach (CheckBox c in checkboxes)
                {
                    c.IsActive = false;
                }

                LastSelectedIndex++;

                if (LastSelectedIndex < buttons.Count)
                {
                    buttons[LastSelectedIndex].IsActive = true;
                }
                else if (LastSelectedIndex - buttons.Count < textboxes.Count)
                {
                    textboxes[LastSelectedIndex - buttons.Count].IsActive = true;
                }
                else if (LastSelectedIndex - buttons.Count - textboxes.Count < checkboxes.Count)
                {
                    checkboxes[LastSelectedIndex - buttons.Count - textboxes.Count].IsActive = true;
                }
                else
                {
                    LastSelectedIndex = 0;
                    buttons[0].IsActive = true;
                }        

                foreach (Button b in buttons)
                {
                    b.Draw();
                }
                foreach (TextBox t in textboxes)
                {
                    t.Draw();
                }
                foreach (CheckBox c in checkboxes)
                {
                    c.Draw();
                }
            }
            else if(info.Key == ConsoleKey.Enter)
            {
                foreach(Button b in buttons)
                {
                    if(b.IsActive)
                    {
                        b.HandleKey(info);
                    }
                }

                foreach (CheckBox c in checkboxes)
                {
                    if (c.IsActive)
                    {
                        c.HandleKey(info);
                    }
                }
            }
            else
            {
                foreach (TextBox t in textboxes)
                {
                    if(t.IsActive)
                        t.HandleKey(info);
                }
            }
        }
    }
}
