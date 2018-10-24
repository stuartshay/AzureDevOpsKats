using System;
using System.Collections.Generic;
using AzureDevOpsKats.Data.Entities;

namespace AzureDevOpsKats.Data.Repository
{
    public interface ICatRepository : IDisposable
    {
        IEnumerable<Cat> GetCats();

        Cat GetCat(int id);

        void EditCat(Cat cat);

        void CreateCat(Cat cat);

        void DeleteCat(int id);
    }
}
