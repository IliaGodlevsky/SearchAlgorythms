﻿using Pathfinding.App.Console.Extensions;
using Pathfinding.Domain.Core;
using System.Data;
using Terminal.Gui;

namespace Pathfinding.App.Console.View
{
    internal sealed partial class GraphsTableView : TableView
    {
        private const string IdCol = "Id";
        private const string NameCol = "Name";
        private const string WidthCol = "Width";
        private const string LengthCol = "Length";
        private const string NeighborsCol = "Neighbors";
        private const string SmoothCol = "Smooth";
        private const string ObstaclesCol = "Obstacles";
        private const string StatusCol = "Status";

        private readonly DataTable table = new();
        private readonly int headerLinesConsumed;

        public GraphsTableView()
        {
            table.Columns.AddRange(new DataColumn[]
            {
                new DataColumn(IdCol, typeof(int)),
                new DataColumn(NameCol, typeof(string)),
                new DataColumn(WidthCol, typeof(int)),
                new DataColumn(LengthCol, typeof(int)),
                new DataColumn(NeighborsCol, typeof(Neighborhoods)),
                new DataColumn(SmoothCol, typeof(SmoothLevels)),
                new DataColumn(ObstaclesCol, typeof(int)),
                new DataColumn(StatusCol, typeof(GraphStatuses)),
            });
            table.PrimaryKey = new DataColumn[] { table.Columns[IdCol] };
            var columnStyles = new Dictionary<DataColumn, ColumnStyle>()
            {
                { table.Columns[IdCol], new() { Visible = false } },
                { table.Columns[NameCol], new() { MinWidth = 24, MaxWidth = 24, Alignment = TextAlignment.Left } },
                { table.Columns[WidthCol], new() { Alignment = TextAlignment.Centered } },
                { table.Columns[LengthCol], new() { Alignment = TextAlignment.Centered } },
                { table.Columns[NeighborsCol], new() { Alignment = TextAlignment.Left,
                    RepresentationGetter = NeighborhoodToString } },
                { table.Columns[SmoothCol], new() { Alignment = TextAlignment.Left,
                    RepresentationGetter = SmoothLevelToString } },
                { table.Columns[ObstaclesCol], new() { Alignment = TextAlignment.Centered } },
                { table.Columns[StatusCol], new () { Alignment = TextAlignment.Centered,
                    RepresentationGetter = GraphStatusToString } },
            };
            Style = new TableStyle()
            {
                ExpandLastColumn = false,
                ShowVerticalCellLines = false,
                AlwaysShowHeaders = true,
                ShowVerticalHeaderLines = false,
                ColumnStyles = columnStyles
            };
            int line = 1;
            if (Style.ShowHorizontalHeaderOverline)
            {
                line++;
            }
            if (Style.ShowHorizontalHeaderUnderline)
            {
                line++;
            }
            headerLinesConsumed = line;
            MultiSelect = true;
            FullRowSelect = true;
            X = 0;
            Y = Pos.Percent(0);
            Width = Dim.Fill();
            Height = Dim.Percent(90);
            Table = table;
        }

        private static string SmoothLevelToString(object level)
        {
            var lvl = (SmoothLevels)level;
            return lvl.ToStringRepresentation();
        }

        private static string NeighborhoodToString(object neighborhood)
        {
            var n = (Neighborhoods)neighborhood;
            return n.ToStringRepresentation();
        }

        private static string GraphStatusToString(object status)
        {
            var s = (GraphStatuses)status;
            return s.ToStringRepresentation();
        }
    }
}
