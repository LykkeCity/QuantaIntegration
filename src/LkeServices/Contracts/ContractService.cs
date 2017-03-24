using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Core;
using Core.Contracts;
using Core.Settings;
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
                            _settings.EthereumMainAccount, new HexBigInteger(500000), _settings.QuantaAssetProxy.Address);

                transactionHashList.Add(transactionHash);
            }

            var contract = _web3.Eth.GetContract(_settings.QuantaAssetProxy.Abi, _settings.QuantaAssetProxy.Address);

            // wait for all <count> contracts transactions
            var contractList = new List<string>();
            for (var i = 0; i < count; i++)
            {
                try 
                {
                    // get contract transaction
                    TransactionReceipt receipt;
                    while((receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHashList[i])) == null) 
                    {
                        await Task.Delay(100);
                    }

                    // check if contract byte code is deployed
                    var code = await _web3.Eth.GetCode.SendRequestAsync(receipt.ContractAddress);

                    if(string.IsNullOrWhiteSpace(code) || code == "0x") 
                    {
                        throw new Exception("Code was not deployed correctly, verify bytecode or enough gas was to deploy the contract");
                    }

                    if(await contract.GetFunction("addUser").CallAsync<bool>(receipt.ContractAddress)) 
                    {
                        await contract.GetFunction("addUser").SendTransactionAsync(_settings.EthereumQuantaAccount, new HexBigInteger(Constants.GasForTransfer),
                            new HexBigInteger(0), receipt.ContractAddress);
                    } 
                    else 
                    {
                        throw new Exception("addUser function failed on QNTB contract");
                    }

                    contractList.Add(receipt.ContractAddress);
                }
                catch (Exception exc)
                {
                    await _logger.WriteWarningAsync("GenerateUserContracts", "GenerateUserContractPoolFunction", exc.Message, "Transaction is failed");
                }
            }

            return contractList.ToArray();
        }
    }
}
