using System;
using System.Threading.Tasks;

namespace Core.Repositories.ApiRequests
{
    public interface IApiRequestBlobRepository
    {
        Task LogToBlob(Guid id, string type, string data);
    }
}