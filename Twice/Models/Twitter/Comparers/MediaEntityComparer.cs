using System;
using System.Collections.Generic;
using LinqToTwitter;

namespace Twice.Models.Twitter.Comparers
{
	internal class MediaEntityComparer : IEqualityComparer<MediaEntity>
	{
		/// <summary>
		///     Determines whether the specified objects are equal.
		/// </summary>
		/// <returns> true if the specified objects are equal; otherwise, false. </returns>
		/// <param name="x"> The first object of type <see cref="MediaEntity" /> to compare. </param>
		/// <param name="y"> The second object of type <see cref="MediaEntity" /> to compare. </param>
		public bool Equals( MediaEntity x, MediaEntity y )
		{
			if( x == null || y == null )
			{
				return false;
			}

			if( x.ID.Equals( y.ID ) )
			{
				return true;
			}

			return x.MediaUrlHttps?.Equals( y.MediaUrlHttps ) == true;
		}

		/// <summary>
		///     Returns a hash code for the specified object.
		/// </summary>
		/// <returns> A hash code for the specified object. </returns>
		/// <param name="obj">
		///     The <see cref="T:System.Object" /> for which a hash code is to be returned.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		///     The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is null.
		/// </exception>
		public int GetHashCode( MediaEntity obj )
		{
			if( obj == null )
			{
				throw new ArgumentNullException();
			}

			return obj.ID.GetHashCode();
		}
	}
}