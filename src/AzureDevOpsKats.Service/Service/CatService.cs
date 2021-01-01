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

        private readonly IMapper _mapper;

        public CatService(ICatRepository catRepository, IOptions<ApplicationOptions> settings, IMapper mapper)
        {
            this._catRepository = catRepository;
            _mapper = mapper;
            _requestPath = settings.Value.FileStorage.RequestPath;
        }

        public IEnumerable<CatModel> GetCats()
        {
            var cats = _catRepository.GetCats().Result;
            var results = _mapper.Map<IEnumerable<CatModel>>(cats);

            var catModels = results as CatModel[] ?? results.ToArray();
            foreach (var result in catModels)
            {
                result.Photo = $"{_requestPath}/{result.Photo}";
            }

            return catModels;
        }

        public CatModel GetCat(int id)
        {
            var cat = _catRepository.GetCat(id).Result;
            var result = _mapper.Map<CatModel>(cat);

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
            var result = _mapper.Map<Cat>(cat);
            return _catRepository.CreateCat(result).Result;
        }

        public void DeleteCat(int id)
        {
            _catRepository.DeleteCat(id);
        }

        public IEnumerable<CatModel> GetCats(int limit, int offset)
        {
            var cats = _catRepository.GetCats(limit, offset).Result;
            var results = _mapper.Map<IEnumerable<CatModel>>(cats);

            var catModels = results as CatModel[] ?? results.ToArray();
            foreach (var result in catModels)
            {
                result.Photo = $"{_requestPath}/{result.Photo}";
            }

            return catModels;
        }

        public long GetCount()
        {
            return _catRepository.GetCount().Result;
        }
    }
}
