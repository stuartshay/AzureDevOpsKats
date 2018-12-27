using System;
using System.Collections.Generic;
using AzureDevOpsKats.Data.Entities;

namespace AzureDevOpsKats.Data.Repository
{
    public interface ICatRepository : IDisposable
    {
        IEnumerable<Cat> GetCats();

        IEnumerable<Cat> GetCats(int limit, int offset);

        Cat GetCat(int id);

        void EditCat(Cat cat);

        void CreateCat(Cat cat);

        void DeleteCat(int id);
    }
}
