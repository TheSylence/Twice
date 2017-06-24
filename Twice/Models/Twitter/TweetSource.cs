using System;
using System.Text.RegularExpressions;

namespace Twice.Models.Twitter
{
	/// <summary>
	///     Represents an application that was used to write a tweet.
	/// </summary>
	internal class TweetSource
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="TweetSource" /> class.
		/// </summary>
		/// <param name="source"> The source tweeter told for the status. </param>
		internal TweetSource( string source )
		{
			if( source == null )
			{
				throw new ArgumentNullException( nameof(source) );
			}

			if( source.Length == 0 || source.Equals( "web", StringComparison.OrdinalIgnoreCase ) )
			{
				Name = "web";
				Url = new Uri( "https://twitter.com" );
			}
			else
			{
				Match match = Pattern.Match( source );

				if( !match.Success )
				{
					throw new ArgumentException( @"Invalid source", nameof(source) );
				}

				Name = match.Groups[2].Value;
				Url = new Uri( match.Groups[1].Value, UriKind.Absolute );
			}
		}

		private static readonly Regex Pattern = new Regex( "<a.*href=\"(.*?)\".*>(.*?)</a>" );

		/// <summary>
		///     Name of the Application.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///     Website of the application.
		/// </summary>
		public Uri Url { get; private set; }
	}
}