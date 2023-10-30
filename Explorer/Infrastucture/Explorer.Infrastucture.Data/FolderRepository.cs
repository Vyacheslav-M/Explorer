using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Explorer.Domain.Core;
using Explorer.Domain.Interfaces;
using Serilog;

namespace Explorer.Infrastructure.Data
{
    public class FolderRepository : IFolderRepository
    {
        private List<Folder> folders;

        // добавление в список каталога
        // name - название каталога
        // size - размер каталога
        public void Create(string name, long size)
        {
            Folder item = new Folder
            {
                Name = name,
                Size = size
            };
            folders.Add(item);
        }

        // получение списка подкаталогов из указанного каталога
        // dirName - название каталога
        // sort - параметр сортировки(по возрастанию, по убыванию)
        public List<Folder> GetList(string dirName, string sort)
        {
            folders = new List<Folder>();
            // получение списка дисков
            if (dirName == null)
            {
                DriveInfo[] drive = DriveInfo.GetDrives();

                foreach (var item in drive)
                {
                    Create(item.Name, GetSize(item.Name));
                }
            }
            // если папка существует
            if (Directory.Exists(dirName))
            {
                try
                {
                    string[] dirs = Directory.GetDirectories(dirName);

                    var toProcess = dirs.Length;
                    var resetEvent = new ManualResetEvent(false);

                    foreach (string item in dirs)
                    {
                        DirectoryInfo d = new DirectoryInfo(item);
                        ThreadPool.QueueUserWorkItem(
                        new WaitCallback(delegate (object state) {
                            Create(d.Name, GetSize(item));
                            if (Interlocked.Decrement(ref toProcess) == 0) resetEvent.Set();
                        }), null);
                    }
                    if (toProcess > 0)
                    {
                        resetEvent.WaitOne();
                    }
                }
                catch (Exception e)
                {
                    Log.Information(e.Message);
                }
            }
            Sorting(sort);
            return folders;
        }

        // получение родительского каталога
        // dirName - название каталога
        public DirectoryInfo GetParent(string dirName)
        {
            DirectoryInfo d = null;
            if (Directory.Exists(dirName))
            {
                d = new DirectoryInfo(dirName).Parent;
            }
            return d;
        }

        // получение размера каталога
        // dirName - название каталога
        public long GetSize(string dirName)
        {
            DirectoryInfo di = new DirectoryInfo(dirName);
            long size = 0;
            IEnumerable<FileInfo> d;
            try
            {
                // получение размера файлов в текущем каталоге
                d = di.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
                foreach (var item in d)
                {
                    size += item.Length;
                }
            }
            catch (Exception e)
            {
                Log.Information(e.Message);
            }
            try
            {
                string[] dirs = Directory.GetDirectories(dirName);
                // рекурсивный вызов функции для вычисления размера
                // файлов во всех подкаталогах
                foreach (string item in dirs)
                {
                    size += GetSize(item);
                }
            }
            catch (Exception e)
            {
                Log.Information(e.Message);
            }
            return size;
        }

        // сортировка списка
        // sort - параметр сортировки(по возрастанию, по убыванию)
        public List<Folder> Sorting(string sort)
        {
            folders.Sort(delegate (Folder x, Folder y)
            {
                if (x == null && y == null) return 0;
                else if (x == null) return -1;
                else if (y == null) return 1;
                else if (sort == "up") return y.Size.CompareTo(x.Size);
                else return x.Size.CompareTo(y.Size);
            });
            return folders;
        }

        // изменение размера
        public string Resize(long size)
        {
            string str;
            if (size < 1024)
            {
                str = size + " Б";
            }
            else if (size < 1024 * 1024)
            {
                str = size / 1024 + " КБ";
            }
            else if (size < 1024 * 1024 * 1024)
            {
                str = size / 1024 / 1024 + " МБ";
            }
            else
            {
                str = size / 1024 / 1024 / 1024 + " ГБ";
            }
            return str;
        }
    }
}