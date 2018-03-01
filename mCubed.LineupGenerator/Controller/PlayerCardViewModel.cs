using System.ComponentModel;
using System.Threading;
using mCubed.LineupGenerator.Services;

namespace mCubed.LineupGenerator.Controller
{
	public class PlayerCardViewModel : INotifyPropertyChanged
	{
		#region Properties

		#region Contest

		public Contest Contest { get; private set; }

		#endregion

		#region IsExpanded

		private bool _isExpanded;
		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				if (_isExpanded != value)
				{
					_isExpanded = value;
					RaisePropertyChanged("IsExpanded");
					RetrievePlayerCard();
				}
			}
		}

		#endregion

		#region IsRetrievingPlayerCard

		private bool _isRetrievingPlayerCard;
		public bool IsRetrievingPlayerCard
		{
			get { return _isRetrievingPlayerCard; }
			private set
			{
				if (_isRetrievingPlayerCard != value)
				{
					_isRetrievingPlayerCard = value;
					RaisePropertyChanged("IsRetrievingPlayerCard");
				}
			}
		}

		#endregion

		#region Player

		public Player Player { get; private set; }

		#endregion

		#region PlayerCard

		private PlayerCard _playerCard;
		public PlayerCard PlayerCard
		{
			get { return _playerCard; }
			private set
			{
				if (_playerCard != value)
				{
					_playerCard = value;
					RaisePropertyChanged("PlayerCard");
				}
			}
		}

		#endregion

		#endregion

		#region Constructors

		public PlayerCardViewModel(Contest contest, Player player)
		{
			Contest = contest;
			Player = player;
		}

		#endregion

		#region Methods

		private void RetrievePlayerCard()
		{
			if (_playerCard == null && IsExpanded && !IsRetrievingPlayerCard)
			{
				IsRetrievingPlayerCard = true;
				ThreadPool.QueueUserWorkItem(q =>
				{
					var service = new LineupGeneratorService();
					PlayerCard = service.GetPlayerCard(Contest.ID, Player.ID);
					IsRetrievingPlayerCard = false;
				});
			}
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
