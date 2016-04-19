using GalaSoft.MvvmLight;
using Twice.Models.Configuration;

namespace Twice.ViewModels.Settings
{
	internal abstract class NotificationModuleSettings : ObservableObject, ISettingsSection
	{
		public abstract void SaveTo( IConfig config );

		public bool Enabled
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return _Enabled; }
			set
			{
				if( _Enabled == value )
				{
					return;
				}

				_Enabled = value;
				RaisePropertyChanged();
			}
		}

		public abstract string Title { get; }

		[System.Diagnostics.DebuggerBrowsable( System.Diagnostics.DebuggerBrowsableState.Never )]
		private bool _Enabled;
	}
}