using System;
using System.Collections.Generic;
using MidnightCommander.Components;
using MidnightCommander.Managers;
using System.Diagnostics;

namespace MidnightCommander
{
    class Application : Window
    {
        ColorManager CS = new ColorManager(ColorTransfer.colorScheme);
        private string colorScheme = ColorTransfer.colorScheme;
        public int SelectedScheme = 1;

        public List<Table> tables = new List<Table>();
        public List<string> schemes = new() { "Basic", "Bee", "EliteCommander", "ElectricBlue" };
        public List<string> menu = new List<string>() { "Škálovat", "Přejmenuj", "Zobraz", "Upravit", "Kopie", "Přesun", "Vytvořit", "Smazat", "Barvy", "Konec" };
        public Dialogue Dialogue { get; set; }
        public bool DialogActive = false;
        public string Path;
        public string PathDest;

        public Application()
        {
            Dialogue = new Dialogue(this);

            Table l = new (0);
            Table r = new (Console.WindowWidth / 2);
            l.IsActive = true;

            tables.Add(l);
            tables.Add(r);

            l.Draw();
            r.Draw();

            DrawMenu();
        }

        //Extenduje HandleKey od Window
        public override void HandleKey(ConsoleKeyInfo info)
        {
            if(DialogActive)
            {
                if(info.Key == ConsoleKey.Escape)
                {
                    return;
                }
            }

            if(!DialogActive)
            {
                foreach (Table table in tables)
                {
                    if(table.IsActive)
                    {
                        table.HandleKey(info);
                        if (table.ERROR)
                        {
                            DialogActive = true;
                            Dialogue.Path = "NEMÁTE PŘÍSTUP DO TÉTO SLOŽKY";
                            Dialogue.Code = 1;
                            Dialogue.Draw();
                        }
                        else
                            table.Draw();
                        if(table.Path.Length > 3)
                            Path = table.Path + @"\" + table.selectedFile;
                        else
                            Path = table.Path + table.selectedFile;
                    }
                }
            }
            else if (info.Key != ConsoleKey.Tab)
            {
                Dialogue.HandleKey(info);
            }

            if (info.Key == ConsoleKey.Tab)
            {
                if(!DialogActive)
                {
                    foreach (Table table in tables)
                    {
                        if (table.IsActive)
                        {
                            if (table.Path.Length > 3)
                                PathDest = table.Path + @"\";
                            else
                                PathDest = table.Path;
                        }

                        table.IsActive = !table.IsActive;
                        table.Draw();
                    }
                }
                else
                {
                    Dialogue.HandleKey(info);
                }
            }
            else if (info.Key == ConsoleKey.F1)
            {
                if (Console.WindowWidth >= 60)
                {
                    Console.Clear();
                    foreach (Table table in tables)
                    {
                        table.Width = Console.WindowWidth / 2 - 2;
                        table.Height = Console.WindowHeight;
                        table.NewLists();
                        table.Add();
                        if (table.WidthStart == 0)
                        {
                            table.Draw();
                        }
                        else
                        {
                            table.WidthStart = Console.WindowWidth / 2;
                            table.Draw();
                        }
                    }
                }
                else
                {
                    Console.WindowWidth = 60;
                    HandleKey(new ConsoleKeyInfo('R', ConsoleKey.R, false, false, false));
                }

                menu = new List<string>() { "Škálovat", "Přejmenuj", "Zobraz", "Upravit", "Kopie", "Přesun", "Vytvořit", "Smazat", "Barvy", "Konec" };
                DrawMenu();
            }
            else if (info.Key == ConsoleKey.F2)
            {
                DialogActive = true;
                Dialogue.Code = 6;
                Dialogue.Draw();
            }
            else if (info.Key == ConsoleKey.F3)
            {
                foreach(Table t in tables)
                {
                    if(t.IsActive)
                    {
                        t.NewPath();
                        t.Draw();
                    }
                }
            }
            else if (info.Key == ConsoleKey.F5)
            {
                DialogActive = true;
                Dialogue.Code = 2;
                Dialogue.Draw();
            }
            else if (info.Key == ConsoleKey.F6)
            {
                DialogActive = true;
                Dialogue.Code = 5;
                Dialogue.Draw();
            }
            else if (info.Key == ConsoleKey.F7)
            {
                DialogActive = true;
                Dialogue.Code = 4;
                Dialogue.Draw();
            }
            else if (info.Key == ConsoleKey.F8)
            {
                DialogActive = true;
                Dialogue.Code = 3;
                Dialogue.Draw();
            }
            else if (info.Key == ConsoleKey.F9)
            {
                if (SelectedScheme >= schemes.Count)
                    SelectedScheme = 0;
                ColorTransfer.colorScheme = schemes[SelectedScheme];
                SelectedScheme++;
                Refresh();
            }
            else if (info.Key == ConsoleKey.F10)
                Environment.Exit(0);
        }

        public void DrawMenu()
        {
            if (colorScheme != ColorTransfer.colorScheme)
            {
                CS = new ColorManager(ColorTransfer.colorScheme);
                colorScheme = ColorTransfer.colorScheme;
            }

            Console.SetCursorPosition(0, Console.WindowHeight - 1);

            int width = Console.WindowWidth;
            for (int i = 0; i < menu.Count; i++)
            {
                CS.SetColor(8, 1);
                CS.SetColor(9);
                Console.Write(" " + Convert.ToString(i + 1));
                CS.SetColor(6, 1);
                CS.SetColor(7);

                if(menu[i].Length <= width / 10)
                {
                    if (i != 9)
                        Console.Write(menu[i].PadRight(width / 10 - 2));
                    else
                        Console.Write(menu[i].PadRight(100));
                }
                else
                {
                    for (int j = 0; j < width / 10; j++)
                    {
                        if (i != 9)
                        {
                            if (menu[i].Length > width / 10)
                            {
                                menu[i] = menu[i].Substring(0, 2) + "-" + menu[i].Substring(4);
                            }
                            else
                            {
                                Console.Write(menu[i]);
                                break;
                            }
                        }
                    }
                }
            }
            Console.SetCursorPosition(0, 0);
        }

        public void Refresh()
        {
            foreach(Table t in tables)
            {
                t.NewLists();
                t.Add();
                t.Draw();
            }
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            DrawMenu();
        }
    }
}
