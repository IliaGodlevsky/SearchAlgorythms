﻿using Pathfinding.GraphLib.Core.Interface;
using Pathfinding.GraphLib.Serialization.Core.Interface;
using System.IO;

namespace Pathfinding.GraphLib.Serialization.Core.Realizations.Extensions
{
    public static class IGraphSerializerExtensions
    {
        public static void SaveGraphToFile<TGraph, TVertex>(this IGraphSerializer<TGraph, TVertex> self, IGraph<IVertex> graph, string filePath)
            where TGraph : IGraph<TVertex>
            where TVertex : IVertex
        {
            var fileMode = File.Exists(filePath) ? FileMode.Truncate : FileMode.Create;
            using (var fileStream = new FileStream(filePath, fileMode, FileAccess.Write))
            {
                self.SaveGraph(graph, fileStream);
            }
        }

        public static TGraph LoadGraphFromFile<TGraph, TVertex>(this IGraphSerializer<TGraph, TVertex> self, string filePath)
            where TGraph : IGraph<TVertex>
            where TVertex : IVertex
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return self.LoadGraph(fileStream);
            }
        }
    }
}