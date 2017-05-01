using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Core.Settings;
using LkeServices.Ethereum;
using Nethereum.Contracts;
using Nethereum.Geth;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Test
{
    [TestFixture]
    public class Test
    {
        private const string TestAddr = "0xfe2b80f7aa6c3d9b4fafeb57d0c9d98005d0e4b6";

        private const int ServiceIndex = 999;
        private const string LocalAddress = "0x057DF8CCFA6b5A38f0b075fB5adc337273e5Ddf5";
        private const string AssetContractOwner = "0x3c86f3337d94B8890b35F27De8a6b4913bc87517";

        [FunctionOutput]
        private class StatusOf
        {
            [Parameter("uint256", "indexOfService", 1)]
            public BigInteger IndexOfService { get; set; }
            
            [Parameter("uint256", "isFrozen", 2)]
            public BigInteger IsFrozen { get; set; }
        }
        [Test]
        public async Task Basic()
        {
            var settings = Config.Services.GetService<BaseSettings>();
            var web3 = Config.Services.GetService<Web3>();

            var contract = web3.Eth.GetContract(settings.QuantaAssetProxy.Abi, settings.QuantaAssetProxy.Address);

            var totalSupply = await contract.GetFunction("statusOf").CallDeserializingToObjectAsync<StatusOf>(LocalAddress);

            var mainBalance = await contract.GetFunction("balanceOf").CallAsync<BigInteger>(LocalAddress);

            //await ExecuteFunction(AssetContractOwner, contract.GetFunction("issue"), "0x8674e1d56021fbc1c33b3dd7db76667905f45e54", BigInteger.Parse("50"));
        }

        [Test]
        public async Task AddService()
        {
            var settings = Config.Services.GetService<BaseSettings>();
            var web3 = Config.Services.GetService<Web3>();

            var contract = web3.Eth.GetContract(settings.QuantaAssetProxy.Abi, settings.QuantaAssetProxy.Address);

            await ExecuteFunction(AssetContractOwner, contract.GetFunction("addService"), TestAddr, 9999);
        }

        [Test]
        public async Task AddUser()
        {
            var settings = Config.Services.GetService<BaseSettings>();
            var web3 = Config.Services.GetService<Web3>();

            var contract = web3.Eth.GetContract(settings.QuantaAssetProxy.Abi, settings.QuantaAssetProxy.Address);

            await ExecuteFunction(TestAddr, contract.GetFunction("addDepositAddress"), new object[] { new[] { TestAddr } });
        }

        [Test]
        public async Task IssueMoney()
        {
            var settings = Config.Services.GetService<BaseSettings>();
            var web3 = Config.Services.GetService<Web3>();

            var contract = web3.Eth.GetContract(settings.QuantaAssetProxy.Abi, settings.QuantaAssetProxy.Address);

            await ExecuteFunction(AssetContractOwner, contract.GetFunction("issue"), settings.EthereumMainAccount, BigInteger.Parse("1000000"));
        }

        private async Task ExecuteFunction(string from, Function function, params object[] inputs)
        {
            var web3 = Config.Services.GetService<Web3>();
            var settings = Config.Services.GetService<BaseSettings>();

            const int gas = 2000000;
            var tx = await function.SendTransactionAsync(from, new HexBigInteger(gas), new HexBigInteger(0), inputs);

            TransactionReceipt receipt;
            while ((receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(tx)) == null)
            {
                await Task.Delay(100);
            }

            var transactionService = new TransactionService(new Web3Geth(settings.EthereumUrl), web3);

            if (!await transactionService.IsTransactionExecuted(tx, gas))
                throw new Exception("Transaction was not executed");
        }
    }
}
