﻿using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Core.Ethereum
{
    public interface ITransactionService
    {
        Task<bool> IsTransactionExecuted(string hash, int gasSended);
        Task<TransactionReceipt> GetTransactionReceipt(string transaction);
        Task<bool> WaitForExecution(string hash, int gasSended);
    }
}
