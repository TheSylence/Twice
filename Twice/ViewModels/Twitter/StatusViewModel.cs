using LinqToTwitter;

namespace Twice.ViewModels.Twitter
{
	internal class StatusViewModel
	{
		public StatusViewModel( Status model )
		{
			Model = model;
		}

		public Status Model { get; }
	}
}