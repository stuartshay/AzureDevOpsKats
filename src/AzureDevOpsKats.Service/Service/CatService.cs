using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureDevOpsKats.Data.Entities;
using AzureDevOpsKats.Data.Repository;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AzureDevOpsKats.Service.Service
{
    public class CatService : ICatService
    {
        private readonly ICatRepository _catRepository;

        private readonly ILogger<CatService> _logger;

        private readonly string _requestPath;

        private readonly IMapper _mapper;

        public CatService(ICatRepository catRepository, IOptions<ApplicationOptions> settings, IMapper mapper, ILogger<CatService> logger)
        {
            this._catRepository = catRepository;
            _mapper = mapper;
            _logger = logger;
            _requestPath = settings.Value.FileStorage.RequestPath;
        }

        public async Task<IEnumerable<CatModel>> GetCats()
        {
            var cats = await _catRepository.GetCats();
            var results = _mapper.Map<IEnumerable<CatModel>>(cats);

            var catModels = results as CatModel[] ?? results.ToArray();
            foreach (var result in catModels)
            {
                result.Photo = $"{_requestPath}/{result.Photo}";
            }

            _logger.LogInformation("Total Cats:{catsCount}", catModels.Length);

            return catModels;
        }

        public async Task<CatModel> GetCat(int id)
        {
            var cat = await _catRepository.GetCat(id);
            var result = _mapper.Map<CatModel>(cat);

            return result;
        }

        public async Task EditCat(int id, CatUpdateModel cat)
        {
            var result = new Cat
            {
                Id = id,
                Name = cat.Name,
                Description = cat.Description,
            };

            await _catRepository.EditCat(result);
        }

        public async Task<long> CreateCat(CatModel cat)
        {
            var result = _mapper.Map<Cat>(cat);
            return await _catRepository.CreateCat(result);
        }

        public async Task DeleteCat(int id)
        {
            await _catRepository.DeleteCat(id);
        }

        public async Task<IEnumerable<CatModel>> GetCats(int limit, int offset)
        {
            var cats = await _catRepository.GetCats(limit, offset);
            var results = _mapper.Map<IEnumerable<CatModel>>(cats);

            var catModels = results as CatModel[] ?? results.ToArray();
            foreach (var result in catModels)
            {
                result.Photo = $"{_requestPath}/{result.Photo}";
            }

            _logger.LogInformation($"Service Total Cats:{{catsCount}}|Limit:{limit}|Offset{offset}", catModels.Length, limit, offset);

            return catModels;
        }

        public async Task<long> GetCount()
        {
            var result = await _catRepository.GetCount();
            return result;
        }
    }
}
