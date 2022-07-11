using System.Collections.Generic;

namespace VisualSortingItems.SortingAlgorithm
{
	public class InsertionSort : SortAlgorithmBase
	{
		public override string Caption
		{
			get { return "Insertion Sort"; }
		}

		public override void Sort(IList<int> input)
		{
			_collection = new List<int>(input);
			OnReportProgress();

			for (int i = 1; i < _collection.Count; i++)
			{
				int index = _collection[i];
				int j = i;

				while (j > 0 && _collection[j - 1].CompareTo(index) > 0)
				{
					_collection[j] = _collection[j - 1];
					j--;
				}
				_collection[j] = index;
				OnReportProgress();
				SortCancellationToken.ThrowIfCancellationRequested();
			}
		}
	}
}
