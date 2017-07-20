using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Common.Log;
using Core;
using Core.Contracts;
using Core.Settings;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace LkeServices.Contracts
{
    public class ContractService : IContractService
    {
        private readonly Web3 _web3;
        private readonly BaseSettings _settings;
        private readonly ILog _logger;

        public ContractService(Web3 web3, BaseSettings settings, ILog logger)
        {
            _web3 = web3;
            _settings = settings;
            _logger = logger;
        }

        public async Task<string> CreateContract(string from, string abi, string bytecode, params object[] constructorParams)
        {
            // deploy contract
            var transactionHash = await _web3.Eth.DeployContract.SendRequestAsync(abi, bytecode, from, new HexBigInteger(3000000), constructorParams);
            TransactionReceipt receipt;
            while ((receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash)) == null)
            {
                await Task.Delay(100);
            }

            // check if contract byte code is deployed
            var code = await _web3.Eth.GetCode.SendRequestAsync(receipt.ContractAddress);

            if (string.IsNullOrWhiteSpace(code) || code == "0x")
            {
                throw new Exception("Code was not deployed correctly, verify bytecode or enough gas was to deploy the contract");
            }

            return receipt.ContractAddress;
        }

        public async Task<string[]> GenerateUserContracts(int count = 10)
        {
            var transactionHashList = new List<string>();

            // sends <count> contracts
            for (var i = 0; i < count; i++)
            {
                // deploy contract
                var transactionHash = await
                        _web3.Eth.DeployContract.SendRequestAsync(_settings.UserContract.Abi, _settings.UserContract.ByteCode,
                            _settings.EthereumMainAccount, new HexBigInteger(Constants.GasForUserContractDeploy), _settings.QuantaAssetProxy.Address);

                transactionHashList.Add(transactionHash);
            }

            var contract = _web3.Eth.GetContract(_settings.QuantaAdminProxy.Abi, _settings.QuantaAdminProxy.Address);

            // wait for all <count> contracts transactions
            var contractList = new List<string>();
            for (var i = 0; i < count; i++)
            {
                try
                {
                    // get contract transaction
                    TransactionReceipt receipt;
                    while ((receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHashList[i])) == null)
                    {
                        await Task.Delay(100);
                    }

                    // check if contract byte code is deployed
                    var code = await _web3.Eth.GetCode.SendRequestAsync(receipt.ContractAddress);

                    if (string.IsNullOrWhiteSpace(code) || code == "0x")
                    {
                        throw new Exception("Code was not deployed correctly, verify bytecode or enough gas was to deploy the contract");
                    }

                    contractList.Add(receipt.ContractAddress);
                }
                catch (Exception exc)
                {
                    await _logger.WriteWarningAsync("GenerateUserContracts", "GenerateUserContractPoolFunction", exc.Message, "Transaction is failed");
                }
            }

            await AddAddressesToQuanta(contract, contractList);

            return contractList.ToArray();
        }

        private async Task AddAddressesToQuanta(Nethereum.Contracts.Contract contract, List<string> contracts)
        {
            var function = contract.GetFunction("addAccount");

            if (await function.CallAsync<bool>(_settings.EthereumMainAccount, new HexBigInteger(Constants.GasForQuantaContractCreation),
                new HexBigInteger(0), new object[] { contracts.ToArray() }))
            {
                var tx = await function.SendTransactionAsync(_settings.EthereumMainAccount, new HexBigInteger(Constants.GasForQuantaContractCreation),
                     new HexBigInteger(0), new object[] { contracts.ToArray() });

                while (await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(tx) == null)
                {
                    await Task.Delay(100);
                }
            }
            else
            {
                throw new Exception("addUser function failed on QNTB contract");
            }
        }

        public async Task<bool> IsQuantaUser(string address)
        {
            try
            {
                var contract = _web3.Eth.GetContract(_settings.QuantaAssetProxy.Abi, _settings.QuantaAssetProxy.Address);

                //function verifyAccount(address _account) public constant returns(uint)
                //If returned service ID > 0: user’s ETH address existed in the whitelist
                //If returned service ID <= 0: user’s ETH address NOT existed in the whitelist.
                // check if user is registered in QNTL contract
                var check = await contract.GetFunction("verifyAccount").CallDeserializingToObjectAsync<BigInteger>(address);

                return check > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    [FunctionOutput]
    public class StatusOf
    {
        [Parameter("uint256", "indexOfService", 1)]
        public BigInteger IndexOfService { get; set; }

        [Parameter("uint256", "isFrozen", 2)]
        public BigInteger IsFrozen { get; set; }
    }
}
