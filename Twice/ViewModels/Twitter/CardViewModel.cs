using GalaSoft.MvvmLight;
using Twice.Models.Media;

namespace Twice.ViewModels.Twitter
{
	internal class CardViewModel : ObservableObject
	{
		public CardViewModel( TwitterCard card )
		{
			Card = card;
		}

		public TwitterCard Card { get; }
		public string DisplayUrl => Card?.Url?.Host ?? string.Empty;
	}
}