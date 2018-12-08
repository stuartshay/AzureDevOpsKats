using AutoMapper;
using AzureDevOpsKats.Data.Entities;
using AzureDevOpsKats.Service.Models;

namespace AzureDevOpsKats.Service.Mappings
{
    public static class AutoMapperConfiguration
    {
        private static bool IsMappinginitialized { get; set; }

        private static object Lock { get; } = new object();

        public static MapperConfiguration Configure() => new MapperConfiguration(cfg =>
        {
            lock (Lock)
            {
                if (!IsMappinginitialized)
                {
                    Mapper.Initialize(x =>
                    {
                        x.CreateMap<Cat, CatModel>().ReverseMap();
                    });

                    Mapper.AssertConfigurationIsValid();
                }

                IsMappinginitialized = true;
            }
        });
    }
}
