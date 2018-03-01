using System;
using System.ComponentModel;
using System.Linq;
using mCubed.LineupGenerator.Model;
using mCubed.LineupGenerator.Services;

namespace mCubed.LineupGenerator.Controller
{
	public class PlayerViewModel : INotifyPropertyChanged
	{
		#region Properties

		#region Contest

		public Contest Contest { get; private set; }

		#endregion

		#region Game

		public Game Game { get; private set; }

		#endregion

		#region HasPlayerStats

		public bool HasPlayerStats
		{
			get { return PlayerStats.Length > 0; }
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

		#region IsHomeTeam

		public bool IsHomeTeam { get; private set; }

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

		#region PlayerStats

		private PlayerStatsGroup[] _playerStats;
		public PlayerStatsGroup[] PlayerStats
		{
			get
			{
				if (_playerStats == null)
				{
					_playerStats = new PlayerStatsGroup[]
					{
						CreatePlayerStatsGroup("Projections", p => p.ProjectedPoints),
						CreatePlayerStatsGroup("Projected Floor", p => p.ProjectedFloor),
						CreatePlayerStatsGroup("Projected Ceiling", p => p.ProjectedCeiling),
						CreatePlayerStatsGroup("Recent Points", p => p.RecentAveragePoints),
						CreatePlayerStatsGroup("Season Points", p => p.SeasonAveragePoints)
					}.Where(p => p != null).ToArray();
				}
				return _playerStats;
			}
		}

		#endregion

		#endregion

		#region Constructors

		public PlayerViewModel(Contest contest, Game game, bool isHomeTeam, Player player)
		{
			Contest = contest;
			Game = game;
			IsHomeTeam = isHomeTeam;
			Player = player;
		}

		#endregion

		#region Methods

		private PlayerStatsGroup CreatePlayerStatsGroup(string groupName, Func<PlayerStats, double?> valueFunc)
		{
			var stats = Player.Stats.Where(p => valueFunc(p) != null).ToArray();
			if (stats.Length == 0)
			{
				return null;
			}
			return new PlayerStatsGroup
			{
				GroupName = groupName,
				Stats = stats.Select(s => new PlayerStatsGroupItem
				{
					Source = s.Source,
					Value = valueFunc(s).Value
				})
			};
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
