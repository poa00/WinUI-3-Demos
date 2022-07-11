using System.Collections.Generic;

namespace VisualSortingItems.SortingAlgorithm
{
    public class BubbleSort : SortAlgorithmBase
    {
		public override string Caption
        {
            get { return "Bubble Sort"; }
        }

        public override void Sort(IList<int> input)
        {
            _collection = new List<int>(input);
            OnReportProgress();

            for (int i = _collection.Count - 1; i >= 0; i--)
            {
                for (int j = 1; j <= i; j++)
                {
                    if (_collection[j - 1].CompareTo(_collection[j]) > 0)
                        SwapIndex(j - 1, j);
                    SortCancellationToken.ThrowIfCancellationRequested();
                }
                OnReportProgress();

                SortCancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}
