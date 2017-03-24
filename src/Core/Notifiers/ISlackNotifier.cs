using System.Threading.Tasks;

namespace Core.Notifiers
{
    public interface ISlackNotifier
    {
        Task WarningAsync(string message);
        Task ErrorAsync(string message);
        Task FinanceWarningAsync(string message);
    }
}
