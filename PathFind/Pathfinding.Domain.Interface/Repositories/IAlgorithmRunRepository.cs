﻿using Pathfinding.Domain.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pathfinding.Domain.Interface.Repositories
{
    public interface IAlgorithmRunRepository
    {
        Task<AlgorithmRun> CreateAsync(AlgorithmRun entity,
            CancellationToken token = default);

        Task<IEnumerable<AlgorithmRun>> ReadByGraphIdAsync(int graphId,
            CancellationToken token = default);

        Task<bool> DeleteByGraphIdAsync(int graphId,
            CancellationToken token = default);

        Task<bool> DeleteByRunIdsAsync(IEnumerable<int> runIds,
            CancellationToken token = default);

        Task<int> ReadCount(int graphId,
            CancellationToken token = default);
    }
}