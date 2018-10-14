﻿using DomainModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IPairRepository
    {
        Task<IEnumerable<Pair>> FindAsync(Specification<Pair> spec);
        Task<IEnumerable<Pair>> FindAllAsync();

        Task SaveAllAsync(IEnumerable<Pair> pairs);

        Task DeleteAllAsync();
    }
}
