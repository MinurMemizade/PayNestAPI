using System.Reflection;
using AutoMapper;
using AutoMapper.Internal;
using PayNestAPI.AutoMapper.Interfaces;

namespace PayNestAPI.AutoMapper.Implementations
{
    public class CustomMapper : ICustomMapper
    {
        private static List<TypePair> typePairs = new();
        private IMapper MapperContainer;

        public TDestination Map<TDestination, TSource>(TSource source, string? ignore = null)
        {
            Config<TDestination,TSource>(5,ignore);
            return MapperContainer.Map<TSource,TDestination>(source);
        }

        public IList<TDestination> Map<TDestination, TSource>(List<TSource> source, string? ignore = null)
        {
            Config<TDestination, TSource>(5, ignore);
            return MapperContainer.Map<IList<TSource>,IList<TDestination>>(source);
        }

        public TDestination Map<TDestination, TSource>(object source, string? ignore = null)
        {
            Config<TDestination, TSource>(5, ignore);
            return MapperContainer.Map<TDestination>(source);
        }

        public IList<TDestination> Map<TDestination, TSource>(List<object> source, string? ignore = null)
        {
            Config<TDestination, TSource>(5, ignore);
            return MapperContainer.Map<IList<TDestination>>(source);
        }

        public TDestination MapOnto<TDestination, TSource>(TSource source, TDestination destination, string? ignore = null)
        {
            Config<TDestination, TSource>(5, ignore);
            return MapperContainer.Map(source, destination);
        }

        public IList<TDestination> MapOnto<TDestination, TSource>(List<TSource> source, IList<TDestination> destination, string? ignore = null)
        {
            Config<TDestination, TSource>(5, ignore);
            return MapperContainer.Map(source, destination);
        }

        public TDestination MapOnto<TDestination, TSource>(object source, TDestination destination, string? ignore = null)
        {
            Config<TDestination, TSource>(5, ignore);
            return MapperContainer.Map(source, destination);
        }

        public IList<TDestination> MapOnto<TDestination, TSource>(List<object> source, IList<TDestination> destination, string? ignore = null)
        {
            Config<TDestination, TSource>(5, ignore);
            return MapperContainer.Map(source, destination);
        }


        protected void Config<TDestination, TSource>(int depth = 5, string? ignore = null)
        {
            var typePair = new TypePair(typeof(TSource), typeof(TDestination));

            if (typePairs.Any(a => a.DestinationType == typePair.DestinationType && a.SourceType == typePair.SourceType) && ignore is null)
                return;

            typePairs.Add(typePair);

            var config = new MapperConfiguration(cfg =>
            {
                foreach (var pair in typePairs)
                {
                    var map = cfg.CreateMap(pair.SourceType, pair.DestinationType).MaxDepth(depth);

                    if (ignore is not null)
                        map.ForMember(ignore, x => x.Ignore());

                    var singleImageProperties = pair.SourceType.GetProperties()
                        .Where(p => p.PropertyType == typeof(IFormFile));

                    foreach (var prop in singleImageProperties)
                    {
                        map.ForMember(prop.Name, opt => opt.MapFrom((src, dest) =>
                        {
                            var file = (IFormFile?)prop.GetValue(src);
                            return file != null ? ConvertToUrl(file) : (string?)dest.GetType().GetProperty(prop.Name)?.GetValue(dest);
                        }));
                    }

                    var multipleImageProperties = pair.SourceType.GetProperties()
                        .Where(p => p.PropertyType == typeof(IFormFile[]));

                    foreach (var prop in multipleImageProperties)
                    {
                        map.ForMember(prop.Name, opt => opt.MapFrom((src, dest) =>
                        {
                            var files = (IFormFile[]?)prop.GetValue(src);
                            return files != null && files.Any()
                                ? files.Select(file => ConvertToUrl(file)).ToArray()
                                : (string[]?)dest.GetType().GetProperty(prop.Name)?.GetValue(dest);
                        }));
                    }

                    map.ReverseMap().ForAllMembers(opts =>
                    {
                        if (opts.DestinationMember is PropertyInfo prop)
                        {
                            if (prop.PropertyType == typeof(IFormFile) || prop.PropertyType == typeof(IFormFile[]))
                                opts.Ignore();
                        }
                    });
                }
            });

            MapperContainer = config.CreateMapper();
        }

        private string? ConvertToUrl(IFormFile? file)
        {
            if (file == null) return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            // Base URL of your API
            var baseUrl = "https://tourmanwebapi.azurewebsites.net";
            return $"{baseUrl}/uploads/{uniqueFileName}";
        }

    }
}

