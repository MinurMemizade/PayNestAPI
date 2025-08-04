namespace PayNestAPI.AutoMapper.Interfaces
{
    public interface ICustomMapper
    {
        TDestination Map<TDestination,TSource>(TSource source, string? ignore=null);
        IList<TDestination> Map<TDestination,TSource>(List<TSource> source, string? ignore=null);
        TDestination Map<TDestination,TSource>(object source, string? ignore=null);
        IList<TDestination> Map<TDestination,TSource>(List<object> source,string? ignore=null);
        TDestination MapOnto<TDestination, TSource>(TSource source, TDestination destination, string? ignore = null);
        IList<TDestination> MapOnto<TDestination, TSource>(List<TSource> source, IList<TDestination> destination, string? ignore = null);
        TDestination MapOnto<TDestination, TSource>(object source, TDestination destination, string? ignore = null);
        IList<TDestination> MapOnto<TDestination, TSource>(List<object> source, IList<TDestination> destination, string? ignore = null);
    }
}
