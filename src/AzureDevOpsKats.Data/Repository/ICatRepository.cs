using AzureDevOpsKats.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureDevOpsKats.Data.Repository
{
    public interface ICatRepository : IDisposable
    {
        Task<IEnumerable<Cat>> GetCats();

        Task<IEnumerable<Cat>> GetCats(int limit, int offset);

        Task<long> GetCount();

        Task<Cat> GetCat(long id);

        Task EditCat(Cat cat);

        Task<long> CreateCat(Cat cat);

        Task DeleteCat(long id);
    }
}
