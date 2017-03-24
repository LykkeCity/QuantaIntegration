using System.Threading.Tasks;
using QuantaApi.Filters;
using QuantaApi.Models;
using Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace QuantaApi.Controllers
{
    [Route("api/[controller]")]
    public class ClientController : Controller
    {
        private readonly IUserContractQueueService _userContractQueueService;

        public ClientController(IUserContractQueueService userContractQueueService)
        {
            _userContractQueueService = userContractQueueService;
        }

        /// <summary>
        /// Gets new ehtereum user contract and saves it for monitoring
        /// </summary>
        /// <returns>Ethereum contract address</returns>
        [HttpGet("register")]
        [ProducesResponseType(typeof(RegisterResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        public async Task<IActionResult> Register()
        {
            var contract = await _userContractQueueService.GetContract();

            var response = new RegisterResponse
            {
                Contract = contract
            };

            return Ok(response);
        }
    }
}
