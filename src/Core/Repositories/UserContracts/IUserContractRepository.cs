﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Core.Repositories.UserContracts
{
    public interface IUserContract
    {
        string Address { get; }

        BigInteger Balance { get; }

        DateTime LastCheck { get; }
    }

    public interface IUserContractRepository
    {
        Task SaveContract(string address);
        Task<IEnumerable<IUserContract>> GetUsedContracts();
        Task SetBalance(string address, BigInteger balance);
        Task DecreaseBalance(string address, BigInteger amount);
    }
}
