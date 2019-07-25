using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AzureDevOpsKats.Data.Entities;
using AzureDevOpsKats.Data.Repository;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Models;
using Microsoft.Extensions.Options;

namespace AzureDevOpsKats.Service.Service
{
    public class CatService : ICatService
    {
        private readonly ICatRepository _catRepository;

        private readonly string _requestPath;

        public CatService(ICatRepository catRepository, IOptions<ApplicationOptions> settings)
        {
            this._catRepository = catRepository;
            _requestPath = settings.Value.FileStorage.RequestPath;
        }

        public IEnumerable<CatModel> GetCats()
        {
            var cats = _catRepository.GetCats();
            var results = Mapper.Map<IEnumerable<CatModel>>(cats);

            var catModels = results as CatModel[] ?? results.ToArray();
            foreach (var result in catModels)
            {
                result.Photo = $"{_requestPath}/{result.Photo}";
            }

            return catModels;
        }

        public CatModel GetCat(int id)
        {
            var cat = _catRepository.GetCat(id);
            var result = Mapper.Map<CatModel>(cat);

            return result;
        }

        public void EditCat(int id, CatUpdateModel cat)
        {
            var result = new Cat
            {
                Id = id,
                Name = cat.Name,
                Description = cat.Description,
            };

            _catRepository.EditCat(result);
        }

        public long CreateCat(CatModel cat)
        {
            var result = Mapper.Map<Cat>(cat);
            return _catRepository.CreateCat(result);
        }

        public void DeleteCat(int id)
        {
            _catRepository.DeleteCat(id);
        }

        public IEnumerable<CatModel> GetCats(int limit, int offset)
        {
            var cats = _catRepository.GetCats(limit, offset);
            var results = Mapper.Map<IEnumerable<CatModel>>(cats);

            var catModels = results as CatModel[] ?? results.ToArray();
            foreach (var result in catModels)
            {
                result.Photo = $"{_requestPath}/{result.Photo}";
            }

            return catModels;
        }

        public long GetCount()
        {
            return _catRepository.GetCount();
        }
    }
}
