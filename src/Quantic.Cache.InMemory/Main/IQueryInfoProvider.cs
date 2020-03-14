namespace Quantic.Cache.InMemory
{
    public interface IQueryInfoProvider
    {
        QueryInfo GetQueryInfo(string fullName);
    }
}