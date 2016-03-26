using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Twice.Models.Configuration;
using WPFLocalizeExtension.Engine;

namespace Twice.ViewModels.Settings
{
	internal class GeneralSettings : ViewModelBaseEx, IGeneralSettings
	{
		public GeneralSettings( IConfig currentConfig )
		{
			AvailableLanguages = new List<CultureInfo>( LocalizeDictionary.Instance.MergedAvailableCultures );
		}

		public void SaveTo( IConfig config )
		{
		}

		public ICollection<CultureInfo> AvailableLanguages { get; }

		public CultureInfo SelectedLanguage
		{
			[DebuggerStepThrough]
			get
			{
				return _SelectedLanguage;
			}
			set
			{
				if( _SelectedLanguage == value )
				{
					return;
				}

				_SelectedLanguage = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private CultureInfo _SelectedLanguage;
	}
}