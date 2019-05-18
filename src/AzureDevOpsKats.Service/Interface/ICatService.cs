using System.Collections.Generic;
using AzureDevOpsKats.Service.Models;

namespace AzureDevOpsKats.Service.Interface
{
    public interface ICatService
    {
        IEnumerable<CatModel> GetCats();

        CatModel GetCat(int id);

        IEnumerable<CatModel> GetCats(int limit, int offset);

        long GetCount();

        void EditCat(int id, CatUpdateModel cat);

        void CreateCat(CatModel cat);

        void DeleteCat(int id);
    }
}
