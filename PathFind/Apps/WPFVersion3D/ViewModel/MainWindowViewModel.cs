﻿using Algorithm.Realizations;
using Common.Extensions;
using Common.Interface;
using GraphLib.Base;
using GraphLib.Interface;
using GraphLib.Serialization.Interfaces;
using GraphViewModel;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Algorithm.Common.Exceptions;
using GraphLib.Exceptions;
using WPFVersion3D.Enums;
using WPFVersion3D.Infrastructure;
using WPFVersion3D.Infrastructure.Animators.Interface;
using WPFVersion3D.Model;
using WPFVersion3D.View;

namespace WPFVersion3D.ViewModel
{
    internal class MainWindowViewModel : MainModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string graphParametres;
        public override string GraphParametres
        {
            get => graphParametres;
            set { graphParametres = value; OnPropertyChanged(); }
        }

        private string statistics;
        public override string PathFindingStatistics
        {
            get => statistics;
            set { statistics = value; OnPropertyChanged(); }
        }

        private IGraphField graphField;
        public override IGraphField GraphField
        {
            get => graphField;
            set { graphField = value; OnPropertyChanged(); }
        }

        public ICommand StartPathFindCommand { get; }
        public ICommand CreateNewGraphCommand { get; }
        public ICommand ClearGraphCommand { get; }
        public ICommand SaveGraphCommand { get; }
        public ICommand LoadGraphCommand { get; }
        public ICommand ChangeOpacityCommand { get; }
        public ICommand AnimatedAxisRotateCommand { get; }

        public MainWindowViewModel(BaseGraphFieldFactory fieldFactory,
            IVertexEventHolder eventHolder,
            IGraphSerializer graphSerializer,
            IGraphAssembler graphFactory,
            IPathInput pathInput) : base(fieldFactory, eventHolder, graphSerializer, graphFactory, pathInput)
        {
            StartPathFindCommand = new RelayCommand(ExecuteStartPathFindCommand, CanExecuteStartFindPathCommand);
            CreateNewGraphCommand = new RelayCommand(ExecuteCreateNewGraphCommand);
            ClearGraphCommand = new RelayCommand(ExecuteClearGraphCommand, CanExecuteGraphOperation);
            SaveGraphCommand = new RelayCommand(ExecuteSaveGraphCommand, CanExecuteGraphOperation);
            LoadGraphCommand = new RelayCommand(ExecuteLoadGraphCommand);
            ChangeOpacityCommand = new RelayCommand(ExecuteChangeOpacity, CanExecuteGraphOperation);
            AnimatedAxisRotateCommand = new RelayCommand(ExecuteAnimatedAxisRotateCommand);
        }

        public override void FindPath()
        {
            try
            {
                string loadPath = GetAlgorithmsLoadPath();
                AlgorithmsFactory.LoadAlgorithms(loadPath);
                var viewModel = new PathFindingViewModel(this);
                var listener = new PluginsWatcher(viewModel);
                var window = new PathFindWindow();
                window.Closing += listener.StopWatching;
                viewModel.OnExceptionCaught += OnExceptionCaught;
                viewModel.EndPoints = EndPoints;
                listener.FolderPath = loadPath;
                listener.StartWatching();
                PrepareWindow(viewModel, window);
            }
            catch (NoAlgorithmsLoadedException ex)
            {
                OnNotFatalExceptionCaught(ex);
            }
            catch (WrongGraphTypeException ex)
            {
                OnErrorExceptionCaught(ex);
            }
            catch (AlgorithmInterruptedException ex)
            {
                OnNotFatalExceptionCaught(ex);
            }
            catch (Exception ex)
            {
                OnNotFatalExceptionCaught(ex);
            }
        }

        public override void CreateNewGraph()
        {
            try
            {
                var model = new GraphCreatingViewModel(this, graphAssembler);
                var window = new GraphCreateWindow();
                model.OnExceptionCaught += OnExceptionCaught;
                PrepareWindow(model, window);
            }
            catch (Exception ex)
            {
                OnNotFatalExceptionCaught(ex);
            }
        }

        public void StretchAlongXAxis(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (GraphField as GraphField3D)?.StretchAlongAxis(Axis.Abscissa, e.NewValue, 1, 0, 0);
        }

        public void StretchAlongYAxis(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (GraphField as GraphField3D)?.StretchAlongAxis(Axis.Ordinate, e.NewValue, 0, 1, 0);
        }

        public void StretchAlongZAxis(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (GraphField as GraphField3D)?.StretchAlongAxis(Axis.Applicate, e.NewValue, 0, 0, 1);
        }

        private void ChangeVerticesOpacity()
        {
            PrepareWindow(new OpacityChangeViewModel(), new OpacityChangeWindow());
        }

        private void ExecuteSaveGraphCommand(object param)
        {
            base.SaveGraph();
        }

        private void ExecuteChangeOpacity(object param)
        {
            ChangeVerticesOpacity();
        }

        private bool CanExecuteStartFindPathCommand(object param)
        {
            return EndPoints.HasEndPointsSet;
        }

        private void ExecuteLoadGraphCommand(object param)
        {
            base.LoadGraph();
        }

        private void ExecuteClearGraphCommand(object param)
        {
            base.ClearGraph();
        }

        private void ExecuteStartPathFindCommand(object param)
        {
            FindPath();
        }

        private void ExecuteCreateNewGraphCommand(object param)
        {
            CreateNewGraph();
        }

        private void ExecuteAnimatedAxisRotateCommand(object param)
        {
            var rotator = (IAnimator)param;
            rotator.ApplyAnimation();
        }

        private void PrepareWindow(IViewModel model, Window window)
        {
            model.OnWindowClosed += (sender, args) => window.Close();
            window.DataContext = model;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Show();
        }

        protected override void OnExceptionCaught(string message)
        {
            MessageBox.Show(message);
        }

        protected override void OnExceptionCaught(Exception ex, string additaionalMessage = "")
        {
            MessageBox.Show(ex.Message, additaionalMessage);
        }

        private bool CanExecuteGraphOperation(object param)
        {
            return !Graph.IsDefault();
        }

        protected override string GetAlgorithmsLoadPath()
        {
            return ConfigurationManager.AppSettings["pluginsPath"];
        }
    }
}
