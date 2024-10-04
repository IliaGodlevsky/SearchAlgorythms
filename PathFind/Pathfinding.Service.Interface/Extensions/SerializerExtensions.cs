﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Pathfinding.Service.Interface.Extensions
{
    public static class SerializerExtensions
    {
        public static async Task<byte[]> SerializeToBytesAsync<T>(this ISerializer<T> serializer,
            T item, CancellationToken token = default)
        {
            using var memory = new MemoryStream();
            await serializer.SerializeToAsync(item, memory, token)
                .ConfigureAwait(false);
            return memory.ToArray();
        }

        public static async Task<T> DeserializeFromBytes<T>(this ISerializer<T> serializer,
            byte[] item, CancellationToken token = default)
        {
            using var memory = new MemoryStream(item);
            return await serializer.DeserializeFromAsync(memory, token)
                .ConfigureAwait(false);
        }

        public static async Task SerializeToFileAsync<T>(this ISerializer<T> self,
            T value, string filePath, CancellationToken token = default)
        {
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await self.SerializeToAsync(value, fileStream, token)
                .ConfigureAwait(false);
        }

        public static async Task<T> DeserializeFromFileAsync<T>(this ISerializer<T> self,
            string filePath, CancellationToken token = default)
        {
            using var fileStream = File.OpenRead(filePath);
            return await self.DeserializeFromAsync(fileStream, token)
                .ConfigureAwait(false);
        }
    }
}