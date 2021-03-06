﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using Fody;
using GalaSoft.MvvmLight.CommandWpf;
using Ninject;
using Twice.Models.Configuration;
using Twice.ViewModels.Twitter;
using Twice.Views.Services;

namespace Twice.ViewModels
{
	/// <summary>
	///     Class containing commands that are available everywhere in the application.
	/// </summary>
	[ConfigureAwait( false )]
	[ExcludeFromCodeCoverage]
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

		private static void ExecuteCopyToClipboardCommand( string text )
		{
			Clipboard.SetText( text );
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

		private static async void ExecuteOpenMessageCommand( MessageViewModel message )
		{
			await ViewServices.ViewDirectMessage( message );
		}

		private static async void ExecuteOpenProfileCommand( object args )
		{
			if( args == null )
			{
				return;
			}

			string screenName = args.ToString();
			ulong userid;
			if( ulong.TryParse( screenName, out userid ) )
			{
				await ViewServices.ViewProfile( userid );
			}
			else
			{
				await ViewServices.ViewProfile( screenName );
			}
		}

		private static async void ExecuteOpenStatusCommand( StatusViewModel vm )
		{
			// TODO: If status is a retweet, display the retweeted status instead of the retweet
			await ViewServices.ViewStatus( vm );
		}

		private static void ExecuteOpenUrlCommand( Uri args )
		{
			Process.Start( args.AbsoluteUri );
		}

		private static async void ExecuteStartSearchCommand( string query )
		{
			await ViewServices.OpenSearch( query );
		}

		private static RelayCommand<string> _CopyToClipboardCommand;
		private static RelayCommand<string> _CreateMuteCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private static RelayCommand<Uri> _OpenImageCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private static RelayCommand<MessageViewModel> _OpenMessageCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private static RelayCommand<object> _OpenProfileCommand;

		private static RelayCommand<StatusViewModel> _OpenStatusCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private static RelayCommand<Uri> _OpenUrlCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private static RelayCommand<string> _StartSearchCommand;
		private static readonly IKernel Kernel;

		public static ICommand CopyToClipboardCommand
			=> _CopyToClipboardCommand ?? ( _CopyToClipboardCommand = new RelayCommand<string>( ExecuteCopyToClipboardCommand ) );

		public static ICommand CreateMuteCommand
			=> _CreateMuteCommand ?? ( _CreateMuteCommand = new RelayCommand<string>( ExecuteCreateMuteCommand ) );

		public static ICommand OpenImageCommand => _OpenImageCommand ?? ( _OpenImageCommand = new RelayCommand<Uri>(
			                                           ExecuteOpenImageCommand ) );

		public static ICommand OpenMessageCommand
			=> _OpenMessageCommand ?? ( _OpenMessageCommand = new RelayCommand<MessageViewModel>( ExecuteOpenMessageCommand ) );

		public static ICommand OpenProfileCommand
			=> _OpenProfileCommand ?? ( _OpenProfileCommand = new RelayCommand<object>( ExecuteOpenProfileCommand ) );

		public static ICommand OpenStatusCommand
			=> _OpenStatusCommand ?? ( _OpenStatusCommand = new RelayCommand<StatusViewModel>(
				   ExecuteOpenStatusCommand ) );

		/// <summary>
		///     Command to open an URL in the default webbrowser.
		/// </summary>
		public static ICommand OpenUrlCommand => _OpenUrlCommand ??
		                                         ( _OpenUrlCommand = new RelayCommand<Uri>( ExecuteOpenUrlCommand, CanExecuteOpenUrlCommand ) );

		public static ICommand StartSearchCommand
			=> _StartSearchCommand ?? ( _StartSearchCommand = new RelayCommand<string>( ExecuteStartSearchCommand ) );

		private static IViewServiceRepository ViewServices => Kernel.Get<IViewServiceRepository>();
	}
}