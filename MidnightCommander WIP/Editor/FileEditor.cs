using System;
using System.Collections.Generic;
using System.Text;
using MidnightCommander.Managers;
using MidnightCommander.Components;

namespace MidnightCommander.Editor
{
    class FileEditor : Window
    {
        ColorManager CS = new ColorManager(ColorTransfer.colorScheme);
        private string colorScheme = ColorTransfer.colorScheme;

        public string Path { get; set; }
        public List<string> originalText = new List<string>();
        public List<string> editedText = new List<string>();

        public List<string> menu = new List<string>() { "Škálovat", "Uložit", "Označ", "Nahraď", "Kopie", "Přesun", "Hledat", "Smazat", "Schéma", "Konec" };
        public Dialogue Dialogue { get; set; }
        public bool DialogActive = false;

        public Application ParentApplication { get; set; }

        public int cursorX = 0;
        public int cursorY = 0;

        public int offsetX = 0;
        public int offsetY = 0;

        public int TotalNumberOfChars;
        public int ActiveChar;

        public string AsciiCode;
        public string HexCode;

        public FileEditor(string path, Application parentApplication)
        {
            ParentApplication = parentApplication;

            Dialogue = new Dialogue(null, this);

            Console.CursorVisible = true;
            Console.Clear();
            Path = path;

            TextFileHandle f = new TextFileHandle(Path);
            f.FileLoad();

            originalText = f.data;
            editedText =new List<string>(originalText);

            CountChars();
            Draw();
        }

        public override void Draw()
        {
            if (colorScheme != ColorTransfer.colorScheme)
            {
                CS = new ColorManager(ColorTransfer.colorScheme);
                colorScheme = ColorTransfer.colorScheme;
            }

            CS.SetColor(0, 1);
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(" ".PadRight(Console.WindowWidth));
            }

            Console.SetCursorPosition(0, 0);

            DrawMenu();

            Console.SetCursorPosition(0, 1);

            CS.SetColor(0, 1);
            CS.SetColor(1);
            for (int i = offsetY; i < Console.WindowHeight - 2 + offsetY; i++)
            {
                if (i < editedText.Count)
                {
                    if (editedText[i].Length > Console.WindowWidth)
                        if (offsetX + Console.WindowWidth < editedText[i].Length)
                            Console.WriteLine(editedText[i].Substring(offsetX, Console.WindowWidth));
                        else if (offsetX > editedText[i].Length)
                            continue;
                        else
                            Console.WriteLine(editedText[i].Substring(offsetX));
                    else
                    {
                        if (editedText[i].Length > offsetX)
                            Console.WriteLine(editedText[i].Substring(offsetX));
                        else
                            Console.WriteLine();
                    }
                }
                else
                    Console.WriteLine();
            }

            Console.SetCursorPosition(cursorX - offsetX, cursorY + 1);
        }

        public void CountChars()
        {
            foreach(string item in editedText)
            {
                TotalNumberOfChars += item.Length;
            }
        }

        public void CheckChar()
        {
            AsciiCode = "";
            HexCode = "";

            if (cursorX != editedText[cursorY + offsetY].Length || editedText[cursorY + offsetY] == " ")
            {
                byte[] ascii = Encoding.ASCII.GetBytes(editedText[cursorY + offsetY].Substring(cursorX, 1));
                foreach (byte b in ascii)
                    AsciiCode += b.ToString();
            }
            else
                AsciiCode = "0010";

            if (cursorX != editedText[cursorY + offsetY].Length || editedText[cursorY + offsetY] == " ")
            {
                byte[] hex = Encoding.ASCII.GetBytes(editedText[cursorY + offsetY].Substring(cursorX, 1));
                HexCode = Convert.ToHexString(hex);
                if (HexCode.Length != 3)
                    HexCode = HexCode.PadLeft(3, '0');
            }
            else
                HexCode = "00A";
        }

        public void DrawMenu()
        {
            CheckChar();
            CS.SetColor(6, 1);
            CS.SetColor(7);
            Console.Write((
                Path.Substring(Path.LastIndexOf(@"\") + 1) +
                "   " + cursorX + " L:[ " + offsetY + "+" + cursorY +
                "   " + (cursorY + 1 + offsetY) + "/" + editedText.Count + "]" +
                "   " + "*(" + ActiveChar + "/" + TotalNumberOfChars + ")" +
                "   " + AsciiCode.PadLeft(4, '0') +
                "   " + "0x" + HexCode)
                .PadRight(Console.WindowWidth)
                );

            Console.SetCursorPosition(0, Console.WindowHeight - 1);

            for (int i = 0; i < menu.Count; i++)
            {
                CS.SetColor(8, 1);
                CS.SetColor(9);
                Console.Write(" " + Convert.ToString(i + 1));
                CS.SetColor(6, 1);
                CS.SetColor(7);

                if (menu[i].Length <= Console.WindowWidth / 10)
                {
                    if (i != 9)
                        Console.Write(menu[i].PadRight(Console.WindowWidth / 10 - 2));
                    else
                        Console.Write(menu[i].PadRight(100));
                }
                else
                {
                    for (int j = 0; j < Console.WindowWidth / 10; j++)
                    {
                        if (i != 9)
                        {
                            if (menu[i].Length > Console.WindowWidth / 10)
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

        public void Save()
        {
            TextFileHandle t = new TextFileHandle(Path);
            t.FileSave(editedText);
            originalText = new List<string>(editedText);
        }

        public bool CheckForChanges()
        {
            for (int i = 0; i < editedText.Count; i++)
            {
                if (editedText[i] != originalText[i])
                    return false;
            }
            return true;
        }

        public void SetParentWindow()
        {
            ParentApplication.ActiveWindow = ParentApplication.SavedBrowser;
            ParentApplication.SavedBrowser.Draw();
            ParentApplication.SavedBrowser.Refresh();
        }

        public override void HandleKey(ConsoleKeyInfo info)
        {
            if (!DialogActive)
            {
                if (info.Key == ConsoleKey.F1)
                {

                }
                else if (info.Key == ConsoleKey.F2 || (info.Modifiers.HasFlag(ConsoleModifiers.Control) && info.Key == ConsoleKey.S))
                {
                    DialogActive = true;
                    Dialogue.Editor_Save();
                    return;
                }
                else if (info.Key == ConsoleKey.F3)
                {

                }
                else if (info.Key == ConsoleKey.F4)
                {

                }
                else if (info.Key == ConsoleKey.F5 || (info.Modifiers.HasFlag(ConsoleModifiers.Control) && info.Key == ConsoleKey.C))
                {

                }
                else if (info.Modifiers.HasFlag(ConsoleModifiers.Control) && info.Key == ConsoleKey.X)
                {

                }
                else if (info.Key == ConsoleKey.F6 || (info.Modifiers.HasFlag(ConsoleModifiers.Control) && info.Key == ConsoleKey.V))
                {

                }
                else if (info.Key == ConsoleKey.F7 || (info.Modifiers.HasFlag(ConsoleModifiers.Control) && info.Key == ConsoleKey.F))
                {

                }
                else if (info.Key == ConsoleKey.F8 || info.Key == ConsoleKey.Delete)
                {

                }
                else if (info.Key == ConsoleKey.F9)
                {

                }
                else if (info.Key == ConsoleKey.F10 || info.Key == ConsoleKey.Escape)
                {
                if (CheckForChanges())
                {
                    SetParentWindow();
                    return;
                }
                else
                {
                    DialogActive = true;
                    Dialogue.Editor_Exit();
                    return;
                }
                }
                else if (info.Key == ConsoleKey.UpArrow)
                    UpArrowOptions();
                else if (info.Key == ConsoleKey.DownArrow)
                    DownArrowOptions();
                else if (info.Key == ConsoleKey.RightArrow)
                    RightArrowOptions();
                else if (info.Key == ConsoleKey.LeftArrow)
                    LeftArrowOptions();
                else if (info.Key == ConsoleKey.Backspace)
                    BackspaceOptions();
                else if (info.Key == ConsoleKey.Enter)
                    EnterOptions();
                else
                    CharacterAddition(info);

                Draw();
            }
            else
                Dialogue.HandleKey(info);
        }

        public void UpArrowOptions()
        {
            if (cursorY != 0)
            {
                ActiveChar -= editedText[cursorY + offsetY].Substring(0, cursorX).Length;
                cursorY--;

                if (cursorX > Console.WindowWidth)
                    offsetX = 0;

                if (cursorX > editedText[cursorY + offsetY].Length)
                    cursorX = editedText[cursorY + offsetY].Length;

                ActiveChar -= editedText[cursorY + offsetY].Substring(cursorX).Length;
            }

            if (offsetY > 0 && cursorY == 0)
                offsetY--;
        }

        public void DownArrowOptions()
        {
            ActiveChar += editedText[cursorY + offsetY].Substring(cursorX).Length;

            if (cursorX > Console.WindowWidth)
                offsetX = 0;

            if (cursorY + offsetY != editedText.Count - 1)
                if (cursorY == Console.WindowHeight - 3)
                    offsetY++;

                else if (cursorY + offsetY != editedText.Count - 1)
                    cursorY++;

            if (editedText[cursorY + offsetY].Length < cursorX)
                cursorX = editedText[cursorY + offsetY].Length;

            ActiveChar += editedText[cursorY + offsetY].Substring(0, cursorX).Length;
        }

        public void RightArrowOptions()
        {
            ActiveChar++;

            if (cursorX == editedText[cursorY + offsetY].Length)
            {
                if (cursorY + offsetY < editedText.Count - 1)
                {
                    cursorY++;
                    offsetX = 0;
                    cursorX = 0;
                }
                else
                    ActiveChar--;
            }
            else
            {
                cursorX++;
                if (cursorX - offsetX == Console.WindowWidth)
                    offsetX++;
            }
        }

        public void LeftArrowOptions()
        {
            ActiveChar--;

            if (cursorX - offsetX == 0)
            {
                if (offsetX > 0)
                {
                    offsetX--;
                    cursorX--;
                }
                else if (cursorY != 0)
                {
                    cursorY--;
                    cursorX = editedText[cursorY + offsetY].Length;

                    if (editedText[cursorY + offsetY].Length > Console.WindowWidth)
                        offsetX = cursorX - Console.WindowWidth + 1;
                    else
                        offsetX = 0;
                }
            }
            else
            {
                cursorX--;
            }
        }
        public void EnterOptions()
        {
            TotalNumberOfChars++;

            if (cursorX == editedText[cursorY + offsetY].Length)
                editedText.Insert(cursorY + offsetY + 1, "");
            else if (cursorX == 0)
                editedText.Insert(cursorY + offsetY, "");
            else
            {
                string temp = editedText[cursorY + offsetY].Substring(cursorX);
                editedText[cursorY + offsetY] = editedText[cursorY + offsetY].Remove(cursorX);
                editedText.Insert(cursorY + offsetY + 1, temp);
            }

            cursorX = 0;
            if (cursorY == Console.WindowHeight - 3)
                offsetY++;
            else
                cursorY++;
        }
        public void BackspaceOptions()
        {
            TotalNumberOfChars--;
            ActiveChar--;

            if (cursorX > 0)
            {
                editedText[cursorY] = editedText[cursorY].Remove(cursorX - 1, 1);
                cursorX--;
            }
            else
            {
                cursorX = editedText[cursorY + offsetY - 1].Length;
                editedText[cursorY + offsetY - 1] += editedText[cursorY + offsetY];
                editedText.RemoveAt(cursorY + offsetY);
                cursorY--;
            }
        }

        public void CharacterAddition(ConsoleKeyInfo info)
        {
            TotalNumberOfChars++;
            ActiveChar++;

            if (cursorX == 0)
                editedText[cursorY] = info.KeyChar + editedText[cursorY];
            else if (cursorX == editedText[cursorY].Length - 1)
                editedText[cursorY] = editedText[cursorY] + info.KeyChar;
            else
            {
                string temp1 = editedText[cursorY].Substring(0, cursorX);
                string temp2 = editedText[cursorY].Substring(cursorX);
                editedText[cursorY] = temp1 + info.KeyChar + temp2;
            }

            cursorX++;

            if (cursorX >= Console.WindowWidth)
                offsetX++;
        }
    }
}
