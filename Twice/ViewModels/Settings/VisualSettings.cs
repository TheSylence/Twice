using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro;
using MaterialDesignColors;
using Twice.Models.Configuration;
using Twice.Resources;
using WPFLocalizeExtension.Engine;

namespace Twice.ViewModels.Settings
{
	internal class VisualSettings : ViewModelBaseEx, IVisualSettings
	{
		public VisualSettings( IConfig currentConfig )
		{
			var swatches = new SwatchesProvider().Swatches;

			AvailableThemes = new List<ColorItem>( ThemeManager.AppThemes.Select( t =>
				new ColorItem
				{
					Name = t.Name,
					ColorBrush = t.Resources["WhiteColorBrush"] as Brush,
					BorderBrush = t.Resources["BlackColorBrush"] as Brush
				}
				) );
			
			AvailableColors = new List<ColorItem>( swatches.Select( a =>
				new ColorItem
				{
					Name = a.Name,
					ColorBrush = new SolidColorBrush( a.PrimaryHues.First().Color )
				}
				) );

			//var currentStyle = ThemeManager.DetectAppStyle( Application.Current );
			//SelectedTheme = AvailableThemes.First( t => t.Name == currentStyle.Item1.Name );
			//SelectedColor = AvailableColors.First( c => c.Name == currentStyle.Item2.Name );

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
				string name = Strings.ResourceManager.GetString( $"FontSize_{kvp.Value}", LocalizeDictionary.CurrentCulture );
				AvailableFontSizes.Add( new FontSizeItem
				{
					DisplayName = name,
					Size = kvp.Key
				} );
			}

			SelectedFontSize = AvailableFontSizes.FirstOrDefault( f => f.Size == currentConfig.Visual.FontSize );
			SelectedHashtagColor = AvailableColors.FirstOrDefault( c => c.Name == currentConfig.Visual.HashtagColor );
			SelectedLinkColor = AvailableColors.FirstOrDefault( c => c.Name == currentConfig.Visual.LinkColor );
			SelectedMentionColor = AvailableColors.FirstOrDefault( c => c.Name == currentConfig.Visual.MentionColor );
			InlineMedias = currentConfig.Visual.InlineMedia;
			UseStars = currentConfig.Visual.UseStars;

			AvailableLanguages = new List<CultureInfo>( LocalizeDictionary.Instance.MergedAvailableCultures );
		}

		public void SaveTo( IConfig config )
		{
			config.Visual.UseDarkTheme = UseDarkTheme;
			config.Visual.AccentColor = SelectedColor.Name;

			config.Visual.FontSize = SelectedFontSize.Size;
			config.Visual.HashtagColor = SelectedHashtagColor.Name;
			config.Visual.LinkColor = SelectedLinkColor.Name;
			config.Visual.MentionColor = SelectedMentionColor.Name;

			config.Visual.InlineMedia = InlineMedias;
			config.Visual.UseStars = UseStars;
			
			Application.Current.Resources["HashtagBrush"] = ThemeManager.GetAccent( config.Visual.HashtagColor ).Resources["HighlightBrush"];
			Application.Current.Resources["LinkBrush"] = ThemeManager.GetAccent( config.Visual.LinkColor ).Resources["HighlightBrush"];
			Application.Current.Resources["MentionBrush"] = ThemeManager.GetAccent( config.Visual.MentionColor ).Resources["HighlightBrush"];
			Application.Current.Resources["GlobalFontSize"] = (double)config.Visual.FontSize;
		}

		public ICollection<ColorItem> AvailableColors { get; }

		public ICollection<FontSizeItem> AvailableFontSizes { get; }

		public ICollection<CultureInfo> AvailableLanguages { get; }

		public ICollection<ColorItem> AvailableThemes { get; }

		public bool InlineMedias
		{
			[DebuggerStepThrough] get { return _InlineMedias; }
			set
			{
				if( _InlineMedias == value )
				{
					return;
				}

				_InlineMedias = value;
				RaisePropertyChanged();
			}
		}

		public ColorItem SelectedColor
		{
			[DebuggerStepThrough] get { return _SelectedColor; }
			set
			{
				if( _SelectedColor == value )
				{
					return;
				}

				_SelectedColor = value;
				RaisePropertyChanged();
			}
		}

		public FontSizeItem SelectedFontSize
		{
			[DebuggerStepThrough] get { return _SelectedFontSize; }
			set
			{
				if( _SelectedFontSize == value )
				{
					return;
				}

				_SelectedFontSize = value;
				RaisePropertyChanged();
			}
		}

		public ColorItem SelectedHashtagColor
		{
			[DebuggerStepThrough] get { return _SelectedHashtagColor; }
			set
			{
				if( _SelectedHashtagColor == value )
				{
					return;
				}

				_SelectedHashtagColor = value;
				RaisePropertyChanged();
			}
		}

		public ColorItem SelectedLinkColor
		{
			[DebuggerStepThrough] get { return _SelectedLinkColor; }
			set
			{
				if( _SelectedLinkColor == value )
				{
					return;
				}

				_SelectedLinkColor = value;
				RaisePropertyChanged();
			}
		}

		public ColorItem SelectedMentionColor
		{
			[DebuggerStepThrough] get { return _SelectedMentionColor; }
			set
			{
				if( _SelectedMentionColor == value )
				{
					return;
				}

				_SelectedMentionColor = value;
				RaisePropertyChanged();
			}
		}

		public bool UseDarkTheme
		{
			[DebuggerStepThrough] get { return _UseDarkTheme; }
			set
			{
				if( _UseDarkTheme == value )
				{
					return;
				}

				_UseDarkTheme = value;
				RaisePropertyChanged();
			}
		}

		public bool UseStars
		{
			[DebuggerStepThrough] get { return _UseStars; }
			set
			{
				if( _UseStars == value )
				{
					return;
				}

				_UseStars = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _InlineMedias;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private ColorItem _SelectedColor;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private FontSizeItem _SelectedFontSize;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private ColorItem _SelectedHashtagColor;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private ColorItem _SelectedLinkColor;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private ColorItem _SelectedMentionColor;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _UseDarkTheme;
		
		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _UseStars;
	}
}