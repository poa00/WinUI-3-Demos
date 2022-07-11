using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

using VisualSortingItems.SortingAlgorithm;

namespace VisualSortingItems
{
    public sealed partial class MainWindow : Window
    {
        private List<ISortStrategy> _algorithmCollection;
        public List<ISortStrategy> AlgorithmCollection { get => _algorithmCollection;  }

        const int Size = 100;
        const int PaddingElement = 2;
        const int ElementSize = 4;
        int _speed = 50;

        IList<int> _collection;
        IList<int> _exceptCollection;
        ISortStrategy _sorter;
        CancellationTokenSource _cancellationTokenSource;


        public MainWindow()
        {
            InitializeCollections();
            InitializeComponent();

            this.SetSize(860, 640);

            // Custom Title Bar
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(myTitleBar);
        }

        private void InitializeCollections()
        {
            _algorithmCollection = new List<ISortStrategy>
                                    {
                                          new BubbleSort(),
                                          new QuickSort(),
                                          new SelectionSort(),
                                          new MergeSort(),
                                          new InsertionSort()
                                      };

            _collection = new List<int>();
            _exceptCollection = new List<int>();
            for (int i = 0; i < Size; i++)
            {
                _exceptCollection.Add(i);
            }
        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            if (AlgorithmComboBox.SelectedItem == null) return;

            InitializeCollectionWithRandonNumbers(_collection, Size);

            _sorter = (ISortStrategy)AlgorithmComboBox.SelectedItem;
            _sorter.ReportProgress += UpdateProgress;

            StartSorting();
        }

        private void InitializeCollectionWithRandonNumbers(IList<int> collection, int size)
        {
            collection.Clear();
            Random random = new();
            for (int i = 0; i < size; i++)
            {
                int newNumber = random.Next(size);
                if (collection.Contains(newNumber))
                {
                    IList<int> tmp = _exceptCollection.Except(collection).ToList();
                    newNumber = tmp[random.Next(tmp.Count - 1)];
                }
                collection.Add(newNumber);
            }
        }

        void UpdateProgress(IList<int> list)
        {
            VisualizeCollection(list);
            Thread.Sleep(_speed);
        }

        private void VisualizeCollection(IList<int> list)
        {
            if (list != null && list.Count > 0)
            {
                canvas.DispatcherQueue.TryEnqueue(() =>
                {
                    canvas.Children.Clear();

                    for (int i = 0; i < list.Count; i++)
                    {
                        int element = list[i];
                        Rectangle rectangle = new();
                        rectangle.Width = rectangle.Height = ElementSize;
                        rectangle.Fill = new SolidColorBrush(Colors.Gray);
                        canvas.Children.Add(rectangle);

                        Canvas.SetLeft(rectangle, list.IndexOf(element) * ElementSize + PaddingElement);
                        Canvas.SetTop(rectangle, element * ElementSize + PaddingElement);
                    }
                });
            }
        }

        private async void StartSorting()
        {
            myInfoBar.IsOpen = true;
            myInfoBar.Message = AppResourceManager.GetInstance.GetString("InforBar.Message.ActionInProgress");
            myInfoBar.Severity = InfoBarSeverity.Warning;
            myProgressRing.IsActive = true;
            DateTime dtStart = DateTime.Now;

            _cancellationTokenSource = new();
            try
            {
                await Task.Run(() =>
                {
                    _sorter.SortCancellationToken = _cancellationTokenSource.Token;
                    _sorter.Sort(_collection);

                    this.DispatcherQueue.TryEnqueue(() =>
                    {
                        TimeSpan time = DateTime.Now.Subtract(dtStart);
                        myInfoBar.Message = $"{AppResourceManager.GetInstance.GetString("InforBar.Message.ActionCompleted")} ({time.TotalSeconds.ToString("F")}s)";
                        myInfoBar.Severity = InfoBarSeverity.Success;
                        myProgressRing.IsActive = false;
                    });

                });
            }
            catch (OperationCanceledException)
            {
                myInfoBar.Message = AppResourceManager.GetInstance.GetString("InforBar.Message.ActionCancelled"); ;
                myInfoBar.Severity = InfoBarSeverity.Error;
                myProgressRing.IsActive = false;
            }
        }

        private void OnStopClick(object sender, RoutedEventArgs e)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                canvas.Children.Clear();
                _collection.Clear();
            }
        }
    }
}
