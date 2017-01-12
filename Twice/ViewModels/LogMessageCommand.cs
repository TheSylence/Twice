using Anotar.NLog;
using System;
using System.Windows.Input;

namespace Twice.ViewModels
{
	internal class LogMessageCommand : ICommand
	{
		public LogMessageCommand( string message, NLog.LogLevel level )
		{
			Message = message;
			Level = level;
		}

		public bool CanExecute( object parameter )
		{
			return true;
		}

		public void Execute( object parameter )
		{
			if( Level == NLog.LogLevel.Warn )
			{
				LogTo.Warn( Message );
			}
			else
			{
				LogTo.Debug( Message );
			}
		}

#pragma warning disable 67 // Event is never used

		public event EventHandler CanExecuteChanged;

#pragma warning restore

		private readonly NLog.LogLevel Level;

		private readonly string Message;
	}
}