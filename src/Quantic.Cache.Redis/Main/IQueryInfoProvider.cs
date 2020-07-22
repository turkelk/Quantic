namespace Quantic.Cache.Redis
{
    public interface IQueryInfoProvider
    {
        QueryInfo GetQueryInfo(string fullName);
    }
}