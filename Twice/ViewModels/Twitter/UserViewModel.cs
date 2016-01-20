using GalaSoft.MvvmLight;
using LinqToTwitter;

namespace Twice.ViewModels.Twitter
{
	internal class UserViewModel : ObservableObject
	{
		public UserViewModel( User user )
		{
			Model = user;
		}

		public User Model { get; }
	}
}