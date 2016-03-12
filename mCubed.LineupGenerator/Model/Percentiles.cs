using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace mCubed.LineupGenerator.Model
{
	public class Percentiles : INotifyPropertyChanged
	{
		#region Data Members

		private readonly ConcurrentDictionary<int, LinkedList<double>> _values = new ConcurrentDictionary<int, LinkedList<double>>();

		#endregion

		#region Properties

		#region Value

		private double? _value;
		public double? Value
		{
			get { return _value; }
			private set
			{
				if (_value != value)
				{
					_value = value;
					RaisePropertyChanged("Value");
				}
			}
		}

		#endregion

		#endregion

		#region Methods

		public void AddPercentile(int id, double value)
		{
			var values = _values.GetOrAdd(id, (key) => new LinkedList<double>());
			values.AddLast(value);
		}

		public void Calculate(int id)
		{
			var values = _values.GetOrAdd(id, (key) => new LinkedList<double>());
			if (values.Count > 0)
			{
				Value = (values.Sum() / (double)values.Count) * 100d;
			}
			else
			{
				Value = null;
			}
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged(string property)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(property));
			}
		}

		#endregion
	}
}
