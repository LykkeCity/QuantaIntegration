using System.Threading.Tasks;

namespace Core.Notifiers
{
    public interface IEmailNotifier
    {
        Task WarningAsync(string title, string message);
    }
}
