﻿using System;
using System.Numerics;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Core;
using Core.Contracts;
using Core.Repositories.Cashout;
using Core.Settings;
using Core.TransactionMonitoring;
using Lykke.JobTriggers.Triggers.Attributes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;

namespace QuantaJob.Functions
{
    public class CashoutFunction
    {
        private readonly ICashoutRepository _cashoutRepository;
        private readonly ILog _logger;
        private readonly Web3 _web3;
        private readonly BaseSettings _settings;
        private readonly ITransactionMonitoringQueueWriter _transactionMonitoringQueueWriter;
        private readonly IContractService _contractService;

        public CashoutFunction(ICashoutRepository cashoutRepository, ILog logger, Web3 web3, BaseSettings settings, ITransactionMonitoringQueueWriter transactionMonitoringQueueWriter, IContractService contractService)
        {
            _cashoutRepository = cashoutRepository;
            _logger = logger;
            _web3 = web3;
            _settings = settings;
            _transactionMonitoringQueueWriter = transactionMonitoringQueueWriter;
            _contractService = contractService;
        }

        [QueueTrigger(Constants.CashoutQueue, notify: true, connection: "cashout")]
        public async Task Process(CashoutModel model)
        {
            if (await _cashoutRepository.GetCashout(model.Id) != null)
            {
                await _logger.WriteWarningAsync("CashoutFunction", "Process", "Cashout already exists", model.ToJson());
                throw new Exception($"Entity already exists, {model.Id}");
            }

            await _cashoutRepository.CreateCashout(model.Id, model.Address, model.Amount);

            await _logger.WriteInfoAsync("CashoutFunction", "Process", model.ToJson(), $"Begin cashout [{model.Id}]");

            var contract = _web3.Eth.GetContract(_settings.QuantaAssetProxy.Abi, _settings.QuantaAssetProxy.Address);

            var amount = model.Amount.ToBlockchainAmount(Constants.QuantaCoinDecimals);

            if (!await _contractService.IsQuantaUser(model.Address))
                throw new Exception($"User is not registered in quanta {model.Address}, {model.Id}");

            var tx = await contract.GetFunction("transfer").SendTransactionAsync(_settings.EthereumMainAccount, new HexBigInteger(Constants.GasForTransfer),
                                       new HexBigInteger(0), model.Address, amount);

            await _transactionMonitoringQueueWriter.AddCashoutToMonitoring(tx, model.Address);

            await _logger.WriteInfoAsync("CashoutFunction", "Process", "", $"End cashout [{model.Id}]");
        }
    }

    public class CashoutModel
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public decimal Amount { get; set; }
    }
}
