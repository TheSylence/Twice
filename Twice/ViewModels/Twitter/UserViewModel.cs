using GalaSoft.MvvmLight;
using LinqToTwitter;
using Twice.Models.Twitter;

namespace Twice.ViewModels.Twitter
{
	internal class UserViewModel : ObservableObject
	{
		public UserViewModel( User user )
		{
			Model = user;

			ProfileImageUrlHttps = user.ProfileImageUrlHttps;
			ProfileImageUrlHttpsOrig = user.ProfileImageUrlHttps.Replace( "_normal", "" );
			ProfileImageUrlHttpsMini = user.ProfileImageUrlHttps.Replace( "_normal", "_mini" );
			ProfileImageUrlHttpsBig = user.ProfileImageUrlHttps.Replace( "_normal", "_bigger" );
		}

		public User Model { get; }
		public string ProfileImageUrlHttps { get; }
		public string ProfileImageUrlHttpsBig { get; }
		public string ProfileImageUrlHttpsMini { get; }
		public string ProfileImageUrlHttpsOrig { get; }
		public ulong UserId => Model.GetUserId();
	}
}