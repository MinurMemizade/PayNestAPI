using PayNestAPI.AutoMapper.Implementations;
using PayNestAPI.AutoMapper.Interfaces;

namespace TourManWebAPI.AutoMapper
{
    public static class MapperDependencyInjection
    {
        public static void AddCustomMapper(this IServiceCollection services)
        {
            services.AddSingleton<ICustomMapper, CustomMapper>();
        }
    }
}
