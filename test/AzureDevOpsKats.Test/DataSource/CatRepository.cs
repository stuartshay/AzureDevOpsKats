using System;
using System.Collections.Generic;
using AzureDevOpsKats.Data.Entities;
using AzureDevOpsKats.Data.Repository;

namespace AzureDevOpsKats.Test.DataSource
{
    /// <summary>
    /// 
    /// </summary>
    public class CatRepository : ICatRepository
    {
        private readonly AzureDevOpsKatsContext _context;

        public CatRepository(AzureDevOpsKatsContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Cat> GetCats()
        {
            throw new NotImplementedException();
        }

        public Cat GetCat(int id)
        {
            throw new NotImplementedException();
        }

        public void EditCat(Cat cat)
        {
            throw new NotImplementedException();
        }

        public void CreateCat(Cat cat)
        {
            throw new NotImplementedException();
        }

        public void DeleteCat(int id)
        {
            throw new NotImplementedException();
        }
    }
}
