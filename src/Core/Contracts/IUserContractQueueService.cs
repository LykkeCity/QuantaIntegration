using System.Threading.Tasks;

namespace Core.Contracts
{
    public interface IUserContractQueueService
    {
        Task<string> GetContract();
        Task PushContract(string contract);
        Task<int> Count();
        Task<string> GetContractRaw();
    }
}
