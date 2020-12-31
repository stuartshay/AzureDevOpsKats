using System;
using System.Collections.Generic;
using System.Linq;
using AzureDevOpsKats.Data.Entities;
using AzureDevOpsKats.Data.Repository;

namespace AzureDevOpsKats.Test.DataSource
{
    /// <summary>
    /// 
    /// </summary>
    public class CatRepository : ICatRepository
    {
        private AzureDevOpsKatsContext _context;

        public CatRepository(AzureDevOpsKatsContext context)
        {
            _context = context;
        }

        public CatRepository()
        {
        }

        public IEnumerable<Cat> GetCats()
        {
            var result = _context.Cats.OrderBy(i => i.Name).ToList();
            return result;
        }

        public IEnumerable<Cat> GetCats(int limit, int offset)
        {
            var result = _context.Cats
                .OrderBy(i => i.Name).Skip(offset * limit).Take(limit).ToList();

            return result;
        }

        public long GetCount()
        {
            var result = _context.Cats.Count();
            return result;
        }

        public Cat GetCat(long id)
        {
            var result = _context.Cats.FirstOrDefault(x => x.Id == id);
            return result;
        }

        public void EditCat(Cat cat)
        {
            _context.Cats.Update(cat);
            _context.SaveChanges();
        }

        public long CreateCat(Cat cat)
        {
            _context.Cats.Add(cat);
            _context.SaveChanges();

            return 0;
        }

        public void DeleteCat(long id)
        {
            var cat = GetCat(id);

            _context.Cats.Remove(cat);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _context != null)
            {
                _context.Dispose();
                _context = null;
            }
        }
    }
}
