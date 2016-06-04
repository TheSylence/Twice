using LinqToTwitter;
using System;
using System.Collections.Generic;

namespace Twice.Models.Twitter.Comparers
{
	internal class MediaEntityComparer : IEqualityComparer<MediaEntity>
	{
		/// <summary>Determines whether the specified objects are equal.</summary>
		/// <returns>true if the specified objects are equal; otherwise, false.</returns>
		/// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
		/// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
		public bool Equals( MediaEntity x, MediaEntity y )
		{
			return x.ID.Equals( y.ID );
		}

		/// <summary>Returns a hash code for the specified object.</summary>
		/// <returns>A hash code for the specified object.</returns>
		/// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///     The type of <paramref name="obj" /> is a reference type and
		///     <paramref name="obj" /> is null.
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