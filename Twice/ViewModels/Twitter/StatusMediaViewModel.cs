using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Twice.ViewModels.Twitter
{
	internal class StatusMediaViewModel : ObservableObject
	{
		public StatusMediaViewModel( Uri url )
		{
			Url = url;
		}

		public event EventHandler OpenRequested;

		private void ExecuteOpenImageCommand()
		{
			OpenRequested?.Invoke( this, EventArgs.Empty );
		}

		public ICommand OpenImageCommand => _OpenImageCommand ?? ( _OpenImageCommand = new RelayCommand(
			ExecuteOpenImageCommand ) );

		public Uri Url { get; }

		private RelayCommand _OpenImageCommand;
	}
}