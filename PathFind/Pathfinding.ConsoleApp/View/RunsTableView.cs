﻿using Autofac.Features.AttributeFilters;
using CommunityToolkit.Mvvm.Messaging;
using Pathfinding.ConsoleApp.Injection;
using Pathfinding.ConsoleApp.Messages.View;
using Pathfinding.ConsoleApp.Model;
using Pathfinding.ConsoleApp.ViewModel.Interface;
using Pathfinding.Shared.Extensions;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Terminal.Gui;

namespace Pathfinding.ConsoleApp.View
{
    internal sealed partial class RunsTableView : TableView
    {
        private readonly CompositeDisposable disposables = new();
        private readonly Dictionary<int, IDisposable> modelsSubs = new();

        public RunsTableView(IRunsTableViewModel viewModel,
            [KeyFilter(KeyFilters.Views)] IMessenger messenger) : this()
        {
            viewModel.Runs.CollectionChanged += OnCollectionChanged;
            this.Events().KeyPress
                .Where(x => x.KeyEvent.Key.HasFlag(Key.A)
                    && x.KeyEvent.Key.HasFlag(Key.CtrlMask))
                .Throttle(TimeSpan.FromMilliseconds(150))
                .Select(x => MultiSelectedRegions
                        .SelectMany(x => (x.Rect.Top, x.Rect.Bottom - 1).Iterate())
                        .Select(GetRunId)
                        .ToArray())
                .Do(x => messenger.Send(new OpenAlgorithmRunViewMessage()))
                .InvokeCommand(viewModel, x => x.SelectRunsCommand)
                .DisposeWith(disposables);
            this.Events().SelectedCellChanged
                .Where(x => x.NewRow > -1 && x.NewRow < Table.Rows.Count)
                .Do(x => messenger.Send(new OpenAlgorithmRunViewMessage()))
                .Select(x => GetSelectedRows())
                .InvokeCommand(viewModel, x => x.SelectRunsCommand)
                .DisposeWith(disposables);
            this.Events().MouseClick
                .Where(x => x.MouseEvent.Flags == MouseFlags.Button1Clicked)
                .Select(x => x.MouseEvent.Y + RowOffset - headerLinesConsumed)
                .Where(x => x >= 0 && x < Table.Rows.Count && x == SelectedRow)
                .Do(x => messenger.Send(new OpenAlgorithmRunViewMessage()))
                .Select(x => GetRunId(x).Enumerate().ToArray())
                .InvokeCommand(viewModel, x => x.SelectRunsCommand)
                .DisposeWith(disposables);
            this.Events().KeyPress
                .Where(args => args.KeyEvent.Key.HasFlag(Key.R)
                    && args.KeyEvent.Key.HasFlag(Key.CtrlMask)
                    && Table.Rows.Count > 1)
                .Do(x => OrderTable(IdCol, Ascending))
                .Do(x => PreviousSortedColumn = string.Empty)
                .Select(x => GetSelectedRows())
                .InvokeCommand(viewModel, x => x.SelectRunsCommand)
                .DisposeWith(disposables);
            this.Events().MouseClick
                .Where(x => x.MouseEvent.Flags == MouseFlags.Button1Clicked
                    && Table.Rows.Count > 1
                    && x.MouseEvent.Y < headerLinesConsumed)
                .Do(OrderOnMouseClick)
                .Select(x => GetSelectedRows())
                .InvokeCommand(viewModel, x => x.SelectRunsCommand)
                .DisposeWith(disposables);
        }
        
        private int[] GetSelectedRows()
        {
            return GetAllSelectedCells().Select(x => x.Y)
                .Distinct().Select(GetRunId).ToArray();
        }

        private IDisposable BindTo<T>(RunInfoModel model, string column,
            Expression<Func<RunInfoModel, T>> expression)
        {
            return model.WhenAnyValue(expression)
                .Do(x => Update(model.Id, column, x))
                .Subscribe();
        }

        private void Update<T>(int id, string column, T value)
        {
            var row = Table.Rows.Find(id);
            row[column] = value;
            Table.AcceptChanges();
            SetNeedsDisplay();
            SetCursorInvisible();
        }

        private int GetRunId(int selectedRow)
        {
            return (int)Table.Rows[selectedRow][IdCol];
        }

        private void OrderOnMouseClick(MouseEventArgs args)
        {
            var selectedColumn = ScreenToCell(args.MouseEvent.X,
                headerLinesConsumed);
            var column = Table.Columns[selectedColumn.Value.X].ColumnName;
            Order = PreviousSortedColumn != column || !Order;
            PreviousSortedColumn = column;
            string sortOrder = Order ? Ascending : Descending;
            OrderTable(column, sortOrder);
        }

        private void OrderTable(string columnName, string order)
        {
            Table.DefaultView.Sort = $"{columnName} {order}";
            Table = Table.DefaultView.ToTable();
            SetTableStyle();
            Table.AcceptChanges();
            SetNeedsDisplay();
        }

        private object ToTableValue<T>(T? value)
            where T : struct => value == null ? DBNull.Value : value.Value;

        private void OnAdded(RunInfoModel model)
        {
            Application.MainLoop.Invoke(() =>
            {
                Table.Rows.Add(model.Id, 
                    model.Algorithm, 
                    model.Visited,
                    model.Steps, 
                    model.Cost, model.Elapsed, 
                    ToTableValue(model.StepRule),
                    ToTableValue(model.Heuristics), 
                    ToTableValue(model.Weight), 
                    model.ResultStatus);
                var sub = new CompositeDisposable();
                BindTo(model, VisitedCol, x => x.Visited).DisposeWith(sub);
                BindTo(model, StepsCol, x => x.Steps).DisposeWith(sub);
                BindTo(model, ElapsedCol, x => x.Elapsed).DisposeWith(sub);
                BindTo(model, CostCol, x => x.Cost).DisposeWith(sub);
                BindTo(model, StatusCol, x => x.ResultStatus).DisposeWith(sub);
                modelsSubs.Add(model.Id, sub);
                Table.AcceptChanges();
                SetNeedsDisplay();
                SetCursorInvisible();
            });
        }

        private void OnRemoved(RunInfoModel model)
        {
            Application.MainLoop.Invoke(() =>
            {
                var row = Table.Rows.Find(model.Id);
                var index = Table.Rows.IndexOf(row);
                if (row != null)
                {
                    row.Delete();
                    modelsSubs[model.Id].Dispose();
                    modelsSubs.Remove(model.Id);
                    Table.AcceptChanges();
                    MultiSelectedRegions.Clear();
                    if (Table.Rows.Count > 0)
                    {
                        SelectedCellChangedEventArgs args = index == Table.Rows.Count
                            ? new(Table, 0, 0, index, index - 1)
                            : new(Table, 0, 0, index, index);
                        OnSelectedCellChanged(args);
                        SetSelection(0, args.NewRow, false);
                    }
                    SetNeedsDisplay();
                    SetCursorInvisible();
                }
            });
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    MultiSelectedRegions.Clear();
                    Table.Clear();
                    Table.AcceptChanges();
                    modelsSubs.Values.ForEach(x => x.Dispose());
                    modelsSubs.Clear();
                    SelectedRow = -1;
                    SetNeedsDisplay();
                    break;
                case NotifyCollectionChangedAction.Add:
                    OnAdded((RunInfoModel)e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    OnRemoved((RunInfoModel)e.OldItems[0]);
                    break;
            }
        }

        private static void SetCursorInvisible()
        {
            Application.Driver.SetCursorVisibility(CursorVisibility.Invisible);
        }
    }
}
