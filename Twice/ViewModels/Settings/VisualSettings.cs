using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using LinqToTwitter;
using Newtonsoft.Json;
using Resourcer;
using Twice.Models.Configuration;
using Twice.Resources;
using Twice.Utilities.Ui;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Settings
{
	internal class VisualSettings : ViewModelBaseEx, IVisualSettings
	{
		public VisualSettings( IConfig currentConfig, IColorProvider colorProvider, ILanguageProvider languageProvider )
		{
			ColorProvider = colorProvider;

			AvailableAccentColors = new List<ColorItem>( ColorProvider.AvailableAccentColors.Select( a =>
				new ColorItem
				{
					Name = a.Name,
					ColorBrush = new SolidColorBrush( a.ExemplarHue.Color )
				}
			) );

			AvailablePrimaryColors = new List<ColorItem>( ColorProvider.AvailablePrimaryColors.Select( a =>
				new ColorItem
				{
					Name = a.Name,
					ColorBrush = new SolidColorBrush( a.ExemplarHue.Color )
				}
			) );

			SelectedPrimaryColor = AvailablePrimaryColors.FirstOrDefault( c => c.Name == currentConfig.Visual.PrimaryColor );
			SelectedAccentColor = AvailableAccentColors.FirstOrDefault( c => c.Name == currentConfig.Visual.AccentColor );

			AvailableFontSizes = new List<FontSizeItem>();
			foreach( var kvp in new Dictionary<int, string>
			{
				{10, "Tiny"},
				{13, "Small"},
				{16, "Normal"},
				{18, "Large"},
				{20, "Huge"}
			} )
			{
				string name = Strings.ResourceManager.GetString( $"FontSize_{kvp.Value}", languageProvider.CurrentCulture );
				AvailableFontSizes.Add( new FontSizeItem
				{
					DisplayName = name,
					Size = kvp.Key
				} );
			}

			UseDarkTheme = currentConfig.Visual.UseDarkTheme;
			SelectedFontSize = AvailableFontSizes.FirstOrDefault( f => f.Size == currentConfig.Visual.FontSize );
			SelectedHashtagColor = AvailableAccentColors.FirstOrDefault( c => c.Name == currentConfig.Visual.HashtagColor );
			SelectedLinkColor = AvailableAccentColors.FirstOrDefault( c => c.Name == currentConfig.Visual.LinkColor );
			SelectedMentionColor = AvailableAccentColors.FirstOrDefault( c => c.Name == currentConfig.Visual.MentionColor );
			InlineMedias = currentConfig.Visual.InlineMedia;

			PreviewStatuses = new List<StatusViewModel>();
		}

		public Task OnLoad( object data )
		{
			var json = Resource.AsString( "Twice.Resources.Data.PreviewStatuses.json" );

			var statuses = JsonConvert.DeserializeObject<List<Status>>( json )
				.Select( s => new StatusViewModel( s, ContextList.Contexts.FirstOrDefault(), Configuration, ViewServiceRepository ) ).ToList();

			statuses[1].QuotedTweet = statuses[0];

			PreviewStatuses.AddRange( statuses );
			return Task.CompletedTask;
		}

		public void SaveTo( IConfig config )
		{
			config.Visual.UseDarkTheme = UseDarkTheme;
			config.Visual.AccentColor = SelectedAccentColor.Name;
			config.Visual.PrimaryColor = SelectedPrimaryColor.Name;

			config.Visual.FontSize = SelectedFontSize.Size;
			config.Visual.HashtagColor = SelectedHashtagColor.Name;
			config.Visual.LinkColor = SelectedLinkColor.Name;
			config.Visual.MentionColor = SelectedMentionColor.Name;

			config.Visual.InlineMedia = InlineMedias;

			ColorProvider.SetHashtagColor( config.Visual.HashtagColor );
			ColorProvider.SetLinkColor( config.Visual.LinkColor );
			ColorProvider.SetMentionColor( config.Visual.MentionColor );

			ColorProvider.SetFontSize( config.Visual.FontSize );

			ColorProvider.SetDarkTheme( config.Visual.UseDarkTheme );
			ColorProvider.SetAccentColor( config.Visual.AccentColor );
			ColorProvider.SetPrimaryColor( config.Visual.PrimaryColor );
		}

		public ICollection<ColorItem> AvailableAccentColors { get; }
		public ICollection<FontSizeItem> AvailableFontSizes { get; }
		public ICollection<ColorItem> AvailablePrimaryColors { get; }

		public bool InlineMedias { get; set; }

		public ICollection<StatusViewModel> PreviewStatuses { get; }

		public ColorItem SelectedAccentColor { get; set; }

		public FontSizeItem SelectedFontSize { get; set; }

		public ColorItem SelectedHashtagColor { get; set; }

		public ColorItem SelectedLinkColor { get; set; }

		public ColorItem SelectedMentionColor { get; set; }

		public ColorItem SelectedPrimaryColor { get; set; }

		public bool UseDarkTheme { get; set; }

		private readonly IColorProvider ColorProvider;
	}
}