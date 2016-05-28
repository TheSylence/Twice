using GalaSoft.MvvmLight.Messaging;
using System;
using System.Diagnostics.CodeAnalysis;
using Twice.Messages;

namespace Twice.Utilities.Ui
{
	[ExcludeFromCodeCoverage]
	internal class WaitOperation : IDisposable
	{
		public WaitOperation( IMessenger messenger = null )
		{
			MessengerInstance = messenger ?? Messenger.Default;
			MessengerInstance.Send( new WaitMessage( true ) );
		}

		public void Dispose()
		{
			MessengerInstance.Send( new WaitMessage( false ) );

			GC.SuppressFinalize( this );
		}

		private readonly IMessenger MessengerInstance;
	}
}