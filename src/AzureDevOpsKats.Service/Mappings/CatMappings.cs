using AutoMapper;
using AzureDevOpsKats.Data.Entities;
using AzureDevOpsKats.Service.Models;

namespace AzureDevOpsKats.Service.Mappings
{
    public class CatMappings : Profile
    {
        public CatMappings()
            : base("CatMappings")
        {
            CreateMap<Cat, CatModel>();
            CreateMap<CatModel, Cat>();
        }
    }
}
