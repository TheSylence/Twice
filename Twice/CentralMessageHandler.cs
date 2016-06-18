using System;
using GalaSoft.MvvmLight.Messaging;
using Ninject;
using Twice.Messages;
using Twice.Models.Configuration;

namespace Twice
{
	/// <summary>
	///     Central message handler to keep stuff out of view models
	/// </summary>
	internal class CentralMessageHandler : IDisposable
	{
		public CentralMessageHandler( IKernel kernel )
		{
			Kernel = kernel;
			Messenger = kernel.Get<IMessenger>();

			Messenger.Register<ColumnLockMessage>( this, OnColumnLock );
		}

		private void OnColumnLock( ColumnLockMessage msg )
		{
			var config = Kernel.Get<IConfig>();
			config.General.ColumnsLocked = msg.Locked;
			config.Save();
		}

		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or
		///     resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Messenger.Unregister( this );
		}

		private readonly IKernel Kernel;
		private readonly IMessenger Messenger;
	}
}