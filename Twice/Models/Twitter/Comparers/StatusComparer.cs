using System;
using System.Collections.Generic;
using LinqToTwitter;

namespace Twice.Models.Twitter.Comparers
{
	internal class StatusComparer : IEqualityComparer<Status>
	{
		public bool Equals( Status x, Status y )
		{
			return x.GetStatusId() == y.GetStatusId();
		}

		public int GetHashCode( Status obj )
		{
			if( obj == null )
			{
				throw new ArgumentNullException();
			}

			return obj.GetStatusId().GetHashCode();
		}
	}
}