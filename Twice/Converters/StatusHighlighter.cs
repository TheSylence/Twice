using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using LinqToTwitter;
using Ninject;
using Twice.Models.Configuration;
using Twice.Resources;
using Twice.ViewModels;

namespace Twice.Converters
{
	/// <summary>
	///     Converts a Status to an InlineCollection.
	/// </summary>
	internal class StatusHighlighter : IValueConverter
	{
		public StatusHighlighter( IConfig config )
		{
			OverrideConfig = config;
		}

		public StatusHighlighter()
		{
		}

		private static ContextMenu CreateHashtagContextMenu( HashTagEntity entity )
		{
			var menu = new ContextMenu();

			menu.Items.Add( new MenuItem
			{
				Header = Strings.MuteHashtag,
				CommandParameter = $"#{entity.Tag}",
				Command = GlobalCommands.CreateMuteCommand
			} );

			return menu;
		}

		private static ContextMenu CreateLinkContextMenu( UrlEntity entity )
		{
			var menu = new ContextMenu();

			menu.Items.Add( new MenuItem
			{
				Header = Strings.CopyUrl
			} );

			return menu;
		}

		private static ContextMenu CreateUserContextMenu( UserMentionEntity entity )
		{
			var menu = new ContextMenu();

			menu.Items.Add( new MenuItem
			{
				Header = Strings.MuteUser,
				CommandParameter = $"@{entity.ScreenName}",
				Command = GlobalCommands.CreateMuteCommand
			} );

			menu.Items.Add( new Separator() );

			menu.Items.Add( new MenuItem
			{
				Header = Strings.Block
			} );

			menu.Items.Add( new MenuItem
			{
				Header = Strings.ReportSpam
			} );

			return menu;
		}

		private static IEnumerable<EntityBase> ExtractEntities( Status tweet )
		{
			IEnumerable<EntityBase> entities = tweet.Entities.HashTagEntities;
			entities = entities.Concat( tweet.Entities.MediaEntities );
			entities = entities.Concat( tweet.Entities.UrlEntities );
			entities = entities.Concat( tweet.Entities.UserMentionEntities );

			var allEntities = entities.ToArray();
			foreach( var entity in allEntities )
			{
				int length = entity.End - entity.Start - 1;
				var extractedText = ExtractEntityText( entity );
				var actualText = tweet.Text.Substring( entity.Start, length );

				// When the tweet contains emojis, the twitter API returns wrong indices for entities
				if( extractedText != actualText )
				{
					var newIndex = tweet.Text.IndexOf( extractedText, entity.Start, StringComparison.Ordinal );
					Debug.Assert( newIndex != -1 );

					entity.Start = newIndex;
					entity.End = entity.Start + length + 1;
				}
			}

			return allEntities.OrderBy( e => e.Start );
		}

		private static string ExtractEntityText( EntityBase entity )
		{
			var mentionEntity = entity as UserMentionEntity;
			if( mentionEntity != null )
			{
				return "@" + mentionEntity.ScreenName;
			}

			var tagEntity = entity as HashTagEntity;
			if( tagEntity != null )
			{
				return "#" + tagEntity.Tag;
			}

			var urlEntity = entity as UrlEntity;
			if( urlEntity != null )
			{
				return urlEntity.Url;
			}

			return string.Empty;
		}

		/// <summary>
		///     Generates an inline from a hashtag entity.
		/// </summary>
		/// <param name="entity">The entity to generate the inline from.</param>
		/// <returns>The generated inline.</returns>
		private static Inline GenerateHashTag( HashTagEntity entity )
		{
			Hyperlink link = new Hyperlink();
			link.Inlines.Add( Constants.Twitter.HashTag + entity.Tag );
			link.SetResourceReference( TextElement.ForegroundProperty, "HashtagBrush" );
			link.TextDecorations = null;
			link.ContextMenu = CreateHashtagContextMenu( entity );

			return link;
		}

		/// <summary>
		///     Generates a collection of inlines from a tweet.
		/// </summary>
		/// <param name="tweet">The tweet to generate inlines from.</param>
		/// <returns>The generated inlines.</returns>
		private static IEnumerable<Inline> GenerateInlines( Status tweet )
		{
			var allEntities = ExtractEntities( tweet ).ToArray();
			List<Inline> mediaPreviews = new List<Inline>();

			if( allEntities.Any() )
			{
				int lastEnd = 0;

				foreach( EntityBase entity in allEntities )
				{
					if( entity.Start > lastEnd )
					{
						string text = tweet.Text.Substring( lastEnd, entity.Start - lastEnd );
						yield return new Run( PrepareText( text ) );
					}

					var tagEntity = entity as HashTagEntity;
					if( tagEntity != null )
					{
						yield return GenerateHashTag( tagEntity );
					}
					else if( entity is UrlEntity )
					{
						if( entity is MediaEntity )
						{
							if( !Config.Visual.InlineMedia )
							{
								MediaEntity mediaEnttiy = entity as MediaEntity;
								yield return GenerateMedia( mediaEnttiy );
							}

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

		/// <summary>
		///     Generates an inline from a link entity.
		/// </summary>
		/// <param name="entity">The entity to generate the inline from.</param>
		/// <returns>The generated inline.</returns>
		private static Inline GenerateLink( UrlEntity entity )
		{
			Hyperlink link = new Hyperlink();
			link.Inlines.Add( entity.DisplayUrl );
			link.CommandParameter = new Uri( entity.ExpandedUrl );
			link.Command = GlobalCommands.OpenUrlCommand;
			link.ToolTip = entity.ExpandedUrl;
			link.SetResourceReference( TextElement.ForegroundProperty, "LinkBrush" );
			link.ContextMenu = CreateLinkContextMenu( entity );

			return link;
		}

		/// <summary>
		///     Generates an inline from a media entity.
		/// </summary>
		/// <param name="entity">The entity to generate the inline from.</param>
		/// <returns>The generated inline.</returns>
		private static Inline GenerateMedia( MediaEntity entity )
		{
			Hyperlink link = new Hyperlink();
			link.Inlines.Add( entity.DisplayUrl );
			link.NavigateUri = new Uri( entity.MediaUrlHttps );
			link.CommandParameter = entity.MediaUrlHttps;
			link.Command = GlobalCommands.OpenUrlCommand;
			link.ToolTip = entity.MediaUrlHttps;
			link.SetResourceReference( TextElement.ForegroundProperty, "LinkBrush" );

			return link;
		}

		/// <summary>
		///     Generates an inline from a mention entity.
		/// </summary>
		/// <param name="entity">The entity to generate the inline from.</param>
		/// <returns>The generated inline.</returns>
		private static Inline GenerateMention( UserMentionEntity entity )
		{
			Hyperlink link = new Hyperlink();
			link.Inlines.Add( Constants.Twitter.Mention + entity.ScreenName );
			link.SetResourceReference( TextElement.ForegroundProperty, "MentionBrush" );
			link.TextDecorations = null;
			link.Command = GlobalCommands.OpenProfileCommand;
			link.CommandParameter = entity.Id;
			link.ContextMenu = CreateUserContextMenu( entity );

			return link;
		}

		private static string PrepareText( string text )
		{
			text = WebUtility.HtmlDecode( text );

			return text;
		}

		/// <summary>
		///     Converts a value.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		///     A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			Status tweet = value as Status;
			if( tweet == null )
			{
				throw new ArgumentException( @"Value is not a status object", nameof( value ) );
			}

			return GenerateInlines( tweet ).ToArray();
		}

		/// <summary>
		///     Converts a value.
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		///     A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}

		private static IConfig OverrideConfig;

		private static IConfig Config => OverrideConfig ?? App.Kernel.Get<IConfig>();
	}
}