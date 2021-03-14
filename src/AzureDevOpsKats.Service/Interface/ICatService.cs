using System.Collections.Generic;
using System.Threading.Tasks;
using AzureDevOpsKats.Service.Models;

namespace AzureDevOpsKats.Service.Interface
{
    public interface ICatService
    {
        Task<IEnumerable<CatModel>> GetCats();

        Task<CatModel> GetCat(int id);

        Task<IEnumerable<CatModel>> GetCats(int limit, int offset);

        Task<long> GetCount();

        Task EditCat(int id, CatUpdateModel cat);

        Task<long> CreateCat(CatModel cat);

        Task DeleteCat(int id);
    }
}
