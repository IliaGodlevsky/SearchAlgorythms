﻿using LiteDB;
using Pathfinding.Domain.Interface;
using Pathfinding.Domain.Interface.Repositories;
using Pathfinding.Infrastructure.Data.LiteDb.Repositories;

namespace Pathfinding.Infrastructure.Data.LiteDb
{
    internal sealed class LiteDbUnitOfWork : IUnitOfWork
    {
        private readonly ILiteDatabase database;

        public IGraphParametresRepository GraphRepository { get; }

        public IVerticesRepository VerticesRepository { get; }

        public IRangeRepository RangeRepository { get; }

        public IStatisticsRepository StatisticsRepository { get; }

        public LiteDbUnitOfWork(string connectionString)
            : this(new ConnectionString(connectionString))
        {
        }

        public LiteDbUnitOfWork(ConnectionString connectionString)
            : this(new LiteDatabase(connectionString))
        {
        }

        public LiteDbUnitOfWork(Stream stream)
            : this(new LiteDatabase(stream))
        {
        }

        public LiteDbUnitOfWork(ILiteDatabase db)
        {
            database = db;
            GraphRepository = new LiteDbGraphRepository(database);
            VerticesRepository = new LiteDbVerticesRepository(database);
            RangeRepository = new LiteDbRangeRepository(database);
            StatisticsRepository = new LiteDbStatisticsRepository(database);
        }

        public void BeginTransaction()
        {
            database.BeginTrans();
        }

        public void Commit()
        {
            database.Commit();
        }

        public void Rollback()
        {
            database.Rollback();
        }

        public void Dispose()
        {
            database.Dispose();
        }

        public async Task RollbackAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            database.Rollback();
            await Task.CompletedTask;
        }

        public async Task CommitAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            database.Commit();
            await Task.CompletedTask;
        }
    }
}
