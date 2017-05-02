using System.Threading.Tasks;
using QuantaApi.Filters;
using QuantaApi.Models;
using Core.Contracts;
using Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace QuantaApi.Controllers
{
    [Route("api/[controller]")]
    public class ClientController : Controller
    {
        private readonly IUserContractQueueService _userContractQueueService;
        private readonly IContractService _contractService;

        public ClientController(IUserContractQueueService userContractQueueService, IContractService contractService)
        {
            _userContractQueueService = userContractQueueService;
            _contractService = contractService;
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

        [HttpGet("isQuantaUser")]
        [ProducesResponseType(typeof(IsQuantaUserResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        public async Task<IActionResult> IsQuantaUser([FromQuery]string address)
        {
            var response = new IsQuantaUserResponse
            {
                IsQuantaUser = await _contractService.IsQuantaUser(address)
            };

            return Ok(response);
        }
    }
}
