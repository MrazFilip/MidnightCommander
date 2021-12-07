using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using MidnightCommander.Managers;

namespace MidnightCommander.Components
{
    class Table : IComponent
    {
        ColorManager CS = new ColorManager(ColorTransfer.colorScheme);
        private string colorScheme = ColorTransfer.colorScheme;

        private List<string> data = new List<string>();
        private List<string> dataSelect = new List<string>();

        public string selectedFile;
        public string LastPath = "";
        public string Path = @"C:\";
        private bool DiskMounted = true;

        public bool ERROR = false;

        public int Offset = 1;
        public int selected = 2;
        public int top = 0;

        public int WidthStart;
        public int Height = Console.WindowHeight;
        public int Width = Console.WindowWidth / 2 - 2;     // -2 protože dvě pipy
        public bool IsActive { get; set; }
        public string Colors { get; set; }

        public Table(int widthStart)
        {
            this.IsActive = false;
            this.WidthStart = widthStart;
            Add();
        }

        public void HandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.UpArrow && selected > 2)
            {
                if (selected == top + 1)
                {
                    top -= Height / 3;
                    Offset -= Height / 3;
                }
                selected--;
            }
            else if (info.Key == ConsoleKey.DownArrow && selected < data.Count - 1)
            {
                if (selected == Height - 6 + Offset)
                {
                    top += Height / 3;
                    Offset += Height / 3;
                }
                selected++;
            }
            else if (info.Key == ConsoleKey.Enter)
            {
                NewPath();
            }
            else if (info.Key == ConsoleKey.Backspace)
            {
                selected = 2;
                NewPath();
            }
        }

        public void Draw()
        {
            if (colorScheme != ColorTransfer.colorScheme)
            {
                CS = new ColorManager(ColorTransfer.colorScheme);
                colorScheme = ColorTransfer.colorScheme;
            }

            try //try protože může spadnou když se okno zmenší na menší než 60 a nestihne se znovu reloadovat
            {
                Console.SetCursorPosition(WidthStart, 0);

                if (DiskMounted == true)
                {
                    DrawLineTop(data[0]);
                    DrawData();
                    DrawInfo(dataSelect[selected]);
                    DrawLineBot();
                    if(IsActive == true)
                        Select();
                }
                else
                {
                    DrawDisks();
                    if (IsActive == true)
                        Select();
                }
            }
            catch { }

            Console.ResetColor();
        }

        private void DrawLineTop(string dir)
        {
            CS.SetColor(0, 1);
            CS.SetColor(1);
            Console.Write("┌─<─");
            if (IsActive)
            {
                CS.SetColor(3);
                CS.SetColor(2, 1);
            }
            else
                CS.SetColor(0, 1);

            if (dir.Length < Width - 10)
                Console.Write(" " + dir + " ");
            else
                Console.Write(" ..." + dir.Remove(0, dir.Length - Width + 11) + " ");

            Console.ResetColor();
            CS.SetColor(0, 1);
            CS.SetColor(1);
            for (int i = 0; i < Width - 8 - dir.Length; i++)
            {
                Console.Write('─');
            }
            Console.Write("─>─┐");
            Console.WriteLine();
        }

        private void DrawLineBot()
        {
            DriveInfo drive = new(data[0].Substring(0, 3));
            double total = Convert.ToInt64(drive.TotalSize) / 1073741824;
            double free = Convert.ToInt64(drive.AvailableFreeSpace) / 1073741824;
            double percent = (free / total) * 100;
            string driveInfo = " " + free + "G/" + total + "G(" + Convert.ToString(percent).Substring(0, 2) + "%) ";
            Console.SetCursorPosition(WidthStart, Height - 2);
            Console.Write('└');
            for (int i = 0; i < Width; i++)
            {
                Console.Write('─');
            }
            Console.SetCursorPosition(WidthStart + Width - driveInfo.Length, Height - 2);
            CS.SetColor(10);
            Console.Write(driveInfo);
            CS.SetColor(1);
            Console.Write("─┘");
        }

        private void DrawInfo(string file)
        {
            Console.SetCursorPosition(WidthStart, Height - 4);
            Console.Write('├');
            for (int i = 0; i < Width; i++)
            {
                Console.Write('─');
            }
            Console.Write('┤');

            Console.SetCursorPosition(WidthStart, Height - 3);
            Console.Write('│');
            CS.SetColor(10);
            if(file.Length >= Width + 1)
            {
                Console.Write(file.Substring(0, Width - 2) + "..");
            }
            else
                Console.Write(file.PadRight(Width));
            CS.SetColor(1);
            Console.Write('│');
        }

        private void DrawData()
        {
            for (int i = Offset; i < Height - 5 + Offset; i++)
            {
                Console.SetCursorPosition(WidthStart, Console.CursorTop);
                CS.SetColor(0, 1);
                CS.SetColor(1);

                Console.Write("│");
                if (i > data.Count - 1)
                {
                    Console.Write("".PadRight(Width - 21) + '│' + "".PadLeft(7) + '│' + "".PadLeft(12));
                }
                else
                    Type(data[i], i);

                Console.Write("│");
                Console.WriteLine();
            }
        }

        private void DrawDisks()
        {
            DrawLineTop(data[0]);
            Console.SetCursorPosition(WidthStart, 1);
            for (int i = 1; i < Height - 2; i++)
            {
                Console.Write('│');
                if ((i > data.Count - 1) || (i == 1))
                {
                    Console.Write("".PadRight(Width));
                }
                else
                    Console.Write(data[i].PadRight(Width));
                Console.Write('│');
                Console.SetCursorPosition(WidthStart, Console.CursorTop + 1);
            }

            Console.SetCursorPosition(WidthStart, Height - 2);
            Console.Write('└');
            for (int i = 1; i < Width; i++)
            {
                Console.Write('─');
            }
            Console.Write("─┘");
        }

        private void Select()
        {
            for (int i = Offset; i < Height - 5 + Offset; i++)
            {
                Console.SetCursorPosition(WidthStart + 1, selected - Offset + 1);
                CS.SetColor(5);
                CS.SetColor(4, 1);
                Console.Write(data[selected]);
                if(!DiskMounted)
                    selectedFile = dataSelect[selected - 2];
                else
                    selectedFile = dataSelect[selected];
            }
        }

        public void Add()
        {
            string temp;

            /* 0 - Entire path
             * 1 - Name, size, editdate tags
             * 2 - Go to higher dir
             * 3-X - dir and file data
             */

            //Debug.WriteLine(Path);

            DirectoryInfo readerDir = new(Path);

            dataSelect.Add("");
            dataSelect.Add("");
            dataSelect.Add("VYŠ-ADR");

            try    //try aby chytil přístup do složky kam nemá přístup
            {
                data.Add(readerDir.FullName);
                data.Add("".PadRight((Width - 21) / 2 - 2) + "Název".PadRight((Width - 20) / 2 + 2) + '│' + "Velikos".PadLeft(7) + '│' + "Okamžik úpra".PadLeft(12));
                data.Add("/..".PadRight(Width - 21) + '│' + "VYŠ-ADR".PadLeft(7) + '│' + DateTime.Now.ToString("d.MMM HH:mm").PadLeft(12));

                foreach (DirectoryInfo item in readerDir.GetDirectories())
                {
                    dataSelect.Add(item.Name);

                    if (item.Name.Length > Width - 22)
                        data.Add("/" + item.Name.Substring(0, Width - 24) + ".." + '│' + Convert.ToString(4096).PadLeft(7) + '│' + item.LastWriteTime.ToString("d.MMM HH:mm").PadLeft(12));
                    else
                        data.Add("/" + item.Name.PadRight(Width - 22) + '│' + Convert.ToString(4096).PadLeft(7) + '│' + item.LastWriteTime.ToString("d.MMM HH:mm").PadLeft(12));
                }

                foreach (FileInfo item in readerDir.GetFiles())
                {
                    dataSelect.Add(item.Name);

                    if (item.Name.EndsWith(".exe"))
                    {
                        if (item.Name.Length > Width - 22)
                            temp = "*" + item.Name.Substring(0, Width - 24) + "..";
                        else
                            temp = "*" + item.Name.PadRight(Width - 22);
                        data.Add(temp + '│' + Convert.ToString(item.Length / 100000).PadLeft(7) + '│' + item.LastWriteTime.ToString("d.MMM HH:mm").PadLeft(12));
                    }
                    else if (item.Name.Length > Width - 22)
                    {
                        data.Add(" " + item.Name.Substring(0, Width - 24) + ".." + '│' + Convert.ToString(item.Length / 100000).PadLeft(7) + '│' + item.LastWriteTime.ToString("d.MMM HH:mm").PadLeft(12));
                    }
                    else
                        data.Add(" " + item.Name.PadRight(Width - 22) + '│' + Convert.ToString(item.Length / 100000).PadLeft(7) + '│' + item.LastWriteTime.ToString("d.MMM HH:mm").PadLeft(12));
                }
            } 
            catch 
            { 
               ERROR = true;
            }
        }

        private void AddDisks()
        {
            /* 0 - Select Drive tag
             * 1 - Empty Space
             * 2-X - Disks
             */

            data.Add("Select Drive");
            data.Add("");

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                data.Add(Convert.ToString(drive));
                dataSelect.Add(Convert.ToString(drive));
            }
            this.DiskMounted = false;
        }

        public void NewPath()
        {
            if (selected == 2 && Path.Length > 3)
            {
                Path = Convert.ToString(new DirectoryInfo(Path).Parent);
                NewLists();
                Add();
                NewPathParams();
            }
            else if (DiskMounted == false)
            {
                Path = data[selected];
                NewLists();
                Add();
                DiskMounted = true;
                NewPathParams();
            }
            else if (selected != 2)
            {
                if (data[selected].Substring(0, 1) == "/")
                    Path = new DirectoryInfo(Path).GetDirectories()[selected - 3].FullName;
                NewLists();
                Add();
                NewPathParams();
            }
            else if (selected == 2 && Path.Length == 3)
            {
                NewLists();
                AddDisks();
            }    
        }

        public void NewPathParams()
        {
            this.top = 1;
            this.selected = 2;
            this.Offset = 1;
        }

        public void NewLists()
        {
            this.data = new List<string>();
            this.dataSelect = new List<string>();
        }

        private void Type(string item, int index)
        {
            bool hasExt = false; 

            if(item == data[1])
            {
                SpecialDraw(CS.colors[10], item);
                hasExt = true;
            }

            if (dataSelect[index].Contains('.'))
            {
                if (dataSelect[index].Substring(dataSelect[index].LastIndexOf('.')) == ".exe")
                {
                    SpecialDraw(ConsoleColor.Green, item);
                    hasExt = true;
                }
                else if (dataSelect[index].Substring(dataSelect[index].LastIndexOf('.')) == ".txt")
                {
                    SpecialDraw(ConsoleColor.DarkGray, item);
                    hasExt = true;
                }
                if (!hasExt)
                {
                    foreach (string s in new string[] { ".gif", ".jpg", ".jpeg", ".pdf", ".png" })
                    {
                        if (dataSelect[index].Substring(dataSelect[index].LastIndexOf('.')) == s)
                        {
                            SpecialDraw(ConsoleColor.Red, item);
                            hasExt = true;
                        }
                    }
                    foreach (string s in new string[] { ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".csv" })
                    {
                        if (dataSelect[index].Substring(dataSelect[index].LastIndexOf('.')) == s)
                        {
                            SpecialDraw(ConsoleColor.Cyan, item);
                            hasExt = true;
                            break;
                        }
                    }
                    foreach (string s in new string[] { ".psd", ".aep" })
                    {
                        if (dataSelect[index].Substring(dataSelect[index].LastIndexOf('.')) == s)
                        {
                            SpecialDraw(ConsoleColor.Magenta, item);
                            hasExt = true;
                        }
                    }
                }
            }
            
            if(hasExt == false)
                Console.Write(item);
        }

        private void SpecialDraw(ConsoleColor color, string item)
        {
            string[] arr = item.Split('│');

            Console.ForegroundColor = color;
            Console.Write(arr[0]);
            CS.SetColor(1);
            Console.Write('│');
            Console.ForegroundColor = color;
            Console.Write(arr[1]);
            CS.SetColor(1);
            Console.Write('│');
            Console.ForegroundColor = color;
            Console.Write(arr[2]);
            CS.SetColor(1);
        }
    }
}
