using System;
using System.Collections.Generic;

namespace VisualSortingItems.SortingAlgorithm
{
    public class SelectionSort : SortAlgorithmBase
    {
		public override string Caption
        {
            get { return "Selection Sort"; }
        }

		public override void Sort(IList<int> input)
        {
            _collection = new List<int>(input);
            OnReportProgress();

            for (int i = 0; i < _collection.Count -1; i++)
            {
                int minimum = i;
                for(int j = i + 1; j < _collection.Count; j++)
                {
                    if (_collection[j].CompareTo(_collection[minimum])  < 0)
                        minimum = j;
                    SortCancellationToken.ThrowIfCancellationRequested();
                }

            	SwapIndex(minimum, i);
                OnReportProgress();
            }
        }
    }
}
