using System.Threading.Tasks;

namespace Quantic.Log
{
    public interface IRequestLogger
    {
        Task Log(RequestLog log);
    }
}
