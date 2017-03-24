using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Web3;

namespace QuantaApi.Controllers
{
    [Route("api/[controller]")]
    public class IsAliveController : Controller
    {
        private readonly Web3 _web3;

        public IsAliveController(Web3 web3)
        {
            _web3 = web3;
        }

        /// <summary>
        /// Check API is alive
        /// </summary>
        /// <returns>Current API version</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult Get()
        {
            var response = new IsAliveResponse()
            {
                Version = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion
            };

            return Ok(response);
        }

        /// <summary>
        /// Check Ethereum RPC is alive
        /// </summary>
        [HttpGet("rpc")]
        public async Task RpcAlive()
        {
            await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
        }

        public class IsAliveResponse
        {
            public string Version { get; set; }
            public string Error { get; set; }
        }
    }
}
