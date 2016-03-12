using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using mCubed.LineupGenerator.Model;
using mCubed.Services.Core.Model;

namespace mCubed.LineupGenerator.Controller
{
	public class PlayerViewModel : INotifyPropertyChanged
	{
		#region Properties

		#region Contest

		public Contest Contest { get; private set; }

		#endregion

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

		#region Likability

		private Percentiles _likability;
		public Percentiles Likability
		{
			get
			{
				if (_likability == null)
				{
					_likability = new Percentiles();
				}
				return _likability;
			}
		}

		#endregion

		#region Player

		public Player Player { get; private set; }

		#endregion

		#region PlayerCard

		private PlayerCardViewModel _playerCard;
		public PlayerCardViewModel PlayerCard
		{
			get
			{
				if (_playerCard == null)
				{
					_playerCard = new PlayerCardViewModel(Contest, Player);
				}
				return _playerCard;
			}
		}

		#endregion

		#region Projections

		public IEnumerable<PlayerStats> Projections
		{
			get { return Player.Stats.Where(s => s.ProjectedPoints != null); }
		}

		#endregion

		#endregion

		#region Constructors

		public PlayerViewModel(Contest contest, Player player)
		{
			Contest = contest;
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
