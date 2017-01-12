using GalaSoft.MvvmLight.CommandWpf;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
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

			AvailableFetchCounts = new[] { 20, 50, 100, 200 };

			var english = AvailableLanguages.FirstOrDefault( IsEnglish );

			RealtimeStreaming = currentConfig.General.RealtimeStreaming;
			SelectedLanguage = AvailableLanguages.SingleOrDefault( l => l.Name == currentConfig.General.Language ) ?? english;
			CheckForUpdates = currentConfig.General.CheckForUpdates;
			IncludePrereleaseUpdates = currentConfig.General.IncludePrereleaseUpdates;
			TweetFetchCount = currentConfig.General.TweetFetchCount;
			FilterSensitiveTweets = currentConfig.General.FilterSensitiveTweets;
			SendVersionStats = currentConfig.General.SendVersionStats;
		}

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

		private static bool IsEnglish( CultureInfo lang )
		{
			var english = CultureInfo.CreateSpecificCulture( "en" );

			return lang.ThreeLetterISOLanguageName.Equals( english.ThreeLetterISOLanguageName );
		}

		private async void ExecuteClearCacheCommand()
		{
			await Cache.Clear();

			Notifier.DisplayMessage( Strings.CacheCleared, NotificationType.Success );
		}

		public ICollection<int> AvailableFetchCounts { get; }

		public ICollection<CultureInfo> AvailableLanguages { get; }

		[Inject]

		// ReSharper disable once MemberCanBePrivate.Global
		public ICache Cache { get; set; }

		public bool CheckForUpdates
		{
			[DebuggerStepThrough]
			get { return _CheckForUpdates; }
			set
			{
				if( _CheckForUpdates == value )
				{
					return;
				}

				_CheckForUpdates = value;
				RaisePropertyChanged();
			}
		}

		public ICommand ClearCacheCommand => _ClearCacheCommand ?? ( _ClearCacheCommand = new RelayCommand( ExecuteClearCacheCommand ) );

		public bool FilterSensitiveTweets
		{
			[DebuggerStepThrough]
			get { return _FilterSensitiveTweets; }
			set
			{
				if( _FilterSensitiveTweets == value )
				{
					return;
				}

				_FilterSensitiveTweets = value;
				RaisePropertyChanged();
			}
		}

		public bool IncludePrereleaseUpdates
		{
			[DebuggerStepThrough]
			get { return _IncludePrereleaseUpdates; }
			set
			{
				if( _IncludePrereleaseUpdates == value )
				{
					return;
				}

				_IncludePrereleaseUpdates = value;
				RaisePropertyChanged();
			}
		}

		[Inject]

		// ReSharper disable once MemberCanBePrivate.Global
		public INotifier Notifier { get; set; }

		public bool RealtimeStreaming
		{
			[DebuggerStepThrough]
			get { return _RealtimeStreaming; }
			set
			{
				if( _RealtimeStreaming == value )
				{
					return;
				}

				_RealtimeStreaming = value;
				RaisePropertyChanged();
			}
		}

		public CultureInfo SelectedLanguage
		{
			[DebuggerStepThrough]
			get { return _SelectedLanguage; }
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

		public bool SendVersionStats
		{
			[DebuggerStepThrough]
			get { return _SendVersionStats; }
			set
			{
				if( _SendVersionStats == value )
				{
					return;
				}

				_SendVersionStats = value;
				RaisePropertyChanged( nameof( SendVersionStats ) );
			}
		}

		public int TweetFetchCount
		{
			[DebuggerStepThrough]
			get { return _TweetFetchCount; }
			set
			{
				if( _TweetFetchCount == value )
				{
					return;
				}

				_TweetFetchCount = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _CheckForUpdates;

		private RelayCommand _ClearCacheCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _FilterSensitiveTweets;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IncludePrereleaseUpdates;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _RealtimeStreaming;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private CultureInfo _SelectedLanguage;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _SendVersionStats;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private int _TweetFetchCount;
	}
}