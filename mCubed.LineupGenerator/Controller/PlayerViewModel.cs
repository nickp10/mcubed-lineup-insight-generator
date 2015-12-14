using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using mCubed.Services.Core.Model;

namespace mCubed.LineupGenerator.Controller
{
	public class PlayerViewModel : INotifyPropertyChanged
	{
		#region Properties

		#region HasProjections

		public bool HasProjections
		{
			get { return Player.Stats.Any(s => s.ProjectedPoints != null); }
		}

		#endregion

		#region IncludeInLineups

		private bool _includeInLineups;
		public bool IncludeInLineups
		{
			get { return _includeInLineups; }
			set
			{
				if (_includeInLineups != value)
				{
					_includeInLineups = value;
					RaisePropertyChanged("IncludeInLineups");
				}
			}
		}

		#endregion

		#region Player

		public Player Player { get; private set; }

		#endregion

		#region Projections

		public IEnumerable<PlayerStats> Projections
		{
			get { return Player.Stats.Where(s => s.ProjectedPoints != null); }
		}

		#endregion

		#endregion

		#region Constructors

		public PlayerViewModel(Player player)
		{
			Player = player;
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChanged(string property)
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
