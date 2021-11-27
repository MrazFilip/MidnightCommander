using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MidnightCommander.Components.Subcomponents;

namespace MidnightCommander.Managers
{
    class FileManager
    {
        private string PathSource;
        private string PathDest;
        private bool movedir = false;

        /* 1 - Copy
         * 2 - Delete
         * 3 - Create
         * 4 - Move
         * 5 - Rename
         */

        public FileManager(string pathSource = "", string pathDest = "")
        {
            PathSource = pathSource;
            PathDest = pathDest;
        }
    

        public void Copy()
        {
            if(Directory.Exists(PathSource))
            {
                if (!movedir)
                    PathDest = PathDest + PathSource.Remove(0, PathSource.LastIndexOf(@"\"));
                else
                    movedir = false;

                 Directory.CreateDirectory(PathDest);

                foreach (string dir in Directory.GetDirectories(PathSource, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dir.Replace(PathSource, PathDest));
                }

                foreach (string file in Directory.GetFiles(PathSource, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(file, file.Replace(PathSource, PathDest), true);
                }
            }
            else
            {
                PathDest = PathDest + PathSource.Remove(0, PathSource.LastIndexOf(@"\"));
                File.Copy(PathSource, PathDest);
            }
        }

        public void Delete()
        {
            if(Directory.Exists(PathSource))
            {
                DirectoryInfo d = new DirectoryInfo(PathSource);
                d.Delete(true);
            }
            else
            {
                FileInfo d = new FileInfo(PathSource);
                d.Delete();
            }
        }

        public void Create(bool isFile = false)
        {
            if(isFile)
                File.Create(PathSource).Close();
            else
                Directory.CreateDirectory(PathSource);   
        }

        public void Move()
        {
            if (Directory.Exists(PathSource))
            {
                if (PathSource.Substring(0, 1) != PathDest.Substring(0, 1))
                {
                    movedir = true;
                    Copy();
                    Delete();
                }
                else
                {
                    DirectoryInfo d = new DirectoryInfo(PathSource);
                    d.MoveTo(PathDest);
                }
            }
            else
            {
                FileInfo m = new FileInfo(PathSource);
                m.MoveTo(PathDest);
            }
        }

        public void Rename(bool keepExt = true)
        {
            if (Directory.Exists(PathSource))
            {
                Directory.Move(PathSource, PathDest);
            }
            else
            {
                if (PathSource.Contains(".") && keepExt)
                    PathDest += PathSource.Substring(PathSource.LastIndexOf('.'));
                File.Move(PathSource, PathDest);
            }
        }
    }
}
