using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using LinqToTwitter;
using Ninject;
using Twice.Models.Configuration;
using Twice.Models.Twitter;
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
			IEnumerable<EntityBase> entities = tweet.Entities.HashTagEntities;
			entities = entities.Concat( tweet.Entities.MediaEntities );
			entities = entities.Concat( tweet.Entities.UrlEntities );
			entities = entities.Concat( tweet.Entities.UserMentionEntities );

			var allEntities = entities.OrderBy( e => e.Start ).ToArray();
			List<Inline> mediaPreviews = new List<Inline>();

			var statusText = TwitterHelper.NormalizeText( tweet.Text );
			var emojis = TwitterHelper.FindEmojis( statusText );

			if( allEntities.Any() )
			{
				int lastEnd = 0;
				int lastEntityEnd = 0;

				foreach( EntityBase entity in allEntities )
				{
					if( entity.Start > lastEntityEnd )
					{
						string text = statusText.Substring( lastEntityEnd, entity.Start - lastEntityEnd + 1 );
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
					lastEntityEnd = lastEntityEnd + emojis.Count( e => e < entity.End );
				}

				if( lastEnd < statusText.Length )
				{
					yield return new Run( PrepareText( statusText.Substring( lastEnd ) ) );
				}

				foreach( Inline preview in mediaPreviews )
				{
					yield return preview;
				}
			}
			else
			{
				yield return new Run( PrepareText( statusText ) );
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