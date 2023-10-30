using System.Collections.Generic;
using System.IO;
using Explorer.Domain.Core;

namespace Explorer.Domain.Interfaces
{
    public interface IFolderRepository
    {
        List<Folder> GetList(string dirName, string sort);
        void Create(string name, long size);
        List<Folder> Sorting(string sort);
        DirectoryInfo GetParent(string dirName);
        long GetSize(string dirName);
        string Resize(long size);
    }
}
