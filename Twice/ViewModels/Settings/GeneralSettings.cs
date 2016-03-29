using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Twice.Models.Configuration;
using WPFLocalizeExtension.Engine;

namespace Twice.ViewModels.Settings
{
	internal class GeneralSettings : ViewModelBaseEx, IGeneralSettings
	{
		public GeneralSettings( IConfig currentConfig )
		{
			AvailableLanguages = new List<CultureInfo>( LocalizeDictionary.Instance.MergedAvailableCultures );
			AvailableLanguages.Remove( CultureInfo.InvariantCulture );

			RealtimeStreaming = currentConfig.General.RealtimeStreaming;
			SelectedLanguage = AvailableLanguages.SingleOrDefault( l => l.Name == currentConfig.General.Language ) ?? AvailableLanguages.First();
			CheckForUpdates = currentConfig.General.CheckForUpdates;
			IncludePrereleaseUpdates = currentConfig.General.IncludePrereleaseUpdates;
		}

		public void SaveTo( IConfig config )
		{
			config.General.Language = SelectedLanguage.Name;
			config.General.RealtimeStreaming = RealtimeStreaming;
			config.General.CheckForUpdates = CheckForUpdates;
			config.General.IncludePrereleaseUpdates = IncludePrereleaseUpdates;
		}

		public ICollection<CultureInfo> AvailableLanguages { get; }

		public bool CheckForUpdates
		{
			[DebuggerStepThrough] get { return _CheckForUpdates; }
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

		public bool IncludePrereleaseUpdates
		{
			[DebuggerStepThrough] get { return _IncludePrereleaseUpdates; }
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

		public bool RealtimeStreaming
		{
			[DebuggerStepThrough] get { return _RealtimeStreaming; }
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

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _CheckForUpdates;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IncludePrereleaseUpdates;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _RealtimeStreaming;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private CultureInfo _SelectedLanguage;
	}
}