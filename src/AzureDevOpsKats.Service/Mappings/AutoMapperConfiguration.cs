using AutoMapper;
using AzureDevOpsKats.Data.Entities;
using AzureDevOpsKats.Service.Models;

namespace AzureDevOpsKats.Service.Mappings
{
    public class AutoMapperConfiguration
    {
        private static bool _isMappinginitialized;

        public static MapperConfiguration Configure() => new MapperConfiguration(cfg =>
        {
            Mapper.Initialize(x =>
            {
                x.CreateMap<Cat, CatModel>().ReverseMap();
            });

            Mapper.AssertConfigurationIsValid();
            _isMappinginitialized = true;
        });
    }
}

