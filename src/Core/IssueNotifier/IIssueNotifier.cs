using System.Threading.Tasks;

namespace Core.IssueNotifier
{
    public interface IIssueNotifier
    {
        Task AddNotify(string txHash, string contract, decimal amount);
    }
}
