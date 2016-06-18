using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Twice.Models.Configuration;

namespace Twice.ViewModels.Settings
{
	internal abstract class NotificationModuleSettings : ObservableObject, ISettingsSection
	{
		public abstract void SaveTo( IConfig config );

		protected virtual void ExecutePreviewCommand()
		{
		}

		public bool Enabled
		{
			[DebuggerStepThrough] get { return _Enabled; }
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

		public ICommand PreviewCommand => _PreviewCommand ?? ( _PreviewCommand = new RelayCommand( ExecutePreviewCommand ) );
		public abstract string Title { get; }

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _Enabled;

		private RelayCommand _PreviewCommand;
	}
}