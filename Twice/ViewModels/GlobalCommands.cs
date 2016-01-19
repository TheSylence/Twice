using System;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Ninject;
using Twice.Services;
using Twice.Services.ViewServices;

namespace Twice.ViewModels
{
	/// <summary>Class containing commands that are available everywhere in the application.</summary>
	internal class GlobalCommands
	{
		static GlobalCommands()
		{
			Kernel = App.Kernel;
		}

		private static bool CanExecuteOpenUrlCommand( Uri args )
		{
			return args != null && args.IsAbsoluteUri;
		}

		private static async void ExecuteOpenProfileCommand( ulong args )
		{
			//ServiceRepository.Default.Show<ProfileService>( args );

			await Services.Show<IViewProfileService, object>( args );
		}

		private static void ExecuteOpenUrlCommand( Uri args )
		{
			Process.Start( args.AbsoluteUri );
		}

		public static ICommand OpenProfileCommand => _OpenProfileCommand ?? ( _OpenProfileCommand = new RelayCommand<ulong>( ExecuteOpenProfileCommand ) );

		/// <summary>Command to open an URL in the default webbrowser.</summary>
		public static ICommand OpenUrlCommand => _OpenUrlCommand ??
														( _OpenUrlCommand = new RelayCommand<Uri>( ExecuteOpenUrlCommand, CanExecuteOpenUrlCommand ) );

		private static IServiceRepository Services => Kernel.Get<IServiceRepository>();
		private static readonly IKernel Kernel;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private static RelayCommand<ulong> _OpenProfileCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private static RelayCommand<Uri> _OpenUrlCommand;
	}
}