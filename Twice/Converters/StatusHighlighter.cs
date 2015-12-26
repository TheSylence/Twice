using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using LinqToTwitter;
using Twice.ViewModels;

namespace Twice.Converters
{
	/// <summary>Converts a Status to an InlineCollection.</summary>
	internal class StatusHighlighter : IValueConverter
	{
		/// <summary>Converts a value.</summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			Status tweet = value as Status;
			if( tweet == null )
			{
				throw new ArgumentException( "Value is not an ITweet object", nameof( value ) );
			}

			IEnumerable<Inline> inlines = GenerateInlines( tweet ).ToArray();

			return inlines;
		}

		/// <summary>Converts a value.</summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotSupportedException();
		}

		/// <summary>Generates an inline from a hashtag entity.</summary>
		/// <param name="entity">The entity to generate the inline from.</param>
		/// <returns>The generated inline.</returns>
		private static Inline GenerateHashTag( HashTagEntity entity )
		{
			Hyperlink link = new Hyperlink();
			link.Inlines.Add( Constants.Twitter.HashTag + entity.Tag );
			//link.Foreground = Application.Current.FindResource<Brush>( "HashtagBrush" );
			link.SetResourceReference( TextElement.ForegroundProperty, "HashtagBrush" );
			link.TextDecorations = null;

			return link;
		}

		/// <summary>Generates a collection of inlines from a tweet.</summary>
		/// <param name="tweet">The tweet to generate inlines from.</param>
		/// <returns>The generated inlines.</returns>
		private static IEnumerable<Inline> GenerateInlines( Status tweet )
		{
			IEnumerable<EntityBase> entities = tweet.Entities.HashTagEntities;
			entities = entities.Concat( tweet.Entities.MediaEntities );
			entities = entities.Concat( tweet.Entities.UrlEntities );
			entities = entities.Concat( tweet.Entities.UserMentionEntities );

			entities = entities.OrderBy( e => e.Start );
			List<Inline> mediaPreviews = new List<Inline>();

			if( entities.Any() )
			{
				int lastEnd = 0;

				foreach( EntityBase entity in entities )
				{
					if( entity.Start > lastEnd )
					{
						string text = tweet.Text.Substring( lastEnd, entity.Start - lastEnd );
						yield return new Run( PrepareText( text ) );
					}

					if( entity is HashTagEntity )
					{
						yield return GenerateHashTag( (HashTagEntity)entity );
					}
					else if( entity is UrlEntity )
					{
						if( entity is MediaEntity )
						{
							MediaEntity mediaEnttiy = entity as MediaEntity;
							yield return GenerateMedia( mediaEnttiy );

							//IMediaService service = PluginManager.Instance.MediaServices.FirstOrDefault( s => s.CanExpand( mediaEnttiy.MediaUrlHttps ) );
							//if( service != null )
							//{
							//	mediaPreviews.Add( service.ExpandMedia( mediaEnttiy.MediaUrlHttps ) );
							//}
						}
						else
						{
							UrlEntity urlEntity = entity as UrlEntity;
							yield return GenerateLink( urlEntity );

							//IMediaService service = PluginManager.Instance.MediaServices.FirstOrDefault( s => s.CanExpand( urlEntity.ExpandedUrl ) );
							//if( service != null )
							//{
							//	mediaPreviews.Add( service.ExpandMedia( urlEntity.ExpandedUrl ) );
							//}
						}
					}
					else if( entity is UserMentionEntity )
					{
						yield return GenerateMention( (UserMentionEntity)entity );
					}

					lastEnd = entity.End;
				}

				if( lastEnd < tweet.Text.Length )
				{
					yield return new Run( PrepareText( tweet.Text.Substring( lastEnd ) ) );
				}

				foreach( Inline preview in mediaPreviews )
				{
					yield return preview;
				}
			}
			else
			{
				yield return new Run( PrepareText( tweet.Text ) );
			}
		}

		private static string PrepareText( string text )
		{
			text = WebUtility.HtmlDecode( text );

			return text;
		}


		/// <summary>Generates an inline from a link entity.</summary>
		/// <param name="entity">The entity to generate the inline from.</param>
		/// <returns>The generated inline.</returns>
		private static Inline GenerateLink( UrlEntity entity )
		{
			Hyperlink link = new Hyperlink();
			link.Inlines.Add( entity.DisplayUrl );
			link.CommandParameter = new Uri( entity.ExpandedUrl );
			link.Command = GlobalCommands.OpenUrlCommand;
			link.ToolTip = entity.ExpandedUrl;
			//link.Foreground = Application.Current.FindResource<Brush>( "LinkBrush" );
			link.SetResourceReference( TextElement.ForegroundProperty, "LinkBrush" );

			return link;
		}

		/// <summary>Generates an inline from a media entity.</summary>
		/// <param name="entity">The entity to generate the inline from.</param>
		/// <returns>The generated inline.</returns>
		private static Inline GenerateMedia( MediaEntity entity )
		{
			Hyperlink link = new Hyperlink();
			link.Inlines.Add( entity.MediaUrlHttps );
			link.NavigateUri = new Uri( entity.MediaUrlHttps );
			link.CommandParameter = entity.MediaUrlHttps;
			link.Command = GlobalCommands.OpenUrlCommand;
			link.ToolTip = entity.MediaUrlHttps;
			//link.Foreground = Application.Current.FindResource<Brush>( "LinkBrush" );
			link.SetResourceReference( TextElement.ForegroundProperty, "LinkBrush" );

			return link;
		}

		/// <summary>Generates an inline from a mention entity.</summary>
		/// <param name="entity">The entity to generate the inline from.</param>
		/// <returns>The generated inline.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Documents.InlineCollection.Add(System.String)", Justification = "Character is always the same" )]
		private static Inline GenerateMention( UserMentionEntity entity )
		{
			Hyperlink link = new Hyperlink();
			link.Inlines.Add( Constants.Twitter.Mention + entity.ScreenName );
			//link.Foreground = Application.Current.FindResource<Brush>( "MentionBrush" );
			link.SetResourceReference( TextElement.ForegroundProperty, "MentionBrush" );
			link.TextDecorations = null;
			link.Command = GlobalCommands.OpenProfileCommand;
			link.CommandParameter = entity.Id;

			return link;
		}
	}
}
