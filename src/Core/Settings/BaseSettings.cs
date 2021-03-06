﻿namespace Core.Settings
{
    public class BaseSettings
    {
        public Db Db { get; set; }

        public string EthereumUrl { get; set; }
        public string SignatureProviderUrl { get; set; }

        public Contract UserContract { get; set; }

        public string EthereumMainAccount { get; set; }

        public int MinContractPoolLength { get; set; } = 100;
        public int MaxContractPoolLength { get; set; } = 200;
        public int ContractsPerRequest { get; set; } = 50;

        public decimal MainAccountMinBalance { get; set; } = 1;
        
        public Contract QuantaAssetProxy { get; set; }

        public int TransactionExecutionTimeoutMinutes { get; set; } = 10;
    }

    public class Db
    {
        public string DataConnString { get; set; }
        public string LogsConnString { get; set; }
        public string SharedStorageConnString { get; set; }
        public string QuantaSrvConnString { get; set; }
    }

    public class Contract
    {
        public string Address { get; set; }
        public string Abi { get; set; }
        public string ByteCode { get; set; }
    }
}
