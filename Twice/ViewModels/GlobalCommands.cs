using System;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Ninject;
using Twice.Models.Configuration;
using Twice.Services.Views;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels
{
	/// <summary>
	///     Class containing commands that are available everywhere in the application.
	/// </summary>
	internal class GlobalCommands
	{
		static GlobalCommands()
		{
			Kernel = App.Kernel;
		}

		private static RelayCommand<StatusViewModel> _OpenStatusCommand;

		public static ICommand OpenStatusCommand => _OpenStatusCommand ?? ( _OpenStatusCommand = new RelayCommand<StatusViewModel>(
			ExecuteOpenStatusCommand ) );

		private static async void ExecuteOpenStatusCommand( StatusViewModel vm )
		{
			await ViewServices.ViewStatus( vm );
		}

		private static bool CanExecuteOpenUrlCommand( Uri args )
		{
			return args != null && args.IsAbsoluteUri;
		}

		private static void ExecuteCreateMuteCommand( string value )
		{
			var config = Kernel.Get<IConfig>();
			config.Mute.Entries.Add( new MuteEntry
			{
				Filter = value
			} );

			config.Save();
		}

		private static async void ExecuteOpenImageCommand( Uri imageUrl )
		{
			await ViewServices.ViewImage( new[] {imageUrl}, imageUrl );
		}

		private static async void ExecuteOpenProfileCommand( ulong args )
		{
			await ViewServices.ViewProfile( args );
		}

		private static void ExecuteOpenUrlCommand( Uri args )
		{
			Process.Start( args.AbsoluteUri );
		}

		public static ICommand CreateMuteCommand
			=> _CreateMuteCommand ?? ( _CreateMuteCommand = new RelayCommand<string>( ExecuteCreateMuteCommand ) );

		public static ICommand OpenImageCommand => _OpenImageCommand ?? ( _OpenImageCommand = new RelayCommand<Uri>(
			ExecuteOpenImageCommand ) );

		public static ICommand OpenProfileCommand
			=> _OpenProfileCommand ?? ( _OpenProfileCommand = new RelayCommand<ulong>( ExecuteOpenProfileCommand ) );

		/// <summary>
		///     Command to open an URL in the default webbrowser.
		/// </summary>
		public static ICommand OpenUrlCommand => _OpenUrlCommand ??
		                                         ( _OpenUrlCommand = new RelayCommand<Uri>( ExecuteOpenUrlCommand, CanExecuteOpenUrlCommand ) );

		private static IViewServiceRepository ViewServices => Kernel.Get<IViewServiceRepository>();
		private static readonly IKernel Kernel;
		private static RelayCommand<string> _CreateMuteCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private static RelayCommand<Uri> _OpenImageCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private static RelayCommand<ulong> _OpenProfileCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private static RelayCommand<Uri> _OpenUrlCommand;
	}
}