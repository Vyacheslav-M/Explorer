using System;
using System.Collections.Generic;
using System.IO;
using Explorer.Domain.Core;
using Explorer.Domain.Interfaces;
using Serilog;

namespace Explorer.Infrastructure.Data
{
    public class FileRepository : IFileRepository
    {
        private List<Files> files;

        // добавление в список файла
        // name - название файла
        // size - размер файла
        public void Create(string name, long size)
        {
            Files item = new Files
            {
                Name = name,
                Size = size
            };
            files.Add(item);
        }

        // получение списка файлов из указанного каталога
        // dirName - название каталога
        // sort - параметр сортировки(по возрастанию, по убыванию)
        public List<Files> GetList(string dirName, string sort)
        {
            files = new List<Files>();
            // если папка существует
            if (Directory.Exists(dirName))
            {
                try
                {
                    string[] file = Directory.GetFiles(dirName);
                    foreach (string item in file)
                    {
                        FileInfo fi = new FileInfo(item);
                        long size;
                        string name;
                        name = fi.Name;
                        size = fi.Length;
                        Create(name, size);
                    }
                }
                catch (Exception e)
                {
                    Log.Information(e.Message);
                }
            }
            Sorting(sort);
            return files;
        }

        // сортировка списка
        // sort - параметр сортировки(по возрастанию, по убыванию)
        public List<Files> Sorting(string sort)
        {
            files.Sort(delegate (Files x, Files y)
            {
                if (x == null && y == null) return 0;
                else if (x == null) return -1;
                else if (y == null) return 1;
                else if (sort == "up") return y.Size.CompareTo(x.Size);
                else return x.Size.CompareTo(y.Size);
            });
            return files;
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
