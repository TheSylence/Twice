using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Ninject;
using Twice.Models.Cache;
using Twice.Models.Configuration;
using Twice.Resources;
using Twice.Utilities.Ui;

namespace Twice.ViewModels.Settings
{
	internal class GeneralSettings : ViewModelBaseEx, IGeneralSettings
	{
		public GeneralSettings( IConfig currentConfig, ILanguageProvider languageProvider )
		{
			var langs = languageProvider.AvailableLanguages.ToList();
			for( int i = 0; i < langs.Count; ++i )
			{
				if( langs[i].Equals( CultureInfo.InvariantCulture ) )
				{
					langs[i] = CultureInfo.CreateSpecificCulture( "en-US" );
				}
			}

			var neutrals = langs.Where( l => l.IsNeutralCulture ).ToArray();
			foreach( var neutral in neutrals.Where( neutral => langs.Any( l => neutral.Equals( l.Parent ) ) ) )
			{
				langs.Remove( neutral );
			}

			AvailableLanguages = langs.Distinct().OrderBy( l => l.NativeName ).ToList();

			AvailableFetchCounts = new[] {20, 50, 100, 200};

			var english = AvailableLanguages.FirstOrDefault( IsEnglish );

			RealtimeStreaming = currentConfig.General.RealtimeStreaming;
			SelectedLanguage = AvailableLanguages.SingleOrDefault( l => l.Name == currentConfig.General.Language ) ?? english;
			CheckForUpdates = currentConfig.General.CheckForUpdates;
			IncludePrereleaseUpdates = currentConfig.General.IncludePrereleaseUpdates;
			TweetFetchCount = currentConfig.General.TweetFetchCount;
			FilterSensitiveTweets = currentConfig.General.FilterSensitiveTweets;
			SendVersionStats = currentConfig.General.SendVersionStats;
		}

		private async void ExecuteClearCacheCommand()
		{
			await Cache.Clear();

			Notifier.DisplayMessage( Strings.CacheCleared, NotificationType.Success );
		}

		private static bool IsEnglish( CultureInfo lang )
		{
			var english = CultureInfo.CreateSpecificCulture( "en" );

			return lang.ThreeLetterISOLanguageName.Equals( english.ThreeLetterISOLanguageName );
		}

		public ICollection<int> AvailableFetchCounts { get; }

		public ICollection<CultureInfo> AvailableLanguages { get; }

		public bool CheckForUpdates { get; set; }

		public ICommand ClearCacheCommand => _ClearCacheCommand ?? ( _ClearCacheCommand = new RelayCommand( ExecuteClearCacheCommand ) );

		public bool FilterSensitiveTweets { get; set; }

		public bool IncludePrereleaseUpdates { get; set; }

		public bool RealtimeStreaming { get; set; }

		public CultureInfo SelectedLanguage
		{
			[DebuggerStepThrough] get { return _SelectedLanguage; }
			set
			{
				if( _SelectedLanguage?.Name == value?.Name )
				{
					return;
				}

				_SelectedLanguage = value;
				RaisePropertyChanged();
			}
		}

		public bool SendVersionStats { get; set; }

		public int TweetFetchCount { get; set; }

		public Task OnLoad( object data )
		{
			return Task.CompletedTask;
		}

		public void SaveTo( IConfig config )
		{
			config.General.Language = SelectedLanguage.Name;
			config.General.RealtimeStreaming = RealtimeStreaming;
			config.General.CheckForUpdates = CheckForUpdates;
			config.General.IncludePrereleaseUpdates = IncludePrereleaseUpdates;
			config.General.TweetFetchCount = Math.Min( 200, Math.Max( 20, TweetFetchCount ) );
			config.General.FilterSensitiveTweets = FilterSensitiveTweets;
			config.General.SendVersionStats = SendVersionStats;
		}

		[Inject]

		// ReSharper disable once MemberCanBePrivate.Global
		public ICache Cache { get; set; }

		[Inject]

		// ReSharper disable once MemberCanBePrivate.Global
		public INotifier Notifier { get; set; }

		private RelayCommand _ClearCacheCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private CultureInfo _SelectedLanguage;
	}
}