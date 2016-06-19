using System;
using GalaSoft.MvvmLight;
using LinqToTwitter;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Entities;

namespace Twice.ViewModels.Twitter
{
	internal class UserViewModel : ObservableObject
	{
		public UserViewModel( User user )
		{
			Model = user;

			ProfileImageUrlHttps = user.ProfileImageUrlHttps;
			ProfileImageUrlHttpsOrig = user.ProfileImageUrlHttps?.Replace( "_normal", "" );
			ProfileImageUrlHttpsMini = user.ProfileImageUrlHttps?.Replace( "_normal", "_mini" );
			ProfileImageUrlHttpsBig = user.ProfileImageUrlHttps?.Replace( "_normal", "_bigger" );

			ScreenName = Constants.Twitter.Mention + Model.GetScreenName();
			if( Uri.IsWellFormedUriString( user.Url, UriKind.Absolute ) )
			{
				Url = new Uri( user.Url );
			}
			else
			{
				Url = user.GetUserUrl();
			}
		}

		public UserViewModel( UserEx user )
			: this( (User)user )
		{
			if( Uri.IsWellFormedUriString( user.UrlDisplay, UriKind.Absolute ) )
			{
				Url = new Uri( user.UrlDisplay );
			}
		}

		public User Model { get; }
		public UserEx ModelEx => Model as UserEx;
		public string ProfileImageUrlHttps { get; }
		public string ProfileImageUrlHttpsBig { get; }
		public string ProfileImageUrlHttpsMini { get; }
		public string ProfileImageUrlHttpsOrig { get; }
		public string ScreenName { get; }
		public Uri Url { get; }
		public ulong UserId => Model.GetUserId();
	}
}