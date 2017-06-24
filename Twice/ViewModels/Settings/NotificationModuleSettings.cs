using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Twice.Models.Configuration;

namespace Twice.ViewModels.Settings
{
	internal abstract class NotificationModuleSettings : ObservableObject, ISettingsSection
	{
		protected virtual void ExecutePreviewCommand()
		{
		}

		public Task OnLoad( object data )
		{
			return Task.CompletedTask;
		}

		public abstract void SaveTo( IConfig config );

		public bool Enabled { get; set; }

		public ICommand PreviewCommand => _PreviewCommand ?? ( _PreviewCommand = new RelayCommand( ExecutePreviewCommand ) );
		public abstract string Title { get; }

		private RelayCommand _PreviewCommand;
	}
}