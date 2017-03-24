using System.Threading.Tasks;

namespace Core.Ethereum
{
    public interface IPaymentService
    {
        Task<decimal> GetMainAccountBalance();
    }
}
