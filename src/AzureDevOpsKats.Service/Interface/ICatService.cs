using System.Collections.Generic;
using AzureDevOpsKats.Service.Models;

namespace AzureDevOpsKats.Service.Interface
{
    public interface ICatService
    {
        IEnumerable<CatModel> GetCats();

        CatModel GetCat(int id);

        void EditCat(int id, CatUpdateModel cat);

        void CreateCat(CatModel cat);

        void DeleteCat(int id);
    }
}
