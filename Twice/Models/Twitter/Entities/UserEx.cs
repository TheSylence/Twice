using System.Diagnostics;
using LinqToTwitter;
using LitJson;

namespace Twice.Models.Twitter.Entities
{
	/// <summary>
	/// Extended user entity that includes some data that Linq2Twitter doesn't offer
	/// </summary>
	internal class UserEx : User
	{
		public UserEx()
		{
		}

		public UserEx( JsonData user )
			: base( user )
		{
		}
	}
}