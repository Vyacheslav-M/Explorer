using System.Collections.Generic;
using Explorer.Domain.Core;

namespace Explorer.Domain.Interfaces
{
    public interface IFileRepository
    {
        List<Files> GetList(string dirName, string sort);
        void Create(string name, long size);
        List<Files> Sorting(string sort);
        string Resize(long size);
    }
}
