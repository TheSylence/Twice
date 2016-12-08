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
using Twice.Models.Media;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Entities;
using Twice.Resources;
using Twice.ViewModels;

namespace Twice.Converters
{
	/// <summary>
	///     Converts a Status to an InlineCollection.
	/// </summary>
	internal class StatusHighlighter : IValueConverter, IEqualityComparer<EntityBase>
	{
		public StatusHighlighter( IConfig config, IMediaExtractorRepository mediaExtractorRepo = null )
		{
			OverrideConfig = config;
			OverrideExtractorRepo = mediaExtractorRepo;
		}

		public StatusHighlighter()
		{
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
			if( value == null )
			{
				return null;
			}

			var tweet = value as IHighlightable;
			if( tweet == null )
			{
				if( Debugger.IsAttached )
				{
					Debugger.Break();
				}
				throw new ArgumentException( @"Value is not an IHighlightable", nameof( value ) );
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

		public bool Equals( EntityBase x, EntityBase y )
		{
			return x.Start == y.Start && x.End == y.End;
		}

		public int GetHashCode( EntityBase obj )
		{
			int hash = 397;
			unchecked
			{
				hash = hash * 23 + obj.Start;
				hash = hash * 23 + obj.End;
			}
			return hash;
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
			return urlEntity != null
				? urlEntity.Url
				: string.Empty;
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
			link.CommandParameter = Constants.Twitter.HashTag + entity.Tag;
			link.Command = GlobalCommands.StartSearchCommand;

			return link;
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
			if( entity.Id == 0 )
			{
				link.CommandParameter = entity.ScreenName;
			}
			else
			{
				link.CommandParameter = entity.Id;
			}
			link.ContextMenu = CreateUserContextMenu( entity );

			return link;
		}

		private static string PrepareText( string text )
		{
			text = WebUtility.HtmlDecode( text );

			return text;
		}

		static IEnumerable<EntityBase> RemoveExtendedTweetUrl( IEnumerable<UrlEntity> urls )
		{
			foreach( var url in urls )
			{
				if( !TwitterHelper.IsExtendedTweetUrl( url.ExpandedUrl ) )
					yield return url;
			}
		}

		private IEnumerable<EntityBase> ExtractEntities( IHighlightable item )
		{
			if( item.Text == null )
			{
				return Enumerable.Empty<EntityBase>();
			}

			var hashTags = item.Entities?.HashTagEntities ?? Enumerable.Empty<EntityBase>();
			var medias = item.Entities?.MediaEntities ?? Enumerable.Empty<EntityBase>();
			var urls = item.Entities?.UrlEntities ?? Enumerable.Empty<UrlEntity>();
			var mentions = item.Entities?.UserMentionEntities ?? Enumerable.Empty<EntityBase>();

			var entities = hashTags.Distinct( this )
				.Concat( medias.Distinct( this ) )
				.Concat( RemoveExtendedTweetUrl(urls).Distinct( this ) )
				.Concat( mentions.Distinct( this ) );

			//IEnumerable<EntityBase> entities = item.Entities?.HashTagEntities ?? Enumerable.Empty<EntityBase>();
			//entities = entities.Concat( item.Entities?.MediaEntities ?? Enumerable.Empty<EntityBase>() );
			//entities = entities.Concat( item.Entities?.UrlEntities ?? Enumerable.Empty<EntityBase>() );
			//entities = entities.Concat( item.Entities?.UserMentionEntities ?? Enumerable.Empty<EntityBase>() );

			var tweetText = TwitterHelper.NormalizeText( item.Text );

			var allEntities = entities.ToArray();
			foreach( var entity in allEntities )
			{
				int length = entity.End - entity.Start - 1;
				List<string> extractedTextVersions = new List<string>
				{
					ExtractEntityText( entity )
				};

				if( entity is UserMentionEntity )
				{
					extractedTextVersions.Add( AlternativeAtSign + extractedTextVersions[0].Substring( 1 ) );
				}
				else if( entity is HashTagEntity )
				{
					extractedTextVersions.Add( AlternativeHashtagSign + extractedTextVersions[0].Substring( 1 ) );
				}

				var actualText = tweetText.Substring( entity.Start, length );

				bool found = false;
				foreach( var extractedText in extractedTextVersions )
				{
					// When the tweet contains emojis, the twitter API returns wrong indices for entities
					if( extractedText != actualText )
					{
						var newIndex = tweetText.IndexOf( extractedText, entity.Start, StringComparison.Ordinal );
						if( newIndex == -1 )
						{
							newIndex = tweetText.IndexOf( extractedText, entity.Start, StringComparison.OrdinalIgnoreCase );
						}
						if( newIndex == -1 )
						{
							continue;
						}

						found = true;
						entity.Start = newIndex;
						entity.End = entity.Start + length + 1;
					}
				}

				Debug.Assert( found );
			}

			var ordered = allEntities.OrderBy( e => e.Start ).ToList();

			for( int i = ordered.Count - 1; i > 0; --i )
			{
				var current = ordered[i];
				var next = ordered[i - 1];

				if( current.Start == next.Start && current.End == next.End )
				{
					ordered.RemoveAt( i );
				}
			}

			return ordered;
		}

		/// <summary>
		///     Generates a collection of inlines from a tweet.
		/// </summary>
		/// <param name="item">The tweet to generate inlines from.</param>
		/// <returns>The generated inlines.</returns>
		private IEnumerable<Inline> GenerateInlines( IHighlightable item )
		{
			var allEntities = ExtractEntities( item ).ToArray();

			if( allEntities.Any() )
			{
				int lastEnd = 0;

				foreach( EntityBase entity in allEntities )
				{
					if( entity.Start > lastEnd )
					{
						string text = item.Text.Substring( lastEnd, entity.Start - lastEnd );
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
						}
						else
						{
							UrlEntity urlEntity = entity as UrlEntity;
							var url = urlEntity.ExpandedUrl;

							if( !TwitterHelper.IsTweetUrl( url ) &&
							    ( !Config.Visual.InlineMedia || !ExtractorRepo.CanExtract( url ) ) )
							{
								yield return GenerateLink( urlEntity );
							}
						}
					}
					else if( entity is UserMentionEntity )
					{
						yield return GenerateMention( (UserMentionEntity)entity );
					}

					lastEnd = entity.End;
				}

				if( lastEnd < item.Text.Length )
				{
					yield return new Run( PrepareText( item.Text.Substring( lastEnd ) ) );
				}
			}
			else
			{
				yield return new Run( PrepareText( item.Text ) );
			}
		}

		private static IConfig Config => OverrideConfig ?? App.Kernel.Get<IConfig>();
		private static IMediaExtractorRepository ExtractorRepo => OverrideExtractorRepo ?? App.Kernel.Get<IMediaExtractorRepository>();
		private const string AlternativeAtSign = "\uFF20";
		private const string AlternativeHashtagSign = "\uFF03";
		private static IConfig OverrideConfig;
		private static IMediaExtractorRepository OverrideExtractorRepo;
	}
}