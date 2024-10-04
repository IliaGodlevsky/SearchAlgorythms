﻿using Pathfinding.Domain.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pathfinding.Domain.Interface.Repositories
{
    public interface INeighborsRepository
    {
        Task<IEnumerable<Neighbor>> CreateAsync(IEnumerable<Neighbor> neighbours,
            CancellationToken token = default);

        Task<IReadOnlyDictionary<int, IReadOnlyCollection<Neighbor>>>
            ReadNeighboursForVerticesAsync(IEnumerable<int> verticesIds,
            CancellationToken token = default);

        Task<bool> DeleteNeighbourAsync(int vertexId,
            int neighbourId, CancellationToken token = default);

        Task<bool> DeleteNeighboursAsync(IEnumerable<(int VertexId, List<int> NeighborsIds)> neighbors,
            CancellationToken token = default);

        Task<bool> DeleteByGraphIdAsync(int graphId, CancellationToken token = default);
    }
}