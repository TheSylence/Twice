using Twice.Models.Media;

namespace Twice.ViewModels.Twitter
{
	internal class CardViewModel
	{
		public CardViewModel( TwitterCard card )
		{
			Card = card;
		}

		public TwitterCard Card { get; }
		public string DisplayUrl => Card.Url.Host;
	}
}