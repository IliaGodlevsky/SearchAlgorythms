﻿using GraphLibrary.Graphs.Interface;
using GraphLibrary.GraphSerialization.GraphSaver.Interface;
using System.IO;

namespace GraphLibrary.Extensions.CustomTypeExtensions
{
    public static class IGraphSaverExtensions
    {
        public static void SaveInFile(this IGraphSaver saver, IGraph graph, string path)
        {
            using (var stream = new FileStream(path, FileMode.OpenOrCreate))
                saver.SaveGraph(graph, stream);
        }
    }
}