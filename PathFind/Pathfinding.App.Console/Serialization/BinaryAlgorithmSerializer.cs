﻿using Pathfinding.App.Console.DAL.Models.TransferObjects;
using Pathfinding.App.Console.Extensions;
using Pathfinding.GraphLib.Serialization.Core.Interface;
using Shared.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pathfinding.App.Console.Serialization
{
    internal sealed class BinaryAlgorithmSerializer : ISerializer<IEnumerable<AlgorithmSerializationDto>>
    {
        public IEnumerable<AlgorithmSerializationDto> DeserializeFrom(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.Default, leaveOpen: true))
            {
                return reader.ReadAlgorithm();
            }
        }

        public void SerializeTo(IEnumerable<AlgorithmSerializationDto> algorithms, Stream stream)
        {
            using (var writer = new BinaryWriter(stream, Encoding.Default, leaveOpen: true))
            {
                writer.WriteHistory(algorithms.ToReadOnly());
            }
        }
    }
}