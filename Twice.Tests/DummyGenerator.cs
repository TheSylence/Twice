using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Twice.Tests
{
	static class DummyGenerator
	{
		internal static Status CreateDummyStatus( User user = null )
		{
			user = user ?? CreateDummyUser();

			return new Status
			{
				User = user
			};
		}

		internal static User CreateDummyUser()
		{
			return new User
			{
				ProfileImageUrl = "http://example.com/image_normal.png",
				ProfileImageUrlHttps = "https://example.com/image_normal.png"
			};
		}
	}
}
