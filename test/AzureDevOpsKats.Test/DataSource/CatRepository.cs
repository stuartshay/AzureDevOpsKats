using AzureDevOpsKats.Data.Entities;
using AzureDevOpsKats.Data.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Cat>> GetCats()
        {
            var result = await _context.Cats.OrderBy(i => i.Name).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Cat>> GetCats(int limit, int offset)
        {
            var result = await _context.Cats
                .OrderBy(i => i.Name).Skip(offset * limit).Take(limit).ToListAsync();

            return result;
        }

        public async Task<long> GetCount()
        {
            var result = await _context.Cats.CountAsync();
            return result;
        }

        public async Task<Cat> GetCat(long id)
        {
            var result = await _context.Cats.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task EditCat(Cat cat)
        {
            _context.Cats.Update(cat);
            await _context.SaveChangesAsync();
        }

        public async Task<long> CreateCat(Cat cat)
        {
            _context.Cats.Add(cat);
            await _context.SaveChangesAsync();

            return 0;
        }

        public async Task DeleteCat(long id)
        {
            var cat = await GetCat(id);

            _context.Cats.Remove(cat);
            await _context.SaveChangesAsync();
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
