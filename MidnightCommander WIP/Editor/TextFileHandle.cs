using System;
using System.Collections.Generic;
using System.IO;

namespace MidnightCommander.Editor
{
    public class TextFileHandle
    {
        public string Path { get; set; }
        public List<string> data = new List<string>();

        public TextFileHandle(string path)
        {
            Path = path;
        }

        public void FileLoad()
        {
            data = new List<string>(File.ReadAllLines(Path));

            if (File.ReadAllLines(Path).Length == 0)
                data.Add("");
        }

        public void FileSave(List<string> dataToSave)
        {
            File.WriteAllText(Path, String.Empty);

            using (TextWriter tw = new StreamWriter(Path))
            {
                foreach (String s in dataToSave)
                    tw.WriteLine(s);
            }
        }
    }
}
