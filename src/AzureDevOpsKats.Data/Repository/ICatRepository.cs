using System;
using System.Collections.Generic;
using AzureDevOpsKats.Data.Entities;

namespace AzureDevOpsKats.Data.Repository
{
    public interface ICatRepository : IDisposable
    {
        IEnumerable<Cat> GetCats();

        IEnumerable<Cat> GetCats(int limit, int offset);

        long GetCount();

        Cat GetCat(long id);

        void EditCat(Cat cat);

        long CreateCat(Cat cat);

        void DeleteCat(long id);
    }
}
